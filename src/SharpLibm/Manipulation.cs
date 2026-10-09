// Exponent and sign manipulation: frexp, ldexp, scalbn, scalbln, ilogb,
// logb, nextafter, nexttoward, copysign, fabs, fdim, fmax, fmin — double and
// float.
//
// frexp, ilogb, logb, scalbln, nextafter: C.math.NET (MIT, Robert Baron,
// Machine Cognitis; C.math/math.cs), word access through Bits. Changes:
//   - FP_ILOGB0 / FP_ILOGBNAN / ilogb(±inf) as in musl (int.MinValue,
//     int.MinValue, int.MaxValue); C.math.NET used ±2147483647 and 32767.
//   - copysign / signbit follow IEEE: C.math.NET flipped the sign of NaN to
//     match .NET's negative default NaN; here a NaN's sign bit is its sign.
//   - nextafter returns x + y for NaN operands (payload propagates, as musl)
//     instead of a fresh NaN.
// fabs, copysign, fdim, fmax, fmin: written here (C99 7.12.11–12).

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        public const int FP_ILOGB0 = int.MinValue;
        public const int FP_ILOGBNAN = int.MinValue;

        // ---- double ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fabs")]
#endif
        public static double fabs(double x) => Bits.Double(Bits.Of(x) & ~Bits.DoubleSignMask);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("copysign")]
#endif
        public static double copysign(double x, double y) =>
            Bits.Double((Bits.Of(x) & ~Bits.DoubleSignMask) | (Bits.Of(y) & Bits.DoubleSignMask));

        /// <summary>x − y if x > y, +0 otherwise; NaN if either is NaN.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fdim")]
#endif
        public static double fdim(double x, double y)
        {
            if (Bits.IsNaN(x) || Bits.IsNaN(y)) return x + y;
            return x > y ? x - y : 0.0;
        }

        /// <summary>The larger; a NaN operand is ignored. fmax(−0, +0) = +0.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fmax")]
#endif
        public static double fmax(double x, double y)
        {
            if (Bits.IsNaN(x)) return y;
            if (Bits.IsNaN(y)) return x;
            if (x == y) return (Bits.Of(x) >> 63) != 0 ? y : x;   // zeros: prefer +0
            return x > y ? x : y;
        }

        /// <summary>The smaller; a NaN operand is ignored. fmin(−0, +0) = −0.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fmin")]
#endif
        public static double fmin(double x, double y)
        {
            if (Bits.IsNaN(x)) return y;
            if (Bits.IsNaN(y)) return x;
            if (x == y) return (Bits.Of(x) >> 63) != 0 ? x : y;   // zeros: prefer −0
            return x < y ? x : y;
        }

        /// <summary>x = m·2^*e with |m| in [0.5, 1); zero, ±inf and NaN return x with *e = 0.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("frexp")]
#endif
        public static double frexp(double x, int* e)
        {
            ulong bits = Bits.Of(x);
            int exp = Bits.Exponent(bits);
            *e = 0;
            if (exp == 0x7ff || x == 0.0) return x + x;
            *e = exp - 1022;
            if (exp == 0)
            {
                // Subnormal: scale into the normal range first.
                x *= 18014398509481984.0;                          // 2^54
                bits = Bits.Of(x);
                exp = Bits.Exponent(bits);
                *e = exp - 1022 - 54;
            }
            return Bits.Double((bits & ~Bits.DoubleExponentMask) | 0x3FE0_0000_0000_0000UL);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ilogb")]
#endif
        public static int ilogb(double x)
        {
            ulong bits = Bits.Of(x) & ~Bits.DoubleSignMask;
            if (bits == 0) return FP_ILOGB0;
            int exp = (int)(bits >> Bits.DoubleMantissaBits);
            if (exp == 0x7ff) return (bits & Bits.DoubleMantissaMask) == 0 ? int.MaxValue : FP_ILOGBNAN;
            if (exp == 0) exp -= Bits.LeadingZeroCount(bits) - 12;   // subnormal: position of the top bit
            return exp - Bits.DoubleBias;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("logb")]
#endif
        public static double logb(double x)
        {
            if (Bits.IsNaN(x)) return x + x;
            if (x == 0.0) return double.NegativeInfinity;           // C: pole error
            ulong bits = Bits.Of(x) & ~Bits.DoubleSignMask;
            if (bits == Bits.DoubleExponentMask) return double.PositiveInfinity;
            return ilogb(x);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("scalbln")]
#endif
        public static double scalbln(double x, long n)
        {
            ulong bits = Bits.Of(x);
            int exp = Bits.Exponent(bits);
            if (exp == 0x7ff) return x;                            // ±inf, NaN
            if (exp == 0)
            {
                if ((bits & Bits.DoubleMantissaMask) == 0) return x;  // ±0
                x *= 18014398509481984.0;                          // 2^54
                bits = Bits.Of(x);
                exp = Bits.Exponent(bits) - 54;
            }
            if (n < -50000) return copysign(0.0, x);
            if (n > 50000 || exp + n > 0x7fe) return copysign(double.PositiveInfinity, x);
            exp += (int)n;
            if (exp > 0) return Bits.Double((bits & ~Bits.DoubleExponentMask) | ((ulong)exp << Bits.DoubleMantissaBits));
            if (exp <= -54) return copysign(0.0, x);
            // Subnormal result: one rounding, by the final multiplication.
            exp += 54;
            x = Bits.Double((bits & ~Bits.DoubleExponentMask) | ((ulong)exp << Bits.DoubleMantissaBits));
            return x * 5.551115123125783e-17;                      // 2^-54
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("scalbn")]
#endif
        public static double scalbn(double x, int n) => scalbln(x, n);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ldexp")]
#endif
        public static double ldexp(double x, int n) => scalbln(x, n);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("nextafter")]
