using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MonRestoAPI.Data;
using MonRestoAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MonRestoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Action pour l'inscription
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userRegister.Username || u.Email == userRegister.Email);
            if (existingUser != null)
            {
                return BadRequest("Username or email already exists.");
            }

            var passwordHash = ComputePasswordHash(userRegister.Password);

            var newUser = new User
            {
                Username = userRegister.Username,
                Email = userRegister.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // Action pour la connexion
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var computedHash = ComputePasswordHash(userLogin.Password);

            if (user.PasswordHash != computedHash)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJwtToken(user.Username);
            return Ok(new { Token = token });
        }

        // Action pour obtenir tous les utilisateurs
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // Action pour obtenir un utilisateur par ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        // Action pour mettre à jour un utilisateur
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRegister userRegister)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Vérifier si le nom d'utilisateur ou l'email existe déjà
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => (u.Username == userRegister.Username || u.Email == userRegister.Email) && u.Id != id);
            if (existingUser != null)
            {
                return BadRequest("Username or email already exists.");
            }

            // Mettre à jour les informations de l'utilisateur
            user.Username = userRegister.Username;
            user.Email = userRegister.Email;
            user.PasswordHash = ComputePasswordHash(userRegister.Password);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("User updated successfully.");
        }

        // Action pour supprimer un utilisateur
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string ComputePasswordHash(string password)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["PasswordHashKey"]); // Clé partagée pour le hachage
            using var hmac = new System.Security.Cryptography.HMACSHA256(key);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token manquant");
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var username = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Token invalide");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return Unauthorized("Utilisateur introuvable");
            }

            return Ok(new
            {
                username = user.Username,
                email = user.Email,
            });
        }

      


    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserRegister
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
