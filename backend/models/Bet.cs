using System;

namespace backend.Models
{
    public class Bet
    {
        public int Id { get; set; }

        public int MatchId { get; set; }
        public virtual Match Match { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } 

        // Bet details
        public decimal Amount { get; set; }
        public string Prediction { get; set; }
        public bool IsResolved { get; set; }
        public decimal? Payout { get; set; }
    }
}
