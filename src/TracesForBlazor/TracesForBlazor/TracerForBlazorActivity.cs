using Serilog;
using TracesForBlazor.Trace;

namespace TracesForBlazor;

public class TracerForBlazorActivity : IDisposable
{
    private readonly TraceHolder _source;
    private readonly TracerService _tracesService;

    internal TracerForBlazorActivity(TraceHolder source, TracerService tracesService)
    {
        Log.Verbose("Starting {SourceName}", source.Name);
        _source = source;
        _tracesService = tracesService;
    }
    
    public void Dispose()
    {
        Log.Verbose("Stopping {SourceName}", _source.Name);
        Stop();
    }

    public void Stop()
    {
        _tracesService.Stop(_source);
    }

    public void AddTag(string uri, string toString)
    {
        _source.AddTag(uri, toString);
    }
}