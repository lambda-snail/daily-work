using Azure.Identity;
using Dapper;
using Microsoft.FluentUI.AspNetCore.Components;
using Server.Common.Dapper;
using Server.Common.Settings;
using Server.Components;
using Server.Features.Todo;

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
