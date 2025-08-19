using Azure.Identity;
using Dapper;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.FluentUI.AspNetCore.Components;
using Server.Common.Dapper;
using Server.Common.Settings;
using Server.Components;
using Server.Features.Todo;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

var appConfigurationEndpoint = Environment.GetEnvironmentVariable("AppConfigurationEndpoint");
ArgumentNullException.ThrowIfNull(appConfigurationEndpoint);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// We avoid default azure credential to support Rider
// TODO: Remove and use default azure credential when Rider supports it
if ("Development" == env)
{
    var tempConfiguration = new ConfigurationBuilder()
        //.AddJsonFile("appsettings.json")
        .AddUserSecrets<Program>()
        .Build();

    var connectionString = tempConfiguration.GetSection("DevelopmentCredentials").GetValue<string>("AppConfigConnectionString");
    builder.Configuration.AddAzureAppConfiguration(options =>
            options
                .Connect(connectionString)
                .Select("Tasks:*", env)
                //.Select("*", env)
    );
}
else
{
     var credential = new ManagedIdentityCredential(Environment.GetEnvironmentVariable("ClientId"));
     builder.Configuration.AddAzureAppConfiguration(options =>
             options
                 .Connect(new Uri(appConfigurationEndpoint), credential)
                 .Select("Tasks:*", env)
         //.ConfigureRefresh(refreshOptions => refreshOptions.SetCacheExpiration(TimeSpan.FromHours(refreshTimer ?? 24)))
     );
}

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Tasks:Database"));
builder.Services.AddScoped<TodoRepository>();

SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new DapperSqlTimeOnlyTypeHandler());






// See https://github.com/moshali1/aspnet_blazor/blob/master/aspnet_blazor/Program.cs

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // The FallbackPolicy is set to 'null' to disable global authorization.
    // This means by default, pages and APIs are accessible without authorization
    // unless explicitly protected using [Authorize] or similar attributes.

    // Set FallbackPolicy to options.DefaultPolicy for default authorization, requiring authentication for all requests.
    // Uncomment below to apply:

    // options.FallbackPolicy = options.DefaultPolicy;

    options.FallbackPolicy = null;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorPages();











// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Must be after authentication
app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
