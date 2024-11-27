using Microsoft.AspNetCore.Mvc;
using backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly Testappcontext _context;

        public MatchController(Testappcontext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllMatches()
        {
            var matches = _context.Matches.ToList();
            return Ok(matches);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateMatch([FromBody] Match match)
        {
            _context.Matches.Add(match);
            _context.SaveChanges();
            return Created($"api/match/{match.Id}", match);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateMatch(int id, [FromBody] Match updatedMatch)
        {
            var match = _context.Matches.Find(id);
            if (match == null)
            {
                return NotFound("Match not found");
            }

            match.HomeTeam = updatedMatch.HomeTeam;
            match.AwayTeam = updatedMatch.AwayTeam;
            match.Team1Score = updatedMatch.Team1Score;
            match.Team2Score = updatedMatch.Team2Score;
            match.Starttime = updatedMatch.Starttime;

            _context.SaveChanges();
            return Ok(match);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteMatch(int id)
        {
            var match = _context.Matches.Find(id);
            if (match == null)
            {
                return NotFound("Match not found");
            }

            _context.Matches.Remove(match);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
