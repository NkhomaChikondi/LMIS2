using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<IdentityRole>?> GetRolesAsync();
        Task<bool> Exists(string name, int rating = 0);
        Task<IdentityResult> remove(string id);
        Task AddRole(IdentityRole identityRole);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole identityRole);
        int TotalCount();
        Task<IdentityRole?> GetRole(string name);
        Task<IdentityRole?> GetRoleByIdAsync(string roleId);
    }
}
