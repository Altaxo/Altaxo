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

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;

namespace Altaxo.Graph.Gdi.HatchBrushes
{
	public class CrossHatchBrush : HatchBrushBase
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossHatchBrush), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CrossHatchBrush)obj;
				info.AddBaseValueEmbedded(s, typeof(CrossHatchBrush).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (CrossHatchBrush)o : new CrossHatchBrush();
				info.GetBaseValueEmbedded(s, typeof(CrossHatchBrush).BaseType, parent);
				return s;
			}
		}


		#endregion

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

				using (Pen pen = new Pen(foreColor, (float)(pixelDim * _structureFactor )))
				{
					pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
					g.DrawLine(pen, 0.5f*bmp.Width, 2*bmp.Height, 0.5f*bmp.Width, -2*bmp.Height);
					g.DrawLine(pen, -2 * bmp.Width, 0.5f * bmp.Height, 2 * bmp.Width, 0.5f * bmp.Height);
				}
			}

			return bmp;
		}

		public override object Clone()
		{
			var result = new CrossHatchBrush();
			result.CopyFrom(this);
			return result;
		}
	}

}
