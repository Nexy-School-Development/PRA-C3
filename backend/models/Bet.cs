using System;

namespace backend.Models
{
    public class Bet
    {
        public int Id { get; set; }
        public int MatchId { get; set; } 
        public Match Match { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Amount { get; set; }
        public string Prediction { get; set; }
        public bool IsResolved { get; set; }
        public decimal? Payout { get; set; }
    }
}