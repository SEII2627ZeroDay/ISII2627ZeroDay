namespace AppForSEII.API.DTOs; //CAMBIARRRRRRRRRRRRR Y PONER .GameDTOs LUEGOOOOOOOOO

public class GameInvitationDTO
{
    public GameInvitationDTO() { }

    public GameInvitationDTO(string teamName, bool invitationAccepted, string? message)
    {
        TeamName = teamName;
        InvitationAccepted = invitationAccepted;
        InvitationMessage = message;
    }

    public string TeamName { get; set; } = "";
    public bool InvitationAccepted { get; set; }
    public string? InvitationMessage { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is GameInvitationDTO other &&
               TeamName == other.TeamName &&
               InvitationAccepted == other.InvitationAccepted &&
               InvitationMessage == other.InvitationMessage;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TeamName, InvitationAccepted, InvitationMessage);
    }
}