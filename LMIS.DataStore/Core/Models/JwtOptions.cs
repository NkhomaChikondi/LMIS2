using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.Models
{
    public class JwtOptions
    {
        public JwtOptions(IConfiguration configuration)
        {

            Configuration = configuration;
            ExpiryMinutes = Configuration.GetValue<double>("TokenString:expiryMinutes");
            Secret = Configuration.GetValue<string>("TokenString:TokenKey");
            Issuer = Configuration.GetValue<string>("TokenString:issuer");
            ValidateLifeTime = Configuration.GetValue<string>("TokenString:validateLifeTime");
        }
        public IConfiguration Configuration { get; }

        public string Secret
        {
            get { return Secret; }
            set { Secret = value; }
        }

        public double ExpiryMinutes
        {
            get { return ExpiryMinutes; }
            set { ExpiryMinutes = value; }
        }

        public string Issuer
        {
            get { return Issuer; }
            set { Issuer = value; }
        }

        public string ValidateLifeTime

        {
            get { return ValidateLifeTime; }
            set { ValidateLifeTime = value; }
        }
    }
}
