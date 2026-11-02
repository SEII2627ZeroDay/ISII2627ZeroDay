using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamInvitationDTO
{
    public TeamInvitationDTO()
    {
    }

    public TeamInvitationDTO(string userId, string invitationMessage)
    {
        UserId = userId;
        InvitationMessage = invitationMessage;
    }

    [Required]
    public string UserId { get; set; } = "";

    [Required]
    [MinLength(3, ErrorMessage = "Invitation message must be at least 3 characters.")]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";

    public override bool Equals(object? obj)
    {
        return obj is TeamInvitationDTO other &&
               UserId == other.UserId &&
               InvitationMessage == other.InvitationMessage;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, InvitationMessage);
    }
}
