
namespace DomainModels.Models
{
    public class MatchingRequest
    {
        public Requestor Requestor { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public int? NumberOfUsers { get; set; }
        public bool LocationProximityRequired { get; set; }
    }
}
