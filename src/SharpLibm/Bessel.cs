// Bessel functions of the first and second kind: j0, j1, jn, y0, y1, yn and
// the float forms.
//
// Ported from Go's math/j0.go, j1.go, jn.go (BSD-3-Clause, The Go Authors),
// taken through the C# conversion in go2cs (src/core/math). Go's code is a
// simplified version of FreeBSD msun e_j0.c, e_j1.c, e_jn.c (fdlibm). The
// algorithms and constants are Go's; what changed:
//   - C semantics for signed zeros, as fdlibm and glibc: j1(±0) = ±0,
//     j1(±inf) = ±0, jn(n, x) of odd order carries the sign of x for x = ±0
//     and ±inf as well, yn(n, +inf) = −0 for odd n < 0. Go returns +0 there.
//   - jn's backward recurrence normalises with the larger of j0(x), j1(x), as
//     FreeBSD's e_jn.c does; Go's j0 alone breaks down next to a zero of J0
//     (jn(3, 2.404825557695773) was −inf). Marked "SharpLibm:" in place.
//   - The order is widened to long, so −n does not overflow for n = int.MinValue.
//   - The rational-approximation tables are method-local ReadOnlySpan<double>
//     (no static arrays), one span per kind with the four ranges in a row.
//   - sin, cos, sqrt, log are SharpLibm's own (correctly rounded).
//
// Not correctly rounded. Near the zeros of the functions the relative error is
// unbounded (the absolute error stays a few ulp of the amplitude); away from
// them a few ulp. jn and yn use the three-term recurrence: forward for n < x
// (jn) and always (yn), backward with a continued fraction for jn with n > x.
// See SharpLibm-tests (--special) for measured errors.
//
// The float forms compute in double and round once to float: the double
// result's error is far below a float ulp except next to a zero, where the
// float result inherits the double's relative error.
//
// Sun notice (fdlibm), kept as required:
// ====================================================
// Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
//
// Developed at SunPro, a Sun Microsystems, Inc. business.
// Permission to use, copy, modify, and distribute this
// software is freely granted, provided that this notice
// is preserved.
// ====================================================

using System;

namespace SharpLibm
{
    public static unsafe partial class Libm
    {
        private const double BesselInvSqrtPi = 0.5641895835477563;          // 1/sqrt(pi)
        private const double BesselTwoOverPi = 0.6366197723675814;          // 2/pi
        private const double BesselTwo129 = 680564733841876926926749214863536422912.0;   // 2^129
        private const double BesselTwo302 =
            8148143905337944345073782753637512644205873574663745002544561797417525199053346824733589504.0;   // 2^302
        private const double BesselHalfMax = 8.988465674311579e+307;       // MaxFloat64 / 2

        // ---- order 0 ----

        /// <summary>J0(x), Bessel function of the first kind of order 0. j0(±inf) = 0, j0(0) = 1.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("j0")]
#endif
        public static double j0(double x)
        {
            const double TwoM27 = 7.450580596923828e-09;   // 2^-27
            const double TwoM13 = 0.0001220703125;         // 2^-13
            const double R02 = 1.56249999999999947958e-02;
            const double R03 = -1.89979294238854721751e-04;
            const double R04 = 1.82954049532700665670e-06;
            const double R05 = -4.61832688532103189199e-09;
            const double S01 = 1.56191029464890010492e-02;
            const double S02 = 1.16926784663337450260e-04;
            const double S03 = 5.13546550207318111446e-07;
            const double S04 = 1.16614003333790000205e-09;

            if (Bits.IsNaN(x)) return x + x;
            x = fabs(x);
            if (x == double.PositiveInfinity) return 0.0;
            if (x == 0.0) return 1.0;

            if (x >= 2.0)
            {
                double s, c;
                sincos(x, &s, &c);
                double ss = s - c;
                double cc = s + c;
                // make sure x+x does not overflow
                if (x < BesselHalfMax)
                {
                    double z = -cos(x + x);
                    if (s * c < 0.0) cc = z / ss;
                    else ss = z / cc;
                }
                // j0(x) = 1/sqrt(pi) * (P(0,x)*cc - Q(0,x)*ss) / sqrt(x)
                // y0(x) = 1/sqrt(pi) * (P(0,x)*ss + Q(0,x)*cc) / sqrt(x)
                if (x > BesselTwo129)
                    return BesselInvSqrtPi * cc / sqrt(x);
                double u = BesselP0(x);
                double v = BesselQ0(x);
                return BesselInvSqrtPi * (u * cc - v * ss) / sqrt(x);
            }
            if (x < TwoM13)
            {
                if (x < TwoM27) return 1.0;
                return 1.0 - 0.25 * x * x;
            }
            double zz = x * x;
            double r = zz * (R02 + zz * (R03 + zz * (R04 + zz * R05)));
            double ssum = 1.0 + zz * (S01 + zz * (S02 + zz * (S03 + zz * S04)));
            if (x < 1.0) return 1.0 + zz * (-0.25 + (r / ssum));
            double h = 0.5 * x;
            return (1.0 + h) * (1.0 - h) + zz * (r / ssum);
        }

