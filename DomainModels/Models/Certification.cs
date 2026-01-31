using System.ComponentModel.DataAnnotations;

namespace DomainModels.Models
{
    public class Certification
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string IssuingOrganization { get; set; }
    }
}
