// sqrt, sqrtf: correctly rounded by IEEE 754 definition, one SSE2
// instruction (sqrtsd). System.Math.Sqrt is how C# reaches it: the JIT and
// ILC both treat it as an intrinsic and emit the instruction. A NoStdLib host
// must mark its Math.Sqrt [Intrinsic] for that (see README, "Host contract").
//
// sqrtf goes through double: the double square root of a float, rounded to
// float, is the correctly rounded float square root (double carries more than
// 2·24 + 2 bits, so there is no double rounding error).

namespace SharpLibm
{
    public static partial class Libm
    {
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sqrt")]
#endif
        public static double sqrt(double x) => System.Math.Sqrt(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("sqrtf")]
#endif
        public static float sqrtf(float x) => (float)System.Math.Sqrt(x);
    }
}
