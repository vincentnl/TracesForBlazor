using Microsoft.Extensions.DependencyInjection;
using TracesForBlazor;

namespace TracerForBlazorTests;

public class HttpClientListenerTests
{
    // [Fact]
    // public async Task HttpClient_EventSource_Is_Registered()
    // {
    //     IServiceCollection services = new ServiceCollection();
    //     services.AddHttpClient();
    //     services.AddTelemetryConsumer<MyTelemetryConsumer>();
    //     var provider = services.BuildServiceProvider();
    //     var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    //     var httpClient = httpClientFactory.CreateClient();
    //     
    //     var result = await httpClient.GetAsync("https://www.google.com");
    //
    //     int d = 9;
    //     //var listeners = httpClient.GetEventListeners();
    //     //listeners.Should().Contain(x => x.GetType() == typeof(HttpClientListener));
    // }
}