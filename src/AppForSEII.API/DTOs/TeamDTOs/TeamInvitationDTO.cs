using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamInvitationDTO
{
    public TeamInvitationDTO()
    {
    }

    public TeamInvitationDTO(string userId, string name, string surname, int age, string invitationMessage, bool invitationAccepted = false)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Age = age;
        InvitationMessage = invitationMessage;
        InvitationAccepted = invitationAccepted;
    }

    public TeamInvitationDTO(string userId, string invitationMessage)
        : this(userId, "", "", 0, invitationMessage, false)
    {
    }

    public TeamInvitationDTO(string name, string surname, int age, string invitationMessage, bool invitationAccepted = false)
        : this("", name, surname, age, invitationMessage, invitationAccepted)
    {
    }

    [Required]
    public string UserId { get; set; } = "";

    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public int Age { get; set; }

    [Required]
    [MinLength(3, ErrorMessage = "Invitation message must be at least 3 characters.")]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";

    public bool InvitationAccepted { get; set; } = false;

    public override bool Equals(object? obj)
    {
        return obj is TeamInvitationDTO other &&
               (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(other.UserId) || UserId == other.UserId) &&
               Name == other.Name &&
               Surname == other.Surname &&
               Age == other.Age &&
               InvitationMessage == other.InvitationMessage &&
               InvitationAccepted == other.InvitationAccepted;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, Name, Surname, Age, InvitationMessage, InvitationAccepted);
    }
}
