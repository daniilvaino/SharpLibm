using System;
using System.Runtime.CompilerServices;

namespace CoreMathSharp;

internal static partial class StrictMathF
{
    // SharpLibm: SharpLibm's own rounding (see Binary64/Helper.cs).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float BuiltinRound(float x) => SharpLibm.Libm.roundf(x);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Truncate(float x) => SharpLibm.Libm.truncf(x);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float BuiltinFloor(float x) => SharpLibm.Libm.floorf(x);
}
