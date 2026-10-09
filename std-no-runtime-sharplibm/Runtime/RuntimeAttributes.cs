// Marker types the compiler expects to exist — cut copy of SharpOS
// std-no-runtime/Runtime/RuntimeAttributes.cs. Kept: IsVolatile and
// InAttribute, which the compiler emits on volatile fields and `in`
// parameters. Cut: extension, indexer-name, module-initializer, versioning,
// CLS and caller-info attributes, and the exception types the original file
// also declares — nothing here uses them.

using System;

namespace System.Runtime.CompilerServices
{
    // Marker type the C# compiler uses as a modreq on volatile field types.
    // No members needed — its existence in this namespace is what the
    // compiler checks. Without it, `volatile int x;` gives CS0518.
    public static class IsVolatile { }
}

namespace System.Runtime.InteropServices
{
    // Required by the compiler whenever a parameter uses the `in` modifier —
    // the compiler emits `[In]` on the parameter implicitly.
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class InAttribute : Attribute
    {
        public InAttribute() { }
    }
}
