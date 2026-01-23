using Haidon_BE.Application;
using Haidon_BE.Application.Services;
using Haidon_BE.Infrastructure;
using Haidon_BE.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Wire application and infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<SeedPermissionService>();
builder.Services.AddScoped<IChatHub, ChatHubService>();

// Add JWT authentication for SignalR
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_DEMO_SECRET_CHANGE_ME_TO_A_LONG_RANDOM_VALUE"))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// SignalR + Redis backplane (?? scale)
// - AddSignalR registers SignalR services. N?u có Redis connection string, ta ??ng ký StackExchange Redis backplane
//   ?? các instance c?a ?ng d?ng có th? chia s? messages (khi scale out)
//var redis = builder.Configuration.GetConnectionString("Redis");
//if (!string.IsNullOrWhiteSpace(redis))
//{
//    builder.Services.AddSignalR().AddStackExchangeRedis(redis);
//}
//else
//{
//    builder.Services.AddSignalR();
//}
builder.Services.AddSignalR();
var app = builder.Build();

// Ensure database is migrated and seeded on startup (non-breaking)
await SeedData.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication(); // Thêm dòng này để bật xác thực
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
