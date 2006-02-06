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
	 * TODO better documentation
	 */
	internal class BoxArea : BinContainerArea
	{
		/**
		 * create a box area.
		 */
		public BoxArea(Area area, BoundingBox box) : base(area)
		{
			this.box = box;
		}

        /**
		 * clone the box area
		 */
		public override Object Clone() { return new BoxArea(child, box); }

		/**
		 * get the bounding box.
		 * as a BoxArea is fit to a certain size, it returns
		 * that size here.
		 */
		public override BoundingBox BoundingBox
		{
			get { return box; }
		}

		public override Area Fit(BoundingBox box)
		{
			return new BoxArea(child.Fit(this.box), this.box);
		}


		/**
		 * box areas allways have 0 strength in all directions
		 * TODO figure out why
		 */
		public override Strength Strength
		{
			get { return new Strength(0, 0, 0); }
		}

		private BoundingBox box;
	}
}
