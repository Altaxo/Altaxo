#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Graph;
using Altaxo.Main.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class Graph3DDocument
	{
		public string Name { get; set; }

		public void Draw(IGraphicContext3D gc)
		{
			var pctb = gc.GetPositionColorIndexedTriangleBuffer(3);

			var offs = pctb.VertexCount;

			var txt = new Primitives.Text3D();
			txt.AddWithNormals(
				(pt, n) =>
				{
					PointD3D color;
					if (n.Z == 1)
						color = new PointD3D(1, 0, 0);
					else if (n.Z == -1)
						color = new PointD3D(0, 1, 0);
					else
						color = new PointD3D(0, 0, 1);

					pctb.AddTriangleVertex((float)pt.X, (float)pt.Y, (float)pt.Z, 1, (float)color.X, (float)color.Y, (float)color.Z, 1);
				},
				(i0, i1, i2) => pctb.AddTriangleIndices(i0, i1, i2),
				ref offs);

			PointD3D[] colors = new[] { new PointD3D(1,0,0), new PointD3D(0,1,0), new PointD3D(0,0,1),
				new PointD3D(1,1,0), new PointD3D(1,0,1), new PointD3D(0,1,1)};

			Primitives.Cube.Add(0, 0, 0, 1, 1, 1,
				(pt, n) =>
				{
					PointD3D color = colors[0];
					if (n.Z == 1)
						color = colors[0];
					else if (n.Z == -1)
						color = colors[1];
					else if (n.Y == 1)
						color = colors[2];
					else if (n.Y == -1)
						color = colors[3];
					else if (n.X == 1)
						color = colors[4];
					else if (n.X == -1)
						color = colors[5];

					pctb.AddTriangleVertex((float)pt.X, (float)pt.Y, (float)pt.Z, 1, (float)color.X, (float)color.Y, (float)color.Z, 1);
				},
				(i0, i1, i2) => pctb.AddTriangleIndices(i0, i1, i2),
				ref offs);

			var penRed = new PenX3D(NamedColors.Red, 4);
			gc.DrawLine(penRed, new PointD3D(0, 0, 0), new PointD3D(30, 0, 0));
			var penGreen = new PenX3D(NamedColors.Green, 4);
			gc.DrawLine(penGreen, new PointD3D(0, 0, 0), new PointD3D(0, 30, 0));
			var penBlue = new PenX3D(NamedColors.AliceBlue, 4);
			gc.DrawLine(penBlue, new PointD3D(0, 0, 0), new PointD3D(0, 0, 30));
		}

		public static double GetDefaultPenWidth(IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(context);
		}

		public static double GetDefaultMajorTickLength(IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(context);
		}

		public static Altaxo.Graph.NamedColor GetDefaultForeColor(IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultForeColor(context);
		}

		public static FontX3D GetDefaultFont(IReadOnlyPropertyBag context)
		{
			var font = Altaxo.Graph.Gdi.GraphDocument.GetDefaultFont(context);

			return new FontX3D(font, font.Size * 0.25);
		}
	}
}