using TracesForBlazor.Trace;

namespace TracesForBlazor;

internal interface ITracesService
{
    Task Send();
}