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

		/// <summary>The span this scale should show.</summary>
		double _scaleSpan = 0.25;

		/// <summary>If true, the _scaleSpan is interpreted as a physical value. Otherwise, it is a logical span.</summary>
		bool _scaleSpanIsPhysical;

		/// <summary>Number of the scale to measure (0: x-axis, 1: y-axis, 2: z-axis).</summary>
		int _scaleNumber = 0;

		/// <summary>Distance between scale and label (points).</summary>
		float _outerDistance = 10; // Point

		/// <summary>Side at which the label should appear.</summary>
		CSAxisSide _labelSide = CSAxisSide.FirstUp;

		/// <summary>Style of the label.</summary>
		AxisLabelStyle _labelStyle = new AxisLabelStyle();

		// Cached members
		/// <summary>Cached path of the isoline.</summary>
		GraphicsPath _cachedPath;


		#region Serialization

		#region Clipboard serialization

		protected FloatingScale(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		/// <summary>
		/// Serializes LineGraphic. 
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			FloatingScale s = this;
			base.GetObjectData(info, context);
		}
		/// <summary>
		/// Deserializes the LineGraphic Version 0.
		/// </summary>
		/// <param name="obj">The empty SLineGraphic object to deserialize into.</param>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		/// <param name="selector">The deserialization surrogate selector.</param>
		/// <returns>The deserialized LineGraphic.</returns>
		public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			FloatingScale s = (FloatingScale)base.SetObjectData(obj, info, context, selector);
			return s;
		}


		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}
		#endregion

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LineGraphic", 0)]
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
			: base(from)
		{
		}

		#endregion

		public override bool AllowNegativeSize
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

		protected GraphicsPath GetPath(float minWidth)
		{
			GraphicsPath gp = (GraphicsPath)_cachedPath.Clone();

			if (Pen.Width != minWidth)
				gp.Widen(new Pen(Color.Black, minWidth));
			else
				gp.Widen(Pen);

			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			var pt = htd.GetHittedPointInWorldCoord(_transformation);
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

		static bool EhHitDoubleClick(IHitTestObject o)
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
			_labelStyle.Paint(g, layer.CoordinateSystem, swt, axisInfo, _outerDistance, false);
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
