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
	 * A VerticalArea stacks two or more areas vertically. The areas are
	 * aligned so that the origins all lay on the same vertical line.
	 * Child areas are stored, and rendered bottom to top.
	 * There is a reference identifier baseline, this specifies the index
	 * of the 'reference area', this is the area who's origin specifies the
	 * baseline of the vertial area. All areas with a index >= baseline
	 * are rendered above the baseline, and all areas with an index < baseline
	 * are rendered below the baseline.
	 */
	internal class VerticalArea : LinearContainer
	{
		/**
		 * index of the baseline element
		 */
		private readonly int baseline;

		/**
		 * the bounding box of this area, calcuated in the ctor
		 */
		protected readonly BoundingBox box;

		/**
		 * default public ctor
		 */
		public VerticalArea(Area[] areas, int baseline) 
			: this(areas, baseline, null)
		{
		}

		/**
		 * private consturctor
		 */
		private VerticalArea(Area[] areas, int baseline, Area source) 
			: base(areas, source)
		{
			this.baseline = baseline;

            BoundingBox bbox = content[baseline].BoundingBox;

			for(int i = 0; i < content.Length; i++)
			{
				if(i < baseline)
				{
					bbox.Over(content[i].BoundingBox);
				}
				else if(i > baseline)
				{
					bbox.Under(content[i].BoundingBox);
				}
			}

			this.box = bbox;
		}


        /**
		 * calculate the extent of this area.
		 * The width is the max width of all child ares.
		 * The Height is the sum of all the area vertical extents
		 * above the baseline, and the depth is the sum of all the 
		 * vertical areas below the baseline.
		 */
		public override BoundingBox BoundingBox
		{
			get { return box; }
		}

		/**
		 * render all of the child areas.
		 */
		public override void Render(IGraphicDevice device, float x, float y)
		{
			y = y + BoundingBox.Depth;
			foreach(Area a in content)
			{
				BoundingBox box = a.BoundingBox;
				y -= box.Depth;
				a.Render(device, x, y);
				y -= box.Height;
			}			
		}

		/**
		 * get the strength
		 * the width strength is the max width strength of the child elements.
		 * the height strength is the sum of the height and depth strength
		 * of all nodes above the baseline.
		 * the depth strength is the sum of all the height and depth strength
		 * of all nodes bellow the baseline.
		 */
		public override Strength Strength
		{
			get
			{
				Strength s = new Strength(0, 0, 0);

				for(int i = 0; i < content.Length; i++)
				{
					Strength p = content[i].Strength;
					s.Width = Math.Max(s.Width, p.Width);
					if(i < baseline)
					{
						s.Depth += p.Depth + p.Height;
					}
					else if(i > baseline)
					{
						s.Height += p.Depth + p.Height;
					}
					else
					{
						s.Depth += p.Depth;
						s.Height += p.Height;
					}
				}

				return s;
			}
		}

        /**
		 * re-size all child nodes to the given width, and a height
		 * and depth proportional 
		 */
		public override Area Fit(BoundingBox box)
		{
			// new content area where fitted area are written to
			Area[] newContent = new Area[content.Length];

			// bounding box (natural size), and strength of the array
			// as a whole
			BoundingBox b = BoundingBox;
			Strength s = Strength;

			// availible height and depth distance to stretch
			float aHeight = Math.Max(0, box.Height - b.Height);
			float aDepth = Math.Max(0, box.Depth - b.Depth);

			for(int i = 0; i < content.Length; i++)
			{
				// strength and size of the current child area
				Strength ps = content[i].Strength;
				BoundingBox pb = content[i].BoundingBox;

				// above baseline
				if(i > baseline && s.Height > 0)
				{
					pb.Height += (aHeight * ps.Height) / s.Height;
					pb.Depth += (aHeight * ps.Depth) / s.Height;
				}
				// below baseline
				else if(i < baseline && s.Depth > 0)
				{
					pb.Height += (aDepth * ps.Height) / s.Depth;
					pb.Depth += (aDepth * ps.Depth) / s.Depth;
				}
				// is baseline
				else if(i == baseline && s.Height + s.Depth > 0)
				{
					if (s.Height > 0) pb.Height += (aHeight * ps.Height) / s.Height;
					if (s.Depth > 0) pb.Depth += (aDepth * ps.Depth) / s.Depth;
				}

				newContent[i] = content[i].Fit(BoundingBox.New(box.Width, pb.Height, pb.Depth));
			}
			return new VerticalArea(newContent, baseline, this);
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			if(!BoundingBox.Contains(x, y, pointX, pointY)) return null;

			float yy = y + BoundingBox.Depth;
			foreach(Area a in content)
			{
				BoundingBox box = a.BoundingBox;
				yy -= box.Depth;
				AreaRegion r = a.GetRegion(x, yy, pointX, pointY);
				if(r != null) 
				{
					return r;
				}
				yy -= box.Height;
			}

			return new AreaRegion(this, x, y);
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			float yy = y + BoundingBox.Depth;
			foreach(Area a in content)
			{
				BoundingBox box = a.BoundingBox;
				yy -= box.Depth;
				AreaRegion r = a.GetEditRegion(context, x, yy, index);
				if(r != null) 
				{
					return r;
				}
				yy -= box.Height;
			}

			return null;
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			float yy = y + BoundingBox.Depth;
			foreach(Area a in content)
			{
				BoundingBox box = a.BoundingBox;
				yy -= box.Depth;
				AreaRegion r = a.GetRegion(context, x, yy, element, index);
				if(r != null) 
				{
					return r;
				}
				yy -= box.Height;
			}

			return null;
		}




		/*
		public override MathMLElement GetElement(float renderX, float renderY, float x, float y)
		{
			if(!BoundingBox.Contains(renderX, renderY, x, y)) return null;
			MathMLElement e = null;
			renderY = renderY + BoundingBox.Depth;
			foreach(Area a in content)
			{
				BoundingBox box = a.BoundingBox;
				renderY -= box.Depth;
				if((e = a.GetElement(renderX, renderY, x, y)) != null) return e;
				renderY -= box.Height;
			}	
			return element;
		}
		*/

		/*
		public override bool GetSelection(float renderX, float renderY, Selection selection)
		{
			// are we looking for element type
			if (selection.Type == SelectionType.Area && selection.Area == source)
			{
				selection.Element = element;
				selection.Area = source;
				selection.CaretX = renderX;
				selection.CaretY = renderY - BoundingBox.Height;
				selection.CaretHeight = BoundingBox.VerticalExtent;
				return true;
			}

			// if this is an element type, we do not care about the coordinates because
			// we do not know the location of the area, otherwise the location has to 
			// be bound by this box
			if(selection.Type != SelectionType.Area && 
				!BoundingBox.Contains(renderX, renderY, selection.CaretX, selection.CaretY)) return false;

			// search each child area
			float y = renderY + BoundingBox.Depth;
			foreach(Area a in content)
			{
				BoundingBox box = a.BoundingBox;
				y -= box.Depth;
				if(a.GetSelection(renderX, y, selection)) 
				{
					if(selection.Element == null) selection.Element = element;
					return true;
				}
				y -= box.Height;
			}

			// if looking for a mouse select, and we got here, the mouse point was inside this box, 
			// but no element returned a caret pos. That means that this is the top most area
			// that coresponds to an element, so we select that element here if we have one.
			// 
			// only return true if this is a top level container for
			// a source mathml element. We do not want to focus 
			// child containers that have no source because we do not
			// know where to move the caret next.
			if(selection.Type == SelectionType.Mouse && element != null)
			{
				selection.Area = source;
				selection.Element = element;
				selection.CaretX = renderX;
				selection.CaretY = renderY - BoundingBox.Height;
				selection.CaretHeight = BoundingBox.VerticalExtent;
				return true;
			}

			return false;			
		}	
		*/
	}
}
