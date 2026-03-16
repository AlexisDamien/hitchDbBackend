using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WatchedMoviesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITmdbService _movies;

    public WatchedMoviesController(AppDbContext db, ITmdbService movies)
    {
        _db = db;
        _movies = movies;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserMovieRelation>>> GetAll()
    {
        var movies = await _db.UserMovieRelations
            .Where(w =>
                w.UserId == GetUserId() &&
                w.WatchedAt != null
            )
            .OrderByDescending(w => w.WatchedAt)
            .ToListAsync();
        await Parallel.ForEachAsync(
            movies,
            async (watchedMovie, _) => watchedMovie.Movie = await _movies.GetMovieAsync(watchedMovie.MovieId)
        );
        return Ok(movies);
    }

    [HttpPost]
    public async Task<ActionResult<UserMovieRelation>> Add([FromBody] AddWatchedMovieDto dto)
    {
        int movieId = dto.MovieId;

        object? movie = await _movies.GetMovieAsync(movieId);
        if (movie is null) return NotFound("Film non trouvé");

        int userId = GetUserId();
        var relation = _db.UserMovieRelations
            .Find(userId, movieId);

        if (relation is not null)
        {
            if (relation.WatchedAt != null) return Conflict("Film déjà dans la liste des films regardés");

            relation.WatchedAt = DateTime.UtcNow;
            relation.Rating = dto.Rating ?? relation.Rating;

            await _db.SaveChangesAsync();
            return Ok(relation);
        }

        relation = new UserMovieRelation
        {
            UserId = userId,
            MovieId = movieId,
            Movie = movie,
            WatchedAt = DateTime.UtcNow,
            Rating = dto.Rating
        };
        _db.UserMovieRelations.Add(relation);
        await _db.SaveChangesAsync();
        return Ok(relation);
    }

    [HttpPut("{movieId}")]
    public async Task<ActionResult<UserMovieRelation>> Update(int movieId, [FromBody] UpdateWatchedMovieDto dto)
    {
        var relation = _db.UserMovieRelations
            .Find(GetUserId(), movieId);
        if (relation is null) return NotFound();

        relation.WatchedAt ??= DateTime.UtcNow;
        relation.Rating = dto.Rating ?? relation.Rating;

        await _db.SaveChangesAsync();
        return Ok(relation);
    }

    [HttpDelete("{movieId}")]
    public async Task<ActionResult<UserMovieRelation>> Delete(int movieId)
    {
        var relation = _db.UserMovieRelations
            .Find(GetUserId(), movieId);
        if (relation is null) return NotFound();
        if (relation.WatchedAt == null) return Conflict("Film n'est pas dans la liste des films regardés");

        relation.WatchedAt = null;
        relation.Rating = null;
        relation.Favorite = false;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}