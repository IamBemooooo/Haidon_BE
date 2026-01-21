using Haidon_BE.Api.Hubs;
using Haidon_BE.Application;
using Haidon_BE.Infrastructure;
using Haidon_BE.Infrastructure.Persistence;

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

// SignalR + Redis backplane (?? scale)
// - AddSignalR registers SignalR services. N?u có Redis connection string, ta ??ng ký StackExchange Redis backplane
//   ?? các instance c?a ?ng d?ng có th? chia s? messages (khi scale out)
var redis = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redis))
{
    builder.Services.AddSignalR().AddStackExchangeRedis(redis);
}
else
{
    builder.Services.AddSignalR();
}

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

// IMPORTANT: n?u b?n có c?u hình authentication (JWT, cookie...), c?n g?i UseAuthentication() tr??c UseAuthorization().
// ChatHub s? d?ng Context.User ?? l?y userId t? claim; n?u không có middleware authentication thì Context.User có th? r?ng.

app.UseAuthorization();

// Map controllers và map hub.
// MapHub<ChatHub>("/chatHub") ??ng ký endpoint WebSocket/LongPolling cho SignalR hub.
// Client s? k?t n?i t?i /chatHub ?? b?t ??u session SignalR.
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
