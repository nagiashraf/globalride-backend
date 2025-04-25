using System.Text.Json.Serialization;

using GlobalRide.Api.Extensions;
using GlobalRide.Api.Extensions.DataSeed;
using GlobalRide.Api.Middleware;
using GlobalRide.Api.Utils;
using GlobalRide.Application;
using GlobalRide.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Client>(builder.Configuration.GetSection(nameof(Client)));

// Retrieve the client URL from the configuration
var clientOptions = builder.Configuration.GetSection(nameof(Client)).Get<Client>();
var clientUrl = clientOptions?.Url ?? throw new InvalidOperationException("Client URL is not configured.");

builder.Services.AddCors(options =>
    options.AddPolicy(
        "AllowVite", policy => policy
            .WithOrigins(clientUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()));

builder.Services.AddHealthChecks();

string connectionString =
    builder.Configuration.GetConnectionString("SqlServerConnection")!;

builder.Services
    .AddApplication()
    .AddInfrastructure(connectionString, builder.Environment.ContentRootPath);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrationsAsync();
}

var seedFilesDirectoryPath = Path.Combine(app.Environment.ContentRootPath);
await app.SeedData(seedFilesDirectoryPath);

app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseCors("AllowVite");

app.UseMiddleware<LocalizationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

await app.RunAsync();
