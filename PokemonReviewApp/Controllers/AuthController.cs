using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokemonReviewApp.Data; 
using PokemonReviewApp.Dto;
using PokemonReviewApp.Hash;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly DataContext _context; 

    public AuthController(IUserRepository userRepository, IConfiguration config, DataContext context)
    {
        _userRepository = userRepository;
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null)
            return BadRequest("Geçersiz giriş verisi");

        var user = _userRepository.GetUserByEmail(loginDto.Email);
        if (user == null)
            return Unauthorized("Kullanıcı bulunamadı");

        string hashedPassword = HashHelper.ComputeSha512Hash(loginDto.Password);
        if (user.Password != hashedPassword)
            return Unauthorized("Şifre hatalı");

        
        var log = new AuthLog
        {
            UserId = user.Id,
            LoginDate = DateTime.Now
        };

        _context.AuthLogs.Add(log);
        _context.SaveChanges();

        // ➕ Claims ve Token işlemleri
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var addedPermissions = new List<string>();

        if (user.UserRoles != null)
        {
            foreach (var userRole in user.UserRoles)
            {
                var role = userRole.Role;
                if (role == null) continue;

                claims.Add(new Claim(ClaimTypes.Role, role.Name.Trim()));

                if (role.RolePermissions != null)
                {
                    foreach (var rolePermission in role.RolePermissions)
                    {
                        var permission = rolePermission.Permission?.Name?.Trim();
                        if (!string.IsNullOrEmpty(permission) && !addedPermissions.Contains(permission))
                        {
                            claims.Add(new Claim("permission", permission));
                            addedPermissions.Add(permission);
                        }
                    }
                }
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtConfig:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: _config["JwtConfig:Issuer"],
            audience: _config["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["JwtConfig:ExpireMinutes"])),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = jwt
        });
    }
}
