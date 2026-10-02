using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique =true)] //hacemos q el name seam único y no pueda haber dos iguales

public class Game
{
    //id
    //NUMERIC PRIMARY KEY
    [Key]
    public int Id{get;set;}

    //name
    [Required]
    public string Name{get;set;} = "";

    //date
    [Required]
    public DateTime Date{get;set;}

    //place
    [Required]
    [StringLength(256, MinimumLength =5)]
    public string Place{get;set;} = "More than 5 chars";

    //description
    public string? Description{get;set;}="";

    //Realtion Team (ResponsibleFor)
    [Required]
    public string ResponsibleForId { get; set; } = "";
    [ForeignKey(nameof(ResponsibleForId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser ResponsibleFor { get; set; } = null!;

    //Relation Sport
    [Required]
    public int SportId { get; set; }
    [ForeignKey(nameof(SportId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Sport Sport { get; set; } = null!;

    //Relation GameInvitation
    public IList<GameInvitation> GameInvitations { get; set; } = new List<GameInvitation>();
}