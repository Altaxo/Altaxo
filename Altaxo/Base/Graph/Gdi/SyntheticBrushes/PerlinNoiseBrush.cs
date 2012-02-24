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
#endregion

//#define IncludePerlinNoise

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;

namespace Altaxo.Graph.Gdi.SyntheticBrushes
{
#if IncludePerlinNoise
	public class PerlinNoiseBrush : SyntheticBrushBase
	{
		protected static Random _randomGenerator = new Random();
		/// <summary>
		/// Number of octaves that this brush is using;
		/// </summary>
		int _numberOfOctaves = 3;

		/// <summary>
		/// Factor to calculate the amplitude of an octave from the amplitude of the previous octave.
		/// </summary>
		double _persistenceFactor = 0.5;


		Altaxo.Graph.Gdi.Plot.ColorProvider.ColorProviderARGBGradient _colorProvider = new Altaxo.Graph.Gdi.Plot.ColorProvider.ColorProviderARGBGradient();


		public override System.Drawing.Image GetImage(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor)
		{
			int pixelDim = GetPixelDimensions(maxEffectiveResolutionDpi);
			Bitmap bmp = new Bitmap(pixelDim, pixelDim, PixelFormat.Format32bppArgb);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				using (var brush = new SolidBrush(backColor))
				{
					g.FillRectangle(brush, new Rectangle(Point.Empty, bmp.Size));
				}

				g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy; // we want the foreground color to be not influenced by the background color if we have a transparent foreground color

				/*
				for (int i = pixelDim * pixelDim; i > 0; i -= 10)
				{
					int x = _randomGenerator.Next(pixelDim - 1);
					int y = _randomGenerator.Next(pixelDim - 1);
					bmp.SetPixel(x, y, foreColor);
				}
				*/

				_colorProvider.ColorAtR0 = backColor;
				_colorProvider.ColorAtR1 = foreColor;

				double scaling = 1/MaxAmplitude;
				for (int i = 0; i < pixelDim; ++i)
				{
					double x = i / (double)pixelDim;
					for (int j = 0; j < pixelDim; ++j)
					{
						double y = j / (double)pixelDim;
						double t = scaling * PerlinNoise_2D(x*4,y*4) + scaling;
						bmp.SetPixel(i, j, _colorProvider.GetColor(t));
					}
				}

			}

			return bmp;
		}

		public override object Clone()
		{
			var result = new PerlinNoiseBrush();
			result.CopyFrom(this);
			return result;
		}


		double Noise(int x, int y)
		{
    int n = x + y * 57;
    n = (n<<13) ^ n;
    return ( 1.0 - ( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);    
		}

		double SmoothedNoise(int x, int y)
		{
    var corners = ( Noise(x-1, y-1)+Noise(x+1, y-1)+Noise(x-1, y+1)+Noise(x+1, y+1) ) / 16;
    var sides   = ( Noise(x-1, y)  +Noise(x+1, y)  +Noise(x, y-1)  +Noise(x, y+1) ) /  8;
    var center  =  Noise(x, y) / 4;
		return corners + sides + center;
		}
  

		double Interpolate(double a, double b, double x)
		{
			var ft = x * Math.PI;
	var f = (1 - Math.Cos(ft)) * 0.5;

	return  a*(1-f) + b*f;
		}

   double InterpolatedNoise(double x, double y)
	 {

      var integer_X    = (int)Math.Floor(x);
      var fractional_X = x - integer_X;

      var integer_Y    = (int)Math.Floor(y);
      var fractional_Y = y - integer_Y;

      var v1 = SmoothedNoise(integer_X,     integer_Y);
      var v2 = SmoothedNoise(integer_X + 1, integer_Y);
      var v3 = SmoothedNoise(integer_X,     integer_Y + 1);
      var v4 = SmoothedNoise(integer_X + 1, integer_Y + 1);

      var i1 = Interpolate(v1 , v2 , fractional_X);
      var i2 = Interpolate(v3 , v4 , fractional_X);

			return Interpolate(i1, i2, fractional_Y);
	 }
  
		double PerlinNoise_2D(double x, double y)
		{
      double total = 0;
      var p = _persistenceFactor;
      var n = _numberOfOctaves -1;
			
			double amplitude=1;
			if (_persistenceFactor <= 1)
			{
				int frequency = 1;
				for (int i = 0; i < _numberOfOctaves; ++i, frequency += frequency, amplitude *= _persistenceFactor)
					total = total + InterpolatedNoise(x * frequency, y * frequency) * amplitude;
			}
			else
			{
				int frequency = 1 << (_numberOfOctaves - 1);
				for (int i = _numberOfOctaves-1; i >=0; --i, frequency >>=1, amplitude /= _persistenceFactor)
					total = total + InterpolatedNoise(x * frequency, y * frequency) * amplitude;
			}
      return total;
		}

		double MaxAmplitude
		{
			get
			{
				double total = 0;
				double amplitude = 1;

				if (_persistenceFactor <= 1)
				{
					for (int i = 0; i < _numberOfOctaves; ++i, amplitude *= _persistenceFactor)
						total += amplitude;
				}
				else
				{
					for (int i = _numberOfOctaves - 1; i >= 0; --i, amplitude /= _persistenceFactor) 
						total += amplitude;
				}
				return total;
			}
		}
	}
#endif
}
