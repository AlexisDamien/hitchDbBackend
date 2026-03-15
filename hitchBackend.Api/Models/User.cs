public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Pseudo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WatchedMovie> WatchedMovies { get; set; } = new List<WatchedMovie>();
    public ICollection<FavoriteMovie> FavoriteMovies { get; set; } = new List<FavoriteMovie>();
    public ICollection<MovieList> MovieLists { get; set; } = new List<MovieList>();
}