using LMIS.DataStore.Core.Models;
using LMIS.DataStore.Core.ViewModels;
using LMIS.DataStore.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<ApplicationUser>> GetAllUsers();


        Task<UserViewModel> GetUserWithRole(string email);
        Task<List<UserViewModel>> GetUsersWithRoles(WebCursorParams @params);
        ApplicationUser? Exists(ApplicationUser applicationUser);
        Task<ApplicationUser?> GetSingleUser(string id, bool includeRelated = true);
        void Remove(ApplicationUser applicationUser);

        Task<IdentityResult> AddUserToRoleAsync(ApplicationUser applicationUser, string roleName);

        Task<IdentityResult> CreateUserAsync(ApplicationUser applicationUser, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser applicationUser);

        Task<ApplicationUser?> FindByIdAsync(string id);
        Task<ApplicationUser?> FindByIdDeleteInclusiveAsync(string id);

        Task<ApplicationUser?> FindByEmailsync(string email);

        Task<IList<string>> GetRolesAsync(string userId);


        Task<IdentityResult> UpdateAsync(ApplicationUser applicationUser);

        Task<int> TotalCount();

        Task<int> TotalCountFiltered(WebCursorParams @params);

        Task<int> TotalUncomfirmedCount();

        Task<int> TotalDeletedCount();
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser applicationUser, IEnumerable<string> roleNames);
        Task<List<ApplicationUser>> GetUsers();

        int RandomNumber();

        Task<ApplicationUser> ConfirmAccount(string id, int pin);
    }
}
