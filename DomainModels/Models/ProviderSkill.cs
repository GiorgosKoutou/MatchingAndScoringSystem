
namespace DomainModels.Models
{
    public class ProviderSkill
    {
        public Provider Provider { get; set; }
        public Service Service { get; set; }
        public int? MaxUsersSupported { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
