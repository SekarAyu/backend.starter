using backend.starter.Common.Commands;
using backend.starter.Common.Dtos;
using System.Collections.Generic;

namespace backend.starter.Service
{
    public interface IUserService
    {
        void AddNewProfile(AddUserProfileCommand command);
        IList<UserProfileDto> GetAllUserProfile();
    }
}
