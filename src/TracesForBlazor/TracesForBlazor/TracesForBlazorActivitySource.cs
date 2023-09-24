namespace TracesForBlazor;

public class TracesForBlazorActivitySource
{
    private readonly string _serviceName;
    public TracesForBlazorActivitySource(string serviceName)
    {
        _serviceName = serviceName;
    }

    internal TracerService? TracerService { get; set; }

    public TracerForBlazorActivity? StartActivity(string name)
    {
        if (TracerService == null)
            return null;
        var trace = TracerService.Start(name, _serviceName);
        return new TracerForBlazorActivity(trace,TracerService );
    }
}