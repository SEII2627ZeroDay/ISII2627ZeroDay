namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamForGameDTO
{
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

    public string Sport{get;set;}

}

