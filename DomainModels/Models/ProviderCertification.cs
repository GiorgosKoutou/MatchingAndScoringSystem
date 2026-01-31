
namespace DomainModels.Models
{
    public class ProviderCertification
    {
        public Provider Provider { get; set; }
        public Certification Certification { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
