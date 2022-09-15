﻿// <copyright file="MklLinearAlgebraProvider.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2018 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.IO;
using System.Security;
using Altaxo.Calc.Providers.LinearAlgebra;

namespace Altaxo.Calc.Providers.MKL.LinearAlgebra
{
  /// <summary>
  /// Error codes return from the MKL provider.
  /// </summary>
  internal enum MklError : int
  {
    /// <summary>
    /// Unable to allocate memory.
    /// </summary>
    MemoryAllocation = -999999
  }

  /// <summary>
  /// Intel's Math Kernel Library (MKL) linear algebra provider.
  /// </summary>
  internal sealed partial class MklLinearAlgebraProvider : ILinearAlgebraProvider, IDisposable
  {
    private const int MinimumCompatibleRevision = 4;
    private readonly string _hintPath;
    private readonly MklConsistency _consistency;
    private readonly MklPrecision _precision;
    private readonly MklAccuracy _accuracy;
    private int _linearAlgebraMajor;
    private int _linearAlgebraMinor;
    private int _vectorFunctionsMajor;
    private int _vectorFunctionsMinor;

    /// <param name="hintPath">Hint path where to look for the native binaries</param>
    /// <param name="consistency">
    /// Sets the desired bit consistency on repeated identical computations on varying CPU architectures,
    /// as a trade-off with performance.
    /// </param>
    /// <param name="precision">VML optimal precision and rounding.</param>
    /// <param name="accuracy">VML accuracy mode.</param>
    internal MklLinearAlgebraProvider(string hintPath, MklConsistency consistency, MklPrecision precision, MklAccuracy accuracy)
    {
      _hintPath = hintPath != null ? Path.GetFullPath(hintPath) : null;
      _consistency = consistency;
      _precision = precision;
      _accuracy = accuracy;
    }

    /// <summary>
    /// Try to find out whether the provider is available, at least in principle.
    /// Verification may still fail if available, but it will certainly fail if unavailable.
    /// </summary>
    public bool IsAvailable()
    {
      return MklProvider.IsAvailable(hintPath: _hintPath);
    }

    /// <summary>
    /// Initialize and verify that the provided is indeed available.
    /// If calling this method fails, consider to fall back to alternatives like the managed provider.
    /// </summary>
    [SecuritySafeCritical]
    public void InitializeVerify()
    {
      int revision = MklProvider.Load(_hintPath, _consistency, _precision, _accuracy);
      if (revision < MinimumCompatibleRevision)
      {
        throw new NotSupportedException(FormattableString.Invariant($"MKL Native Provider revision r{revision} is too old. Consider upgrading to a newer version. Revision r{MinimumCompatibleRevision} and newer are supported."));
      }

      _linearAlgebraMajor = SafeNativeMethods.query_capability((int)ProviderCapability.LinearAlgebraMajor);
      _linearAlgebraMinor = SafeNativeMethods.query_capability((int)ProviderCapability.LinearAlgebraMinor);
      _vectorFunctionsMajor = SafeNativeMethods.query_capability((int)ProviderCapability.VectorFunctionsMajor);
      _vectorFunctionsMinor = SafeNativeMethods.query_capability((int)ProviderCapability.VectorFunctionsMinor);

      // we only support exactly one major version, since major version changes imply a breaking change.
      if (_linearAlgebraMajor != 2)
      {
        throw new NotSupportedException(FormattableString.Invariant($"MKL Native Provider not compatible. Expecting linear algebra v2 but provider implements v{_linearAlgebraMajor}."));
      }
    }

    /// <summary>
    /// Frees memory buffers, caches and handles allocated in or to the provider.
    /// Does not unload the provider itself, it is still usable afterwards.
    /// </summary>
    public void FreeResources()
    {
      MklProvider.FreeResources();
    }

    public override string ToString()
    {
      return MklProvider.Describe();
    }

    public void Dispose()
    {
      FreeResources();
    }
  }
}
