using System.Text;
using System.Text.Json.Serialization;
using BL.Repositories.Company;
using BL.Repositories.ConfirmationCode;
using BL.Repositories.Document;
using BL.Repositories.SignOperation;
using BL.Repositories.User;
using Common.Interfaces;
using Common.Services.CodeService;
using Common.Services.DocumentWatermarkService;
using Common.Services.FileService;
using Common.Services.QrCodeService;
using DAL;
using DAL.Models;
using DataTransferObjects;
using DauInvestTest.WebApp.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication
    .CreateBuilder(args);
    
    var env = builder.Environment;
    
// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles;
        
    });

if (env.IsProduction())
{
    builder.Configuration.AddEnvironmentVariables();
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var configuration = builder.Configuration;

ConfigureLogging(configuration);
builder.Host.UseSerilog();

// For Entity Framework
builder.Services.AddDbContext<AppDbContext>(options => options
    .UseNpgsql(configuration.GetConnectionString("Default")));

// For Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

// Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
    });

builder.Services.ConfigureAutomapper();
builder.Services.AddVolumeFileService(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "DauInvest Test Api", 
        Version = "v1" 
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, 
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey 
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                } 
            },
            new string[] { } 
        } 
    });
    c.OperationFilter<FileOperationFilter>();
    c.CustomSchemaIds(type => type.FullName);
});

//services
builder.Services.AddScoped<IRepository<CompanyDto>, CompanyRepository>();
builder.Services.AddScoped<IRepository<UserDto>, UserRepository>();
builder.Services.AddScoped<IRepository<DocumentDto>, DocumentRepository>();
builder.Services.AddScoped<IRepository<CompanyDto>, CompanyRepository>();
 builder.Services.AddScoped<IRepository<SignOperationDto>, SignOperationRepository>();
builder.Services.AddScoped<IRepository<ConfirmationCodeDto>, ConfirmationCodeRepository>();

builder.Services.AddScoped<DocumentWatermarkService>();
builder.Services.AddScoped<QrCodeService>();

builder.Services.AddScoped<ICodeService, CodeServiceMock>();
var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

 app.UseRouting();
    // app.UseEndpoints(endpoints =>
    // {
    //     endpoints.MapODataRoute("odata", "odata", AppEdmModel.GetEdmModel());
    // });

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
    Log.Information("Application started");


    void ConfigureLogging(IConfigurationRoot configuration)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.Debug()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}