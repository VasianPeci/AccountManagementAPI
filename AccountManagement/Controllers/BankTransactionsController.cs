using AccountManagement.CustomActionFilters;
using AccountManagement.Data;
using AccountManagement.DTO;
using AccountManagement.Models.Domain;
using AccountManagement.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransactionsController : ControllerBase
    {
        private readonly IBankTransactionRepository bankTransactionRepository;
        private readonly IMapper mapper;
        private readonly AccountManagementDbContext dbContext;

        public BankTransactionsController(IBankTransactionRepository bankTransactionRepository, IMapper mapper, AccountManagementDbContext dbContext)
        {
            this.bankTransactionRepository = bankTransactionRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        // Get all bank transactions
        [HttpGet]
        [Authorize(Roles = "Auditor, Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bankTransactions = await bankTransactionRepository.GetAllAsync();

            // Convert from Domain Model to DTO

            //var bankTransactionDtos = new List<BankTransactionDto>();

            //foreach (var transaction in bankTransactions)
            //{
            //    bankTransactionDtos.Add(new BankTransactionDto()
            //    {
            //        Id = transaction.Id,
            //        BankAccountId = transaction.BankAccountId,
            //        Action = transaction.Action,
            //        Amount = transaction.Amount,
            //        IsActive = transaction.IsActive,
            //        DateCreated = transaction.DateCreated,
            //        DateModified = transaction.DateModified
            //    });
            //}

            var bankTransactionDtos = mapper.Map<List<BankTransactionDto>>(bankTransactions);

            return Ok(bankTransactionDtos);
        }

        // Get transaction by id
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Client, Auditor, Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var transaction = await bankTransactionRepository.GetByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO
            var bankTransactionDto = mapper.Map<BankTransactionDto>(transaction);

            return Ok(bankTransactionDto);
        }

        // Create transaction
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> Create([FromBody] AddBankTransactionDto addBankTransactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (addBankTransactionDto.Action != 0 && addBankTransactionDto.Action != 1)
            {
                return BadRequest("Transaction action must be 0 for deposit or 1 for withdrawal.");
            }

            if (addBankTransactionDto.Amount <= 0)
            {
                return BadRequest("Transaction amount must be greater than zero.");
            }

            var bankAccount = await dbContext.BankAccounts.FirstOrDefaultAsync(x => x.Id == addBankTransactionDto.BankAccountId);

            if (bankAccount == null)
            {
                return NotFound("Bank account not found");
            }

            if (!User.IsInRole("Admin") &&
                bankAccount.ClientId.ToString() != User.FindFirstValue("clientId"))
            {
                return Forbid();
            }

            if (addBankTransactionDto.Action == 1 && addBankTransactionDto.Amount > bankAccount.Balance)
            {
                return BadRequest("Withdrawal amount cannot exceed the current balance.");
            }

            // Conversion from DTO to Domain Model

            var transactionDomainModel = mapper.Map<BankTransaction>(addBankTransactionDto);
            transactionDomainModel.Id = Guid.NewGuid();
            transactionDomainModel.DateCreated = DateTime.UtcNow;

            transactionDomainModel = await bankTransactionRepository.CreateAsync(transactionDomainModel);

            // Conversion from Domain Model to DTO

            var bankTransactionDto = mapper.Map<BankTransactionDto>(transactionDomainModel);

            return Ok(bankTransactionDto);
        }

        // Update transaction
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBankTransactionDto updateBankTransactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Conversion from DTO to Domain Model

            var transactionDomainModel = mapper.Map<BankTransaction>(updateBankTransactionDto);
            transactionDomainModel.Id = Guid.NewGuid();
            transactionDomainModel.DateCreated = DateTime.UtcNow;

            transactionDomainModel = await bankTransactionRepository.UpdateAsync(id, transactionDomainModel);

            if (transactionDomainModel == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO

            var bankTransactionDto = mapper.Map<BankTransactionDto>(transactionDomainModel);

            return Ok(bankTransactionDto);
        }

        // Delete transaction
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var transactionDomainModel = await bankTransactionRepository.DeleteAsync(id);

            if (transactionDomainModel == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO

            var bankTransactionDto = mapper.Map<BankTransactionDto>(transactionDomainModel);

            return Ok(bankTransactionDto);
        }
    }
}
