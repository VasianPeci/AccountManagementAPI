namespace AccountManagement.DTO
{
    public class AddBankTransactionDto
    {
        public Guid BankAccountId { get; set; }
        public int Action { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
