using Microsoft.Net.Http.Headers;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique =true)] //hacemos q el name seam único y no pueda haber dos iguales

public class Game
{
    //NUMERIC PRIMARY KEY
    [Key]
    public int Id{get;set;}

    //place
    [StringLength(256, MinimumLength =5)]
    public string Name{get;set;} = "";

    private string Description{get;set;}="";
}