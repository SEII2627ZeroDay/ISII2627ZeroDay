using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]

public class Team
{
    [Key]
    public int Id { get; set; }

    [StringLength(10,MinimumLength = 3)]
    public string Name { get; set; } = "Team Name";
    public string Description { get; set; } = "Team Name";
    // new attribute of application user and gender
    
}