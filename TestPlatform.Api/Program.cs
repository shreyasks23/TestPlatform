using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenTelemetry;
using TestPlatform.Api.Configuration;
using TestPlatform.Api.Endpoints;
using TestPlatform.Api.Services;
using TestPlatform.Application;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Security;
using TestPlatform.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.Issuer) &&
        !string.IsNullOrWhiteSpace(options.Audience) &&
        !string.IsNullOrWhiteSpace(options.SigningKey) &&
        options.AccessTokenLifetimeMinutes > 0 &&
        options.RefreshTokenLifetimeDays > 0,
        "JWT configuration is invalid.")
    .ValidateOnStart();

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
    throw new InvalidOperationException("Jwt:SigningKey must be configured.");

builder.Services.AddOpenApi();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var validator = context.HttpContext.RequestServices.GetRequiredService<JwtSessionValidator>();
                if (!await validator.IsValidAsync(context.Principal!, context.HttpContext.RequestAborted))
                    context.Fail("The access token session is no longer active.");
            }
        };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.Policies.CanManageUsers, policy => policy.RequireClaim("permission", AuthConstants.Permissions.ManageUsers))
    .AddPolicy(AuthConstants.Policies.CanReadQuestionBank, policy => policy.RequireClaim("permission", AuthConstants.Permissions.ReadQuestionBank))
    .AddPolicy(AuthConstants.Policies.CanWriteContent, policy => policy.RequireClaim("permission", AuthConstants.Permissions.WriteContent))
    .AddPolicy(AuthConstants.Policies.CanReadTests, policy => policy.RequireClaim("permission", AuthConstants.Permissions.ReadTests))
    .AddPolicy(AuthConstants.Policies.CanSubmitTests, policy => policy.RequireClaim("permission", AuthConstants.Permissions.SubmitTests));
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? (builder.Environment.IsDevelopment()
        ? ["http://localhost:5173", "http://localhost:4173"]
        : []);
if (!builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
    throw new InvalidOperationException("Configure at least one Cors:AllowedOrigins value outside Development.");
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));
if (!string.IsNullOrWhiteSpace(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
    builder.UseMicrosoftOpenTelemetry(options => options.Exporters = ExportTarget.AzureMonitor);
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddSingleton<IAuthTokenLifetimeProvider, JwtAuthTokenLifetimeProvider>();
builder.Services.AddScoped<IAccessTokenService, JwtAccessTokenService>();
builder.Services.AddScoped<JwtSessionValidator>();

var app = builder.Build();

if (args.Contains("--migrate-database", StringComparer.Ordinal))
{
    await app.Services.MigrateDatabaseAsync();
    return;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.Services.InitializePersistenceAsync(app.Environment.IsDevelopment());

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapQuestionEndpoints();
app.MapTestEndpoints();
app.MapUserEndpoints();

app.Run();

public partial class Program { }
