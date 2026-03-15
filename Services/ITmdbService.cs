public interface ITmdbService
{
    Task<object?> SearchMoviesAsync(string query, int page = 1);
    Task<object?> GetMovieAsync(int tmdbId);
    Task<object?> GetPopularMoviesAsync(int page = 1);
    Task<object?> GetTopRatedMoviesAsync(int page = 1);
}