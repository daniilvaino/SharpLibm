// The C exports whose C signature has a `long`: scalbln, scalblnf take one,
// lrint, lround, lrintf, lroundf return one. C's long is 32 bits on Windows
// (LLP64) and 64 on Linux (LP64), while C#'s long is always 64 — so the
// exports go through these wrappers, with CLong the target's width
// (SHARPLIBM_CLONG32, set by native/ for Windows). The managed API (Libm.*)
// keeps C#'s long. Out of range, a 32-bit lrint saturates to int's range, as
// the 64-bit one does to long's (C leaves the value unspecified).
//
// nexttoward / nexttowardf are not exported: their C argument is a long
// double — double with MSVC, 80-bit x87 passed on the stack with gcc and
// mingw — which no single C# signature matches. C callers have nextafter.

#if SHARPLIBM_EXPORTS
#if SHARPLIBM_CLONG32
using CLong = System.Int32;
#else
using CLong = System.Int64;
#endif

namespace SharpLibm
{
    internal static class CAbiExports
    {
        [System.Runtime.RuntimeExport("scalbln")]
        private static double scalbln(double x, CLong n) => Libm.scalbln(x, n);

        [System.Runtime.RuntimeExport("scalblnf")]
        private static float scalblnf(float x, CLong n) => Libm.scalblnf(x, n);

        [System.Runtime.RuntimeExport("lrint")]
        private static CLong lrint(double x) => ToCLong(Libm.llrint(x));

        [System.Runtime.RuntimeExport("lround")]
        private static CLong lround(double x) => ToCLong(Libm.llround(x));

        [System.Runtime.RuntimeExport("lrintf")]
        private static CLong lrintf(float x) => ToCLong(Libm.llrintf(x));

        [System.Runtime.RuntimeExport("lroundf")]
        private static CLong lroundf(float x) => ToCLong(Libm.llroundf(x));

        private static CLong ToCLong(long v) =>
            v > CLong.MaxValue ? CLong.MaxValue : v < CLong.MinValue ? CLong.MinValue : (CLong)v;
    }
}
#endif
