using AccountManagement.CustomActionFilters;
using AccountManagement.Data;
using AccountManagement.DTO;
using AccountManagement.Models.Domain;
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
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountRepository bankAccountRepository;
        private readonly IMapper mapper;

        public BankAccountsController(IBankAccountRepository bankAccountRepository, IMapper mapper)
        {
            this.bankAccountRepository = bankAccountRepository;
            this.mapper = mapper;
        }

        // Get all bank accounts
        [HttpGet]
        [Authorize(Roles = "Auditor, Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bankAccounts = await bankAccountRepository.GetAllAsync();

            // Conversion from Domain Model to DTO

            //var bankAccountDtos = new List<BankAccountDto>();

            //foreach (var bankAccount in bankAccounts)
            //{
            //    bankAccountDtos.Add(new BankAccountDto()
            //    {
            //        Id = bankAccount.Id,
            //        Code = bankAccount.Code,
            //        Name = bankAccount.Name,
            //        IsActive = bankAccount.IsActive,
            //        Balance = bankAccount.Balance,
            //        DateCreated = bankAccount.DateCreated,
            //        DateModified = bankAccount.DateModified,
            //        CurrencyId = bankAccount.CurrencyId,
            //        ClientId = bankAccount.ClientId
            //    });

            var bankAccountDtos = mapper.Map<List<BankAccountDto>>(bankAccounts);

            return Ok(bankAccountDtos);
        }

        // Get bank account by id
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Client, Auditor, Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var bankAccount = await bankAccountRepository.GetByIdAsync(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO

            var bankAccountDto = mapper.Map<BankAccountDto>(bankAccount);

            return Ok(bankAccountDto);
        }

        // Create bank account
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> Create(AddBankAccountDto addBankAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!User.IsInRole("Admin") &&
                addBankAccountDto.ClientId.ToString() != User.FindFirstValue("clientId"))
            {
                return Forbid();
            }

            if (addBankAccountDto.Balance < 0)
            {
                return BadRequest("Balance cannot be negative.");
            }

            var existingAccounts = await bankAccountRepository.GetAllAsync();

            if (existingAccounts.Any(account => account.ClientId == addBankAccountDto.ClientId && account.IsActive))
            {
                return BadRequest("Client already has a bank account.");
            }

            // Conversion from DTO to Domain Model
            var bankAccountDomainModel = mapper.Map<BankAccount>(addBankAccountDto);
            bankAccountDomainModel.Id = Guid.NewGuid();
            bankAccountDomainModel.DateCreated = DateTime.UtcNow;

            bankAccountDomainModel = await bankAccountRepository.CreateAsync(bankAccountDomainModel);

            // Conversion from Domain Model to DTO
            var bankAccountDto = mapper.Map<BankAccountDto>(bankAccountDomainModel);

            return Ok(bankAccountDto);
        }

        // Update bank account
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBankAccountDto updateBankAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Conversion from DTO to Domain Model
            var bankAccountDomainModel = mapper.Map<BankAccount>(updateBankAccountDto);
            bankAccountDomainModel.Id = Guid.NewGuid();
            bankAccountDomainModel.DateCreated = DateTime.UtcNow;

            bankAccountDomainModel = await bankAccountRepository.UpdateAsync(id, bankAccountDomainModel);

            if (bankAccountDomainModel == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO
            var bankAccountDto = mapper.Map<BankAccountDto>(bankAccountDomainModel);

            return Ok(bankAccountDto);
        }

        // Delete bank account
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var bankAccountDomainModel = await bankAccountRepository.DeleteAsync(id);

            if (bankAccountDomainModel == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO
            var bankAccountDto = mapper.Map<BankAccountDto>(bankAccountDomainModel);

            return Ok(bankAccountDto);
        }
    }
}
