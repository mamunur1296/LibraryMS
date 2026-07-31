using System.Text;
using Hangfire;
using LibraryMS.Application;
using LibraryMS.Application.BackgroundJobs;
using LibraryMS.EntityFrameworkCore;
using LibraryMS.HttpApi.Controllers;
using LibraryMS.HttpApi.Host.Middleware;
using LibraryMS.Infrastructure;
using LibraryMS.Infrastructure.Jobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// 1. Add Layer Services
builder.Services.AddEntityFrameworkCoreServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. Add Controllers from HttpApi project
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LibraryMS.HttpApi.Filters.ApiResponseFilter>();
})
    .AddApplicationPart(typeof(BaseController).Assembly);

// 3. Configure Authentication (JWT)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// 5. Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Management System API", Version = "v1" });
    
    // Add JWT support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraryMS API v1"));
}

app.UseCors("DefaultPolicy");

// app.UseHttpsRedirection(); // Usually disabled for local dev behind reverse proxy

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Hangfire Dashboard (accessible at /hangfire)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "LibraryMS Job Dashboard",
    // In production, add authorization filter here
});

// Schedule recurring jobs
RecurringJob.AddOrUpdate<OutboxProcessorJob>(
    "outbox-processor",
    job => job.ProcessAsync(CancellationToken.None),
    "*/30 * * * * *",  // Every 30 seconds
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc, QueueName = "outbox" });

RecurringJob.AddOrUpdate<OverdueCheckJob>(
    "overdue-checker",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(2), // Every day at 02:00 UTC
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

RecurringJob.AddOrUpdate<ReservationExpiryJob>(
    "reservation-expiry",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Hourly(), // Every hour
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

RecurringJob.AddOrUpdate<DailyFineAccumulationJob>(
    "daily-fine-accumulation",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(0), // Every day at 00:00 UTC
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

RecurringJob.AddOrUpdate<DueDateReminderJob>(
    "due-date-reminder",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(9), // Every day at 09:00 UTC
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

RecurringJob.AddOrUpdate<MembershipExpiryJob>(
    "membership-expiry",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(8), // Every day at 08:00 UTC
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

app.Run();
