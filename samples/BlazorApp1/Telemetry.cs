using TracesForBlazor;

namespace BlazorApp1;

public static class Telemetry
{
    public static readonly string ServiceName = typeof(Program).Assembly.GetName().Name??"BlazorApp1";
    public static TracesForBlazorActivitySource Instance { get; } = new(ServiceName);
}