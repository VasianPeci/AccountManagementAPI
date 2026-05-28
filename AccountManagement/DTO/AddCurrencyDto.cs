namespace AccountManagement.DTO
{
    public class AddCurrencyDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
