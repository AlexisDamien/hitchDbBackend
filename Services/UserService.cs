using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _db.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) =>
        await _db.Users.FindAsync(id);

    public async Task<User> CreateAsync(CreateUserDto dto)
    {
        var user = new User { Email = dto.Email };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is not null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}