        /// <summary>Y0(x), Bessel function of the second kind of order 0. y0(±0) = −inf, y0(x &lt; 0) = NaN, y0(+inf) = 0.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("y0")]
#endif
        public static double y0(double x)
        {
            const double TwoM27 = 7.450580596923828e-09;   // 2^-27
            const double U00 = -7.38042951086872317523e-02;
            const double U01 = 1.76666452509181115538e-01;
            const double U02 = -1.38185671945596898896e-02;
            const double U03 = 3.47453432093683650238e-04;
            const double U04 = -3.81407053724364161125e-06;
            const double U05 = 1.95590137035022920206e-08;
            const double U06 = -3.98205194132103398453e-11;
            const double V01 = 1.27304834834123699328e-02;
            const double V02 = 7.60068627350353253702e-05;
            const double V03 = 2.59150851840457805467e-07;
            const double V04 = 4.41110311332675467403e-10;

            if (Bits.IsNaN(x)) return x + x;
            if (x == 0.0) return double.NegativeInfinity;   // C: pole error, either zero
            if (x < 0.0) return double.NaN;                 // C: domain error
            if (x == double.PositiveInfinity) return 0.0;

            if (x >= 2.0)
            {
                // y0(x) = sqrt(2/(pi*x))*(p0(x)*sin(x0)+q0(x)*cos(x0)), x0 = x-pi/4,
                // with sin(x) +- cos(x) = -cos(2x)/(sin(x) -+ cos(x)) for the worse one.
                double s, c;
                sincos(x, &s, &c);
                double ss = s - c;
                double cc = s + c;
                if (x < BesselHalfMax)
                {
                    double z = -cos(x + x);
                    if (s * c < 0.0) cc = z / ss;
                    else ss = z / cc;
                }
                if (x > BesselTwo129)
                    return BesselInvSqrtPi * ss / sqrt(x);
                double u = BesselP0(x);
                double v = BesselQ0(x);
                return BesselInvSqrtPi * (u * ss + v * cc) / sqrt(x);
            }
            if (x <= TwoM27)
                return U00 + BesselTwoOverPi * log(x);
            double zz = x * x;
            double uu = U00 + zz * (U01 + zz * (U02 + zz * (U03 + zz * (U04 + zz * (U05 + zz * U06)))));
            double vv = 1.0 + zz * (V01 + zz * (V02 + zz * (V03 + zz * V04)));
            return uu / vv + BesselTwoOverPi * j0(x) * log(x);
        }

