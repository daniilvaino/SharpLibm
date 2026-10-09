// Rounding to integral values: floor, ceil, trunc, round, roundeven, rint,
// nearbyint, modf — double and float. Bit-mask bodies after Go's
// math/floor.go and math/modf.go (BSD-3-Clause, The Go Authors; see
// THIRD-PARTY-NOTICES.md), with C semantics where Go differs (modf of ±inf
// gives a ±0 fraction, not NaN) and explicit guards where Go relies on
// shifts by 64 or more being zero (C# masks the count).
//
// rint and nearbyint round to nearest-even: the CLI fixes the rounding mode
// to nearest (ECMA-335 I.12.1.3) and raises no floating-point exceptions, so
// both are roundeven here.
//
// floor, ceil, trunc, rint, nearbyint and their float forms are replaceable
// (out of line): SSE4.1 does each in one roundsd/roundss. modf goes through
// the trunc entry, so it is accelerated with it.

using System.Runtime.CompilerServices;

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        // ---- double ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("trunc")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double trunc(double x)
        {
            ulong u = Bits.Of(x);
            int e = Bits.Exponent(u) - Bits.DoubleBias;
            if (e >= Bits.DoubleMantissaBits) return x;          // integral, ±inf, NaN
            if (e < 0) return Bits.Double(u & Bits.DoubleSignMask);
            ulong m = Bits.DoubleMantissaMask >> e;
            if ((u & m) == 0) return x;
            return Bits.Double(u & ~m);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("floor")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double floor(double x)
        {
            ulong u = Bits.Of(x);
            int e = Bits.Exponent(u) - Bits.DoubleBias;
            if (e >= Bits.DoubleMantissaBits) return x;
            if (e < 0)
            {
                if ((u << 1) == 0) return x;                     // ±0
                return (u >> 63) != 0 ? -1.0 : 0.0;
            }
            ulong m = Bits.DoubleMantissaMask >> e;
            if ((u & m) == 0) return x;
            if ((u >> 63) != 0) u += m + 1;                      // one unit up in magnitude; carries into the exponent
            return Bits.Double(u & ~m);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ceil")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double ceil(double x)
        {
            ulong u = Bits.Of(x);
            int e = Bits.Exponent(u) - Bits.DoubleBias;
            if (e >= Bits.DoubleMantissaBits) return x;
            if (e < 0)
            {
                if ((u << 1) == 0) return x;
                return (u >> 63) != 0 ? -0.0 : 1.0;
            }
            ulong m = Bits.DoubleMantissaMask >> e;
            if ((u & m) == 0) return x;
            if ((u >> 63) == 0) u += m + 1;
            return Bits.Double(u & ~m);
        }

        /// <summary>Nearest integer, halfway cases away from zero.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("round")]
#endif
        public static double round(double x)
        {
            ulong u = Bits.Of(x);
            int e = Bits.Exponent(u);
            if (e < Bits.DoubleBias)
            {
                // |x| < 1, subnormals included.
                u &= Bits.DoubleSignMask;
                if (e == Bits.DoubleBias - 1) u |= 0x3FF0_0000_0000_0000UL;   // ±1
            }
            else if (e < Bits.DoubleBias + Bits.DoubleMantissaBits)
            {
                // |x| >= 1 with a fractional part [0, 1).
                e -= Bits.DoubleBias;
                u += (1UL << (Bits.DoubleMantissaBits - 1)) >> e;
                u &= ~(Bits.DoubleMantissaMask >> e);
            }
            return Bits.Double(u);
        }

        /// <summary>Nearest integer, halfway cases to even (C23).</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("roundeven")]
#endif
        public static double roundeven(double x)
        {
            ulong u = Bits.Of(x);
            int e = Bits.Exponent(u);
            if (e >= Bits.DoubleBias + Bits.DoubleMantissaBits) return x;   // integral, ±inf, NaN
            if (e >= Bits.DoubleBias)
            {
                // |x| >= 1: add 0.499… or 0.5 before truncating, as the
                // truncated value is even or odd.
                const ulong halfMinusUlp = (1UL << (Bits.DoubleMantissaBits - 1)) - 1;
                e -= Bits.DoubleBias;
                u += (halfMinusUlp + ((u >> (Bits.DoubleMantissaBits - e)) & 1)) >> e;
                u &= ~(Bits.DoubleMantissaMask >> e);
            }
            else if (e == Bits.DoubleBias - 1 && (u & Bits.DoubleMantissaMask) != 0)
            {
                u = (u & Bits.DoubleSignMask) | 0x3FF0_0000_0000_0000UL;    // 0.5 < |x| < 1 → ±1
            }
            else
            {
                u &= Bits.DoubleSignMask;                                    // |x| <= 0.5 → ±0
            }
            return Bits.Double(u);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("rint")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double rint(double x) => roundeven(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("nearbyint")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double nearbyint(double x) => roundeven(x);

        /// <summary>Splits x into integral and fractional parts, both with the sign of x.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("modf")]
#endif
        public static double modf(double x, double* iptr)
        {
            double i = trunc(x);            // the replaceable entry: one roundsd with SSE4.1
            *iptr = i;
            if (x == i) return Bits.Double(Bits.Of(x) & Bits.DoubleSignMask);   // integral, ±inf: fraction ±0
            return x - i;                   // exact; NaN stays NaN
        }

        // ---- float ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("truncf")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float truncf(float x)
        {
            uint u = Bits.Of(x);
            int e = Bits.Exponent(u) - Bits.SingleBias;
            if (e >= Bits.SingleMantissaBits) return x;
            if (e < 0) return Bits.Single(u & Bits.SingleSignMask);
            uint m = Bits.SingleMantissaMask >> e;
            if ((u & m) == 0) return x;
            return Bits.Single(u & ~m);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("floorf")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float floorf(float x)
        {
            uint u = Bits.Of(x);
            int e = Bits.Exponent(u) - Bits.SingleBias;
            if (e >= Bits.SingleMantissaBits) return x;
            if (e < 0)
            {
                if ((u << 1) == 0) return x;
                return (u >> 31) != 0 ? -1.0f : 0.0f;
            }
            uint m = Bits.SingleMantissaMask >> e;
            if ((u & m) == 0) return x;
            if ((u >> 31) != 0) u += m + 1;
            return Bits.Single(u & ~m);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ceilf")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float ceilf(float x)
        {
            uint u = Bits.Of(x);
            int e = Bits.Exponent(u) - Bits.SingleBias;
            if (e >= Bits.SingleMantissaBits) return x;
            if (e < 0)
            {
                if ((u << 1) == 0) return x;
                return (u >> 31) != 0 ? -0.0f : 1.0f;
            }
            uint m = Bits.SingleMantissaMask >> e;
            if ((u & m) == 0) return x;
            if ((u >> 31) == 0) u += m + 1;
            return Bits.Single(u & ~m);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("roundf")]
#endif
        public static float roundf(float x)
        {
            uint u = Bits.Of(x);
            int e = Bits.Exponent(u);
            if (e < Bits.SingleBias)
            {
                u &= Bits.SingleSignMask;
                if (e == Bits.SingleBias - 1) u |= 0x3F80_0000U;
            }
            else if (e < Bits.SingleBias + Bits.SingleMantissaBits)
            {
                e -= Bits.SingleBias;
                u += (1U << (Bits.SingleMantissaBits - 1)) >> e;
                u &= ~(Bits.SingleMantissaMask >> e);
            }
            return Bits.Single(u);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("roundevenf")]
#endif
        public static float roundevenf(float x)
        {
            uint u = Bits.Of(x);
            int e = Bits.Exponent(u);
            if (e >= Bits.SingleBias + Bits.SingleMantissaBits) return x;
            if (e >= Bits.SingleBias)
            {
                const uint halfMinusUlp = (1U << (Bits.SingleMantissaBits - 1)) - 1;
                e -= Bits.SingleBias;
                u += (halfMinusUlp + ((u >> (Bits.SingleMantissaBits - e)) & 1)) >> e;
                u &= ~(Bits.SingleMantissaMask >> e);
            }
            else if (e == Bits.SingleBias - 1 && (u & Bits.SingleMantissaMask) != 0)
            {
                u = (u & Bits.SingleSignMask) | 0x3F80_0000U;
            }
            else
            {
                u &= Bits.SingleSignMask;
            }
            return Bits.Single(u);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("rintf")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float rintf(float x) => roundevenf(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("nearbyintf")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float nearbyintf(float x) => roundevenf(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("modff")]
#endif
        public static float modff(float x, float* iptr)
        {
            float i = truncf(x);            // the replaceable entry: one roundss with SSE4.1
            *iptr = i;
            if (x == i) return Bits.Single(Bits.Of(x) & Bits.SingleSignMask);
            return x - i;
        }

        // ---- to integer ----

        /// <summary>
        /// rint/round to a 64-bit integer. Out of range and NaN are
        /// unspecified in C; here they saturate (NaN gives 0), as the .NET 9+
        /// conversion does — not the x64 "integer indefinite" 0x8000….
        /// </summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("llrint")]
#endif
        public static long llrint(double x) => ToInt64(rint(x));

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("llround")]
#endif
        public static long llround(double x) => ToInt64(round(x));

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("llrintf")]
#endif
        public static long llrintf(float x) => ToInt64(rintf(x));

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("llroundf")]
#endif
        public static long llroundf(float x) => ToInt64(roundf(x));

        // C's long is 32-bit on Windows, 64-bit elsewhere; these follow the
        // 64-bit (LP64) definition. The C exports are in CAbiExports.cs, sized
        // to the target's long.
        public static long lrint(double x) => llrint(x);
        public static long lround(double x) => llround(x);
        public static long lrintf(float x) => llrintf(x);
        public static long lroundf(float x) => llroundf(x);

        private static long ToInt64(double integral)
        {
            if (Bits.IsNaN(integral)) return 0;
            if (integral >= 9223372036854775808.0) return long.MaxValue;
            if (integral < -9223372036854775808.0) return long.MinValue;
            return (long)integral;
        }
    }
}
