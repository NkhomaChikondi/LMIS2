using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LMIS.DataStore.Core.Models;

namespace LMIS.DataStore.Core.ViewModels
{
    public class UserViewModel: ApplicationUser
    {
        TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public string DataInvalid { get; set; } = "true";
        public string FormattedFirstName => myTI.ToTitleCase(FirstName);
        public string FormattedLastName => myTI.ToTitleCase(LastName);
        public string FormattedGender => (!string.IsNullOrEmpty(Gender)) ? myTI.ToTitleCase(Gender) : "";
       


      
    }
}
