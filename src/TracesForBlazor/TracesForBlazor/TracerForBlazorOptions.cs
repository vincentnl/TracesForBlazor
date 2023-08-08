namespace TracesForBlazor;

public class TracerForBlazorOptions
{
    /// <summary>
    /// Currently only HttpProto is supported
    /// </summary>
    public OpenTelemetrySendTypes SendType { get; set; } = OpenTelemetrySendTypes.HttpProto;
    /// <summary>
    /// Required. The URL to send traces to - for example: http://localhost:4318
    /// If no endpoint is added, the default of v1/traces will be assumed
    /// </summary>
    public required string Url { get; set; }
    public TimeSpan SendInterval { get; set; } = TimeSpan.FromSeconds(3);
}