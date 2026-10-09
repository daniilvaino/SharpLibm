// System.ValueTuple — cut copy of SharpOS std-no-runtime/Runtime/ValueTuple.cs
// (from dotnet/runtime v8.0.27 ValueTuple.cs, TupleElementNamesAttribute.cs).
// Kept: arities 1..5 with their fields and constructors (the compiler lowers
// tuple expressions to them by name; SharpLibm returns and deconstructs
// tuples of up to five) and TupleElementNamesAttribute. Cut: IEquatable,
// Equals, GetHashCode and with them EqualityComparer<T> — tuples here are
// never compared.

namespace System
{
    public struct ValueTuple<T1>
    {
        public T1 Item1;

        public ValueTuple(T1 item1)
        {
            Item1 = item1;
        }
    }

    public struct ValueTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public struct ValueTuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }

    public struct ValueTuple<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }
    }

    public struct ValueTuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }
    }
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that the use of <see cref="System.ValueTuple"/> on a member is meant to be treated as a tuple with element names.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Event)]
    public sealed class TupleElementNamesAttribute : Attribute
    {
        private readonly string[] _transformNames;

        public TupleElementNamesAttribute(string[] transformNames)
        {
            _transformNames = transformNames;
        }

        public string[] TransformNames => _transformNames;
    }
}
