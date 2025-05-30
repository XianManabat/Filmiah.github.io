using Microsoft.AspNetCore.Identity;

namespace Final_web_app.Models
{
    public class User : IdentityUser
    {
        public string Fullname { get; set; }
    }
}
