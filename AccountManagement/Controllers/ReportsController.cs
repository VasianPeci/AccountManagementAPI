using AccountManagement.Data;
using AccountManagement.DTO;
using AccountManagement.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsRepository reportsRepository;
        private readonly IMapper mapper;

        public ReportsController(IReportsRepository reportsRepository, IMapper mapper)
        {
            this.reportsRepository = reportsRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("accounts")]
        [Authorize(Roles = "Auditor, Admin")]
        public async Task<IActionResult> GetAccountReports()
        {
            var bankAccounts = await reportsRepository.GetAccountReports();

            // Map Bank Account Domain Models to Account Report DTO List

            //foreach (var account in bankAccounts)
            //{
            //    accountReportDtos.Add(new AccountReportDto()
            //    {
            //        ClientCode = account.Client.UserId,
            //        ClientName = account.Client.FirstName + " " + account.Client.LastName,
            //        AccountCode = account.Code,
            //        AccountName = account.Name,
            //        Currency = account.CurrencyId,
            //        CurrentBalance = account.Balance
            //    });
            //}

            var accountReportDtos = mapper.Map<List<AccountReportDto>>(bankAccounts);

            return Ok(accountReportDtos);
        }

        [HttpGet]
        [Route("accounts/{id:guid}/transactions")]
        [Authorize(Roles = "Auditor, Admin")]
        public async Task<IActionResult> GetTransactionReports([FromRoute] Guid id)
        {
            var transactions = await reportsRepository.GetTransactionReports(id);

            // Map Transaction Domain Models to Transaction Report DTO List

            //foreach (var transaction in transactions)
            //{
            //    transactionReportDtos.Add(new TransactionReportDto()
            //    {
            //        Action = transaction.Action == 0 ? "Depozitim" : "Terheqje",
            //        Amount = transaction.Amount,
            //        Date = transaction.DateCreated
            //    });
            //}

            var transactionReportDtos = mapper.Map<List<TransactionReportDto>>(transactions);

            return Ok(transactionReportDtos);
        }

        [HttpGet]
        [Route("clients/{id:guid}/accounts")]
        [Authorize(Roles = "Client, Auditor, Admin")]
        public async Task<IActionResult> GetClientAccountReports([FromRoute] Guid id)
        {
            var bankAccounts = await reportsRepository.GetClientAccountReports(id);

            // Map Bank Account Domain Models to Account Report DTO List

            //foreach (var account in bankAccounts)
            //{
            //    clientAccountReportDtos.Add(new ClientAccountReportDto()
            //    {
            //        AccountCode = account.Code,
            //        AccountName = account.Name,
            //        Currency = account.CurrencyId,
            //        CurrentBalance = account.Balance
            //    });
            //}

            var clientAccountReportDtos = mapper.Map<List<ClientAccountReportDto>>(bankAccounts);

            return Ok(clientAccountReportDtos);
        }
    }
}
