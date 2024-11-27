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
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {

            var user = _context.Users.ToList().SingleOrDefault(u => u.Email == email && u.ValidatePassword(password));
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            return Ok(user);
        }

        // Retrieve All Users (GET)
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

        // Retrieve Single User (GET by ID)
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

        // Delete User (DELETE)
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