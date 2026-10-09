// Inverse error functions: erfinv, erfcinv and the float forms. Not in ISO C;
// the names and semantics are those of CUDA's and Go's (Erfinv, Erfcinv).
//
// Ported from Go's math/erfinv.go (BSD-3-Clause, The Go Authors), taken
// through the C# conversion in go2cs (src/core/math): Wichura's rational
// approximation of the normal quantile, AS 241 (PPND16), Applied Statistics
// 37 (1988), https://www.jstor.org/stable/2347330. What changed:
//   - erfcinv does not go through erfinv(1 − x): 1 − x loses the low bits of
//     a small x (and gives erfcinv(x) = +inf for x < 2^-54). The tail of the
//     approximation needs 1 − |erfinv argument|, which for erfcinv is x itself
//     (or 2 − x, exact for x in [1, 2]), so both functions call one core with
//     that value passed exactly.
//   - erfinv(−0) = −0 (Go: +0).
//
// Then one Newton step on SharpLibm's correctly rounded erf / erfc (Refine):
// about one ulp — erfinv at most 1.33, erfcinv 4.58 for arguments below 1e-300
// where the step is skipped (SharpLibm-tests, --special). Not correctly rounded.
// The float forms compute in double and round once to float.

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        /// <summary>The y with erf(y) = x. erfinv(±1) = ±inf, NaN for |x| &gt; 1.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erfinv")]
#endif
        public static double erfinv(double x)
        {
            if (Bits.IsNaN(x)) return x + x;
            double a = fabs(x);
            if (a >= 1.0)
                return a == 1.0 ? copysign(double.PositiveInfinity, x) : double.NaN;
            // 1 - a is exact where the tail uses it (a > 0.85) and where
            // Refine does (a > 0.5): Sterbenz.
            double q = 1.0 - a;
            return copysign(Refine(ErfinvCore(a, q), a, q), x);
        }

        /// <summary>The y with erfc(y) = x. erfcinv(0) = +inf, erfcinv(2) = −inf, NaN outside [0, 2].</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erfcinv")]
