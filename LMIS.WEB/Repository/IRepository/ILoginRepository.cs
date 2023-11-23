using LMIS.WEB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.WEB.Repository.IRepository
{
    public interface ILoginRepository
    {
        Task<Dictionary<int, string>> AuthenticateAsync(LoginViewModel loginViewModel);

    }
}
