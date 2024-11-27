using System;

namespace backend.Models
{
    public class Tourney
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Match> Matches { get; set; } = new List<Match>();
    }
}