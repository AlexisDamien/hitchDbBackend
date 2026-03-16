using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    public string Pseudo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<UserMovieRelation> MovieRelations { get; set; } = new List<UserMovieRelation>();
    [JsonIgnore]
    public ICollection<MovieList> MovieLists { get; set; } = new List<MovieList>();
}