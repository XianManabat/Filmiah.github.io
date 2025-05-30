using System.Diagnostics;
using Final_web_app.Data;
using Final_web_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_web_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<User>_userManager;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Portfolio()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Portfolio(ContactMessage msg)
        {
            _context.ContactMessages.Add(msg);
            await _context.SaveChangesAsync();
            return RedirectToAction("Portfolio"); // or show success message
        }

        public IActionResult Services()
        {
            return View();
        }


        public IActionResult ADMIN()
        {
            return View();
        }

        public IActionResult Register_page()
        {
            return View();
        }

        [Authorize]
        public IActionResult Storage()
        {
            return RedirectToAction("MEMBER");
        }

        [Authorize]
        public IActionResult MEMBER()
        {
            var userId = _userManager.GetUserId(User);
            var images = _context.UserImages.Where(img => img.UserId == userId).ToList();
            ViewBag.Images = images;
            return View();
        }

        // Updated image upload method with proper validation and error handling
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                // Validate file
                if (image == null || image.Length == 0)
                {
                    TempData["Error"] = "Please select an image file.";
                    return RedirectToAction("MEMBER");
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(image.ContentType.ToLower()))
                {
                    TempData["Error"] = "Only JPEG, PNG, and GIF files are allowed.";
                    return RedirectToAction("MEMBER");
                }

                // Validate file size (e.g., max 5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "File size cannot exceed 5MB.";
                    return RedirectToAction("MEMBER");
                }

                using var ms = new MemoryStream();
                await image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("MEMBER");
                }

                var userImage = new UserImage
                {
                    UserId = user.Id,
                    ImageData = imageBytes,
                    ImageName = image.FileName,
                    ContentType = image.ContentType
                };

                _context.UserImages.Add(userImage);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Image uploaded successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                TempData["Error"] = "An error occurred while uploading the image. Please try again.";
            }

            return RedirectToAction("MEMBER");
        }

        // Action to serve images
        [Authorize]
        public async Task<IActionResult> GetImage(int id)
        {
            var userId = _userManager.GetUserId(User);
            var image = await _context.UserImages
                .FirstOrDefaultAsync(img => img.Id == id && img.UserId == userId);

            if (image == null)
                return NotFound();

            return File(image.ImageData, image.ContentType);
        }

        // Action to delete images
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var image = await _context.UserImages
                    .FirstOrDefaultAsync(img => img.Id == id && img.UserId == userId);

                if (image != null)
                {
                    _context.UserImages.Remove(image);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Image deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Image not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                TempData["Error"] = "An error occurred while deleting the image.";
            }

            return RedirectToAction("MEMBER");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
