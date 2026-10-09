using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(UserId), nameof(TeamId))]
public class TeamInvitation
{
    public TeamInvitation()
    {
    }

    public TeamInvitation(ApplicationUser user, Team team, string invitationMessage, bool invitationAccepted = false)
    {
        User = user;
        UserId = user?.Id ?? "0";
        Team = team;
        TeamId = team?.Id ?? 0;
        InvitationMessage = invitationMessage;
        InvitationAccepted = invitationAccepted;
    }

    [Required]
    public string UserId { get; set; } = "0";

    [Required]
    public int TeamId { get; set; }

    public bool InvitationAccepted { get; set; } = false;

    [MinLength(3)]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;
}