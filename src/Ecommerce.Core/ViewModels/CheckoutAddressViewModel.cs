using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    public class CheckoutAddressViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name is too long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name is too long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        [MaxLength(200)]
        public string AddressLine1 { get; set; }

        [MaxLength(200)]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [Display(Name = "ZIP / Postal Code")]
        [Required(ErrorMessage = "Postal code is required.")]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(40)]
        public string Phone { get; set; }
    }
}