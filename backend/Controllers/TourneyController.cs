using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourneyController : ControllerBase
    {
        private readonly Testappcontext _context;

        public TourneyController(Testappcontext context)
        {
            _context = context;
        }

        [HttpPost("generate-schedule")]
        public IActionResult GenerateSchedule([FromHeader] string token, [FromBody] int fieldsAvailable)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null || (!requestingUser.IsAdmin ?? false))
            {
                return Forbid("Only admins can generate schedules.");
            }

            var teams = _context.Teams.ToList();
            if (teams.Count < 2)
            {
                return BadRequest("Not enough teams to generate a schedule.");
            }

            var matches = new List<Match>();
            int fieldCounter = 1;
            DateTime startTime = DateTime.Now;

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    matches.Add(new Match
                    {
                        HomeTeam = teams[i],
                        AwayTeam = teams[j],
                        Team1Score = 0,
                        Team2Score = 0,
                        Starttime = startTime,
                        IsFinished = false
                    });

                    fieldCounter++;
                    if (fieldCounter > fieldsAvailable)
                    {
                        fieldCounter = 1;
                        startTime = startTime.AddHours(1);
                    }
                }
            }

            var tourney = new Tourney
            {
                Name = "Generated Tournament",
                Matches = matches
            };

            _context.Tourneys.Add(tourney);
            _context.SaveChanges();

            return Ok(tourney);
        }

        [HttpGet("schedule")]
        public IActionResult ViewSchedule([FromHeader] string token)
        {
            var requestingUser = _context.Users.SingleOrDefault(u => u.Token == token);
            if (requestingUser == null)
            {
                return Unauthorized("User not logged in.");
            }

            var tourney = _context.Tourneys
                .Select(t => new
                {
                    t.Name,
                    Matches = t.Matches.Select(m => new
                    {
                        HomeTeam = m.HomeTeam.Name,
                        AwayTeam = m.AwayTeam.Name,
                        StartTime = m.Starttime,
                        IsFinished = m.IsFinished
                    })
                })
                .FirstOrDefault();

            if (tourney == null)
            {
                return NotFound("No tournament schedule found.");
            }

            return Ok(tourney);
        }
    }
}