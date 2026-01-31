using System.ComponentModel.DataAnnotations;


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
