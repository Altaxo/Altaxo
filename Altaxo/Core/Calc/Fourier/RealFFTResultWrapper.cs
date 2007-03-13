#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Class to provide convenient access to the result of a real valued fourier transform.
  /// </summary>
  public class RealFFTResultWrapper
  {
    double[] _fftresult;

    /// <summary>
    /// Constructur. You must provide the array with the result of a real valued fourier transformation.
    /// </summary>
    /// <param name="fftresult">The result of a real valued fourier transformation.</param>
    public RealFFTResultWrapper(double[] fftresult)
    {
      _fftresult = fftresult;
    }

    #region Amplitude
    AmplitudeWrapper _amplitudeWrapper;
    public IROVector Amplitude
    {
      get
      {
        if (null == _amplitudeWrapper)
          _amplitudeWrapper = new AmplitudeWrapper(_fftresult);
        return _amplitudeWrapper;
      }
    }
    class AmplitudeWrapper : IROVector
    {
      double[] _arr;
      int _wlen;

      public AmplitudeWrapper(double[] arr)
      {
        _arr = arr;
        _wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
      }

      #region IROVector Members

      public int LowerBound
      {
        get { return 0; }
      }

      public int UpperBound
      {
        get { return _wlen - 1; }
      }

      public int Length
      {
        get { return _wlen; }
      }

      #endregion

      #region INumericSequence Members

      public double this[int i]
      {
        get 
        {
          if (i == 0)
            return 0.5*Math.Abs(_arr[0]);
          else if ((i + i) == _arr.Length)
            return 0.5*Math.Abs(_arr[_arr.Length / 2]);
          else
            return RMath.Hypot(_arr[i], _arr[_arr.Length - i]);
        }
      }

      #endregion
    }
    #endregion

    #region Real part
    RealPartWrapper _realPartWrapper;
    public IROVector RealPart
    {
      get
      {
        if (null == _realPartWrapper)
          _realPartWrapper = new RealPartWrapper(_fftresult);
        return _realPartWrapper;
      }
    }
    class RealPartWrapper : IROVector
    {
      double[] _arr;
      int _wlen;

      public RealPartWrapper(double[] arr)
      {
        _arr = arr;
        _wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
      }

      #region IROVector Members

      public int LowerBound
      {
        get { return 0; }
      }

      public int UpperBound
      {
        get { return _wlen - 1; }
      }

      public int Length
      {
        get { return _wlen; }
      }

      #endregion

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          if (i == 0)
            return 0.5 * _arr[0];
          else if ((i + i) == _arr.Length)
            return 0.5 * _arr[_arr.Length / 2];
          else
            return _arr[i];
        }
      }

      #endregion
    }
    #endregion

    #region Imaginary part
    ImaginaryPartWrapper _imagPartWrapper;
    public IROVector ImaginaryPart
    {
      get
      {
        if (null == _imagPartWrapper)
          _imagPartWrapper = new ImaginaryPartWrapper(_fftresult);
        return _imagPartWrapper;
      }
    }
    class ImaginaryPartWrapper : IROVector
    {
      double[] _arr;
      int _wlen;

      public ImaginaryPartWrapper(double[] arr)
      {
        _arr = arr;
        _wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
      }

      #region IROVector Members

      public int LowerBound
      {
        get { return 0; }
      }

      public int UpperBound
      {
        get { return _wlen - 1; }
      }

      public int Length
      {
        get { return _wlen; }
      }

      #endregion

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          if (i == 0)
            return 0;
          else if ((i + i) == _arr.Length)
            return 0;
          else
            return -_arr[_arr.Length-i];
        }
      }

      #endregion
    }
    #endregion

    #region Phase
    PhaseWrapper _phaseWrapper;
    public IROVector Phase
    {
      get
      {
        if (null == _phaseWrapper)
          _phaseWrapper = new PhaseWrapper(_fftresult);
        return _phaseWrapper;
      }
    }
    class PhaseWrapper : IROVector
    {
      double[] _arr;
      int _wlen;

      public PhaseWrapper(double[] arr)
      {
        _arr = arr;
        _wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
      }

      #region IROVector Members

      public int LowerBound
      {
        get { return 0; }
      }

      public int UpperBound
      {
        get { return _wlen - 1; }
      }

      public int Length
      {
        get { return _wlen; }
      }

      #endregion

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          if (i == 0)
            return _arr[0] >= 0 ? 0 : Math.PI;
          else if ((i + i) == _arr.Length)
            return _arr[_arr.Length / 2] >= 0 ? 0 : Math.PI;
          else
            return Math.Atan2(_arr[_arr.Length - i] , _arr[i]);
        }
      }

      #endregion
    }
    #endregion

    #region Frequency
    public IROVector FrequenciesFromTimeIncrement(double timeincrement)
    {
      return new FrequencyWrapper(_fftresult,1/timeincrement);
    }
    public IROVector FrequenciesFromSampleRate(double samplerate)
    {
      return new FrequencyWrapper(_fftresult, samplerate);
    }
    class FrequencyWrapper : IROVector
    {
      int _wlen;
      double _frequencyIncrement;

      public FrequencyWrapper(double[] arr, double samplerate)
      {
         _wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
         _frequencyIncrement = samplerate / arr.Length;
      }

      #region IROVector Members

      public int LowerBound
      {
        get { return 0; }
      }

      public int UpperBound
      {
        get { return _wlen - 1; }
      }

      public int Length
      {
        get { return _wlen; }
      }

      #endregion

      #region INumericSequence Members

      public double this[int i]
      {
        get
        {
          return i * _frequencyIncrement;
        }
      }

      #endregion
    }
    #endregion

    #region Division of two spectra

    /// <summary>
    /// Divides two spectra that where created as the result of two real fourier transformations.
    /// </summary>
    /// <param name="nominator">The spectrum acting as nominator of the division.</param>
    /// <param name="denominator">The denominator spectrum.</param>
    /// <param name="result">The resulting divided spectrum. See remarks on how the data are organized in the array.</param>
    /// <remarks>
    /// The data in the nominator and denominator spectral array and in the resulting array are organized as follows.
    /// a[0] is the real part of f=0 (for f=0 the imaginary part is always zero)
    /// a[i] and a[length-i] are the real and imaginary part of the spectrum at frequency i, respectively.
    /// If the length of a is even, then a[length/2] is the real part of the frequency length/2 (the imaginary part at this frequency is always zero).
    /// </remarks>
    public static void DivideSpectra(double[] nominator, double[] denominator, double[] result)
    {
      if (nominator.Length != denominator.Length)
        throw new ArgumentException("Nominator length not equal to denominator length");
      if (nominator.Length != result.Length)
        throw new ArgumentException("Result length not equal to nominator/denominator length");

      int len = nominator.Length;
      for (int i = 1, j = len - 1; i < j; ++i, --j)
      {
        Complex r = Complex.FromRealImaginary(nominator[i], nominator[j]) / Complex.FromRealImaginary(denominator[i], denominator[j]);
        result[i] = r.Re;
        result[j] = r.Im;
      }

      result[0] = nominator[0] / denominator[0];

      if (len % 2 == 0)
      {
        int len2 = len / 2;
        result[len2] = nominator[len2] / denominator[len2];
      }
    }



    #endregion

    #region Addition of two spectra
    /// <summary>
    /// Addition of two spectra that where created as the result of two real fourier transformations.
    /// </summary>
    /// <param name="spectrumA">The first spectrum to add.</param>
    /// <param name="spectrumB">The second spectrum to add.</param>
    /// <param name="result">The resulting of the addition of the two spectras. May be identical to one of the parameters <c>spectraA</c> or <c>spectraB</c>. See remarks on how the data are organized in the array.</param>
    /// <remarks>
    /// The data in the nominator and denominator spectral array and in the resulting array are organized as follows.
    /// a[0] is the real part of f=0 (for f=0 the imaginary part is always zero)
    /// a[i] and a[length-i] are the real and imaginary part of the spectrum at frequency i, respectively.
    /// If the length of a is even, then a[length/2] is the real part of the frequency length/2 (the imaginary part at this frequency is always zero).
    /// </remarks>
    public static void AddSpectra(double[] spectrumA, double[] spectrumB, double[] result)
    {
      if (spectrumA.Length != spectrumB.Length)
        throw new ArgumentException("Length of first spectrum not equal to length of second spectrum");
      if (spectrumA.Length != result.Length)
        throw new ArgumentException("Resulting spectrum length not equal to first/second spectrum length");

      int len = spectrumA.Length;
      for (int i = 0; i < len; i++)
        result[i] = spectrumA[i] + spectrumB[i];
    }
    #endregion
  }
}
