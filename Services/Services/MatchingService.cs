using DomainModels.Models;

namespace DomainServices.Services
{
    public class MatchingService
    {
        // Assumption: size thresholds derived from EmployeeCount for cost-profile matching.
        // These values can be adjusted without changing the matching logic.
        private int smallProviders = 15;
        private int smeProviders = 40;

        private readonly ProviderScoringService _providerScoringService;

        public MatchingService(ProviderScoringService providerScoringService)
        {
            _providerScoringService = providerScoringService;
        }

        /// <summary>
        /// Finds and ranks the top 3 providers for a matching request.
        /// The pipeline is: filter by required criteria, compute scores, sort with tie-breakers, and assign ranks.
        /// Returns fewer than 3 results if not enough providers match.
        /// </summary>
        public List<MatchingResult> FindTopProviders(MatchingRequest request,List<Provider> providerList)
        {
            // Fail-safe checks for null inputs
            ValidateInputs(request, providerList);

            // Early exit if no providers are available
            if (providerList.Count == 0)
                return new List<MatchingResult>();

            // Step 1: Filter providers based on strict criteria
            var matchedProviders = StrictMatchProviders(request, providerList);

            // Step 1b: Relaxed matching if no strict matches found
            if (matchedProviders.Count == 0)
            {
                // Relaxed matching if no strict matches found
                matchedProviders = ServiceAndCapacityFilter(request, providerList);
            }

            // Step 2: Score the matched providers
            var scoredProviders = ScoreProviders(matchedProviders);

            // Step 3: Apply sorting criteria to the providers
            var finalProviders = SortProvidersByPriority(scoredProviders, request);


            // Step 4: Take the top 3 providers
            var topProviders = TopRankedProviders(finalProviders);


            return topProviders;

        }


        #region Private Helper Methods

        /// <summary>
        /// Performs strict matching by applying all  matching criteria.
        /// Providers must support the requested service and user capacity,
        /// match the requestor's cost profile and digital maturity level,
        /// and (if required) be in the same location.
        /// </summary>
        private List<Provider> StrictMatchProviders(MatchingRequest serviceRequest, List<Provider> providerList)
        {
            // First, filter by service and capacity
            var strictMatches = ServiceAndCapacityFilter(serviceRequest, providerList);

            // Then apply additional strict criteria
            strictMatches = strictMatches.Where(p => CostProfileCriteria(p, serviceRequest))
                                .Where(p => DigitalMaturityCriteria(p, serviceRequest))
                                .ToList();

            // Finally, apply location proximity if required
            if (serviceRequest.LocationProximityRequired)
            {
                strictMatches = strictMatches
                                    .Where(p => LocationProximityCriteria(p, serviceRequest))
                                    .ToList();
            }

            return strictMatches;
        }

        /// <summary>
        /// Filters providers that offer the requested service and can satisfy the user capacity (if provided).
        /// Capacity is considered unlimited when MaxUsersSupported is null.
        /// </summary>
        private List<Provider> ServiceAndCapacityFilter(MatchingRequest serviceRequest, List<Provider> providerList)
        {
            // Filter providers that offer the requested service and can satisfy the user capacity.
            return providerList.Where(p => p.Skills
                                    .Any(ps => ps.Service.Name == serviceRequest.Service.Name &&
                                           (
                                               !serviceRequest.NumberOfUsers.HasValue
                                               || ps.MaxUsersSupported == null
                                               || ps.MaxUsersSupported >= serviceRequest.NumberOfUsers.Value
                                            )
                                         )
                                    ).ToList();
        }

        

        /// <summary>
        /// Checks whether a provider "fits" the requestor's cost profile using provider size (employee count).
        /// This returns a boolean used as a secondary sorting criterion (relaxed matching).
        /// </summary>
        private bool CostProfileCriteria(Provider provider, MatchingRequest costRequest)
        {
            return costRequest.Requestor.CostProfile switch
            {
                // Very Small and Small Providers: 1-15 employees
                DomainModels.Enums.CostProfile.Low => provider.EmployeeCount <= smallProviders,

                // Small and SME Providers: 16-40 employees
                DomainModels.Enums.CostProfile.Medium => provider.EmployeeCount > smallProviders && provider.EmployeeCount <= smeProviders,

                // SME and Big Providers: 40+ employees    
                DomainModels.Enums.CostProfile.High => provider.EmployeeCount > smeProviders,

                _ => false
            };
        }

