using backend.starter.Repository.Models;
using QSI.Persistence.Repository;
using System;
using System.Collections.Generic;

namespace backend.starter.Repository
{
    public interface IUserProfileDao : BaseDao<UserProfile, int>
    {
        
    }
}