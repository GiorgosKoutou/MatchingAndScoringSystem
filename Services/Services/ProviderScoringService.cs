using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainServices.Services
{
    public class ProviderScoringService
    {

        /// <summary>
        /// Calculates an overall score for a provider by aggregating multiple independent scoring factors.
        /// Each factor is computed separately, null values are ignored, and the final score is the average
        /// of all available factors. Returns 0 when no factors can be evaluated.
        /// </summary>
        public decimal CalculateProviderScore(DomainModels.Models.Provider provider, List<DomainModels.Models.Certification> certifications)
        {
            // Compute each discrete factor, allowing helpers to return null when input is unknown.
            var scores = new List<decimal?>
            {
                CalculateAssessmentScoreFactor(provider.AssessmentScore),
                CalculateRecencyFactor(provider.LastActivityDate),
                CalculateFrequencyFactor(provider.ProjectCount),
                CalculateMonetaryValueFactor(provider.AverageProjectValue),
                CalculateCertificationFactor(certifications)
            };

            // Filter out nulls and take the only the valid scores for averaging.
            var validScores = scores.Where(s => s.HasValue).Select(s => s.Value).ToList();

            // If no valid factors were provided, return 0 to indicate score cannot be computed.
            if (validScores.Count == 0)
                return 0m;

            // Return the average of the valid scores as the overall provider score.
            return validScores.Average();
        }


        #region Private Helper Methods

        /// <summary>
        /// Computes a certification-based factor.
        /// Returns null when certification data is unavailable, 1 when no certifications exist,
        /// and 9 when at least one certification is present.
        /// </summary>
        private decimal? CalculateCertificationFactor(List<DomainModels.Models.Certification>? certifications)
        { 
            if (certifications is null)
                return null;

            if (!certifications.Any())
                return 1m;

            return 9m;
        }

        /// <summary>
        /// Converts a provider assessment score into a discrete scoring factor.
        /// Low, medium, and high score ranges are mapped to increasing weights.
        /// Returns null when the assessment score is unknown.
        /// </summary>
        private decimal? CalculateAssessmentScoreFactor(decimal? assessmentScore)
        {
            if (!assessmentScore.HasValue)
                return null;

            if (assessmentScore < 2.26m)
                return 1m;

            if (assessmentScore < 3.76m)
                return 3m;

            return 9m;
        }


        /// <summary>
        /// Calculates a recency factor based on the number of months since the provider's last activity.
        /// More recent activity yields a higher score, while older activity yields lower weighting.
        /// Returns null when the activity date is unknown.
        /// </summary>
        private decimal? CalculateRecencyFactor(DateTime? lastActivityDate)
        {
            if (lastActivityDate is null)
                return null;

            var monthsSinceActivity = ((DateTime.UtcNow.Year - lastActivityDate.Value.Year) * 12)
                                        + DateTime.UtcNow.Month - lastActivityDate.Value.Month;

            if (DateTime.UtcNow.Day < lastActivityDate.Value.Day)
                monthsSinceActivity--;

            if (monthsSinceActivity < 6)
                return 6m;

            if (monthsSinceActivity >= 6 && monthsSinceActivity <= 12)
                return 3m;

            return 1m;
        }

        /// <summary>
        /// Maps the provider's total number of completed projects to a frequency factor.
        /// Higher project volume results in a higher score contribution.
        /// Returns null when project count is unavailable.
        /// </summary>
        private decimal? CalculateFrequencyFactor(int? projectCount)
        {
            if (!projectCount.HasValue)
                return null;

            if (projectCount < 24)
                return 1m;

            if (projectCount >= 24 && projectCount <= 48)
                return 6m;

            return 12m;
        }

        /// <summary>
        /// Converts the provider's average project value into a discrete monetary factor.
        /// Higher average project values contribute a higher weighting to the final score.
        /// Returns null when the monetary value is unknown.
        /// </summary>
        private decimal? CalculateMonetaryValueFactor(decimal? averageProjectValue)
        {
            if(!averageProjectValue.HasValue)
                return null;

            if (averageProjectValue < 100000m)
                return 1m;

            if (averageProjectValue >= 100000m && averageProjectValue <= 250000m)
                return 3m;

            return 6m;
        }
        #endregion
    }
}
