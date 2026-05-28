namespace AccountManagement.DTO
{
    public class AccountReportDto
    {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public Guid Currency { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
