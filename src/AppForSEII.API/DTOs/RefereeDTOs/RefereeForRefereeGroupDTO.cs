namespace AppForSEII.API.DTOs.RefereeDTOs
{
    public class RefereeForRefereeGroupDTO
    {
        public RefereeForRefereeGroupDTO(string id, string? name, string? surname, int yearsRefereeing, int rating)
        {
            Id = id;
            Name = name;
            Surname = surname;
            YearsRefereeing = yearsRefereeing;
            Rating = rating;
        }

        public string Id { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public int YearsRefereeing { get; set; }

        public int Rating { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RefereeForRefereeGroupDTO other &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Surname == other.Surname &&
                   YearsRefereeing == other.YearsRefereeing &&
                   Rating == other.Rating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, YearsRefereeing, Rating);
        }
    }
}