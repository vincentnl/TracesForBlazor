using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp1;
using TracesForBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

TracerForBlazorOptions options = new TracerForBlazorOptions()
{
    //Url = "http://192.168.1.100:4318/v1/traces",
    Url = "http://192.168.1.100:4318",
    Type = OpenTelemetrySendTypes.HttpProto,
    SendInterval = TimeSpan.FromSeconds(3)
};

builder.Services.AddTracesForBlazor(options,Telemetry.Instance);

await builder.Build().RunAsync();