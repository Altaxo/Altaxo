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
using System.Diagnostics;
using Scaled = System.Single;

namespace MathML.Rendering
{
	internal class HorizontalArea : LinearContainer
	{
		/** 
		 * size of this area, calculated in ctor
		 */
		protected readonly BoundingBox box;

		/**
		 * create a new horizontal area, initialize the content
		 * child nodes list.
		 */
		public HorizontalArea(Area[] content) : this(content, null) 
		{
		}

		/**
		 * private ctor used for fit'ing
		 */
		private HorizontalArea(Area[] content, Area source) : base(content, source)
		{
			// calulate bounding box
			BoundingBox box = BoundingBox.New();
			foreach(Area a in content) box.Append(a.BoundingBox);
			this.box = box;
		}

		/**
		 * get a bounding box that encompases all child nodes
		 */
		public override BoundingBox BoundingBox
		{
			get { return box; }
		}

		/**
		 * render each child area with an updated rendering context
		 * to give each child a correct left side
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			foreach(Area a in content)
			{
				a.Render(device, x, y);
				x += a.BoundingBox.HorizontalExtent;
			}
		}

		/**
		 * left edge only deals with first element
		 */
		public override Scaled LeftEdge
		{
			get 
			{
				/*
				 * could have left edge from righter area portrude beyond 
				 * lefter areas, but very unlikley
				 *
				Scaled edge = 0;
				Scaled d = 0;
				foreach(Area a in content)
				{
					Scaled aedge = a.LeftEdge;
					if(aedge < Scaled.MaxValue) edge = Math.Min(edge, d + aedge);
					d += a.BoundingBox.Width;
				}
				return edge;
				*/
				return content[0].LeftEdge;
			}
		}

		/**
		 * right edge only cares about last element
		 */
		public override Scaled RightEdge
		{
			get
			{
				/*
				 * could have really long edge from lefter area portrudes
				 * beyond righter area, but very unlikly
				Scaled edge = Scaled.Epsilon;
				Scaled d = 0;
				foreach(Area a in content)
				{
					Scaled aedge = a.LeftEdge;
					if(aedge > Scaled.Epsilon) edge = Math.Max(edge, d + aedge);
					d += a.BoundingBox.Width;
				}
				return edge;
				*/
				return content[content.Length - 1].RightEdge;
			}
		}

		/**
		 * get the width of all nodes up to the identified nodes, 
		 * plus the the origin of the identifed node.
		 * This is the relative distance from the identified node's origin
		 * to this node's origin.
		 */
		public override Scaled Origin(AreaIdentifier id)
		{
			if(id.End)
			{
				return 0;
			}
			else if(id.Current < content.Length)
			{
				Scaled width = 0;
				// all nodes up to, but not including the identified node
				for(int i = 0; i < id.Current; i++) 
				{
					width += content[i].BoundingBox.Width;
				}
				return content[id.Current].Origin(id++) + width;
			}
			else
			{
				throw new InvalidIdentifier();
			}
		}

		/**
		 * fit the child areas into a box.
		 * Essentially, each child area is given an availible space which is proportional
		 * to its' strength with respect to the overall strength of the overall array. Areas
		 * that can not be re-sized in the width direction (have zero width strenght) simply
		 * get re-sized to thier natural width.
		 * 
		 * NOTE: Derived classes must override this method (compound glyphs) because this returns
		 * a new HorzArea, not a derived type.
		 */
		public override Area Fit(BoundingBox box)
		{
			// the strength and un-adjusted size of the total array.
			Strength s = Strength;
			BoundingBox b = BoundingBox;
			Area[] areas = new Area[content.Length];

			// new width of each child area
			float width;

			// availible space for exanding filler areas in the width direction
			// use max here, because width may be negative if we are asked to size into
			// a smaller area.
			float availibleSpace = Math.Max(0, box.Width - b.Width);

			for(int i = 0; i < content.Length; i++)
			{
				Strength sa = content[i].Strength;
				BoundingBox ba = content[i].BoundingBox;

				if(s.Width == 0 || sa.Width == 0)
				{
					// no re-sizing strength, so re-size to natural width
					width = ba.Width;								
				}
				else
				{
					// re-size to natural width + % of availible space
					width = ba.Width + availibleSpace * sa.Width / s.Width;					
				}
				areas[i] = content[i].Fit(BoundingBox.New(width, box.Height, box.Depth));
			}
			return new HorizontalArea(areas, this);
		}

		/**
		 * Sum the strengths of the child nodes in the width direction, and get the 
		 * max of the height and depth strengths.
		 */
		public override Strength Strength
		{
			get 
			{
				Strength strength = new Strength();
				foreach(Area a in content)
				{
					Strength s = a.Strength;
					strength.Width += s.Width;
					strength.Height = Math.Max(strength.Height, s.Height);
					strength.Depth = Math.Max(strength.Depth, s.Depth);
				}
				return strength;
			}
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			if(!BoundingBox.Contains(x, y, pointX, pointY)) return null;
			AreaRegion r = null;
			float xx = x;
			for(int i = 0; i < content.Length; i++)
			{
				if((r = content[i].GetRegion(xx, y, pointX, pointY)) != null) return r;
				xx += content[i].BoundingBox.Width;
			}
			return new AreaRegion(this, x, y);
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, Area area, int index)
		{
			if(area == this || area == source)
			{
				return new AreaRegion(this, x, y);
			}
			else
			{
				foreach(Area a in content)
				{
					AreaRegion r = a.GetRegion(context, x, y, area, index);
					if(r != null) return r;
					x += a.BoundingBox.HorizontalExtent;					
				}
			}
			return null;
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			foreach(Area a in content)
			{
				AreaRegion r = a.GetRegion(context, x, y, element, index);
				if(r != null) return r;
				x += a.BoundingBox.HorizontalExtent;					
			}
			return null;
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			foreach(Area a in content)
			{
				AreaRegion r = a.GetEditRegion(context, x, y, index);
				if(r != null) return r;
				x += a.BoundingBox.HorizontalExtent;					
			}
			return null;
		}
	}
}
