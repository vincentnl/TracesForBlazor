using System.Diagnostics;

namespace TracesForBlazor.Trace;

[DebuggerDisplay("{Name} {End==default?\"running\":\"closed\"}}")]
internal class TraceHolder
{
    public required string ServiceName { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public required TraceId TraceId { get; set; }
    public SpanId SpanId { get; set; } = new();
    public SpanId? ParentSpanId => ParentTrace?.SpanId;
    public TraceHolder? ParentTrace { get; set; }
    public List<TraceHolder> ChildTraces { get; set; } = new();
    public string Name { get; set; } = string.Empty;
}