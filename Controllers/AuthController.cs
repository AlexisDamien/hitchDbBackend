using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _db;

    public AuthController(IAuthService authService, AppDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return Conflict("Email déjà utilisé");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { token = _authService.GenerateToken(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CreateUserDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Identifiants invalides");

        return Ok(new { token = _authService.GenerateToken(user) });
    }
}