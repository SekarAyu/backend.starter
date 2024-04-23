using AutoMapper;
using backend.starter.Common.Commands;
using backend.starter.Common.Dtos;
using backend.starter.Repository;
using backend.starter.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace backend.starter.Service.Impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserProfileDao userProfileDao;
        private readonly IMapper mapper;

        public UserServiceImpl(IUserProfileDao userProfileDao, IMapper mapper)
        {
            this.userProfileDao = userProfileDao;
            this.mapper = mapper;
        }

        public void AddNewProfile(AddUserProfileCommand command)
        {
            UserProfile newProfile = new UserProfile()
            {
                Name = command.Name,
                Email = command.Email
            };
            userProfileDao.Save(newProfile);
        }

        public IList<UserProfileDto> GetAllUserProfile()
        {
            var users = userProfileDao.GetAll();
            IList<UserProfileDto> result = mapper.Map<List<UserProfileDto>>(users);
            return result;
        }
    }
}
