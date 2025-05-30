using System.ComponentModel.DataAnnotations;

namespace Final_web_app.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [EmailAddress]
        public string username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        
    }
}
