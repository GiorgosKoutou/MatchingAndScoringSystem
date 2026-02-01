
namespace DomainModels.Models
{
    public class ProviderSkill
    {
        public Provider Provider { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public int? MaxUsersSupported { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
