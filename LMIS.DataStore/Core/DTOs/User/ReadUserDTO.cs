using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.DTOs.User
{
    public class ReadUserDTO
    {
        public int Id { get; set; }
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
    }
}
