using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.ApplicationUserDTO;

public class ApplicationUserForTeamDTO
{
    public ApplicationUserForTeamDTO()
    {
    }

    public ApplicationUserForTeamDTO(string id, string name, string surname, int age, string sport, int skill)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Age = age;
        Sport = sport;
        Skill = skill;
    }

    public string Id { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string Surname { get; set; } = "";

    [Range(0, 120)]
    public int Age { get; set; }

    [Required]
    [StringLength(50)]
    public string Sport { get; set; } = "";

    [Range(1, 5)]
    public int Skill { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is ApplicationUserForTeamDTO other &&
               Id == other.Id &&
               Name == other.Name &&
               Surname == other.Surname &&
               Age == other.Age &&
               Sport == other.Sport &&
               Skill == other.Skill;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Surname, Age, Sport, Skill);
    }
}
