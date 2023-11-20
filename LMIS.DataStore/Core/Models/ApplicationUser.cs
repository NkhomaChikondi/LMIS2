using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            
        }
        [Required]
        [StringLength(maximumLength: 100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(maximumLength: 15)]
        public string Gender { get; set; }
        [StringLength(maximumLength: 255)]
        public string Location { get; set; }
        public int Pin { get; set; }

        [NotMapped]
        public String FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
