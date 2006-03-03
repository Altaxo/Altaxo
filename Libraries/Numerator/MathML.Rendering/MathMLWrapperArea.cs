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
using System.Diagnostics;

namespace MathML.Rendering
{
	/// <summary>
	/// Contains an area tree that is the result of formatting a MathMLElement, 
	/// this area type co-relates that area tree to the source MathMLElement.
	/// </summary>
	internal class MathMLWrapperArea : BinContainerArea
	{
		private MathMLElement element;

		public MathMLWrapperArea(Area area, MathMLElement element) : base(area)
		{
			this.element = element;
		}

		public override AreaRegion GetRegion(float x, float y, float pointX, float pointY)
		{
			AreaRegion region = child.GetRegion (x, y, pointX, pointY);
			if(region != null && region.Element == null)
			{
				region = new AreaRegion(this, element, x, y);
			}
			return region;
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, MathMLElement element, int index)
		{
			if(element == this.element)
			{
				AreaRegion region = child.GetEditRegion(context, x, y, index);
				if(region != null)
				{
					region.Element = element;
				}
				else
				{
					Debug.WriteLine("MathMLWrapperArea: child.GetEditRegion returned null, returning new AreaRegion for this area");
					region = new AreaRegion(this, element, x, y);
				}
				return region;
			}
			else
			{
				return child.GetRegion(context, x, y, element, index);
			}
		}

    public override AreaRegion GetRegion(IFormattingContext context, float x, float y, Area area, int index)
		{
			AreaRegion region = child.GetRegion (context, x, y, area, index);
			if(region != null && region.Element == null)
			{
				region.Element = element;				
			}
			return region;
		}

    public override AreaRegion GetEditRegion(IFormattingContext context, float x, float y, int index)
		{
			return null;
		}

		public override Object Clone()
		{
			return new MathMLWrapperArea(child, element);
		}
	}
}
