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

#endregion Copyright

using Altaxo.Calc;
using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	using Altaxo.Graph.Scales.Rescaling;

	/// <summary>
	/// Tick settings for a Probability scale.
	/// </summary>
	public class CumulativeProbabilityTickSpacing : NumericTickSpacing
	{
		private static readonly double[] _majorSpanValues = new double[] { 0.0002, 0.0005, 0.001, 0.002, 0.005, 0.01, 0.02, 0.03, 0.04, 0.05, 0.1, 0.2, 0.3, 0.4, 0.5 };
		private static readonly int[] _minorSpanTicks = new int[] { 3, 5, 5, 3, 5, 5, 5, 2, 2, 5, 10, 10, 10, 10, 10, 10 };

		/// <summary>Maximum allowed number of ticks in case manual tick input will produce a big amount of ticks.</summary>
		protected static readonly int _maxSafeNumberOfTicks = 10000;

		private double _orgGrace = 1 / 16.0;
		private double _endGrace = 1 / 16.0;

		private int _targetNumberOfMajorTicks = 4;
		private int _targetNumberOfMinorTicks = 2;

		private double _transformationDivider = 1;
		private bool _transformationOperationIsMultiply;

		/// <summary>If true, the boundaries will be set on a minor or major tick.</summary>
		private BoundaryTickSnapping _snapOrgToTick;

		private BoundaryTickSnapping _snapEndToTick;

		private SuppressedTicks _suppressedMajorTicks;
		private SuppressedTicks _suppressedMinorTicks;
		private AdditionalTicks _additionalMajorTicks;
		private AdditionalTicks _additionalMinorTicks;

		// Results
		private List<double> _majorTicks;

		private List<double> _minorTicks;

		// Cached values
		private class CachedMajorMinor : ICloneable
		{
			public double Org, End;
			/// <summary>Physical span value between two major ticks.</summary>

			public CachedMajorMinor(double org, double end)
			{
				Org = org;
				End = end;
			}

			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}

		private CachedMajorMinor _cachedMajorMinor;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CumulativeProbabilityTickSpacing), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				CumulativeProbabilityTickSpacing s = (CumulativeProbabilityTickSpacing)obj;

				info.AddValue("MinGrace", s._orgGrace);
				info.AddValue("MaxGrace", s._endGrace);
				info.AddEnum("SnapOrgToTick", s._snapOrgToTick);
				info.AddEnum("SnapEndToTick", s._snapEndToTick);

				info.AddValue("TargetNumberOfMajorTicks", s._targetNumberOfMajorTicks);
				info.AddValue("TargetNumberOfMinorTicks", s._targetNumberOfMinorTicks);

				info.AddValue("TransformationDivider", s._transformationDivider);
				info.AddValue("TransformationIsMultiply", s._transformationOperationIsMultiply);

				if (s._suppressedMajorTicks.IsEmpty)
					info.AddValue("SuppressedMajorTicks", (object)null);
				else
					info.AddValue("SuppressedMajorTicks", s._suppressedMajorTicks);

				if (s._suppressedMinorTicks.IsEmpty)
					info.AddValue("SuppressedMinorTicks", (object)null);
				else
					info.AddValue("SuppressedMinorTicks", s._suppressedMinorTicks);

				if (s._additionalMajorTicks.IsEmpty)
					info.AddValue("AdditionalMajorTicks", (object)null);
				else
					info.AddValue("AdditionalMajorTicks", s._additionalMajorTicks);

				if (s._additionalMinorTicks.IsEmpty)
					info.AddValue("AdditionalMinorTicks", (object)null);
				else
					info.AddValue("AdditionalMinorTicks", s._additionalMinorTicks);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				CumulativeProbabilityTickSpacing s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual CumulativeProbabilityTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				CumulativeProbabilityTickSpacing s = null != o ? (CumulativeProbabilityTickSpacing)o : new CumulativeProbabilityTickSpacing();
				s._orgGrace = info.GetDouble("MinGrace");
				s._endGrace = info.GetDouble("MaxGrace");
				s._snapOrgToTick = (BoundaryTickSnapping)info.GetEnum("SnapOrgToTick", typeof(BoundaryTickSnapping));
				s._snapEndToTick = (BoundaryTickSnapping)info.GetEnum("SnapEndToTick", typeof(BoundaryTickSnapping));

				s._targetNumberOfMajorTicks = info.GetInt32("TargetNumberOfMajorTicks");
				s._targetNumberOfMinorTicks = info.GetInt32("TargetNumberOfMinorTicks");

				s._transformationDivider = info.GetDouble("TransformationDivider");
				s._transformationOperationIsMultiply = info.GetBoolean("TransformationIsMultiply");

				s.SuppressedMajorTicks = (SuppressedTicks)info.GetValue("SuppressedMajorTicks", s);
				s.SuppressedMinorTicks = (SuppressedTicks)info.GetValue("SuppressedMinorTicks", s);
				s.AdditionalMajorTicks = (AdditionalTicks)info.GetValue("AdditionalMajorTicks", s);
				s.AdditionalMinorTicks = (AdditionalTicks)info.GetValue("AdditionalMinorTicks", s);

				return s;
			}
		}

		#endregion Serialization

		public CumulativeProbabilityTickSpacing()
		{
			_majorTicks = new List<double>();
			_minorTicks = new List<double>();
			_suppressedMajorTicks = new SuppressedTicks() { ParentObject = this };
			_suppressedMinorTicks = new SuppressedTicks() { ParentObject = this };
			_additionalMajorTicks = new AdditionalTicks() { ParentObject = this };
			_additionalMinorTicks = new AdditionalTicks() { ParentObject = this };
		}

		public CumulativeProbabilityTickSpacing(CumulativeProbabilityTickSpacing from)
			: base(from) // everything is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as CumulativeProbabilityTickSpacing;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				CopyHelper.Copy(ref _cachedMajorMinor, from._cachedMajorMinor);

				_targetNumberOfMajorTicks = from._targetNumberOfMajorTicks;
				_targetNumberOfMinorTicks = from._targetNumberOfMinorTicks;

				_orgGrace = from._orgGrace;
				_endGrace = from._endGrace;

				_snapOrgToTick = from._snapOrgToTick;
				_snapEndToTick = from._snapEndToTick;

				ChildCopyToMember(ref _suppressedMajorTicks, from._suppressedMajorTicks);
				ChildCopyToMember(ref _suppressedMinorTicks, from._suppressedMinorTicks);
				ChildCopyToMember(ref _additionalMajorTicks, from._additionalMajorTicks);
				ChildCopyToMember(ref _additionalMinorTicks, from._additionalMinorTicks);

				_transformationDivider = from._transformationDivider;
				_transformationOperationIsMultiply = from._transformationOperationIsMultiply;

				_majorTicks = new List<double>(from._majorTicks);
				_minorTicks = new List<double>(from._minorTicks);

				EhSelfChanged();
				suspendToken.Resume();
			}

			return true;
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _suppressedMajorTicks)
				yield return new Main.DocumentNodeAndName(_suppressedMajorTicks, "SuppressedMajorTicks");
			if (null != _suppressedMinorTicks)
				yield return new Main.DocumentNodeAndName(_suppressedMinorTicks, "SuppressedMinorTicks");

			if (null != _additionalMajorTicks)
				yield return new Main.DocumentNodeAndName(_additionalMajorTicks, "AdditionalMajorTicks");
			if (null != _additionalMinorTicks)
				yield return new Main.DocumentNodeAndName(_additionalMinorTicks, "AdditionalMinorTicks");
		}

		public override object Clone()
		{
			return new CumulativeProbabilityTickSpacing(this);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			else if (!(obj is CumulativeProbabilityTickSpacing))
				return false;
			else
			{
				var from = (CumulativeProbabilityTickSpacing)obj;

				if (_targetNumberOfMajorTicks != from._targetNumberOfMajorTicks)
					return false;
				if (_targetNumberOfMinorTicks != from._targetNumberOfMinorTicks)
					return false;

				if (_orgGrace != from._orgGrace)
					return false;
				if (_endGrace != from._endGrace)
					return false;

				if (_snapOrgToTick != from._snapOrgToTick)
					return false;
				if (_snapEndToTick != from._snapEndToTick)
					return false;

				if (_transformationDivider != from._transformationDivider)
					return false;
				if (_transformationOperationIsMultiply != from._transformationOperationIsMultiply)
					return false;

				if (!_suppressedMajorTicks.Equals(from._suppressedMajorTicks))
					return false;

				if (!_suppressedMinorTicks.Equals(from._suppressedMinorTicks))
					return false;

				if (!_additionalMajorTicks.Equals(from._additionalMajorTicks))
					return false;

				if (!_additionalMinorTicks.Equals(from._additionalMinorTicks))
					return false;
			}

			return true;
		}

		#region User parameters

		/// <summary>
		/// Gets or sets the origin grace. This is a value (0..1) relative to the span of the scale, that designates how far
		/// the origin of the scale is extended in Auto rescale mode.
		/// </summary>
		/// <value>
		/// The origin grace.
		/// </value>
		public double OrgGrace
		{
			get
			{
				return _orgGrace;
			}
			set
			{
				var oldValue = _orgGrace;
				_orgGrace = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		/// <summary>
		/// Gets or sets the end grace. This is a value (0..1) relative to the span of the scale, that designates how far
		/// the end of the scale is extended in Auto rescale mode.
		/// </summary>
		/// <value>
		/// The origin grace.
		/// </value>
		public double EndGrace
		{
			get
			{
				return _endGrace;
			}
			set
			{
				var oldValue = _endGrace;
				_endGrace = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public BoundaryTickSnapping SnapOrgToTick
		{
			get
			{
				return _snapOrgToTick;
			}
			set
			{
				var oldValue = _snapOrgToTick;
				_snapOrgToTick = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public BoundaryTickSnapping SnapEndToTick
		{
			get
			{
				return _snapEndToTick;
			}
			set
			{
				var oldValue = _snapEndToTick;
				_snapEndToTick = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public int TargetNumberOfMajorTicks
		{
			get
			{
				return _targetNumberOfMajorTicks;
			}
			set
			{
				var oldValue = _targetNumberOfMajorTicks;
				_targetNumberOfMajorTicks = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public int TargetNumberOfMinorTicks
		{
			get
			{
				return _targetNumberOfMinorTicks;
			}
			set
			{
				var oldValue = _targetNumberOfMinorTicks;
				_targetNumberOfMinorTicks = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public double TransformationDivider
		{
			get
			{
				return _transformationDivider;
			}
			set
			{
				var oldValue = _transformationDivider;
				_transformationDivider = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public bool TransformationOperationIsMultiply
		{
			get
			{
				return _transformationOperationIsMultiply;
			}
			set
			{
				var oldValue = _transformationOperationIsMultiply;
				_transformationOperationIsMultiply = value;
				if (oldValue != value)
					EhSelfChanged();
			}
		}

		public SuppressedTicks SuppressedMajorTicks
		{
			get
			{
				return _suppressedMajorTicks;
			}
			protected set
			{
				_suppressedMajorTicks = value ?? new SuppressedTicks();
				_suppressedMajorTicks.ParentObject = this;
			}
		}

		public SuppressedTicks SuppressedMinorTicks
		{
			get
			{
				return _suppressedMinorTicks;
			}
			protected set
			{
				_suppressedMinorTicks = value ?? new SuppressedTicks();
				_suppressedMinorTicks.ParentObject = this;
			}
		}

		public AdditionalTicks AdditionalMajorTicks
		{
			get
			{
				return _additionalMajorTicks;
			}
			protected set
			{
				_additionalMajorTicks = value ?? new AdditionalTicks();
				_additionalMajorTicks.ParentObject = this;
			}
		}

		public AdditionalTicks AdditionalMinorTicks
		{
			get
			{
				return _additionalMinorTicks;
			}
			protected set
			{
				_additionalMinorTicks = value ?? new AdditionalTicks();
				_additionalMinorTicks.ParentObject = this;
			}
		}

		#endregion User parameters

		private double TransformOriginalToModified(double x)
		{
			if (_transformationOperationIsMultiply)
				return x * _transformationDivider;
			else
				return x / _transformationDivider;
		}

		private double TransformModifiedToOriginal(double y)
		{
			if (_transformationOperationIsMultiply)
				return (y) / _transformationDivider;
			else
				return (y) * _transformationDivider;
		}

		/// <summary>
		/// GetMajorTicks returns the physical values
		/// at which major ticks should occur
		/// </summary>
		/// <returns>physical values for the major ticks</returns>
		public override double[] GetMajorTicks()
		{
			return _majorTicks.ToArray();
		}

		/// <summary>
		/// GetMinorTicks returns the physical values
		/// at which minor ticks should occur
		/// </summary>
		/// <returns>physical values for the minor ticks</returns>
		public override double[] GetMinorTicks()
		{
			return _minorTicks.ToArray();
		}

		public override double[] GetMajorTicksNormal(Scale scale)
		{
			double[] r = new double[_majorTicks.Count];
			for (int i = 0; i < r.Length; i++)
				r[i] = scale.PhysicalVariantToNormal(TransformModifiedToOriginal(_majorTicks[i]));

			return r;
		}

		public override double[] GetMinorTicksNormal(Scale scale)
		{
			double[] r = new double[_minorTicks.Count];
			for (int i = 0; i < r.Length; i++)
				r[i] = scale.PhysicalVariantToNormal(TransformModifiedToOriginal(_minorTicks[i]));

			return r;
		}

		/// <summary>
		/// Decides giving a raw org and end value, whether or not the scale boundaries should be extended to
		/// have more 'nice' values. If the boundaries should be changed, the function return true, and the
		/// org and end argument contain the proposed new scale boundaries.
		/// </summary>
		/// <param name="org">Raw scale org.</param>
		/// <param name="end">Raw scale end.</param>
		/// <param name="isOrgExtendable">True when the org is allowed to be extended.</param>
		/// <param name="isEndExtendable">True when the scale end can be extended.</param>
		/// <returns>True when org or end are changed. False otherwise.</returns>
		public override bool PreProcessScaleBoundaries(ref AltaxoVariant org, ref AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
		{
			double dorg = (double)org;
			double dend = (double)end;

			if (InternalPreProcessScaleBoundaries(ref dorg, ref dend, isOrgExtendable, isEndExtendable))
			{
				org = dorg;
				end = dend;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Calculates the ticks based on the org and end of the scale.
		/// </summary>
		/// <param name="org">Scale origin.</param>
		/// <param name="end">Scale end.</param>
		/// <param name="scale">The underlying scale.</param>
		public override void FinalProcessScaleBoundaries(AltaxoVariant org, AltaxoVariant end, Scale scale)
		{
			double dorg = (double)org;
			double dend = (double)end;

			if (_cachedMajorMinor == null || _cachedMajorMinor.Org != dorg || _cachedMajorMinor.End != dend)
			{
				InternalPreProcessScaleBoundaries(ref dorg, ref dend, false, false); // make sure that _cachedMajorMinor is valid now
			}

			System.Diagnostics.Debug.Assert(null != _cachedMajorMinor);

			_majorTicks.Clear();
			_minorTicks.Clear();
			InternalCalculateMajorTicks();
			InternalCalculateMinorTicks();
		}

		#region Calculation of tick values

		private void InternalCalculateMajorTicks()
		{
			_majorTicks.Clear();

			var org = _cachedMajorMinor.Org;
			var end = _cachedMajorMinor.End;

			foreach (var majVal in _majorSpanValues)
			{
				if (org <= majVal && majVal <= end)
					_majorTicks.Add(TransformOriginalToModified(majVal));
			}
			foreach (var majVal in _majorSpanValues.Reverse().Skip(1))
			{
				if (org <= (1 - majVal) && (1 - majVal) <= end)
					_majorTicks.Add(TransformOriginalToModified(1 - majVal));
			}

			// Remove suppressed ticks
			_suppressedMajorTicks.RemoveSuppressedTicks(_majorTicks);

			// Add additional ticks
			if (!_additionalMajorTicks.IsEmpty)
			{
				foreach (AltaxoVariant v in _additionalMajorTicks.Values)
				{
					_majorTicks.Add(v);
				}
			}
		}

		private void InternalCalculateMinorTicks()
		{
			_minorTicks.Clear();

			var org = _cachedMajorMinor.Org;
			var end = _cachedMajorMinor.End;

			for (int i = 0; i < _majorSpanValues.Length - 1; ++i)
			{
				double majTick = _majorSpanValues[i];
				int numMinTicks = _minorSpanTicks[i];
				var nextMajorTick = _majorSpanValues[i + 1];

				for (int j = 1; j < numMinTicks; ++j)
				{
					var tickVal = majTick + j * (nextMajorTick - majTick) / numMinTicks;

					if (org <= tickVal && tickVal <= end)
						_minorTicks.Add(TransformOriginalToModified(tickVal));
				}
			}

			for (int i = _majorSpanValues.Length - 1; i > 0; --i)
			{
				double majTick = 1 - _majorSpanValues[i];
				double nextMajorTick = 1 - _majorSpanValues[i - 1];
				int numMinTicks = _minorSpanTicks[i];
				for (int j = 1; j < numMinTicks; ++j)
				{
					var tickVal = majTick + j * (nextMajorTick - majTick) / numMinTicks;

					if (org <= tickVal && tickVal <= end)
						_minorTicks.Add(TransformOriginalToModified(tickVal));
				}
			}

			// Remove suppressed ticks
			_suppressedMinorTicks.RemoveSuppressedTicks(_minorTicks);

			// add additional ticks
			if (!_additionalMinorTicks.IsEmpty)
			{
				foreach (AltaxoVariant v in _additionalMinorTicks.Values)
				{
					_minorTicks.Add(v);
				}
			}
		}

		#endregion Calculation of tick values

		#region Functions to predict change of scale by tick snapping, grace, and OneLever

		private bool InternalPreProcessScaleBoundaries(ref double xorg, ref double xend, bool isOrgExtendable, bool isEndExtendable)
		{
			_cachedMajorMinor = null;
			bool modified = false;

			if (xorg <= 0)
			{
				xorg = CumulativeProbabilityScaleRescaleConditions.DefaultOrgValue;
				modified = true;
			}
			else if (xorg >= 1)
			{
				xorg = CumulativeProbabilityScaleRescaleConditions.DefaultEndValue;
				modified = true;
			}

			if (xend <= 0)
			{
				xend = CumulativeProbabilityScaleRescaleConditions.DefaultOrgValue;
				modified = true;
			}
			else if (xend >= 1)
			{
				xend = CumulativeProbabilityScaleRescaleConditions.DefaultEndValue;
				modified = true;
			}

			if (xend == xorg)
			{
				double h = Math.Abs(xorg / 2);
				xorg -= h;
				xend += h;
				modified = true;
			}

			if (!(xorg <= xend))
			{
				var h = xorg;
				xorg = xend;
				xend = h;
				modified = true;
			}
			// here xorg and xend should both be positive, with xorg < xend

			// try applying Grace and OneLever only ...
			double xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever;
			bool modGraceAndOneLever = GetOrgEndWithGrace(xorg, xend, isOrgExtendable, isEndExtendable, out xOrgWithGraceAndOneLever, out xEndWithGraceAndOneLever);

			// try applying tick snapping only (without Grace and OneLever)
			double xOrgWithTickSnapping, xEndWithTickSnapping;
			bool modTickSnapping = GetOrgEndWithTickSnappingOnly(xend - xorg, xorg, xend, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping);

			// now compare the two
			if (xOrgWithTickSnapping <= xOrgWithGraceAndOneLever && xEndWithTickSnapping >= xEndWithGraceAndOneLever)
			{
				// then there is no need to apply Grace and OneLever
				modified |= modTickSnapping;
			}
			else
			{
				modified |= modGraceAndOneLever;
				modified |= GetOrgEndWithTickSnappingOnly(xEndWithGraceAndOneLever - xOrgWithGraceAndOneLever, xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping);
			}

			xorg = xOrgWithTickSnapping;
			xend = xEndWithTickSnapping;

			_cachedMajorMinor = new CachedMajorMinor(xOrgWithTickSnapping, xEndWithTickSnapping);

			return modified; ;
		}

		/// <summary>
		/// Applies the value for <see cref="OrgGrace"/>, <see cref="EndGrace"/> to the scale and calculated proposed values for the boundaries.
		/// </summary>
		/// <param name="scaleOrg">Scale origin.</param>
		/// <param name="scaleEnd">Scale end.</param>
		/// <param name="isOrgExtendable">True if the scale org can be extended.</param>
		/// <param name="isEndExtendable">True if the scale end can be extended.</param>
		/// <param name="propOrg">Returns the proposed value of the scale origin.</param>
		/// <param name="propEnd">Returns the proposed value of the scale end.</param>
		public bool GetOrgEndWithGrace(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
		{
			double SquareRootOf2 = Math.Sqrt(2);
			var modified = false;

			propOrg = scaleOrg;
			propEnd = scaleEnd;

			double quantOrg = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * scaleOrg);
			double quantEnd = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * scaleEnd);

			if (isOrgExtendable && OrgGrace > 0)
			{
				double propQuantOrg = quantOrg - OrgGrace * (quantEnd - quantOrg);
				propOrg = 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(propQuantOrg / SquareRootOf2));
				modified = true;
			}

			if (isEndExtendable && EndGrace > 0)
			{
				double propQuantEnd = quantEnd + EndGrace * (quantEnd - quantOrg);
				propEnd = 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(propQuantEnd / SquareRootOf2));
				modified = true;
			}

			return modified;
		}

		/// <summary>Applies the tick snapping settings to the scale origin and scale end. This is done by a determination of the number of decades per major tick and the minor ticks per major tick interval.
		/// Then, the snapping values are applied, and the org and end values of the scale are adjusted (if allowed so).</summary>
		/// <param name="overriddenScaleSpan">The overriden scale span.</param>
		/// <param name="scaleOrg">The scale origin.</param>
		/// <param name="scaleEnd">The scale end.</param>
		/// <param name="isOrgExtendable">If set to <c>true</c>, it is allowed to adjust the scale org value.</param>
		/// <param name="isEndExtendable">if set to <c>true</c>, it is allowed to adjust the scale end value.</param>
		/// <param name="propOrg">The adjusted scale orgin.</param>
		/// <param name="propEnd">The adjusted scale end.</param>
		/// <returns>True if at least either org or end were adjusted to a new value.</returns>
		private bool GetOrgEndWithTickSnappingOnly(double overriddenScaleSpan, double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
		{
			bool modified = false;
			propOrg = scaleOrg;
			propEnd = scaleEnd;

			return modified;
		}

		/// <summary>
		/// Adjusts the parameter <paramref name="x"/> so that <paramref name="x"/> snaps to a tick according to the setting of <paramref name="snapping"/>.
		/// </summary>
		/// <param name="x">The boundary value to adjust.</param>
		/// <param name="majorSpan">Value of the major tick span.</param>
		/// <param name="minorTicks">Number of minor ticks.</param>
		/// <param name="snapping">Setting of the tick snapping.</param>
		/// <param name="upwards">If true, the value is towards higher values, if false it is adjusted towards smaller values.</param>
		/// <returns>The adjusted value of x.</returns>
		public static double GetOrgOrEndSnappedToTick(double x, double majorSpan, int minorTicks, BoundaryTickSnapping snapping, bool upwards)
		{
			switch (snapping)
			{
				default:
				case BoundaryTickSnapping.SnapToNothing:
					{
						return x;
					}
				case BoundaryTickSnapping.SnapToMajorOnly:
					{
						double rel = x / majorSpan;
						if (upwards)
							return Math.Ceiling(rel) * majorSpan;
						else
							return Math.Floor(rel) * majorSpan;
					}
				case BoundaryTickSnapping.SnapToMinorOnly:
					{
						double rel = x * minorTicks / (majorSpan);
						if (upwards)
							rel = Math.Ceiling(rel);
						else
							rel = Math.Floor(rel);

						if (Math.IEEERemainder(rel, 1) != 0 && minorTicks > 1)
							rel = upwards ? rel + 1 : rel - 1;

						return rel * majorSpan / minorTicks;
					}
				case BoundaryTickSnapping.SnapToMinorOrMajor:
					{
						double rel = x * minorTicks / (majorSpan);
						if (upwards)
							return Math.Ceiling(rel) * majorSpan / minorTicks;
						else
							return Math.Floor(rel) * majorSpan / minorTicks;
					}
			}
		}

		/// <summary>
		/// Calculates the major span from the scale span, taking into account the setting for targetMajorTicks.
		/// </summary>
		/// <param name="scaleSpan">Scale span (end-origin).</param>
		/// <param name="targetNumberOfMajorTicks">Target number of major ticks.</param>
		public static double CalculateMajorSpan(double scaleSpan, int targetNumberOfMajorTicks)
		{
			if (!(scaleSpan > 0))
				throw new ArgumentOutOfRangeException("scaleSpan must be >0");

			double rawMajorSpan = targetNumberOfMajorTicks >= 1 ? scaleSpan / targetNumberOfMajorTicks : scaleSpan;
			int log10RawMajorSpan = (int)Math.Floor(Math.Log10(rawMajorSpan));

			double normMajorSpan = rawMajorSpan / RMath.Pow(10, log10RawMajorSpan); // number between 1 and 10
			foreach (double span in _majorSpanValues)
			{
				if (span >= normMajorSpan)
				{
					normMajorSpan = span;
					break;
				}
			}
			return normMajorSpan * RMath.Pow(10, log10RawMajorSpan);
		}

		#endregion Functions to predict change of scale by tick snapping, grace, and OneLever
	}
}