        /// <summary>
        /// Validates digital maturity compatibility by comparing the requestor maturity with the service maturity stage.
        /// Returns true when the provider offers the requested service at an adequate maturity level.
        /// </summary>
        private bool DigitalMaturityCriteria(Provider provider, MatchingRequest maturityRequest)
        {
            return provider.Skills.Any(p =>  p.Service.Name == maturityRequest.Service.Name && 
                                        p.Service.MaturityStage >= maturityRequest.Requestor.DigitalMaturityIndex);
        }

        /// <summary>
        /// Applies location proximity preference when required by the request.
        /// If proximity is not required, this criterion is neutral (returns true for all providers).
        /// </summary>
        private bool LocationProximityCriteria(Provider provider, MatchingRequest locationRequest)
        {
            if (locationRequest.LocationProximityRequired)
            {
                return provider.Location == locationRequest.Requestor.Location;
            }

            return true;
        }


        /// <summary>
        /// Builds MatchingResult objects by computing a matching score for each provider.
        /// The score is computed from provider attributes and (non-expired) certifications.
        /// </summary>
        private List<MatchingResult> ScoreProviders(List<Provider> providerlist)
        {
            return providerlist.Select(p => new MatchingResult
            {
                Provider = p,
                MatchingScore = _providerScoringService.CalculateProviderScore(p, GetCertifications(p)),
                Rank = 0
            }).ToList();
        }

        /// <summary>
        /// Extracts the provider's active certifications from the ProviderCertification relationship.
        /// Expired certifications are ignored based on ExpiryDate (if provided).
        /// </summary>
        private List<Certification> GetCertifications(Provider provider)
        {
            return provider.Certifications
                                .Where(pc => pc.ExpiryDate == null || pc.ExpiryDate > DateTime.UtcNow)
                                .Select(pc => pc.Certification)
                                .ToList();
        }

        /// <summary>
        /// Orders the scored providers according to the matching priority rules.
        /// Providers are sorted primarily by their matching score and secondarily by
        /// cost profile fit, digital maturity compatibility, and location proximity.
        /// </summary>
        private List<MatchingResult> SortProvidersByPriority(List<MatchingResult> scoredProviders, MatchingRequest request)
        {
            return scoredProviders
                        .OrderByDescending(mp => mp.MatchingScore)
                        .ThenByDescending(mp => CostProfileCriteria(mp.Provider, request))
                        .ThenByDescending(mp => DigitalMaturityCriteria(mp.Provider, request))
                        .ThenByDescending(mp => LocationProximityCriteria(mp.Provider, request))
                        .ToList();
        }

        /// <summary>
        /// Selects the top-ranked providers and assigns ranking positions.
        /// Returns up to three results, ordered by their existing priority,
        /// and assigns ranks starting from 1 based on their position in the list.
        /// </summary>
        private List<MatchingResult> TopRankedProviders(List<MatchingResult> result)
        {

            
            return result.Take(3)
                         .Select((mp, index) =>
                         {
                            mp.Rank = index + 1;
                            return mp;

                         }).ToList();
        }

        /// <summary>
        /// Validates the required inputs for the matching process.
        /// Throws an ArgumentNullException when mandatory arguments or nested request
        /// data are missing, enforcing a fail-fast behavior for invalid input.
        /// </summary>
        private void ValidateInputs(MatchingRequest request, List<Provider> providerList)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (providerList is null)
                throw new ArgumentNullException(nameof(providerList));

            if (request.Service is null)
                throw new ArgumentNullException(nameof(request.Service));

            if (request.Requestor is null)
                throw new ArgumentNullException(nameof(request.Requestor));
        }
        #endregion
    }
}
