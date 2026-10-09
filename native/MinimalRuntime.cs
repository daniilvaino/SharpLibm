// The core types of the SharpLibm C library — cut copy of SharpOS
// apps_native/sdk/MinimalRuntime.cs (the app tier). The library is its own
// system module (NoStdLib), so these are the types the compiler and ILC need
// by name; std-no-runtime-sharplibm/ adds Span, tuples, Unsafe and the like.
//
// Kept: Object's MethodTable field; the primitives with their value field and
// constants; IntPtr/UIntPtr; ValueType, Enum, Array; the runtime handles
// ldtoken stores into; Attribute and AttributeUsage; MethodImpl, Intrinsic,
// RuntimeExport/RuntimeImport, StructLayout; RuntimeHelpers.CreateSpan (the
// constant tables) and OffsetToStringData (fixed on a string); RuntimeFeature;
// ILC's StartupCodeHelpers and ThrowHelpers.
// Cut: interfaces on the primitives, Equals/GetHashCode/ToString, Parse,
// Type/RuntimeType/EETypePtr, Nullable, AppContext, DllImport, the memory
// stubs. ThrowHelpers end the process instead of throwing: there are no
// exceptions in this library, and nothing to catch them in C.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.Runtime
{
    internal struct MethodTable { }
}

namespace System
{
    public unsafe class Object
    {
#pragma warning disable 169
        // Field name `m_pEEType` is contract with NativeAOT ILC.
        private IntPtr m_pEEType;
#pragma warning restore 169
    }

    public struct Void { }

    public struct Boolean
    {
#pragma warning disable 169
        private bool _value;
#pragma warning restore 169
    }

    public struct Char
    {
        public const char MaxValue = (char)0xFFFF;
        public const char MinValue = (char)0x00;
#pragma warning disable 169
        private char _value;
#pragma warning restore 169
    }

    public struct SByte
    {
        public const sbyte MaxValue = 0x7F;
        public const sbyte MinValue = unchecked((sbyte)0x80);
#pragma warning disable 169
        private sbyte _value;
#pragma warning restore 169
    }

    public struct Byte
    {
        public const byte MaxValue = 0xFF;
        public const byte MinValue = 0;
#pragma warning disable 169
        private byte _value;
#pragma warning restore 169
    }

    public struct Int16
    {
        public const short MaxValue = 0x7FFF;
        public const short MinValue = unchecked((short)0x8000);
#pragma warning disable 169
        private short _value;
#pragma warning restore 169
    }

    public struct UInt16
    {
        public const ushort MaxValue = 0xFFFF;
        public const ushort MinValue = 0;
#pragma warning disable 169
        private ushort _value;
#pragma warning restore 169
    }

    public struct Int32
    {
        public const int MaxValue = 0x7FFFFFFF;
        public const int MinValue = unchecked((int)0x80000000);
#pragma warning disable 169
        private int _value;
#pragma warning restore 169
    }

    public struct UInt32
    {
        public const uint MaxValue = 0xFFFFFFFFu;
        public const uint MinValue = 0u;
#pragma warning disable 169
        private uint _value;
#pragma warning restore 169
    }

    public struct Int64
    {
        public const long MaxValue = 0x7FFFFFFFFFFFFFFFL;
        public const long MinValue = unchecked((long)0x8000000000000000L);
#pragma warning disable 169
        private long _value;
#pragma warning restore 169
    }

    public struct UInt64
    {
        public const ulong MaxValue = 0xFFFFFFFFFFFFFFFFuL;
        public const ulong MinValue = 0uL;
#pragma warning disable 169
        private ulong _value;
#pragma warning restore 169
    }

    // IntPtr / UIntPtr: the conversions the compiler emits calls to whenever
    // code casts (Unsafe's bodies do). The storage is a recursive `nint` — ILC
    // special-cases primitives by namespace+name, same trick as Int32._value.
    public readonly struct IntPtr
    {
        private readonly nint _value;

        public IntPtr(int value) { _value = (nint)value; }
        public IntPtr(long value) { _value = (nint)value; }
        public unsafe IntPtr(void* value) { _value = (nint)value; }

        public static explicit operator IntPtr(int value) => new IntPtr(value);
        public static explicit operator IntPtr(long value) => new IntPtr(value);
        public static unsafe explicit operator IntPtr(void* value) => new IntPtr(value);
        public static explicit operator int(IntPtr value) => (int)value._value;
        public static explicit operator long(IntPtr value) => (long)value._value;
        public static unsafe explicit operator void*(IntPtr value) => (void*)value._value;
    }

