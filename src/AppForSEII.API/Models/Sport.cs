using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Sport
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = "Game";

    public int MinimumNumberOfPlayers { get; set; } = 2;

    public int NumberofReferees { get; set; } = 1;

    public string Description { get; set; } = "Description";

    public string BasicRules { get; set; } = "Rules for the game";

    public IList<Team> Teams { get; set; } = new List<Team>();

    public IList<InterestedIn> Interested { get; set; } = new List<InterestedIn>();
    public IList<Game> Games { get; set; } = new List<Game>();
     public IList<Referee> Referees { get; set; }= new List<Referee>();
}