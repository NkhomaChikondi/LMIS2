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
                    //person p = new person { name = "Sourav", surname = "Kayal" };
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
                            new Claim("DMS", "Document Management System")
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
                        if (roleName == "Managing Director")
                        {
                            return Json(new { status = "success", area = "ManagingDirector" }); ;
                        }
                        else if (roleName == "Country Lead")
                        {
                            return Json(new { status = "success", area = "CountryLead" }); ;
                        }
                        else if (roleName == "Admin")
                        {
                            return Json(new { status = "success", area = "Admin" }); ;
                        }
                        else if (roleName == "Courier")
                        {
                            return Json(new { status = "success", area = "Courier" }); ;
                        }
                        else
                        {
                            return Json(new { status = "success", area = "SalesEngineer" });

                        }


                        // return RedirectToAction("Index", "Home", new { Area = "CountryLead" });

                        //return Redirect("http://129.151.153.61/dms/home/");//
                        // return RedirectToAction("Index", "Home");//193.123.83.250/dms
                    }
                    else
                    {

                        string res = await response.Content.ReadAsStringAsync();
                        TempData["error"] = res;
                        return Json(new { status = "failed", message = res });
                        //return Redirect("http://129.151.153.61/dms/");
                        //  return RedirectToAction("Index", "Login");
                    }
                }
            }
            catch (Exception ex)
            {
                //TempData["error"] = "We could not connect you to the website. Contact the admin";
                //return RedirectToAction("Index", "Login");
                TempData["error"] = "We could not connect you to the website. Contact the admin or try again later" + ex;
                // return RedirectToAction("Index", "Home");
                return Json(new { status = "failed", message = ex });
                //return Redirect("http://129.151.153.61/dms/");
            }

        }
    }
}
