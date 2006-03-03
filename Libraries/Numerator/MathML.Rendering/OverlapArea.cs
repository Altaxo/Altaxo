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
	 * a container area where all child nodes share the same
	 * origin.
	 */
	internal class OverlapArea : LinearContainer
	{
		public OverlapArea(Area[] areas) : base(areas, null) {}

		private OverlapArea(Area[] areas, Area source) : base(areas, source) {}

		/**
		 * get the bounding box that is the region of all the child
		 * areas overlapped
		 */
		public override BoundingBox BoundingBox
		{
			get
			{
				BoundingBox box = BoundingBox.New();
				foreach(Area a in content)
				{
					box.Overlap(a.BoundingBox);
				}
				return box;
			}
		}

		/**
		 * get the maximum strength of the child nodes in each direction.
		 */
		public override Strength Strength
		{
			get
			{
				Strength s = new Strength();
				foreach(Area a in content)
				{
					Strength sa = a.Strength;
					s.Depth = Math.Max(s.Depth, sa.Depth);
					s.Height = Math.Max(s.Height, sa.Height);
					s.Width = Math.Max(s.Width, sa.Width);
				}
				return s;
			}
		}

		/**
		 * fit all child area to the same box.
		 */
		public override Area Fit(BoundingBox box)
		{
			Area[] areas = new Area[content.Length];

			for(int i = 0; i < content.Length; i++)
			{
				areas[i] = content[i].Fit(box);
			}
			return new OverlapArea(areas, this);
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			if(!BoundingBox.Contains(x, y, pointX, pointY)) return null;
			AreaRegion r = null;
			for(int i = 0; i < content.Length; i++)
			{
				if((r = content[i].GetRegion(x, y, pointX, pointY)) != null) return r;
			}
			return new AreaRegion(this, x, y);
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			foreach(Area a in content)
			{
				AreaRegion r = a.GetRegion(context, x, y, element, index);
				if(r != null) return r;
			}
			return null;
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			foreach(Area a in content)
			{
				AreaRegion r = a.GetEditRegion(context, x, y, index);
				if(r != null) return r;				
			}
			return null;
		}
	}
}
