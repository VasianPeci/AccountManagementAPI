using AccountManagement.CustomActionFilters;
using AccountManagement.Data;
using AccountManagement.DTO;
using AccountManagement.Models.Domain;
using AccountManagement.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IMapper mapper;

        public CurrenciesController(ICurrencyRepository currencyRepository, IMapper mapper)
        {
            this.currencyRepository = currencyRepository;
            this.mapper = mapper;
        }

        // Get all currencies
        [HttpGet]
        [Authorize(Roles = "Client, Auditor, Admin")]
        public async Task<IActionResult> GetAll()
        {
            var currencies = await currencyRepository.GetAllAsync();

            // Convert from Domain Model to DTO

            //foreach (var currency in currencies)
            //{
            //    currencyDtos.Add(new CurrencyDto()
            //    {
            //        Id = currency.Id,
            //        Code = currency.Code,
            //        ExchangeRate = currency.ExchangeRate,
            //        Description = currency.Description,
            //        DateCreated = currency.DateCreated,
            //        DateModified = currency.DateModified
            //    });
            //}

            var currencyDtos = mapper.Map<List<CurrencyDto>>(currencies);

            return Ok(currencyDtos);
        }

        // Get currency by id
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Client, Auditor, Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var currency = await currencyRepository.GetByIdAsync(id);

            if (currency == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO

            var currencyDto = mapper.Map<CurrencyDto>(currency);

            return Ok(currencyDto);
        }

        // Create currency
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AddCurrencyDto addCurrencyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Conversion from DTO to Domain Model

            var currencyDomainModel = mapper.Map<Currency>(addCurrencyDto);

            currencyDomainModel.Id = Guid.NewGuid();
            currencyDomainModel.DateCreated = DateTime.UtcNow;

            currencyDomainModel = await currencyRepository.CreateAsync(currencyDomainModel);

            // Conversion from Domain Model to DTO

            var currencyDto = mapper.Map<CurrencyDto>(currencyDomainModel);

            return Ok(currencyDto);
        }

        // Update currency
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCurrencyDto updateCurrencyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Conversion from DTO to Domain Model

            var currencyDomainModel = mapper.Map<Currency>(updateCurrencyDto);

            currencyDomainModel.Id = Guid.NewGuid();
            currencyDomainModel.DateCreated = DateTime.UtcNow;

            currencyDomainModel = await currencyRepository.UpdateAsync(id, currencyDomainModel);

            if (currencyDomainModel == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO

            var currencyDto = mapper.Map<CurrencyDto>(currencyDomainModel);

            return Ok(currencyDto);
        }

        // Delete currency
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var currencyDomainModel = await currencyRepository.DeleteAsync(id);

            if (currencyDomainModel == null)
            {
                return NotFound();
            }

            // Conversion from Domain Model to DTO

            var currencyDto = mapper.Map<CurrencyDto>(currencyDomainModel);

            return Ok(currencyDto);
        }
    }
}
