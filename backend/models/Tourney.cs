using System;

namespace backend.Models;

public class Tourney
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Match> Matches { get; set; }
}
