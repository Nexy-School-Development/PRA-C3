using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend.Models; // Include your Models namespace
using System.Linq;
using System.Collections.Generic;
using System;

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

        /// <summary>
        /// Generates a schedule for the tournament.
        /// </summary>
        /// <param name="fieldsAvailable">Number of fields available for scheduling matches</param>
        [HttpPost("generate-schedule")]
        public IActionResult GenerateSchedule([FromBody] int fieldsAvailable)
        {
            // Fetch all teams
            var teams = _context.Teams.ToList();
            if (teams.Count < 2)
            {
                return BadRequest("Not enough teams to generate a schedule.");
            }

            // Initialize match list and scheduling variables
            var matches = new List<Match>();
            int fieldCounter = 1; // Track which field a match is assigned to
            DateTime startTime = DateTime.Now;

            // Generate round-robin matches
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

                    // Rotate through fields and adjust match time
                    fieldCounter++;
                    if (fieldCounter > fieldsAvailable)
                    {
                        fieldCounter = 1;
                        startTime = startTime.AddHours(1);
                    }
                }
            }

            // Create a new tournament with the generated matches
            var tourney = new Tourney
            {
                Name = "Generated Tournament",
                Matches = matches
            };

            _context.Tourneys.Add(tourney);
            _context.SaveChanges();

            return Ok(new { Message = "Tournament schedule generated successfully.", Tourney = tourney });
        }

        /// <summary>
        /// View the tournament schedule.
        /// </summary>
        [HttpGet("schedule")]
        public IActionResult ViewSchedule()
        {
            // Retrieve tournament with match details
            var tourney = _context.Tourneys
                .Select(t => new
                {
                    TournamentName = t.Name,
                    Matches = t.Matches.Select(m => new
                    {
                        HomeTeam = m.HomeTeam.Name,
                        AwayTeam = m.AwayTeam.Name,
                        StartTime = m.Starttime,
                        IsFinished = m.IsFinished ? "Finished" : "Upcoming"
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
