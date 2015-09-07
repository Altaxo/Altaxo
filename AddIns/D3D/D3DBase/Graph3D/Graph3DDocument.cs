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
			/*
			var pctb = gc.GetPositionColorTriangleBuffer(3);

			pctb.Add(0.0f, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f, 1f);
			pctb.Add(0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 1.0f, 0.0f, 1f);
			pctb.Add(-0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1f);
			*/

			var pctb = gc.GetPositionColorIndexedTriangleBuffer(3);

			var offs = pctb.VertexCount;

			/* Indexed triangle
			pctb.AddTriangleVertex(0.0f, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f, 1f);
			pctb.AddTriangleVertex(0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 1.0f, 0.0f, 1f);
			pctb.AddTriangleVertex(-0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1f);
			pctb.AddTriangleIndices(0 + offs, 1 + offs, 2 + offs);
			*/

			PointD3D[] colors = new[] { new PointD3D(1,0,0), new PointD3D(0,1,0), new PointD3D(0,0,1),
				new PointD3D(1,1,0), new PointD3D(1,0,1), new PointD3D(0,1,1)};

			int colorIndex = 0;
			foreach (var vertexAndNormal in Cube.GetIndexedVerticesWithNormal_Vertices(-0.5f, -0.5f, -0.5f, 1, 1, 1))
			{
				var color = colors[colorIndex / 4];
				pctb.AddTriangleVertex((float)vertexAndNormal.P1.X, (float)vertexAndNormal.P1.Y, (float)vertexAndNormal.P1.Z, 1f, (float)color.X, (float)color.Y, (float)color.Z, 1f);
				++colorIndex;
			}

			foreach (var idx in Cube.GetIndexedVerticesWithNormal_Indices())
			{
				pctb.AddTriangleIndices(offs + idx.Index1, offs + idx.Index2, offs + idx.Index3);
			}
		}
	}

	public struct PointD3D
	{
		public double X;
		public double Y;
		public double Z;

		public PointD3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	public struct TwoPointsD3D
	{
		public PointD3D P1;
		public PointD3D P2;
	}

	public struct ThreeIndices
	{
		public int Index1;
		public int Index2;
		public int Index3;

		public ThreeIndices(int i1, int i2, int i3)
		{
			Index1 = i1;
			Index2 = i2;
			Index3 = i3;
		}
	}

	public class Cube
	{
		public static IEnumerable<TwoPointsD3D> GetIndexedVerticesWithNormal_Vertices(double _x, double _y, double _z, double _dx, double _dy, double _dz)
		{
			// Front z = _z

			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y, _z), P2 = new PointD3D(0, 0, -1) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y + _dy, _z), P2 = new PointD3D(0, 0, -1) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y + _dy, _z), P2 = new PointD3D(0, 0, -1) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y, _z), P2 = new PointD3D(0, 0, -1) };

			// Back z = z+dz
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y, _z + _dz), P2 = new PointD3D(0, 0, 1) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y + _dy, _z + _dz), P2 = new PointD3D(0, 0, 1) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y + _dy, _z + _dz), P2 = new PointD3D(0, 0, 1) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y, _z + _dz), P2 = new PointD3D(0, 0, 1) };

			// Top y = y+dy
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y + _dy, _z), P2 = new PointD3D(0, 1, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y + _dy, _z + _dz), P2 = new PointD3D(0, 1, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y + _dy, _z + _dz), P2 = new PointD3D(0, 1, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y + _dy, _z), P2 = new PointD3D(0, 1, 0) };

			// Bottom y = y
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y, _z), P2 = new PointD3D(0, -1, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y, _z + _dz), P2 = new PointD3D(0, -1, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y, _z + _dz), P2 = new PointD3D(0, -1, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y, _z), P2 = new PointD3D(0, -1, 0) };

			// Left x = x
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y, _z), P2 = new PointD3D(-1, 0, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y, _z + _dz), P2 = new PointD3D(-1, 0, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y + _dy, _z + _dz), P2 = new PointD3D(-1, 0, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x, _y + _dy, _z), P2 = new PointD3D(-1, 0, 0) };

			// Right x = x + dx
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y, _z), P2 = new PointD3D(1, 0, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y + _dy, _z + _dz), P2 = new PointD3D(1, 0, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y, _z + _dz), P2 = new PointD3D(1, 0, 0) };
			yield return new TwoPointsD3D { P1 = new PointD3D(_x + _dx, _y + _dy, _z), P2 = new PointD3D(1, 0, 0) };
		}

		public static IEnumerable<ThreeIndices> GetIndexedVerticesWithNormal_Indices()
		{
			// Front z = _z
			yield return new ThreeIndices(0, 1, 2);
			yield return new ThreeIndices(0, 2, 3);

			// Back z = z+dz
			yield return new ThreeIndices(4, 5, 6);
			yield return new ThreeIndices(4, 7, 5);

			// Top y = y+dy
			yield return new ThreeIndices(8, 9, 10);
			yield return new ThreeIndices(8, 10, 11);

			// Bottom y = y
			yield return new ThreeIndices(12, 13, 14);
			yield return new ThreeIndices(12, 15, 13);

			// Left x = x
			yield return new ThreeIndices(16, 17, 18);
			yield return new ThreeIndices(16, 18, 19);

			// Right x = x + dx
			yield return new ThreeIndices(20, 21, 22);
			yield return new ThreeIndices(20, 23, 21);
		}
	}
}