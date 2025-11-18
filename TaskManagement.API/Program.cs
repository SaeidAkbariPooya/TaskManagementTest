using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text;
using TaskManagement.API.Configurations;
using TaskManagement.API.Middlewares;
using TaskManagement.Application;
using TaskManagement.Core.IRepositories;
using TaskManagement.Core.IServices;
using TaskManagement.Infra.Repositories;
using TaskManagement.Infra.Service;

var builder = WebApplication.CreateBuilder(args); // ساده‌سازی CreateBuilder

// افزودن لاگینگ دقیق
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// پیکربندی Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)

    // Enrichers
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "TaskManager.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)

    // Console Sink
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Code,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s{SourceContext}>{NewLine}{Exception}"
    )

    // File Sink
    .WriteTo.File(
        path: Path.Combine("Logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1),
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )

    // JSON File Sink
    .WriteTo.File(
        new CompactJsonFormatter(),
        Path.Combine("Logs", "log-.json"),
        rollingInterval: RollingInterval.Day
    )

    // Seq Sink
    .WriteTo.Seq(
        serverUrl: builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341",
        apiKey: builder.Configuration["Seq:ApiKey"],
        controlLevelSwitch: null
    )
    .CreateLogger();

builder.Host.UseSerilog();

// اضافه کردن سرویس‌ها
builder.Services.AddDatabaseSetup(builder.Configuration);

// کنترلرها
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.ConfigureApplicationServices();

builder.Services.AddEndpointsApiExplorer();

// Reoisitory DI
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service DI
//builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();



builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ticket System API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// JWT Authentication - SIMPLE AND WORKING VERSION
var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong123!");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Temporarily disable for testing
        ValidateAudience = false, // Temporarily disable for testing
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    x.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token Validated Successfully");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();

// CORS

builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowAll",
                 builder =>
                 {
                     builder.WithOrigins("*");
                     builder.WithHeaders("*");
                     builder.WithMethods("*");
                     builder.SetIsOriginAllowed(origin => true);
                 });
});

var app = builder.Build();

// 🔹 میدلور سفارشی - اینجا ثبت شود
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRequestLogging();

// ✅ **اصلاح مهم: شرطی کردن UseHttpsRedirection**
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // فقط در production از HTTPS استفاده کن
}

// خطایابی: اضافه کردن middleware خطایابی
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// اضافه کردن endpoint برای خطا
app.Map("/error", () => Results.Problem("An error occurred."));

try
{
    Log.Information("Starting Task Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}