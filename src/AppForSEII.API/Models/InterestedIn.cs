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

    public InterestedIn(int sportId, string userId, int skill)
    {
        SportId = sportId;
        UserId = userId;
        Skill = skill;
    }

    [Required]
    public int SportId { get; set; }

    [Required]
    public string UserId { get; set; } = "0";

    [Range(1, 5)]
    public int Skill { get; set; } = 5;

    [ForeignKey(nameof(SportId))]
    [InverseProperty("Interested")]
    public Sport Sport { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Interested")]
    public ApplicationUser UserInterested { get; set; } = null!;
}