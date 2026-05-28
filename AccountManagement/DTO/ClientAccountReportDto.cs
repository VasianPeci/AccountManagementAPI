namespace AccountManagement.DTO
{
    public class ClientAccountReportDto
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public Guid Currency { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
