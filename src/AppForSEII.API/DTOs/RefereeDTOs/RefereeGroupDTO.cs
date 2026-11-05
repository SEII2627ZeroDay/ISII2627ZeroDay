using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeGroupDTO
    {
        public RefereeGroupDTO(int id, string name, string? description, string? rules, int gameId, List<RefereeAssignedToDTO> refereeAssignedTos)
        {
            Id = id;
            Name = name;
            Description = description;
            Rules = rules;
            GameId = gameId;
            RefereeAssignedTos = refereeAssignedTos;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Name between 3 and 10 chars")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Rules { get; set; }

        [Range(1, int.MaxValue)]
        public int GameId { get; set; }

        [Required]
        public List<RefereeAssignedToDTO> RefereeAssignedTos { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RefereeGroupDTO other &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Description == other.Description &&
                   Rules == other.Rules &&
                   GameId == other.GameId &&
                   RefereeAssignedTos.SequenceEqual(other.RefereeAssignedTos);
        }

        // The list is left out of the hash on purpose: two equal DTOs still get the same hash code.
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, Rules, GameId);
        }
    }
}