using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class ProviderScoringService
    {
        public decimal CalculateProviderScore(DomainModels.Models.Provider provider, List<DomainModels.Models.Certification?> certifications)
        {

            var scores = new List<decimal?>
            {
                CalculateAssessmentScoreFactor(provider.AssessmentScore),
                CalculateRecencyFactor(provider.LastActivityDate),
                CalculateFrequencyFactor(provider.ProjectCount),
                CalculateMonetaryValueFactor(provider.AverageProjectValue),
                CalculateCertificationFactor(certifications)
            };

            var validScores = scores.Where(s => s.HasValue).Select(s => s.Value).ToList();

            if (validScores.Count == 0)
                return 0m;

            return validScores.Average();
        }

        private decimal? CalculateCertificationFactor(List<DomainModels.Models.Certification?> certifications)
        {
            if(certifications is null)
                return null;

            if(!certifications.Any())
                return 1m;

            return 9m; 
        }


        private decimal? CalculateAssessmentScoreFactor(decimal? assessmentScore)
        {
            // If no assessment score provided, we cannot compute a factor => return null.
            if (!assessmentScore.HasValue)
                return null;

            // Map the numeric assessment score to a discrete factor:
            // 0 <= score < 2.26  => factor 1
            if (assessmentScore < 2.26m)
                return 1m;

            // 2.26 <= score < 3.76 => factor 3
            if (assessmentScore < 3.76m)
                return 3m;

            // 3.76 >= score <=5 => factor 9
            return 9m;
        }

        private decimal? CalculateRecencyFactor(DateTime? lastActivityDate)
        {
            if (lastActivityDate is null)
                return null;

            var monthsSinceActivity = ((DateTime.UtcNow.Year - lastActivityDate.Value.Year) * 12)
                                        + DateTime.UtcNow.Month - lastActivityDate.Value.Month;

            if(DateTime.UtcNow.Day < lastActivityDate.Value.Day)
                monthsSinceActivity--;

            if (monthsSinceActivity < 6)
                return 6m;

            if(monthsSinceActivity >= 6 && monthsSinceActivity < 12)
                return 3m;

            return 1m;
        }

        private decimal? CalculateFrequencyFactor(int? projectCount)
        {

            if(!projectCount.HasValue)
                return null;

            if (projectCount < 24)
                return 1m;

            if (projectCount >= 24 && projectCount < 48)
                return 6m;

            return 12m;
        }

        private decimal? CalculateMonetaryValueFactor(decimal? averageProjectValue)
        {

            if(!averageProjectValue.HasValue)
                return null;

            if (averageProjectValue < 100000m)
                return 1m;

            if (averageProjectValue >= 100000m && averageProjectValue < 250000m)
                return 3m;

            return 6m;
        }
    }
}
