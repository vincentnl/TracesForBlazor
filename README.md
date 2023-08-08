# TracesForBlazor
With multithread incoming in .Net8, Blazor might be a first class citizen again for OpenTelemetry.

Until that time, this library will allow you to manually instrument your Blazor app, and send traces to an OpenTelemetry endpoint.

It does not yet support automatic instrumentation, but that might be considered for Http.

The usage is very similar to the [OpenTelemetry .Net], so switching over in .Net8 should be easy.

Currently in PoC fase, and only HttpProto is supported (usually port 4318).


## Usage

### In Program.cs
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        var options = new TracerForBlazorOptions
        {
            SendType = OpenTelemetrySendTypes.HttpProto,
            Url = "http://localhost:4318"
        };

        builder.Services.AddTracesForBlazor(
            options,
            Telemetry.ActivitySource,
            Logic.Telemetry.ActivitySource
        );

### TelemetrySources
Add these sources in your project, or in libraries.

    public static class Telemetry
    {
        public static readonly string ServiceName = "WebUI";

        //public static ActivitySource ActivitySource = new ActivitySource(ServiceName); Open telemetry way of doing this
        public static TracesForBlazorActivitySource ActivitySource = new(ServiceName);
    }

### Manual instrumentation
Adding a using statement, like the example below, will create a trace with the name of the action, and the start and stop times will be recorded.
If you use Mediatr, you could put this in a pipeline behaviour, catching all calls, and maintaining the hierarchy.

Alternatively, you can use it more manually.

    using (var myActivity = Telemetry.ActivitySource.StartActivity(action))
    {
        // Do something here
    }


### HTTP instrumentation
TODO: Future feature in consideration. Making the IHttpClient or IHttpClientFactory be the source of all http traffic, and adding a handler that reports start and stop times, and some http details might be a quick fix