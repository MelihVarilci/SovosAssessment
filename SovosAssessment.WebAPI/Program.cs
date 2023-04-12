using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SovosAssessment.Application.Abstractions.Services;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;
using SovosAssessment.WebAPI.Hangfire;
using System.Transactions;
using SovosAssessment.Application.DTOs;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger konfig�rasyonu
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SovosAssessment API", Version = "v1" });
    //options.SchemaFilter<SwaggerJsonSchemaFilter>();
});

var connectionString = builder.Configuration.GetConnectionString("MySql");

// Database Connection
builder.Services.AddDbContext<SovosAssessmentDbContext>(
    opt =>
    {
        opt.UseMySQL(connectionString);
    }, ServiceLifetime.Transient);

// appsettings deki Mail ayarlar�n� Dto nesnesine mapliyoruz
builder.Services.Configure<MailSettingsDto>(builder.Configuration.GetSection("MailSettings"));

// Dependency Injection
builder.Services.AddTransient<MailService>();
//builder.Services.AddTransient<IInvoiceRepository, InvoiceRepository>();
//builder.Services.AddTransient<IInvoiceLineRepository, InvoiceLineRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
        options.SerializerSettings.Formatting = Formatting.Indented;
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

// Cors politikas�n� burada ayarl�yoruz.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("" +
            "http://localhost:4200/",
            "https://localhost:4200/")
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

// Serilog konfig�rasyonu
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
        TablesPrefix = "Hangfire_" // Tablo �neki belirleyin
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

// Her gelen iste�i belirli bir responce modelde d�nebilmek i�in 
//app.UseMiddleware<RequestResponseMiddleware>();

app.UseCors(MyAllowSpecificOrigins);

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard'un herkes taraf�ndan eri�ilebilir �ekilde a��k olmas� bir g�venlik zafiyeti do�urur.
// Hangfire Dashboard'u korumak i�in uygun bir kimlik do�rulama mekanizmas� kullan�lmas� en do�rusu olacakt�r.
app.UseHangfireDashboard("/hangfire");

HangfireService.InitializeJobs();

app.Run();
