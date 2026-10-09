// compoundn, compoundnf — C23 (7.12.7.2) and IEEE 754-2019 (9.2) compound:
// (1 + x)^n for an integer n. compoundf(x, y), CORE-MATH's real-exponent
// form, is in Transcendental.cs.
//
// IEEE 754-2019 special cases: compoundn(x, 0) = 1 for every x, NaN included;
// x < −1 gives NaN; compoundn(−1, n) is +0 for n > 0 and +inf for n < 0 (a
// pole); compoundn(+inf, n) is +inf for n > 0 and +0 for n < 0.
//
// compoundnf: correctly rounded wherever n is exact in float (every |n| up to
// 2^24, and larger n with trailing zeros), through CORE-MATH's compoundf.
// Otherwise 1 + x is exact in double, pow (correctly rounded) gives the double
// result, and it is rounded once more to float.
//
// compoundn, within one ulp (not correctly rounded; glibc's vectors pass at
// that bound):
//   - 1 + x exact in double and |n| ≤ 2^53: pow(1 + x, n), correctly rounded;
//   - |x| < 2^-26: exp(n·log1p(x)) with the exponent in double-double
//     (CompoundTiny) — for n near 2^63 the exponent needs more than 53 bits;
//   - otherwise 1 + x = h + l exactly (Fast2Sum) and (h + l)^n =
//     pow(h, n) · e^(n·l/h), |l/h| ≤ 2^-53; n beyond 2^53 is split into a part
//     exact in double and a small remainder.

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        /// <summary>(1 + x)^n, n an integer (C23 compoundn).</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("compoundn")]
#endif
        public static double compoundn(double x, long n)
        {
            // x < −1 is a domain error for every n, 0 included; x^0 = 1 even for
            // a quiet NaN, but a signaling NaN signals (IEEE 754-2019 9.2).
            if (x < -1.0) return double.NaN;
            if (n == 0) return IsSignaling(x) ? x + x : 1.0;
            if (Bits.IsNaN(x)) return x + x;
            if (x == -1.0) return n > 0 ? 0.0 : double.PositiveInfinity;

            const long Exact = 1L << 53;
            // 1 + x exact and n exact in double: pow is correctly rounded.
            bool onePlusExact = (1.0 + x) - 1.0 == x;
            if (onePlusExact && n <= Exact && n >= -Exact) return pow(1.0 + x, n);

            // Small x: everything is in n·log1p(x), kept in double-double. A
            // split into pow(h, n) and a correction could not do it: for
            // x ≈ −2^-54 and n = 2^63 the first factor alone underflows.
            if (fabs(x) < 1.4901161193847656e-08) return CompoundTiny(x, n);   // 2^-26

            if (n > Exact || n < -Exact)
            {
                // n = nh + nl split in integers: nh has its low 11 bits clear, so
                // at most 52 significant bits — exact in double — and nl is small.
                long nh = n & ~0x7FFL, nl = n & 0x7FFL;
                double r = CompoundExact(x, nh);
                return nl == 0 ? r : r * CompoundExact(x, nl);
            }
            return CompoundExact(x, n);
        }

        // (1 + x)^n = exp(n·log1p(x)) for |x| < 2^-26, the exponent in
        // double-double: n = nh + nl (both exact doubles); log1p(x) =
        // x − x²/2 + x³/3 with x² exact through fma (x⁴/4 is below 2^-78 of x);
        // n·x exact through fma. Then exp(ph)·(1 + pl): one correctly rounded
        // exp and a correction far below its ulp — about one ulp in all.
        private static double CompoundTiny(double x, long n)
        {
            double nh = n & ~0x7FFL, nl = n & 0x7FFL;
            double x2h = x * x, x2l = fma(x, x, -x2h);
            double low = -0.5 * x2h - 0.5 * x2l + x2h * x / 3.0;   // log1p(x) − x
            double ph = nh * x;
            double pl = fma(nh, x, -ph) + nl * x + (nh + nl) * low;
            double s = ph + pl;                       // renormalise
            pl = (ph - s) + pl;
            double r = exp(s);
            if (r == 0.0 || r == double.PositiveInfinity) return r;
            return r + r * pl;
        }

        private static bool IsSignaling(double x) =>
            Bits.IsNaN(x) && (Bits.Of(x) & 0x0008_0000_0000_0000UL) == 0;

        // (1 + x)^n for n exact in double, x ≥ −1 finite or +inf, n ≠ 0.
        private static double CompoundExact(double x, double n)
        {
            double h = 1.0 + x;
            // Fast2Sum (|1| ≥ |x| or the other way round, both orders exact
            // for the low part when the larger magnitude goes first).
            double l = fabs(x) <= 1.0 ? (1.0 - h) + x : (x - h) + 1.0;
            double r = pow(h, n);
            if (l == 0.0 || r == 0.0 || r == double.PositiveInfinity || Bits.IsNaN(r))
                return r;
            // The correction (1 + l/h)^n = e^c. Small c: r + r·expm1(c) keeps its
            // bits; large c (huge n with tiny x, where h rounded to 1): r·e^c —
            // r + r·expm1(c) would cancel when expm1(c) is near −1.
            double c = n * (l / h);
            return fabs(c) < 0.5 ? r + r * expm1(c) : r * exp(c);
        }

        /// <summary>(1 + x)^n, n an integer (C23 compoundnf).</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("compoundnf")]
#endif
        public static float compoundnf(float x, long n)
        {
            if (x < -1.0f) return float.NaN;
            if (n == 0) return Bits.IsNaN(x) && (Bits.Of(x) & 0x0040_0000u) == 0 ? x + x : 1.0f;
            if (Bits.IsNaN(x)) return x + x;
            if (x == -1.0f) return n > 0 ? 0.0f : float.PositiveInfinity;

            float nf = n;
            if ((long)nf == n && nf != 9.22337204e18f)   // exact in float (2^63 would not convert back)
                return compoundf(x, nf);
            return (float)compoundn(x, n);                // 1 + x is exact in double
        }
    }
}
