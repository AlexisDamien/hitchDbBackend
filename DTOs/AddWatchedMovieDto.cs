public class AddWatchedMovieDto
{
    public string ImdbId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public bool Liked { get; set; }
    public int? Rating { get; set; }
}