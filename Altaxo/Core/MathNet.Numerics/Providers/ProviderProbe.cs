// <copyright file="ProviderProbe.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// https://numerics.mathdotnet.com
// https://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2021 Math.NET
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
using System.Threading;

namespace Altaxo.Calc.Providers
{
  /// <summary>
  /// Lazily probes for and creates provider instances.
  /// </summary>
  /// <typeparam name="T">The provider type.</typeparam>
  public class ProviderProbe<T> where T : class
  {
    private readonly bool _disabled;
    private readonly Lazy<IProviderCreator<T>> _creator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderProbe{T}"/> class.
    /// </summary>
    /// <param name="typeName">The assembly-qualified type name of the provider creator.</param>
    /// <param name="disabled">If set to <see langword="true"/>, probing is disabled.</param>
    public ProviderProbe(string typeName, bool disabled = false)
    {
      _disabled = disabled;
      _creator = new Lazy<IProviderCreator<T>>(() =>
      {
        var type = Type.GetType(typeName);
        return type is null ? null : Activator.CreateInstance(type) as IProviderCreator<T>;
      }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Creates the provider or throws if probing is disabled or fails.
    /// </summary>
    /// <returns>The created provider.</returns>
    public T Create()
    {
      if (_disabled)
      {
        throw new NotSupportedException("Specific Native Provider disabled by an application switch");
      }

      if (AppSwitches.DisableNativeProviders)
      {
        throw new NotSupportedException("Native Providers are disabled by an application switch");
      }

      if (AppSwitches.DisableNativeProviderProbing)
      {
        throw new NotSupportedException("Native Provider Probing is disabled by an application switch");
      }

      var creator = _creator.Value;
      if (creator is null)
      {
        throw new NotSupportedException("Native Provider Probing failed to resolve creator");
      }

      return creator.CreateProvider();
    }

    /// <summary>
    /// Attempts to create the provider without throwing on failure.
    /// </summary>
    /// <returns>The created provider, or <see langword="null"/> if probing fails.</returns>
    public T TryCreate()
    {
      if (_disabled || AppSwitches.DisableNativeProviderProbing || AppSwitches.DisableNativeProviders)
      {
        return null;
      }

      try
      {
        return _creator.Value?.CreateProvider();
      }
      catch
      {
        // intentionally swallow exceptions here - use the explicit variants if you're interested in why
        return null;
      }
    }
  }
}
