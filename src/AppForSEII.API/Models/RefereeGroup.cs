using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class RefereeGroup
{
    [Key]
    public int Id { get; set; }

    [StringLength(20, MinimumLength = 3)]
    public string Name { get; set; } = "Name between 3 and 20 chars";

    public string Description { get; set; } = "Description";

    public string Rules { get; set; } = "Rules for the game";

    public int GameId { get; set; }
    public IList<RefereeAssignedTo> RefereeAssignments { get; set; } = new List<RefereeAssignedTo>();
    
}