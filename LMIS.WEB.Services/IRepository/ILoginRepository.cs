using LMIS.WEB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.WEB.Services.IRepository
{
    public interface ILoginRepository : IApiRepository<LoginViewModel>
    {
        Task<string> AuthenticateAsync(LoginViewModel loginViewModel);
    }
}
