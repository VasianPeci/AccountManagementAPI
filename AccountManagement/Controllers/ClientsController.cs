using AccountManagement.CustomActionFilters;
using AccountManagement.CustomActionFilters;
using AccountManagement.Data;
using AccountManagement.DTO;
using AccountManagement.Models.Domain;
using AccountManagement.Models.Identity;
using AccountManagement.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AccountManagementDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IClientRepository clientRepository;
        private readonly IMapper mapper;

        public ClientsController(AccountManagementDbContext dbContext, UserManager<ApplicationUser> userManager, IClientRepository clientRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.clientRepository = clientRepository;
            this.mapper = mapper;
        }

        // Get all clients
        [HttpGet]
        [Authorize(Roles = "Auditor, Admin")]
        public async Task<IActionResult> GetAll()
        {
            var clients = await clientRepository.GetAllAsync();

            // Conversion from Client Domain Model to DTO

            var dtoClients = new List<ClientDto>();

            foreach (var client in clients)
            {
                var dto = mapper.Map<ClientDto>(client);

                var user = await userManager.FindByIdAsync(client.UserId);

                if (user != null)
                {
                    dto.Username = user.UserName;

                    var roles = await userManager.GetRolesAsync(user);

                    dto.Roles = roles.ToArray();
                }

                dtoClients.Add(dto);
            }

            return Ok(dtoClients);
        }

        // Get client by id
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Client, Auditor, Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var client = await clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            // Conversion from Client Domain Model to DTO

            // Base mapping
            var clientDto = mapper.Map<ClientDto>(client);

            // Identity data
            var user = await userManager.FindByIdAsync(client.UserId);

            if (user != null)
            {
                clientDto.Username = user.UserName;

                var roles = await userManager.GetRolesAsync(user);

                clientDto.Roles = roles.ToArray();
            }

            return Ok(clientDto);
        }

        // Create a client
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> CreateClient([FromBody] RegisterRequestDto dto)
        {
            var requestedRole = dto.Roles?.FirstOrDefault()?.Trim();
            var normalizedRequestedRole = requestedRole?.ToLowerInvariant();
            var isAdminRegistration =
                string.Equals(dto.FirstName?.Trim(), "admin", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(dto.LastName?.Trim(), "admin", StringComparison.OrdinalIgnoreCase);
            var roleToAssign = requestedRole;

            if (isAdminRegistration)
            {
                var existingAdmins = await userManager.GetUsersInRoleAsync("Admin");

                if (existingAdmins.Any())
                {
                    return BadRequest("Admin can only be registered once.");
                }

                roleToAssign = "Admin";
            }
            else if (normalizedRequestedRole == "client")
            {
                roleToAssign = "Client";
            }
            else if (normalizedRequestedRole == "auditor")
            {
                roleToAssign = "Auditor";
            }
            else
            {
                return BadRequest("Role must be Client or Auditor.");
            }

            // Create identity first

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Username
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Add roles to created identity

            if (!string.IsNullOrEmpty(roleToAssign))
            {
                var roleResult = await userManager.AddToRoleAsync(user, roleToAssign);

                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }
            }

            // Create client linked to created identity

            var client = new Client
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Birthdate = dto.Birthdate,
                Phone = dto.Phone,
                DateCreated = DateTime.UtcNow,
                UserId = user.Id
            };

            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync();

            return Ok("Client created successfully");
        }

        // Update a client
        [HttpPost]
        [Route("{id:guid}")]
        [ValidateModel]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateClientRequestDto updateClientRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Find client to update

            var clientDomainModel = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id);

            if (clientDomainModel == null)
            {
                return NotFound("Client not found");
            }

            if (!User.IsInRole("Admin") &&
                clientDomainModel.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            // Find and update identity linked to client

            var user = await userManager.FindByIdAsync(clientDomainModel.UserId);

            if (user == null)
            {
                return NotFound("Linked user not found");
            }

            if (!string.IsNullOrEmpty(updateClientRequestDto.Username))
            {
                user.UserName = updateClientRequestDto.Username;
                user.Email = updateClientRequestDto.Username;
            }

            if (!string.IsNullOrEmpty(updateClientRequestDto.Password))
            {
                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, updateClientRequestDto.Password);
            }

            if (updateClientRequestDto.Roles != null && updateClientRequestDto.Roles.Any())
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, currentRoles);
                await userManager.AddToRolesAsync(user, updateClientRequestDto.Roles);
            }

            await userManager.UpdateAsync(user);

            // Update client

            clientDomainModel.FirstName = updateClientRequestDto.FirstName ?? clientDomainModel.FirstName;
            clientDomainModel.LastName = updateClientRequestDto.LastName ?? clientDomainModel.LastName;
            clientDomainModel.Phone = updateClientRequestDto.Phone ?? clientDomainModel.Phone;
            clientDomainModel.DateModified = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            // Conversion from client domain model to dto

            var clientDto = mapper.Map<ClientDto>(clientDomainModel);

            return Ok(clientDto);
        }

        // Delete a client
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Find client to delete

            var client = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id);

            if (client == null)
            {
                return NotFound("Client not found");
            }

            if (!User.IsInRole("Admin") &&
                client.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            // Delete identity linked to client

            var user = await userManager.FindByIdAsync(client.UserId);

            if (user == null)
            {
                return NotFound("Linked user not found");
            }

            var identityResult = await userManager.DeleteAsync(user);

            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            // Delete client

            dbContext.Clients.Remove(client);
            await dbContext.SaveChangesAsync();

            return Ok("Client deleted successfully");
        }
    }
}
