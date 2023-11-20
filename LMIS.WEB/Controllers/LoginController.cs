using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using LMIS.WEB.ViewModels;

namespace LMIS.WEB.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginViewModel loginmodel)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(_configuration["Application:ApiEndPoint"]);
                    var response = client.PostAsJsonAsync("api/login", loginmodel).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //Get returned token
                        var content = await response.Content.ReadAsStringAsync();
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
                        Response.Cookies.Append("token", token, options);
                        Response.Cookies.Append("RoleName", roleName, options);
                        Response.Cookies.Append("FirstName", FirstName, options);
                        Response.Cookies.Append("LastName", LastName, options);

                        HttpContext.Session.SetString("token", token);

                        if (loginmodel != null)
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
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                                new AuthenticationProperties()
                                {
                                    IsPersistent = loginmodel.RememberMe
                                });

                            //Login
                        }
                        if (roleName == "Management")
                        {
                            return Json(new { status = "success", area = "Management" }); ;
                        }
                        else if (roleName == "Front Office")
                        {
                            return Json(new { status = "success", area = "FrontOffice" }); ;
                        }
                        else 
                        {
                            return Json(new { status = "success", area = "Back Office" }); ;
                        }
                       
                        
                    }
                    else
                    {

                        string res = await response.Content.ReadAsStringAsync();
                        TempData["error"] = res;
                        return Json(new { status = "failed", message = res });
                        
                    }
                }
            }
            catch (Exception ex)
            {

                TempData["error"] = "We could not connect you to the website. Contact the admin or try again later" + ex;
                // return RedirectToAction("Index", "Home");
                return Json(new { status = "failed", message = ex });
               
            }

        }
    }
}
