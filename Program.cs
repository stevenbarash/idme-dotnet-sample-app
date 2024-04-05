using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

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
.AddOpenIdConnect("IDme", options =>
        {
            options.Authority = "https://api.idmelabs.com/oidc"; // The OIDC authority
            options.ClientId = builder.Configuration?.GetValue<string>("IDme:ClientId");
            options.ClientSecret = builder.Configuration?.GetValue<string>("IDme:ClientSecret");
            options.ResponseType = "code";
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            // options.Scope.Add("profile");
            options.Scope.Add("http://idmanagement.gov/ns/assurance/ial/2/aal/2");

            options.CallbackPath = new PathString("/authorization-code/callback");

            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.NameIdentifier, "sub");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Expiration, "exp");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.DateOfBirth, "birth_date");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Locality, "city");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "emails_confirmed");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.GivenName, "fname");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Surname, "lname");

            options.ClaimActions.MapJsonKey("Social Security", "social");
            options.ClaimActions.MapJsonKey("identity_document_number", "identity_document_number");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.MobilePhone, "phone");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.PostalCode, "zip");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.StateOrProvince, "state");
            options.ClaimActions.MapJsonKey("uuid", "uuid");
            // options.Authority = "https://api.idmelabs.com"; // The OIDC authority
            // options.ClientId = builder.Configuration?.GetValue<string>("IDme:ClientId");
            // options.ClientSecret = builder.Configuration?.GetValue<string>("IDme:ClientSecret");
            // options.ResponseType = "code";
            // options.GetClaimsFromUserInfoEndpoint = true;
            // options.SaveTokens = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("http://idmanagement.gov/ns/assurance/ial/2/aal/2");

            options.CallbackPath = new PathString("/authorization-code/callback");

            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.NameIdentifier, "sub");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Expiration, "exp");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.DateOfBirth, "birth_date");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Locality, "city");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "emails_confirmed");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.GivenName, "fname");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Surname, "lname");

            options.ClaimActions.MapJsonKey("Social Security", "social");
            options.ClaimActions.MapJsonKey("identity_document_number", "identity_document_number");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.MobilePhone, "phone");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.PostalCode, "zip");
            options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.StateOrProvince, "state");
            options.ClaimActions.MapJsonKey("uuid", "uuid");
            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = async context =>
                {
                    var identity = context.Principal.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        var userClaims = new Dictionary<string, object>();
                        // Map the JWT claims to user claims dictionary
                        foreach (var claim in context.Principal.Claims)
                        {
                            userClaims.Add(claim.Type, claim.Value);
                        }

                        // Add the user claims to the ClaimsIdentity
                        foreach (var claim in userClaims)
                        {
                            identity.AddClaim(new Claim(claim.Key, claim.Value.ToString()));
                        }
                    }
                }
            };
        }
);
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