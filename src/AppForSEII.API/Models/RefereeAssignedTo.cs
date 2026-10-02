public class RefereeAssignedTo
{
    [Key]
public int RefereeGroupId {get; set;}
public string RefereeId {get; set;} ="0";
public bool AcceptedAssignement {get; set;}=false;
[StringLength(20, MinimumLength = 3)]
public string Role {get; set;} ="Name of the role has to be between 3 and 20 characters";
[StringLength(255, MinimumLength = 3)]
public string RoleDescription {get; set;} ="Between 3 and 255 chars";
public RefereeGroup RefereeGroup { get; set; } = null!;
public Referee Referee { get; set; } = null!;

}