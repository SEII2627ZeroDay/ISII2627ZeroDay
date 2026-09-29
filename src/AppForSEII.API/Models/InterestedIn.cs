using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class InterestedIn
{
    [Required]
    public int SportId { get; set; }

    [Required]
    public string UserId { get; set; } = "0";

    [Range(1, 5)]
    public int Skill { get; set; } = 5;
}