namespace Haidon_BE.Domain.Models;

public class MatchCriteria
{
    public string Gender { get; set; } = "Any";
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
}