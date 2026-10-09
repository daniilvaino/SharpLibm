// Bit access for double and float. Pointer reinterpretation: it needs nothing
// from the host CoreLib (no BitConverter, no Unsafe), and ILC turns it into a
// plain register move.

using System.Runtime.CompilerServices;

namespace SharpLibm
{
    internal static unsafe class Bits
    {
        public const int DoubleMantissaBits = 52;
        public const int DoubleBias = 1023;
        public const ulong DoubleSignMask = 0x8000_0000_0000_0000UL;
        public const ulong DoubleExponentMask = 0x7FF0_0000_0000_0000UL;
        public const ulong DoubleMantissaMask = 0x000F_FFFF_FFFF_FFFFUL;

        public const int SingleMantissaBits = 23;
        public const int SingleBias = 127;
        public const uint SingleSignMask = 0x8000_0000U;
        public const uint SingleExponentMask = 0x7F80_0000U;
        public const uint SingleMantissaMask = 0x007F_FFFFU;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Of(double x) => *(ulong*)&x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Double(ulong bits) => *(double*)&bits;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Of(float x) => *(uint*)&x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Single(uint bits) => *(float*)&bits;

        /// <summary>Biased exponent field of a double, 0..0x7FF.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Exponent(ulong bits) => (int)(bits >> DoubleMantissaBits) & 0x7FF;

        /// <summary>Biased exponent field of a float, 0..0xFF.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Exponent(uint bits) => (int)(bits >> SingleMantissaBits) & 0xFF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(double x) => (Of(x) & ~DoubleSignMask) > DoubleExponentMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(float x) => (Of(x) & ~SingleSignMask) > SingleExponentMask;

        /// <summary>Leading zero count of a 64-bit value (64 for zero), no intrinsics.</summary>
        public static int LeadingZeroCount(ulong x)
        {
            if (x == 0) return 64;
            int n = 0;
            if ((x & 0xFFFF_FFFF_0000_0000UL) == 0) { n += 32; x <<= 32; }
            if ((x & 0xFFFF_0000_0000_0000UL) == 0) { n += 16; x <<= 16; }
            if ((x & 0xFF00_0000_0000_0000UL) == 0) { n += 8; x <<= 8; }
            if ((x & 0xF000_0000_0000_0000UL) == 0) { n += 4; x <<= 4; }
            if ((x & 0xC000_0000_0000_0000UL) == 0) { n += 2; x <<= 2; }
            if ((x & 0x8000_0000_0000_0000UL) == 0) { n += 1; }
            return n;
        }

        /// <summary>Leading zero count of a 32-bit value (32 for zero), no intrinsics.</summary>
        public static int LeadingZeroCount(uint x) =>
            x == 0 ? 32 : LeadingZeroCount((ulong)x) - 32;
    }
}
