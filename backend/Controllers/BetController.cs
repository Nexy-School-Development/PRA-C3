using Microsoft.AspNetCore.Mvc;
using backend.Models;
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

        // Get all bets
        [HttpGet]
        public IActionResult GetAllBets()
        {
            var bets = _context.Bets.ToList();
            return Ok(bets);
        }

        // Place a bet
        [HttpPost]
        public IActionResult PlaceBet([FromBody] Bet bet)
        {
            // Log the incoming bet for debugging
            System.Diagnostics.Debug.WriteLine($"Received Bet: UserId = {bet.UserId}, MatchId = {bet.MatchId}, Amount = {bet.Amount}, Prediction = {bet.Prediction}");

            // Retrieve user details using UserId
            var user = _context.Users.SingleOrDefault(u => u.Id == bet.UserId);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Check if the user has sufficient balance
            if (user.Balance < bet.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            // Validate the match
            var match = _context.Matches.Find(bet.MatchId);
            if (match == null || match.IsFinished)
            {
                return BadRequest("Match is invalid or already finished.");
            }

            // Associate the bet with the user
            bet.Prediction = bet.Prediction.ToLower(); // Make sure prediction is in lowercase
            bet.IsResolved = false; // Bet is unresolved initially

            // Deduct the bet amount from the user's balance
            user.Balance -= bet.Amount;

            // Save the bet to the database
            _context.Bets.Add(bet);
            _context.SaveChanges();

            // Debugging: Log the user's current balance after the bet is placed
            System.Diagnostics.Debug.WriteLine($"User {user.Id}'s updated balance: {user.Balance}");

            return Created($"api/bet/{bet.Id}", bet);
        }

        // Cancel a bet
        [HttpDelete("{id}")]
        public IActionResult CancelBet(int id, [FromBody] int userId)
        {
            var bet = _context.Bets.Find(id);
            if (bet == null)
            {
                return NotFound("Bet not found.");
            }

            // Check if the bet belongs to the user trying to cancel it
            if (bet.UserId != userId)
            {
                return Forbid("You can only cancel your own bets.");
            }

            // Refund the user their bet amount
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.Balance += bet.Amount;
                _context.SaveChanges();
            }

            // Remove the bet from the database
            _context.Bets.Remove(bet);
            _context.SaveChanges();

            return Ok("Bet cancelled.");
        }

        // Resolve all bets
        [HttpPost("resolve-bets")]
        public IActionResult ResolveBets()
        {
            var finishedMatches = _context.Matches.Where(m => m.IsFinished).ToList();
            foreach (var bet in _context.Bets.Where(b => !b.IsResolved).ToList())
            {
                var match = finishedMatches.SingleOrDefault(m => m.Id == bet.MatchId);
                if (match != null)
                {
                    bool isCorrect = bet.Prediction == "home" && match.Team1Score > match.Team2Score ||
                                     bet.Prediction == "away" && match.Team1Score < match.Team2Score ||
                                     bet.Prediction == "draw" && match.Team1Score == match.Team2Score;

                    if (isCorrect)
                    {
                        bet.Payout = bet.Amount * 2; // Double the bet if correct
                        var user = _context.Users.Find(bet.UserId);
                        if (user != null)
                        {
                            user.Balance += bet.Payout.Value; // Add the payout to the user's balance
                        }
                    }
                    else
                    {
                        bet.Payout = 0;
                    }

                    bet.IsResolved = true; // Mark the bet as resolved
                }
            }

            _context.SaveChanges();
            return Ok("All bets have been resolved.");
        }

        // Get all bets placed by a specific user
        [HttpGet("user-bets")]
        public IActionResult GetUserBets([FromQuery] int userId)
        {
            // Debugging: Log the incoming userId
            System.Diagnostics.Debug.WriteLine($"Fetching bets for UserId: {userId}");

            var bets = _context.Bets
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.MatchId,
                    b.Prediction,
                    b.Amount,
                    b.IsResolved,
                    b.Payout
                }).ToList();

            return Ok(bets);
        }
    }
}
