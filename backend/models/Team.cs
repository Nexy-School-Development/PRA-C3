using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Points { get; set; } = 0;
        public int CreatorId { get; set; }

        // Prevent circular reference during serialization
        [JsonIgnore]
        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();

        [JsonIgnore]
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}
