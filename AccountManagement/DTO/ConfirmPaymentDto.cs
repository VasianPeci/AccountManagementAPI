using System.ComponentModel.DataAnnotations;

namespace AccountManagement.DTO
{
    public class ConfirmPaymentDto
    {
        [Required]
        public string SessionId { get; set; } = string.Empty;
    }
}
