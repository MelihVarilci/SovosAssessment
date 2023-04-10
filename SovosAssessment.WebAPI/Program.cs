using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog;
using SovosAssessment.Application.Abstractions.Services;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;
using SovosAssessment.WebAPI.Hangfire;
using System.Transactions;
using FluentAssertions.Common;
using SovosAssessment.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger konfigürasyonu
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SovosAssessment API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("MySql");

// Database Connection
builder.Services.AddDbContext<SovosAssessmentDbContext>(
    opt =>
    {
        opt.UseMySQL(connectionString);
    }, ServiceLifetime.Transient);

// appsettings deki Mail ayarlarýný Dto nesnesine mapliyoruz
builder.Services.Configure<MailSettingsDto>(builder.Configuration.GetSection("MailSettings"));

// Dependency Injection
builder.Services.AddTransient<MailService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Cors politikasýný burada ayarlýyoruz.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("" +
            "http://localhost:4200/",
            "https://localhost:4200/",
            "http://localhost:4200/",
            "http://unitedroyal.net/")
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

// Serilog konfigürasyonu
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();

builder.Logging.AddSerilog(logger);

builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
    config.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions
    {
        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
        QueuePollInterval = TimeSpan.FromSeconds(15),
        JobExpirationCheckInterval = TimeSpan.FromHours(1),
        CountersAggregateInterval = TimeSpan.FromMinutes(5),
        PrepareSchemaIfNecessary = true,
        DashboardJobListLimit = 50000,
        TransactionTimeout = TimeSpan.FromMinutes(1),
        TablesPrefix = "Hangfire_" // Tablo öneki belirleyin
    }));
});

builder.Services.AddHangfireServer();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SovosAssessment API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard'un herkes tarafýndan eriþilebilir þekilde açýk olmasý bir güvenlik zafiyeti doðurur.
// Hangfire Dashboard'u korumak için uygun bir kimlik doðrulama mekanizmasý kullanýlmasý en doðrusu olacaktýr.
app.UseHangfireDashboard("/hangfire");

HangfireService.InitializeJobs();

app.Run();
