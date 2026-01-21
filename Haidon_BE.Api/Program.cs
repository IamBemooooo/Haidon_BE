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

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
