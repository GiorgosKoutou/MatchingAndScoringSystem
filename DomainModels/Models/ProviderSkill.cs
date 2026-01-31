using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
