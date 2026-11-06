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
    ?? ["http://localhost:5173", "http://localhost:4173"];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
builder.Services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddSingleton<IAuthTokenLifetimeProvider, JwtAuthTokenLifetimeProvider>();
builder.Services.AddScoped<IAccessTokenService, JwtAccessTokenService>();
builder.Services.AddScoped<JwtSessionValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.Services.InitializePersistenceAsync(app.Environment.IsDevelopment());

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapQuestionEndpoints();
app.MapTestEndpoints();
app.MapUserEndpoints();

app.Run();
