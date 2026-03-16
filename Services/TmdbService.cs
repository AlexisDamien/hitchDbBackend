using System.Text.Json;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public TmdbService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Tmdb:ApiKey"] ?? throw new InvalidOperationException("Missing configuration key 'Tmdb:ApiKey'.");
    }

    public async Task<object?> SearchMoviesAsync(string query, int page = 1)
    {
        var url = $"search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }

    public async Task<object?> GetMovieAsync(int tmdbId)
    {
        var url = $"movie/{tmdbId}?api_key={_apiKey}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }

    public async Task<object?> GetPopularMoviesAsync(int page = 1)
    {
        var url = $"movie/popular?api_key={_apiKey}&page={page}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }

    public async Task<object?> GetTopRatedMoviesAsync(int page = 1)
    {
        var url = $"movie/top_rated?api_key={_apiKey}&page={page}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }
}