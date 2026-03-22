using TestPlatform.Api.Endpoints;
using TestPlatform.Application;
using TestPlatform.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.Services.InitializePersistenceAsync(app.Environment.IsDevelopment());

app.UseHttpsRedirection();

app.MapQuestionEndpoints();
app.MapTestEndpoints();
app.MapUserEndpoints();

app.Run();
