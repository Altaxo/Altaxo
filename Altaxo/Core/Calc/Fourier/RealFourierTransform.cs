#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;

namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Performs real-valued Fourier transformations for a fixed length.
  /// Depending on the configured length, the fastest available transformation method is selected.
  /// Temporary storage required for some algorithms is held by this instance to avoid repeated allocations.
  /// </summary>
  public class RealFourierTransform
  {
    private enum Method { Trivial, Hartley, Pfa235, Chirp };

    private Method _method;
    private int _numberOfData;
    private Pfa235FFT? _pfa235;
    private double[]? _tempArr1N;
    private object? _fftTempStorage;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealFourierTransform"/> class for the specified length.
    /// </summary>
    /// <param name="length">The length (number of data points) to be transformed. Must be greater than zero.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="length"/> is less than 1.</exception>
    public RealFourierTransform(int length)
    {
      _numberOfData = length;

      if (length < 1)
        throw new ArgumentException("length smaller than 1 is not appropriate here.");
      else if (length < 3)
      {
        _method = Method.Trivial;
      }
      else if (Calc.BinaryMath.IsPowerOfTwoOrZero(length))
      {
        // use Hartley transform
        _method = Method.Hartley;
      }
      else if (Pfa235FFT.CanFactorized(length))
      {
        // use Pfa235 transform
        _method = Method.Pfa235;
        _pfa235 = new Pfa235FFT(_numberOfData);
      }
      else
      {
        // use chirp transform
        _method = Method.Chirp;
      }
    }

    /// <summary>
    /// Performs an out-of-place Fourier transformation. The original input array is preserved.
    /// </summary>
    /// <param name="inputarr">The data to transform.</param>
    /// <param name="direction">Specify forward or reverse transformation here.</param>
    /// <param name="outputarr">On output, contains the Fourier transformed data.</param>
    /// <exception cref="ArgumentException">Thrown when the input or output array lengths do not match the configured length.</exception>
    public void Transform(double[] inputarr, FourierDirection direction, double[] outputarr)
    {
      if (inputarr.Length != _numberOfData)
        throw new ArgumentException(string.Format("Length of array inputarr ({0}) is different from the length specified at construction ({1})", inputarr.Length, _numberOfData), "inputarr");
      if (outputarr.Length != _numberOfData)
        throw new ArgumentException(string.Format("Length of array outputarr ({0}) is different from the length specified at construction ({1})", outputarr.Length, _numberOfData), "outputarr");

      Array.Copy(inputarr, 0, outputarr, 0, inputarr.Length);
      Transform(outputarr, direction);
    }

    /// <summary>
    /// Performs an in-place Fourier transformation. The original values in <paramref name="arr"/> are overwritten by the transformed values.
    /// </summary>
    /// <param name="arr">The data to transform. On output, the Fourier transformed data.</param>
    /// <param name="direction">Specify forward or reverse transformation here.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="arr"/> length does not match the configured length.</exception>
    public void Transform(double[] arr, FourierDirection direction)
    {
      if (arr.Length != _numberOfData)
        throw new ArgumentException(string.Format("Length of array arr ({0}) is different from the length specified at construction ({1})", arr.Length, _numberOfData), "arr");

      switch (_method)
      {
        case Method.Trivial:
          {
            if (_numberOfData == 2)
            {
              double a0 = arr[0], a1 = arr[1];
              arr[0] = a0 + a1;
              arr[1] = a0 - a1;
            }
          }
          break;

        case Method.Hartley:
          {
            FastHartleyTransform.RealFFT(arr, direction);
          }
          break;

        case Method.Pfa235:
          {
            var tempArr1N = NullifyTempArrN1();
            _pfa235!.RealFFT(arr, tempArr1N, direction);
          }
          break;

        case Method.Chirp:
          {
            double[] tempArr1N;
            if (direction == FourierDirection.Forward)
            {
              tempArr1N = NullifyTempArrN1();
            }
            else
            {
              if (_tempArr1N is null)
                _tempArr1N = new double[_numberOfData];

              _tempArr1N[0] = 0;
              for (int k = 1; k <= _numberOfData / 2; k++)
              {
                double sumreal = arr[k];
                double sumimag = arr[_numberOfData - k];

                _tempArr1N[k] = sumimag;
                _tempArr1N[_numberOfData - k] = -sumimag;
                arr[_numberOfData - k] = sumreal;
              }
              tempArr1N = _tempArr1N;
            }

            ChirpFFT.FFT(arr, tempArr1N, direction, ref _fftTempStorage);

            if (direction == FourierDirection.Forward)
            {
              for (int k = 0; k <= _numberOfData / 2; k++)
              {
                double sumreal = arr[k];
                double sumimag = tempArr1N[k];

                if (k != 0 && (k + k) != _numberOfData)
                  arr[_numberOfData - k] = sumimag;
                arr[k] = sumreal;
              }
            }
          }
          break;
      }
    }

    #region Helper

    private double[] NullifyTempArrN1()
    {
      if (_tempArr1N is null)
      {
        _tempArr1N = new double[_numberOfData];
      }
      else
      {
        Array.Clear(_tempArr1N, 0, _tempArr1N.Length);
      }
      return _tempArr1N;
    }

    #endregion Helper
  }
}
