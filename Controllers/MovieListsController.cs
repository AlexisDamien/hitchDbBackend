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
    private readonly ITmdbService _movies;

    public MovieListsController(AppDbContext db, ITmdbService movies)
    {
        _db = db;
        _movies = movies;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var lists = await _db.MovieLists
            .Where(l => l.UserId == GetUserId())
            .Include(l => l.Items)
            .ToListAsync();
        await Parallel.ForEachAsync(
            lists,
            async (list, _) => {
                await Parallel.ForEachAsync(
                    list.Items,
                    async (item, _) => {
                        item.Movie = await _movies.GetMovieAsync(item.MovieId);
                    }
                );
            }
        );
        return Ok(lists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieList>> GetById(int id)
    {
        var list = await _db.MovieLists
            .Include(i => i.User)
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == GetUserId());
        return list is null ? NotFound() : Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<MovieList>> Create([FromBody] CreateMovieListDto dto)
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
    public async Task<ActionResult<MovieList>> Update(int id, [FromBody] CreateMovieListDto dto)
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
    public async Task<ActionResult<MovieList>> Delete(int id)
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
    public async Task<ActionResult<MovieListItem>> AddMovie(int id, [FromBody] AddFavoriteMovieDto dto)
    {
        MovieList? list = await _db.MovieLists
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == GetUserId());
        if (list is null) return NotFound();

        int movieId = dto.MovieId;

        object? movie = await _movies.GetMovieAsync(movieId);
        if (movie is null) return NotFound();

        var exists = await _db.MovieListItems
            .AnyAsync(i => i.MovieListId == id && i.MovieId == movieId);
        if (exists) return Conflict("Film déjà dans la liste");

        var item = new MovieListItem
        {
            MovieListId = id,
            MovieId = movieId,
            Movie = movie,
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