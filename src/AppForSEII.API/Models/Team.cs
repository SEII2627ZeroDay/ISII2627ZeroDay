using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]

public class Team
{
    [Key]
    public int Id { get; set; }


    public string Name { get; set; } = "Team name";
}