        // pzero(x) = 1 + R/S for x >= 2, |pzero(x) - 1 - R/S| <= 2^-60.26.
        // Ranges: [8, inf), [4.5454, 8), [2.8571, 4.5454), [2, 2.8571).
        private static double BesselP0(double x)
        {
            ReadOnlySpan<double> R =
            [
                0.00000000000000000000e+00, -7.03124999999900357484e-02, -8.08167041275349795626e+00,
                -2.57063105679704847262e+02, -2.48521641009428822144e+03, -5.25304380490729545272e+03,

                -1.14125464691894502584e-11, -7.03124940873599280078e-02, -4.15961064470587782438e+00,
                -6.76747652265167261021e+01, -3.31231299649172967747e+02, -3.46433388365604912451e+02,

                -2.54704601771951915620e-09, -7.03119616381481654654e-02, -2.40903221549529611423e+00,
                -2.19659774734883086467e+01, -5.80791704701737572236e+01, -3.14479470594888503854e+01,

                -8.87534333032526411254e-08, -7.03030995483624743247e-02, -1.45073846780952986357e+00,
                -7.63569613823527770791e+00, -1.11931668860356747786e+01, -3.23364579351335335033e+00,
            ];
            ReadOnlySpan<double> S =
            [
                1.16534364619668181717e+02, 3.83374475364121826715e+03, 4.05978572648472545552e+04,
                1.16752972564375915681e+05, 4.76277284146730962675e+04,

                6.07539382692300335975e+01, 1.05125230595704579173e+03, 5.97897094333855784498e+03,
                9.62544514357774460223e+03, 2.40605815922939109441e+03,

                3.58560338055209726349e+01, 3.61513983050303863820e+02, 1.19360783792111533330e+03,
                1.12799679856907414432e+03, 1.73580930813335754692e+02,

                2.22202997532088808441e+01, 1.36206794218215208048e+02, 2.70470278658083486789e+02,
                1.53875394208320329881e+02, 1.46576176948256193810e+01,
            ];
            int k = BesselRange(x);
            ReadOnlySpan<double> p = R.Slice(6 * k, 6);
            ReadOnlySpan<double> q = S.Slice(5 * k, 5);
            double z = 1.0 / (x * x);
            double r = p[0] + z * (p[1] + z * (p[2] + z * (p[3] + z * (p[4] + z * p[5]))));
            double s = 1.0 + z * (q[0] + z * (q[1] + z * (q[2] + z * (q[3] + z * q[4]))));
            return 1.0 + r / s;
        }

        // qzero(x) = s*(-0.125 + R/S), s = 1/x, |qzero(x)/s + 0.125 - R/S| <= 2^-61.22.
        private static double BesselQ0(double x)
        {
            ReadOnlySpan<double> R =
            [
                0.00000000000000000000e+00, 7.32421874999935051953e-02, 1.17682064682252693899e+01,
                5.57673380256401856059e+02, 8.85919720756468632317e+03, 3.70146267776887834771e+04,

                1.84085963594515531381e-11, 7.32421766612684765896e-02, 5.83563508962056953777e+00,
                1.35111577286449829671e+02, 1.02724376596164097464e+03, 1.98997785864605384631e+03,

                4.37741014089738620906e-09, 7.32411180042911447163e-02, 3.34423137516170720929e+00,
                4.26218440745412650017e+01, 1.70808091340565596283e+02, 1.66733948696651168575e+02,

                1.50444444886983272379e-07, 7.32234265963079278272e-02, 1.99819174093815998816e+00,
                1.44956029347885735348e+01, 3.16662317504781540833e+01, 1.62527075710929267416e+01,
            ];
            ReadOnlySpan<double> S =
            [
                1.63776026895689824414e+02, 8.09834494656449805916e+03, 1.42538291419120476348e+05,
                8.03309257119514397345e+05, 8.40501579819060512818e+05, -3.43899293537866615225e+05,

                8.27766102236537761883e+01, 2.07781416421392987104e+03, 1.88472887785718085070e+04,
                5.67511122894947329769e+04, 3.59767538425114471465e+04, -5.35434275601944773371e+03,

                4.87588729724587182091e+01, 7.09689221056606015736e+02, 3.70414822620111362994e+03,
                6.46042516752568917582e+03, 2.51633368920368957333e+03, -1.49247451836156386662e+02,

                3.03655848355219184498e+01, 2.69348118608049844624e+02, 8.44783757595320139444e+02,
                8.82935845112488550512e+02, 2.12666388511798828631e+02, -5.31095493882666946917e+00,
            ];
            int k = BesselRange(x);
            ReadOnlySpan<double> p = R.Slice(6 * k, 6);
            ReadOnlySpan<double> q = S.Slice(6 * k, 6);
            double z = 1.0 / (x * x);
            double r = p[0] + z * (p[1] + z * (p[2] + z * (p[3] + z * (p[4] + z * p[5]))));
            double s = 1.0 + z * (q[0] + z * (q[1] + z * (q[2] + z * (q[3] + z * (q[4] + z * q[5])))));
            return (-0.125 + r / s) / x;
        }

