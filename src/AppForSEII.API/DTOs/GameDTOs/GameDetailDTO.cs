using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AppForSEII.API.DTOs.GameDTOs;

public class GameDetailDTO : GameForCreateDTO
{
    public GameDetailDTO() : base() { }

    public GameDetailDTO(
        int id,
        string name,
        DateTime date,
        string place,
        string? description,
        string sportName,
        string responsibleUserName,
        IList<GameInvitationDTO> gameInvitations)
        : base(name, date, place, description, sportName, responsibleUserName, gameInvitations)
    {
        Id = id;
    }

    public int Id { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is GameDetailDTO other &&
               base.Equals(obj) &&
               Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id);
    }
}
