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
using System.Drawing;
using System.Drawing.Drawing2D;

using Altaxo.Serialization;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Scales;


namespace Altaxo.Graph.Gdi.Shapes
{
	[Serializable]
	public class FloatingScale : ClosedPathShapeBase
	{
		#region Inner classes

		class PartialScale : Scale
		{
			Scale _underlyingScale;

			double _logicalOrg;
			double _logicalEnd;

			public PartialScale(Scale underlyingScale, double orgAsNormal, double endAsNormal)
			{
				if (null == underlyingScale)
					throw new ArgumentNullException("underlyingScale");
				_underlyingScale = underlyingScale;

				_logicalOrg = orgAsNormal;
				_logicalEnd = endAsNormal;
			}


			#region Scale implementation

			public override object Clone()
			{
				var result = new PartialScale(_underlyingScale,_logicalOrg,_logicalEnd);
				return result;
			}

			public override double PhysicalVariantToNormal(Data.AltaxoVariant x)
			{
				var n = _underlyingScale.PhysicalVariantToNormal(x);
				return (n - _logicalOrg) / (_logicalEnd - _logicalOrg);
			}

			public override Data.AltaxoVariant NormalToPhysicalVariant(double x)
			{
				var r = (1 - x) * _logicalOrg + x * _logicalEnd;
				return _underlyingScale.NormalToPhysicalVariant(r);
			}

			public override object RescalingObject
			{
				get { return _underlyingScale.RescalingObject; }
			}

			public override Scales.Boundaries.IPhysicalBoundaries DataBoundsObject
			{
				get { return _underlyingScale.DataBoundsObject; }
			}

			public override Data.AltaxoVariant OrgAsVariant
			{
				get { return _underlyingScale.NormalToPhysicalVariant(_logicalOrg); }
			}

			public override Data.AltaxoVariant EndAsVariant
			{
				get { return _underlyingScale.NormalToPhysicalVariant(_logicalEnd); }
			}

			public override bool IsOrgExtendable
			{
				get { return false; }
			}

			public override bool IsEndExtendable
			{
				get { return false; }
			}

			public override string SetScaleOrgEnd(Data.AltaxoVariant org, Data.AltaxoVariant end)
			{
				_logicalOrg = _underlyingScale.PhysicalVariantToNormal(org);
				_logicalEnd = _underlyingScale.PhysicalVariantToNormal(end);
				return null;
			}

			public override void Rescale()
			{

			}
			#endregion
		}

		#endregion


		/// <summary>The span this scale should show. It is either a physical or a logical value, depending on <see cref="_scaleSpanIsPhysical"/>.</summary>
		double _scaleSpan = 0.25;

		/// <summary>If true, the _scaleSpan is interpreted as a physical value. Otherwise, it is a logical span.</summary>
		bool _scaleSpanIsPhysical = false;

		/// <summary>Number of the scale to measure (0: x-axis, 1: y-axis, 2: z-axis).</summary>
		int _scaleNumber = 0;

		/// <summary>Side at which the label should appear.</summary>
		CSAxisSide _labelSide = CSAxisSide.FirstUp;

		/// <summary>Style of the label.</summary>
		AxisLabelStyle _labelStyle = new AxisLabelStyle();

		// Cached members
		/// <summary>Cached path of the isoline.</summary>
		GraphicsPath _cachedPath;


		#region Serialization


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FloatingScale), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				FloatingScale s = (FloatingScale)obj;
				info.AddBaseValueEmbedded(s, typeof(FloatingScale).BaseType);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				FloatingScale s = null != o ? (FloatingScale)o : new FloatingScale();
				info.GetBaseValueEmbedded(s, typeof(FloatingScale).BaseType, parent);

