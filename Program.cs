using BlazorPanzoom;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MyAncestry;
using MyAncestry.Interfaces;
using MyAncestry.Services;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddSingleton<IDataRepository, DataRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazorPanzoomServices();

await builder.Build().RunAsync();