    public readonly struct UIntPtr
    {
        private readonly nuint _value;

        public UIntPtr(uint value) { _value = (nuint)value; }
        public UIntPtr(ulong value) { _value = (nuint)value; }
        public unsafe UIntPtr(void* value) { _value = (nuint)value; }

        public static explicit operator UIntPtr(uint value) => new UIntPtr(value);
        public static explicit operator UIntPtr(ulong value) => new UIntPtr(value);
        public static unsafe explicit operator UIntPtr(void* value) => new UIntPtr(value);
        public static explicit operator uint(UIntPtr value) => (uint)value._value;
        public static explicit operator ulong(UIntPtr value) => (ulong)value._value;
        public static unsafe explicit operator void*(UIntPtr value) => (void*)value._value;
    }

    // Single / Double carry the recursive `_value` field so the type has its
    // true 4/8-byte size. Bit patterns of the constants match dotnet/runtime.
    public struct Single
    {
#pragma warning disable 169
        private float _value;
#pragma warning restore 169

        public const float MinValue = -3.40282347E+38F;
        public const float MaxValue = 3.40282347E+38F;
        public const float Epsilon = 1.401298E-45F;
        public const float PositiveInfinity = (float)1.0 / (float)0.0;
        public const float NegativeInfinity = (float)-1.0 / (float)0.0;
        public const float NaN = (float)0.0 / (float)0.0;
    }

    public struct Double
    {
#pragma warning disable 169
        private double _value;
#pragma warning restore 169

        public const double MinValue = -1.7976931348623157E+308;
        public const double MaxValue = 1.7976931348623157E+308;
        public const double Epsilon = 4.9406564584124654E-324;
        public const double PositiveInfinity = 1.0 / 0.0;
        public const double NegativeInfinity = -1.0 / 0.0;
        public const double NaN = 0.0 / 0.0;
    }

    public abstract class ValueType { }
    public abstract class Enum : ValueType { }

    // Declarations only. The compiler looks these up while resolving any
    // `==` — the built-in delegate equality operator is a candidate — so
    // `x == 0` does not compile without them. No delegate is ever made here.
    public abstract class Delegate { }
    public abstract class MulticastDelegate : Delegate { }

    // Declaration only, for `throw null` in Unsafe's intrinsic bodies (never
    // run). Nothing here throws: ThrowHelpers and Libm.Unreachable end the
    // process.
    public class Exception { }

    // Base class for all arrays. Length is at offset 8, after the MethodTable
    // pointer — NativeAOT's layout.
    [StructLayout(LayoutKind.Sequential)]
    public abstract class Array
    {
        public readonly int Length;
    }

    // One pointer-sized slot each: ILC's ldtoken lowering stores the token
    // into it (CreateSpan's RuntimeFieldHandle).
    public struct RuntimeTypeHandle { internal IntPtr _value; }
    public struct RuntimeMethodHandle { internal IntPtr _value; }
    public struct RuntimeFieldHandle { internal IntPtr _value; }

    public class Attribute { }

    [Flags]
    public enum AttributeTargets
    {
        Assembly = 1,
        Module = 2,
        Class = 4,
        Struct = 8,
        Enum = 16,
        Constructor = 0x20,
        Method = 0x40,
        Property = 128,
        Field = 0x100,
        Event = 512,
        Interface = 1024,
        Parameter = 2048,
        Delegate = 4096,
        ReturnValue = 8192,
        GenericParameter = 16384,
        All = 32767,
    }

    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]
    public sealed class FlagsAttribute : Attribute
    {
        public FlagsAttribute() { }
    }

    public sealed class AttributeUsageAttribute : Attribute
    {
        public AttributeUsageAttribute(AttributeTargets validOn) { }
        public bool AllowMultiple { get; set; }
        public bool Inherited { get; set; }
    }

    namespace Runtime.CompilerServices
    {
        public class RuntimeHelpers
        {
            public static unsafe int OffsetToStringData => sizeof(IntPtr) + sizeof(int);

            // Roslyn lowers `ReadOnlySpan<T> x = [1,2,3,...]` into
            // `ldtoken <field> + call RuntimeHelpers.CreateSpan<T>`. [Intrinsic]
            // tells ILC to fold it into a span over the data blob in the image —
            // the body never executes.
            [Intrinsic]
            public static ReadOnlySpan<T> CreateSpan<T>(RuntimeFieldHandle fldHandle)
                => default;
        }

        public static class RuntimeFeature
        {
            public const string UnmanagedSignatureCallingConvention = nameof(UnmanagedSignatureCallingConvention);
            // `ref T` fields in ref structs (Span<T>). Without it: CS9064.
            public const string ByRefFields = nameof(ByRefFields);
        }
    }
}

namespace System.Runtime.InteropServices
{
    // The compiler's modreq for `where T : unmanaged` (MemoryMarshal.Cast).
    public class UnmanagedType { }

    sealed class StructLayoutAttribute : Attribute
    {
        public StructLayoutAttribute(LayoutKind layoutKind) { }
        public int Size;
        public int Pack;
    }

    internal enum LayoutKind
    {
        Sequential = 0,
        Explicit = 2,
        Auto = 3,
    }
}

