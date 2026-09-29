public class RefereeGroup
{
    [Key]
public int Id {get; set;}
[StringLength(20, MinimumLength = 3)]
public string Name {get; set;} ="Name between 3 and 20 chars";
public string Description {get; set;}
public string Rules {get; set;}
public int GameId {get; set;}
}