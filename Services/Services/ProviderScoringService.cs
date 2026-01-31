using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainServices.Services
{
    public class ProviderScoringService
    {

        /// Calculates an overall provider score by aggregating multiple discrete factor methods.
        /// The method invokes helper functions to compute factors for assessment score, recency, frequency,
        /// monetary value, and certifications, then ignores any unknown (null) factors and averages the rest.
        /// If no factors are available (all helpers returned null) the method returns 0m to indicate an uncomputable score.
        /// This produces a single decimal value representing the combined weighting of the provider's attributes.
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

        /// Determines a discrete certification factor based on the provided certifications list.
        /// If the input is null the factor cannot be computed and null is returned. If the list exists but is empty a baseline factor of 1 is returned. 
        /// If one or more certifications are present a higher factor of 9 is returned. This method encodes a simple ternary mapping: unknown -> null, none -> 1, any -> 9
        private decimal? CalculateCertificationFactor(List<DomainModels.Models.Certification>? certifications)
        { 
            if (certifications is null)
                return null;

            if (!certifications.Any())
                return 1m;

            return 9m;
        }

        /// Maps a nullable assessment score into a discrete factor used in provider scoring.
        /// If the input is null the factor cannot be computed and null is returned. For numeric values it returns 1 for low scores ( < 2.26), 3 for mid-range scores (2.26 > && < 3.76), and 9 for higher scores.
        /// This creates a simple three-tier mapping from a continuous assessment metric into a discrete weighting factor
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


        /// Calculates a recency factor from the provider's last activity date. 
        /// Returns null when the input date is unknown, otherwise it computes whole months elapsed since the activity (using UTC and adjusting for day-of-month) 
        /// and maps that duration to a discrete factor: less than 6 months -> 6, 6–12 months -> 3, and more than 12 months -> 1. 
        /// This factor is intended to be one component of the overall provider scoring calculation
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

        /// Calculates a discrete frequency factor from the provider's total project count.
        /// Returns null when the project count is unknown, otherwise it maps counts to three tiers: less than 24 projects yields a factor of 1, 
        /// 24–48 projects yields a factor of 6, and more than 48 projects yields a factor of 12. 
        /// This tiered factor is a component used in the provider's overall scoring computation
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

        /// Maps a nullable average project value into a discrete monetary factor used in provider scoring.
        /// If the input is null the factor cannot be computed and null is returned. 
        /// For numeric values it returns 1 for values below 100,000, 3 for values between 100,000 and 250,000 (inclusive), and 6 for values above 250,000.
        /// This discrete mapping simplifies a continuous monetary metric into three weighting tiers for use in the overall provider score calculation.
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
