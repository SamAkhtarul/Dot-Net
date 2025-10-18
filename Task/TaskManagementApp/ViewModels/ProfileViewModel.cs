using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(25, ErrorMessage = "Name must be less than or equal 25 characters.")]
        public string Name { get; set; } = string.Empty;

        public IFormFile? Picture { get; set; }
    }
}
