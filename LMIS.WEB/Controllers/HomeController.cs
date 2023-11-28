using LMIS.WEB.Models;
using LMIS.WEB.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LMIS.WEB.ViewModels;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Claims;

namespace LMIS.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ILoginRepository _loginRepository;

        public HomeController(ILogger<HomeController> logger, Repository.IRepository.ILoginRepository loginRepository)
        {
            _logger = logger;
            _loginRepository = loginRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public  IActionResult LoginUser()
        {
        
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginViewModel loginmodel)
        {
             await  _loginRepository.AuthenticateAsync(loginmodel);
            return View(Index);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}