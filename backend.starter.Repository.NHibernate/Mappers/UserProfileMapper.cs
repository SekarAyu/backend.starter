using QSI.ORM.NHibernate.Mapper;

namespace backend.starter.Repository.Mappers
{
    public class UserProfileMapper : BaseAuditedEntityMapper<Models.UserProfile, int>
    {
        public UserProfileMapper() : base()
        {
            Table("user_profile");

            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Column("name").Not.Nullable().Unique();
            Map(x => x.Email).Column("email").Not.Nullable();
        }
    }
}
