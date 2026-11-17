using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamForCreateDTO
{
    public TeamForCreateDTO()
    {
    }

    public TeamForCreateDTO(
        string name,
        string? description,
        int maxMembers,
        int minAge,
        int maxAge,
        string sport,
        string captainId,
        IList<TeamInvitationDTO> invitedPlayers)
    {
        Name = name;
        Description = description;
        MaxMembers = maxMembers;
        MinAge = minAge;
        MaxAge = maxAge;
        Sport = sport;
        CaptainId = captainId;
        InvitedPlayers = invitedPlayers ?? new List<TeamInvitationDTO>();
    }

    [Required]
    [StringLength(9, MinimumLength = 4, ErrorMessage = "Team name must be between 4 and 9 characters.")]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "MaxMembers must be at least 1.")]
    public int MaxMembers { get; set; }

    [Range(3, int.MaxValue, ErrorMessage = "MinAge must be at least 3.")]
    public int MinAge { get; set; }

    [Range(3, int.MaxValue, ErrorMessage = "MaxAge must be at least 3.")]
    public int MaxAge { get; set; }

    [Required]
    public string Sport { get; set; } = "";

    [Required]
    public string CaptainId { get; set; } = "";

    public IList<TeamInvitationDTO> InvitedPlayers { get; set; } = new List<TeamInvitationDTO>();

    public override bool Equals(object? obj)
    {
        return obj is TeamForCreateDTO other &&
               Name == other.Name &&
               Description == other.Description &&
               MaxMembers == other.MaxMembers &&
               MinAge == other.MinAge &&
               MaxAge == other.MaxAge &&
               Sport == other.Sport &&
               (string.IsNullOrEmpty(CaptainId) || string.IsNullOrEmpty(other.CaptainId) || CaptainId == other.CaptainId) &&
               (InvitedPlayers == other.InvitedPlayers ||
                (InvitedPlayers != null && other.InvitedPlayers != null && InvitedPlayers.SequenceEqual(other.InvitedPlayers)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Sport, CaptainId, MaxMembers, MinAge, MaxAge);
    }
}
