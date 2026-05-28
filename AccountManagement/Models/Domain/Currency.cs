namespace AccountManagement.Models.Domain
{
    public class Currency
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
