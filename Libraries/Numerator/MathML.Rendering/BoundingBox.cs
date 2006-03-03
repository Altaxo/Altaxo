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
using System.Runtime.InteropServices;
using Scaled = System.Single;

namespace MathML.Rendering
{
	/// <summary>
	/// the screen size of an area
	/// <pre>
	///                    |<-------Width--------->|
	///                    |-----------------------|--
	///                    |                       | ^
	///                    |                       | |
	///                    |                       | | Height
	///                    |                       | |
	///                    |       Baseline        | v
	///  Reference Point-> *-----------------------|--
	///                    |                       | ^
	///                    |                       | | Depth
	///                    |                       | v
	///                    |-----------------------|--
	/// </summary>
	public struct BoundingBox
	{
		/// <summary>
		/// create a new bounding box with the given parameters
		/// </summary>
		public static BoundingBox New(Scaled width, Scaled height, Scaled depth)
		{
			BoundingBox box;
			box.Width = width;
			box.Depth = depth;
			box.Height = height;
     	return box;
		}

		/// <summary>
		/// create a new box with the default values
		/// unfourtunatly, with the current version of c#, you can not
		/// have default consturctors on structs, so we 'fake' one here
		/// so we can set the values that are appropriate for this context.
		/// </summary>
		/// <returns>a new bounding box</returns>
		public static BoundingBox New()
		{
			BoundingBox box;
			box.Width = 0;
			box.Depth = Single.Epsilon;
			box.Height = Single.Epsilon;
      return box;
		}

		/// <summary>
		/// add the contents of the given bounding box to this bounding box
		/// </summary>
		/// <param name="box"></param>
		public void Append(BoundingBox box)
		{
			Width += box.Width;
			Depth = Math.Max(Depth, box.Depth);
			Height = Math.Max(Height, box.Height);
		}

		/// <summary>
		/// set this box to the area that is the overlap of this box, 
		/// and the given box.
		/// </summary>
		/// <param name="box"></param>
		public void Overlap(BoundingBox box)
		{
            Width = Math.Max(Width, box.Width);
            if (!box.Defined)
                return;
            else if (Defined)
            {
                Height = Math.Max(Height, box.Height);
                Depth = Math.Max(Depth, box.Depth);
            }
            else
            {
                Height = box.Height;
                Depth = box.Depth;
            }
		}

		/// <summary>
		/// set this size to a region made by placing this box
		/// under the given box, e.g add the vertical extent of the
		/// given box to the height this box.
		/// </summary>
		/// <param name="box"></param>
		public void Under(BoundingBox box)
		{
			Width = Math.Max(Width, box.Width);
			if (!box.Defined)
				return;
			else if (Defined)
				Height += box.Height + box.Depth;
			else
			{
				Height = box.Height + box.Depth;
				Depth = 0;
			}
		}

		/// <summary>
		/// set this size to a region made by placing this box
		/// over the given box, e.g add the vertical extent of the
		/// given box to the depth this box.
		/// </summary>
		/// <param name="box"></param>
		public void Over(BoundingBox box)
		{
			Width = Math.Max(Width, box.Width);
			if (!box.Defined)
				return;
			else if (Defined)
				Depth += box.Height + box.Depth;
			else
			{
				Height = 0;
				Depth = box.Height + box.Depth;
			}
		}

		/**
		 * get the total vertical distance of this box
		 */
		public float VerticalExtent { get { return Height + Depth; } }

		/**
		 * get the total horizontal distance of this box
		 * this just returns the Width, but is here to be consistant
		 * with VerticalExtent
		 */
		public float HorizontalExtent { get { return Width; } }

		/**
		 * true if this box has been set to non-default values
		 */
		public bool Defined{ get { return Height != Single.Epsilon || Depth != Single.Epsilon || Width != 0; } }

		/**
		 * determines if a bounding box contains the given point.
		 * @param x the x coordinate of the point
		 * @param y the y cooridnate of the point
		 * @return true if the box contains the point, false otherwise
		 */
		public bool Contains(float originX, float originY, float x, float y)
		{
			return x >= originX && x <= (originX + Width) && y <= (originY + Depth) && y >= (originY - Height);
		}

		/**
		 * Horizontal distance from the reference point, and the left vertical 
		 * edge of the box
		 */
		public float Width;

        /**
		 * distance above the baseline
		 */
		public float Height;

		/**
		 * distance below the baseline
		 */
		public float Depth;

		public override string ToString()
		{
			return String.Format("Width: {0}, Height: {1}, Depth: {2}", Width, Height, Depth);
		}
	}
}
