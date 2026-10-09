// The CORE-MATH functions whose C signatures differ from a plain forward:
// lgamma / lgamma_r (the sign of Γ through a pointer), sincos (two results
// through pointers), and fma (the replaceable entry every FMA in the library
// goes through).

using System.Runtime.CompilerServices;
using CoreMathSharp;

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        /// <summary>ln|Γ(x)|. C's lgamma also sets the global signgam; there is none here — use lgamma_r.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("lgamma")]
#endif
        public static double lgamma(double x) => StrictMath.LGamma(x).value;

        /// <summary>ln|Γ(x)|, and the sign of Γ(x) (±1) in *sign.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("lgamma_r")]
#endif
        public static double lgamma_r(double x, int* sign)
        {
            (double value, int s) = StrictMath.LGamma(x);
            *sign = s;
            return value;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("lgammaf")]
#endif
        public static float lgammaf(float x) => StrictMathF.LGamma(x).value;

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("lgammaf_r")]
#endif
        public static float lgammaf_r(float x, int* sign)
        {
            (float value, int s) = StrictMathF.LGamma(x);
            *sign = s;
            return value;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sincos")]
#endif
        public static void sincos(double x, double* sin, double* cos)
        {
            (double s, double c) = StrictMath.SinCos(x);
            *sin = s;
            *cos = c;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sincosf")]
#endif
        public static void sincosf(float x, float* sin, float* cos)
        {
            (float s, float c) = StrictMathF.SinCos(x);
            *sin = s;
            *cos = c;
        }

        /// <summary>x·y + z rounded once. Replaceable: FMA3 does it in one vfmadd.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fma")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static double fma(double x, double y, double z) => StrictMath.FusedMultiplyAddCore(x, y, z);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fmaf")]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static float fmaf(float x, float y, float z) => StrictMathF.FusedMultiplyAddCore(x, y, z);
    }
}
