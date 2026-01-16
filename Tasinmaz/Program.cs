using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Business.Concrete;
using Tasinmaz.Controllers;
using Tasinmaz.Data;
using Tasinmaz.DataAccess;




QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("PostgreSqlConnection"),
        o => o.UseNetTopologySuite()
    )
);


builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


builder.Services.AddScoped<ITasinmazService, TasinmazService>();
builder.Services.AddScoped<IIlService, IlService>();
builder.Services.AddScoped<IIlceService, IlceService>();
builder.Services.AddScoped<IMahalleService, MahalleService>();
builder.Services.AddScoped<IAreaAnalysisService, AreaAnalysisService>();



builder.Services.AddScoped<IAuthRepository, AuthRepository>();


builder.Services.AddLogging();
builder.Services.AddControllers();


builder.Services.AddRazorPages();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (secretKey == null || secretKey.Length < 64)
    throw new Exception("JwtSettings:SecretKey must be at least 64 characters for HMAC-SHA512.");

var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
 
    options.RequireHttpsMetadata = false;


    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
      
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

      
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtSettings["Issuer"]),
        ValidIssuer = jwtSettings["Issuer"],

     
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtSettings["Audience"]),
        ValidAudience = jwtSettings["Audience"],

      
        ValidateLifetime = true,

       
        ClockSkew = TimeSpan.Zero,

        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

 
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "REMS API", Version = "v1" });

  
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT yetkilendirmesi için Bearer şemasını kullanın. Örn: 'Bearer {token}'",
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseStaticFiles();

app.Run();




