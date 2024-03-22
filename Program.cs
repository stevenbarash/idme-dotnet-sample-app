using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "IDme";
})
.AddCookie()
.AddOAuth("IDme", options =>
{
    options.AuthorizationEndpoint = "https://api.idmelabs.com/oauth/authorize";
    options.Scope.Add("http://idmanagement.gov/ns/assurance/ial/2/aal/2");
    options.CallbackPath = new PathString("/authorization-code/callback");
    options.ClientId = builder.Configuration?.GetValue<string>("IDme:ClientId");
    options.ClientSecret = builder.Configuration?.GetValue<string>("IDme:ClientSecret");
    options.TokenEndpoint = "https://api.idmelabs.com/oauth/token";
    options.UserInformationEndpoint = "https://api.idmelabs.com/api/public/v3/userinfo";
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.NameIdentifier, "sub");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Expiration, "exp");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.DateOfBirth, "birth_date");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Locality, "city");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "emails_confirmed");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "email");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.GivenName, "fname");
    options.ClaimActions.MapJsonKey("Social Security", "social");
    options.ClaimActions.MapJsonKey("identity_document_number", "identity_document_number");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Surname, "lname");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.MobilePhone, "phone");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.PostalCode, "zip");
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.StateOrProvince, "state");
    options.ClaimActions.MapJsonKey("uuid", "uuid");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var user = json.RootElement;

            var token = user.GetString();

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var payload = securityToken?.Payload;

            var userClaims = new Dictionary<string, object>();

            if (securityToken != null)
            {
                foreach (var claim in securityToken.Claims)
                {
                    userClaims.Add(claim.Type, claim.Value);
                }
            }

            var userClaimsJson = JsonDocument.Parse(JsonSerializer.Serialize(userClaims)).RootElement;

            context.RunClaimActions(userClaimsJson);
        }
    };
});

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