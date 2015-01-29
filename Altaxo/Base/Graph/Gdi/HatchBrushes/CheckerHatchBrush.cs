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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Altaxo.Graph.Gdi.HatchBrushes
{
	public class CheckerHatchBrush : HatchBrushBase
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CheckerHatchBrush), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CheckerHatchBrush)obj;
				info.AddBaseValueEmbedded(s, typeof(CheckerHatchBrush).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (CheckerHatchBrush)o : new CheckerHatchBrush();
				info.GetBaseValueEmbedded(s, typeof(CheckerHatchBrush).BaseType, parent);
				return s;
			}
		}

		#endregion Serialization

		public override Image GetImage(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor)
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

				using (var brush = new SolidBrush(foreColor))
				{
					float s = (float)(pixelDim * _structureFactor);
					float o = (float)(pixelDim * (0.25 - 0.5 * _structureFactor));
					g.FillRectangle(brush, o, o, s, s);
					g.FillRectangle(brush, o + 0.5f * pixelDim, o + 0.5f * pixelDim, s, s);
				}
			}

			return bmp;
		}

		public override object Clone()
		{
			var result = new CheckerHatchBrush();
			result.CopyFrom(this);
			return result;
		}
	}
}