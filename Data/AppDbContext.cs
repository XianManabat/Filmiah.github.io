using Final_web_app.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Final_web_app.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserImage> UserImages { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }

    }
}
