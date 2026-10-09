using System.Runtime.CompilerServices;

#if COREMATHSHARP_NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
#endif

namespace CoreMathSharp;

internal static partial class StrictMathF
{
    /// <inheritdoc cref="StrictMath.FusedMultiplyAdd(double, double, double)"/>
    // SharpLibm: through SharpLibm.Libm.fmaf, as the double form (see FusedMultiplyAdd.cs).
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FusedMultiplyAdd(float x, float y, float z) => SharpLibm.Libm.fmaf(x, y, z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float FusedMultiplyAddCore(float x, float y, float z)
    {
#if COREMATHSHARP_NETCOREAPP3_0_OR_GREATER
        //return MathF.FusedMultiplyAdd(x, y, z);

        if (Fma.IsSupported)
        {
            return Fma.MultiplyAdd(Vector128.CreateScalarUnsafe(x), Vector128.CreateScalarUnsafe(y), Vector128.CreateScalarUnsafe(z)).ToScalar();
        }
        if (AdvSimd.IsSupported)
        {
            return AdvSimd.FusedMultiplyAddScalar(Vector64.CreateScalarUnsafe(z), Vector64.CreateScalarUnsafe(x), Vector64.CreateScalarUnsafe(y)).ToScalar();
        }
#endif

        /*
        // https://hal.science/hal-04575249/document
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool isNot1Or3TimesPowerOf2(float x)
        {
            float delta = (4194305.0f * x) - (4194304.0f * x);
            return delta != x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (float h, float l) twoSum(float a, float b)
        {
            float h = a + b;
            float aprime = h - b;
            float l = (a - aprime) + (b - (h - aprime));
            return (h, l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (float h, float l) split(float x)
        {
            float k = 4097.0f;
            float gamma = k * x;
            float h = gamma + (x - gamma);
            float l = x - h;
            return (h, l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (float h, float l) dekkerProd(float a, float b)
        {
            (float ah, float al) = split(a);
            (float bh, float bl) = split(b);

            float h = a * b;
            float l = (((-h + ah * bh) + (ah * bl)) + al * bh) + al * bl;
            return (h, l);
        }


        static float FastEmulation(float x, float y, float z)
        {
            float xl, xh, sl, sh, vl, vh;
            (xh, xl) = dekkerProd(x, y);

            if (!float.IsNormal(xh))
            {
                return float.NaN;
            }

            (sh, sl) = twoSum(xh, z);
            (vh, vl) = twoSum(xl, sl);

            if (!float.IsNormal(vh))
            {
                return float.NaN;
            }

            if (!float.IsFinite(sh) || !float.IsFinite(xl))
            {
                if (float.IsFinite(x) && float.IsFinite(y) && !float.IsFinite(z))
                {
                    return z;
                }
                return sh;
            }

            if (isNot1Or3TimesPowerOf2(vh) || vl == 0.0f)
            {
                return sh + vh;
            }
            if ((vl < 0.0f) ^ (vh < 0.0f))
            {
                return sh + (0.875f * vh);
            }
            return sh + (1.125f * vh);
        }
        //*/



        // SharpLibm fix: musl's fmaf (the port's Fallback) catches a double-
        // rounding tie by testing the bits that float precision drops — which
        // is too few bits when the result is a float subnormal, so 23 of
        // TestFloat's 6.1 M f32_mulAdd level-1 cases came out one ulp off.
        // Round to odd instead: x·y is exact in double (24 + 24 bits), the sum
        // with z is r plus an exact error (TwoSum); when the error is not zero,
        // r's last bit is forced to 1 toward the exact value. Rounding that to
        // float is then correct at any float precision, subnormal included,
        // because double carries at least two more bits (Boldo–Melquiond).
        static float Fallback(float x, float y, float z)
        {
            double xy = (double)x * y;
            double r = xy + z;

            ulong u = Polyfill.DoubleToUInt64Bits(r);
            if ((u & 0x7ff0000000000000ul) == 0x7ff0000000000000ul)
            {
                return (float)r;                        // ±inf, NaN
            }

            double bz = r - xy;
            double err = (xy - (r - bz)) + (z - bz);
            if (err != 0 && (u & 1) == 0)
            {
                // Toward the exact value: up in magnitude when err has r's sign.
                if ((err > 0) == (r > 0))
                {
                    u++;
                }
                else
                {
                    u--;
                }
            }

            return (float)Polyfill.UInt64BitsToDouble(u);
        }


        /*
        // not so fast (about 3x slower)
        float fastPath = FastEmulation(x, y, z);
        if (!float.IsNaN(fastPath))
        {
            return fastPath;
        }
        //*/
        return Fallback(x, y, z);
    }
}
