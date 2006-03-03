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

namespace MathML.Rendering
{
	/// <summary>
	/// set the background color of a branch of the area tree
	/// </summary>
	internal class BackgroundArea : BinContainerArea
	{
		private Color color;

		public BackgroundArea(Color color, Area area) : base(area)
		{
			this.color = color;
		}

		public override void Render(IGraphicDevice device, float x, float y)
		{
			BoundingBox box = child.BoundingBox;
			Color oldColor = device.Color;
			device.Color = color;
			device.DrawFilledRectangle(y - box.Height, x, x + box.Width, y + box.Depth);
			device.Color = oldColor;
			child.Render(device, x, y);
		}	

		public override Object Clone()
		{
			return new BackgroundArea(color, child);
		}
	}
}
