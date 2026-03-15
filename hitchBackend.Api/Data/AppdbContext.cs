using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<WatchedMovie> WatchedMovies => Set<WatchedMovie>();
    public DbSet<FavoriteMovie> FavoriteMovies => Set<FavoriteMovie>();
    public DbSet<MovieList> MovieLists => Set<MovieList>();
    public DbSet<MovieListItem> MovieListItems => Set<MovieListItem>();
}