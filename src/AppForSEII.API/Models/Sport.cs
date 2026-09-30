using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Sport
{
    public Sport()
    {
    }

    public Sport(string name, int minimumNumberOfPlayers, int numberofReferees, string description, string basicRules)
    {
        Name = name;
        MinimumNumberOfPlayers = minimumNumberOfPlayers;
        NumberofReferees = numberofReferees;
        Description = description;
        BasicRules = basicRules;
    }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = "Game";

    public int MinimumNumberOfPlayers { get; set; } = 2;

    public int NumberofReferees { get; set; } = 1;

    public string Description { get; set; } = "Description";

    public string BasicRules { get; set; } = "Rules for the game";

    [InverseProperty("Sport")]
    public IList<Team> Teams { get; set; } = new List<Team>();

    [InverseProperty("Sport")]
    public IList<InterestedIn> Interested { get; set; } = new List<InterestedIn>();
}