namespace Haidon_BE.Api.Models;

public class MatchCriteria
{
    /// <summary>
    /// "Male" | "Female" | "Any" (or empty)
    /// </summary>
    public string Gender { get; set; } = "Any";
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
}
