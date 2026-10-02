using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(TeamId), nameof(GameId))]
public class GameInvitation
{
    [Required]
    public int TeamId { get; set; }

    [ForeignKey(nameof(TeamId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Team Team { get; set; } = null!;

    [Required]
    public int GameId { get; set; }

    [ForeignKey(nameof(GameId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Game Game { get; set; } = null!;

    [Required]
    public bool AcceptedGame { get; set; } = false;

    [MinLength(10)]
    public string Message { get; set; } = "Message more than 10 chars";

}