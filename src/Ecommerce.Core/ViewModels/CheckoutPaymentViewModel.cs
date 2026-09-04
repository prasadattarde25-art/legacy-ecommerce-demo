using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    public class CheckoutPaymentViewModel
    {
        [Required(ErrorMessage = "Select a payment method.")]
        [MaxLength(40)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [MaxLength(100)]
        [Display(Name = "Name on Card")]
        public string CardHolderName { get; set; }

        [CreditCard(ErrorMessage = "Enter a valid credit card number.")]
        [MaxLength(19)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Range(1, 12, ErrorMessage = "Enter a valid expiry month.")]
        [Display(Name = "Expiry Month")]
        public int? ExpiryMonth { get; set; }

        [Range(2024, 2040, ErrorMessage = "Enter a valid expiry year.")]
        [Display(Name = "Expiry Year")]
        public int? ExpiryYear { get; set; }

        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits.")]
        [RegularExpression("^[0-9]{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
        [Display(Name = "CVV")]
        public string Cvv { get; set; }
    }
}