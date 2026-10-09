// System.Runtime.CompilerServices.Unsafe — cut copy of SharpOS
// std-no-runtime/Runtime/Unsafe.cs (verbatim port of dotnet/runtime
// Unsafe.cs). [Intrinsic]: ILC replaces every call with IL, the bodies never
// run. Kept: what Span, ReadOnlySpan and MemoryMarshal.Cast here use —
// SizeOf, As, Add, AddByteOffset, AsRef, NullRef. Cut: the rest, and the
// bodies' `throw new PlatformNotSupportedException()` is `throw null` (no
// exception types in this library's core).

#pragma warning disable IDE0060 // implementations provided as intrinsics

namespace System.Runtime.CompilerServices
{
    public static unsafe class Unsafe
    {
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(ref T value)
        {
            throw null;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>()
        {
            throw null;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TTo As<TFrom, TTo>(ref TFrom source)
        {
            throw null;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, int elementOffset)
        {
            return ref AddByteOffset(ref source, (IntPtr)(elementOffset * (nint)SizeOf<T>()));
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(ref T source, IntPtr elementOffset)
        {
            return ref AddByteOffset(ref source, (IntPtr)((nint)elementOffset * (nint)SizeOf<T>()));
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            throw null;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(void* source)
        {
            return ref Unsafe.As<byte, T>(ref *(byte*)source);
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(scoped in T source)
        {
            throw null;
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T NullRef<T>()
        {
            return ref Unsafe.AsRef<T>(null);
        }
    }
}
