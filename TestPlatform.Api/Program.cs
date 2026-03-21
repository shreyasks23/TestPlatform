using TestPlatform.Api.Endpoints;
using TestPlatform.Application;
using TestPlatform.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddPersistence();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapQuestionEndpoints();
app.MapTestEndpoints();

app.Run();
