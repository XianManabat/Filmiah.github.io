namespace Final_web_app.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}