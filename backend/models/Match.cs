using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }  // Nullable HomeTeam

        public int AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }  // Nullable AwayTeam

        public int Team1Score { get; set; }
        public int Team2Score { get; set; }

        public DateTime Starttime { get; set; }
        public bool IsFinished { get; set; }

        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
