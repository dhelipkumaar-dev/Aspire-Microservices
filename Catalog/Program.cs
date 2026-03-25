using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");

builder.Services.AddScoped<ProductAIService>();
builder.Services.AddScoped<ProductService>();

// Add AI Chat Client
var credential =
    new ApiKeyCredential(
        "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");
var options = new OpenAIClientOptions
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

var openAiClient = new OpenAIClient(credential, options);

var chatClient =
    openAiClient.GetChatClient("openai/gpt-5-mini").AsIChatClient(); // if not worked use: gpt-4o-mini

var embeddingGenerator =
    openAiClient.GetEmbeddingClient("openai/text-embedding-3-small").AsIEmbeddingGenerator();

builder.Services.AddChatClient(chatClient);
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

// Add Vector DB Search Operations
builder.AddQdrantClient("vectordb");
builder.Services.AddQdrantCollection<ulong, ProductVector>("product-vectors");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseMigration();

app.MapProductEndpoints();

app.Run();