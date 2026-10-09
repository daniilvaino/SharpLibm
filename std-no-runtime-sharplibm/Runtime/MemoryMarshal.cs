// System.Runtime.InteropServices.MemoryMarshal — cut copy of SharpOS
// std-no-runtime/Runtime/MemoryMarshal.cs. Kept: GetReference and
// Cast<TFrom, TTo>(ReadOnlySpan<TFrom>), which SharpLibm uses to read packed
// tables (CORE-MATH's exp/expm1/exp10). Cut: everything else.

using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
    public static class MemoryMarshal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetReference<T>(Span<T> span) => ref span._reference;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetReference<T>(ReadOnlySpan<T> span) => ref span._reference;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ReadOnlySpan<TTo> Cast<TFrom, TTo>(ReadOnlySpan<TFrom> span)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            long scaled = (long)span.Length * sizeof(TFrom) / sizeof(TTo);
            if (scaled > int.MaxValue)
                SpanHelpers.Halt();
            return new ReadOnlySpan<TTo>(ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in span._reference)), (int)scaled);
        }
    }
}
