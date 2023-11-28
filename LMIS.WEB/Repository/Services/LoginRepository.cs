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
using LMIS.WEB.Repository.IRepository;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;

namespace LMIS.WEB.Repository.Services
{
    public class LoginRepository : ILoginRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;
        private readonly HttpResponse _httpResponse;

        public LoginRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["Application:ApiEndPoint"]);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_context = httpContext;
        }

        public async Task<string> AuthenticateAsync(LoginViewModel loginViewModel)
        {
            try
            {
                // Serialize the LoginViewModel object to JSON
                string serializedLoginViewModel = JsonConvert.SerializeObject(loginViewModel);

                var response =  _httpClient.PostAsJsonAsync("api/Auth/login", serializedLoginViewModel).Result;

                if (response.IsSuccessStatusCode)
                {
                    HttpContext Context = _context.HttpContext;
                    //Get returned token
                    var content =  await response.Content.ReadAsStringAsync();
                    // userDetails = JsonConvert.DeserializeObject<List<UserDetails>>(content);

                    var data = (JObject)JsonConvert.DeserializeObject(content);
                    string token = data["token"].Value<string>();
                    string userId = data["userId"].Value<string>();
                    string roleName = data["role"].Value<string>();
                    string Username = data["username"].Value<string>();
                    string FirstName = data["firstName"].Value<string>();
                    string LastName = data["lastName"].Value<string>();

                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(1);
                    _httpResponse.Cookies.Append("token", token, options);
                    _httpResponse.Cookies.Append("RoleName", roleName, options);
                    _httpResponse.Cookies.Append("FirstName", FirstName, options);
                    _httpResponse.Cookies.Append("LastName", LastName, options);

                    Context.Session.SetString("token", token);

                    if (loginViewModel != null)
                    {
                        var claims = new List<Claim>()
                            {
                                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userId)),
                                new Claim(ClaimTypes.Name, Convert.ToString(Username)),
                                new Claim(ClaimTypes.Role, Convert.ToString(roleName)),
                                new Claim("LMIS", "Library Management Information System")
                            };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            new AuthenticationProperties()
                            {
                                IsPersistent = loginViewModel.RememberMe
                            });
                        //Login
                    }

                    if (roleName == "Management")
                    {
                        // return Json(new { status = "success", area = "Management" }); ;
                        return "Success";
                    }
                    else if (roleName == "Front Office")
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Success";
                    }


                }
                else
                {

                    string res = await response.Content.ReadAsStringAsync();
                    return "Error";
                    //TempData["error"] = res;
                    //return Json(new { status = "failed", message = res });

                }
            }
            catch (Exception ex)
            {

                var error = ex;
                return( error.Message );
            }
          
        }
    }
}
