// System.Math — cut copy of SharpOS std-no-runtime/Runtime/Math.Double.cs.
// Kept: Sqrt, the one member SharpLibm calls (sqrt/sqrtf, and CORE-MATH's
// hypot/acos/… through StrictMath.Sqrt). It is an intrinsic: ILC emits
// sqrtsd. Cut: every forward to SharpLibm.Libm — in SharpOS those are the
// public System.Math; a C library has no use for them.

namespace System
{
    public static partial class Math
    {
        // The body is only reached if the call is not expanded, and then it
        // recurses: a missing expansion shows at once instead of computing
        // something approximate.
        [System.Runtime.CompilerServices.Intrinsic]
        public static double Sqrt(double d) => Sqrt(d);
    }
}
