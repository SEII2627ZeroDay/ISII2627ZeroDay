using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamDetailDTO
{
    public TeamDetailDTO()
    {
    }

    public TeamDetailDTO(
        int id,
        string name,
        string? description,
        int maxMembers,
        int minAge,
        int maxAge,
        string sport,
        string captainName,
        string captainSurname,
        int captainAge,
        IList<TeamPlayerDTO> invitedPlayers)
    {
        Id = id;
        Name = name;
        Description = description;
        MaxMembers = maxMembers;
        MinAge = minAge;
        MaxAge = maxAge;
        Sport = sport;
        CaptainName = captainName;
        CaptainSurname = captainSurname;
        CaptainAge = captainAge;
        InvitedPlayers = invitedPlayers;
    }

    public int Id { get; set; }

    [Required]
    [StringLength(9, MinimumLength = 4)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxMembers { get; set; }

    [Range(3, int.MaxValue)]
    public int MinAge { get; set; }

    [Range(3, int.MaxValue)]
    public int MaxAge { get; set; }

    [Required]
    public string Sport { get; set; } = "";

    [Required]
    public string CaptainName { get; set; } = "";

    [Required]
    public string CaptainSurname { get; set; } = "";

    public int CaptainAge { get; set; }

    public IList<TeamPlayerDTO> InvitedPlayers { get; set; } = new List<TeamPlayerDTO>();

    public override bool Equals(object? obj)
    {
        return obj is TeamDetailDTO other &&
               Id == other.Id &&
               Name == other.Name &&
               Description == other.Description &&
               MaxMembers == other.MaxMembers &&
               MinAge == other.MinAge &&
               MaxAge == other.MaxAge &&
               Sport == other.Sport &&
               CaptainName == other.CaptainName &&
               CaptainSurname == other.CaptainSurname &&
               CaptainAge == other.CaptainAge &&
               (InvitedPlayers == other.InvitedPlayers || 
                (InvitedPlayers != null && other.InvitedPlayers != null && InvitedPlayers.SequenceEqual(other.InvitedPlayers)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Sport, CaptainName, InvitedPlayers);
    }
}
