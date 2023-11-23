using LMIS.WEB.Services.IRepository;
using LMIS.WEB.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMIS.WEB.Services.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly HttpContext _context;

        public LoginRepository(HttpClient httpClient,IConfiguration configuration,HttpContext httpContext)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["Application:ApiEndPoint"]);
        }

       
        public async Task<Dictionary<int, string>> AuthenticateAsync(LoginViewModel loginViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/login", loginViewModel);
            var cookieData = new Dictionary<int, string>();
            if (response.IsSuccessStatusCode)
            {
              // Get the returned token
              var content = await response.Content.ReadAsStringAsync();
               
                // deserializeObject
                var data = (JObject)JsonConvert.DeserializeObject(content);
                string token = data["token"].Value<string>();
                string userId = data["userId"].Value<string>();
                string roleName = data["role"].Value<string>();
                string Username = data["username"].Value<string>();
                string FirstName = data["firstName"].Value<string>();
                string LastName = data["lastName"].Value<string>();

                
                cookieData.Add(1, token);
                cookieData.Add(2, userId);
                cookieData.Add(3, roleName);
                cookieData.Add(4, Username);
                cookieData.Add(5, FirstName);
                cookieData.Add(6, LastName);
                
                // Create Claims for authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{FirstName} {LastName}"),
                    new Claim(ClaimTypes.Role, roleName),
                   // Add other claims as needed
                };
               
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

            
                // Sign in the user
                await _context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = loginViewModel.RememberMe
                    });
                return (cookieData);

            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                // Handle authentication failure
                // You can return error details or throw an exception
                cookieData.Add(7,$"Authentication failed: {error}");
            }
            return (cookieData);
        }
    }
}
