using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class TeamInvitation
{
    [Required]
    public string UserId { get; set; } = "0";

    [Required]
    public int TeamId { get; set; }

    public bool InvitationAccepted { get; set; } = false;

    [MinLength(3)]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";
}