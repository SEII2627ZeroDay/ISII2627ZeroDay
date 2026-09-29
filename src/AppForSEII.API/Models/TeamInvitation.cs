using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class TeamInvitation
{
    public string UserId { get; set; } = "0";

    public int TeamId { get; set; }

    public bool InvitationAccepted { get; set; } = false;

    [MinLength(3)]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";
}