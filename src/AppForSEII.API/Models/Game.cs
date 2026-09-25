using Microsoft.Net.Http.Headers;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique =true)] //hacemos q el name seam único y no pueda haber dos iguales

public class Game
{
    //NUMERIC PRIMARY KEY
    [Key]
    public int Id{get;set;}

    public string Name{get;set;} = "";
}