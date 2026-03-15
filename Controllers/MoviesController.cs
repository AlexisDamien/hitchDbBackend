using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly ITmdbService _tmdb;

    public MoviesController(ITmdbService tmdb) => _tmdb = tmdb;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1)
    {
        if (string.IsNullOrWhiteSpace(query)) return BadRequest("Query requise");
        var result = await _tmdb.SearchMoviesAsync(query, page);
        return result is null ? StatusCode(503, "TMDB indisponible") : Ok(result);
    }

    [HttpGet("{tmdbId}")]
    public async Task<IActionResult> GetMovie(int tmdbId)
    {
        var result = await _tmdb.GetMovieAsync(tmdbId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("popular")]
    public async Task<IActionResult> GetPopular([FromQuery] int page = 1)
    {
        var result = await _tmdb.GetPopularMoviesAsync(page);
        return result is null ? StatusCode(503, "TMDB indisponible") : Ok(result);
    }

    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int page = 1)
    {
        var result = await _tmdb.GetTopRatedMoviesAsync(page);
        return result is null ? StatusCode(503, "TMDB indisponible") : Ok(result);
    }
}