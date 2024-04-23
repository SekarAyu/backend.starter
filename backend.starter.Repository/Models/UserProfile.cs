using QSI.Persistence.Model;
using System.Collections.Generic;

namespace backend.starter.Repository.Models
{
    public class UserProfile : AuditedEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }

        public override bool Equals(object obj)
        {
            return obj is UserProfile profile &&
                   Id == profile.Id &&
                   Actor == profile.Actor &&
                   Timestamp == profile.Timestamp &&
                   Name == profile.Name &&
                   Email == profile.Email;
        }

        public override int GetHashCode()
        {
            int hashCode = 101690330;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Actor);
            hashCode = hashCode * -1521134295 + Timestamp.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Email);
            return hashCode;
        }
    }
}
