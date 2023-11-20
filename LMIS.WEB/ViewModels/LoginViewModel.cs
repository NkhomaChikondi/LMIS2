using System.ComponentModel.DataAnnotations;

namespace LMIS.WEB.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your Username")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
