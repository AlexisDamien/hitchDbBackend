using System.Text.Json;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public TmdbService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Tmdb:ApiKey"]!;
        _baseUrl = config["Tmdb:BaseUrl"]!;
    }

    public async Task<object?> SearchMoviesAsync(string query, int page = 1)
    {
        var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }

    public async Task<object?> GetMovieAsync(int tmdbId)
    {
        var url = $"{_baseUrl}/movie/{tmdbId}?api_key={_apiKey}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }

    public async Task<object?> GetPopularMoviesAsync(int page = 1)
    {
        var url = $"{_baseUrl}/movie/popular?api_key={_apiKey}&page={page}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }

    public async Task<object?> GetTopRatedMoviesAsync(int page = 1)
    {
        var url = $"{_baseUrl}/movie/top_rated?api_key={_apiKey}&page={page}&language=fr-FR";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<object>(json);
    }
}