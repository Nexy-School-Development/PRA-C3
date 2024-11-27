using System;

namespace backend.Models;

public class Team
{
        public int Id { get; set; }

        public string Name { get; set; }

        public int Points { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }
}