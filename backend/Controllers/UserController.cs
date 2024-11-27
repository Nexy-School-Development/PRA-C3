using Microsoft.AspNetCore.Mvc;
using backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Testappcontext _context;

        public UserController(Testappcontext context)
        {
            _context = context;
        }

        // Create User (POST)
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            User? duplicate = _context.Users.SingleOrDefault(u => u.Email == user.Email);
            if (duplicate != null)
            {
                return Conflict("User with this email already exists");
            }
            user.Password = Models.User.ComputeSha256Hash(user.Password);
            _context.Users.Add(user);
            user.Balance = 50;
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email && u.ValidatePassword(password));
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAllUsers([FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null || (!requestingUser.IsAdmin ?? false))
            {
                return Forbid("Only admins can view all users");
            }
            return Ok(_context.Users.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id, [FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null || (!requestingUser.IsAdmin ?? false))
            {
                return Forbid("Only admins can view user details");
            }
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(string email, string newPassword)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long.");
            }

            user.Password = Models.User.ComputeSha256Hash(newPassword);
            _context.SaveChanges();
            return Ok("Password reset successfully.");
        }

        [HttpPost("grant-admin/{id}")]
        public IActionResult GrantAdminPrivileges(int id, [FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null || (!requestingUser.IsAdmin ?? false))
            {
                return Forbid("Only admins can grant admin privileges");
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.IsAdmin = true;
            _context.SaveChanges();
            return Ok("Admin privileges granted successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id, [FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null || (!requestingUser.IsAdmin ?? false))
            {
                return Forbid("Only admins can delete users");
            }
            var userToDelete = _context.Users.Find(id);
            if (userToDelete == null)
            {
                return NotFound("User not found");
            }
            _context.Users.Remove(userToDelete);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
