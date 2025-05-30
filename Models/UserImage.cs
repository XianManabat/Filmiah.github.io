using System.ComponentModel.DataAnnotations;

namespace Final_web_app.Models
{
    public class UserImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

    
        public byte[] ImageData { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageName { get; set; }

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; }

        
    }
}