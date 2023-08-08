using Serilog;
using TracesForBlazor.Trace;

namespace TracesForBlazor;

public class TracerForBlazorActivity : IDisposable
{
    private readonly TraceHolder _source;
    private readonly TracerService _tracesService;

    internal TracerForBlazorActivity(TraceHolder source, TracerService tracesService)
    {
        Log.Information("Starting {SourceName}", source.Name);
        _source = source;
        _tracesService = tracesService;
    }
    
    public void Dispose()
    {
        Log.Information("Stopping {SourceName}", _source.Name);
        Stop();
    }

    public void Stop()
    {
        _tracesService.Stop(_source);
    }
}