using AccountManagement.Data;
using AccountManagement.DTO;
using AccountManagement.Models.Domain;
using AccountManagement.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Globalization;
using System.Security.Claims;

namespace AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IBankTransactionRepository bankTransactionRepository;
        private readonly AccountManagementDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public PaymentsController(
            IBankTransactionRepository bankTransactionRepository,
            AccountManagementDbContext dbContext,
            IMapper mapper,
            IConfiguration configuration)
        {
            this.bankTransactionRepository = bankTransactionRepository;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpPost("topup")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> TopUp(CreateStripePaymentDto dto)
        {
            if (dto.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            var bankAccount = await dbContext.BankAccounts
                .Include(account => account.Currency)
                .FirstOrDefaultAsync(account => account.Id == dto.BankAccountId);

            if (bankAccount == null)
            {
                return NotFound("Bank account not found.");
            }

            if (!bankAccount.IsActive)
            {
                return BadRequest("Bank account is not active.");
            }

            if (bankAccount.ClientId.ToString() != User.FindFirstValue("clientId"))
            {
                return Forbid();
            }

            if (bankAccount.CurrencyId != dto.CurrencyId)
            {
                return BadRequest("Currency does not match the bank account.");
            }

            var currencyCode = bankAccount.Currency.Code.ToLowerInvariant();
            var amountInMinorUnits = GetAmountInMinorUnits(dto.Amount, currencyCode);

            if (amountInMinorUnits <= 0)
            {
                return BadRequest("Invalid amount.");
            }

            var frontendUrl = configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"{frontendUrl}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{frontendUrl}/payment-cancel",
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currencyCode,
                            UnitAmount = amountInMinorUnits,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Top up {bankAccount.Name}"
                            }
                        }
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    ["bankAccountId"] = bankAccount.Id.ToString(),
                    ["clientId"] = bankAccount.ClientId.ToString(),
                    ["amount"] = dto.Amount.ToString(CultureInfo.InvariantCulture),
                    ["currency"] = currencyCode
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        ["bankAccountId"] = bankAccount.Id.ToString(),
                        ["clientId"] = bankAccount.ClientId.ToString()
                    }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new
            {
                session.Id,
                session.Url
            });
        }

        [HttpPost("confirm")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Confirm(ConfirmPaymentDto dto)
        {
            var service = new SessionService();
            var session = await service.GetAsync(dto.SessionId);

            if (session == null)
            {
                return NotFound("Stripe session not found.");
            }

            if (session.PaymentStatus != "paid")
            {
                return BadRequest("Payment has not been completed.");
            }

            var bankAccountId = Guid.Parse(session.Metadata["bankAccountId"]);
            var clientId = Guid.Parse(session.Metadata["clientId"]);
            var amount = decimal.Parse(session.Metadata["amount"], CultureInfo.InvariantCulture);
            var stripePaymentId = session.PaymentIntentId ?? session.Id;

            if (clientId.ToString() != User.FindFirstValue("clientId"))
            {
                return Forbid();
            }

            var bankAccount = await dbContext.BankAccounts
                .FirstOrDefaultAsync(account => account.Id == bankAccountId);

            if (bankAccount == null)
            {
                return NotFound("Bank account not found.");
            }

            if (!bankAccount.IsActive)
            {
                return BadRequest("Bank account is not active.");
            }

            if (bankAccount.ClientId != clientId)
            {
                return Forbid();
            }

            var existingTransaction = await dbContext.BankTransactions
                .FirstOrDefaultAsync(transaction => transaction.StripePaymentId == stripePaymentId);

            if (existingTransaction != null)
            {
                return Ok(mapper.Map<BankTransactionDto>(existingTransaction));
            }

            var transactionDomainModel = new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = bankAccountId,
                Action = 0,
                Amount = amount,
                IsActive = true,
                StripePaymentId = stripePaymentId,
                DateCreated = DateTime.UtcNow
            };

            try
            {
                transactionDomainModel = await bankTransactionRepository.CreateAsync(transactionDomainModel);
            }
            catch (DbUpdateException)
            {
                dbContext.Entry(transactionDomainModel).State = EntityState.Detached;
                await dbContext.Entry(bankAccount).ReloadAsync();

                var transactionAfterDuplicate = await dbContext.BankTransactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(transaction => transaction.StripePaymentId == stripePaymentId);

                if (transactionAfterDuplicate == null)
                {
                    throw;
                }

                return Ok(mapper.Map<BankTransactionDto>(transactionAfterDuplicate));
            }

            return Ok(mapper.Map<BankTransactionDto>(transactionDomainModel));
        }

        private static long GetAmountInMinorUnits(decimal amount, string currencyCode)
        {
            var zeroDecimalCurrencies = new HashSet<string> { "jpy" };

            if (zeroDecimalCurrencies.Contains(currencyCode))
            {
                return (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
            }

            return (long)Math.Round(amount * 100, 0, MidpointRounding.AwayFromZero);
        }
    }
}
