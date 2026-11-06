using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamForGameDTO
{
    public TeamForGameDTO()
    {
    }

    public TeamForGameDTO(int id, string name, int maxMembers, int minAge, int maxAge, string sport)
    {
        Id = id;
        Name = name;
        MaxMembers = maxMembers;
        MinAge = minAge;
        MaxAge = maxAge;
        Sport = sport;
    }

    public int Id { get; set; }

    [Required]
    [StringLength(9, MinimumLength = 4)]
    public string Name { get; set; } = "Team name";

    [Range(1, int.MaxValue)]
    public int MaxMembers { get; set; }

    [Range(3, int.MaxValue)]
    public int MinAge { get; set; } = 3;

    [Range(3, int.MaxValue)]
    public int MaxAge { get; set; } = 3;

    public string Sport { get; set; } = "";

    public override bool Equals(object? obj)
    {
        return obj is TeamForGameDTO other &&
               Id == other.Id &&
               Name == other.Name &&
               MaxMembers == other.MaxMembers &&
               MinAge == other.MinAge &&
               MaxAge == other.MaxAge &&
               Sport == other.Sport;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, MaxMembers, MinAge, MaxAge, Sport);
    }
}

