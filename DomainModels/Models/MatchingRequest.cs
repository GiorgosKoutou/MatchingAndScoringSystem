
namespace DomainModels.Models
{
    public class MatchingRequest
    {
        public Requestor Requestor { get; set; }
        public Service Service { get; set; }
        public int? NumberOfUsers { get; set; }
        public bool LocationProximityRequired { get; set; }
    }
}
