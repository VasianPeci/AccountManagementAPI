namespace AccountManagement.DTO
{
    public class UpdateBankAccountDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid ClientId { get; set; }
    }
}
