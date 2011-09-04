#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.LineCaps
{
	/// <summary>
	/// Draws a cap that is a line perpendicular to the end of the line, and on the right side of the line.
	/// </summary>
	public class BreakLineCap : LineCapExtension
	{
		const float _designWidth = 4f;
		const double _designAngle = 45;



		public override string Name { get { return "Break"; } }
		public override float DefaultSize { get { return 8; } }


		CustomLineCap GetClone(Pen pen, float size)
		{
			float endPoint;

			if (pen.Width * _designWidth < size)
				endPoint = pen.Width == 0 ? 1 : size / pen.Width;
			else
				endPoint = _designWidth;

			endPoint /= 2;

			GraphicsPath hPath = new GraphicsPath();

			var r = endPoint / (2 * Math.Sin(_designAngle * (Math.PI / 180)));
			var b = r - r * Math.Cos(_designAngle * (Math.PI / 180));
			var h = endPoint / 2;

			// Create the outline for our custom end cap.

			hPath.AddArc(
				(float)(-r - h), (float)(-b),
				(float)(2 * r), (float)(2 * r),
				(float)(-90 - _designAngle), (float)(2 * _designAngle));


			hPath.AddArc(
				(float)(h - r), (float)(b - 1.999999 * r),
				(float)(2 * r), (float)(2 * r),
				(float)(90 + _designAngle), (float)(-2 * _designAngle));

			CustomLineCap clone = new CustomLineCap(null, hPath); // we set the stroke path only
			clone.SetStrokeCaps(LineCap.Flat, LineCap.Flat);
			return clone;
		}

		public override void SetStartCap(Pen pen, float size)
		{
			pen.StartCap = LineCap.Custom;
			pen.CustomStartCap = GetClone(pen, size);
		}
		public override void SetEndCap(Pen pen, float size)
		{
			pen.EndCap = LineCap.Custom;
			pen.CustomEndCap = GetClone(pen, size);
		}
	}
}
