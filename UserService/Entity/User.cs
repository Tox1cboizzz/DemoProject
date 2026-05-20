using System.ComponentModel.DataAnnotations;

namespace UserService.Entity
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username ko dc de trong")]
        [MaxLength(50, ErrorMessage = "dai toi da 50 ki tu")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "ko dc de trong Password")]
        [MinLength(6, ErrorMessage = "dai toi thieu 6 ki tu")]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}