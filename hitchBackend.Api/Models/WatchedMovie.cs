public class WatchedMovie
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string ImdbId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;

    public bool Liked { get; set; }
    public int? Rating { get; set; }
    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}