namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeforgroupDTO
    {
        public RefereeforgroupDTO(string id, int sportId, int numberofReferees, string name, string? surname, int yearsRefereeing, int rating)
        {
            Id = id;
            SportId = sportId;
            NumberofReferees = numberofReferees;
            Name = name;
            Surname = surname;
            YearsRefereeing = yearsRefereeing;
            Rating = rating;
        }

        public string Id { get; set; } 
    public int SportId { get; set; }

    public int NumberofReferees { get; set; } = 1;
     [StringLength(20, MinimumLength = 3)]
    public string Name { get; set; } = "Name between 3 and 20 chars";
    [StringLength(50)]
    public string? Surname { get; set; }
    [Range(1, int.MaxValue)]
    public int YearsRefereeing { get; set; }
    [Range(0, 5)]
    public int Rating { get; set; }




   



    

    }
}