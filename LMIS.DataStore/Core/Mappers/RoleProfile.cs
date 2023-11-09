using AutoMapper;
using LMIS.DataStore.Core.DTOs.Role;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.Mappers
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, ReadRoleDTO>();
            CreateMap<CreateRoleDTO, IdentityRole>();
        }
    }
}
