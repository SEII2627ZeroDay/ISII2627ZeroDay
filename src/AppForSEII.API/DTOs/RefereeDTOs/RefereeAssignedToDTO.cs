using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeAssignedToDTO
    {
        public RefereeAssignedToDTO(int refereeGroupId, string refereeId, bool acceptedAssignement, string role, string roleDescription)
        {
            RefereeGroupId = refereeGroupId;
            RefereeId = refereeId;
            AcceptedAssignement = acceptedAssignement;
            Role = role;
            RoleDescription = roleDescription;
        }

        public int RefereeGroupId { get; set; }

        [Required]
        public string RefereeId { get; set; }

        public bool AcceptedAssignement { get; set; } = false;

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name of the Role has to be between 3 and 20 characters")]
        public string Role { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Between 3 and 255 chars")]
        public string RoleDescription { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RefereeAssignedToDTO other &&
                   RefereeGroupId == other.RefereeGroupId &&
                   RefereeId == other.RefereeId &&
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