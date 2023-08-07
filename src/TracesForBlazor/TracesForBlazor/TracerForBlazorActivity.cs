using TracesForBlazor.Trace;

namespace TracesForBlazor;

public class TracerForBlazorActivity : IDisposable
{
    private readonly TraceHolder _source;

    internal TracerForBlazorActivity(TraceHolder source)
    {
        _source = source;
    }

    public void Dispose()
    {
        Stop();
    }

    public void Stop()
    {
        if (_source.End == default)
            _source.End = DateTimeOffset.UtcNow;
    }
}