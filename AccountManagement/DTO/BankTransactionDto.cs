namespace AccountManagement.DTO
{
    public class BankTransactionDto
    {
        public Guid Id { get; set; }
        public Guid BankAccountId { get; set; }
        public int Action { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public string? StripePaymentId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
