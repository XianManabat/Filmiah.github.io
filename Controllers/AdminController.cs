using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_web_app.Data; 
using Final_web_app.Models; 

namespace Final_web_app.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDbContext _context;
        private const string ADMIN_USERNAME = "adminako";
        private const string ADMIN_PASSWORD = "admin1234";
        private const string ADMIN_SESSION_KEY = "AdminLoggedIn";

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString(ADMIN_SESSION_KEY) == "true";
        }

        public IActionResult admin_login()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult admin_login(string username, string password)
        {
            // Check credentials
            if (username == ADMIN_USERNAME && password == ADMIN_PASSWORD)
            {
               
                HttpContext.Session.SetString(ADMIN_SESSION_KEY, "true");

                 
                return RedirectToAction("Dashboard");
            }
            else
            {
                
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }

        // Logout action
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(ADMIN_SESSION_KEY);
            return RedirectToAction("admin_login");
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        
        public async Task<IActionResult> User_list()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("admin_login");
            }

            try
            {
                // Fetch users from AspNetUsers table
                var users = await _context.Users.ToListAsync();
                return View(users);
            }
            catch (Exception ex)
            {
            
                ViewBag.Error = "Error loading users: " + ex.Message;
                return View(new List<Microsoft.AspNetCore.Identity.IdentityUser>());
            }
        }

        public async Task<IActionResult> Inquiry_list()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("admin_login");
            }

            try
            {
                // Fetch contact messages from database, ordered by most recent first
                var inquiries = await _context.ContactMessages
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return View(inquiries);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading inquiries: " + ex.Message;
                return View(new List<ContactMessage>());
            }
        }

        // Get inquiry details for modal/popup view
        [HttpGet]
        public async Task<IActionResult> GetInquiryDetails(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var inquiry = await _context.ContactMessages.FindAsync(id);
                if (inquiry == null)
                {
                    return Json(new { success = false, message = "Inquiry not found" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = inquiry.Id,
                        name = inquiry.Name,
                        email = inquiry.Email,
                        phone = inquiry.Phone,
                        message = inquiry.Message,
                        createdDate = inquiry.CreatedDate.ToString("MMM dd, yyyy 'at' hh:mm tt")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading inquiry details" });
            }
        }

        // Delete inquiry
        [HttpPost]
        public async Task<IActionResult> DeleteInquiry(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var inquiry = await _context.ContactMessages.FindAsync(id);
                if (inquiry != null)
                {
                    _context.ContactMessages.Remove(inquiry);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Inquiry deleted successfully" });
                }
                return Json(new { success = false, message = "Inquiry not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting inquiry" });
            }
        }

        // Mark inquiry as read (if you want to add this feature)
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var inquiry = await _context.ContactMessages.FindAsync(id);
                if (inquiry != null)
                {
                    // Assuming you add an IsRead property to your ContactMessage model
                    // inquiry.IsRead = true;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Inquiry marked as read" });
                }
                return Json(new { success = false, message = "Inquiry not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating inquiry" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber ?? "Not provided",
                        emailConfirmed = user.EmailConfirmed,
                        lockoutEnd = user.LockoutEnd?.ToString("MMM dd, yyyy 'at' hh:mm tt"),
                        accessFailedCount = user.AccessFailedCount,
                        // Add these if you have these properties in your User model
                        createdDate = "Registration date not available", // Modify if you have this field
                        lastLogin = "Last login not available" // Modify if you have this field
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading user details: " + ex.Message });
            }
        }

        // Delete user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!IsAdminLoggedIn())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Optional: Prevent deleting admin user (if you have a way to identify admin)
                if (user.UserName == ADMIN_USERNAME)
                {
                    return Json(new { success = false, message = "Cannot delete admin user" });
                }

                // Remove user from database
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting user: " + ex.Message });
            }
        }

        // Optional: Edit user method (if you want to implement editing)
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("admin_login");
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("User_list");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading user: " + ex.Message;
                return RedirectToAction("User_list");
            }
        }
    }
}
