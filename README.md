# SharpLibm

The C math library (`libm`) in C#: `sin`, `pow`, `fmod`, `floorf` and the
rest, with C names, C signatures and C99/C23 semantics. Written to compile
under NativeAOT with `NoStdLib` — no CoreLib math, no native libm, no static
constructors, no heap — and usable from any other .NET host as well.

Status: early.

- Exact operations (rounding to integral values, `fmod`/`remainder`/`remquo`,
  `frexp`/`ldexp`/`scalbn`/`ilogb`/`logb`/`nextafter`, `copysign`/`fabs`/`fdim`/
  `fmax`/`fmin`, `llrint`/`llround`), double and float: bit-exact against
  musl's libc-test vectors.
- Transcendental functions — 47 for double, 48 for float, from CORE-MATH
  through CoreMathSharp (correctly rounded): exp/log families, pow, trig and
  inverse trig, the π-scaled forms, hyperbolic, erf/erfc, tgamma/lgamma,
  hypot, cbrt, rsqrt, compoundf, and a correctly rounded software `fma`.
  Double: bit-exact against MPFR on all of CORE-MATH's worst cases (≈30 M
  inputs). Float: bit-exact against CORE-MATH's C on every one of the 2^32
  inputs.
- CoreMathSharp 1.0.0 is not correctly rounded on hard inputs: about 80
  translation errors, several of them gross (pow, tgamma, log1p, expm1,
  rsqrtf, exp10f). They are fixed here and marked `// SharpLibm fix:`,
  together with CORE-MATH fixes made after the port (hypot, cospi, pow,
  error bounds).
- Builds and links under NativeAOT 8 `NoStdLib` with no native math left.
- Not yet: Bessel functions, `erfinv`.

## Design

- **Base bodies use SSE2 only.** The library does not detect the CPU and
  does not depend on it beyond x86-64's baseline.
- **Replaceable entry points.** Functions that one instruction does on newer
  CPUs — `floor`, `ceil`, `trunc`, `rint`, `nearbyint`, `modf` (SSE4.1
  `roundsd`/`roundss`) and `fma`, `fmaf` (FMA3) — are kept out of line
  (`[MethodImpl(NoInlining)]`). A host that knows the CPU may overwrite their
  machine code with the instruction; every caller, inside the library as
  well, goes through the entry point and picks that up. Without that, the
  library works the same, only slower. SharpLibm itself does none of this.
- **Semantics are C's**, not `System.Math`'s: `fmax` ignores a NaN operand,
  `modf` of ±inf gives a ±0 fraction, NaN signs are IEEE signs.
- The rounding mode is always round-to-nearest (the CLI fixes it) and no
  floating-point exception flags are raised.

## Using it

Import `SharpLibm.props` into the consuming project; the sources compile
into that assembly (in a NativeAOT `NoStdLib` image the application is its own
system module, so a separate compiled assembly would have no CoreLib to bind
against).

```xml
<Import Project="path/to/SharpLibm/SharpLibm.props" />
```

Then call `SharpLibm.Libm.fmod(x, y)` and so on.

Properties:

- `SharpLibmExports=true` — export every function under its C name
  (`[RuntimeExport("fmod")]`) so C code linked into the image can call it.
  Off by default: an exported method is a root, and ILC would then keep every
  function and its tables in the image.
- `SharpLibmDefineRuntimeExport=true` — compile a stand-in
  `System.Runtime.RuntimeExportAttribute` for hosts whose CoreLib has none
  (an ordinary .NET project). A `NoStdLib` host declares its own.

### Host contract

What the consuming CoreLib must provide:

- `unsafe` code (pointers), `MethodImplAttribute`;
- `System.Math.Sqrt` marked `[Intrinsic]`, so the compiler emits `sqrtsd`
  (an ordinary .NET CoreLib has this);
- `Span<T>` / `ReadOnlySpan<T>` with `Slice`, and
  `RuntimeHelpers.CreateSpan<T>` — constant tables are `ReadOnlySpan<T> t = [...]`,
  which then live in the image's data instead of being allocated;
- `MemoryMarshal.Cast<TFrom, TTo>(ReadOnlySpan<TFrom>)`, `stackalloc`;
- `ValueTuple<…>` (2 to 4 elements) and deconstruction;
- `InvalidOperationException` (CORE-MATH's "unexpected worst case" guard).

No `System.Range`, no records, no `BitConverter`, no string formatting, no
static constructors, no heap allocation.

## Tests

The tests live in a separate repository, SharpLibm-tests: the test suites
they use come under many licences (MIT, BSD, Sun, GPL, LGPL), so they are kept
apart from this MIT code. They run on an ordinary .NET 10 host and check,
bit for bit, in round-to-nearest:

- exact operations (floor … fmod, remquo, frexp, nextafter …) against musl's libc-test;
- every transcendental function within 0.5 ulp on libc-test's vectors;
- the double functions on CORE-MATH's worst cases (≈30 million inputs) against MPFR;
- the float functions on all 2^32 inputs against CORE-MATH's C.

## Licence

MIT (`LICENSE`). Code derived from other projects keeps their notices; see
`THIRD-PARTY-NOTICES.md` and the header of each source file.
