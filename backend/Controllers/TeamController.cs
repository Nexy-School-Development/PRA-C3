using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly Testappcontext _context;

        public TeamController(Testappcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTeams([FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var teams = _context.Teams.ToList();
            return Ok(teams);
        }

        [HttpPost]
        public IActionResult CreateTeam([FromHeader] string token, [FromBody] Team team)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            team.CreatorId = requestingUser.Id;
            _context.Teams.Add(team);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTeam([FromHeader] string token, int id, [FromBody] Team updatedTeam)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var existingTeam = _context.Teams.Find(id);
            if (existingTeam == null)
            {
                return NotFound("Team not found.");
            }

            if (existingTeam.CreatorId != requestingUser.Id)
            {
                return Forbid("You can only update teams you created.");
            }

            existingTeam.Name = updatedTeam.Name;
            _context.SaveChanges();

            return Ok(existingTeam);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTeam([FromHeader] string token, int id)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            if (team.CreatorId != requestingUser.Id)
            {
                return Forbid("You can only delete teams you created.");
            }

            _context.Teams.Remove(team);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("admin/{id}")]
        public IActionResult AdminDeleteTeam([FromHeader] string token, int id)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null || (!requestingUser.IsAdmin ?? false))
            {
                return Forbid("Only admins can delete any team.");
            }

            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            _context.Teams.Remove(team);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
