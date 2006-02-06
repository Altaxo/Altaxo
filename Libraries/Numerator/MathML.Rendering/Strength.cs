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

namespace MathML.Rendering
{
	/**
	 * store the attributes of a strength direction.
	 * this could just as easily been a BoundingBox, but making
	 * a separate class is a bit clearer to read
	 */
	internal struct Strength
	{
		/**
		 * create a Strength object
		 */
		public Strength(int width, int height, int depth)
		{
			Width = width; Height = height; Depth = depth;
		}

		/**
		 * strength in the Width direction
		 */
		public int Width;

		/**
		 * strength in the Height direction
		 */
		public int Height;

		/**
		 * strength in the Depth direction
		 */
		public int Depth;
	}
}
