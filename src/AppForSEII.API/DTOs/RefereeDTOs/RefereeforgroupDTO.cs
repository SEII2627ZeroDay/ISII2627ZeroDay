namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeforgroupDTO
    {
        public int RefereeId { get; set; }
        public int RefereeGroupId { get; set; }

      [Range(0, 5)]
    public int Rating { get; set; }

    [Range(1, int.MaxValue)]
    public int YearsRefereeing { get; set; }

    public int SportId { get; set; }
    public string Id { get; set; } 
      [StringLength(20, MinimumLength = 3)]
    public string Name { get; set; } = "Name between 3 and 20 chars";

    }
}