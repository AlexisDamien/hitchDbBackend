using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteMoviesController : ControllerBase
{
    private readonly AppDbContext _db;

    public FavoriteMoviesController(AppDbContext db) => _db = db;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var favorites = await _db.FavoriteMovies
            .Where(f => f.UserId == GetUserId())
            .ToListAsync();
        return Ok(favorites);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _db.FavoriteMovies
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == GetUserId());
        return movie is null ? NotFound() : Ok(movie);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddFavoriteMovieDto dto)
    {
        var exists = await _db.FavoriteMovies
            .AnyAsync(f => f.ImdbId == dto.ImdbId && f.UserId == GetUserId());
        if (exists) return Conflict("Film déjà dans les favoris");

        var favorite = new FavoriteMovie
        {
            UserId = GetUserId(),
            ImdbId = dto.ImdbId,
            Title = dto.Title,
            PosterUrl = dto.PosterUrl
        };
        _db.FavoriteMovies.Add(favorite);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = favorite.Id }, favorite);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _db.FavoriteMovies
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == GetUserId());
        if (movie is null) return NotFound();

        _db.FavoriteMovies.Remove(movie);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}