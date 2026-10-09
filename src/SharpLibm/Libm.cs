// SharpLibm — the C math library in C#, for NativeAOT NoStdLib images and
// any other .NET host. Functions carry their C names (sin, fmod, floorf …),
// take and return what the C functions do (pointers for out-values), and
// follow C99/C23 semantics, not System.Math's.
//
// Base bodies use SSE2 only and do not depend on the host CPU. Some entry
// points are kept out of line ([MethodImpl(NoInlining)], noted as
// "replaceable") so a host may overwrite their machine code with a faster
// equivalent once it knows the CPU (SSE4.1 rounding, FMA); every caller,
// inside the library too, goes through the entry and picks that up. The
// library itself knows nothing about such replacement.

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
    }
}
