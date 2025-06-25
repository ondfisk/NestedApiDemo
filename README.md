# Nested Api Demo

## Tutorial 1

[Secure an ASP.NET Core Blazor WebAssembly standalone app with Microsoft Entra ID](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/standalone-with-microsoft-entra-id?view=aspnetcore-9.0)

## Tutorial 2

[Quickstart: Call a web API that is protected by the Microsoft identity platform](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-web-api-aspnet-sign-in?tabs=aspnet-core-workforce)

## Tutorial 3

[A web API that calls web APIs: App registration](https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-api-call-api-app-registration)

Make Web API an authorized client application to Web API 2.

Create client secret for `src/WebApi` in user secrets:

```sh
CLIENT_SECRET="..."
dotnet user-secrets set "AzureAd:ClientCredentials:0:ClientSecret" "$CLIENT_SECRET" --project src/WebApi
```
