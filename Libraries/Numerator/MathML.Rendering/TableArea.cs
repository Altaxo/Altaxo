//This file is part of MathML.Rendering, a library for displaying mathml
//Copyright (C) 2003, Andy Somogyi
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//For details, see http://numerator.sourceforge.net, or send mail to
//(slightly obfuscated for spam mail harvesters)
//andy[at]epsilon3[dot]net

using System;
using System.Diagnostics;
using System.Drawing;

namespace MathML.Rendering
{
	/// <summary>
	/// Summary description for TableArea.
	/// </summary>
	internal class TableArea : LinearContainer
	{
		private PointF[] solidLines;
		private PointF[] dashedLines;
		private BoundingBox box;
		private MathMLTableElement element;

		public TableArea(MathMLTableElement e, Area[] cells, BoundingBox box, PointF[] solidLines, 
			PointF[] dashedLines) : this(e, cells, box, solidLines, dashedLines, null)
		{		
		}

		/**
		 * private ctor used for fit operation
		 */
		private TableArea(MathMLTableElement e, Area[] cells, BoundingBox box, PointF[] solidLines, 
			PointF[] dashedLines, Area source) : base(cells, source)
		{
			this.element = e;
			this.dashedLines = dashedLines;
			this.solidLines = solidLines;
			this.box = box;			
		}


		public override void Render(IGraphicDevice device, float x, float y)
		{
			// base renders all areas at the same origin, shift origin to upper
			// left corner
			base.Render (device, x, y - box.Height);

			LineStyle oldStyle = device.LineStyle;

            device.LineStyle = LineStyle.Solid;
			for(int i = 0; i < solidLines.Length;)
			{
				PointF from = solidLines[i++];
				PointF to = solidLines[i++];
				from.Y = y - box.Height + from.Y; 
				from.X = x + from.X;
				to.Y = y - box.Height + to.Y;
				to.X = x + to.X;
				device.DrawLine(from, to);
			}

			device.LineStyle = LineStyle.Dashed;
			for(int i = 0; i < dashedLines.Length;)
			{
				PointF from = dashedLines[i++];
				PointF to = dashedLines[i++];
				from.Y = y - box.Height + from.Y; 
				from.X = x + from.X;
				to.Y = y - box.Height + to.Y;
				to.X = x + to.X;
				device.DrawLine(from, to);
			}

			device.LineStyle = oldStyle;			
		}

		/**
		 * fit all child area to thier own bounding box.
		 */
		public override Area Fit(BoundingBox box)
		{
			Area[] areas = new Area[content.Length];

			for(int i = 0; i < content.Length; i++)
			{
				areas[i] = content[i].Fit(content[i].BoundingBox);
			}

			// TODO optimize if all areas are the same as this
			return new TableArea(element, areas, box, solidLines, dashedLines, this);
		}

		public override BoundingBox BoundingBox
		{
			get { return box; }
		}

		public override Object Clone()
		{
			return new TableArea(element, content, box, solidLines, dashedLines, null);
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			AreaRegion result = null;
			if(BoundingBox.Contains(x, y, pointX, pointY))
			{
				foreach(Area a in content)
				{
					// if we find a region, it will allready have a element from
					// the table cell area
					result = a.GetRegion(x, y - box.Height, pointX, pointY);
					if(result != null) 
					{
						Debug.Assert(result.Element != null, "Invalid element type for TableArea cell area");
						Debug.Assert(result.Area != null, "Invalid area type for TableArea cell area");
						return result;
					}
				}
				// we should only get here if a user clicks in either the frame or
				// spacing areas
				result = new AreaRegion(this, element, x, y);
			}
			return result;
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			AreaRegion result = null;
			if(this.element == element)
			{
				return new AreaRegion(this, element, x, y);
			}
			else
			{
				foreach(Area a in content)
				{
					result = a.GetRegion(context, x, y - box.Height, element, index);
					if(result != null) 
					{
						Debug.Assert(result.Element != null, "Invalid element type for TableArea cell area");
						Debug.Assert(result.Area != null, "Invalid area type for TableArea cell area");
						return result;
					}
				}
			}
			return null;
		}
	}
}
