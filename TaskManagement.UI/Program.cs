using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskManagement.Blazor.Services.Auth;
using TaskManagement.UI;
using TaskManagement.UI.Auth;
using TaskManagement.UI.Services.Auth;
using TaskManagement.UI.Services.Common;
using TaskManagement.UI.Services.TaskItem;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5246/") });

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ApiClient>();


// Authentication & Authorization
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();


await builder.Build().RunAsync();