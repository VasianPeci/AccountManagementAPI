namespace AccountManagement.Models.Domain
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid ClientId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        // Navigation Properties
        public Currency Currency { get; set; }
        public Client Client { get; set; }
    }
}
