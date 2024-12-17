using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using api.Models;
using api.Services;
using api.Configuration;
using Microsoft.Extensions.Options;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db")); // SQLite database file

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Bind OpenAiSettings from configuration
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAiSettings"));

// Register OpenAiService as a typed client
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<OpenAiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
    client.DefaultRequestHeaders.Add("OpenAI-Beta", settings.AssistantsApiVersion);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();


// Polly policies
    IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Optionally log retry attempts
                    var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>();
                    logger?.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}.");
                });
    }
    IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>();
                    logger?.LogWarning($"Circuit breaker opened for {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}.");
                },
                onReset: () =>
                {
                    var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>();
                    logger?.LogInformation("Circuit breaker reset.");
                });
    }

