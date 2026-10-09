// A stand-in for hosts whose CoreLib has no RuntimeExportAttribute (an
// ordinary .NET project). In a NativeAOT NoStdLib image the host declares
// its own and ILC exports the marked methods under the given symbol names;
// here the attribute is inert.

namespace System.Runtime
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class RuntimeExportAttribute : Attribute
    {
        public RuntimeExportAttribute(string entry) { }
    }
}
