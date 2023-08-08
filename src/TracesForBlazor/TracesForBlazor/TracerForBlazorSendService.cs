using System.Net.Http.Headers;
using Google.Protobuf;
using Grpc.Net.Client;
using OpenTelemetry.Proto.Collector.Trace.V1;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Resource.V1;
using OpenTelemetry.Proto.Trace.V1;
using TracesForBlazor.Trace;
using Grpc.Net.Client.Web; 

namespace TracesForBlazor;

internal class TracerForBlazorSendService
{
    private readonly TracerForBlazorOptions _options;
    
    internal TracerForBlazorSendService(TracerForBlazorOptions options)
    {
        _options = options;
    }

    private ExportTraceServiceRequest CreateExportTraceServiceRequest(List<TraceHolder> trace)
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

    private async Task SendToProtoEndpoint(List<TraceHolder> traces)
    {
        var request = CreateExportTraceServiceRequest(traces);
        var bytes = Serialize(request);
        HttpClient client = new HttpClient();
        var httprequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_options.Url));
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

    private async Task SendToGrpcEndpoint(List<TraceHolder> traces)
    {
        var request = CreateExportTraceServiceRequest(traces);
        var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())); 
        //var baseUri = services.GetRequiredService<NavigationManager>().BaseUri; 
        var channel = GrpcChannel.ForAddress(_options.Url, new GrpcChannelOptions { HttpClient = httpClient }); 
        //return new WeatherForecasts.WeatherForecastsClient(channel); 
        
        
        
        //var channel = GrpcChannel.ForAddress(url);
        var client = new TraceService.TraceServiceClient(channel);
        var response = await client.ExportAsync(request);
        Console.WriteLine(response.ToString());
    }

    private byte[] Serialize(ExportTraceServiceRequest request)
    {
        byte[] protobufData = new byte[request.CalculateSize()];
        using (var stream = new CodedOutputStream(protobufData))
        {
            request.WriteTo(stream);
        }
        return protobufData;
    }

    public async Task Send(List<TraceHolder> traces, OpenTelemetrySendTypes sendType)
    {
        switch (sendType)
        {
            case OpenTelemetrySendTypes.HttpProto:
                await SendToProtoEndpoint(traces);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sendType), sendType, null);
        }
    }
}