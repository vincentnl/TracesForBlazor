using TracesForBlazor.Trace;

namespace TracesForBlazor;

public class TracesForBlazorActivitySource
{
    private readonly string _serviceName;
    private List<TraceHolder> _traces = new();
    private TraceHolder? _currentTrace;
    private TraceHolder? _parentTrace;


    public TracesForBlazorActivitySource(string serviceName)
    {
        _serviceName = serviceName;
    }

    internal List<TraceHolder> Traces
    {
        get
        {
            UpdateCurrentTrace();
            if(!_traces.Any())
                return new();
            var finishedTraces = _traces.ToLookup(t=>t.End!=default);
            _traces = finishedTraces[false].ToList();
            return finishedTraces[true].ToList();
        }
    }
   
    public TracerForBlazorActivity StartActivity(string name)
    {
        UpdateCurrentTrace();
        _parentTrace = _currentTrace;
        _currentTrace = new TraceHolder
        {
            TraceId = _parentTrace?.TraceId ?? new TraceId(),
            Start = DateTimeOffset.UtcNow,
            ParentTrace = _parentTrace,
            ServiceName = _serviceName,
            Name = name
        };
        if (_parentTrace == null)
            _traces.Add(_currentTrace);
        else
            _parentTrace.ChildTraces.Add(_currentTrace);
        return new TracerForBlazorActivity(_currentTrace);
    }

    private void UpdateCurrentTrace()
    {
        if (_currentTrace == null)
            return;
        if (_currentTrace.End == default) //it's the running trace - we are up to date
            return;
        while (_currentTrace != null)
        {
            //current trace has ended, assign parent as current
            _currentTrace = _parentTrace;
            if (_currentTrace != null)
            {
                _parentTrace = _currentTrace.ParentTrace;
            }
            if (_currentTrace?.End == default)
                return;
        }
    }
}