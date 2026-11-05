using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AppForSEII.API.DTOs.GameDTOs;

public class GameDetailDTO
{
    public GameDetailDTO() { }

    public GameDetailDTO(
        int id, 
        string name, 
        DateTime date, 
        string place, 
        string? description, 
        string sportName, 
        string responsibleUserName, 
        IList<GameInvitationDTO> gameInvitations)
    {
        Id = id;
        Name = name;
        Date = date;
        Place = place;
        Description = description;
        SportName = sportName;
        ResponsibleUserName = responsibleUserName;
        GameInvitations = gameInvitations;
    }

    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = "";
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public string Place { get; set; } = "";
    
    public string? Description { get; set; }
    
    [Required]
    public string SportName { get; set; } = "";
    
    [Required]
    public string ResponsibleUserName { get; set; } = "";

    public IList<GameInvitationDTO> GameInvitations { get; set; } = new List<GameInvitationDTO>();

    public override bool Equals(object? obj)
    {
        return obj is GameDetailDTO other &&
               Id == other.Id &&
               Name == other.Name &&
               Date == other.Date &&
               Place == other.Place &&
               Description == other.Description &&
               SportName == other.SportName &&
               ResponsibleUserName == other.ResponsibleUserName &&
               (GameInvitations == other.GameInvitations || 
                (GameInvitations != null && other.GameInvitations != null && GameInvitations.SequenceEqual(other.GameInvitations)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Date, SportName, GameInvitations);
    }
}