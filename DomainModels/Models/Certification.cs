using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class Certification
    {
        [Required]
        public string CertificationName { get; set; }

        [Required]
        public string IssuingOrganization { get; set; }
    }
}
