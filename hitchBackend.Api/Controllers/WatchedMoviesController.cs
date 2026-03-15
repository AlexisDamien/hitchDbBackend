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

    public WatchedMoviesController(AppDbContext db) => _db = db;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _db.WatchedMovies
            .Where(w => w.UserId == GetUserId())
            .ToListAsync();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _db.WatchedMovies
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == GetUserId());
        return movie is null ? NotFound() : Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWatchedMovieDto dto)
    {
        var watched = new WatchedMovie
        {
            UserId = GetUserId(),
            ImdbId = dto.ImdbId,
            Title = dto.Title,
            PosterUrl = dto.PosterUrl,
            Liked = dto.Liked,
            Rating = dto.Rating
        };
        _db.WatchedMovies.Add(watched);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = watched.Id }, watched);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AddWatchedMovieDto dto)
    {
        var movie = await _db.WatchedMovies
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == GetUserId());
        if (movie is null) return NotFound();

        movie.Liked = dto.Liked;
        movie.Rating = dto.Rating;
        await _db.SaveChangesAsync();
        return Ok(movie);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _db.WatchedMovies
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == GetUserId());
        if (movie is null) return NotFound();

        _db.WatchedMovies.Remove(movie);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}