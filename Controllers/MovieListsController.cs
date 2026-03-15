using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovieListsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MovieListsController(AppDbContext db) => _db = db;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var lists = await _db.MovieLists
            .Where(l => l.UserId == GetUserId())
            .Include(l => l.Items)
            .ToListAsync();
        return Ok(lists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var list = await _db.MovieLists
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == GetUserId());
        return list is null ? NotFound() : Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMovieListDto dto)
    {
        var list = new MovieList
        {
            UserId = GetUserId(),
            Name = dto.Name,
            Description = dto.Description
        };
        _db.MovieLists.Add(list);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMovieListDto dto)
    {
        var list = await _db.MovieLists
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == GetUserId());
        if (list is null) return NotFound();

        list.Name = dto.Name;
        list.Description = dto.Description;
        await _db.SaveChangesAsync();
        return Ok(list);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var list = await _db.MovieLists
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == GetUserId());
        if (list is null) return NotFound();

        _db.MovieLists.Remove(list);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // --- Gestion des films dans une liste ---

    [HttpPost("{id}/movies")]
    public async Task<IActionResult> AddMovie(int id, [FromBody] AddFavoriteMovieDto dto)
    {
        var list = await _db.MovieLists
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == GetUserId());
        if (list is null) return NotFound();

        var item = new MovieListItem
        {
            MovieListId = id,
            ImdbId = dto.ImdbId,
            Title = dto.Title,
            PosterUrl = dto.PosterUrl
        };
        _db.MovieListItems.Add(item);
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}/movies/{itemId}")]
    public async Task<IActionResult> RemoveMovie(int id, int itemId)
    {
        var item = await _db.MovieListItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.MovieListId == id);
        if (item is null) return NotFound();

        _db.MovieListItems.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}