// System.ReadOnlySpan<T> — cut copy of SharpOS std-no-runtime/Runtime/
// ReadOnlySpan.cs (from dotnet/runtime ReadOnlySpan.cs). Kept: the fields, the
// constructors from a pointer and from a ref, the indexer, Length, IsEmpty,
// Slice, GetPinnableReference — SharpLibm's constant tables are
// `ReadOnlySpan<T> t = [...]` over data in the image. Cut as in Span.cs.

using System.Runtime.CompilerServices;

namespace System
{
    public readonly ref struct ReadOnlySpan<T>
    {
        internal readonly ref T _reference;
        private readonly int _length;

        // The compiler requires this constructor for a collection expression
        // `ReadOnlySpan<T> t = [...]` even where it lays the data out as a blob
        // and calls RuntimeHelpers.CreateSpan instead.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan(T[] array)
        {
            if (array == null)
            {
                this = default;
                return;
            }
            _reference = ref SpanHelpers.GetArrayDataReference(array);
            _length = array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan(void* pointer, int length)
        {
            if (length < 0) SpanHelpers.Halt();
            _reference = ref Unsafe.As<byte, T>(ref *(byte*)pointer);
            _length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlySpan(ref T reference, int length)
        {
            _reference = ref reference;
            _length = length;
        }

        public ref readonly T this[int index]
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

        public ref readonly T GetPinnableReference()
        {
            ref T ret = ref Unsafe.NullRef<T>();
            if (_length != 0) ret = ref _reference;
            return ref ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> Slice(int start)
        {
            if ((uint)start > (uint)_length) SpanHelpers.Halt();
            return new ReadOnlySpan<T>(ref Unsafe.Add(ref _reference, (nint)(uint)start), _length - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> Slice(int start, int length)
        {
            if ((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
                SpanHelpers.Halt();
            return new ReadOnlySpan<T>(ref Unsafe.Add(ref _reference, (nint)(uint)start), length);
        }
    }
}
