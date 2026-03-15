public class MovieListItem
{
    public int Id { get; set; }
    public int MovieListId { get; set; }
    public MovieList MovieList { get; set; } = null!;

    public string ImdbId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}