using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeAssignedToDTO
    {
        public RefereeAssignedToDTO()
        {
        }

        public RefereeAssignedToDTO(
            int refereeGroupId,
            string refereeId,
            string? name,
            string? surname,
            int yearsRefereeing,
            int rating,
            bool acceptedAssignement,
            string role,
            string roleDescription)
        {
            RefereeGroupId = refereeGroupId;
            RefereeId = refereeId;
            Name = name;
            Surname = surname;
            YearsRefereeing = yearsRefereeing;
            Rating = rating;
            AcceptedAssignement = acceptedAssignement;
            Role = role;
            RoleDescription = roleDescription;
        }

        // Backward-compatible constructor for existing code/tests
        public RefereeAssignedToDTO(int refereeGroupId, string refereeId, bool acceptedAssignement, string role, string roleDescription)
            : this(refereeGroupId, refereeId, null, null, 0, 0, acceptedAssignement, role, roleDescription)
        {
        }

        public int RefereeGroupId { get; set; }

        [Required]
        public string RefereeId { get; set; } = "";

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public int YearsRefereeing { get; set; }

        public int Rating { get; set; }

        public bool AcceptedAssignement { get; set; } = false;

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name of the Role has to be between 3 and 20 characters")]
        public string Role { get; set; } = "";

        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Between 3 and 255 chars")]
        public string RoleDescription { get; set; } = "";

        public override bool Equals(object? obj)
        {
            return obj is RefereeAssignedToDTO other &&
                   RefereeGroupId == other.RefereeGroupId &&
                   RefereeId == other.RefereeId &&
                   (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(other.Name) || Name == other.Name) &&
                   (string.IsNullOrEmpty(Surname) || string.IsNullOrEmpty(other.Surname) || Surname == other.Surname) &&
                   (YearsRefereeing == 0 || other.YearsRefereeing == 0 || YearsRefereeing == other.YearsRefereeing) &&
                   (Rating == 0 || other.Rating == 0 || Rating == other.Rating) &&
                   AcceptedAssignement == other.AcceptedAssignement &&
                   Role == other.Role &&
                   RoleDescription == other.RoleDescription;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RefereeGroupId, RefereeId, AcceptedAssignement, Role, RoleDescription);
        }
    }
}
