using backend.starter.Repository.Models;
using QSI.ORM.NHibernate.Repository;
using System;

namespace backend.starter.Repository.NHibernate
{
    public class UserProfileDaoImpl : BaseDaoNHibernate<UserProfile, int>, IUserProfileDao
    {
        
    }
}
