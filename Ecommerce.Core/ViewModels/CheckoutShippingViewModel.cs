using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    public class CheckoutShippingViewModel
    {
        [Required(ErrorMessage = "Select a shipping method.")]
        [MaxLength(40)]
        [Display(Name = "Shipping Method")]
        public string ShippingMethod { get; set; }

        [MaxLength(600)]
        public string DeliveryNotes { get; set; }
    }
}