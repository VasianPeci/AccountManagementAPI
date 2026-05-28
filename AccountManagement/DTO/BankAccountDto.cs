using AccountManagement.Models.Domain;

namespace AccountManagement.DTO
{
    public class BankAccountDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid ClientId { get; set; }
    }
}
