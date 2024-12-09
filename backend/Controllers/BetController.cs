using Microsoft.AspNetCore.Mvc;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BetController : ControllerBase
    {
        private readonly Testappcontext _context;

        public BetController(Testappcontext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllBets([FromHeader] string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null || user.IsAdmin != true)
            {
                return Forbid("Only admins can view all bets.");
            }

            var bets = _context.Bets.ToList();
            return Ok(bets);
        }

        [Authorize]
        [HttpPost]
        public IActionResult PlaceBet([FromBody] Bet bet, [FromHeader] string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null)
            {
                return Unauthorized("Only gamblers can place bets.");
            }

            if (user.Balance < bet.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            var match = _context.Matches.Find(bet.Match.Id);
            if (match == null || match.IsFinished)
            {
                return BadRequest("Match is invalid or already finished.");
            }

            bet.User = user;
            bet.Prediction = bet.Prediction.ToLower();
            bet.IsResolved = false;
            user.Balance -= bet.Amount;

            _context.Bets.Add(bet);
            _context.SaveChanges();

            return Created($"api/bet/{bet.Id}", bet);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult CancelBet(int id, [FromHeader] string token)
        {
            var bet = _context.Bets.Find(id);
            if (bet == null)
            {
                return NotFound("Bet not found.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null || bet.User.Id != user.Id)
            {
                return Forbid("You can only cancel your own bets.");
            }

            user.Balance += bet.Amount;
            _context.Bets.Remove(bet);
            _context.SaveChanges();

            return Ok("Bet cancelled.");
        }

        [Authorize]
        [HttpPost("resolve-bets")]
        public IActionResult ResolveBets([FromHeader] string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null || user.IsAdmin != true)
            {
                return Forbid("Only admins can resolve bets.");
            }

            var finishedMatches = _context.Matches.Where(m => m.IsFinished).ToList();
            foreach (var bet in _context.Bets.Where(b => !b.IsResolved).ToList())
            {
                var match = finishedMatches.SingleOrDefault(m => m.Id == bet.Match.Id);
                if (match != null)
                {
                    bool isCorrect = bet.Prediction == "home" && match.Team1Score > match.Team2Score ||
                                     bet.Prediction == "away" && match.Team1Score < match.Team2Score ||
                                     bet.Prediction == "draw" && match.Team1Score == match.Team2Score;

                    if (isCorrect)
                    {
                        bet.Payout = bet.Amount * 2;
                        bet.User.Balance += bet.Payout.Value;
                    }
                    else
                    {
                        bet.Payout = 0;
                    }

                    bet.IsResolved = true;
                }
            }

            _context.SaveChanges();
            return Ok("All bets have been resolved.");
        }

        [Authorize]
        [HttpGet("user-bets")]
        public IActionResult GetUserBets([FromHeader] string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null)
            {
                return Unauthorized("User not logged in.");
            }

            var bets = _context.Bets
                .Where(b => b.User.Id == user.Id)
                .Select(b => new
                {
                    MatchId = b.Match.Id,
                    Prediction = b.Prediction,
                    Amount = b.Amount,
                    IsResolved = b.IsResolved,
                    Payout = b.Payout
                }).ToList();

            return Ok(bets);
        }
    }
}
