using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{

	/// <summary>
	/// This color provider provides the colors of the visible light spectrum in the wavelength
	/// range between 350 nm and 780 nm.
	/// </summary>
	public class VisibleLightSpectrum : ColorProviderBase
	{
		/// <summary>Minimum wavelength of the light in nm which can be shown as color.</summary>
		public static readonly int MinVisibleWavelength_nm = 350;
		/// <summary>Maximum wavelength of the light in nm which can be shown as color.</summary>
		public static readonly int MaxVisibleWavelength_nm = 780;

		/// <summary>Default maximum intensity (brightness) of the colors delivered.</summary>
		public static readonly int DefaultIntensityMaximum = 255;

		/// <summary>Default gamma value.</summary>
		public static readonly double DefaultGamma = 1;

		/// <summary>Maximum value of intensity (0..255).</summary>
		private int _intensityMaximum;

		/// <summary>Gamma value for colorization.</summary>
		private double _gamma = 1;

		/// <summary>
		/// Default constructor. The maximum intensity and the gamma value are set to their default values.
		/// </summary>
		public VisibleLightSpectrum()
		{
			_intensityMaximum = DefaultIntensityMaximum;
			_gamma = DefaultGamma;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">Other instance to copy the data from.</param>
		public VisibleLightSpectrum(VisibleLightSpectrum from)
		{
			CopyFrom(from);
		}

		/// <summary>
		/// Copies all data from another instance to this instance.
		/// </summary>
		/// <param name="o">Object to copy from.</param>
		/// <returns>True if all or some of the data of the other instance could be copied. False if no data could be copied.</returns>
		public override bool CopyFrom(object o)
		{
			var result = base.CopyFrom(o);
			var from = o as VisibleLightSpectrum;
			if (null != from)
			{
				this._intensityMaximum = from._intensityMaximum;
				this._gamma = from._gamma;
				result = true;
			}
			return result;
		}


		/// <summary>
		/// Gets the color in dependence of the wavelength with <see cref="DefaultGamma"/> value, <see cref="DefaultIntensityMaximum"/> and no transparency.
		/// </summary>
		/// <param name="Wavelength">Wavelength in nm (ranging from 350 to 780).</param>
		/// <returns>Color in dependence of the provided wavelength value.</returns>
		public static Color GetColorFromWaveLength(double Wavelength)
		{
			return GetColorFromWaveLength(Wavelength, DefaultGamma, DefaultIntensityMaximum, 255);
		}

		/// <summary>
		/// Gets the color in dependence of the wavelength, gamma, intensity max and transparency.
		/// </summary>
		/// <param name="Wavelength">Wavelength in nm (ranging from 350 to 780).</param>
		/// <param name="Gamma">Gamma value (positive).</param>
		/// <param name="IntensityMax">Maximum brightness (0..255).</param>
		/// <param name="alphaChannel">Value of the alpha channel (0.255), corresponding to full transparent (0) to full opaque (255).</param>
		/// <returns>The color in dependence of the provided arguments.</returns>
		public static Color GetColorFromWaveLength(double Wavelength, double Gamma, int IntensityMax, int alphaChannel)
		{
			double Blue;
			double Green;
			double Red;
			double Factor;

			if (Wavelength >= 350 && Wavelength < 440)
			{
				Red = -(Wavelength - 440d) / (440d - 350d);
				Green = 0.0;
				Blue = 1.0;
			}
			else if (Wavelength >= 440 && Wavelength < 490)
			{
				Red = 0.0;
				Green = (Wavelength - 440d) / (490d - 440d);
				Blue = 1.0;
			}
			else if (Wavelength >= 490 && Wavelength < 510)
			{
				Red = 0.0;
				Green = 1.0;
				Blue = -(Wavelength - 510d) / (510d - 490d);
			}
			else if (Wavelength >= 510 && Wavelength < 580)
			{
				Red = (Wavelength - 510d) / (580d - 510d);
				Green = 1.0;
				Blue = 0.0;
			}
			else if (Wavelength >= 580 && Wavelength < 645)
			{
				Red = 1.0;
				Green = -(Wavelength - 645d) / (645d - 580d);
				Blue = 0.0;
			}
			else if (Wavelength >= 645 && Wavelength <= 780)
			{
				Red = 1.0;
				Green = 0.0;
				Blue = 0.0;
			}
			else
			{
				Red = 0.0;
				Green = 0.0;
				Blue = 0.0;
			}



			if (Wavelength >= 350 && Wavelength < 420)
			{
				Factor = 0.3 + 0.7 * (Wavelength - 350d) / (420d - 350d);
			}
			else if (Wavelength >= 420 && Wavelength < 700)
			{
				Factor = 1.0;
			}
			else if (Wavelength >= 700 && Wavelength <= 780)
			{
				Factor = 0.3 + 0.7 * (780d - Wavelength) / (780d - 700d);
			}
			else
			{
				Factor = 0.0;
			}

			int R = AdjustFactor(Red, Factor, IntensityMax, Gamma);
			int G = AdjustFactor(Green, Factor, IntensityMax, Gamma);
			int B = AdjustFactor(Blue, Factor, IntensityMax, Gamma);

			return Color.FromArgb(alphaChannel,R, G, B);
		}

		private static int AdjustFactor(double Color,
		 double Factor,
		 int IntensityMax,
		 double Gamma)
		{
			if (Color == 0.0)
			{
				return 0;
			}
			else
			{
				return (int)Math.Round(IntensityMax * Math.Pow(Color * Factor, Gamma));
			}
		}

		/// <summary>
		/// Get a clone of this instance.
		/// </summary>
		/// <returns>Clone of this instance.</returns>
		public override object Clone()
		{
			return new VisibleLightSpectrum(this);
		}


		/// <summary>
		/// Gets the color in dependence of a relative value ranging from 0..1. The instance members
		/// for gamma, intensity maximum and transparency are used to calculate the color.
		/// </summary>
		/// <param name="relVal">Relative value (0..1), which is transformed linearly to the wavelength range of 350..780 nm.</param>
		/// <returns>Color in dependence of the relative value.</returns>
		protected override System.Drawing.Color GetColorFrom0To1Continuously(double relVal)
		{
			return GetColorFromWaveLength(
				MinVisibleWavelength_nm + relVal*(MaxVisibleWavelength_nm-MinVisibleWavelength_nm),
				_gamma,
				_intensityMaximum,
				_alphaChannel);
		}
	}
}
