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
	/// <summary>
	/// a filler area is used to specify a space area that will fit the avalible
	/// space in the containing area. A filler area can be thought of as a spring
	/// that pushes the areas next to it to opposite directions. A filler area is
	/// meant to be a temporary area to be substituted by a space area in the
	/// Fit method.
	/// </summary>
	internal class HorizontalFillerArea : FillerArea
	{
		/// <summary>
		/// nothing to initialize except base class
		/// </summary>
		public HorizontalFillerArea() {}

		/// <summary>
		/// get a bounding box. 
		/// TODO figure out why we get these sizes
		/// </summary>
		public override BoundingBox BoundingBox
		{
			get { return BoundingBox.New(); }
		}

		/// <summary>
		/// fit this area to a bounding box
		/// create a new horizontal space area with the width of
		/// the given bounding box
		/// </summary>
		public override Area Fit(BoundingBox box)
		{
			return new HorizontalSpaceArea(box.Width);
		}

		/// <summary>
		/// get the strength of this area
		/// Horizontal fillers can only stretch in the width direction, therefore, 
		/// only have re-size strength in the width direction
		/// </summary>
		public override Strength Strength
		{
			get { return new Strength(1, 0, 0); }
		}

		/// <summary>
		/// get the left edge
		/// TODO figure out why we return these values
		/// </summary>
		public override Scaled LeftEdge
		{
			get { return Scaled.Epsilon; } 
		}

		/// <summary>
		/// get the right edge
		/// TODO figure out why we return this value
		/// </summary>
		public override Scaled RightEdge
		{
			get { return Scaled.Epsilon; } 
		}

		public override Object Clone()
		{
			return new HorizontalFillerArea();
		}
	}
}
