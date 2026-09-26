using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]

public class Team
{
    [Key]
    public int Id { get; set; }

    [StringLength(10, MinimumLength = 3)]
    public string Name { get; set; } = "Team name";

    public string? Description { get; set; } = "Team name";

    public int MaxMembers { get; set; }

    public int MinAge { get; set; } = 3;

    public int MaxAge { get; set; } = 3;
}