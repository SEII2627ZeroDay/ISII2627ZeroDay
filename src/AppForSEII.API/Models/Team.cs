using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Team
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(9, MinimumLength = 4)]
    public string Name { get; set; } = "Team name";

    public string? Description { get; set; } = "Team name";

    [Range(1, int.MaxValue)]
    public int MaxMembers { get; set; }

    [Range(3, int.MaxValue)]
    public int MinAge { get; set; } = 3;

    [Range(3, int.MaxValue)]
    public int MaxAge { get; set; } = 3;

    public string CaptainId { get; set; } = "";

    [ForeignKey(nameof(CaptainId))]
    public ApplicationUser Captain { get; set; } = null!;

    public int SportId { get; set; }

    [ForeignKey(nameof(SportId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Sport Sport { get; set; } = null!;

    public IList<TeamInvitation> TeamInvitations { get; set; } = new List<TeamInvitation>();
    
    public IList<GameInvitation> GameInvitations { get; set; } = new List<GameInvitation>();

}