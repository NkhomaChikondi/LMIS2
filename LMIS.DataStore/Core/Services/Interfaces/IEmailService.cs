using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.Services.Interfaces
{
    public interface IEmailService
    {
        IConfiguration _configuration { get; }

        public string SendMail(string email, string subject, string HtmlMessage);
    }
}
