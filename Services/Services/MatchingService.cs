using DomainModels.Enums;
using DomainModels.Models;

namespace DomainServices.Services
{
    public class MatchingService
    {
        // Assumption: size thresholds derived from EmployeeCount for cost-profile matching.
        private int smallProviders = 15;
        private int smeProviders = 40;

        private List<DomainModels.Models.Provider> matchedProviders = [];

        public List<DomainModels.Models.MatchingResult> FindTopProviders(DomainModels.Models.MatchingRequest request,
                                                                               List<DomainModels.Models.Provider> providerList)
        {

            // Implementation goes here
            throw new NotImplementedException();
        }

        /**
        Filters the provided list of providers to those that offer the requested service.
        The method updates the internal `matchedProviders` collection to include only providers whose `Skills`
        contain a `Service` whose `Name` matches the `serviceRequest.Service.Name`.
        This is the initial narrowing step in the matching pipeline.
       */
        private void ServiceFilter(DomainModels.Models.MatchingRequest serviceRequest,
                                        List<DomainModels.Models.Provider> providerList)
        {
            matchedProviders = providerList
                                     .Where(p => p.Skills
                                     .Any(
                                            ps => ps.Service.Name == serviceRequest.Service.Name)

                                         ).ToList(); 
        }

        private void UserCapacityFilter(DomainModels.Models.MatchingRequest capacityRequest)
        {
            matchedProviders = matchedProviders
                                .Where(mp => mp.Skills
                                    .Any(  mps => mps.Service.Name == capacityRequest.Service.Name &&
                                           (
                                            !capacityRequest.NumberOfUsers.HasValue
                                            || mps.MaxUsersSupported == null
                                            || mps.MaxUsersSupported >= capacityRequest.NumberOfUsers
                                           )
                                         )
                                       ).ToList();
        }

        private void CostProfileFilter(DomainModels.Models.MatchingRequest costRequest)
        {
            matchedProviders = costRequest.Requestor.CostProfile switch
            {
                // Very Small and Small Providers: 1-15 employees
                DomainModels.Enums.CostProfile.Low => matchedProviders
                                                        .Where(mp => mp.EmployeeCount <= smallProviders)
                                                        .ToList(),
                // Small and SME Providers: 16-40 employees
                DomainModels.Enums.CostProfile.Medium => matchedProviders
                                                        .Where(mp => mp.EmployeeCount > smallProviders && mp.EmployeeCount <= smeProviders)
                                                        .ToList(),
                // SME and Big Providers: 40+ employees    
                DomainModels.Enums.CostProfile.High => matchedProviders
                                                        .Where(mp => mp.EmployeeCount > smeProviders)
                                                        .ToList(),
                _ => matchedProviders
            };
        }

        private void DigitalMaturityFilter(DomainModels.Models.MatchingRequest maturityRequest)
        {
            matchedProviders = matchedProviders
                                    .Where(mp => mp.Skills
                                    .Any(
                                          mps => mps.Service.Name == maturityRequest.Service.Name &&
                                          mps.Service.MaturityStage >= maturityRequest.Requestor.DigitalMaturityIndex
                                        )
                                    ).ToList();
        }

        private void LocationProximityFilter(DomainModels.Models.MatchingRequest locationRequest)
        {
            if (locationRequest.LocationProximityRequired)
            {
                matchedProviders = matchedProviders
                                     .OrderByDescending(p => p.Location == locationRequest.Requestor.Location).ToList();
            }
        }
    }
}
