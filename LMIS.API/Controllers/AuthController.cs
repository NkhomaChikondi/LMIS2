using LMIS.DataStore.Core.DTOs.Role;
using LMIS.DataStore.Core.DTOs.User;
using LMIS.DataStore.Core.Models;
using LMIS.DataStore.Core.Services.Interfaces;
using LMIS.DataStore.Core.ViewModels;
using LMIS.DataStore.Data;
using LMIS.DataStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMIS.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenManager tokenManager;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IRoleRepository _roleRepository;
        public IConfiguration Configuration { get; }

        public AuthController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ITokenManager tokenManager, IUserRepository userRepository, IUnitOfWork unitOfWork, IEmailService emailService, IRoleRepository roleRepository)
        {

            this._userManager = userManager;
            this._signInManager = signInManager;
            Configuration = configuration;
            this.tokenManager = tokenManager;
            this.userRepository = userRepository;
            this._unitOfWork = unitOfWork;
            this._emailService = emailService;
            this._context = context;
            this._roleRepository = roleRepository;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            //getting user from database
            var user = await this.userRepository.FindByEmailsync(loginModel.Email);

            //checking if user was found
            if (user != null)
            {
               
                // checking is the account is confirmed already
                if (user.EmailConfirmed == false)
                {
                    return BadRequest("Account not confirmed, please check your email for the activation link");
                }

                //checking user password
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

                //if successful generate the token based on details given. Valid for one day
                if (signInResult.Succeeded)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();

                    LoginDTO userData = GenerateToken(user);


                    await this._unitOfWork.SaveToStoreAsync();

                    return Ok(new { TokenData = userData });

                }
                else
                {
                    return BadRequest("login credentials do not match");
                }
            }

            return BadRequest("Account not found");
        }
        // POST: UserController/Create
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<ActionResult> Register([FromBody] CreateUserDTO applicationViewModel)
        {
            // generating a random number
            int pin = this.userRepository.RandomNumber();
            applicationViewModel.Pin = pin;

            //check for model state validity
            if (ModelState.IsValid)
            {
                //check if the role given exist in the system

                var mappedRoleRecord = new IdentityRole() { Name = applicationViewModel.RoleName };

                if (await this._roleRepository.Exists(applicationViewModel.RoleName) == false)
                {
                    ModelState.AddModelError(nameof(applicationViewModel.RoleName), $"This Role {applicationViewModel.RoleName} doees not exists in the system");

                    return BadRequest(ModelState);
                }              

                var applicationUser = new ApplicationUser
                {
                    FirstName = applicationViewModel.FirstName,
                    LastName = applicationViewModel.LastName,
                    Gender = applicationViewModel.Gender,
                    UserName = applicationViewModel.Email,
                    Email = applicationViewModel.Email,
                    Location = applicationViewModel.Location,
                    Pin = applicationViewModel.Pin,

                };

                //check if the email account is already take
                var recordPresence = this.userRepository.Exists(applicationUser);

                if (recordPresence != null)
                {


                    ModelState.AddModelError(nameof(applicationViewModel.Email), "This email is already in used by another account");

                    return BadRequest(ModelState);
                }
                // call the generate password method
                var _password = userRepository.GeneratePassword(applicationUser);
                // pass the password to the applicationViewModel
                applicationViewModel.Password = _password;
                //try adding a new user
                var result = await this.userRepository.CreateUserAsync(applicationUser, applicationViewModel.Password);

                if (result.Succeeded && applicationViewModel.RoleName != null)
                {
                    //associate user to role
                    var dbResult = await this.userRepository.AddUserToRoleAsync(applicationUser, applicationViewModel.RoleName);

                    if (dbResult.Succeeded)
                    {
                        string PasswordBody = "Your account has been created on LMIS. Your password is " + applicationViewModel.Password;
                        this._emailService.SendMail(applicationUser.Email, "Login Details", PasswordBody);

                        //check for presence of parent email

                        string PinBody = "An account has been created on LMIS. Your OTP is " + pin + " <br /> Enter the OTP to activate your account" + " <br /> You can activate your account by clicking here</a>";

                        this._emailService.SendMail(applicationUser.Email, "Login Details", PinBody);
                    }
                    else
                    {
                        return BadRequest(ModelState);
                    }
                    return Ok(applicationUser);
                }



                result.Errors.ToList().ForEach(error =>
                {
                    var description = error.Description;
                    var errorCode = error.Description;

                    ModelState.AddModelError(errorCode, description);
                });
            }

            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        private LoginDTO GenerateToken(ApplicationUser user)
        {
            //if successful generate the token based on details given. Valid for one day
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("TokenString:TokenKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Configuration.GetValue<double>("TokenString:expiryMinutes")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // getting user role
            var role = _context.UserRoles.Where(u => u.UserId == user.Id).FirstOrDefault();
            var roleData = _context.Roles.Where(u => u.Id == role.RoleId).FirstOrDefault();


            // login DTO
            var userData = new LoginDTO()
            {
                Token = tokenString,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roleData.Name,
                Email = user.Email,
                TokenType = "bearer",
                TokenExpiryMinutes = (DateTime)tokenDescriptor.Expires,

            };
            return userData;

        }

        [HttpGet]
        [Route("ConfirmAccount/{email}/{pin}")]
        // confirming account
        public async Task<IActionResult> ConfirmAccount(string email, int pin)
        {
            //check if values are not null
            if (pin < 0 || email == null)
            {
                ModelState.AddModelError("Pin", "Pin Cannot be empty");

                return BadRequest("pin cannot be empty");
            }

            // check if user exist
            var user = await this.userRepository.FindByEmailsync(email);

            if (user == null)
            {

                return BadRequest("account not found");
            }

            // checking is the pin submitted is the same as the pin in db
            if (user.Pin != pin)
            {


                return BadRequest("invalid pin");
            }

            // confirming account and saving 
            user = await userRepository.ConfirmAccount(user.Id, pin);

            await _unitOfWork.SaveToStoreAsync();

            return Ok(new { response = "Account confirmed", user = user });

        }

        [HttpGet]
        [Route("ResendPin/{email}")]
        // Resending account activation pin
        public async Task<IActionResult> ResendPin(string email)
        {
            //check if values are not null
            if (email == null)
            {

                return BadRequest("Insufficient parameters, try again");
            }

            // check if user exist
            var user = await userRepository.FindByEmailsync(email);

            if (user == null)
            {
                return BadRequest("Account doesn't exist, please create one");
            }

            // confirming account and saving 
            int pin = userRepository.RandomNumber();
            user.Pin = pin;

            await _unitOfWork.SaveToStoreAsync();

            // sending an email
            string PinBody = "Your OTP for LMIS is " + pin + " <br /> Enter the OTP, email address and the new password to reset your account";
            this._emailService.SendMail(user.Email, "Account Reset Details", PinBody);

            return Ok("Check your email for the pin");

        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel model)
        {

            //check for model state

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find user with the email sent

            var user = await this.userRepository.FindByEmailsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Email not recognized");

                return BadRequest(ModelState);
            }

            //check if the email and pin match

            if (user.Pin != model.Pin)
            {
                ModelState.AddModelError("Pin", "Pin is invalid");
                return BadRequest(ModelState);
            }

            //update the password of the user 

            var token = await this._userManager.GeneratePasswordResetTokenAsync(user);

            var result = await this._userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                return Ok("password reset successfully");
            }
            else
            {
                ModelState.AddModelError("GeneralError", "Failed to reset password");

                return BadRequest(ModelState);
            }



        }

    }
}
