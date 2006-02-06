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
	 * a filler area is used to specify a space area that will fit the avalible
	 * space in the containing area. A filler area can be thought of as a spring
	 * that pushes the areas next to it to opposite directions. A filler area is
	 * meant to be a temporary area to be substituted by a space area in the
	 * Fit method.
	 */
	internal class VerticalFillerArea : FillerArea
	{
		/**
		 * just intiialize the base class
		 */
		public VerticalFillerArea() {}

		/**
		 * filler areas have no size
		 */
		public override BoundingBox BoundingBox
		{
			get
			{
				return BoundingBox.New(0, 0, 0);
			}
		}

		/**
		 * fit this area to a bounding box. This just
		 * creates a new vertical space area that is the height and depth
		 * of the bounding box
		 */
		public override Area Fit(BoundingBox box)
		{
			return new VerticalSpaceArea(box.Height, box.Depth);
		}

		/**
		 * get the max value of Scaled.
		 * TODO figure out why
		 */
		public override Scaled LeftEdge
		{
			get { return Scaled.MaxValue; }
		}

		/**
		 * get the min value for Scaled
		 * TODO figure out why
		 */
		public override Scaled RightEdge
		{
			get { return Scaled.Epsilon; }
		}

		/**
		 * vertical filler areas always have strength of 1 in
		 * the height and depth directions, and 0 in width
		 * TODO figure out why
		 */
		public override Strength Strength
		{
			get { return new Strength(0, 1, 1); }
		}
	}
}
