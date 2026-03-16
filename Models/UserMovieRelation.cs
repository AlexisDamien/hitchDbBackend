using System.ComponentModel.DataAnnotations.Schema;

public class UserMovieRelation
{
    public int UserId { get; set; }
    [NotMapped]
    public User? User { get; set; } = null;

    public int MovieId { get; set; }
    [NotMapped]
    public object? Movie { get; set; } = null;

    public DateTime? WatchedAt { get; set; } = null;
    public bool Favorite { get; set; }
    public int? Rating { get; set; }
    public DateTime? MarkedForWatchLaterAt { get; set; }
    // public DateTime? SkippedAt { get; set; } // TODO: implement skip functionality
}
