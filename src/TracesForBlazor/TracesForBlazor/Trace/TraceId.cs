using System.Security.Cryptography;

namespace TracesForBlazor.Trace;

internal struct TraceId
{
    public byte[] Id { get; set; } = new byte[16];

    public TraceId()
    {
        RandomNumberGenerator.Fill(Id);
    }
}