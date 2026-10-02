public class Referee
{
public int Rating {get; set;}
public int YearsRefereeing {get; set;}
public IList<RefereeAssignedTo> Assignments { get; set; } = new List<RefereeAssignedTo>();
public IList<Sport> Sports { get; set; }= new List<Sport>();

}