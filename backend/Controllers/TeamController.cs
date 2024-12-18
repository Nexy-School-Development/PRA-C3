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
                    t.CreatorId
                })
                .ToList();

            return Ok(teams);
        }

        [HttpPut("changePoints/{id}")]
        public IActionResult ChangePoints(int id, [FromBody] int newPoints)
        {
            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            team.Points = newPoints;
            _context.SaveChanges();

            return Ok(new
            {
                message = $"Points updated successfully for team ID {id}.",
                teamId = team.Id,
                newPoints = team.Points
            });
        }

        [HttpPost("createTeam")]
        public IActionResult CreateTeam([FromBody] Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
        }

        [HttpDelete("delete/{id}")]
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
