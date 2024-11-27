using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.Linq;

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

            var teams = _context.Teams
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Points,
                    Players = _context.Users
                        .Where(u => u.Id == t.Id)
                        .Select(u => new { u.Id, u.Email })
                        .ToList()
                }).ToList();

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

        [HttpPost("{Id}/add-player/{userId}")]
        public IActionResult AddPlayerToTeam([FromHeader] string token, int Id, int userId)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var team = _context.Teams.Find(Id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            if (team.CreatorId != requestingUser.Id)
            {
                return Forbid("You can only add players to teams you created.");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Id == team.Id)
            {
                return BadRequest("User is already part of this team.");
            }

            user.Id = team.Id;
            _context.SaveChanges();

            return Ok("Player added to the team successfully.");
        }

        [HttpDelete("{Id}/remove-player/{userId}")]
        public IActionResult RemovePlayerFromTeam([FromHeader] string token, int Id, int userId)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var team = _context.Teams.Find(Id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            if (team.CreatorId != requestingUser.Id)
            {
                return Forbid("You can only remove players from teams you created.");
            }

            var user = _context.Users.Find(userId);
            if (user == null || user.Id != team.Id)
            {
                return NotFound("Player not found in this team.");
            }

            user.Id = 0;
            _context.SaveChanges();

            return Ok("Player removed from the team successfully.");
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
