using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Models;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
    }

    public ApplicationUser(string id, string name, string surname, string userName)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
    }

    public ApplicationUser(string id, string name, string surname, string userName, DateOnly birthDate, int age, Gender gender)
        : this(id, name, surname, userName)
    {
        BirthDate = birthDate;
        Age = age;
        Gender = gender;
    }

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Surname { get; set; }

    public DateOnly BirthDate { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }

    public IList<Team> CaptainOf { get; set; } = new List<Team>();

    public IList<TeamInvitation> TeamInvitations { get; set; } = new List<TeamInvitation>();

    public IList<InterestedIn> Interested { get; set; } = new List<InterestedIn>();
}
