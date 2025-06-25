using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();
builder.Services.AddAuthorization();

// Configure authorization
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("AuthZPolicy", policy =>
        policy.RequireRole("Forecast.Read"));
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("weatherForecast")
.RequireAuthorization("AuthZPolicy"); // Protect this endpoint with the AuthZPolicy

app.MapGet("/downstream/weatherforecast", async (IDownstreamApi downstreamApi) =>
{
    var response = await downstreamApi.CallApiForUserAsync("DownstreamApi", options => { options.RelativePath = "weatherforecast"; }).ConfigureAwait(false);
    var forecast = await response.Content.ReadFromJsonAsync<WeatherForecast[]>().ConfigureAwait(false);
    return forecast;
})
.WithName("downstreamWeatherForecast")
.RequireAuthorization("AuthZPolicy"); // Protect this endpoint with the AuthZPolicy

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}