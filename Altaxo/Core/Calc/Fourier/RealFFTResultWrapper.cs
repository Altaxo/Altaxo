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

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Altaxo.Calc.Fourier
{
	/// <summary>
	/// Class to provide convenient access to the result of a real valued fourier transform. See remarks for learning about different representations of a real valued Fourier transform.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is a wrapper for different representations of the result of a real valued fourier transformation.
	/// </para>
	/// <para>1. Real value representation</para>
	/// <para>----------------------------</para>
	/// <para>
	/// The first value of the array is the real part of the spectrum at zero frequency, followed by the real part of the spectrum at first frequency and so on.
	/// The last value of the array is the imaginary part of the spectrum at first frequency, the value before this is the imaginary part of the spectrum at second frequency and so on.
	/// If the length of the fourier transform is even, the value of the array at index of half the length (N/2) is the real part of the spectrum at nyquist frequency. If the length of the fourier
	/// transform is odd, there is no value of the spectrum at nyquist frequency.
	/// </para>
	/// <para></para>
	/// <para>2. Verbose complex representation</para>
	/// <para>---------------------------------</para>
	/// <para>
	/// The real and imaginary part of the spectrum are properly ordered from zero frequency to nyquist frequency.
	/// The element at zero frequency contains only the real part, the imaginary part of this element is zero.
	/// The element at nyquist frequency contains only the real part at nyquist frequency, the imaginary part of this element is zero.
	/// Note that if the length of the FFT is for instance 1024, there are 513 complex spectral values. If the length of the FFT is 1023 (odd), there are 512 complex spectral values.
	/// </para>
	/// <para>3. Compact complex representation</para>
	/// <para>---------------------------------</para>
	/// <para>
	/// The real and imaginary part of the spectrum are properly ordered from zero frequency to one element below the nyquist frequency. The value at nyquist frequency
	/// is put into the imaginary part of the spectrum at zero index. If the length of the FFT is odd, there is no value at nyquist frequency, and hence the imaginary part
	/// at zero index is set to zero.
	/// If the length of the FFT is for instance 1024, there are 512 complex spectral values. If the length of the FFT is 1023 (odd), there are still 512 complex spectral values, but the imaginary part of the first value is set to zero.
	/// </para>
	/// </remarks>
	public class RealFFTResultWrapper
	{
		private double[] _fftresult;
		private int _count;

		/// <summary>
		/// Constructur. You must provide the array with the result of a real valued fourier transformation.
		/// </summary>
		/// <param name="fftresult">The result of a real valued fourier transformation.</param>
		public RealFFTResultWrapper(double[] fftresult)
		{
			_fftresult = fftresult;
			_count = fftresult.Length % 2 == 0 ? fftresult.Length / 2 + 1 : (fftresult.Length + 1) / 2;
		}

		#region Count

		/// <summary>
		/// Return the length of the wrapper vectors Amplitude, RealPart, ImaginaryPart and Phase
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		#endregion Count

		#region Amplitude

		private AmplitudeWrapper _amplitudeWrapper;

		/// <summary>
		/// Returns the vector of amplitudes, i.e. the modulus of the complex result.
		/// </summary>
		public IROVector<double> Amplitude
		{
			get
			{
				if (null == _amplitudeWrapper)
					_amplitudeWrapper = new AmplitudeWrapper(_fftresult);
				return _amplitudeWrapper;
			}
		}

		private class AmplitudeWrapper : IROVector<double>
		{
			private double[] _arr;
			private int _wlen;

			public AmplitudeWrapper(double[] arr)
			{
				_arr = arr;
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			public int Count
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

			#region INumericSequence Members

			public double this[int i]
			{
				get
				{
					if (i == 0)
						return Math.Abs(_arr[0]);
					else if ((i + i) == _arr.Length)
						return Math.Abs(_arr[_arr.Length / 2]);
					else
						return RMath.Hypot(_arr[i], _arr[_arr.Length - i]);
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			#endregion INumericSequence Members
		}

		#endregion Amplitude

		#region Real part

		private RealPartWrapper _realPartWrapper;

		/// <summary>
		/// Returns the vector of the resulting real parts of the FFT.
		/// </summary>
		public IROVector<double> RealPart
		{
			get
			{
				if (null == _realPartWrapper)
					_realPartWrapper = new RealPartWrapper(_fftresult);
				return _realPartWrapper;
			}
		}

		private class RealPartWrapper : IROVector<double>
		{
			private double[] _arr;
			private int _wlen;

			public RealPartWrapper(double[] arr)
			{
				_arr = arr;
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			public int Count
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

			#region INumericSequence Members

			public double this[int i]
			{
				get
				{
					if (i == 0)
						return _arr[0];
					else if ((i + i) == _arr.Length)
						return _arr[_arr.Length / 2];
					else
						return _arr[i];
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			#endregion INumericSequence Members
		}

		#endregion Real part

		#region Imaginary part

		private ImaginaryPartWrapper _imagPartWrapper;

		/// <summary>
		/// Returns the vector of the resulting imaginary parts of the FFT.
		/// </summary>
		public IROVector<double> ImaginaryPart
		{
			get
			{
				if (null == _imagPartWrapper)
					_imagPartWrapper = new ImaginaryPartWrapper(_fftresult);
				return _imagPartWrapper;
			}
		}

		private class ImaginaryPartWrapper : IROVector<double>
		{
			private double[] _arr;
			private int _wlen;

			public ImaginaryPartWrapper(double[] arr)
			{
				_arr = arr;
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			public int Count
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

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
						return _arr[_arr.Length - i];
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			#endregion INumericSequence Members
		}

		#endregion Imaginary part

		#region Complex Re-Im

		private ComplexPartWrapper _complexPartWrapper;

		/// <summary>
		/// Returns the vector of the complex result of the FFT.
		/// </summary>
		public IROComplexDoubleVector ComplexResult
		{
			get
			{
				if (null == _complexPartWrapper)
					_complexPartWrapper = new ComplexPartWrapper(_fftresult);
				return _complexPartWrapper;
			}
		}

		private class ComplexPartWrapper : IROComplexDoubleVector
		{
			private double[] _arr;
			private int _wlen;

			public ComplexPartWrapper(double[] arr)
			{
				_arr = arr;
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

			#region INumericSequence Members

			public Complex this[int i]
			{
				get
				{
					if (i == 0)
						return Complex.FromRealImaginary(_arr[0], 0);
					else if ((i + i) == _arr.Length)
						return Complex.FromRealImaginary(_arr[_arr.Length / 2], 0);
					else
						return Complex.FromRealImaginary(_arr[i], _arr[_arr.Length - i]);
				}
			}

			#endregion INumericSequence Members
		}

		#endregion Complex Re-Im

		#region Phase

		private PhaseWrapper _phaseWrapper;

		/// <summary>
		/// Returns the vector of the resulting phases of the FFT.
		/// </summary>
		public IROVector<double> Phase
		{
			get
			{
				if (null == _phaseWrapper)
					_phaseWrapper = new PhaseWrapper(_fftresult);
				return _phaseWrapper;
			}
		}

		private class PhaseWrapper : IROVector<double>
		{
			private double[] _arr;
			private int _wlen;

			public PhaseWrapper(double[] arr)
			{
				_arr = arr;
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			public int Count
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

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
						return Math.Atan2(_arr[_arr.Length - i], _arr[i]);
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			#endregion INumericSequence Members
		}

		#endregion Phase

		#region Frequency

		/// <summary>
		/// Given a value for the xincrement (x interval between two points before the Fourier transformation), the vector of frequencies
		/// is returned.
		/// </summary>
		/// <param name="xincrement">X interval between two points before the Fourier transformation (sample period).</param>
		/// <returns>The vector of frequencies that correspond to the vectors Amplitude, RealPart, ImaginaryPart and Phase.</returns>
		public IROVector<double> FrequenciesFromXIncrement(double xincrement)
		{
			return new FrequencyWrapper(_fftresult, 1 / xincrement);
		}

		/// <summary>
		/// Given a value for the xrate (inverse of x interval between two points before the Fourier transformation), the vector of frequencies
		/// is returned.
		/// </summary>
		/// <param name="xrate">Inverse of the x interval between two points before the Fourier transformation (sample rate).</param>
		/// <returns>The vector of frequencies that correspond to the vectors Amplitude, RealPart, ImaginaryPart and Phase.</returns>
		public IROVector<double> FrequenciesFromXRate(double xrate)
		{
			return new FrequencyWrapper(_fftresult, xrate);
		}

		private class FrequencyWrapper : IROVector<double>
		{
			private int _wlen;
			private double _frequencyIncrement;

			public FrequencyWrapper(double[] arr, double samplerate)
			{
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
				_frequencyIncrement = samplerate / arr.Length;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			public int Count
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

			#region INumericSequence Members

			public double this[int i]
			{
				get
				{
					return i * _frequencyIncrement;
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			#endregion INumericSequence Members
		}

		#endregion Frequency

		#region CircularFrequency

		/// <summary>
		/// Given a value for the xincrement (x interval between two points before the Fourier transformation), the vector of circular frequencies
		/// is returned.
		/// </summary>
		/// <param name="xincrement">X interval between two points before the Fourier transformation (sample period).</param>
		/// <returns>The vector of circular frequencies that correspond to the vectors Amplitude, RealPart, ImaginaryPart and Phase.</returns>
		public IROVector<double> CircularFrequenciesFromXIncrement(double xincrement)
		{
			return new CircularFrequencyWrapper(_fftresult, 1 / xincrement);
		}

		/// <summary>
		/// Given a value for the xrate (inverse of x interval between two points before the Fourier transformation), the vector of circular frequencies
		/// is returned.
		/// </summary>
		/// <param name="xrate">Inverse of the x interval between two points before the Fourier transformation (sample rate).</param>
		/// <returns>The vector of circular frequencies that correspond to the vectors Amplitude, RealPart, ImaginaryPart and Phase.</returns>
		public IROVector<double> CircularFrequenciesFromXRate(double xrate)
		{
			return new CircularFrequencyWrapper(_fftresult, xrate);
		}

		private class CircularFrequencyWrapper : IROVector<double>
		{
			private int _wlen;
			private double _omegaIncrement;

			public CircularFrequencyWrapper(double[] arr, double samplerate)
			{
				_wlen = arr.Length % 2 == 0 ? arr.Length / 2 + 1 : (arr.Length + 1) / 2;
				_omegaIncrement = 2 * Math.PI * samplerate / arr.Length;
			}

			#region IROVector Members

			public int Length
			{
				get { return _wlen; }
			}

			public int Count
			{
				get { return _wlen; }
			}

			#endregion IROVector Members

			#region INumericSequence Members

			public double this[int i]
			{
				get
				{
					return i * _omegaIncrement;
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < _wlen; ++i)
					yield return this[i];
			}

			#endregion INumericSequence Members
		}

		#endregion CircularFrequency

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

		#endregion Division of two spectra

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

		#endregion Addition of two spectra

		#region Static functions

		/// <summary>
		/// Transforms from the real representation of a spectrum to the compact complex representation (nyquist frequency value put in imaginary part of first element).
		/// </summary>
		/// <param name="src">Real representation of the spectrum.</param>
		/// <param name="destRe">On return, contains the real part of the spectrum.</param>
		/// <param name="destIm">On return, contains the imaginary part of the spectrum.</param>
		public static void FromRepresentationRealToCompactComplex(IReadOnlyList<double> src, IVector<double> destRe, IVector<double> destIm)
		{
			bool isEven = 0 == (src.Count % 2);
			int destLen2;
			if (isEven)
			{
				destLen2 = src.Count / 2;
				destRe[0] = src[0];
				destIm[0] = src[destLen2];
			}
			else // odd
			{
				destLen2 = (src.Count - 1) / 2;
				destRe[0] = src[0];
				destIm[0] = 0;
			}
			for (int i = 1, j = src.Count - 1; i < j; i++, j--)
			{
				destRe[i] = src[i];
				destIm[i] = src[j];
			}
		}

		/// <summary>
		/// Transforms from the real representation of a spectrum to the compact complex representation (nyquist frequency value put in imaginary part of first element).
		/// </summary>
		/// <param name="src">Real representation of the spectrum.</param>
		/// <param name="dest">On return, contains the complex spectrum.</param>
		public static void FromRepresentationRealToCompactComplex(IReadOnlyList<double> src, IComplexDoubleVector dest)
		{
			bool isEven = 0 == (src.Count % 2);
			int destLen2;
			if (isEven)
			{
				destLen2 = src.Count / 2;
				dest[0] = Complex.FromRealImaginary(src[0], src[destLen2]);
			}
			else // odd
			{
				destLen2 = (src.Count - 1) / 2;
				dest[0] = Complex.FromRealImaginary(src[0], 0);
			}
			for (int i = 1, j = src.Count - 1; i < j; i++, j--)
			{
				dest[i] = Complex.FromRealImaginary(src[i], src[j]);
			}
		}

		/// <summary>
		/// Transforms from a compact complex representation (nyquist frequency value put in imaginary part of first element) to real representation.
		/// </summary>
		/// <param name="re">Stores the real part of the spectrum.</param>
		/// <param name="im">Stores the imaginary part of the spectrum.</param>
		/// <param name="destination">After return, stores the spectrum in normalized real representation. The length of the vector has to be equal to the length of the FFT. </param>
		public static void FromRepresentationCompactComplexToReal(IReadOnlyList<double> re, IReadOnlyList<double> im, IVector<double> destination)
		{
			bool isEven = 0 == (destination.Length % 2);
			int destLen2;
			if (isEven)
			{
				destLen2 = destination.Length / 2;
				destination[0] = re[0];
				destination[destLen2] = im[0];
			}
			else // odd
			{
				destLen2 = (destination.Length - 1) / 2;
				destination[0] = re[0];
			}
			for (int i = 1, j = destination.Length - 1; i < j; i++, j--)
			{
				destination[i] = re[i];
				destination[j] = im[i];
			}
		}

		/// <summary>
		/// Transforms from a compact complex representation (nyquist frequency value put in imaginary part of first element) to real representation.
		/// </summary>
		/// <param name="src">Stores the  complex spectrum.</param>
		/// <param name="destination">After return, stores the spectrum in normalized real representation. The length of the vector has to be equal to the length of the FFT. </param>
		public static void FromRepresentationCompactComplexToReal(IROComplexDoubleVector src, IVector<double> destination)
		{
			bool isEven = 0 == (destination.Length % 2);
			int destLen2;
			if (isEven)
			{
				destLen2 = destination.Length / 2;
				destination[0] = src[0].Re;
				destination[destLen2] = src[0].Im; ;
			}
			else // odd
			{
				destLen2 = (destination.Length - 1) / 2;
				destination[0] = src[0].Re;
			}
			for (int i = 1, j = destination.Length - 1; i < j; i++, j--)
			{
				destination[i] = src[i].Re;
				destination[j] = src[i].Im;
			}
		}

		#endregion Static functions
	}
}