// System.Span<T> — cut copy of SharpOS std-no-runtime/Runtime/Span.cs
// (itself ported from dotnet/runtime src/libraries/System.Private.CoreLib/
// src/System/Span.cs). Kept, as there: the fields, the constructors from a
// pointer and from a ref, the indexer, Length, IsEmpty, Slice,
// GetPinnableReference and the conversion to ReadOnlySpan<T> — what
// `stackalloc` into a span and SharpLibm's code use.
// Cut: arrays (no arrays here), Equals/GetHashCode/ToString, the enumerator,
// Clear/Fill/CopyTo/ToArray. An index out of range goes to the ILC throw
// helper (MinimalRuntime.cs), which ends the process in this build.

using System.Runtime.CompilerServices;

namespace System
{
    public readonly ref struct Span<T>
    {
        /// <summary>A byref or a native ptr.</summary>
        internal readonly ref T _reference;
        /// <summary>The number of elements this Span contains.</summary>
        private readonly int _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Span(void* pointer, int length)
        {
            if (length < 0) SpanHelpers.Halt();
            _reference = ref Unsafe.As<byte, T>(ref *(byte*)pointer);
            _length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span(ref T reference)
        {
            _reference = ref reference;
            _length = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span(ref T reference, int length)
        {
            _reference = ref reference;
            _length = length;
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)_length) SpanHelpers.Halt();
                return ref Unsafe.Add(ref _reference, (nint)(uint)index);
            }
        }

        public int Length => _length;

        public bool IsEmpty => _length == 0;

        public ref T GetPinnableReference()
        {
            ref T ret = ref Unsafe.NullRef<T>();
            if (_length != 0) ret = ref _reference;
            return ref ret;
        }

        public static implicit operator ReadOnlySpan<T>(Span<T> span) =>
            new ReadOnlySpan<T>(ref span._reference, span._length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start)
        {
            if ((uint)start > (uint)_length) SpanHelpers.Halt();
            return new Span<T>(ref Unsafe.Add(ref _reference, (nint)(uint)start), _length - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start, int length)
        {
            if ((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
                SpanHelpers.Halt();
            return new Span<T>(ref Unsafe.Add(ref _reference, (nint)(uint)start), length);
        }
    }

    internal static class SpanHelpers
    {
        // BCL equivalent: MemoryMarshal.GetArrayDataReference(T[] array).
        // Array object layout: [MethodTable*](8) [Length(4)+pad(4)]
        // [element 0, element 1, ...]. Element 0 starts at object+16.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe ref T GetArrayDataReference<T>(T[] array)
        {
            T[] local = array;
            nint objAddr = *(nint*)Unsafe.AsPointer(ref local);
            return ref *(T*)(objAddr + 16);
        }

        internal static void Halt()
            => Internal.Runtime.CompilerHelpers.ThrowHelpers.ThrowIndexOutOfRangeException();
    }
}