namespace System.Runtime
{
    internal sealed class RuntimeExportAttribute : Attribute
    {
        public RuntimeExportAttribute(string entry) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class RuntimeImportAttribute : Attribute
    {
        public RuntimeImportAttribute(string dllName) { }
        public RuntimeImportAttribute(string dllName, string entryPoint) { }
    }
}

namespace System.Runtime.CompilerServices
{
    public enum MethodImplOptions
    {
        NoInlining = 0x0008,
        AggressiveInlining = 0x0100,
        InternalCall = 0x1000,
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class MethodImplAttribute : Attribute
    {
        public MethodImplAttribute(MethodImplOptions methodImplOptions) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct
        | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field,
        Inherited = false)]
    public sealed class IntrinsicAttribute : Attribute
    {
        public IntrinsicAttribute() { }
    }
}

namespace Internal.Runtime.CompilerHelpers
{
    using System.Runtime;

    class StartupCodeHelpers
    {
        [RuntimeExport("RhpReversePInvoke")]
        static void RhpReversePInvoke(IntPtr frame) { }

        [RuntimeExport("RhpReversePInvokeReturn")]
        static void RhpReversePInvokeReturn(IntPtr frame) { }

        [RuntimeExport("RhpPInvoke")]
        static void RhpPInvoke(IntPtr frame) { }

        [RuntimeExport("RhpPInvokeReturn")]
        static void RhpPInvokeReturn(IntPtr frame) { }

        [RuntimeExport("RhpFallbackFailFast")]
        static void RhpFallbackFailFast() => ThrowHelpers.FailFast();

        // ILC guards frames that use stackalloc with a stack cookie read from
        // __security_cookie, which the CRT would define. Here it is this
        // function: the cookie is the first eight bytes of its code. A
        // constant, not a random value set at load time — enough to catch a
        // stackalloc buffer overrunning its frame, not meant to stop an
        // attacker.
        [RuntimeExport("__security_cookie")]
        static void SecurityCookie() { }
    }

    internal enum ExceptionStringID
    {
        Unknown = 0,
    }

    // memset / memcpy / memmove under the CRT names: ILC calls them for block
    // copies and initialisation, and there is no CRT here. The bodies are
    // std-no-runtime's MemoryPrimitives.
    internal static unsafe class NativeMemoryStubs
    {
        [RuntimeExport("memset")]
        private static void* Memset(void* destination, int value, ulong count)
            => SharpOS.Std.NoRuntime.MemoryPrimitives.Memset(destination, (byte)value, count);

        [RuntimeExport("memcpy")]
        private static void* Memcpy(void* destination, void* source, ulong count)
            => SharpOS.Std.NoRuntime.MemoryPrimitives.Memcpy(destination, source, count);

        [RuntimeExport("memmove")]
        private static void* Memmove(void* destination, void* source, ulong count)
            => SharpOS.Std.NoRuntime.MemoryPrimitives.Memmove(destination, source, count);
    }

    // ILC resolves these by fixed name for checked arithmetic, division and
    // bounds checks (Span's indexer calls the index one). In the app tier they
    // throw; a C library has no exceptions and a C caller could not catch one,
    // so each ends the process, as C's abort() would.
    internal static unsafe class ThrowHelpers
    {
        /// <summary>
        /// A write to address 1 — not null, so the compiler emits a plain store
        /// rather than a null check — faults, and an unhandled access violation
        /// ends the caller's process.
        /// </summary>
        public static void FailFast()
        {
            *(int*)(nint)1 = 0;
            while (true) { }
        }

        public static void ThrowOverflowException() => FailFast();
        public static void ThrowDivideByZeroException() => FailFast();
        public static void ThrowArrayTypeMismatchException() => FailFast();
        public static void ThrowFeatureBodyRemoved() => FailFast();
        public static void ThrowTypeLoadException() => FailFast();
        public static void ThrowTypeLoadExceptionWithArgument(ExceptionStringID id) => FailFast();
        public static void ThrowMissingFieldException() => FailFast();
        public static void ThrowMissingMethodException() => FailFast();
        public static void ThrowFileNotFoundException() => FailFast();
        public static void ThrowInvalidProgramException() => FailFast();
        public static void ThrowInvalidProgramExceptionWithArgument(ExceptionStringID id) => FailFast();
        public static void ThrowBadImageFormatException() => FailFast();
        public static void ThrowMarshalDirectiveException() => FailFast();
        public static void ThrowNullReferenceException() => FailFast();
        public static void ThrowIndexOutOfRangeException() => FailFast();
        public static void ThrowArgumentNullException() => FailFast();
        public static void ThrowArgumentOutOfRangeException() => FailFast();
        public static void ThrowArgumentException() => FailFast();
        public static void ThrowNotImplementedException() => FailFast();
        public static void ThrowPlatformNotSupportedException() => FailFast();
    }
}
