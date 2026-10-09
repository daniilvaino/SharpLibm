# Third-party notices

SharpLibm itself is MIT (see `LICENSE`). Parts of it are derived from the
projects below; their notices are reproduced as their licences require. Each
source file names the origin of its code in its header.

## CoreMathSharp and CORE-MATH — MIT

Used in: `src/SharpLibm/CoreMath/` — the transcendental functions and the
software `fma`, ported from CoreMathSharp (Andante), itself a port of the
CORE-MATH project (Alexei Sibidanov et al.), with `fma` from musl. Their full
notices are kept next to the code: `src/SharpLibm/CoreMath/LICENSE.md`
(CoreMathSharp) and `src/SharpLibm/CoreMath/THIRD-PARTY.md` (CORE-MATH,
musl).

What SharpLibm changed in that port: the `#if` symbols that gated hardware
intrinsics and newer framework APIs are renamed to `COREMATHSHARP_*`, which
nothing defines, so the portable branches are used everywhere; `ParseFormat`,
`MinMax` and `Sqrt` (test helper and `System.Math` wrappers) are left out;
ranges are written as `Slice` calls. Edits beyond those mechanical ones are
marked `// SharpLibm:` — records turned into plain structs, `System.Math`
rounding and `sqrt` replaced by SharpLibm's own, the FMA routed through
`Libm.fma`, exception messages without interpolation.

```
CoreMathSharp: Copyright (c) 2026 Andante
CORE-MATH:     Copyright (c) 2023-2025 Alexei Sibidanov <sibid@uvic.ca>
musl:          Copyright © 2005-2020 Rich Felker, et al.
```

## The Go Authors — BSD-3-Clause

Used in: `src/SharpLibm/Rounding.cs` (after `math/floor.go`, `math/modf.go`),
`src/SharpLibm/Remainders.cs` (`remainder`, after `math/remainder.go`),
`src/SharpLibm/Bessel.cs` (after `math/j0.go`, `math/j1.go`, `math/jn.go`),
`src/SharpLibm/Erfinv.cs` (after `math/erfinv.go`).
Taken through the C# conversion in go2cs (`src/core/math`).

```
Copyright 2009 The Go Authors.
Copyright © 2026 The go2cs Authors. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

   * Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.
   * Neither the name of Google LLC nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```

## CosmosOS (Cosmos gen3, nativeaot-patcher) — BSD-3-Clause

Used in: `src/SharpLibm/Remainders.cs` (`fmod`, from
`src/Cosmos.Kernel.Core/Runtime/Math.cs`).

```
Copyright (c) 2007-2026, CosmosOS, COSMOS Project
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```

## Sun Microsystems fdlibm

Used in: `fmod` and `remainder` (`src/SharpLibm/Remainders.cs`), whose
algorithms are fdlibm's (e_fmod.c, e_remainder.c), and the Bessel functions
(`src/SharpLibm/Bessel.cs`, e_j0.c, e_j1.c, e_jn.c through FreeBSD msun and Go).

```
Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.

Developed at SunSoft, a Sun Microsystems, Inc. business.
Permission to use, copy, modify, and distribute this
software is freely granted, provided that this notice
is preserved.
```

## musl libc — MIT

Used in: `src/SharpLibm/Remainders.cs` (`remquo`, musl `src/math/remquo.c`,
taken through GeographicLib.NET).

```
Copyright © 2005-2020 Rich Felker, et al.

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```

## GeographicLib.NET — MIT

Used in: `src/SharpLibm/Remainders.cs` (`remquo`, `CMathManaged.cs`).

```
Copyright (c) 2008-2023, Charles Karney
Copyright (c) 2021-2024, GeographicLib.NET contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## C.math.NET — MIT

Used in: `src/SharpLibm/Manipulation.cs` (`frexp`, `ilogb`, `logb`,
`scalbln`, `nextafter` and their float forms).

```
Copyright (c) 2016 Robert Baron, Machine Cognitis

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
