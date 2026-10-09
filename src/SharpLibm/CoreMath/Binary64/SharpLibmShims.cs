using System.Runtime.CompilerServices;

namespace CoreMathSharp;

// SharpLibm: what the dropped Sqrt.cs / MinMax.cs (thin wrappers over
// System.Math) provided to the rest of the port.
internal static partial class StrictMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double Sqrt(double x) => SharpLibm.Libm.sqrt(x);

    // System.Math.Max / Min semantics (.NET Core 3.0+), which the callers were
    // written against: NaN if either operand is NaN; +0 above −0.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double Max(double x, double y)
    {
        if (x != y) return SharpLibm.Bits.IsNaN(x) ? x : (y < x ? x : y);
        return (SharpLibm.Bits.Of(y) >> 63) != 0 ? x : y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double Min(double x, double y)
    {
        if (x != y) return SharpLibm.Bits.IsNaN(x) ? x : (x < y ? x : y);
        return (SharpLibm.Bits.Of(x) >> 63) != 0 ? x : y;
    }
}

internal static partial class StrictMathF
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Sqrt(float x) => SharpLibm.Libm.sqrtf(x);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Max(float x, float y)
    {
        if (x != y) return SharpLibm.Bits.IsNaN(x) ? x : (y < x ? x : y);
        return (SharpLibm.Bits.Of(y) >> 31) != 0 ? x : y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Min(float x, float y)
    {
        if (x != y) return SharpLibm.Bits.IsNaN(x) ? x : (x < y ? x : y);
        return (SharpLibm.Bits.Of(x) >> 31) != 0 ? x : y;
    }
}
