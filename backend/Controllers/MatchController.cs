using Microsoft.AspNetCore.Mvc;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using backend.DTOs;

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

        [Authorize]
        [HttpPut("{id}/result")]
        public IActionResult UpdateMatchResult(int id, [FromBody] MatchResultDto result, [FromHeader] string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null || user.IsAdmin != true)
            {
                return Forbid("Only admins can update match results.");
            }

            var match = _context.Matches.SingleOrDefault(m => m.Id == id);
            if (match == null)
            {
                return NotFound("Match not found.");
            }

            if (match.IsFinished)
            {
                return BadRequest("Match result has already been entered.");
            }

            match.Team1Score = result.Team1Score;
            match.Team2Score = result.Team2Score;
            match.IsFinished = true;

            if (match.Team1Score > match.Team2Score)
            {
                match.HomeTeam.Points += 3;
            }
            else if (match.Team1Score < match.Team2Score)
            {
                match.AwayTeam.Points += 3;
            }
            else
            {
                match.HomeTeam.Points += 1;
                match.AwayTeam.Points += 1;
            }

            _context.SaveChanges();
            return Ok("Match result updated, and points assigned.");
        }

        [HttpGet("played-matches")]
        public IActionResult GetPlayedMatches([FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var playedMatches = _context.Matches
                .Where(m => m.IsFinished)
                .Select(m => new
                {
                    MatchId = m.Id,
                    HomeTeam = m.HomeTeam.Name,
                    AwayTeam = m.AwayTeam.Name,
                    Team1Score = m.Team1Score,
                    Team2Score = m.Team2Score
                }).ToList();

            return Ok(playedMatches);
        }
    }
}
