// The C-named entry points of the transcendental functions: thin forwards
// to the CORE-MATH port (CoreMath/, from CoreMathSharp — correctly rounded,
// see CoreMath/THIRD-PARTY.md). lgamma, lgamma_r, sincos and fma are in
// TranscendentalExtra.cs.

using CoreMathSharp;

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        // ---- double ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("acos")]
#endif
        public static double acos(double x) => StrictMath.Acos(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("acospi")]
#endif
        public static double acospi(double x) => StrictMath.AcosPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("acosh")]
#endif
        public static double acosh(double x) => StrictMath.Acosh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("asin")]
#endif
        public static double asin(double x) => StrictMath.Asin(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("asinpi")]
#endif
        public static double asinpi(double x) => StrictMath.AsinPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("asinh")]
#endif
        public static double asinh(double x) => StrictMath.Asinh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atan")]
#endif
        public static double atan(double x) => StrictMath.Atan(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atanpi")]
#endif
        public static double atanpi(double x) => StrictMath.AtanPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atanh")]
#endif
        public static double atanh(double x) => StrictMath.Atanh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atan2")]
#endif
        public static double atan2(double y, double x) => StrictMath.Atan2(y, x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atan2pi")]
#endif
        public static double atan2pi(double y, double x) => StrictMath.Atan2Pi(y, x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cbrt")]
#endif
        public static double cbrt(double x) => StrictMath.Cbrt(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cos")]
#endif
        public static double cos(double x) => StrictMath.Cos(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cospi")]
#endif
        public static double cospi(double x) => StrictMath.CosPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cosh")]
#endif
        public static double cosh(double x) => StrictMath.Cosh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sin")]
#endif
        public static double sin(double x) => StrictMath.Sin(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sinpi")]
#endif
        public static double sinpi(double x) => StrictMath.SinPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sinh")]
#endif
        public static double sinh(double x) => StrictMath.Sinh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tan")]
#endif
        public static double tan(double x) => StrictMath.Tan(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tanpi")]
#endif
        public static double tanpi(double x) => StrictMath.TanPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tanh")]
#endif
        public static double tanh(double x) => StrictMath.Tanh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erf")]
#endif
        public static double erf(double x) => StrictMath.Erf(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erfc")]
#endif
        public static double erfc(double x) => StrictMath.Erfc(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp")]
#endif
        public static double exp(double x) => StrictMath.Exp(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp2")]
#endif
        public static double exp2(double x) => StrictMath.Exp2(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp10")]
#endif
        public static double exp10(double x) => StrictMath.Exp10(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("expm1")]
#endif
        public static double expm1(double x) => StrictMath.ExpM1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp2m1")]
#endif
        public static double exp2m1(double x) => StrictMath.Exp2M1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp10m1")]
#endif
        public static double exp10m1(double x) => StrictMath.Exp10M1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log")]
#endif
        public static double log(double x) => StrictMath.Log(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log2")]
#endif
        public static double log2(double x) => StrictMath.Log2(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log10")]
#endif
        public static double log10(double x) => StrictMath.Log10(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log1p")]
#endif
        public static double log1p(double x) => StrictMath.Log1P(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log2p1")]
#endif
        public static double log2p1(double x) => StrictMath.Log2P1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log10p1")]
#endif
        public static double log10p1(double x) => StrictMath.Log10P1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("hypot")]
#endif
        public static double hypot(double x, double y) => StrictMath.Hypot(x, y);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("pow")]
#endif
        public static double pow(double x, double y) => StrictMath.Pow(x, y);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("rsqrt")]
#endif
        public static double rsqrt(double x) => StrictMath.ReciprocalSqrt(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tgamma")]
#endif
        public static double tgamma(double x) => StrictMath.TGamma(x);

        // ---- float ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("acosf")]
#endif
        public static float acosf(float x) => StrictMathF.Acos(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("acospif")]
#endif
        public static float acospif(float x) => StrictMathF.AcosPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("acoshf")]
#endif
        public static float acoshf(float x) => StrictMathF.Acosh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("asinf")]
#endif
        public static float asinf(float x) => StrictMathF.Asin(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("asinpif")]
#endif
        public static float asinpif(float x) => StrictMathF.AsinPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("asinhf")]
#endif
        public static float asinhf(float x) => StrictMathF.Asinh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atanf")]
#endif
        public static float atanf(float x) => StrictMathF.Atan(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atanpif")]
#endif
        public static float atanpif(float x) => StrictMathF.AtanPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atanhf")]
#endif
        public static float atanhf(float x) => StrictMathF.Atanh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atan2f")]
#endif
        public static float atan2f(float y, float x) => StrictMathF.Atan2(y, x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("atan2pif")]
#endif
        public static float atan2pif(float y, float x) => StrictMathF.Atan2Pi(y, x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cbrtf")]
#endif
        public static float cbrtf(float x) => StrictMathF.Cbrt(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cosf")]
#endif
        public static float cosf(float x) => StrictMathF.Cos(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("cospif")]
#endif
        public static float cospif(float x) => StrictMathF.CosPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("coshf")]
#endif
        public static float coshf(float x) => StrictMathF.Cosh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sinf")]
#endif
        public static float sinf(float x) => StrictMathF.Sin(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sinpif")]
#endif
        public static float sinpif(float x) => StrictMathF.SinPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sinhf")]
#endif
        public static float sinhf(float x) => StrictMathF.Sinh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tanf")]
#endif
        public static float tanf(float x) => StrictMathF.Tan(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tanpif")]
#endif
        public static float tanpif(float x) => StrictMathF.TanPi(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tanhf")]
#endif
        public static float tanhf(float x) => StrictMathF.Tanh(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erff")]
#endif
        public static float erff(float x) => StrictMathF.Erf(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("erfcf")]
#endif
        public static float erfcf(float x) => StrictMathF.Erfc(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("expf")]
#endif
        public static float expf(float x) => StrictMathF.Exp(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp2f")]
#endif
        public static float exp2f(float x) => StrictMathF.Exp2(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp10f")]
#endif
        public static float exp10f(float x) => StrictMathF.Exp10(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("expm1f")]
#endif
        public static float expm1f(float x) => StrictMathF.ExpM1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp2m1f")]
#endif
        public static float exp2m1f(float x) => StrictMathF.Exp2M1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("exp10m1f")]
#endif
        public static float exp10m1f(float x) => StrictMathF.Exp10M1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("logf")]
#endif
        public static float logf(float x) => StrictMathF.Log(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log2f")]
#endif
        public static float log2f(float x) => StrictMathF.Log2(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log10f")]
#endif
        public static float log10f(float x) => StrictMathF.Log10(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log1pf")]
#endif
        public static float log1pf(float x) => StrictMathF.Log1P(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log2p1f")]
#endif
        public static float log2p1f(float x) => StrictMathF.Log2P1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("log10p1f")]
#endif
        public static float log10p1f(float x) => StrictMathF.Log10P1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("hypotf")]
#endif
        public static float hypotf(float x, float y) => StrictMathF.Hypot(x, y);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("powf")]
#endif
        public static float powf(float x, float y) => StrictMathF.Pow(x, y);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("rsqrtf")]
#endif
        public static float rsqrtf(float x) => StrictMathF.ReciprocalSqrt(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("tgammaf")]
#endif
        public static float tgammaf(float x) => StrictMathF.TGamma(x);

        // compoundf (CORE-MATH): (1 + x)^y.
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("compoundf")]
#endif
        public static float compoundf(float x, float y) => StrictMathF.Compound(x, y);
    }
}
