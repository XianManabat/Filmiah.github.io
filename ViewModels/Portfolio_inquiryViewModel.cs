using System.ComponentModel.DataAnnotations;

namespace Final_web_app.ViewModels
{
    public class Portfolio_inquiryViewModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string Message { get; set; }
    }
}
