using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(UserId), nameof(TeamId))]
public class TeamInvitation
{
    [Required]
    public string UserId { get; set; } = "0";

    [Required]
    public int TeamId { get; set; }

    public bool InvitationAccepted { get; set; } = false;

    [MinLength(3)]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;
}