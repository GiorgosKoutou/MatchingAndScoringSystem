using System.ComponentModel.DataAnnotations;

namespace DomainModels.Models
{
    public class Service
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Domain { get; set; }

        [Required]
        public string SubDomain { get; set; }

        [Range(1,4)]
        public int MaturityStage { get; set; }
    }
}
