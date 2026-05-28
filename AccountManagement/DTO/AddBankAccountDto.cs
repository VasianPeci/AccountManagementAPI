namespace AccountManagement.DTO
{
    public class AddBankAccountDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CurrencyId { get; set; }
        public Guid ClientId { get; set; }
    }
}
