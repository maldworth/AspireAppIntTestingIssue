using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client.Domain.Builders.Faceted;
using Gotenberg.Sharp.API.Client.Domain.Settings;
using Gotenberg.Sharp.API.Client.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddOptions<GotenbergSharpClientOptions>()
            .Bind(builder.Configuration.GetSection(nameof(GotenbergSharpClient)));
builder.Services.AddGotenbergSharpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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
});

app.MapGet("/generatepdf", async ([FromServices] GotenbergSharpClient gotenbergSharpClient) =>
{
    var html = "<html><h1>Hello, World!</h1></html>";


    var builder = new HtmlRequestBuilder()
            .AddDocument(doc =>
                doc.SetBody(html)
            ).WithDimensions(dims =>
            {
                dims.SetPaperSize(PaperSizes.Letter)
                    .SetMargins(Margins.Normal)
                    .SetScale(.99);
            });

    var request = await builder.BuildAsync();

    var result = await gotenbergSharpClient.HtmlToPdfAsync(request, CancellationToken.None);

    return Results.File(result, "application/pdf", "output.pdf");
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
