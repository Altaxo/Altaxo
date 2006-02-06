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
	 * The space area represents a generic space area that renders
	 * nothing. Filler areas are replaced by space areas in the
	 * Fit process This class simply takes up horizontal space
	 */
	internal class HorizontalSpaceArea : SimpleArea
	{
		private Scaled width;

		/**
		 * create a new space area with the given width
		 */
		public HorizontalSpaceArea(Scaled width)
		{
			this.width = width;
		}

		/**
		 * get the bounding box of this area
		 * this area only takes up horizontal space, therefore
		 * it has no height or depth
		 */
		public override BoundingBox BoundingBox
		{
			get { return BoundingBox.New(width, Scaled.Epsilon, Scaled.Epsilon); }
		}

		/**
		 * TODO figure out why we return this value
		 */
		public override Scaled LeftEdge
		{
			get { return Scaled.Epsilon; }
		}

		/**
		 * TODO figure out why we return this value
		 */
		public override Scaled RightEdge
		{
			get { return Scaled.Epsilon; }
		}

		public override Object Clone()
		{
			return new HorizontalSpaceArea(width);
		}
	}
}
