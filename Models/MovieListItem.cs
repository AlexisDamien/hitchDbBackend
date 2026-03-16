using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class MovieListItem
{
    public int MovieListId { get; set; }
    [JsonIgnore]
    public MovieList MovieList { get; set; } = null!;

    public int MovieId { get; set; }
    [NotMapped]
    public object? Movie { get; set; } = null;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}