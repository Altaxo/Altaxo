/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{

	/// <remarks>LabelStyle is the abstract base class of all LabelStyles. It is derived from
	/// LayerEdge, since the formatting mainly depends on which edge of the layer the label is positioned.</remarks>
	public abstract class LabelStyle : LayerEdge
	{
		/// <summary>
		/// Creates the abstract base class instance. You have to provided, for which edge of the layer
		/// this LabelStyle is intended.
		/// </summary>
		/// <param name="st"></param>
		public LabelStyle(EdgeType st)
			: base(st)
		{
		}

		/// <summary>
		/// Abstract paint function for the LabelStyle.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="layer">The layer the lables belongs to.</param>
		/// <param name="axis">The axis for which to paint the (major) labels.</param>
		/// <param name="axisstyle">The axis style the axis is formatted with.</param>
		public abstract void Paint(Graphics g, Layer layer, Axis axis, XYLayerAxisStyle axisstyle);


	}


	/// <summary>
	/// 
	/// </summary>
	/// <remarks>This class paints a simple label based the general numeric format, i.e. 
	/// a fixed decimal point representation for small numeric values and a exponential
	/// form representation using the 'E' as separator between mantissa and exponent.
	/// Some effort has been done to make sure that all labels have the same number of trailing decimal
	/// digits.
	/// </remarks>
	public class SimpleLabelStyle : LabelStyle
	{
		protected Font m_Font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);

		public SimpleLabelStyle(EdgeType st)
		 : base(st)
		{
		}

		public override void Paint(Graphics g, Layer layer, Axis axis, XYLayerAxisStyle axisstyle)
		{
			SizeF layerSize = layer.Size;
			PointF orgP = GetOrg(layerSize);
			PointF endP = GetEnd(layerSize);
			PointF outVector = OuterVector;
			float dist_x = axisstyle.OuterDistance+axisstyle.GetOffset(layerSize); // Distance from axis tick point to label
			float dist_y = axisstyle.OuterDistance+axisstyle.GetOffset(layerSize); // y distance from axis tick point to label

			dist_x += this.m_Font.SizeInPoints/3; // add some space to the horizontal direction in order to separate the chars a little from the ticks

			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = StringFormat.GenericTypographic;
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			// next statement is necessary to have a consistent string length both
			// on 0 degree rotated text and rotated text
			// without this statement, the text is fitted to the pixel grid, which
			// leads to "steps" during scaling
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			// set the alignment and line alignment of the strings
			switch(this.m_StyleType)
			{
				case EdgeType.Bottom:
					strfmt.LineAlignment = StringAlignment.Near;
					strfmt.Alignment     = StringAlignment.Center;
					dist_x = 0;
					break;
				case EdgeType.Top:
					strfmt.LineAlignment = StringAlignment.Far;
					strfmt.Alignment     = StringAlignment.Center;
					dist_x = 0;
					dist_y = -dist_y;
					break;
				case EdgeType.Left:
					strfmt.LineAlignment = StringAlignment.Center;
					strfmt.Alignment     = StringAlignment.Far;
					dist_x = -dist_x;
					dist_y = 0;
					break;
				case EdgeType.Right:
					strfmt.LineAlignment = StringAlignment.Center;
					strfmt.Alignment     = StringAlignment.Near;
					dist_y = 0;
					break;
			}


			// print the major ticks
			double[] majorticks = axis.GetMajorTicks();
			bool[] bExponentialForm = new Boolean[majorticks.Length];
			// determine the number of trailing decimal digits
			string mtick;
			int posdecimalseparator;
			int posexponent;
			int digits;
			int maxtrailingdigits=0;
			int maxexponentialdigits=1;
			System.Globalization.NumberFormatInfo numinfo = System.Globalization.NumberFormatInfo.InvariantInfo;
			for(int i=0;i<majorticks.Length;i++)
			{
				mtick = majorticks[i].ToString(numinfo);
				posdecimalseparator = mtick.LastIndexOf(numinfo.NumberDecimalSeparator);
				if(posdecimalseparator<0) continue;
				posexponent = mtick.LastIndexOf('E');
				if(posexponent<0) // no exponent-> count the trailing decimal digits
				{
					bExponentialForm[i]=false;
					digits = mtick.Length-posdecimalseparator-1;
					if(digits>maxtrailingdigits)
						maxtrailingdigits = digits;
				}
				else // the exponential form is used
				{
					bExponentialForm[i]=true;
					// the total digits used for exponential form are the characters until the 'E' of the exponent
					// minus the decimal separator minus the minus sign
					digits = mtick[0]=='-' ? posexponent-2 : posexponent-1; // the digits
					if(digits>maxexponentialdigits)
						maxexponentialdigits=digits;
				}
			}


			// now format the lables
			string exponentialformat=string.Format("G{0}",maxexponentialdigits);
			string fixedformat = string.Format("F{0}",maxtrailingdigits);
			for(int i=0;i<majorticks.Length;i++)
			{
				double r = axis.PhysicalToNormal(majorticks[i]);
				PointF tickorg = GetEdgePoint(layerSize,r);

				if(bExponentialForm[i])
					mtick = majorticks[i].ToString(exponentialformat);
				else
					mtick = majorticks[i].ToString(fixedformat);

				g.DrawString(mtick, m_Font, Brushes.Black, tickorg.X + dist_x, tickorg.Y + dist_y, strfmt);
			
			}

		}

	}
}
