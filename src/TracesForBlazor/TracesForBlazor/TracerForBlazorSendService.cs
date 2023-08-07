using System.Net.Http.Headers;
using System.Timers;
using Google.Protobuf;
using Grpc.Net.Client;
using OpenTelemetry.Proto.Collector.Trace.V1;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Resource.V1;
using OpenTelemetry.Proto.Trace.V1;
using TracesForBlazor.Trace;
using Timer = System.Timers.Timer;
using Grpc.Net.Client.Web; 

namespace TracesForBlazor;

internal class TracerForBlazorSendService : ITracesForBlazorSendService
{
    private readonly TracerForBlazorOptions _options;
    private readonly Timer _timer;
    private readonly TracesForBlazorActivitySource[] _tracesForBlazorActivitySources;

    internal TracerForBlazorSendService(TracerForBlazorOptions options,
        TracesForBlazorActivitySource[] tracesForBlazorActivitySources)
    {
        _options = options;
        _tracesForBlazorActivitySources = tracesForBlazorActivitySources;
        _timer = new Timer(_options.SendInterval.TotalMilliseconds);
        _timer.Elapsed += TimerOnElapsed;
        _timer.Start();
    }

    public async Task Purge()
    {
        var traces2Bsent = new List<TraceHolder>();
        foreach (var traceSource in _tracesForBlazorActivitySources) 
            traces2Bsent.AddRange(traceSource.Traces);
        if(traces2Bsent.Count == 0) 
            return;
        switch (_options.Type)
        {
            // case OpenTelemetrySendTypes.Grpc:
            //     await SendToGrpcEndpoint(traces2Bsent, _options.Url);
            //     break;
            case OpenTelemetrySendTypes.HttpProto:
                await SendToProtoEndpoint(traces2Bsent, _options.Url);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        #if DEBUG
            _timer.Stop();
        #endif
        await Purge();
        #if DEBUG
            _timer.Start();
        #endif
    }

    internal ExportTraceServiceRequest CreateExportTraceServiceRequest(List<TraceHolder> trace)
    {
        var tracesPerService = trace.GroupBy(x => x.ServiceName);

        var request = new ExportTraceServiceRequest();
        var resourceSpans = new ResourceSpans();
        resourceSpans.Resource = new Resource();
        foreach (var traces in tracesPerService)
        {
            resourceSpans.Resource.Attributes.Add(new KeyValue
            {
                Key = "service.name",
                Value = new AnyValue
                {
                    StringValue = traces.Key
                }
            });
            //resourceSpans.Resource.Attributes.Add("telemetry.sdk.name", "serilog");
            //resourceSpans.Resource.Attributes.Add("telemetry.sdk.language", "dotnet");
            //resourceSpans.Resource.Attributes.Add("telemetry.sdk.version", "1.0.2");
            request.ResourceSpans.Add(resourceSpans);
            var scopeSpan = new ScopeSpans
            {
                Scope = new InstrumentationScope
                {
                    Name = "my.library",
                    Version = "1.0.0"
                }
            };
            AddChildrenRecursive(trace, null);

            resourceSpans.ScopeSpans.Add(scopeSpan);

            void AddChildrenRecursive(List<TraceHolder> children, TraceHolder? parent)
            {
                foreach (var child in children)
                {
                    var span = new Span
                    {
                        TraceId = ByteString.CopyFrom(child.TraceId
                            .Id), //ByteString.FromBase64(JsonSender.RandomByteArrayAsBase64(16)),
                        SpanId = ByteString.CopyFrom(child.SpanId
                            .Id), //ByteString.FromBase64(JsonSender.RandomByteArrayAsBase64(8)),
                        Name = child.Name,
                        StartTimeUnixNano = TraceRequestUtil.GetUnixTimeNanoSeconds(child.Start),
                        EndTimeUnixNano = TraceRequestUtil.GetUnixTimeNanoSeconds(child.End),
                        Kind = Span.Types.SpanKind.Internal
                    };
                    if (parent != null) span.ParentSpanId = ByteString.CopyFrom(parent.SpanId.Id);
                    scopeSpan.Spans.Add(span);
                    if (child.ChildTraces.Count > 0) AddChildrenRecursive(child.ChildTraces, child);
                }
            }
        }

        return request;
    }

    internal async Task SendToProtoEndpoint(List<TraceHolder> traces, string url)
    {
        var request = CreateExportTraceServiceRequest(traces);
        var bytes = Serialize(request);
        HttpClient client = new HttpClient();
        var httprequest = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
        httprequest.Content = new ByteArrayContent(bytes);
        httprequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
        httprequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
        var response = await client.SendAsync(httprequest);
        string responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
        //todo add logger
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to send trace. Status code: {response.StatusCode}");
        }
    }

    internal async Task SendToGrpcEndpoint(List<TraceHolder> traces , string url)
    {
        var request = CreateExportTraceServiceRequest(traces);
        var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())); 
        //var baseUri = services.GetRequiredService<NavigationManager>().BaseUri; 
        var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions { HttpClient = httpClient }); 
        //return new WeatherForecasts.WeatherForecastsClient(channel); 
        
        
        
        //var channel = GrpcChannel.ForAddress(url);
        var client = new TraceService.TraceServiceClient(channel);
        var response = await client.ExportAsync(request);
        Console.WriteLine(response.ToString());
    }
        
    internal byte[] Serialize(ExportTraceServiceRequest request)
    {
        byte[] protobufData = new byte[request.CalculateSize()];
        using (var stream = new CodedOutputStream(protobufData))
        {
            request.WriteTo(stream);
        }
        return protobufData;
    }

}