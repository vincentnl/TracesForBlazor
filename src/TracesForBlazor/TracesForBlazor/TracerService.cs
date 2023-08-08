﻿using System.Timers;
using TracesForBlazor.Trace;
//using Timer = System..Timer;

namespace TracesForBlazor;

internal class TracerService:ITracesService
{
    private readonly TracerForBlazorOptions _options;
    private readonly System.Timers.Timer _timer = new(3000);
    private readonly TracerForBlazorSendService _sendService;
     
    public TracerService(TracesForBlazorActivitySource[] sources, TracerForBlazorOptions options)
    {
        foreach (var source in sources)
        {
            source.TracerService = this;
        }
        _options = options;
        _activitySources = sources.ToList();
        _sendService = new TracerForBlazorSendService(_options);
        _timer.AutoReset = true;
        _timer.Elapsed += TimerOnElapsed;
        _timer.Interval = _options.SendInterval.TotalMilliseconds;
    }

    private TraceHolder? _current;
    private readonly List<TraceHolder> _finishedTraces = new();
    private List<TracesForBlazorActivitySource> _activitySources;

    public void Stop(TraceHolder trace)
    {
        #if DEBUG
        if(trace!=_current)
            throw new InvalidOperationException("Trace is not current");
        #endif
        if (trace.End.Equals(default))
        {
            trace.End = DateTimeOffset.UtcNow;
        }
        _current = trace.ParentTrace;
        if (_current == null)
        {
            _finishedTraces.Add(trace);
        }         
    }
    
    public TraceHolder Start(string name, string serviceName)
    {
        var parent = _current;
        _current = new TraceHolder
        {
            TraceId = parent?.TraceId ?? new TraceId(),
            Start = DateTimeOffset.UtcNow,
            ParentTrace = parent,
            ServiceName = serviceName,
            Name = name
        };
        if (parent != null)
            parent.ChildTraces.Add(_current);
        else
        {//it's start - parent is null
            _timer.Start();
        }
        return _current;
    }

    private async void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_current == null)
        {
            _timer.Stop();
        }
        await Send();
    }

    public async Task Send()
    {
        if (_finishedTraces.Any())
        {
            await _sendService.Send(_finishedTraces, _options.SendType);
        }         
    }
}