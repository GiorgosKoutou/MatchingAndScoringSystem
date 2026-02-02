using DomainModels.Models;
using DomainModels.Enums;
using DomainServices.Services;


namespace MatchingAndScoringSystem.UnitTests.ServiceTests
{
    [TestFixture]
    public class MatchingServiceTests
    {
        private List<Provider> providerList;

        [SetUp]
        public void ProviderListSetUp()
        {
            providerList =
            [

               // First Provider
               new Provider
                {
                    CompanyName = "Provider A",
                    AssessmentScore = 3.8m,
                    LastActivityDate = DateTime.UtcNow.AddMonths(-10),
                    ProjectCount = 15,
                    AverageProjectValue = 200000m,
                    EmployeeCount = 25,
                    Location = "Athens",
                    Certifications = 
                    [
                        new ProviderCertification
                        {
                            Certification = new Certification { Name = "CertA" },
                        }
                    ],
                    Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "Cloud Hosting" , MaturityStage = 3},
                            MaxUsersSupported = 55,
                        }
                    ]
                },
                // Second Provider
                new Provider
                {
                    CompanyName = "Provider B",
                    AssessmentScore = 5.0m,
                    LastActivityDate = DateTime.UtcNow.AddMonths(-5),
                    ProjectCount = 35,
                    AverageProjectValue = 500000m,
                    EmployeeCount = 27,
                    Location = "Athens",
                    Certifications =
                    [
                        new ProviderCertification
                        {
                            Certification = new Certification { Name = "CertB" },
                        }
                    ],
                    Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "Cloud Hosting", MaturityStage = 3 },
                            MaxUsersSupported = 60,
                        }
                    ]
                },
                // Third Provider
                 new Provider
                {
                    CompanyName = "Provider C",
                    AssessmentScore = 1.5m,
                    LastActivityDate = DateTime.UtcNow.AddMonths(-20),
                    ProjectCount = 5,
                    AverageProjectValue = 99000m,
                    EmployeeCount = 40,
                    Location = "Athens",
                    Certifications = [],
                    Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "Cloud Hosting", MaturityStage = 4 },
                            MaxUsersSupported = 65,
                        }
                    ]
                },
                 // Fourth Provider
                  new Provider
                  {
                      CompanyName = "Provider D",
                      AssessmentScore = 4.8m,
                      LastActivityDate = DateTime.UtcNow.AddMonths(-2),
                      ProjectCount = 50,
                      AverageProjectValue = 750000m,
                      EmployeeCount = 60,
                      Location = "Thessaloniki",
                      Certifications =
                      [
                            new ProviderCertification
                            {
                                Certification = new Certification { Name = "CertC" },
                            }
                      ],
                      Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "Cloud Hosting", MaturityStage = 1 },
                            MaxUsersSupported = 100,
                        }
                    ]
                  },
                  // Fifth Provider
                   new Provider
                   {
                       CompanyName = "Provider E",
                       AssessmentScore = 2.3m,
                       LastActivityDate = DateTime.UtcNow.AddMonths(-15),
                       ProjectCount = 8,
                       AverageProjectValue = 120000m,
                       EmployeeCount = 5,
                       Location = "Thessaloniki",
                       Certifications = [],
                       Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "Cloud Hosting", MaturityStage = 1 },
                            MaxUsersSupported = 100,
                        }
                    ]
                   }
            ];

        }

        [Test]
        public void FindTopProviders_WithExactMatches_ReturnMatchingResultList()
        {
            // Arrange
            MatchingRequest matchingRequest = new()
            {
                Requestor = new Requestor
                {
                    CostProfile = CostProfile.Medium,
                    DigitalMaturityIndex = 3,
                    Location = "Athens"
                },
                Service = new Service
                {
                    Name = "Cloud Hosting"
                },
                NumberOfUsers = 50,
                LocationProximityRequired = true
            };

            var scoringService = new ProviderScoringService();

            var matchingService = new MatchingService(scoringService);

            // Act
            var resultList = matchingService.FindTopProviders(matchingRequest, providerList);

            // Assert
            Assert.That(resultList.Count(), Is.EqualTo(3));
            Assert.That(resultList.Select(rl => rl.Provider.CompanyName),
                                  Is.EqualTo(new[] { "Provider B", "Provider A", "Provider C" }));
        }

        [Test]
        public void FindTopProviders_NoProvidersMatch_ReturnEmptyList()
        {
            // Arrange
            MatchingRequest matchingRequest = new()
            {
                Requestor = new Requestor
                {
                    CostProfile = CostProfile.Medium,
                    DigitalMaturityIndex = 3,
                    Location = "Athens"
                },
                Service = new Service
                {
                    Name = "WebSite"
                },
                NumberOfUsers = 1000,
                LocationProximityRequired = true
            };
            var scoringService = new ProviderScoringService();
            var matchingService = new MatchingService(scoringService);

            // Act
            var resultList = matchingService.FindTopProviders(matchingRequest, providerList);

            // Assert
            Assert.That(resultList, Is.Empty);
        }

        [Test]
        public void FindTopProviders_WithFewerThanThreeMatches_ReturnMatchingResultList()
        {
            // Arrange
            MatchingRequest matchingRequest = new()
            {
                Requestor = new Requestor
                {
                    CostProfile = CostProfile.Medium,
                    DigitalMaturityIndex = 3,
                    Location = "Thessaloniki"
                },
                Service = new Service
                {
                    Name = "WebSite"
                },
                NumberOfUsers = 100,
                LocationProximityRequired = true
            };

            var newProviderList = providerList =
            [

               // First Provider
               new Provider
                {
                    CompanyName = "Provider A",
                    AssessmentScore = 3.8m,
                    LastActivityDate = DateTime.UtcNow.AddMonths(-10),
                    ProjectCount = 15,
                    AverageProjectValue = 200000m,
                    EmployeeCount = 7,
                    Location = "Athens",
                    Certifications =
                    [
                        new ProviderCertification
                        {
                            Certification = new Certification { Name = "CertA" },
                        }
                    ],
                    Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "WebSite" , MaturityStage = 3},
                            MaxUsersSupported = 100,
                        }
                    ]
                },
                // Second Provider
                new Provider
                {
                    CompanyName = "Provider B",
                    AssessmentScore = 5.0m,
                    LastActivityDate = DateTime.UtcNow.AddMonths(-5),
                    ProjectCount = 35,
                    AverageProjectValue = 500000m,
                    EmployeeCount = 8,
                    Location = "Athens",
                    Certifications =
                    [
                        new ProviderCertification
                        {
                            Certification = new Certification { Name = "CertB" },
                        }
                    ],
                    Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "WebSite", MaturityStage = 3 },
                            MaxUsersSupported = 100,
                        }
                    ]
                },
                // Third Provider
                 new Provider
                {
                    CompanyName = "Provider C",
                    AssessmentScore = 1.5m,
                    LastActivityDate = DateTime.UtcNow.AddMonths(-20),
                    ProjectCount = 5,
                    AverageProjectValue = 99000m,
                    EmployeeCount = 40,
                    Location = "Athens",
                    Certifications = [],
                    Skills =
                    [
                        new ProviderSkill
                        {
                            Service = new Service { Name = "Cloud Hosting", MaturityStage = 4 },
                            MaxUsersSupported = 65,
                        }
                    ]
                }
            ];

            var scoringService = new ProviderScoringService();

            var matchingService = new MatchingService(scoringService);

            // Act
            var resultList = matchingService.FindTopProviders(matchingRequest, newProviderList);

            // Assert
            Assert.That(resultList.Count(), Is.EqualTo(2));
            Assert.That(resultList.Select(rs => rs.Provider.CompanyName),
                                        Is.EqualTo(new[] { "Provider B", "Provider A" }));  

        }

        [Test]
        public void FindTopProviders_TestProperRanking_ReturnMatchingResultListWithProperRanking()
        {
            // Arrange 
            MatchingRequest matchingRequest = new()
            {
                Requestor = new Requestor
                {
                    CostProfile = CostProfile.Medium,
                    DigitalMaturityIndex = 3,
                    Location = "Athens"
                },
                Service = new Service
                {
                    Name = "Cloud Hosting"
                },
                NumberOfUsers = 50,
                LocationProximityRequired = true
            };

            var scoringService = new ProviderScoringService();

            var matchingService = new MatchingService(scoringService);
            // Act
            var resultList = matchingService.FindTopProviders(matchingRequest, providerList);

            // Assert
            Assert.That(resultList.Count(), Is.EqualTo(3));
            Assert.That(resultList.Select(rs => rs.Rank), Is.EqualTo(new [] {1, 2, 3 }));
        }

        [Test]
        public void FindTopProviders_WithFilterByUserCapcity_ReturnMatchingResultList()
        {
            // Arrange
            MatchingRequest matchingRequest = new()
            {
                Requestor = new Requestor
                {
                    CostProfile = CostProfile.Medium,
                    DigitalMaturityIndex = 1,
                    Location = "Drama"
                },
                Service = new Service
                {
                    Name = "Cloud Hosting"
                },
                NumberOfUsers = 100,
                LocationProximityRequired = false
            };

            var scoringService = new ProviderScoringService();

            var matchingService = new MatchingService(scoringService);

            // Act

            var resultList = matchingService.FindTopProviders(matchingRequest, providerList);

            // Assert
            Assert.That(resultList.Count(), Is.EqualTo(2));
            Assert.That(resultList.Select(rs => rs.Provider.CompanyName),
                                           Is.EqualTo(new [] {"Provider D", "Provider E"}));
        }

        [Test]
        public void FindTopProviders_WithCostProfilMatching_ReturnMatchingResultList()
        {
            // Arrange
            MatchingRequest matchingRequest = new()
            {
                Requestor = new Requestor
                {
                    CostProfile = CostProfile.High,
                    DigitalMaturityIndex = 1,
                    Location = "Drama"
                },
                Service = new Service
                {
                    Name = "Cloud Hosting"
                },
                NumberOfUsers = 100,
                LocationProximityRequired = false
            };

            var scoringService = new ProviderScoringService();

            var matchingService = new MatchingService(scoringService);

            // Act
            var resultList = matchingService.FindTopProviders(matchingRequest, providerList);

            // Assert
            Assert.That(resultList.Count(), Is.EqualTo(1));
            Assert.That(resultList.Select(rs => rs.Provider.CompanyName),
                                           Is.EqualTo(new[] { "Provider D" }));
        }

      
    }
}
