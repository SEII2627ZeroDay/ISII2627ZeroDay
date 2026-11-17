using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamDetailDTO : TeamForCreateDTO
{
    public TeamDetailDTO() : base()
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
        string captainId,
        string captainName,
        string captainSurname,
        int captainAge,
        IList<TeamInvitationDTO> invitedPlayers)
        : base(name, description, maxMembers, minAge, maxAge, sport, captainId, invitedPlayers)
    {
        Id = id;
        CaptainName = captainName;
        CaptainSurname = captainSurname;
        CaptainAge = captainAge;
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
        IList<TeamInvitationDTO> invitedPlayers)
        : this(id, name, description, maxMembers, minAge, maxAge, sport, "", captainName, captainSurname, captainAge, invitedPlayers)
    {
    }

    public int Id { get; set; }

    [Required]
    public string CaptainName { get; set; } = "";

    [Required]
    public string CaptainSurname { get; set; } = "";

    public int CaptainAge { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is TeamDetailDTO other &&
               base.Equals(obj) &&
               Id == other.Id &&
               CaptainName == other.CaptainName &&
               CaptainSurname == other.CaptainSurname &&
               CaptainAge == other.CaptainAge;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id, CaptainName, CaptainSurname, CaptainAge);
    }
}
