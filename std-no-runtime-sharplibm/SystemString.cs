// System.String — a stub, as in zerosharp's MiniRuntime: SharpLibm uses no
// string at run time. Literals (the CORE-MATH guard messages) are frozen
// objects ILC lays out in the image, never allocated; ILC builds them by
// these two field names (TypePreinit.ConstructStringInstance), so they stay
// exactly as in std-no-runtime/SystemString.cs. The rest of that file — the
// string API — is not here.

using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class String
    {
        private readonly int _stringLength;
        internal char _firstChar;

        public int Length => _stringLength;
    }
}
