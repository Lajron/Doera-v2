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
    }
}
