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
using Scaled = System.Single;

namespace MathML.Rendering
{
	/**
	 * shift the vertical position of the child branch
	 * by the shift amount. This aftects position and rendering
	 * 
	 * Note, a positive value for the shift shifts the area up, a negative value shifts
	 * the area down
	 */
	internal class ShiftArea : BinContainerArea
	{
		/**
		 * the shift amount
		 */
		private Scaled shift;

		public ShiftArea(Scaled shift, Area area) : base(area)
		{
			this.shift = shift;
		}

		/**
		 * get the child's box shifted by the shift amount
		 */
		public override BoundingBox BoundingBox
		{
			get
			{
				BoundingBox childBox = child.BoundingBox;
				return BoundingBox.New(childBox.Width, childBox.Height + shift, childBox.Depth - shift);
			}
		}

		/**
		 * render the child area branch shifted by the shift 
		 * amount
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			child.Render(device, x, y - shift);
		}

		/**
		 * make a duplicate of this node
		 */
		public override Object Clone()
		{
			return new ShiftArea(shift, child);
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, Area area, int index)
		{
			if(area == source)
			{
				return new AreaRegion(this, x, y);
			}
			else
			{
				return child.GetRegion(context, x, y - shift, area, index);
			}
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			return child.GetRegion (x, y - shift, pointX, pointY);
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			return child.GetRegion (context, x, y - shift, element, index);
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			return child.GetEditRegion (context, x, y - shift, index);
		}


//		public override bool GetSelection(float renderX, float renderY, Selection selection)
//		{
//			// are we looking for element type
//			if (selection.Type == SelectionType.Area && selection.Area == source)
//			{
//				//selection.Element = element;
//				selection.Area = source;
//				selection.CaretX = renderX;
//				selection.CaretY = renderY - BoundingBox.Height;
//				selection.CaretHeight = BoundingBox.VerticalExtent;
//				return true;
//			}
//			return child.GetSelection(renderX, renderY - shift, selection);
//		}
	}
}
