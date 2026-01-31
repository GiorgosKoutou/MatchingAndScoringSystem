using DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class Requestor
    {
        [Required]
        public string CompanyName { get; set; }
        public decimal Revenue { get; set; }
        public int EmployeeCount { get; set; }
        public CostProfile CostProfile { get; set; }

        [Range(1,4)]
        public int DigitalMaturityIndex { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}
