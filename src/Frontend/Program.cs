using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure DownstreamApi settings
var downstreamApiSettings = new DownstreamApiSettings();
builder.Configuration.GetSection("DownstreamApi").Bind(downstreamApiSettings);
builder.Services.AddSingleton(downstreamApiSettings);

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";
    options.ProviderOptions.DefaultAccessTokenScopes = downstreamApiSettings.Scopes;
});

builder.Services.AddScoped(sp =>
{
    var authorizationMessageHandler = sp.GetRequiredService<AuthorizationMessageHandler>();
    authorizationMessageHandler.InnerHandler = new HttpClientHandler();
    authorizationMessageHandler.ConfigureHandler(
        authorizedUrls: [ downstreamApiSettings.BaseUrl ],
        scopes: downstreamApiSettings.Scopes
    );

    return new HttpClient(authorizationMessageHandler);
});

await builder.Build().RunAsync();
