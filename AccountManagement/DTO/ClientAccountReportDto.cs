namespace AccountManagement.DTO
{
    public class ClientAccountReportDto
    {
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public Guid Currency { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
