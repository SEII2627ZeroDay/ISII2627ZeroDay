using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(SportId), nameof(UserId))]
public class InterestedIn
{
    public InterestedIn()
    {
    }

    public InterestedIn(Sport sport, ApplicationUser userInterested, int skill)
    {
        Sport = sport;
        SportId = sport?.Id ?? 0;
        UserInterested = userInterested;
        UserId = userInterested?.Id ?? "0";
        Skill = skill;
    }

    [Required]
    public int SportId { get; set; }

    [Required]
    public string UserId { get; set; } = "0";

    [Range(1, 5)]
    public int Skill { get; set; } = 5;

    [ForeignKey(nameof(SportId))]
    public Sport Sport { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser UserInterested { get; set; } = null!;
}