        // Which of the four approximation ranges x (>= 2) falls into.
        private static int BesselRange(double x) =>
            x >= 8.0 ? 0 : x >= 4.5454 ? 1 : x >= 2.8571 ? 2 : 3;

        // ---- order 1 ----

        /// <summary>J1(x), Bessel function of the first kind of order 1. j1(±0) = ±0, j1(±inf) = ±0.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("j1")]
#endif
        public static double j1(double x)
        {
            const double TwoM27 = 7.450580596923828e-09;   // 2^-27
            const double R00 = -6.25000000000000000000e-02;
            const double R01 = 1.40705666955189706048e-03;
            const double R02 = -1.59955631084035597520e-05;
            const double R03 = 4.96727999609584448412e-08;
            const double S01 = 1.91537599538363460805e-02;
            const double S02 = 1.85946785588630915560e-04;
            const double S03 = 1.17718464042623683263e-06;
            const double S04 = 5.04636257076217042715e-09;
            const double S05 = 1.23542274426137913908e-11;

            if (Bits.IsNaN(x)) return x + x;
            if (x == 0.0) return x;                                          // ±0
            if (x == double.PositiveInfinity || x == double.NegativeInfinity) return 1.0 / x;   // ±0

            bool sign = x < 0.0;
            if (sign) x = -x;
            double z;
            if (x >= 2.0)
            {
                double s, c;
                sincos(x, &s, &c);
                double ss = -s - c;
                double cc = s - c;
                if (x < BesselHalfMax)
                {
                    double t = cos(x + x);
                    if (s * c > 0.0) cc = t / ss;
                    else ss = t / cc;
                }
                // j1(x) = 1/sqrt(pi) * (P(1,x)*cc - Q(1,x)*ss) / sqrt(x)
                // y1(x) = 1/sqrt(pi) * (P(1,x)*ss + Q(1,x)*cc) / sqrt(x)
                if (x > BesselTwo129)
                    z = BesselInvSqrtPi * cc / sqrt(x);
                else
                    z = BesselInvSqrtPi * (BesselP1(x) * cc - BesselQ1(x) * ss) / sqrt(x);
                return sign ? -z : z;
            }
            if (x < TwoM27)
                z = 0.5 * x;
            else
            {
                double zz = x * x;
                double r = zz * (R00 + zz * (R01 + zz * (R02 + zz * R03)));
                double s = 1.0 + zz * (S01 + zz * (S02 + zz * (S03 + zz * (S04 + zz * S05))));
                r *= x;
                z = 0.5 * x + r / s;
            }
            return sign ? -z : z;
        }

        /// <summary>Y1(x), Bessel function of the second kind of order 1. y1(±0) = −inf, y1(x &lt; 0) = NaN, y1(+inf) = 0.</summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("y1")]
#endif
        public static double y1(double x)
        {
            const double TwoM54 = 5.551115123125783e-17;   // 2^-54
            const double U00 = -1.96057090646238940668e-01;
            const double U01 = 5.04438716639811282616e-02;
            const double U02 = -1.91256895875763547298e-03;
            const double U03 = 2.35252600561610495928e-05;
            const double U04 = -9.19099158039878874504e-08;
            const double V00 = 1.99167318236649903973e-02;
            const double V01 = 2.02552581025135171496e-04;
            const double V02 = 1.35608801097516229404e-06;
            const double V03 = 6.22741452364621501295e-09;
            const double V04 = 1.66559246207992079114e-11;

            if (Bits.IsNaN(x)) return x + x;
            if (x == 0.0) return double.NegativeInfinity;   // C: pole error, either zero
            if (x < 0.0) return double.NaN;                 // C: domain error
            if (x == double.PositiveInfinity) return 0.0;

            if (x >= 2.0)
            {
                // y1(x) = sqrt(2/(pi*x))*(p1(x)*sin(x1)+q1(x)*cos(x1)), x1 = x-3pi/4.
                double s, c;
                sincos(x, &s, &c);
                double ss = -s - c;
                double cc = s - c;
                if (x < BesselHalfMax)
                {
                    double t = cos(x + x);
                    if (s * c > 0.0) cc = t / ss;
                    else ss = t / cc;
                }
                if (x > BesselTwo129)
                    return BesselInvSqrtPi * ss / sqrt(x);
                return BesselInvSqrtPi * (BesselP1(x) * ss + BesselQ1(x) * cc) / sqrt(x);
            }
            if (x <= TwoM54)
                return -BesselTwoOverPi / x;
            double z = x * x;
            double u = U00 + z * (U01 + z * (U02 + z * (U03 + z * U04)));
            double v = 1.0 + z * (V00 + z * (V01 + z * (V02 + z * (V03 + z * V04))));
            return x * (u / v) + BesselTwoOverPi * (j1(x) * log(x) - 1.0 / x);
        }

