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
	 * The ink area renders a solid box of ink (using the current color in the
	 * rendering context), whose extent is equal to the bounding box of the
	 * child area. The child area is not rendered
	 */
	internal class InkArea : BinContainerArea
	{
		/**
		 * create an ink area.
		 * the ink area gets its' color from the color area which
		 * is rendered first.
		 */
		public InkArea(Area area) : base(area) 
    {
    }

		/**
		 * render a filled rectangle with the current color that fills
		 * the area of the child node. Note, the context is given with
		 * coordinates at the origin of the bounding box, so we need to 
		 * calculate the extent of that rectangle here.
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			BoundingBox box = child.BoundingBox;
			device.DrawFilledRectangle(y - box.Height, x, x + box.Width, y + box.Depth);
		}

		/**
		 * clone the object.
		 * Make a shallow copy of this object
		 */
		public override Object Clone() 
		{ 
			return new InkArea(child);
		}
	}
}
