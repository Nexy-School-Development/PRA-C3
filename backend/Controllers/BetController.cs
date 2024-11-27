using Microsoft.AspNetCore.Mvc;
using backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BetController(Testappcontext context) : ControllerBase
    {
        private readonly Testappcontext _context = context;

        [Authorize]
        [HttpGet]
        public IActionResult GetAllBets()
        {
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
                return Forbid("Only gamblers can place bets");
            }

            bet.User = user;
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
                return NotFound("Bet not found");
            }

            var user = _context.Users.SingleOrDefault(u => u.Token == token);
            if (user == null || (bet.User != user))
            {
                return Forbid("You can only cancel your own bets");
            }

            _context.Bets.Remove(bet);
            _context.SaveChanges();
            return Ok("Bet cancelled");
        }
    }
}
