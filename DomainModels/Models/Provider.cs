using System.ComponentModel.DataAnnotations;


namespace DomainModels.Models
{
    public class Provider
    {
        [Required]
        public string CompanyName { get; set; }
        public int EmployeeCount { get; set; }
        public string Location { get; set; } = string.Empty;

        [Range(0.0,5.0)]
        public decimal? AssessmentScore { get; set; }
        public DateTime? LastActivityDate { get; set; }

        [Range(0,10000)]
        public int? ProjectCount { get; set; }

        [Range(0.0, 100000000.00)]
        public decimal? AverageProjectValue { get; set; }
        public decimal CurrentScore { get; set; }
    }
}
