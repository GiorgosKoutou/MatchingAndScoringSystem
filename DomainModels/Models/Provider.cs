using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class Provider
    {
        [Required]
        public string CompanyName { get; set; }
        public int EmployeeCount { get; set; }
        public string Location { get; set; } = string.Empty;

        [Range(0,5)]
        public decimal AssessemtScore { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public int ProjectCount { get; set; }
        public decimal AverageProjectValue { get; set; }
        public decimal CurrentScore { get; set; }
    }
}
