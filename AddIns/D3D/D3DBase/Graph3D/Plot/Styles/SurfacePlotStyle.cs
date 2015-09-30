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

using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph3D.Plot.Styles
{
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi.Plot;
	using Altaxo.Graph.Gdi.Plot.ColorProvider;
	using Altaxo.Graph.Scales.Ticks;
	using Graph.Plot.Data;

	/// <summary>
	/// This plot style is responsible for showing density plots as pixel image. Because of the limitation to a pixel image, each pixel is correlated
	/// with a single data point in the table. Splining of data is not implemented here. Beause of this limitation, the image can only be shown
	/// on linear axes.
	/// </summary>
	[Serializable]
	public class DensityImagePlotStyle
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
		/// Converts the numerical values into logical values.
		/// </summary>
		private NumericalScale _scale;

		/// <summary>The lower bound of the y plot range</summary>
		[NonSerialized]
		private AltaxoVariant _yRangeFrom = double.NaN;

		/// <summary>The upper bound of the y plot range</summary>
		[NonSerialized]
		private AltaxoVariant _yRangeTo = double.NaN;

		[NonSerialized]
		private CachedImageType _imageType;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;

				info.AddValue("ClipToLayer", s._clipToLayer);
				info.AddValue("Scale", s._scale);
				info.AddValue("Colorization", s._colorProvider);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DensityImagePlotStyle s = null != o ? (DensityImagePlotStyle)o : new DensityImagePlotStyle();

				s._clipToLayer = info.GetBoolean("ClipToLayer");
				s.Scale = (NumericalScale)info.GetValue("Scale", s);
				s.ColorProvider = (IColorProvider)info.GetValue("Colorization", s);

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
		public DensityImagePlotStyle()
		{
			this.ColorProvider = new ColorProviderBGMYR();
			this.Scale = new LinearScale() { TickSpacing = new NoTickSpacing() }; // Ticks are not needed here, they will only disturb the bounds of the scale
			InitializeMembers();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The style to copy from.</param>
		public DensityImagePlotStyle(DensityImagePlotStyle from)
		{
			InitializeMembers();
			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as DensityImagePlotStyle;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this._clipToLayer = from._clipToLayer;
				this.ColorProvider = (IColorProvider)from._colorProvider.Clone();
				this.Scale = (NumericalScale)from._scale.Clone();
				this._imageType = CachedImageType.None;

				EhSelfChanged();
				suspendToken.Resume();
			}
			return true;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _colorProvider)
				yield return new Main.DocumentNodeAndName(_colorProvider, "ColorProvider");

			if (null != _scale)
				yield return new Main.DocumentNodeAndName(_scale, "Scale");
		}

		public object Clone()
		{
			return new DensityImagePlotStyle(this);
		}

		public NumericalScale Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (ChildSetMember(ref _scale, value))
				{
					if (!(_scale.TickSpacing is NoTickSpacing))
						_scale.TickSpacing = new NoTickSpacing(); // strip the old tickspacing, use NoTickspacing, since Ticks are not needed in the density image plot style

					if (null != _scale)
						EhChildChanged(_scale, EventArgs.Empty);
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

				if (ChildSetMember(ref _colorProvider, value))
					EhChildChanged(_colorProvider, EventArgs.Empty);
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
			this._imageType = CachedImageType.None;
			EhSelfChanged(EventArgs.Empty);
		}

		/// <summary>
		/// This routine ensures that the plot item updates all its cached data and send the appropriate
		/// events if something has changed. Called before the layer paint routine paints the axes because
		/// it must be ensured that the axes are scaled correctly before the plots are painted.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		public void PrepareScales(IPlotArea3D layer, XYZMeshedColumnPlotData plotData)
		{
			NumericalBoundaries pb = _scale.DataBounds;
			plotData.SetVBoundsFromTemplate(pb); // ensure that the right v-boundary type is set
			using (var suspendToken = pb.SuspendGetToken())
			{
				pb.Reset();
				plotData.MergeVBoundsInto(pb);
				suspendToken.Resume();
			}
		}

		/// <summary>
		/// Paint the density image in the layer.
		/// </summary>
		/// <param name="gfrx">The graphics context painting in.</param>
		/// <param name="gl">The layer painting in.</param>
		/// <param name="plotObject">The data to plot.</param>
		public void Paint(IGraphicContext3D gfrx, IPlotArea3D gl, object plotObject) // plots the curve with the choosen style
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

		private Color GetColor(double val)
		{
			double relval = _scale.PhysicalToNormal(val);
			return _colorProvider.GetColor(relval);
		}

		private void BuildImage(IGraphicContext3D gfrx, IPlotArea3D gl, XYZMeshedColumnPlotData myPlotAssociation, IROMatrix matrix, IROVector logicalRowHeaderValues, IROVector logicalColumnHeaderValues)
		{
			BuildImageV1(gfrx, gl, logicalRowHeaderValues, logicalColumnHeaderValues, matrix); // affine, linear scales, and equidistant points
		}

		private void BuildImageV1(
			IGraphicContext3D g,
			IPlotArea3D gl,
			IROVector lx,
			IROVector ly,
			IROMatrix matrix)
		{
			_imageType = CachedImageType.LinearEquidistant;

			var buf = g.GetPositionColorIndexedTriangleBuffer(0);

			var offs = buf.VertexCount;

			PointD3D pt;
			var zScale = gl.Scales[2];
			for (int i = 0; i < lx.Length; ++i)
			{
				for (int j = 0; j < ly.Length; ++j)
				{
					double lz = zScale.PhysicalVariantToNormal(matrix[i, j]);
					var color = _colorProvider.GetColor(lz);
					gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(lx[i], ly[j], lz), out pt);
					buf.AddTriangleVertex((float)pt.X, (float)pt.Y, (float)pt.Z, 1, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
				}
			}

			int lxl = lx.Length;
			int lyl = ly.Length;
			int lxlm1 = lx.Length - 1;
			int lylm1 = ly.Length - 1;

			for (int i = 0; i < lxlm1; ++i)
			{
				for (int j = 0; j < lylm1; ++j)
				{
					// upper side
					buf.AddTriangleIndices(offs + 0, offs + lxl, offs + 1);
					buf.AddTriangleIndices(offs + 1, offs + lxl, offs + lxl + 1);

					// from below
					buf.AddTriangleIndices(offs + 0, offs + 1, offs + lxl);
					buf.AddTriangleIndices(offs + 1, offs + lxl + 1, offs + lxl);

					++offs;
				}
				++offs; // one extra increment because inner loop ends at one less than array size
			}
		}

		#region Changed event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, _scale))
			{
				this._imageType = CachedImageType.None;
			}
			else if (object.ReferenceEquals(sender, _colorProvider))
			{
				this._imageType = CachedImageType.None;
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Changed event handling
	} // end of class DensityImagePlotStyle
}