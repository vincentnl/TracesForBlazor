using TracesForBlazor.Trace;

namespace TracesForBlazor;

public interface ITracesForBlazorSendService
{
    public Task Purge();
}