        // pone(x) = 1 + R/S for x >= 2, |pone(x) - 1 - R/S| <= 2^-60.06.
        private static double BesselP1(double x)
        {
            ReadOnlySpan<double> R =
            [
                0.00000000000000000000e+00, 1.17187499999988647970e-01, 1.32394806593073575129e+01,
                4.12051854307378562225e+02, 3.87474538913960532227e+03, 7.91447954031891731574e+03,

                1.31990519556243522749e-11, 1.17187493190614097638e-01, 6.80275127868432871736e+00,
                1.08308182990189109773e+02, 5.17636139533199752805e+02, 5.28715201363337541807e+02,

                3.02503916137373618024e-09, 1.17186865567253592491e-01, 3.93297750033315640650e+00,
                3.51194035591636932736e+01, 9.10550110750781271918e+01, 4.85590685197364919645e+01,

                1.07710830106873743082e-07, 1.17176219462683348094e-01, 2.36851496667608785174e+00,
                1.22426109148261232917e+01, 1.76939711271687727390e+01, 5.07352312588818499250e+00,
            ];
            ReadOnlySpan<double> S =
            [
                1.14207370375678408436e+02, 3.65093083420853463394e+03, 3.69562060269033463555e+04,
                9.76027935934950801311e+04, 3.08042720627888811578e+04,

                5.92805987221131331921e+01, 9.91401418733614377743e+02, 5.35326695291487976647e+03,
                7.84469031749551231769e+03, 1.50404688810361062679e+03,

                3.47913095001251519989e+01, 3.36762458747825746741e+02, 1.04687139975775130551e+03,
                8.90811346398256432622e+02, 1.03787932439639277504e+02,

                2.14364859363821409488e+01, 1.25290227168402751090e+02, 2.32276469057162813669e+02,
                1.17679373287147100768e+02, 8.36463893371618283368e+00,
            ];
            int k = BesselRange(x);
            ReadOnlySpan<double> p = R.Slice(6 * k, 6);
            ReadOnlySpan<double> q = S.Slice(5 * k, 5);
            double z = 1.0 / (x * x);
            double r = p[0] + z * (p[1] + z * (p[2] + z * (p[3] + z * (p[4] + z * p[5]))));
            double s = 1.0 + z * (q[0] + z * (q[1] + z * (q[2] + z * (q[3] + z * q[4]))));
            return 1.0 + r / s;
        }

