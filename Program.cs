using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Enable PII logging (only for development, remove in production)
if (builder.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "IDme";
})
.AddCookie()
.AddOpenIdConnect("IDme", options =>
{
    options.ClientId = builder.Configuration["IDme:ClientId"];
    options.ClientSecret = builder.Configuration["IDme:ClientSecret"];

    options.Authority = "https://api.idmelabs.com/oidc";

    // Set the custom endpoints and JWKS URI
    options.Configuration = new OpenIdConnectConfiguration
    {
        AuthorizationEndpoint = "https://api.idmelabs.com/oauth/authorize",
        TokenEndpoint = "https://api.idmelabs.com/oauth/token",
        UserInfoEndpoint = "https://api.idmelabs.com/api/public/v3/userinfo",
        JwksUri = "https://api.idmelabs.com/oidc/.well-known/jwks"
    };

    // Implement custom ConfigurationManager
    var httpClient = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
    options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        "https://api.idmelabs.com/oidc/.well-known/openid-configuration",
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever(httpClient)
    );

    options.ResponseType = OpenIdConnectResponseType.Code;
    options.CallbackPath = new PathString("/authorization-code/callback");

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("http://idmanagement.gov/ns/assurance/ial/2/aal/2");

    // Configure token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://api.idmelabs.com/oidc",
        ValidateAudience = true,
        ValidAudience = options.ClientId,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    // Add event handlers for debugging
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated successfully");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnRedirectToIdentityProviderForSignOut = context =>
{
    var logoutUri = "/";
    context.Response.Redirect(logoutUri);
    context.HandleResponse();
    return Task.CompletedTask;
}

    };

    // Enable logging for the backchannel
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    // Disable HTTPS requirement for development (remove in production)
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
});

// Add logging
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "profile",
    pattern: "Profile/{action=Index}/{id?}");

app.Run();