				return s;
			}
		}


		#endregion


		#region Constructors
		public FloatingScale()
		{
		}

		public FloatingScale(double lineWidth, NamedColor lineColor)
		{
			this.Pen.Width = (float)lineWidth;
			this.Pen.Color = lineColor;
		}

		public FloatingScale(FloatingScale from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			bool isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as FloatingScale;
				if (null != from)
				{
					_scaleSpan = from._scaleSpan;
					_scaleSpanIsPhysical = from._scaleSpanIsPhysical;
					_scaleNumber = from._scaleNumber;

					_labelSide = from._labelSide;
					_labelStyle = from._labelStyle;
					_cachedPath = null;
				}

			}
			return isCopied;
		}

		#endregion

		public AxisLabelStyle LabelStyle
		{
			get
			{
				return _labelStyle;
			}
			set
			{
				_labelStyle = value;
			}
		}

		public int ScaleNumber
		{
			get
			{
				return _scaleNumber;
			}
			set
			{
				_scaleNumber = value;
			}
		}

		

			public double ScaleSpan
			{
				get
				{
					return _scaleSpan;
				}
				set
				{
					_scaleSpan = value;
				}
			}

			public bool IsScaleSpanPhysicalValue
			{
				get
				{
					return _scaleSpanIsPhysical;
				}
				set
				{
					_scaleSpanIsPhysical = value;
				}
			}

		public override bool AllowNegativeSize
		{
			get
			{
				return true;
			}
		}

		public override bool AutoSize
		{
			get
			{
				return true;
			}
		}

		public override object Clone()
		{
			return new FloatingScale(this);
		}


		public GraphicsPath GetSelectionPath()
		{
			GraphicsPath result;
			if (Pen.Width <= 5)
				result = GetPath(5);
			else
				result = GetPath(Pen.Width);

			var labelPath = _labelStyle.GetSelectionPath();
			result.AddPath(labelPath, false);
			return result;
		}

		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return _cachedPath;
		}

		protected GraphicsPath GetPath(double minWidth)
		{
			GraphicsPath gp = (GraphicsPath)_cachedPath.Clone();

			if (Pen.Width != minWidth)
				gp.Widen(new Pen(Color.Black, (float)minWidth));
			else
				gp.Widen(Pen);

			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			var pt = htd.GetHittedPointInWorldCoord();
			HitTestObjectBase result = null;
			GraphicsPath gp = GetSelectionPath();
			if (gp.IsVisible((PointF)pt))
			{
				result = new GraphicBaseHitTestObject(this);
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		static new bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Line properties", true);
			((FloatingScale)hitted).OnChanged();
			return true;
		}





		public override void Paint(Graphics g, object obj)
		{
			var layer = (XYPlotLayer)obj;

			Logical3D rBegin;
			layer.CoordinateSystem.LayerToLogicalCoordinates(X, Y, out rBegin);

			Logical3D rEnd = rBegin;
			if (_scaleSpanIsPhysical)
			{
				var physValue = layer.Scales[_scaleNumber].Scale.NormalToPhysicalVariant(rBegin[this._scaleNumber]);
				physValue += _scaleSpan; // to be replaced by the scale span
				var logValue = layer.Scales[_scaleNumber].Scale.PhysicalVariantToNormal(physValue);
				rEnd[_scaleNumber] = logValue;
			}
			else // _scaleSpan is a logical value
			{
				rEnd[_scaleNumber] = rBegin[_scaleNumber] + _scaleSpan;
			}

			double relMiddle = 0.5 * (rBegin[_scaleNumber] + rEnd[_scaleNumber]);

			_cachedPath = new GraphicsPath();
			layer.CoordinateSystem.GetIsoline(_cachedPath, rBegin, rEnd);

			Pen.SetEnvironment((RectangleF)_bounds, BrushX.GetEffectiveMaximumResolution(g, Math.Max(_scaleX, _scaleY)));
			g.DrawPath(Pen, _cachedPath);

			Data.AltaxoVariant span;
			if (_scaleSpanIsPhysical)
				span = _scaleSpan;
			else
				span = layer.Scales[_scaleNumber].Scale.NormalToPhysicalVariant(rEnd[this._scaleNumber]) - layer.Scales[_scaleNumber].Scale.NormalToPhysicalVariant(rBegin[this._scaleNumber]);

			var spanTickSpacing = new SpanTickSpacing(span, relMiddle);
			var swt = new Altaxo.Graph.Scales.ScaleWithTicks(layer.Scales[_scaleNumber].Scale, spanTickSpacing);
			CSLineID isolineID = new CSLineID(_scaleNumber, rBegin);
			var axisInfo = new CSAxisInformation(isolineID);
			_labelStyle.Paint(g, layer.CoordinateSystem, swt, axisInfo, 0, false);

			// calculate size information
			RectangleD bounds1 = _cachedPath.GetBounds();
			RectangleD bounds2 = _labelStyle.GetSelectionPath().GetBounds();
			bounds1.ExpandToInclude(bounds2);
			SetSize(bounds1.Width, bounds1.Height, true);
		}

		#region Inner classes

		private class SpanTickSpacing : Altaxo.Graph.Scales.Ticks.TickSpacing
		{
			Data.AltaxoVariant _span;
			double _relTickPosition;

			public SpanTickSpacing()
				: this(1, 0.5)
			{
			}

			public SpanTickSpacing(Data.AltaxoVariant span, double relTickPosition)
			{
				_span = span;
				_relTickPosition = relTickPosition;
			}



			public override object Clone()
			{
				return new SpanTickSpacing(_span, _relTickPosition);
			}

			public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
			{
				return false;
			}

			public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Altaxo.Graph.Scales.Scale scale)
			{
			}

			public override double[] GetMajorTicksNormal(Scale scale)
			{
				return new double[] { _relTickPosition };
			}

			public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
			{
				return new Altaxo.Data.AltaxoVariant[] { _span };
			}

			public override double[] GetMinorTicksNormal(Scale scale)
			{
				return new double[0];
			}

			public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
			{
				return new Altaxo.Data.AltaxoVariant[0];
			}
		}

		class ScaleSegment : Scale
		{
			double _relOrg;
			double _relEnd;
			Scale _underlyingScale;

			public ScaleSegment(Scale underlyingScale, double relOrg, double relEnd)
			{
				_underlyingScale = underlyingScale;
				_relOrg = relOrg;
				_relEnd = relEnd;
			}

			public override object Clone()
			{
				return new ScaleSegment(_underlyingScale, _relOrg, _relEnd);
			}

			public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
			{
				double r = _underlyingScale.PhysicalVariantToNormal(x);
				return (r - _relOrg) / (_relEnd - _relOrg);
			}

			public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
			{
				double r = _relOrg * (1 - x) + _relEnd * x;
				return _underlyingScale.PhysicalVariantToNormal(r);
			}

			public override object RescalingObject
			{
				get { return _underlyingScale.RescalingObject; }
			}

			public override Altaxo.Graph.Scales.Boundaries.IPhysicalBoundaries DataBoundsObject
			{
				get { return _underlyingScale.DataBoundsObject; }
			}

			public override Altaxo.Data.AltaxoVariant OrgAsVariant
			{
				get { return _underlyingScale.NormalToPhysicalVariant(_relOrg); }
			}

			public override Altaxo.Data.AltaxoVariant EndAsVariant
			{
				get { return _underlyingScale.NormalToPhysicalVariant(_relEnd); }
			}

			public override bool IsOrgExtendable
			{
				get { return false; }
			}

			public override bool IsEndExtendable
			{
				get { return false; }
			}

			public override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
			{
				_relOrg = _underlyingScale.PhysicalVariantToNormal(org);
				_relEnd = _underlyingScale.PhysicalVariantToNormal(end);
				return null;
			}

			public override void Rescale()
			{
			}
		}


		#endregion

	} // End Class
} // end Namespace
