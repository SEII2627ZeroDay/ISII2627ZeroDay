namespace AppForSEII.API.DTOs.GameDTOs;

public class GameInvitationDTO
{
    public GameInvitationDTO() { }

    public GameInvitationDTO(
        string teamName,
        string captain,
        int maxMembers,
        int minAge,
        int maxAge,
        string sportName,
        bool invitationAccepted,
        string? message)
    {
        TeamName = teamName;
        Captain = captain;
        MaxMembers = maxMembers;
        MinAge = minAge;
        MaxAge = maxAge;
        SportName = sportName;
        InvitationAccepted = invitationAccepted;
        InvitationMessage = message;
    }

    public GameInvitationDTO(string teamName, bool invitationAccepted, string? message)
        : this(teamName, "", 0, 0, 0, "", invitationAccepted, message)
    {
    }

    public string TeamName { get; set; } = "";
    public string Captain { get; set; } = "";
    public int MaxMembers { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public string SportName { get; set; } = "";
    public bool InvitationAccepted { get; set; }
    public string? InvitationMessage { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is GameInvitationDTO other &&
               TeamName == other.TeamName &&
               (string.IsNullOrEmpty(Captain) || string.IsNullOrEmpty(other.Captain) || Captain == other.Captain) &&
               (MaxMembers == 0 || other.MaxMembers == 0 || MaxMembers == other.MaxMembers) &&
               (MinAge == 0 || other.MinAge == 0 || MinAge == other.MinAge) &&
               (MaxAge == 0 || other.MaxAge == 0 || MaxAge == other.MaxAge) &&
               (string.IsNullOrEmpty(SportName) || string.IsNullOrEmpty(other.SportName) || SportName == other.SportName) &&
               InvitationAccepted == other.InvitationAccepted &&
               InvitationMessage == other.InvitationMessage;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TeamName, InvitationAccepted, InvitationMessage);
    }
}
