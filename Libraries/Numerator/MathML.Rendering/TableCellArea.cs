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
using System.Drawing;

namespace MathML.Rendering
{
	/**
	 * TODO better documentation
	 */
	internal class TableCellArea : BinContainerArea
	{
		private PointF cellShift;
		private PointF areaShift;
		private BoundingBox box;
		private MathMLElement element;

		/**
		 * create a box area.
		 */
		public TableCellArea(MathMLElement element, Area area, BoundingBox box, 
			PointF cellShift, PointF areaShift) : base(area)
		{
			this.cellShift = cellShift;
			this.areaShift = areaShift;
			this.box = box;
			this.element = element;
		}

        /**
		 * x and y are the upper left corner of the parent table
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			child.Render(device, x + areaShift.X, y + areaShift.Y);
		}

		/**
		 * clone the box area
		 */
		public override Object Clone() { return new TableCellArea(element, child, box, cellShift, areaShift); }

		/**
		 * get the bounding box.
		 * as a BoxArea is fit to a certain size, it returns
		 * that size here.
		 */
		public override BoundingBox BoundingBox
		{
			get { return box; }
		}

		/**
		 * A table cell is not stretchable after it is formatted, so return
		 * 0 in all directions
		 */
		public override Strength Strength
		{
			get { return new Strength(0, 0, 0); }
		}	
	
		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{		
			if(box.Contains(x + cellShift.X, y + cellShift.Y, pointX, pointY))
			{
				AreaRegion region = child.GetRegion(x + areaShift.X, y + areaShift.Y, pointX, pointY);
				if(region == null || (region != null && region.Element == null))
				{
					region = new AreaRegion(this, element, x + cellShift.X, y + cellShift.Y);
				}
				return region;
			}
			return null;
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			if(element == this.element)
			{
				return new AreaRegion(this, element, x + cellShift.X, y + cellShift.Y);
			}
			else
			{
				return child.GetRegion(context, x + areaShift.X, y + areaShift.Y, element, index);
			}
		}
	}
}
