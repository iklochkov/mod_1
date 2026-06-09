using MyCRM.Api.Services.Impl;
using MyCRM.Api.Services;
using MyCRM.Infrastructure.Services;
using MyCRM.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
// ===== ДОБАВИТЬ ЭТИ USING =====
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// ===== КОНЕЦ ДОБАВЛЕНИЙ =====
using MyCRM.Application.Interfaces;
using MyCRM.Infrastructure.Cache;
using MyCRM.Infrastructure.Persistence;
using MyCRM.Application.Interfaces;
using MyCRM.Infrastructure.Repositories;
using MyCRM.Application.Interfaces;
using MyCRM.Api.Mapper;
using MyCRM.Api.Services.Impl;
using MyCRM.Api.Services.Impl;
using NReco.Logging.File;
using MyCRM.Api.Services.Impl;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using MyCRM.Infrastructure.UnitOfWork;
using MyCRM.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ===== ЗАГРУЗКА КОНФИГУРАЦИИ =====
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//====== JWT =======
var jwtOptions = new JwtOptions();
builder.Configuration.GetSection("Jwt").Bind(jwtOptions);


// ===== ЛОГИРОВАНИЕ =====
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
builder.Logging.AddFile(logFilePath, options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.MinLevel = LogLevel.Debug;
        options.MaxRollingFiles = 5;
        options.FileSizeLimitBytes = 10 * 1024 * 1024;
    }
    else
    {
        options.MinLevel = LogLevel.Warning;
        options.MaxRollingFiles = 10;
        options.FileSizeLimitBytes = 50 * 1024 * 1024;
    }
});

// ===== РЕГИСТРАЦИЯ СЕРВИСОВ (DI) =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ===== ДОБАВИТЬ IDENTITY =====
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ===== ДОБАВИТЬ JWT АУТЕНТИФИКАЦИЮ =====
var key = Encoding.ASCII.GetBytes(jwtOptions.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = false,
        ValidateAudience = false
    };
    
    // ===== ДОБАВИТЬ ЭТО ДЛЯ SIGNALR =====
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

// ===== ДОБАВИТЬ АВТОРИЗАЦИЮ =====
builder.Services.AddAuthorization();

// ===== ВАШИ СУЩЕСТВУЮЩИЕ СЕРВИСЫ =====
builder.Services.AddScoped<IClientRepository, DbClientRepository>();
builder.Services.AddScoped<IDealRepository, DbDealRepository>();
builder.Services.AddScoped<IContactRepository, DbContactRepository>();
builder.Services.AddScoped<ITaskRepository, DbTaskRepository>();
builder.Services.AddScoped<IAnalyticsRepository, DbAnalyticsRepository>();


builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IHotClientsCache, HotClientsCacheImpl>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<CsvExportService>();
builder.Services.AddScoped<NotificationService>();


builder.Services.AddScoped<IClientWebService, ClientWebService>();
builder.Services.AddScoped<IDealWebService, DealWebService>();
builder.Services.AddScoped<IJwtService, JwtServiceImpl>();
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSignalR();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// MediatR для CQRS
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(MyCRM.Application.DTOs.ClientDto).Assembly));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database", tags: new[] { "ready", "live" })
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

builder.Services.AddResponseCaching();

// ===== НАСТРОЙКА HSTS =====
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// HSTS только для production
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

// ===== ВАЖНО: ПОРЯДОК MIDDLEWARE =====
app.UseAuthentication();  // ← ДОБАВИТЬ (СНАЧАЛА АУТЕНТИФИКАЦИЯ)
app.UseAuthorization();   // ← ДОБАВИТЬ (ПОТОМ АВТОРИЗАЦИЯ)

app.MapHub<NotificationHub>("/notificationHub");

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = feature?.Error;

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Internal Server Error",
            status = StatusCodes.Status500InternalServerError,
            detail = exception?.Message ?? "Произошла непредвиденная ошибка",
            instance = context.Request.Path.ToString()
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseResponseCaching(); 
app.MapControllers();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

// ===== ИНИЦИАЛИЗАЦИЯ РОЛЕЙ =====
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    
    // Создаем роли
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    if (!await roleManager.RoleExistsAsync("Manager"))
        await roleManager.CreateAsync(new IdentityRole("Manager"));
    
    // Создаем админа
    var adminEmail = "admin@crm.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.AddToRoleAsync(admin, "Manager");
        Console.WriteLine("✅ Администратор создан: admin@crm.com / Admin123!");
    }
}

app.Run();