#endif
        public static double nextafter(double x, double y)
        {
            if (Bits.IsNaN(x) || Bits.IsNaN(y)) return x + y;
            if (x == y) return y;
            if (x == 0.0) return y > 0 ? 4.9406564584124654E-324 : -4.9406564584124654E-324;
            ulong bits = Bits.Of(x);
            // Away from zero when moving toward y increases |x|.
            if ((x > 0) ^ (x > y)) bits += 1;
            else bits -= 1;
            return Bits.Double(bits);
        }

        // nexttoward takes a long double in C; long double is double here.
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("nexttoward")]
#endif
        public static double nexttoward(double x, double y) => nextafter(x, y);

        // ---- float ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fabsf")]
#endif
        public static float fabsf(float x) => Bits.Single(Bits.Of(x) & ~Bits.SingleSignMask);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("copysignf")]
#endif
        public static float copysignf(float x, float y) =>
            Bits.Single((Bits.Of(x) & ~Bits.SingleSignMask) | (Bits.Of(y) & Bits.SingleSignMask));

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fdimf")]
#endif
        public static float fdimf(float x, float y)
        {
            if (Bits.IsNaN(x) || Bits.IsNaN(y)) return x + y;
            return x > y ? x - y : 0.0f;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fmaxf")]
#endif
        public static float fmaxf(float x, float y)
        {
            if (Bits.IsNaN(x)) return y;
            if (Bits.IsNaN(y)) return x;
            if (x == y) return (Bits.Of(x) >> 31) != 0 ? y : x;
            return x > y ? x : y;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fminf")]
#endif
        public static float fminf(float x, float y)
        {
            if (Bits.IsNaN(x)) return y;
            if (Bits.IsNaN(y)) return x;
            if (x == y) return (Bits.Of(x) >> 31) != 0 ? x : y;
            return x < y ? x : y;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("frexpf")]
#endif
        public static float frexpf(float x, int* e)
        {
            uint bits = Bits.Of(x);
            int exp = Bits.Exponent(bits);
            *e = 0;
            if (exp == 0xff || x == 0.0f) return x + x;
            *e = exp - 126;
            if (exp == 0)
            {
                x *= 33554432.0f;                                  // 2^25
                bits = Bits.Of(x);
                exp = Bits.Exponent(bits);
                *e = exp - 126 - 25;
            }
            return Bits.Single((bits & ~Bits.SingleExponentMask) | 0x3F00_0000U);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ilogbf")]
#endif
        public static int ilogbf(float x)
        {
            uint bits = Bits.Of(x) & ~Bits.SingleSignMask;
            if (bits == 0) return FP_ILOGB0;
            int exp = (int)(bits >> Bits.SingleMantissaBits);
            if (exp == 0xff) return (bits & Bits.SingleMantissaMask) == 0 ? int.MaxValue : FP_ILOGBNAN;
            if (exp == 0) exp -= Bits.LeadingZeroCount(bits) - 9;
            return exp - Bits.SingleBias;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("logbf")]
#endif
        public static float logbf(float x)
        {
            if (Bits.IsNaN(x)) return x + x;
            if (x == 0.0f) return float.NegativeInfinity;
            uint bits = Bits.Of(x) & ~Bits.SingleSignMask;
            if (bits == Bits.SingleExponentMask) return float.PositiveInfinity;
            return ilogbf(x);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("scalblnf")]
#endif
        public static float scalblnf(float x, long n)
        {
            uint bits = Bits.Of(x);
            int exp = Bits.Exponent(bits);
            if (exp == 0xff) return x;
            if (exp == 0)
            {
                if ((bits & Bits.SingleMantissaMask) == 0) return x;
                x *= 33554432.0f;                                  // 2^25
                bits = Bits.Of(x);
                exp = Bits.Exponent(bits) - 25;
            }
            if (n < -50000) return copysignf(0.0f, x);
            if (n > 50000 || exp + n > 0xfe) return copysignf(float.PositiveInfinity, x);
            exp += (int)n;
            if (exp > 0) return Bits.Single((bits & ~Bits.SingleExponentMask) | ((uint)exp << Bits.SingleMantissaBits));
            if (exp <= -25) return copysignf(0.0f, x);
            exp += 25;
            x = Bits.Single((bits & ~Bits.SingleExponentMask) | ((uint)exp << Bits.SingleMantissaBits));
            return x * 2.9802322387695312e-8f;                     // 2^-25
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("scalbnf")]
#endif
        public static float scalbnf(float x, int n) => scalblnf(x, n);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ldexpf")]
#endif
        public static float ldexpf(float x, int n) => scalblnf(x, n);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("nextafterf")]
#endif
        public static float nextafterf(float x, float y)
        {
            if (Bits.IsNaN(x) || Bits.IsNaN(y)) return x + y;
            if (x == y) return y;
            if (x == 0.0f) return y > 0 ? 1.401298464324817E-45f : -1.401298464324817E-45f;
            uint bits = Bits.Of(x);
            if ((x > 0) ^ (x > y)) bits += 1;
            else bits -= 1;
            return Bits.Single(bits);
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("nexttowardf")]
#endif
        public static float nexttowardf(float x, double y)
        {
            if (Bits.IsNaN(x) || Bits.IsNaN(y)) return (float)(x + y);
            if (x == y) return (float)y;
            if (x == 0.0f) return y > 0 ? 1.401298464324817E-45f : -1.401298464324817E-45f;
            uint bits = Bits.Of(x);
            if ((x > 0) ^ (x > y)) bits += 1;
            else bits -= 1;
            return Bits.Single(bits);
        }
    }
}
