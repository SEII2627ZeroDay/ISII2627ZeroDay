using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class RefereeGroup
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Name { get; set; } = "Name between 3 and 20 chars";

    public string? Description { get; set; } = "Description";

    public string? Rules { get; set; } = "Rules for the game";

    [Required]
    public int GameId { get; set; }

    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!;

    public IList<RefereeAssignedTo> RefereeAssignedTos { get; set; } = new List<RefereeAssignedTo>();
}