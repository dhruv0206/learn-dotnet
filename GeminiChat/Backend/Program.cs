using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Adds the UI at /scalar/v1
}

app.UseCors("AllowAll");
// app.UseHttpsRedirection(); // Disable HTTPS redirection for local development to avoid port warnings

app.MapPost("/api/chat", async ([FromBody] ChatRequest request, IHttpClientFactory httpClientFactory) =>
{
    // Use the API key provided in code or environment variable
    var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "AIzaSyBkukDO4ih5Lu8f4pzOGYQ8V69usLmPbCo";
    
    // Only fail if the key is empty or is the generic placeholder
    if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "YOUR_API_KEY_HERE")
    {
        return Results.Problem("API Key is missing. Please set GEMINI_API_KEY environment variable or update the code.");
    }

    var client = httpClientFactory.CreateClient();
    
    var geminiRequest = new
    {
        contents = new[]
        {
            new { parts = new[] { new { text = request.Message } } }
        }
    };

    var jsonContent = new StringContent(
        JsonSerializer.Serialize(geminiRequest),
        Encoding.UTF8,
        "application/json");

    // Using gemini-2.5-pro as requested
    var response = await client.PostAsync(
        $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={apiKey}",
        jsonContent);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[Gemini API Error] Status: {response.StatusCode}");
        Console.WriteLine($"[Gemini API Error] Details: {errorContent}");
        return Results.Problem($"Error calling Gemini API: {response.ReasonPhrase}. Details: {errorContent}");
    }

    var responseString = await response.Content.ReadAsStringAsync();
    
    try 
    {
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var text = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "No response from AI.";
        return Results.Ok(new ChatResponse(text));
    }
    catch (Exception ex)
    {
        return Results.Problem($"Failed to parse response: {ex.Message}");
    }
});

app.Run();

// Models
record ChatRequest(string Message);
record ChatResponse(string Response);

// Gemini API Response Models
public class GeminiResponse
{
    public Candidate[]? Candidates { get; set; }
}

public class Candidate
{
    public Content? Content { get; set; }
}

public class Content
{
    public Part[]? Parts { get; set; }
}

public class Part
{
    public string? Text { get; set; }
}
