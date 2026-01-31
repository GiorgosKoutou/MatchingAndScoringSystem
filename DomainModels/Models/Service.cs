using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class Service
    {
        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string Domain { get; set; }

        [Required]
        public string SubDomain { get; set; }

        [Range(1,4)]
        public int MaturityStage { get; set; }
    }
}
