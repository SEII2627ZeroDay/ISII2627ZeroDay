using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Referee
{
    [Key]
    public string Id { get; set; } = "0";

    [Range(0, 5)]
    public int Rating { get; set; }

    [Range(1, int.MaxValue)]
    public int YearsRefereeing { get; set; }

    public int SportId { get; set; }

    [ForeignKey(nameof(SportId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Sport SportForReferee { get; set; } = null!;

    [ForeignKey(nameof(Id))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ApplicationUser User { get; set; } = null!;

    public IList<RefereeAssignedTo> RefereeAssignedTos { get; set; } = new List<RefereeAssignedTo>();
}