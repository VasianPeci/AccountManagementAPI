using AccountManagement.Data;
using AccountManagement.Data.Seeding;
using AccountManagement.Mappings;
using AccountManagement.Models.Identity;
using AccountManagement.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// DB Contexts
builder.Services.AddDbContext<AccountManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountManagementConnectionString"))
);

builder.Services.AddDbContext<AccountManagementAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountManagementAuthConnectionString"))
);

// Repositories
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
builder.Services.AddScoped<IReportsRepository, ReportsRepository>();

// JWT Authentication — must be registered BEFORE Identity
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

// Identity — AddIdentityCore does NOT override authentication schemes
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AccountManagementAuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Build App
var app = builder.Build();

// SEEDING
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = services.GetRequiredService<AccountManagementDbContext>();

    var identitySeeder = new IdentitySeeder(userManager);
    await identitySeeder.SeedUsersAsync();

    var clientSeeder = new ClientSeeder(dbContext, userManager);
    await clientSeeder.SeedAsync();
}

// Middleware
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();