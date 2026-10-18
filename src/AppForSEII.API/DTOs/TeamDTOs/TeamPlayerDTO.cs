namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamPlayerDTO
{
    public TeamPlayerDTO()
    {
    }

    public TeamPlayerDTO(string name, string surname, int age, string invitationMessage, bool invitationAccepted)
    {
        Name = name;
        Surname = surname;
        Age = age;
        InvitationMessage = invitationMessage;
        InvitationAccepted = invitationAccepted;
    }

    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public int Age { get; set; }
    public string InvitationMessage { get; set; } = "";
    public bool InvitationAccepted { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is TeamPlayerDTO other &&
               Name == other.Name &&
               Surname == other.Surname &&
               Age == other.Age &&
               InvitationMessage == other.InvitationMessage &&
               InvitationAccepted == other.InvitationAccepted;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Surname, Age, InvitationMessage, InvitationAccepted);
    }
}
