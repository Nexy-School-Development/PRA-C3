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
        public IActionResult GetTeams()
        {
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

        [HttpPost("createteam")]
        public IActionResult CreateTeam([FromBody] Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateTeam(int id, [FromBody] Team updatedTeam)
        {
            var existingTeam = _context.Teams.Find(id);
            if (existingTeam == null)
            {
                return NotFound("Team not found.");
            }

            existingTeam.Name = updatedTeam.Name;
            _context.SaveChanges();

            return Ok(existingTeam);
        }

        [HttpPost("{Id}/add-player/{userId}")]
        public IActionResult AddPlayerToTeam(int Id, int userId)
        {
            var team = _context.Teams.Find(Id);
            if (team == null)
            {
                return NotFound("Team not found.");
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
        public IActionResult RemovePlayerFromTeam(int Id, int userId)
        {
            var team = _context.Teams.Find(Id);
            if (team == null)
            {
                return NotFound("Team not found.");
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

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteTeam(int id)
        {
            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            _context.Teams.Remove(team);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("admin/delete/{id}")]
        public IActionResult AdminDeleteTeam(int id)
        {
            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            _context.Teams.Remove(team);
            _context.SaveChanges();
            return Ok($"Team with ID {id} has been deleted successfully.");
        }
    }
}