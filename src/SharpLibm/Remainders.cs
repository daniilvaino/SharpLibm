// Remainders: fmod, remainder, remquo — double and float.
//
// fmod: fdlibm e_fmod.c via Cosmos gen3 (nativeaot-patcher,
//   src/Cosmos.Kernel.Core/Runtime/Math.cs; BSD-3-Clause, CosmosOS), word
//   access through Bits. Exact: shift-and-subtract on the significands.
// remainder: FreeBSD msun e_remainder.c via Go's math/remainder.go
//   (BSD-3-Clause, The Go Authors), reduction through fmod here instead of
//   Go's Mod.
// remquo: musl src/math/remquo.c via GeographicLib.NET (MIT; CMathManaged.cs).
// The float forms compute in double: for float operands each result is exact
// in double and representable in float, so the conversion is exact.
//
// Sun notice (fdlibm), kept as required:
// ====================================================
// Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
//
// Developed at SunSoft, a Sun Microsystems, Inc. business.
// Permission to use, copy, modify, and distribute this
// software is freely granted, provided that this notice
// is preserved.
// ====================================================

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        private static int HighWord(double x) => (int)(Bits.Of(x) >> 32);

        private static uint LowWord(double x) => (uint)Bits.Of(x);

        private static double FromWords(int hi, uint lo) => Bits.Double(((ulong)(uint)hi << 32) | lo);

        /// <summary>x − n·y, n = trunc(x/y), exactly; the sign of x.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fmod")]
#endif
        public static double fmod(double x, double y)
        {
            int n, hx, hy, hz, ix, iy, sx, i;
            uint lx, ly, lz;

            hx = HighWord(x);
            lx = LowWord(x);
            hy = HighWord(y);
            ly = LowWord(y);
            sx = hx & unchecked((int)0x80000000);   /* sign of x */
            hx ^= sx;                               /* |x| */
            hy &= 0x7fffffff;                       /* |y| */

            /* purge off exception values: y=0, x not finite, or y NaN */
            if ((hy | (int)ly) == 0 || hx >= 0x7ff00000 ||
                (hy | (ly != 0 ? 1 : 0)) > 0x7ff00000)
                return (x * y) / (x * y);

            if (hx <= hy)
            {
                if (hx < hy || lx < ly) return x;               /* |x| < |y| */
                if (lx == ly) return sx != 0 ? -0.0 : 0.0;      /* |x| == |y| */
            }

            /* determine ix = ilogb(x) */
            if (hx < 0x00100000)
            {
                if (hx == 0) { for (ix = -1043, i = (int)lx; i > 0; i <<= 1) ix -= 1; }
                else { for (ix = -1022, i = hx << 11; i > 0; i <<= 1) ix -= 1; }
            }
            else ix = (hx >> 20) - 1023;

            /* determine iy = ilogb(y) */
            if (hy < 0x00100000)
            {
                if (hy == 0) { for (iy = -1043, i = (int)ly; i > 0; i <<= 1) iy -= 1; }
                else { for (iy = -1022, i = hy << 11; i > 0; i <<= 1) iy -= 1; }
            }
            else iy = (hy >> 20) - 1023;

            /* set up {hx,lx}, {hy,ly} and align y to x */
            if (ix >= -1022) hx = 0x00100000 | (0x000fffff & hx);
            else
            {
                /* subnormal x, shift x to normal */
                n = -1022 - ix;
                if (n <= 31) { hx = (hx << n) | (int)(lx >> (32 - n)); lx <<= n; }
                else { hx = (int)(lx << (n - 32)); lx = 0; }
            }
            if (iy >= -1022) hy = 0x00100000 | (0x000fffff & hy);
            else
            {
                /* subnormal y, shift y to normal */
                n = -1022 - iy;
                if (n <= 31) { hy = (hy << n) | (int)(ly >> (32 - n)); ly <<= n; }
                else { hy = (int)(ly << (n - 32)); ly = 0; }
            }

            /* fixed-point fmod */
            n = ix - iy;
            while (n-- != 0)
            {
                hz = hx - hy; lz = lx - ly; if (lx < ly) hz -= 1;
                if (hz < 0) { hx = hx + hx + (int)(lx >> 31); lx += lx; }
                else
                {
                    if ((hz | (int)lz) == 0) return sx != 0 ? -0.0 : 0.0;   /* return sign(x)*0 */
                    hx = hz + hz + (int)(lz >> 31); lx = lz + lz;
                }
            }
            hz = hx - hy; lz = lx - ly; if (lx < ly) hz -= 1;
            if (hz >= 0) { hx = hz; lx = lz; }

            /* convert back to floating value and restore the sign */
            if ((hx | (int)lx) == 0) return sx != 0 ? -0.0 : 0.0;
            while (hx < 0x00100000)
            {
                /* normalize x */
                hx = hx + hx + (int)(lx >> 31); lx += lx;
                iy -= 1;
            }
            if (iy >= -1022)
            {
                /* normalize output */
                hx = (hx - 0x00100000) | ((iy + 1023) << 20);
                return FromWords(hx | sx, lx);
            }

            /* subnormal output */
            n = -1022 - iy;
            if (n <= 20) { lx = (lx >> n) | ((uint)hx << (32 - n)); hx >>= n; }
            else if (n <= 31) { lx = (uint)((hx << (32 - n)) | (int)(lx >> n)); hx = sx; }
            else { lx = (uint)(hx >> (n - 32)); hx = sx; }
            return FromWords(hx | sx, lx) * 1.0;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("fmodf")]
