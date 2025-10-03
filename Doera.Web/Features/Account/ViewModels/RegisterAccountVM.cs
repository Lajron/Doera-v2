using System.ComponentModel.DataAnnotations;

namespace Doera.Web.Features.Account.ViewModels {

    public record RegisterAccountVM() {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
