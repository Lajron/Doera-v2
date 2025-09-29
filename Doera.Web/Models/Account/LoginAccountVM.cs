using System.ComponentModel.DataAnnotations;

namespace Doera.Web.Models.Account {
    public record LoginAccountVM {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }
        [Required]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = false;
        }
}
