using AutoMapper;
using LMIS.DataStore.Core.Models;
using LMIS.DataStore.Core.ViewModels;
using LMIS.DataStore.Data;
using LMIS.DataStore.Helpers;
using LMIS.DataStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Repositories.PostGresRepos
{
    public class UserRepo : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public async Task<IdentityResult> AddUserToRoleAsync(ApplicationUser applicationUser, string roleName)
        {
            //add user to role
            return await this._userManager.AddToRoleAsync(applicationUser, roleName);
        }

        public Task<ApplicationUser> ConfirmAccount(string id, int pin)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser applicationUser, string password)
        {
            return await this._userManager.CreateAsync(applicationUser, password);
        }

        public ApplicationUser? Exists(ApplicationUser applicationUser)
        {
            return this._context.Users.FirstOrDefault(u => u.Email == applicationUser.Email);
        }

        public async Task<ApplicationUser?> FindByEmailsync(string email)
        {
            return await this._userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> FindByIdAsync(string id)
        {
            return await this._context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<ApplicationUser?> FindByIdDeleteInclusiveAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser applicationUser)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await this._context.Users.ToListAsync();
        }

        public async Task<IList<string>> GetRolesAsync(string userId)
        {

            //find user based on the id provided

            var user = await this._userManager.FindByIdAsync(userId);



            return await this._userManager.GetRolesAsync(user);
        }

        public async Task<ApplicationUser?> GetSingleUser(string id, bool includeRelated = true)
        {
            return await this._context.Users.FirstOrDefaultAsync(u => u.Id == id);

        }

        public async Task<List<ApplicationUser>> GetUsers()
        {
            return await this._context.Users.Where(u => u.EmailConfirmed == true).ToListAsync();
        }

        public async Task<List<UserViewModel>> GetUsersWithRoles(WebCursorParams @params)
        {
            //Initialize the mapper
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ApplicationUser, UserViewModel>()
                );

            Mapper mapper = new Mapper(config);
            List<ApplicationUser> users = new List<ApplicationUser>();


            if (string.IsNullOrEmpty(@params.SearchTerm))
            {

                users = await this._context.Users.Where(u =>  u.EmailConfirmed == false).Skip(@params.Skip).Take(@params.Take).ToListAsync();

            }
            else
            {
                users = await this._context.Users
                    .Where(u =>  u.EmailConfirmed == false
                            || u.FirstName.ToLower().Contains(@params.SearchTerm.ToLower().Trim())
                            || u.LastName.ToLower().Contains(@params.SearchTerm.ToLower().Trim())
                            || u.Gender.ToLower().Contains(@params.SearchTerm.ToLower().Trim())
                            || u.Email.ToLower().Contains(@params.SearchTerm.ToLower().Trim())
                            || u.PhoneNumber.ToLower().Contains(@params.SearchTerm.ToLower().Trim()))
                    .Skip(@params.Skip)
                    .Take(@params.Take)
                    .ToListAsync();

            }

            //convert the list of users records to a list of UserViewModels

            var userViewModelUnFiltered = mapper.Map<List<UserViewModel>>(users);
            List<UserViewModel> userViewModels = new();

            userViewModelUnFiltered.ForEach(u => {
                //get a list of roles of the particular user
                var roles = this.GetRolesAsync(u.Id).Result;

                if (roles.Count > 0)
                {
                    u.RoleName = roles.First();

                    //add the updated user to the userViewModels class

                    userViewModels.Add(u);
                }


            });

            return userViewModels;
        }

        public async Task<UserViewModel> GetUserWithRole(string email)
        {
            //Initialize the mapper
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ApplicationUser, UserViewModel>()
                );

            Mapper mapper = new Mapper(config);

            var user = await this._context.Users.Where(u => u.Email == email  && u.EmailConfirmed == true).FirstOrDefaultAsync();

            //convert the list of users records to a list of UserViewModels

            UserViewModel userViewModel = null;

            if (user != null)
            {
                var userViewModelUnFiltered = mapper.Map<UserViewModel>(user);




                //get a list of roles of the particular user
                var roles = this.GetRolesAsync(userViewModelUnFiltered.Id).Result;

                if (roles.Count > 0)
                {
                    userViewModelUnFiltered.RoleName = roles.First();

                    //add the updated user to the userViewModels class

                    userViewModel = userViewModelUnFiltered;
                }
            }


            return userViewModel;
        }

        public int RandomNumber()
        {
            // generating a random number
            Random generator = new Random();
            int number = generator.Next(100000, 999999);

            return number;
        }

        public void Remove(ApplicationUser applicationUser)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser applicationUser, IEnumerable<string> roleNames)
        {
            return await this._userManager.RemoveFromRolesAsync(applicationUser, roleNames);
        }

        public async Task<int> TotalCount()
        {
            return await this._context.Users.Where(u =>  u.EmailConfirmed == true).CountAsync();
        }

        public Task<int> TotalCountFiltered(WebCursorParams @params)
        {
            throw new NotImplementedException();
        }

        public Task<int> TotalDeletedCount()
        {
            throw new NotImplementedException();
        }

        public Task<int> TotalUncomfirmedCount()
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser applicationUser)
        {
            return await this._userManager.UpdateAsync(applicationUser);
        }
    }
}
