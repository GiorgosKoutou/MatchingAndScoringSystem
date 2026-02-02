using DomainModels.Models;
using DomainServices.Services;

namespace MatchingAndScoringSystem.UnitTests.ServiceTests
{
    [TestFixture]
    public class ProviderScoringServiceTests
    {

        [Test]
        public void CalculateProviderScore_AllFactorsPresent_ReturnAverageScore()
        {
            // Arrange
            var provider = new Provider
            {
                AssessmentScore = 3.0m,
                LastActivityDate =  DateTime.UtcNow.AddMonths(-2),
                ProjectCount = 25,
                AverageProjectValue = 150000m,
                Certifications = 
                [
                    new ProviderCertification 
                    { 
                        Certification = new Certification { Name = "CertA" }
                    },
                    new ProviderCertification 
                    { 
                        Certification = new Certification { Name = "CertB"} 
                    }
                ]
            };

            var certifications = provider.Certifications.Select(pc => pc.Certification).ToList();

            var scoringService = new ProviderScoringService();

            // Act
            var score = scoringService.CalculateProviderScore(provider, certifications);

            // Assert
            Assert.That(score, Is.EqualTo(5.4m));
        }

        [Test]
        public void CalculateProviderScore_NoFactorsPresent_ReturnZero()
        {
            // Arrange
            var provider = new Provider
            {
                AssessmentScore = null,
                LastActivityDate = null,
                ProjectCount = null,
                AverageProjectValue = null,

            };

            List<Certification>? certifications = null;

            var scoreService = new ProviderScoringService();

            // Act
            var score = scoreService.CalculateProviderScore(provider, certifications);

            // Assert
            Assert.That(score, Is.EqualTo(0m));
        }

        [Test]
        public void CalculateProviderScore_WithSomeFactorsPresent_ReturnAverageOfValidFactors()
        {
            // Arrange
            var provider = new Provider()
            {
                AssessmentScore = 4.0m,
                LastActivityDate = null,
                ProjectCount = 25,
                AverageProjectValue = null,
                Certifications = []
            };

            var certifications = provider.Certifications.Select(pc => pc.Certification).ToList();

            var scoringService = new ProviderScoringService();

            // Act

            var score = scoringService.CalculateProviderScore(provider, certifications);

            // Assert 
            Assert.That(score, Is.EqualTo(5.33m).Within(0.01m));
        }

        [Test]
        public void CalculateProviderScore_WithEdgeCaseFactors_ReturnExpectedAverageScore()
        {
            // Arrange 
            var provider = new Provider()
            {
                AssessmentScore = 2.26m,
                LastActivityDate = DateTime.UtcNow.AddMonths(-12),
                ProjectCount = 24,
                AverageProjectValue = 100000m,
                Certifications =
                [
                    new ProviderCertification 
                    { 
                        Certification = new Certification { Name = "CertA" }
                    }
                ]
            };

            var certifications = provider.Certifications.Select(pc => pc.Certification).ToList();

            var scoreSrvice = new ProviderScoringService();

            // Act
            var score = scoreSrvice.CalculateProviderScore(provider, certifications);

            // Assert
            Assert.That(score, Is.EqualTo(4.8m));
        }
    }
}
