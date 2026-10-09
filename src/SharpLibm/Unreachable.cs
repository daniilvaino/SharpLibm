// What CORE-MATH does when its error analysis turns out wrong: the three
// "unexpected worst case" / "rounding test failed" guards in pow, atan2 and
// compoundf. The C prints the input and calls exit(1) — an assertion, not an
// error a caller could handle: the result can no longer be guaranteed
// correctly rounded. The SharpLibm-tests worst-case runs never reach them.
//
// With exceptions (a .NET host, SharpOS) it throws InvalidOperationException,
// which reaches the host's unhandled-exception report with a stack. With
// SharpLibmNoExceptions=true (the NoStdLib C library: no exception types, no
// unwinder, and C callers could not catch one) it ends the process, like
// C's abort().

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        internal static double Unreachable(string message)
        {
#if SHARPLIBM_NO_EXCEPTIONS
            // A write to address 1 — not null, so the compiler emits a plain
            // store rather than a null check — faults.
            *(int*)(nint)1 = 0;
            while (true) { }
#else
            throw new System.InvalidOperationException(message);
#endif
        }
    }
}
