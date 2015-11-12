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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	using Altaxo.Geometry;
	using Altaxo.Graph;

	public class FontManager3D
	{
		private static FontManager3D _instance;

		/// <summary>
		/// The _cached character outlines. Key is the invariant Gdi typeface name (without size information, as obtained with <see cref="Altaxo.Graph.FontX.InvariantDescriptionStringWithoutSizeInformation"/>). Value is a dictionary with text character as key and the polygonal shape of this character as value.
		/// </summary>
		protected Dictionary<string, Dictionary<char, Primitives.CharacterGeometry>> _cachedCharacterOutlines = new Dictionary<string, Dictionary<char, Primitives.CharacterGeometry>>();

		protected Bitmap _bmp = new Bitmap(16, 16);
		protected Graphics _graphics;

		public static FontManager3D Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				_instance = value;
			}
		}

		static FontManager3D()
		{
			_instance = new FontManager3D();
		}

		protected FontManager3D()
		{
		}

		private void EnsureGraphicsCreated()
		{
			if (null == _graphics)
			{
				_bmp = new Bitmap(16, 16);
				_graphics = Graphics.FromImage(_bmp);
			}
		}

		public virtual VectorD3D MeasureString(string text, FontX3D font, StringFormat format)
		{
			EnsureGraphicsCreated();
			var size = _graphics.MeasureString(text, Altaxo.Graph.Gdi.GdiFontManager.ToGdi(font.Font), new PointF(0, 0), format);
			return new VectorD3D(size.Width, size.Height, font.Depth);
		}

		public virtual FontInfo GetFontInformation(FontX3D font)
		{
			// get some properties of the font
			EnsureGraphicsCreated();
			var gdiFont = Altaxo.Graph.Gdi.GdiFontManager.ToGdi(font.Font);
			double size = gdiFont.Size;
			double cyLineSpace = gdiFont.GetHeight(_graphics); // space between two lines
			int iCellSpace = gdiFont.FontFamily.GetLineSpacing(gdiFont.Style);
			int iCellAscent = gdiFont.FontFamily.GetCellAscent(gdiFont.Style);
			int iCellDescent = gdiFont.FontFamily.GetCellDescent(gdiFont.Style);
			double cyAscent = cyLineSpace * iCellAscent / iCellSpace;
			double cyDescent = cyLineSpace * iCellDescent / iCellSpace;

			return new FontInfo(cyLineSpace, cyAscent, cyDescent, size);
		}

		public FontX3D GetFont(string fontFamilyName, double size, double depth, Altaxo.Graph.FontXStyle style)
		{
			return new FontX3D(Altaxo.Graph.Gdi.GdiFontManager.GetFont(fontFamilyName, size, style), depth);
		}

		protected const double FontSizeForCaching = 1024;

		public Primitives.CharacterGeometry GetCharacterGeometry(Altaxo.Graph.FontX font, char textChar)
		{
			var typefaceName = font.InvariantDescriptionStringWithoutSizeInformation;

			Dictionary<char, Primitives.CharacterGeometry> cachedCharacters;
			if (!_cachedCharacterOutlines.TryGetValue(typefaceName, out cachedCharacters))
			{
				cachedCharacters = new Dictionary<char, Primitives.CharacterGeometry>();
				_cachedCharacterOutlines.Add(typefaceName, cachedCharacters);
			}

			Primitives.CharacterGeometry cachedChar;
			if (!cachedCharacters.TryGetValue(textChar, out cachedChar))
			{
				cachedChar = InternalGetCharacterGeometryForCaching(textChar, font);
				cachedCharacters.Add(textChar, cachedChar);
			}

			return cachedChar;
		}

		protected Primitives.CharacterGeometry InternalGetCharacterGeometryForCaching(char textChar, Altaxo.Graph.FontX font)
		{
			var charOutline = InternalGetCharacterOutlineForCaching(textChar, font); // get the - already simplified - polygonal shape of the character

			var polygons = charOutline.Outline;

			var polygonsWithNormal = new List<PolygonClosedWithNormalsD2D>(polygons.Select(poly => new PolygonClosedWithNormalsD2D(poly))); // get the polygons with normal, used to form the walls of the character

			var indexedTriangles = Triangulate(polygons); // Triangles that can be used for the front and the back side of the character

			var result = new Primitives.CharacterGeometry(
				polygonsWithNormal, indexedTriangles,
				charOutline.FontSize, charOutline.LineSpacing, charOutline.Baseline,
				charOutline.AdvanceWidth, charOutline.LeftSideBearing, charOutline.RightSideBearing);

			return result;
		}

		/// <summary>
		/// Triangulates the specified polygons. The result are indexed triangles.
		/// </summary>
		/// <param name="polygons">The polygons to triangulate.</param>
		/// <returns>Instance of the <see cref="Primitives.IndexedTriangles"/> class, which holds the triangle vertices as well as the indices.</returns>
		private static Primitives.IndexedTriangles Triangulate(IList<PolygonClosedD2D> polygons)
		{
			var triangles = GetTriangles(polygons);
			var pointList = new List<PointD2D>();
			var pointToIndex = new Dictionary<PointD2D, int>();
			var indexList = new List<int>();

			foreach (var triangulatedPolygon in triangles)
			{
				foreach (var triangle in triangulatedPolygon.Triangles)
				{
					var p0 = new PointD2D(triangle.Points[0].X, triangle.Points[0].Y);
					var p1 = new PointD2D(triangle.Points[1].X, triangle.Points[1].Y);
					var p2 = new PointD2D(triangle.Points[2].X, triangle.Points[2].Y);

					int i0, i1, i2;

					if (!pointToIndex.TryGetValue(p0, out i0))
					{
						i0 = pointList.Count;
						pointToIndex.Add(p0, i0);
						pointList.Add(p0);
					}
					if (!pointToIndex.TryGetValue(p1, out i1))
					{
						i1 = pointList.Count;
						pointToIndex.Add(p1, i1);
						pointList.Add(p1);
					}
					if (!pointToIndex.TryGetValue(p2, out i2))
					{
						i2 = pointList.Count;
						pointToIndex.Add(p2, i2);
						pointList.Add(p2);
					}

					indexList.Add(i0);
					indexList.Add(i1);
					indexList.Add(i2);
				}
			}

			var indexedTriangles = new Primitives.IndexedTriangles(pointList.ToArray(), indexList.ToArray());
			return indexedTriangles;
		}

		private RawCharacterOutline InternalGetCharacterOutlineForCaching(char textChar, Altaxo.Graph.FontX font)
		{
			var glyphTypeface = font.InvariantDescriptionStringWithoutSizeInformation;

			var rawOutline = GetRawCharacterOutline(textChar, font, FontSizeForCaching);

			List<List<ClipperLib.IntPoint>> clipperPolygonsInput = new List<List<ClipperLib.IntPoint>>();

			var sharpPoints = new HashSet<ClipperLib.IntPoint>();
			var allPoints = new HashSet<ClipperLib.IntPoint>(); // allPoints to determine whether after the simplification new points were added

			foreach (var polygon in rawOutline.Outline)
			{
				foreach (var p in polygon.SharpPoints)
				{
					sharpPoints.Add(new ClipperLib.IntPoint(p.X * 65536, p.Y * 65536));
				}

				var clipperPolygon = new List<ClipperLib.IntPoint>(polygon.Points.Select((x) => new ClipperLib.IntPoint(x.X * 65536, x.Y * 65536)));
				clipperPolygonsInput.Add(clipperPolygon);

				foreach (var clipperPoint in clipperPolygon)
					allPoints.Add(clipperPoint);
			}

			//clipperPolygons = ClipperLib.Clipper.SimplifyPolygons(clipperPolygons, ClipperLib.PolyFillType.pftEvenOdd);

			var clipperPolygons = new ClipperLib.PolyTree();
			ClipperLib.Clipper clipper = new ClipperLib.Clipper();
			clipper.StrictlySimple = true;
			clipper.AddPaths(clipperPolygonsInput, ClipperLib.PolyType.ptSubject, true);
			clipper.Execute(ClipperLib.ClipType.ctUnion, clipperPolygons, ClipperLib.PolyFillType.pftNonZero, ClipperLib.PolyFillType.pftNegative);

			var polygons = new List<PolygonClosedD2D>();
			var dictClipperNodeToNode = new Dictionary<ClipperLib.PolyNode, PolygonClosedD2D>(); // helper dictionary
			ClipperPolyTreeToPolygonListRecursively(clipperPolygons, sharpPoints, allPoints, polygons, dictClipperNodeToNode);

			var result = rawOutline;
			result.Outline = polygons;

			return result;
		}

		private static void ClipperPolyTreeToPolygonListRecursively(ClipperLib.PolyNode node, HashSet<ClipperLib.IntPoint> sharpPoints, HashSet<ClipperLib.IntPoint> allPoints, List<PolygonClosedD2D> polygonList, IDictionary<ClipperLib.PolyNode, PolygonClosedD2D> dictClipperNodeToNode)
		{
			if (node.Contour != null && node.Contour.Count != 0)
			{
				var pointsInThisPolygon = node.Contour.Select(clipperPt => new PointD2D(clipperPt.X / 65536.0, clipperPt.Y / 65536.0));
				var sharpPointsInThisPolygon = node.Contour.Where(clipperPt => sharpPoints.Contains(clipperPt)).Select(clipperPt => new PointD2D(clipperPt.X / 65536.0, clipperPt.Y / 65536.0));

				var polygon = new PolygonClosedD2D(pointsInThisPolygon.ToArray(), new HashSet<PointD2D>(sharpPointsInThisPolygon));
				polygon.IsHole = node.IsHole;
				polygonList.Add(polygon);

				if (node.IsHole)
				{
					polygon.Parent = dictClipperNodeToNode[node.Parent];
				}

				dictClipperNodeToNode.Add(node, polygon);
			}

			if (0 != node.ChildCount)
			{
				foreach (var childNode in node.Childs)
				{
					ClipperPolyTreeToPolygonListRecursively(childNode, sharpPoints, allPoints, polygonList, dictClipperNodeToNode);
				}
			}
		}

		public static List<Poly2Tri.Polygon> GetTriangles(IList<PolygonClosedD2D> polygons)
		{
			var result = new List<Poly2Tri.Polygon>();

			for (int i = 0; i < polygons.Count; ++i)
			{
				if (polygons[i].IsHole)
					continue;

				var mainPolygon = new Poly2Tri.Polygon(polygons[i].Points.Select(pt => new Poly2Tri.PolygonPoint(pt.X, pt.Y)));

				// find all holes in mainPolygon
				for (int j = 0; j < polygons.Count; ++j)
				{
					if (polygons[j].IsHole && object.ReferenceEquals(polygons[j].Parent, polygons[i]))
					{
						var holePolygon = new Poly2Tri.Polygon(polygons[j].Points.Select(pt => new Poly2Tri.PolygonPoint(pt.X, pt.Y)));
						mainPolygon.AddHole(holePolygon);
					}
				}

				result.Add(mainPolygon);
			}

			foreach (var p in result)
			{
				Poly2Tri.P2T.Triangulate(p);
			}

			return result;
		}

		protected struct RawCharacterOutline
		{
			public IList<PolygonClosedD2D> Outline;
			public double AdvanceWidth;
			public double LeftSideBearing;
			public double RightSideBearing;
			public double FontSize;
			public double LineSpacing;
			public double Baseline;
		}

		/// <summary>
		/// Gets the raw character outline, i.e. the polygonal shape that forms a character. The polygons are in their raw form, i.e. not simplified.
		/// </summary>
		/// <param name="textChar">The text character.</param>
		/// <param name="font">The font. The font size of this font is ignored, because it is given in the next parameter.</param>
		/// <param name="fontSize">Size of the font.</param>
		/// <returns>The list of polygons which forms the character.</returns>
		protected virtual RawCharacterOutline GetRawCharacterOutline(char textChar, Altaxo.Graph.FontX font, double fontSize)
		{
			throw new NotImplementedException("This is not implemented here, because it should be implemented in a derived class. This class should then set the static instance of this class to an instance of the derived class.");
		}
	}
}