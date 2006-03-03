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
using System.Collections;
using MathML;

namespace MathML.Rendering
{
	/**
	 * interface describing basic geometric rendering area.
	 * TODO - import documentation from thesis.
	 */
	public class Area : ICloneable
	{
		/**
		 * make sure that an instance of this base class is
		 * not created
		 */
		protected Area() {}

        /**
		 * render this area. 
		 */
		public virtual void Render(IGraphicDevice device, float x, float y) {}

		/**
		 * Fit this area to a given size. Some areas are not re-sizeable, 
		 * like glyph or string elements. Containers recursivly fit all of
		 * thier child nodes. Only 2 kinds of area return new types, 
		 * these are Filler areas, and Stretch glyphs.
		 */
		public virtual Area Fit(BoundingBox box) { return this; }

		// public virtual Area Replace(ReplacementContext context) { return null; }

		public virtual BoundingBox BoundingBox 
		{
			get { return BoundingBox.New(); }
		}

		public virtual float LeftEdge { get{ return 0;} }

		public virtual float RightEdge { get { return 0;} }

		/**
		 * compute the horizontal component of the identified node's
		 * origin with respect to this node's origin, e.g. the identified
		 * node's reference point with respect to this node's refernce point.
		 * The following recursive relation holds:
		 * 
		 * 1: a node with no child nodes returns 0:
		 * a.Origin([]) = 0 
		 * 
		 * 2: a horizontal array returns the width of all previous nodes
		 * plus the origin of the identified node:
		 * HorizontalArray.Origin([k1,k2,k3,...kn] = sum(0.width -> k1.width) +
		 * k1.Origin([k2,k3,...kn])
		 * 
		 * 3: a node with child nodes that is not a horizontal array
		 * returns the identified node's origin:
		 * a.Origin([k1,k2,k3...kn]) = k1.Origin([k2,k3,...,kn])
		 */
		public virtual float Origin(AreaIdentifier id) { return 0;} 

		public virtual float LeftSide(AreaIdentifier id) { return 0;} 

		public virtual float RightSide(AreaIdentifier id) { return 0;} 

		/**
		 * The 'stretch strength', this is a measure of how capable of 
		 * stretching an area is. This is used for fitting in compund areas.
		 * The token area types are not re-sizable, so use this as a default,
		 * and return a zero re-size strength here in all directions. 
		 * 
		 * The default Strength is all zeros, this means that it can not
		 * stretch in any direction.
		 */
		public virtual Strength Strength { get { return new Strength(); } }

		/**
		 * creates an identfier of the first occurrence of the argument
		 * during a depth-first visit of the area tree
		 * throws InvalidArea if the argument is not found
		 */
		public virtual AreaIdentifier GetIdentifier(Area area)
		{
			Stack path = new Stack();

			if(GetPath(area, path))
			{
				return new AreaIdentifier(path);
			}
			else
			{
				throw new ArgumentException(
					"the area was not found as a descendant of this node", "area");
			}
		}

        /**
		 * get an area with an identifier that is a relative path
		 * to this node.
		 */
		public virtual Area GetArea(AreaIdentifier id) {return null;}

		/**
		 * create a path from this node to one of its' child nodes.
		 * a path from a node to itself is a zero lenght empty path.
		 * note, a path is a zero based offset to one of a node's
		 * child nodes.
		 * 
		 * This method should return true if a path is found, false
		 * otherwise.
		 * 
		 * The default implemtation just returns false. 
		 */
		public virtual bool GetPath(Area area, Stack path) { return false; }

		/**
		 * empty implementation of ICloneable interface. All implementors of this interface should
		 * perform ONLY A SHALLOW COPY. This method is comonly use by a comon base class in the 
		 * Fit operation.
		 */
		public virtual Object Clone() { return null; }

    public virtual AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			return BoundingBox.Contains(x, y, pointX, pointY) ? new AreaRegion(this, x + LeftEdge, y) : null;
		}

    public virtual AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			return null;
		}

    public virtual AreaRegion GetRegion(IFormattingContext context, float x, float y, Area area, int index)
		{
			if(index == 0 && area == this)
			{
				return new AreaRegion(this, x + LeftEdge, y);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Get a editable (where a cursor can be placed) terminal or leaf area. 
		/// Terminal nodes should return themselves, and container areas should return 
		/// the first child node.
		/// </summary>
    public virtual AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			return null;
		}

        /**
		 * get an area from a MathMLElement into which a area
		 * was previously set by SetArea
		 */
		public static Area GetArea(MathMLElement element)
		{
			return element.GetUserData(areaGuid) as Area;
		}

		/**
		 * set an area into a MathMLElement. The area can be later returned
		 * by GetArea.
		 * @return the area that is set into the element
		 */
		public static Area SetArea(MathMLElement element, Area area)
		{
			element.SetUserData(areaGuid, area, null);
			return area;
		}

		/**
		 * string to identifiy an area in the MathMLElement node
		 */
		private const string areaGuid = "DEF47EE-E23A-11D3-B4D0-8208CCE0C829";
	}
}
