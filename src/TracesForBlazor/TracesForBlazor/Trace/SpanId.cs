using System.Security.Cryptography;

namespace TracesForBlazor.Trace;

internal struct SpanId
{
    public bool Equals(SpanId other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is SpanId other && this == other;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public byte[] Id { get; set; } = new byte[8];

    public SpanId()
    {
        RandomNumberGenerator.Fill(Id);
    }

    public static bool operator !=(SpanId left, SpanId right)
    {
        return !left.Id.SequenceEqual(right.Id);
    }

    public static bool operator ==(SpanId left, SpanId right)
    {
        return left.Id.SequenceEqual(right.Id);
    }
}