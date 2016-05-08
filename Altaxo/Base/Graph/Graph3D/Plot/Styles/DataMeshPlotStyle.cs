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

using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Geometry;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi.Plot;
	using Altaxo.Graph.Gdi.Plot.ColorProvider;
	using Altaxo.Graph.Scales.Ticks;
	using Drawing.D3D;
	using Drawing.D3D.Material;
	using Graph.Plot.Data;
	using GraphicsContext;

	/// <summary>
	/// This plot style is responsible for showing density plots as pixel image. Because of the limitation to a pixel image, each pixel is correlated
	/// with a single data point in the table. Splining of data is not implemented here. Beause of this limitation, the image can only be shown
	/// on linear axes.
	/// </summary>
	[Serializable]
	public class DataMeshPlotStyle
			:
			Main.SuspendableDocumentNodeWithEventArgs,
			Main.ICopyFrom
	{
		[Serializable]
		private enum CachedImageType { None, LinearEquidistant, Other };

		/// <summary>If true, the image is clipped to the layer boundaries.</summary>
		private bool _clipToLayer = true;

		/// <summary>Calculates the color from the relative value.</summary>
		private IColorProvider _colorProvider;

		/// <summary>
		/// Converts the numerical height values into logical values used for color calculation.
		/// This member can be null. In this case the z-scale of the parent coordinate system is used for coloring.
		/// </summary>
		private NumericalScale _colorScale;

		/// <summary>
		/// The material used to show the surface. Here only the specular properties of the material are used, because the color is provided by the color provider
		/// </summary>
		private IMaterial _material;

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataMeshPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DataMeshPlotStyle s = (DataMeshPlotStyle)obj;

				info.AddValue("ClipToLayer", s._clipToLayer);
				info.AddValue("Colorization", s._colorProvider);
				info.AddValue("ColorScale", s._colorScale);
				info.AddValue("Material", s._material);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DataMeshPlotStyle s = null != o ? (DataMeshPlotStyle)o : new DataMeshPlotStyle();

				s._clipToLayer = info.GetBoolean("ClipToLayer");
				s.ColorProvider = (IColorProvider)info.GetValue("Colorization", s);
				s.ColorScale = (NumericalScale)info.GetValue("ColorScale", s);
				s._material = (IMaterial)info.GetValue("Material", s);

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initialized the member variables to default values.
		/// </summary>
		protected void InitializeMembers()
		{
		}

		/// <summary>
		/// Initializes the style to default values.
		/// </summary>
		public DataMeshPlotStyle()
		{
			this.ColorProvider = new ColorProviderBGRY();
			this._material = new MaterialWithoutColorOrTexture();

			InitializeMembers();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The style to copy from.</param>
		public DataMeshPlotStyle(DataMeshPlotStyle from)
		{
			InitializeMembers();
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as DataMeshPlotStyle;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this._clipToLayer = from._clipToLayer;
				_colorProvider = from._colorProvider;
				ChildCloneToMember(ref _colorScale, from._colorScale);
				this._material = from._material; // Material is immutable

				EhSelfChanged();
				suspendToken.Resume();
			}
			return true;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _colorScale)
				yield return new Main.DocumentNodeAndName(_colorScale, "ColorScale");
		}

		public object Clone()
		{
			return new DataMeshPlotStyle(this);
		}

		/// <summary>
		/// Converts the numerical height values into logical values used for color calculation.
		/// This member can be null, in this case the z-scale of the parent coordinate system is used for coloring.
		/// </summary>
		public NumericalScale ColorScale
		{
			get
			{
				return _colorScale;
			}
			set
			{
				if (ChildSetMember(ref _colorScale, value))
				{
					if (null != _colorScale && !(_colorScale.TickSpacing is NoTickSpacing))
						_colorScale.TickSpacing = new NoTickSpacing(); // strip the old tickspacing, use NoTickspacing, since Ticks are not needed in the density image plot style

					if (null != _colorScale)
						EhChildChanged(_colorScale, EventArgs.Empty);
					else
						EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public IColorProvider ColorProvider
		{
			get { return _colorProvider; }
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (!object.ReferenceEquals(value, _colorProvider))
				{
					_colorProvider = value;
					EhChildChanged(_colorProvider, EventArgs.Empty);
				}
			}
		}

		public bool ClipToLayer
		{
			get { return _clipToLayer; }
			set
			{
				bool oldValue = _clipToLayer;
				_clipToLayer = value;

				if (_clipToLayer != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public IMaterial Material
		{
			get { return _material; }
			set
			{
				var oldValue = _material;
				_material = value;
				if (!object.ReferenceEquals(oldValue, value))
					EhSelfChanged();
			}
		}

		private bool NaNEqual(double a, double b)
		{
			if (double.IsNaN(a) && double.IsNaN(b))
				return true;

			return a == b;
		}

		/// <summary>
		/// Called by the parent plot item to indicate that the associated data has changed. Used to invalidate the cached bitmap to force
		/// rebuilding the bitmap from new data.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		public void EhDataChanged(object sender)
		{
			EhSelfChanged(EventArgs.Empty);
		}

		/// <summary>
		/// This routine ensures that the plot item updates all its cached data and send the appropriate
		/// events if something has changed. Called before the layer paint routine paints the axes because
		/// it must be ensured that the axes are scaled correctly before the plots are painted.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		/// <param name="plotData">The plot data.</param>
		public void PrepareScales(IPlotArea layer, XYZMeshedColumnPlotData plotData)
		{
			if (_colorScale != null)
			{
				// in case we use our own scale for coloring, we need to calculate the data bounds
				NumericalBoundaries pb = _colorScale.DataBounds;
				plotData.SetVBoundsFromTemplate(pb); // ensure that the right v-boundary type is set
				using (var suspendToken = pb.SuspendGetToken())
				{
					pb.Reset();
					plotData.MergeVBoundsInto(pb);
					suspendToken.Resume();
				}
			}
		}

		/// <summary>
		/// Paint the density image in the layer.
		/// </summary>
		/// <param name="gfrx">The graphics context painting in.</param>
		/// <param name="gl">The layer painting in.</param>
		/// <param name="plotObject">The data to plot.</param>
		public void Paint(IGraphicsContext3D gfrx, IPlotArea gl, object plotObject) // plots the curve with the choosen style
		{
			if (!(plotObject is XYZMeshedColumnPlotData))
				return; // we cannot plot any other than a TwoDimMeshDataAssociation now

			XYZMeshedColumnPlotData myPlotAssociation = (XYZMeshedColumnPlotData)plotObject;

			IROMatrix matrix;
			IROVector logicalRowHeaderValues, logicalColumnHeaderValues;

			myPlotAssociation.DataTableMatrix.GetWrappers(
					gl.XAxis.PhysicalVariantToNormal, // transformation function for row header values
					Altaxo.Calc.RMath.IsFinite,       // selection functiton for row header values
					gl.YAxis.PhysicalVariantToNormal, // transformation function for column header values
					Altaxo.Calc.RMath.IsFinite,       // selection functiton for column header values
					out matrix,
					out logicalRowHeaderValues,
					out logicalColumnHeaderValues
					);

			int cols = matrix.Columns;
			int rows = matrix.Rows;

			if (cols <= 1 || rows <= 1)
				return; // we cannot plot anything  if one length is zero or one

			BuildImage(gfrx, gl, myPlotAssociation, matrix, logicalRowHeaderValues, logicalColumnHeaderValues);
		}

		private static bool IsEquidistant(double[] x, Altaxo.Collections.IAscendingIntegerCollection indices, double relthreshold)
		{
			if (indices.Count <= 1)
				return true;
			int N = indices.Count;
			double first = x[indices[0]];
			double last = x[indices[N - 1]];
			double spanByNM1 = (last - first) / (N - 1);
			double threshold = Math.Abs(relthreshold * spanByNM1);

			for (int i = 0; i < N; i++)
			{
				if (Math.Abs((x[indices[i]] - first) - i * spanByNM1) > threshold)
					return false;
			}
			return true;
		}

		private static bool IsEquidistant(IROVector x, double relthreshold)
		{
			int NM1 = x.Length - 1;
			if (NM1 <= 0)
				return true;
			double first = x[0];
			double last = x[NM1];
			double spanByNM1 = (last - first) / (NM1);
			double threshold = Math.Abs(relthreshold * spanByNM1);

			for (int i = 0; i <= NM1; i++)
			{
				if (Math.Abs((x[i] - first) - i * spanByNM1) > threshold)
					return false;
			}
			return true;
		}

		private void BuildImage(IGraphicsContext3D gfrx, IPlotArea gl, XYZMeshedColumnPlotData myPlotAssociation, IROMatrix matrix, IROVector logicalRowHeaderValues, IROVector logicalColumnHeaderValues)
		{
			BuildImageWithUColor(gfrx, gl, logicalRowHeaderValues, logicalColumnHeaderValues, matrix); // affine, linear scales, and equidistant points
		}

		private void BuildImageWithUColor(
				IGraphicsContext3D g,
				IPlotArea gl,
				IROVector lx,
				IROVector ly,
				IROMatrix matrix)
		{
			IPositionNormalUIndexedTriangleBuffer buffers;

			if (gl.ClipDataToFrame == LayerDataClipping.None && !_clipToLayer)
			{
				buffers = g.GetPositionNormalUIndexedTriangleBuffer(_material, null, _colorProvider);
			}
			else
			{
				var clipPlanes = new PlaneD3D[6];
				clipPlanes[0] = new PlaneD3D(1, 0, 0, 0);
				clipPlanes[1] = new PlaneD3D(-1, 0, 0, -gl.Size.X);
				clipPlanes[2] = new PlaneD3D(0, 1, 0, 0);
				clipPlanes[3] = new PlaneD3D(0, -1, 0, -gl.Size.Y);
				clipPlanes[4] = new PlaneD3D(0, 0, 1, 0);
				clipPlanes[5] = new PlaneD3D(0, 0, -1, -gl.Size.Z);

				buffers = g.GetPositionNormalUIndexedTriangleBuffer(_material, clipPlanes, _colorProvider);
			}

			var buf = buffers;
			var offs = buf.VertexCount;

			int lxl = lx.Length;
			int lyl = ly.Length;
			int lxlm1 = lx.Length - 1;
			int lylm1 = ly.Length - 1;

			var vertexPoints = new PointD3D[lxl, lyl];
			var vertexColors = new Color[lxl, lyl];

			PointD3D pt;
			var zScale = gl.ZAxis;
			for (int i = 0; i < lx.Length; ++i)
			{
				for (int j = 0; j < ly.Length; ++j)
				{
					double lz = zScale.PhysicalVariantToNormal(matrix[i, j]);
					gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(lx[i], ly[j], lz), out pt);

					vertexPoints[i, j] = pt;
					vertexColors[i, j] = _colorProvider.GetColor(null == _colorScale ? lz : _colorScale.PhysicalVariantToNormal(matrix[i, j])); // either use the scale of the coordinate system or our own color scale
				}
			}

			// calculate the normals
			for (int i = 0; i < lx.Length; ++i)
			{
				for (int j = 0; j < ly.Length; ++j)
				{
					var pm = vertexPoints[i, j];
					var vec1 = vertexPoints[Math.Min(i + 1, lxlm1), j] - vertexPoints[Math.Max(i - 1, 0), j];
					var vec2 = vertexPoints[i, Math.Min(j + 1, lylm1)] - vertexPoints[i, Math.Max(j - 1, 0)];

					var normal = VectorD3D.CrossProduct(vec1, vec2).Normalized;
					double lz = null != _colorScale ? _colorScale.PhysicalVariantToNormal(matrix[i, j]) : zScale.PhysicalVariantToNormal(matrix[i, j]);
					buf.AddTriangleVertex(pm.X, pm.Y, pm.Z, normal.X, normal.Y, normal.Z, lz);
					buf.AddTriangleVertex(pm.X, pm.Y, pm.Z, -normal.X, -normal.Y, -normal.Z, lz);
				}
			}

			for (int i = 0; i < lxlm1; ++i)
			{
				for (int j = 0; j < lylm1; ++j)
				{
					// upper side
					buf.AddTriangleIndices(offs + 0, offs + 2 * lyl, offs + 2);
					buf.AddTriangleIndices(offs + 2, offs + 2 * lyl, offs + 2 * lyl + 2);

					// from below
					buf.AddTriangleIndices(offs + 0 + 1, offs + 2 + 1, offs + 2 * lyl + 1);
					buf.AddTriangleIndices(offs + 2 + 1, offs + 2 * lyl + 2 + 1, offs + 2 * lyl + 1);

					offs += 2;
				}
				offs += 2; // one extra increment because inner loop ends at one less than array size
			}
		}

		#region Changed event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, _colorScale))
			{
			}
			else if (object.ReferenceEquals(sender, _colorProvider))
			{
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Changed event handling
	} // end of class DensityImagePlotStyle
}