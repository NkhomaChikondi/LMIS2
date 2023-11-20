using LMIS.DataStore.Data;
using LMIS.DataStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Repositories.PostGresRepos
{
    public class RoleRepo : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepo(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._roleManager = roleManager;
        }
        public async Task AddRole(IdentityRole identityRole)
        {
            await this._roleManager.CreateAsync(identityRole);
        }

        public async Task<bool> Exists(string name, int rating = 0)
        {
            IdentityRole? identityRole = null;
            if (rating != 0)
            {
                identityRole = await this._context.Roles.FirstOrDefaultAsync(r => r.Name.Trim().ToLower() == name.Trim().ToLower());
            }


            return (identityRole != null ? true : false);
        }

        public async Task<IdentityRole?> GetRole(string name)
        {
            return await this._roleManager.FindByNameAsync(name);
        }

       

        public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
        {
            return await this._context.Roles.FirstOrDefaultAsync(r => r.Id.Trim().ToLower() == roleId.Trim().ToLower());
        }

        public async Task<List<IdentityRole>?> GetRolesAsync()
        {
            var roles = await this._context.Roles.ToListAsync();

            return roles;
        }

        public async Task<IdentityResult> remove(string id)
        {
            var record = await _roleManager.FindByIdAsync(id);

            IdentityResult result = new IdentityResult();


            if(record != null)
            {
                 result = await _roleManager.DeleteAsync(record);
            }
            return result;
        }

        public int TotalCount()
        {
            return this._roleManager.Roles.Count();
        }

        public async Task<IdentityResult> UpdateRoleAsync(IdentityRole identityRole)
        {
            return await this._roleManager.UpdateAsync(identityRole);
        }
    }
}