#endif
        public static double erfcinv(double x)
        {
            if (Bits.IsNaN(x)) return x + x;
            if (x <= 0.0 || x >= 2.0)
            {
                if (x == 0.0) return double.PositiveInfinity;
                if (x == 2.0) return double.NegativeInfinity;
                return double.NaN;
            }
            // erfcinv(x) = erfinv(1 - x); 1 - |1 - x| is x for x <= 1 and 2 - x
            // (exact, Sterbenz) for x in [1, 2).
            // 1 - x is exact for x in [0.5, 1], x - 1 for x in [1, 2]: where
            // Refine uses y they are exact, and q is always exact.
            if (x <= 1.0)
            {
                double y = 1.0 - x;
                return Refine(ErfinvCore(y, x), y, x);
            }
            double yn = x - 1.0, qn = 2.0 - x;
            return -Refine(ErfinvCore(yn, qn), yn, qn);
        }

        /// <summary>
        /// One Newton step on the root r of erf(r) = y, from the rational
        /// approximation (a few ulp) to about one ulp. The residual comes from
        /// SharpLibm's correctly rounded erf, or erfc in the tail — with q = 1 − y
        /// passed exactly, since there erf(r) − y would cancel away. erf' is
        /// (2/√π)·e^{−r²}; the step divides by it as a product with e^{r²},
        /// skipped where that overflows (r &gt; ~26.6, q below 1e-300: the tail
        /// approximation alone is used there).
        /// </summary>
        private static double Refine(double r, double y, double q)
        {
            const double HalfSqrtPi = 0.886226925452758013649083741671; // √π / 2
            double scale = exp(r * r);
            if (scale == double.PositiveInfinity) return r;
            double residual = y <= 0.5 ? erf(r) - y : q - erfc(r);
            return r - residual * HalfSqrtPi * scale;
        }

        /// <summary>erfinv(y) for y in [0, 1), given q = 1 − y (exactly, where it matters: y &gt; 0.85).</summary>
        private static double ErfinvCore(double y, double q)
        {
            const double Ln2 = 0.693147180559945309417232121458176568;
            const double A0 = 1.1975323115670912564578e0;
            const double A1 = 4.7072688112383978012285e1;
            const double A2 = 6.9706266534389598238465e2;
            const double A3 = 4.8548868893843886794648e3;
            const double A4 = 1.6235862515167575384252e4;
            const double A5 = 2.3782041382114385731252e4;
            const double A6 = 1.1819493347062294404278e4;
            const double A7 = 8.8709406962545514830200e2;
            const double B0 = 1.0000000000000000000e0;
            const double B1 = 4.2313330701600911252e1;
            const double B2 = 6.8718700749205790830e2;
            const double B3 = 5.3941960214247511077e3;
            const double B4 = 2.1213794301586595867e4;
            const double B5 = 3.9307895800092710610e4;
            const double B6 = 2.8729085735721942674e4;
            const double B7 = 5.2264952788528545610e3;
            const double C0 = 1.42343711074968357734e0;
            const double C1 = 4.63033784615654529590e0;
            const double C2 = 5.76949722146069140550e0;
            const double C3 = 3.64784832476320460504e0;
            const double C4 = 1.27045825245236838258e0;
            const double C5 = 2.41780725177450611770e-1;
            const double C6 = 2.27238449892691845833e-2;
            const double C7 = 7.74545014278341407640e-4;
            const double D0 = 1.4142135623730950488016887e0;
            const double D1 = 2.9036514445419946173133295e0;
            const double D2 = 2.3707661626024532365971225e0;
            const double D3 = 9.7547832001787427186894837e-1;
            const double D4 = 2.0945065210512749128288442e-1;
            const double D5 = 2.1494160384252876777097297e-2;
            const double D6 = 7.7441459065157709165577218e-4;
            const double D7 = 1.4859850019840355905497876e-9;
            const double E0 = 6.65790464350110377720e0;
            const double E1 = 5.46378491116411436990e0;
            const double E2 = 1.78482653991729133580e0;
            const double E3 = 2.96560571828504891230e-1;
            const double E4 = 2.65321895265761230930e-2;
            const double E5 = 1.24266094738807843860e-3;
            const double E6 = 2.71155556874348757815e-5;
            const double E7 = 2.01033439929228813265e-7;
            const double F0 = 1.414213562373095048801689e0;
            const double F1 = 8.482908416595164588112026e-1;
            const double F2 = 1.936480946950659106176712e-1;
            const double F3 = 2.103693768272068968719679e-2;
            const double F4 = 1.112800997078859844711555e-3;
            const double F5 = 2.611088405080593625138020e-5;
            const double F6 = 2.010321207683943062279931e-7;
            const double F7 = 2.891024605872965461538222e-15;

            if (y <= 0.85)
            {
                double r = 0.180625 - 0.25 * y * y;
                double z1 = ((((((A7 * r + A6) * r + A5) * r + A4) * r + A3) * r + A2) * r + A1) * r + A0;
                double z2 = ((((((B7 * r + B6) * r + B5) * r + B4) * r + B3) * r + B2) * r + B1) * r + B0;
                return (y * z1) / z2;
            }
            double t = sqrt(Ln2 - log(q));
            double n1, n2;
            if (t <= 5.0)
            {
                t -= 1.6;
                n1 = ((((((C7 * t + C6) * t + C5) * t + C4) * t + C3) * t + C2) * t + C1) * t + C0;
                n2 = ((((((D7 * t + D6) * t + D5) * t + D4) * t + D3) * t + D2) * t + D1) * t + D0;
            }
            else
            {
                t -= 5.0;
                n1 = ((((((E7 * t + E6) * t + E5) * t + E4) * t + E3) * t + E2) * t + E1) * t + E0;
                n2 = ((((((F7 * t + F6) * t + F5) * t + F4) * t + F3) * t + F2) * t + F1) * t + F0;
            }
            return n1 / n2;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erfinvf")]
#endif
        public static float erfinvf(float x) => (float)erfinv(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erfcinvf")]
#endif
        public static float erfcinvf(float x) => (float)erfcinv(x);
    }
}
