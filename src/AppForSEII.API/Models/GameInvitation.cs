using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(TeamId), nameof(GameId))]
public class GameInvitation
{
    [Required]
    public int TeamId { get; set; }

    [Required]
    public int GameId { get; set; }

    public bool AcceptedGame { get; set; } = false;

    [MinLength(10)]
    public string Message { get; set; } = "Message more than 10 chars";

}