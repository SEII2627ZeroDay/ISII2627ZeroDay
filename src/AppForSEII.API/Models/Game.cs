using Microsoft.Net.Http.Headers;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique =true)] //hacemos q el name seam único y no pueda haber dos iguales

public class Game
{
    //id
    //NUMERIC PRIMARY KEY
    [Key]
    public int Id{get;set;}

    //name
    public string Name{get;set;} = "";

    //date
    public DateTime Date{get;set;}

    //place
    [StringLength(256, MinimumLength =5)]
    public string Place{get;set;} = "More than 5 chars";

    //description
    private string? Description{get;set;}="";
}