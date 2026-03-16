using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<UserMovieRelation> UserMovieRelations => Set<UserMovieRelation>();
    public DbSet<MovieList> MovieLists => Set<MovieList>();
    public DbSet<MovieListItem> MovieListItems => Set<MovieListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserMovieRelation>()
            .HasKey(r => new { r.UserId, r.MovieId });
    }
}