namespace AccountManagement.DTO
{
    public class UpdateCurrencyDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
