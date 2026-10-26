namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeGroupDTO
    {
        public RefereeGroupDTO(int id, string name, string? description, string? rules, int gameId, List<RefereeAssignedToDTO>? refereeAssignedTos = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Rules = rules;
            GameId = gameId;
            RefereeAssignedTos = refereeAssignedTos ?? new List<RefereeAssignedToDTO>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name between 3 and 20 chars")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Rules { get; set; }

        [Required]
        public int GameId { get; set; }

        public List<RefereeAssignedToDTO> RefereeAssignedTos { get; set; }
    }
}