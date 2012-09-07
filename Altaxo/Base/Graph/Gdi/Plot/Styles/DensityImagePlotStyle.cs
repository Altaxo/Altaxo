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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Data;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Plot.Data;
	using Graph.Plot.Data;

	/// <summary>
	/// This plot style is responsible for showing density plots as pixel image. Because of the limitation to a pixel image, each pixel is correlated
	/// with a single data point in the table. Splining of data is not implemented here. Beause of this limitation, the image can only be shown
	/// on linear axes.
	/// </summary>
	[Serializable]
	public class DensityImagePlotStyle
		:
		System.ICloneable,
		Main.ICopyFrom,
		System.Runtime.Serialization.IDeserializationCallback,
		Main.IChangedEventSource,
		Main.IDocumentNode
	{
		[Serializable]
		enum CachedImageType { None, LinearEquidistant, Other };

		/// <summary>
		/// Deprecated: The kind of scaling of the values between from and to.
		/// </summary>
		private enum ScalingStyle
		{
			/// <summary>Linear scale, i.e. color changes linear between from and to.</summary>
			Linear,

			/// <summary>Logarithmic style, i.e. color changes with log(value) between log(from) and log(to).</summary>
			Logarithmic
		};

		/// <summary>
		/// The image which is shown during paint.
		/// </summary>
		[NonSerialized]
		System.Drawing.Bitmap _cachedImage;

		/// <summary>If true, the image is clipped to the layer boundaries.</summary>
		bool _clipToLayer = true;

		/// <summary>Calculates the color from the relative value.</summary>
		IColorProvider _colorProvider;

		/// <summary>
		/// Converts the numerical values into logical values.
		/// </summary>
		NumericalScale _scale;

		/// <summary>The lower bound of the y plot range</summary>
		[NonSerialized]
		AltaxoVariant _yRangeFrom = double.NaN;

		/// <summary>The upper bound of the y plot range</summary>
		[NonSerialized]
		AltaxoVariant _yRangeTo = double.NaN;


		[NonSerialized]
		CachedImageType _imageType;

		/// <summary>
		/// Stores the conditions under which the image is valid. Depends on the type of image.
		/// </summary>
		[NonSerialized]
		object _imageConditionMemento;

		[NonSerialized]
		protected object _parent;

		[field: NonSerialized]
		public event System.EventHandler Changed;



		#region Serialization
		/// <summary>Used to serialize the XYLineScatterPlotStyle Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes XYLineScatterPlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The DensityImagePlotStyle to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;

				// nothing to save up to now
			}
			/// <summary>
			/// Deserializes the DensityImagePlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The empty DensityImagePlotStyle to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized DensityImagePlotStyle.</returns>
			public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;
				s.InitializeMembers();

				// Nothing to deserialize in the moment

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.DensityImagePlotStyle", 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;

				// nothing to save up to now
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				DensityImagePlotStyle s = null != o ? (DensityImagePlotStyle)o : new DensityImagePlotStyle();

				// Nothing to deserialize in the moment

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.DensityImagePlotStyle", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotStyle), 2)]
		class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotImplementedException("This function should not be called, since a newer serialization version is available");
				/*
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;

				info.AddEnum("ScalingStyle", s._scalingStyle);
				info.AddValue("RangeFrom", s._vRangeFrom);
				info.AddValue("RangeTo", s._vRangeTo);
				info.AddValue("ClipToLayer", s._clipToLayer);
				info.AddValue("ColorBelow", s._colorBelow);
				info.AddValue("ColorAbove", s._colorAbove);
				info.AddValue("ColorInvalid", s._colorInvalid);
				*/
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				DensityImagePlotStyle s = null != o ? (DensityImagePlotStyle)o : new DensityImagePlotStyle();

				var scalingStyle = (ScalingStyle)info.GetEnum("ScalingStyle", typeof(ScalingStyle));
				var vRangeFrom = info.GetDouble("RangeFrom");
				var vRangeTo = info.GetDouble("RangeTo");
				s._clipToLayer = info.GetBoolean("ClipToLayer");
				var colorBelow = (NamedColor)info.GetValue("ColorBelow", parent);
				var colorAbove = (NamedColor)info.GetValue("ColorAbove", parent);
				var colorInvalid = (NamedColor)info.GetValue("ColorInvalid", parent);

				var colorProvider = new ColorProvider.ColorProviderBGMYR() { ColorBelow = colorBelow, ColorAbove = colorAbove, ColorInvalid = colorInvalid, Transparency = 0 };
				var scale = scalingStyle == ScalingStyle.Logarithmic ? (NumericalScale)new Log10Scale() : (NumericalScale)new LinearScale();

				scale.Rescaling.SetOrgAndEnd(
					double.IsNaN(vRangeFrom) ? Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Auto : Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Fixed,
					vRangeFrom,
					double.IsNaN(vRangeTo) ? Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Auto : Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Fixed,
					vRangeTo);

				s.Scale = scale;
				s.ColorProvider = colorProvider;



				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotStyle), 3)]
		class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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


		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// At the moment, there is nothing to do here
		}
		#endregion


		/// <summary>
		/// Initialized the member variables to default values.
		/// </summary>
		protected void InitializeMembers()
		{
			_cachedImage = null;

		}

		/// <summary>
		/// Initializes the style to default values.
		/// </summary>
		public DensityImagePlotStyle()
		{
			this.ColorProvider = new ColorProvider.ColorProviderBGMYR();
			this.Scale = new LinearScale();
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
			bool hasCopied = false;
			if (null != from)
			{
				this._clipToLayer = from._clipToLayer;
				this.ColorProvider = (IColorProvider)from._colorProvider.Clone();
				this.Scale = (NumericalScale)from._scale.Clone();

				this._parent = from._parent;

				this._imageType = CachedImageType.None;
				hasCopied = true;
			}
			return hasCopied;
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
				if (null != _scale)
					_scale.Changed -= EhScaleChanged;

				var oldValue = _scale;
				_scale = value;

				if (null != _scale)
					_scale.Changed += EhScaleChanged;

				if (!object.ReferenceEquals(oldValue, value))
					OnChanged();
			}
		}

		public IColorProvider ColorProvider
		{
			get { return _colorProvider; }
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (null != _colorProvider)
					_colorProvider.Changed -= EhColorProviderChanged;

				var oldValue = _colorProvider;
				_colorProvider = value;

				if (null != _colorProvider)
					_colorProvider.Changed += EhColorProviderChanged;

				if (null != oldValue && !object.ReferenceEquals(oldValue, _colorProvider))
					EhColorProviderChanged(_colorProvider, EventArgs.Empty);
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
					OnChanged();
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
			OnChanged();
		}



		/// <summary>
		/// Paint the density image in the layer.
		/// </summary>
		/// <param name="gfrx">The graphics context painting in.</param>
		/// <param name="gl">The layer painting in.</param>
		/// <param name="plotObject">The data to plot.</param>
		public void Paint(Graphics gfrx, IPlotArea gl, object plotObject) // plots the curve with the choosen style
		{
			if (!(plotObject is XYZMeshedColumnPlotData))
				return; // we cannot plot any other than a TwoDimMeshDataAssociation now

			XYZMeshedColumnPlotData myPlotAssociation = (XYZMeshedColumnPlotData)plotObject;

			Altaxo.Data.INumericColumn xColumn = myPlotAssociation.XColumn as Altaxo.Data.INumericColumn;
			Altaxo.Data.INumericColumn yColumn = myPlotAssociation.YColumn as Altaxo.Data.INumericColumn;

			if (null == xColumn || null == yColumn)
				return;
		

			//double layerWidth = gl.Size.Width;
			//double layerHeight = gl.Size.Height;

			int cols = myPlotAssociation.ColumnCount;
			int rows = myPlotAssociation.RowCount;


			if (cols <= 0 || rows <= 0)
				return; // we cannot show a picture if one length is zero


			// there is a need for rebuilding the bitmap only if the data are invalid for some reason
			// if the cached image is valid, then test if the conditions hold any longer
			switch (_imageType)
			{
				case CachedImageType.LinearEquidistant:
					{
						ImageTypeEquiLinearMemento memento = new ImageTypeEquiLinearMemento(gl);
						if (!memento.Equals(_imageConditionMemento))
							this._imageType = CachedImageType.None;

					}
					break;
				case CachedImageType.Other:
					{
						ImageTypeOtherMemento memento = new ImageTypeOtherMemento(gl);
						if (!memento.Equals(_imageConditionMemento))
							this._imageType = CachedImageType.None;
					}
					break;
			}


			// now build the image 
			// note that the image type can change during the call of BuildImage
			if (_imageType == CachedImageType.None || _cachedImage == null)
			{
				BuildImage(gfrx, gl, myPlotAssociation, cols, rows);
				switch (_imageType)
				{
					case CachedImageType.LinearEquidistant:
						_imageConditionMemento = new ImageTypeEquiLinearMemento(gl);
						break;
					case CachedImageType.Other:
						_imageConditionMemento = new ImageTypeOtherMemento(gl);
						break;
				}
			}

			// and now draw the image 
			switch (_imageType)
			{
				case CachedImageType.LinearEquidistant:
					{


						double x_rel_left = gl.XAxis.PhysicalVariantToNormal(xColumn[0]);
						double x_rel_right = gl.XAxis.PhysicalVariantToNormal(xColumn[rows - 1]);

						double y_rel_bottom = gl.YAxis.PhysicalVariantToNormal(_yRangeFrom);

						double y_rel_top = gl.YAxis.PhysicalVariantToNormal(_yRangeTo);

						double x0, y0, x1, y1, x2, y2;
						if (gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(x_rel_left, y_rel_top), out x0, out y0) &&
							gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(x_rel_left, y_rel_bottom), out x1, out y1) &&
							gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(x_rel_right, y_rel_top), out x2, out y2)
							)
						{
							/*
							// calculate the parameters of the transformation matrix
							double r0, s0, r1, s1, r2, s2;
							r0 = 0.5;
							s0 = 0.5;
							r1 = 0.5;
							s1 = _cachedImage.Height - 0.5;
							r2 = _cachedImage.Width - 0.5;
							s2 = _cachedImage.Height - 0.5;

							double det = r2 * (s0 - s1) + r0 * (s1 - s2) + r1 * (-s0 + s2);
							double a = (s2 * (-x0 + x1) + s1 * (x0 - x2) + s0 * (-x1 + x2)) / det;
							double b = (s2 * (-y0 + y1) + s1 * (y0 - y2) + s0 * (-y1 + y2)) / det;
							double c = (r2 * (x0 - x1) + r0 * (x1 - x2) + r1 * (-x0 + x2)) / det;
							double d = (r2 * (y0 - y1) + r0 * (y1 - y2) + r1 * (-y0 + y2)) / det;
							double e = (-(r2 * s1 * x0) + r1 * s2 * x0 + r2 * s0 * x1 - r0 * s2 * x1 - r1 * s0 * x2 + r0 * s1 * x2) / det;
							double f = (-(r2 * s1 * y0) + r1 * s2 * y0 + r2 * s0 * y1 - r0 * s2 * y1 - r1 * s0 * y2 + r0 * s1 * y2) / det;

							System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix((float)a, (float)b, (float)c, (float)d, (float)e, (float)f);

							//PointF[] transfor = new PointF[] { new PointF((float)r0, (float)s0), new PointF((float)r1, (float)s1), new PointF((float)r2, (float)s2) };
							//mat.TransformPoints(transfor);

							GraphicsState savedGraphicsState = gfrx.Save();
							*/

							if (this._clipToLayer)
								gfrx.Clip = gl.CoordinateSystem.GetRegion();

							var pts = new PointF[] { new PointF((float)x0, (float)y0), new PointF((float)x2, (float)y2), new PointF((float)x1, (float)y1) };
							gfrx.DrawImage(_cachedImage, pts, new RectangleF(0, 0, _cachedImage.Width - 1, _cachedImage.Height - 1), GraphicsUnit.Pixel);

							/*
							gfrx.MultiplyTransform(mat, MatrixOrder.Prepend);
							gfrx.DrawImage(_cachedImage, 0, 0, _cachedImage.Width, _cachedImage.Height);
							gfrx.Restore(savedGraphicsState);
							*/
						}
					}
					break;
				case CachedImageType.Other:
					{
						gfrx.DrawImage(_cachedImage, 0, 0, (float)gl.Size.X, (float)gl.Size.Y);
					}
					break;
			}
		}
		class ImageTypeEquiLinearMemento
		{
			Type xtype, ytype;
			Type cstype;

			public ImageTypeEquiLinearMemento(IPlotArea gl)
			{
				xtype = gl.XAxis.GetType();
				ytype = gl.YAxis.GetType();
				cstype = gl.CoordinateSystem.GetType();
			}

			public override bool Equals(object obj)
			{
				ImageTypeEquiLinearMemento from = obj as ImageTypeEquiLinearMemento;
				if (from == null)
					return false;
				else
					return
						this.xtype == from.xtype &&
						this.ytype == from.ytype &&
						this.cstype == from.cstype;

			}
		}
		class ImageTypeOtherMemento
		{
			Type xtype, ytype;
			AltaxoVariant xorg, xend, yorg, yend;

			Type cstype;
			double x00, x10, x01, x11, x32;
			double y00, y10, y01, y11, y32;

			public ImageTypeOtherMemento(IPlotArea gl)
			{
				xtype = gl.XAxis.GetType();
				xorg = gl.XAxis.OrgAsVariant;
				xend = gl.XAxis.EndAsVariant;

				ytype = gl.YAxis.GetType();
				yorg = gl.YAxis.OrgAsVariant;
				yend = gl.YAxis.EndAsVariant;

				cstype = gl.CoordinateSystem.GetType();
				gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 0), out x00, out y00);
				gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 0), out x10, out y10);
				gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 1), out x01, out y01);
				gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 1), out x11, out y11);
				gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0.3, 0.2), out x32, out y32);
			}

			public override bool Equals(object obj)
			{
				ImageTypeOtherMemento from = obj as ImageTypeOtherMemento;
				if (from == null)
					return false;
				else
					return
						this.xtype == from.xtype &&
						this.ytype == from.ytype &&
						this.xorg == from.xorg &&
						this.xend == from.xend &&
						this.yend == from.yend &&
						this.cstype == from.cstype &&
						this.x00 == from.x00 &&
						this.x10 == from.x10 &&
						this.x01 == from.x01 &&
						this.x11 == from.x11 &&
						this.x32 == from.x32 &&
						this.y00 == from.y00 &&
						this.y10 == from.y10 &&
						this.y01 == from.y01 &&
						this.y11 == from.y11 &&
						this.y32 == from.y32;

			}
		}


		void BuildImage(Graphics gfrx, IPlotArea gl, XYZMeshedColumnPlotData myPlotAssociation, int cols, int rows)
		{
			List<double> lx = new List<double>(rows);
			List<double> ly = new List<double>(cols);
			List<INumericColumn> vcolumns = new List<INumericColumn>(cols);

			Altaxo.Data.IReadableColumn xColumn = myPlotAssociation.XColumn;
			Altaxo.Data.IReadableColumn yColumn = myPlotAssociation.YColumn;

			// now we sort out only those lx and ly values, which are valid
			Altaxo.Collections.AscendingIntegerCollection validX = new Altaxo.Collections.AscendingIntegerCollection();
			Altaxo.Collections.AscendingIntegerCollection validY = new Altaxo.Collections.AscendingIntegerCollection();


			// build the logical values of x and y
			for (int i = 0; i < rows; i++)
			{
				double val = gl.XAxis.PhysicalVariantToNormal(xColumn[i]);
				if (!double.IsNaN(val) && !double.IsInfinity(val))
				{
					validX.Add(i);
					lx.Add(val);
				}
			}
			// now y is more hard, we had to find out the column number of the column associated
			// we have also to maintain the physical values of y of the first and the last column
			bool isFirstY = true;
			for (int i = 0; i < cols; i++)
			{
				int nColIdx = i;
				Altaxo.Data.IReadableColumn rcol = myPlotAssociation.GetDataColumn(i);
				Altaxo.Data.DataColumn dcol = rcol as Altaxo.Data.DataColumn;
				if (null != dcol)
				{
					DataColumnCollection parentcoll = DataColumnCollection.GetParentDataColumnCollectionOf(dcol);
					if (parentcoll != null)
						nColIdx = parentcoll.GetColumnNumber(dcol);
				}

				if (isFirstY)
				{
					isFirstY = false;
					_yRangeFrom = yColumn[nColIdx];
				}
				else
				{
					_yRangeTo = yColumn[nColIdx];
				}

				double val = gl.YAxis.PhysicalVariantToNormal(yColumn[nColIdx]);

				if (!double.IsNaN(val) && !double.IsInfinity(val) && (rcol is INumericColumn))
				{
					validY.Add(i);
					ly.Add(val);
					vcolumns.Add(rcol as INumericColumn);
				}
			}

			// ---------------- prepare the color scaling -------------------------------------

			NumericalBoundaries pb = _scale.DataBounds;
			myPlotAssociation.SetVBoundsFromTemplate(pb); // ensure that the right v-boundary type is set
			myPlotAssociation.MergeVBoundsInto(pb);

			// --------------- end preparation of color scaling ------------------------------


			// test if the coordinate system is affine and the scale is linear
			if (!gl.CoordinateSystem.IsAffine)
			{
				BuildImageV3(gfrx, gl, lx.ToArray(), ly.ToArray(), vcolumns.ToArray());
			}
			else // Coordinate System is affine
			{
				// now test lx and ly (only the valid indices for equidistantness
				bool isEquististantX = IsEquidistant(lx, 0.2);
				bool isEquististantY = IsEquidistant(ly, 0.2);

				bool areLinearScales = (gl.XAxis is LinearScale) && (gl.YAxis is LinearScale);

				if (areLinearScales && isEquististantX && isEquististantY)
				{
					BuildImageV1(myPlotAssociation, cols, rows); // affine, linear scales, and equidistant points
				}
				else if (areLinearScales)
				{
					BuildImageV3(gfrx, gl, lx.ToArray(), ly.ToArray(), vcolumns.ToArray()); // affine and linear, but nonequidistant scale
					//BuildImageV2(); // affine and linear scales, but not equidistant
				}
				else
				{
					BuildImageV3(gfrx, gl, lx.ToArray(), ly.ToArray(), vcolumns.ToArray()); // affine, but nonlinear scales
				}
			}


		}


		static bool IsEquidistant(double[] x, Altaxo.Collections.IAscendingIntegerCollection indices, double relthreshold)
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

		static bool IsEquidistant(IList<double> x, double relthreshold)
		{
			int NM1 = x.Count - 1;
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

		Color GetColor(double val)
		{
			double relval = _scale.PhysicalToNormal(val);
			return _colorProvider.GetColor(relval);
		}



		// CoordinateSystem is not affine, or scales are non-linear
		void BuildImageV3(Graphics gfrx, IPlotArea gl,
			double[] lx,
			double[] ly,
			INumericColumn[] vcolumns)
		{
			// allocate a bitmap of same dimensions than the underlying layer
			_imageType = CachedImageType.Other;

			int dimX = (int)Math.Ceiling(gl.Size.X / 72.0 * gfrx.DpiX);
			int dimY = (int)Math.Ceiling(gl.Size.Y / 72.0 * gfrx.DpiY);

			dimX = Math.Min(2048, dimX);
			dimY = Math.Min(2048, dimY);

			// look if the image has the right dimensions
			if (null == _cachedImage || _cachedImage.Width != dimX || _cachedImage.Height != dimY)
			{
				if (null != _cachedImage)
					_cachedImage.Dispose();

				// please notice: the horizontal direction of the image is related to the row index!!! (this will turn the image in relation to the table)
				// and the vertical direction of the image is related to the column index
				_cachedImage = new System.Drawing.Bitmap(dimX, dimY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			}

			byte[] imageBytes = new byte[dimX * dimY * 4];


			double widthByDimX = gl.Size.X / dimX;
			double heightByDimY = gl.Size.Y / dimY;
			Logical3D rel = new Logical3D();

			double minRX = lx[0];
			double maxRX = lx[lx.Length - 1];
			double minRY = ly[0];
			double maxRY = ly[ly.Length - 1];

			if (minRX > maxRX)
			{
				double h = minRX;
				minRX = maxRX;
				maxRX = h;
			}
			if (minRY > maxRY)
			{
				double h = minRY;
				minRY = maxRY;
				maxRY = h;
			}

			BivariateLinearSpline interpol = new BivariateLinearSpline(
				VectorMath.ToROVector(lx),
				VectorMath.ToROVector(ly),
				DataTableWrapper.ToROColumnMatrix(vcolumns, new Altaxo.Collections.ContiguousIntegerRange(0, lx.Length)));

			Color colorInvalid = _colorProvider.GetColor(double.NaN);

			int addr = 0;
			for (int ny = 0; ny < dimY; ny++)
			{
				double py = (ny + 0.5) * heightByDimY;

				for (int nx = 0; nx < dimX; nx++, addr += 4)
				{
					double px = (nx + 0.5) * widthByDimX;


					if (false == gl.CoordinateSystem.LayerToLogicalCoordinates(px, py, out rel))
					{
						_cachedImage.SetPixel(nx, ny, colorInvalid);
					}
					else // conversion to relative coordinates was possible
					{
						double rx = rel.RX;
						double ry = rel.RY;

						if (rx < minRX || rx > maxRX || ry < minRY || ry > maxRY)
						{
							//_cachedImage.SetPixel(nx, ny, _colorInvalid);
							imageBytes[addr + 0] = colorInvalid.B;
							imageBytes[addr + 1] = colorInvalid.G;
							imageBytes[addr + 2] = colorInvalid.R;
							imageBytes[addr + 3] = colorInvalid.A;
						}
						else
						{
							double val = interpol.Interpolate(rx, ry);
							if (double.IsNaN(val))
							{
								//_cachedImage.SetPixel(nx, ny, _colorInvalid);
								imageBytes[addr + 0] = colorInvalid.B;
								imageBytes[addr + 1] = colorInvalid.G;
								imageBytes[addr + 2] = colorInvalid.R;
								imageBytes[addr + 3] = colorInvalid.A;

							}
							else
							{
								//_cachedImage.SetPixel(nx, ny, GetColor(val));
								Color c = GetColor(val);
								imageBytes[addr + 0] = c.B;
								imageBytes[addr + 1] = c.G;
								imageBytes[addr + 2] = c.R;
								imageBytes[addr + 3] = c.A;
							}
						}

					}
				}
			}

			// Lock the bitmap's bits.  
			Rectangle rect = new Rectangle(0, 0, _cachedImage.Width, _cachedImage.Height);
			System.Drawing.Imaging.BitmapData bmpData =
					_cachedImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly,
					_cachedImage.PixelFormat);

			// Get the address of the first line.
			IntPtr ptr = bmpData.Scan0;

			// Copy the RGB values back to the bitmap
			System.Runtime.InteropServices.Marshal.Copy(imageBytes, 0, ptr, imageBytes.Length);

			_cachedImage.UnlockBits(bmpData);

		}
		void BuildImageV1(XYZMeshedColumnPlotData myPlotAssociation, int cols, int rows)
		{
			_imageType = CachedImageType.LinearEquidistant;
			// look if the image has the right dimensions
			if (null != _cachedImage && (_cachedImage.Width != cols || _cachedImage.Height != rows))
			{
				_cachedImage.Dispose();
				_cachedImage = null;
			}

			GetPixelwiseImage(myPlotAssociation, cols, rows, ref _cachedImage);
		}

		/// <summary>
		/// Gets a pixelwise image of the data. Horizontal or vertical axes are not taken into accout.
		/// The horizontal dimension of the image is associated with the columns of the data table. The
		/// vertical dimension of the image is associated with the rows of the data table.
		/// </summary>
		/// <param name="myPlotAssociation">The data to plot.</param>
		/// <param name="cols">Number of columns (horizontal pixels) to plot.</param>
		/// <param name="rows">Number of rows (vertical pixels) to plot.</param>
		/// <param name="image">Bitmap to fill with the plot image. If null, a new image is created.</param>
		/// <exception cref="ArgumentException">An exception will be thrown if the provided image is smaller than the required dimensions.</exception>
		public void GetPixelwiseImage(XYZMeshedColumnPlotData myPlotAssociation, int cols, int rows, ref System.Drawing.Bitmap image)
		{
			// look if the image has the right dimensions

			if (null != image && (image.Width < cols || image.Height < rows))
				throw new ArgumentException("The provided image is smaller than required");

			if (null == image)
			{
				// please notice: the horizontal direction of the image is related to the row index!!! (this will turn the image in relation to the table)
				// and the vertical direction of the image is related to the column index
				image = new System.Drawing.Bitmap(rows, cols, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			}

			// now we can fill the image with our data
			for (int i = 0; i < cols; i++)
			{
				Altaxo.Data.INumericColumn col = myPlotAssociation.GetDataColumn(i) as Altaxo.Data.INumericColumn;
				if (null == col)
					continue;

				for (int j = 0; j < rows; j++)
				{
					image.SetPixel(j, cols - i - 1, GetColor(col[j]));
				} // for all pixel of a column
			} // for all columns
		}

		void EhColorProviderChanged(object sender, EventArgs e)
		{
			this._imageType = CachedImageType.None;
			OnChanged();
		}

		void EhScaleChanged(object sender, EventArgs e)
		{
			this._imageType = CachedImageType.None;
			OnChanged();
		}

		#region IChangedEventSource Members


		protected virtual void OnChanged()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);
			if (null != Changed)
				Changed(this, new EventArgs());
		}

		#endregion

		public virtual object ParentObject
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public virtual string Name
		{
			get
			{
				Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
				return null == noc ? null : noc.GetNameOfChildObject(this);
			}
		}


	} // end of class DensityImagePlotStyle
}
