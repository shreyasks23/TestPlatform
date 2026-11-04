using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TestPlatform.Api.Configuration;
using TestPlatform.Api.Endpoints;
using TestPlatform.Api.Services;
using TestPlatform.Application;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Security;
using TestPlatform.Persistence;

var builder = WebApplication.CreateBuilder(args);

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
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.Policies.CanWriteApi, policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("permission", AuthConstants.Permissions.ApiWrite));
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddSingleton<IAuthTokenLifetimeProvider, JwtAuthTokenLifetimeProvider>();
builder.Services.AddScoped<IAccessTokenService, JwtAccessTokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.Services.InitializePersistenceAsync(app.Environment.IsDevelopment());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapQuestionEndpoints();
app.MapTestEndpoints();
app.MapUserEndpoints();

app.Run();