        // qone(x) = s*(0.375 + R/S), s = 1/x, |qone(x)/s - 0.375 - R/S| <= 2^-61.13.
        private static double BesselQ1(double x)
        {
            ReadOnlySpan<double> R =
            [
                0.00000000000000000000e+00, -1.02539062499992714161e-01, -1.62717534544589987888e+01,
                -7.59601722513950107896e+02, -1.18498066702429587167e+04, -4.84385124285750353010e+04,

                -2.08979931141764104297e-11, -1.02539050241375426231e-01, -8.05644828123936029840e+00,
                -1.83669607474888380239e+02, -1.37319376065508163265e+03, -2.61244440453215656817e+03,

                -5.07831226461766561369e-09, -1.02537829820837089745e-01, -4.61011581139473403113e+00,
                -5.78472216562783643212e+01, -2.28244540737631695038e+02, -2.19210128478909325622e+02,

                -1.78381727510958865572e-07, -1.02517042607985553460e-01, -2.75220568278187460720e+00,
                -1.96636162643703720221e+01, -4.23253133372830490089e+01, -2.13719211703704061733e+01,
            ];
            ReadOnlySpan<double> S =
            [
                1.61395369700722909556e+02, 7.82538599923348465381e+03, 1.33875336287249578163e+05,
                7.19657723683240939863e+05, 6.66601232617776375264e+05, -2.94490264303834643215e+05,

                8.12765501384335777857e+01, 1.99179873460485964642e+03, 1.74684851924908907677e+04,
                4.98514270910352279316e+04, 2.79480751638918118260e+04, -4.71918354795128470869e+03,

                4.76651550323729509273e+01, 6.73865112676699709482e+02, 3.38015286679526343505e+03,
                5.54772909720722782367e+03, 1.90311919338810798763e+03, -1.35201191444307340817e+02,

                2.95333629060523854548e+01, 2.52981549982190529136e+02, 7.57502834868645436472e+02,
                7.39393205320467245656e+02, 1.55949003336666123687e+02, -4.95949898822628210127e+00,
            ];
            int k = BesselRange(x);
            ReadOnlySpan<double> p = R.Slice(6 * k, 6);
            ReadOnlySpan<double> q = S.Slice(6 * k, 6);
            double z = 1.0 / (x * x);
            double r = p[0] + z * (p[1] + z * (p[2] + z * (p[3] + z * (p[4] + z * p[5]))));
            double s = 1.0 + z * (q[0] + z * (q[1] + z * (q[2] + z * (q[3] + z * (q[4] + z * q[5])))));
            return (0.375 + r / s) / x;
        }

        // ---- order n ----

        /// <summary>
        /// Jn(x), Bessel function of the first kind of order n. jn(n, ±inf) = ±0 and
        /// jn(n, ±0) = ±0 for n ≠ 0 (the sign is that of J_n: negative for odd n and
        /// negative x, after J_{−n}(x) = J_n(−x)).
        /// </summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("jn")]
#endif
        public static double jn(int n, double x)
        {
            const double TwoM29 = 1.862645149230957e-09;   // 2^-29

            if (Bits.IsNaN(x)) return x + x;
            // J(-n, x) = (-1)^n J(n, x), J(n, -x) = (-1)^n J(n, x), so J(-n, x) = J(n, -x)
            long m = n;
            if (m < 0) { m = -m; x = -x; }
            if (m == 0) return j0(x);
            if (m == 1) return j1(x);
            bool sign = (m & 1) == 1 && (long)Bits.Of(x) < 0;   // odd n and negative x (−0, −inf too)
            x = fabs(x);
            double b;
            if (x == 0.0 || x == double.PositiveInfinity)
                b = 0.0;
            else if (m <= x)
            {
                // Safe to use J(n+1,x) = 2n/x*J(n,x) - J(n-1,x)
                if (x >= BesselTwo302)
                {
                    // x >> n^2: Jn(x) = cos(x-(2n+1)*pi/4)*sqrt(2/(x*pi))
                    double s, c;
                    sincos(x, &s, &c);
                    double temp = (m & 3) switch
                    {
                        0 => c + s,
                        1 => -c + s,
                        2 => -c - s,
                        _ => c - s,
                    };
                    b = BesselInvSqrtPi * temp / sqrt(x);
                }
                else
                {
                    b = j1(x);
                    double a = j0(x);
                    for (long i = 1; i < m; i++)
                    {
                        double t = b;
                        b = b * ((double)(i + i) / x) - a;   // avoid underflow
                        a = t;
                    }
                }
            }
            else if (x < TwoM29)
            {
                // x tiny: J(n,x) = 1/n!*(x/2)^n - ...
                if (m > 33) b = 0.0;   // underflow
                else
                {
                    double temp = x * 0.5;
                    b = temp;
                    double a = 1.0;
                    for (long i = 2; i <= m; i++)
                    {
                        a *= i;      // a = n!
                        b *= temp;   // b = (x/2)^n
                    }
                    b /= a;
                }
            }
            else
            {
                // Backward recurrence from the continued fraction
                //   J(n,x)/J(n-1,x) = 1/(w - 1/(w+h - 1/(w+2h - ...))), w = 2n/x, h = 2/x,
                // with as many terms as Q(k) = (w+k*h)*Q(k-1) - Q(k-2) takes to pass 1e9.
                double w = (double)(m + m) / x;
                double h = 2.0 / x;
                double q0 = w;
                double z = w + h;
                double q1 = w * z - 1.0;
                long k = 1;
                while (q1 < 1e9)
                {
                    k++;
                    z += h;
                    double q2 = z * q1 - q0;
                    q0 = q1;
                    q1 = q2;
                }
                long mm = m + m;
                double t = 0.0;
                for (long i = 2 * (m + k); i >= mm; i -= 2)
                    t = 1.0 / ((double)i / x - t);
                double a = t;
                b = 1.0;
                // log((2/x)^n * n!) ~ n*log(2n/x): past ln(MaxFloat64) the recurrence may
                // overflow, so rescale b on the way.
                double tmp = (double)m;
                tmp = tmp * log(fabs(2.0 / x * tmp));
                if (tmp < 7.09782712893383973096e+02)
                {
                    for (long i = m - 1; i > 0; i--)
                    {
                        double di = (double)(i + i);
                        double nb = b * di / x - a;
                        a = b;
                        b = nb;
                    }
                }
                else
                {
                    for (long i = m - 1; i > 0; i--)
                    {
                        double di = (double)(i + i);
                        double nb = b * di / x - a;
                        a = b;
                        b = nb;
                        if (b > 1e100)   // scale b to avoid spurious overflow
                        {
                            a /= b;
                            t /= b;
                            b = 1.0;
                        }
                    }
                }
                // b and a are now J(0,x) and J(1,x) up to one common factor; normalise
                // with the larger of j0, j1. SharpLibm: Go uses j0 alone, which gives
                // 0/0-like garbage (up to ±inf) next to a zero of J0, e.g.
                // jn(3, 2.404825557695773); FreeBSD's e_jn.c picks the larger, as here.
                double z0 = j0(x), z1 = j1(x);
                b = fabs(z0) >= fabs(z1) ? t * z0 / b : t * z1 / a;
            }
            return sign ? -b : b;
        }

