using System.ComponentModel.DataAnnotations;

namespace AccountManagement.DTO
{
    public class CreateStripePaymentDto
    {
        [Required]
        public Guid BankAccountId { get; set; }

        [Required]
        public Guid CurrencyId { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Amount { get; set; }
    }
}
