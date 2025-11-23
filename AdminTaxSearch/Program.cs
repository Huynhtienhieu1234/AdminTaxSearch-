using AdminTaxSearch.Data;
using AdminTaxSearch.Services;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AdminTaxSearch.Options;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// 1. Add Services
// ----------------------------
builder.Services.AddControllers();


builder.Services.AddControllers();




// Bind JwtOptions từ appsettings.json
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwtConfig = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
var key = Encoding.UTF8.GetBytes(jwtConfig.SecretKey);
builder.Services.AddHttpClient();
builder.Services.AddScoped<ChatbotService>();
builder.Services.AddScoped<TaxService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


// DbContext SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS để Angular 20 có thể gọi
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4300")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ----------------------------
// Gemini / AI Service
// ----------------------------
builder.Services.AddSingleton<GoogleAI>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["Gemini:ApiKey"];
    return new GoogleAI(apiKey);
});
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApikeyService, ApikeyService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();




// Swagger (tuỳ chọn, để test API dễ)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------
// 2. Build App
// ----------------------------
var app = builder.Build();

// ----------------------------
// 3. Middleware
// ----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map các Controller
app.MapControllers();

app.Run();
