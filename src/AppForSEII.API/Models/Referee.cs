using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class Referee
{
    [Key]
    public string Id { get; set; } = "0";

    public int Rating { get; set; }

    public int YearsRefereeing { get; set; }
}