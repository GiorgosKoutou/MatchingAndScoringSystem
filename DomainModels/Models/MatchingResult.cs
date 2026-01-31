using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class MatchingResult
    {
        public Provider Provider { get; set; }
        public decimal MatchingScore { get; set; }

        [Range(1,3)]
        public int Rank { get; set; }
    }
}