#endif
        public static float fmodf(float x, float y) => (float)fmod(x, y);

        /// <summary>IEEE remainder: x − n·y, n the integer nearest x/y, ties to even.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("remainder")]
#endif
        public static double remainder(double x, double y)
        {
            const double Tiny = 4.45014771701440276618e-308;   // 0x0020000000000000
            const double HalfMax = 8.988465674311579e+307;       // MaxValue / 2

            if (Bits.IsNaN(x) || Bits.IsNaN(y)) return x + y;
            if ((Bits.Of(x) << 1) == (Bits.DoubleExponentMask << 1) || y == 0.0)
                return (x * y) / (x * y);                       // remainder(±inf, y), remainder(x, 0): NaN
            if ((Bits.Of(y) << 1) == (Bits.DoubleExponentMask << 1)) return x;

            bool sign = false;
            if (x < 0.0) { x = -x; sign = true; }
            if (y < 0.0) y = -y;
            if (x == y) return sign ? -0.0 : 0.0;
            if (y <= HalfMax) x = fmod(x, y + y);               // now x < 2y
            if (y < Tiny)
            {
                if (x + x > y)
                {
                    x -= y;
                    if (x + x >= y) x -= y;
                }
            }
            else
            {
                double yHalf = 0.5 * y;
                if (x > yHalf)
                {
                    x -= y;
                    if (x >= yHalf) x -= y;
                }
            }
            // A zero result keeps the sign of x (−0 for negative x).
            return sign ? -x : x;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("remainderf")]
#endif
        public static float remainderf(float x, float y) => (float)remainder(x, y);

        /// <summary>remainder(x, y), and in *quo the sign and at least the low 3 bits of the quotient.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("remquo")]
#endif
        public static double remquo(double x, double y, int* quo)
        {
            ulong uxi = Bits.Of(x);
            ulong uyi = Bits.Of(y);
            int ex = (int)(uxi >> 52 & 0x7ff);
            int ey = (int)(uyi >> 52 & 0x7ff);
            int sx = (int)(uxi >> 63);
            int sy = (int)(uyi >> 63);
            uint q;
            ulong i;

            *quo = 0;
            if (uyi << 1 == 0 || Bits.IsNaN(y) || ex == 0x7ff)
                return (x * y) / (x * y);
            if (uxi << 1 == 0)
                return x;

            /* normalize x and y */
            if (ex == 0)
            {
                for (i = uxi << 12; i >> 63 == 0; ex--, i <<= 1) ;
                uxi <<= -ex + 1;
            }
            else
            {
                uxi &= ulong.MaxValue >> 12;
                uxi |= 1UL << 52;
            }
            if (ey == 0)
            {
                for (i = uyi << 12; i >> 63 == 0; ey--, i <<= 1) ;
                uyi <<= -ey + 1;
            }
            else
            {
                uyi &= ulong.MaxValue >> 12;
                uyi |= 1UL << 52;
            }

            q = 0;
            if (ex < ey)
            {
                if (ex + 1 == ey) goto end;
                return x;
            }

            /* x mod y */
            for (; ex > ey; ex--)
            {
                i = uxi - uyi;
                if (i >> 63 == 0) { uxi = i; q++; }
                uxi <<= 1;
                q <<= 1;
            }
            i = uxi - uyi;
            if (i >> 63 == 0) { uxi = i; q++; }
            if (uxi == 0) ex = -60;
            else for (; uxi >> 52 == 0; uxi <<= 1, ex--) ;

        end:
            /* scale result and decide between |x| and |x|-|y| */
            if (ex > 0)
            {
                uxi -= 1UL << 52;
                uxi |= (ulong)ex << 52;
            }
            else
            {
                uxi >>= -ex + 1;
            }
            x = Bits.Double(uxi);
            if (sy != 0) y = -y;
            if (ex == ey || (ex + 1 == ey && (2 * x > y || (2 * x == y && (q % 2) != 0))))
            {
                x -= y;
                q++;
            }
            q &= 0x7fffffff;
            *quo = (sx ^ sy) != 0 ? -(int)q : (int)q;
            return sx != 0 ? -x : x;
        }

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("remquof")]
#endif
        public static float remquof(float x, float y, int* quo) => (float)remquo(x, y, quo);
    }
}
