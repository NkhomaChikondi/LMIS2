using AutoMapper;
using LMIS.DataStore.Core.DTOs.User;
using LMIS.DataStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDTO, ApplicationUser>();
            CreateMap<UpdateUserDTO, ApplicationUser>();
            CreateMap<ApplicationUser, ReadUserDTO>();


        }
    }
}
