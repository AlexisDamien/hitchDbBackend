using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WatchLaterMoviesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITmdbService _movies;

    public WatchLaterMoviesController(AppDbContext db, ITmdbService movies)
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
                w.MarkedForWatchLaterAt != null
            )
            .OrderByDescending(w => w.MarkedForWatchLaterAt)
            .ToListAsync();
        await Parallel.ForEachAsync(
            movies,
            async (movie, _) => {
                movie.Movie = await _movies.GetMovieAsync(movie.MovieId);
            }
        );
        return Ok(movies);
    }

    [HttpPost]
    public async Task<ActionResult<UserMovieRelation>> Add([FromBody] AddWatchForLaterMovieDto dto)
    {
        int userId = GetUserId();
        int movieId = dto.MovieId;

        object? movie = await _movies.GetMovieAsync(movieId);
        if (movie is null) return NotFound("Film non trouvé");

        UserMovieRelation? relation = _db.UserMovieRelations
            .Find(userId, movieId);

        if (relation is not null)
        {
            if (relation.MarkedForWatchLaterAt != null) return Conflict("Film déjà dans la watch later list");

            relation.MarkedForWatchLaterAt = DateTime.UtcNow;
            relation.Movie = movie;

            await _db.SaveChangesAsync();
            return Ok(relation);
        }

        relation = new UserMovieRelation
        {
            UserId = userId,
            MovieId = movieId,
            Movie = movie,
            MarkedForWatchLaterAt = DateTime.UtcNow,
        };
        _db.UserMovieRelations.Add(relation);

        await _db.SaveChangesAsync();
        return Ok(relation);
    }

    [HttpDelete("{movieId}")]
    public async Task<ActionResult<UserMovieRelation>> Delete(int movieId)
    {
        var movie = _db.UserMovieRelations
            .Find(GetUserId(), movieId);
        if (movie is null) return NotFound();
        if (movie.MarkedForWatchLaterAt == null) return Conflict("Film n'est pas dans la watch later list");

        movie.MarkedForWatchLaterAt = null;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}