        /// <summary>
        /// Yn(x), Bessel function of the second kind of order n. yn(n, x &lt; 0) = NaN;
        /// yn(n, ±0) = −inf, +inf for odd n &lt; 0; yn(n, +inf) = 0 (−0 for odd n &lt; 0).
        /// </summary>
#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("yn")]
#endif
        public static double yn(int n, double x)
        {
            if (Bits.IsNaN(x)) return x + x;
            if (x < 0.0) return double.NaN;   // C: domain error (−0 is not negative)
            long m = n;
            bool sign = false;                 // Y(-n, x) = (-1)^n Y(n, x)
            if (m < 0)
            {
                m = -m;
                sign = (m & 1) == 1;
            }
            if (m == 0) return y0(x);
            if (x == 0.0) return sign ? double.PositiveInfinity : double.NegativeInfinity;   // pole error
            if (x == double.PositiveInfinity) return sign ? -0.0 : 0.0;
            if (m == 1) return sign ? -y1(x) : y1(x);

            double b;
            if (x >= BesselTwo302)
            {
                // x >> n^2: Yn(x) = sin(x-(2n+1)*pi/4)*sqrt(2/(x*pi))
                double s, c;
                sincos(x, &s, &c);
                double temp = (m & 3) switch
                {
                    0 => s - c,
                    1 => -s - c,
                    2 => -s + c,
                    _ => s + c,
                };
                b = BesselInvSqrtPi * temp / sqrt(x);
            }
            else
            {
                double a = y0(x);
                b = y1(x);
                // quit if b is -inf
                for (long i = 1; i < m && b != double.NegativeInfinity; i++)
                {
                    double t = b;
                    b = ((double)(i + i) / x) * b - a;
                    a = t;
                }
            }
            return sign ? -b : b;
        }

        // ---- float: through double, rounded once ----

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("j0f")]
#endif
        public static float j0f(float x) => (float)j0(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("j1f")]
#endif
        public static float j1f(float x) => (float)j1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("jnf")]
#endif
        public static float jnf(int n, float x) => (float)jn(n, x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("y0f")]
#endif
        public static float y0f(float x) => (float)y0(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("y1f")]
#endif
        public static float y1f(float x) => (float)y1(x);

#if SHARPLIBM_EXPORTS
        [System.Runtime.RuntimeExport("ynf")]
#endif
        public static float ynf(int n, float x) => (float)yn(n, x);
    }
}
