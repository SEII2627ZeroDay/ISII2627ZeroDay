using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(RefereeGroupId), nameof(RefereeId))]
public class RefereeAssignedTo
{
    [Required]
    public int RefereeGroupId { get; set; }

    [Required]
    public string RefereeId { get; set; } = "0";

    public bool AcceptedAssignement { get; set; } = false;

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Role { get; set; } = "Name of the Role has to be between 3 and 20 characters";

    [Required]
    [StringLength(255, MinimumLength = 3)]
    public string RoleDescription { get; set; } = " Between 3 and 255 chars";

    [ForeignKey(nameof(RefereeGroupId))]
    public RefereeGroup RefereeGroup { get; set; } = null!;

    [ForeignKey(nameof(RefereeId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Referee Referee { get; set; } = null!;
}