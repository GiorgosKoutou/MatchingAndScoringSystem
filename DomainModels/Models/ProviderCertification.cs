
namespace DomainModels.Models
{
    public class ProviderCertification
    {
        public Provider Provider { get; set; } = null!;
        public Certification Certification { get; set; } = null!;   
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
