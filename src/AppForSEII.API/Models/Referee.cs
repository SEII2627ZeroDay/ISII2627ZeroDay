using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Referee : ApplicationUser
{
    public Referee(int rating, int yearsRefereeing, int sportId)
    {
        Rating = rating;
        YearsRefereeing = yearsRefereeing;
        SportId = sportId;
    }

public Referee(string id, string name, string surname, string userName, int rating, int yearsRefereeing, int sportId)
       : base(id, name, surname, userName)
   {
       Rating = rating;
       YearsRefereeing = yearsRefereeing;
       SportId = sportId;
   }
   

    [Range(0, 5)]
    public int Rating { get; set; }

    [Range(1, int.MaxValue)]
    public int YearsRefereeing { get; set; }

    public int SportId { get; set; }

    [ForeignKey(nameof(SportId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Sport SportForReferee { get; set; } = null!;
    public IList<RefereeAssignedTo> RefereeAssignedTos { get; set; } = new List<RefereeAssignedTo>();
}