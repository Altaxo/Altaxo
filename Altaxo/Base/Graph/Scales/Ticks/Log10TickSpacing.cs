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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	using Altaxo.Calc;
	using Altaxo.Data;

	public class Log10TickSpacing : NumericTickSpacing
	{
		private static readonly int[] minorTickMantissa9 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		private static readonly int[] minorTickMantissa3 = new int[] { 1, 4, 7 };

		private List<double> _majorTicks;
		private List<double> _minorTicks;

		/// <summary>If set, gives the number of minor ticks choosen by the user.</summary>
		private int? _userDefinedMinorTicks;

		/// <summary>If set, gives the physical value between two major ticks choosen by the user.</summary>
		private int? _userDefinedNumberOfDecadesPerMajorTick;

		private double _oneLever = 0.25;
		private double _orgGrace = 0.05;
		private double _endGrace = 0.05;
		private int _targetNumberOfMajorTicks = 6;
		private int _targetNumberOfMinorTicks = 50;

		private double _transformationDivider = 1;
		private bool _transformationOperationIsMultiply;
		private double _transformationExponent = 1;

		/// <summary>If true, the boundaries will be set on a minor or major tick.</summary>
		private BoundaryTickSnapping _snapOrgToTick = BoundaryTickSnapping.SnapToMinorOrMajor;

		private BoundaryTickSnapping _snapEndToTick = BoundaryTickSnapping.SnapToMinorOrMajor;

		private SuppressedTicks _suppressedMajorTicks;
		private SuppressedTicks _suppressedMinorTicks;
		private AdditionalTicks _additionalMajorTicks;
		private AdditionalTicks _additionalMinorTicks;

		private class CachedMajorMinor : ICloneable
		{
			/// <summary>Cached scale org.</summary>
			public double Org;

			/// <summary>Cached scale end.</summary>
			public double End;

			/// <summary>Number of decades per major tick. Note that major ticks only appear at integer multiples of this value, so that at a value of 1 (10^0) there is always a major tick.</summary>
			public int DecadesPerMajorTick;

			/// <summary>
			/// <para>If the value is positive, the minor ticks will mark full decades only. The value then determines the number of decades per minor tick as (numberOfDecadesPerMajorTick / MinorTicks).</para>
			/// <para>If the value is 0 or 1, no minor ticks will be visible.</para>
			/// <para>If the value is -1, only mantissa values of 1, 4, and 7 are used as minor ticks (the 1 is used only if there is more than one decade per major tick).</para>
			/// <para>If the value is -2, mantissa values of 1, 2 , 3, 4, 5, 6, 7, 8 and 9 are used as minor ticks (the 1 is used only if there is more than one decade per major tick).</para>
			/// </summary>
			public int MinorTicks;

			public CachedMajorMinor(double org, double end, int majorDecadesPerTick, int minor)
			{
				Org = org;
				End = end;
				DecadesPerMajorTick = majorDecadesPerTick;
				MinorTicks = minor;
			}

			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}

		private CachedMajorMinor _cachedMajorMinor;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Log10TickSpacing), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Log10TickSpacing s = (Log10TickSpacing)obj;

				info.AddValue("OneLever", s._oneLever);
				info.AddValue("MinGrace", s._orgGrace);
				info.AddValue("MaxGrace", s._endGrace);
				info.AddEnum("SnapOrgToTick", s._snapOrgToTick);
				info.AddEnum("SnapEndToTick", s._snapEndToTick);

				info.AddValue("TargetNumberOfMajorTicks", s._targetNumberOfMajorTicks);
				info.AddValue("TargetNumberOfMinorTicks", s._targetNumberOfMinorTicks);
				info.AddValue("UserDefinedMajorDecades", s._userDefinedNumberOfDecadesPerMajorTick);
				info.AddValue("UserDefinedMinorTicks", s._userDefinedMinorTicks);

				info.AddValue("TransformationExponent", s._transformationExponent);
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
				Log10TickSpacing s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual Log10TickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				Log10TickSpacing s = null != o ? (Log10TickSpacing)o : new Log10TickSpacing();

				s._oneLever = info.GetDouble("OneLever");
				s._orgGrace = info.GetDouble("MinGrace");
				s._endGrace = info.GetDouble("MaxGrace");
				s._snapOrgToTick = (BoundaryTickSnapping)info.GetEnum("SnapOrgToTick", typeof(BoundaryTickSnapping));
				s._snapEndToTick = (BoundaryTickSnapping)info.GetEnum("SnapEndToTick", typeof(BoundaryTickSnapping));

				s._targetNumberOfMajorTicks = info.GetInt32("TargetNumberOfMajorTicks");
				s._targetNumberOfMinorTicks = info.GetInt32("TargetNumberOfMinorTicks");
				s._userDefinedNumberOfDecadesPerMajorTick = info.GetNullableInt32("UserDefinedMajorDecades");
				s._userDefinedMinorTicks = info.GetNullableInt32("UserDefinedMinorTicks");

				s._transformationExponent = info.GetDouble("TransformationExponent");
				s._transformationDivider = info.GetDouble("TransformationDivider");
				s._transformationOperationIsMultiply = info.GetBoolean("TransformationIsMultiply");

				s.ChildSetMember(ref s._suppressedMajorTicks, (SuppressedTicks)info.GetValue("SuppressedMajorTicks", s));
				s.ChildSetMember(ref s._suppressedMinorTicks, (SuppressedTicks)info.GetValue("SuppressedMinorTicks", s));
				s.ChildSetMember(ref s._additionalMajorTicks, (AdditionalTicks)info.GetValue("AdditionalMajorTicks", s));
				s.ChildSetMember(ref s._additionalMinorTicks, (AdditionalTicks)info.GetValue("AdditionalMinorTicks", s));

				if (s._suppressedMajorTicks == null)
					s._suppressedMajorTicks = new SuppressedTicks() { ParentObject = s };
				if (s._suppressedMinorTicks == null)
					s._suppressedMinorTicks = new SuppressedTicks() { ParentObject = s };

				if (s._additionalMajorTicks == null)
					s._additionalMajorTicks = new AdditionalTicks() { ParentObject = s };
				if (s._additionalMinorTicks == null)
					s._additionalMinorTicks = new AdditionalTicks() { ParentObject = s };

				return s;
			}
		}

		#endregion Serialization

		public Log10TickSpacing()
		{
			_majorTicks = new List<double>();
			_minorTicks = new List<double>();
			_suppressedMajorTicks = new SuppressedTicks() { ParentObject = this };
			_suppressedMinorTicks = new SuppressedTicks() { ParentObject = this };
			_additionalMajorTicks = new AdditionalTicks() { ParentObject = this };
			_additionalMinorTicks = new AdditionalTicks() { ParentObject = this };
		}

		public Log10TickSpacing(Log10TickSpacing from)
			: base(from) // everything is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as Log10TickSpacing;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				CopyHelper.Copy(ref _cachedMajorMinor, from._cachedMajorMinor);

				_userDefinedNumberOfDecadesPerMajorTick = from._userDefinedNumberOfDecadesPerMajorTick;
				_userDefinedMinorTicks = from._userDefinedMinorTicks;

				_targetNumberOfMajorTicks = from._targetNumberOfMajorTicks;
				_targetNumberOfMinorTicks = from._targetNumberOfMinorTicks;

				_oneLever = from._oneLever;
				_orgGrace = from._orgGrace;
				_endGrace = from._endGrace;
				_snapOrgToTick = from._snapOrgToTick;
				_snapEndToTick = from._snapEndToTick;

				_transformationDivider = from._transformationDivider;
				_transformationOperationIsMultiply = from._transformationOperationIsMultiply;
				_transformationExponent = from._transformationExponent;

				ChildCopyToMember(ref _suppressedMajorTicks, from._suppressedMajorTicks);
				ChildCopyToMember(ref _suppressedMinorTicks, from._suppressedMinorTicks);
				ChildCopyToMember(ref _additionalMajorTicks, from._additionalMajorTicks);
				ChildCopyToMember(ref _additionalMinorTicks, from._additionalMinorTicks);

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
			return new Log10TickSpacing(this);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			else if (!(obj is Log10TickSpacing))
				return false;
			else
			{
				var from = (Log10TickSpacing)obj;

				if (_userDefinedNumberOfDecadesPerMajorTick != from._userDefinedNumberOfDecadesPerMajorTick)
					return false;
				if (_userDefinedMinorTicks != from._userDefinedMinorTicks)
					return false;

				if (_targetNumberOfMajorTicks != from._targetNumberOfMajorTicks)
					return false;
				if (_targetNumberOfMinorTicks != from._targetNumberOfMinorTicks)
					return false;

				if (_oneLever != from._oneLever)
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

		public override int GetHashCode()
		{
			return base.GetHashCode() + 13 * _targetNumberOfMajorTicks + 31 * _targetNumberOfMinorTicks;
		}

		#region User parameters

		public double OneLever
		{
			get
			{
				return _oneLever;
			}
			set
			{
				var oldValue = _oneLever;
				_oneLever = value;
				if (value != oldValue)
					EhSelfChanged();
			}
		}

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
				if (value != oldValue)
					EhSelfChanged();
			}
		}

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
				if (value != oldValue)
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
				if (value != oldValue)
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
				if (value != oldValue)
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
				if (value != oldValue)
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
				if (value != oldValue)
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
				if (value != oldValue)
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
				if (value != oldValue)
					EhSelfChanged();
			}
		}

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

		/// <summary>Gets or sets the minor ticks.</summary>
		/// <value>
		/// <para>If the value is null, the number of minor ticks is determined automatically.</para>
		/// <para>If the value is positive, the minor ticks will mark full decades only. The value then determines the number of decades per minor tick as (numberOfDecadesPerMajorTick / MinorTicks).</para>
		/// <para>If the value is 0 or 1, no minor ticks will be visible.</para>
		/// <para>If the value is -1, only mantissa values of 1, 4, and 7 are used as minor ticks (the 1 is used only if there is more than one decade per major tick).</para>
		/// <para>If the value is -2, mantissa values of 1, 2 , 3, 4, 5, 6, 7, 8 and 9 are used as minor ticks (the 1 is used only if there is more than one decade per major tick).</para>
		/// </value>
		public int? MinorTicks
		{
			get
			{
				return _userDefinedMinorTicks;
			}
			set
			{
				var oldValue = _userDefinedMinorTicks;
				_userDefinedMinorTicks = value;
				if (value != oldValue)
					EhSelfChanged();
			}
		}

		/// <summary>Gets or sets the number of decades per major tick.</summary>
		/// <value><para>If null, the number of decades per major tick is calculated automatically.</para>
		/// <para>If the value is negative or null, no major ticks will be shown.</para>
		/// <para>If the value is positive, the value represents the number of decades per major tick. Note that major ticks will appear only at decades that are a integer multiple of this value.</para>
		/// </value>
		public int? DecadesPerMajorTick
		{
			get
			{
				return _userDefinedNumberOfDecadesPerMajorTick;
			}
			set
			{
				var oldValue = _userDefinedNumberOfDecadesPerMajorTick;
				_userDefinedNumberOfDecadesPerMajorTick = value;
				if (value != oldValue)
					EhSelfChanged();
			}
		}

		public SuppressedTicks SuppressedMajorTicks
		{
			get
			{
				return _suppressedMajorTicks;
			}
		}

		public SuppressedTicks SuppressedMinorTicks
		{
			get
			{
				return _suppressedMinorTicks;
			}
		}

		public AdditionalTicks AdditionalMajorTicks
		{
			get
			{
				return _additionalMajorTicks;
			}
		}

		public AdditionalTicks AdditionalMinorTicks
		{
			get
			{
				return _additionalMinorTicks;
			}
		}

		#endregion User parameters

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

			dorg = TransformOriginalToModified(dorg);
			dend = TransformOriginalToModified(dend);

			if (InternalPreProcessScaleBoundaries(ref dorg, ref dend, isOrgExtendable, isEndExtendable))
			{
				org = TransformModifiedToOriginal(dorg);
				end = TransformModifiedToOriginal(dend);
				return true;
			}
			else
				return false;
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

			dorg = TransformOriginalToModified(dorg);
			dend = TransformOriginalToModified(dend);

			if (_cachedMajorMinor == null || _cachedMajorMinor.Org != dorg || _cachedMajorMinor.End != dend)
			{
				InternalPreProcessScaleBoundaries(ref dorg, ref dend, false, false); // make sure that _cachedMajorMinor is valid now
			}

			if (!(null != _cachedMajorMinor)) throw new InvalidProgramException();

			var lg10Org = Math.Log10(dorg);
			var lg10End = Math.Log10(dend);

			_majorTicks.Clear();
			_minorTicks.Clear();
			InternalCalculateMajorTicks(lg10Org, lg10End);
			InternalCalculateMinorTicks(lg10Org, lg10End);
		}

		#region Calculation of tick values

		/// <summary>Calculates the major ticks for the logarithmic scale. It is assumed, <see cref="_cachedMajorMinor"/> contains valid information concerning the number of decades per major tick and
		/// the number of minor ticks per major tick interval. The function filles the <see cref="_majorTicks"/> collection with the major tick values.</summary>
		/// <param name="lg10Org">The Log10() value of the scale origin.</param>
		/// <param name="lg10End">The Log10() value of the scale end.</param>
		private void InternalCalculateMajorTicks(double lg10Org, double lg10End)
		{
			_majorTicks.Clear();

			int decadesPerMajorTick = _cachedMajorMinor.DecadesPerMajorTick;

			if (decadesPerMajorTick > 0)
			{
				double lgScaleSpan = lg10End - lg10Org;
				double lg10OrgRoundedDown = lg10Org - Math.Abs(1e-6 * lgScaleSpan);
				double lg10EndRoundedUp = lg10End + Math.Abs(1e-6 * lgScaleSpan);

				int beg = decadesPerMajorTick * (int)Math.Floor(lg10OrgRoundedDown / decadesPerMajorTick); // we ensure that we will have "even" multiples of the decadesPerMajorTick, so that 1 is always included.
				int end = decadesPerMajorTick * (int)Math.Ceiling(lg10EndRoundedUp / decadesPerMajorTick);

				for (int i = beg; i <= end; i += decadesPerMajorTick)
				{
					if (i >= lg10OrgRoundedDown && i <= lg10EndRoundedUp)
					{
						double tickValue = RMath.Pow(10, i);
						_majorTicks.Add(tickValue);
					}
				}
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

		/// <summary>Calculates the minor ticks for the logarithmic scale. It is assumed, <see cref="_cachedMajorMinor"/> contains valid information concerning the number of decades per major tick and
		/// the number of minor ticks per major tick interval. The function filles the <see cref="_minorTicks"/> collection with the minor tick values.</summary>
		/// <param name="lg10Org">The Log10() value of the scale origin.</param>
		/// <param name="lg10End">The Log10() value of the scale end.</param>
		private void InternalCalculateMinorTicks(double lg10Org, double lg10End)
		{
			_minorTicks.Clear();

			int decadesPerMajorTick = _cachedMajorMinor.DecadesPerMajorTick;
			int minorTicks = _cachedMajorMinor.MinorTicks;

			double lgScaleSpan = lg10End - lg10Org;
			double lg10OrgRoundedDown = lg10Org - Math.Abs(1e-6 * lgScaleSpan);
			double lg10EndRoundedUp = lg10End + Math.Abs(1e-6 * lgScaleSpan);

			if (minorTicks >= 2 && decadesPerMajorTick > 0)
			{
				if (!(decadesPerMajorTick % minorTicks == 0)) throw new InvalidProgramException();
				int decadesPerMinorTick = decadesPerMajorTick / minorTicks;

				int beg = (int)Math.Floor(lg10OrgRoundedDown); // we ensure that we will have "even" multiples of the decadesPerMajorTick, so that 1 is always included.
				int end = (int)Math.Ceiling(lg10EndRoundedUp);

				// now, add the ticks, taking into account the possible suppression of ticks
				for (int i = beg; i <= end; ++i)
				{
					if (i >= lg10OrgRoundedDown && i <= lg10EndRoundedUp && 0 == i % decadesPerMinorTick)
					{
						double tickValue = RMath.Pow(10, i);
						if (0 != i % decadesPerMajorTick) // add minor tick only if it is not a major tick
							_minorTicks.Add(tickValue);
					}
				}
			}
			else if (minorTicks < 0) // minor ticks are mantissa values, like 1, 2, 3 etc.
			{
				int[] mantissa = minorTicks == -1 ? minorTickMantissa3 : minorTickMantissa9;
				int beg = (int)Math.Floor(lg10OrgRoundedDown); // we ensure that we will have "even" multiples of the decadesPerMajorTick, so that 1 is always included.
				int end = (int)Math.Ceiling(lg10EndRoundedUp);

				// now add all minor ticks
				for (int i = beg; i <= end; ++i)
				{
					for (int j = 0; j < mantissa.Length; ++j)
					{
						double lgVal = i + Math.Log10(mantissa[j]);
						if (lgVal >= lg10OrgRoundedDown && lgVal <= lg10EndRoundedUp)
						{
							double tickValue = mantissa[j] * RMath.Pow(10, i);
							if (j != 0 || decadesPerMajorTick <= 0 || 0 != i % decadesPerMajorTick) // add minor tick only if it is not already a major tick
								_minorTicks.Add(tickValue);
						}
					}
				}
			}

			// Remove suppressed ticks
			_suppressedMinorTicks.RemoveSuppressedTicks(_minorTicks);

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

			// both xorg and xend have to be positive finite
			if (xorg == xend)
			{
				xorg /= 4;
				xend *= 4;
				modified = true;
			}
			if (!(xorg > 0 && xorg < double.MaxValue))
			{
				xorg = double.Epsilon;
				modified = true;
			}
			if (!(xend > 0 && xend < double.MaxValue))
			{
				xend = double.MaxValue;
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
			bool modGraceAndOneLever = GetOrgEndWithGraceAndOneLever(xorg, xend, isOrgExtendable, isEndExtendable, out xOrgWithGraceAndOneLever, out xEndWithGraceAndOneLever);

			// try applying tick snapping only (without Grace and OneLever)
			double xOrgWithTickSnapping, xEndWithTickSnapping;
			int decadesPerMajorTick, minorTicks;
			bool modTickSnapping = GetOrgEndWithTickSnappingOnly(Math.Log10(xend) - Math.Log10(xorg), xorg, xend, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping, out decadesPerMajorTick, out minorTicks);

			// now compare the two
			if (xOrgWithTickSnapping <= xOrgWithGraceAndOneLever && xEndWithTickSnapping >= xEndWithGraceAndOneLever)
			{
				// then there is no need to apply Grace and OneLever
				modified |= modTickSnapping;
			}
			else
			{
				modified |= modGraceAndOneLever;
				modified |= GetOrgEndWithTickSnappingOnly(Math.Log10(xEndWithGraceAndOneLever) - Math.Log10(xOrgWithGraceAndOneLever), xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping, out decadesPerMajorTick, out minorTicks);
			}

			xorg = xOrgWithTickSnapping;
			xend = xEndWithTickSnapping;

			_cachedMajorMinor = new CachedMajorMinor(xorg, xend, decadesPerMajorTick, minorTicks);

			return modified;
		}

		/// <summary>
		/// Applies the value for <see cref="OrgGrace"/>, <see cref="EndGrace"/> and <see cref="OneLever"/> to the scale and calculated proposed values for the boundaries.
		/// </summary>
		/// <param name="scaleOrg">Scale origin.</param>
		/// <param name="scaleEnd">Scale end.</param>
		/// <param name="isOrgExtendable">True if the scale org can be extended.</param>
		/// <param name="isEndExtendable">True if the scale end can be extended.</param>
		/// <param name="propOrg">Returns the proposed value of the scale origin.</param>
		/// <param name="propEnd">Returns the proposed value of the scale end.</param>
		public bool GetOrgEndWithGraceAndOneLever(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
		{
			bool modified = false;
			double scaleSpan = Math.Abs(Math.Log10(scaleEnd) - Math.Log10(scaleOrg));
			propOrg = Math.Log10(scaleOrg);
			if (isOrgExtendable)
			{
				propOrg -= Math.Abs(_orgGrace * scaleSpan);
				modified |= (0 != _orgGrace);
			}

			propEnd = Math.Log10(scaleEnd);
			if (isEndExtendable)
			{
				propEnd += Math.Abs(_endGrace * scaleSpan);
				modified |= (0 != _endGrace);
			}

			double lever = Math.Abs(_oneLever * scaleSpan);
			double propOrg2 = Math.Log10(scaleOrg) - lever;
			double propEnd2 = Math.Log10(scaleEnd) + lever;

			if (isOrgExtendable && propOrg > 0 && propOrg2 <= 0)
			{
				propOrg = 0;
				modified = true;
			}

			if (isEndExtendable && propEnd < 0 && propEnd2 >= 0)
			{
				propEnd = 0;
				modified = true;
			}

			double range = propEnd - propOrg;
			if (range == 0) // Emergency plan if range is zero
			{
				double extend = propOrg == 0 ? 0.5 : Math.Abs(propOrg / 10);
				if (isOrgExtendable && isEndExtendable)
				{
					propOrg -= extend;
					propEnd += extend;
					modified = true;
				}
				else if (isOrgExtendable)
				{
					propOrg -= 2 * extend;
					modified = true;
				}
				else if (isEndExtendable)
				{
					propEnd += 2 * extend;
					modified = true;
				}
			}

			propOrg = Math.Pow(10, propOrg);
			propEnd = Math.Pow(10, propEnd);

			return modified;
		}

		/// <summary>Applies the tick snapping settings to the scale origin and scale end. This is done by a determination of the number of decades per major tick and the minor ticks per major tick interval.
		/// Then, the snapping values are applied, and the org and end values of the scale are adjusted (if allowed so).</summary>
		/// <param name="overridenScaleDecades">The overriden scale decades.</param>
		/// <param name="scaleOrg">The scale origin.</param>
		/// <param name="scaleEnd">The scale end.</param>
		/// <param name="isOrgExtendable">If set to <c>true</c>, it is allowed to adjust the scale org value.</param>
		/// <param name="isEndExtendable">if set to <c>true</c>, it is allowed to adjust the scale end value.</param>
		/// <param name="propOrg">The adjusted scale orgin.</param>
		/// <param name="propEnd">The adjusted scale end.</param>
		/// <param name="numberOfDecadesPerMajorTick">The number of decades per major tick.</param>
		/// <param name="minorTicks">Number of minor ticks per major tick interval. This variable has some special values (see <see cref="MinorTicks"/>).</param>
		/// <returns>True if at least either org or end were adjusted to a new value.</returns>
		private bool GetOrgEndWithTickSnappingOnly(double overridenScaleDecades, double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd, out int numberOfDecadesPerMajorTick, out int minorTicks)
		{
			bool modified = false;
			propOrg = scaleOrg;
			propEnd = scaleEnd;

			if (null != _userDefinedNumberOfDecadesPerMajorTick)
			{
				numberOfDecadesPerMajorTick = _userDefinedNumberOfDecadesPerMajorTick.Value;
			}
			else
			{
				numberOfDecadesPerMajorTick = CalculateNumberOfDecadesPerMajorTick(overridenScaleDecades, _targetNumberOfMajorTicks);
			}

			if (null != _userDefinedMinorTicks)
			{
				minorTicks = _userDefinedMinorTicks.Value;
				if (minorTicks > numberOfDecadesPerMajorTick)
					minorTicks = numberOfDecadesPerMajorTick;
				if (minorTicks >= 2)
				{
					for (int i = minorTicks; i <= numberOfDecadesPerMajorTick; ++i)
						if (numberOfDecadesPerMajorTick % i == 0)
						{
							minorTicks = i;
							break;
						}
				}
			}
			else
			{
				minorTicks = CalculateNumberOfMinorTicks(overridenScaleDecades, numberOfDecadesPerMajorTick, _targetNumberOfMinorTicks);
			}

			if (isOrgExtendable)
			{
				propOrg = GetOrgOrEndSnappedToTick(scaleOrg, numberOfDecadesPerMajorTick, minorTicks, _snapOrgToTick, false);
				modified |= BoundaryTickSnapping.SnapToNothing != _snapOrgToTick;
			}

			if (isEndExtendable)
			{
				propEnd = GetOrgOrEndSnappedToTick(scaleEnd, numberOfDecadesPerMajorTick, minorTicks, _snapEndToTick, true);
				modified |= BoundaryTickSnapping.SnapToNothing != _snapEndToTick;
			}

			return modified;
		}

		/// <summary>
		/// Adjusts the parameter <paramref name="x"/> so that <paramref name="x"/> snaps to a tick according to the setting of <paramref name="snapping"/>.
		/// </summary>
		/// <param name="x">The boundary value to adjust.</param>
		/// <param name="decadesPerMajorTick">Number of decades between the major ticks.</param>
		/// <param name="minorTicks">Number of minor ticks.</param>
		/// <param name="snapping">Setting of the tick snapping.</param>
		/// <param name="upwards">If true, the value is towards higher values, if false it is adjusted towards smaller values.</param>
		/// <returns>The adjusted value of x.</returns>
		public static double GetOrgOrEndSnappedToTick(double x, int decadesPerMajorTick, int minorTicks, BoundaryTickSnapping snapping, bool upwards)
		{
			switch (snapping)
			{
				case BoundaryTickSnapping.SnapToMajorOnly:
					{
						double rel = Math.Log10(x) / decadesPerMajorTick;
						if (upwards)
							return RMath.Pow(10, decadesPerMajorTick * (int)Math.Ceiling(rel));
						else
							return RMath.Pow(10, decadesPerMajorTick * (int)Math.Floor(rel));
					}
				case BoundaryTickSnapping.SnapToMinorOnly:
					{
						if (minorTicks > 1)
						{
							int decadesPerMinorTick = decadesPerMajorTick / minorTicks;
							double rel = Math.Log10(x) / decadesPerMinorTick;
							if (upwards)
							{
								int nrel = decadesPerMinorTick * (int)Math.Ceiling(rel);
								if (0 == (nrel % decadesPerMajorTick))
									nrel += decadesPerMinorTick;
								return RMath.Pow(10, nrel);
							}
							else
							{
								int nrel = decadesPerMinorTick * (int)Math.Floor(rel);
								if (0 == (nrel % decadesPerMajorTick))
									nrel -= decadesPerMinorTick;
								return RMath.Pow(10, nrel);
							}
						}
						else if (minorTicks < 0)
						{
							int[] mantissa = minorTicks == -1 ? minorTickMantissa3 : minorTickMantissa9;
							double rel = Math.Log10(x);
							if (upwards)
							{
								int nrel = (int)Math.Floor(rel);
								for (int i = 0; i < mantissa.Length; ++i)
								{
									double result = mantissa[i] * RMath.Pow(10, nrel);
									if (result >= x && (i != 0 || 0 != (nrel % decadesPerMajorTick)))
										return result;
								}
							}
							else
							{
								int nrel = (int)Math.Ceiling(rel) - 1;
								for (int i = mantissa.Length - 1; i >= 0; --i)
								{
									double result = mantissa[i] * RMath.Pow(10, (int)(rel));
									if (result <= x && (i != 0 || 0 != (nrel % decadesPerMajorTick)))
										return result;
								}
							}
						}
						else // no minor ticks
						{
							double rel = Math.Log10(x) / decadesPerMajorTick;
							if (upwards)
								return RMath.Pow(10, decadesPerMajorTick * (int)Math.Ceiling(rel));
							else
								return RMath.Pow(10, decadesPerMajorTick * (int)Math.Floor(rel));
						}
					}
					break;

				case BoundaryTickSnapping.SnapToMinorOrMajor:
					{
						if (minorTicks > 1)
						{
							int decadesPerMinorTick = decadesPerMajorTick / minorTicks;
							double rel = Math.Log10(x) / decadesPerMinorTick;
							if (upwards)
								return RMath.Pow(10, decadesPerMinorTick * (int)Math.Ceiling(rel));
							else
								return RMath.Pow(10, decadesPerMinorTick * (int)Math.Floor(rel));
						}
						else if (minorTicks < 0)
						{
							int[] mantissa = minorTicks == -1 ? minorTickMantissa3 : minorTickMantissa9;
							double rel = Math.Log10(x);
							if (upwards)
							{
								int nrel = (int)Math.Floor(rel);
								double basis = RMath.Pow(10, nrel);
								for (int i = 0; i < mantissa.Length; ++i)
								{
									double result = mantissa[i] * basis;
									if (result >= x)
										return result;
								}
								return 10 * basis;
							}
							else // downwards
							{
								int nrel = (int)Math.Ceiling(rel) - 1;
								double basis = RMath.Pow(10, nrel);
								double result = 10 * basis;
								if (result <= x)
									return result;
								for (int i = mantissa.Length - 1; i >= 0; --i)
								{
									result = mantissa[i] * basis;
									if (result <= x)
										return result;
								}
							}
						}
						else // no minor ticks
						{
							double rel = Math.Log10(x) / decadesPerMajorTick;
							if (upwards)
								return RMath.Pow(10, decadesPerMajorTick * (int)Math.Ceiling(rel));
							else
								return RMath.Pow(10, decadesPerMajorTick * (int)Math.Floor(rel));
						}
					}
					break;
			}
			return x;
		}

		/// <summary>Calculates the number of decades per major tick.</summary>
		/// <param name="overridenScaleDecades">The overriden scale decades (Log10(end)-Log10(org)).</param>
		/// <param name="targetNumberOfMajorTicks">The target number of major ticks.</param>
		/// <returns>The number of decimal decades per major tick.</returns>
		private static int CalculateNumberOfDecadesPerMajorTick(double overridenScaleDecades, int targetNumberOfMajorTicks)
		{
			if (targetNumberOfMajorTicks <= 0)
				return 0;

			if (!(overridenScaleDecades > 0))
				overridenScaleDecades = 1;

			double rawMajorDecades = overridenScaleDecades / targetNumberOfMajorTicks;

			// now we have two possibilities: to round rawMajorDecades up or down
			// we will see which number is closer to targetNumberOfMajorTicks

			double small = Math.Floor(rawMajorDecades);
			double big = Math.Ceiling(rawMajorDecades);

			var numberOfTicksSmall = Math.Floor(overridenScaleDecades / small);
			var numberOfTicksBig = Math.Floor(overridenScaleDecades / big);

			if (Math.Abs(numberOfTicksSmall - targetNumberOfMajorTicks) < Math.Abs(numberOfTicksBig - targetNumberOfMajorTicks))
				return (int)small;
			else
				return (int)big;
		}

		/// <summary>Calculates the number of minor ticks.</summary>
		/// <param name="overridenScaleDecades">Number of decades that the full scale should span.</param>
		/// <param name="numberOfDecadesPerMajorTick">The number of decades per major tick.</param>
		/// <param name="targetNumberOfMinorTicks">The target number of minor ticks.</param>
		/// <returns>
		/// <para>If the return value is positive, the minor ticks will mark full decades only. The return value then determines the number of decades per minor tick = (numberOfDecadesPerMajorTick / ReturnValue).</para>
		/// <para>If the return value is 0 or 1, no minor ticks will be visible.</para>
		/// <para>If the return value is -1, only manissas of 1, 4, and 7 are used as minor ticks (the 1 is used only if there is more than one decade per major tick).</para>
		/// <para>If the return value is -2, mantissas of 1, 2 , 3, 4, 5, 6, 7, 8 and 9 are used as minor ticks (the 1 is used only if there is more than one decade per major tick).</para>
		/// </returns>
		private static int CalculateNumberOfMinorTicks(double overridenScaleDecades, int numberOfDecadesPerMajorTick, int targetNumberOfMinorTicks)
		{
			int optimalMinorTicks = 0;

			if (numberOfDecadesPerMajorTick >= 1)
			{
				double majorSpanIntervals = overridenScaleDecades / numberOfDecadesPerMajorTick;

				optimalMinorTicks = 0;
				double smallestDeviation = double.MaxValue;
				for (int i = -2; i <= numberOfDecadesPerMajorTick; ++i)
				{
					if (i == 1)
						continue;
					if (i >= 2 && (numberOfDecadesPerMajorTick % i) != 0)
						continue;

					double ticks;
					if (i == -2)
						ticks = 9 * overridenScaleDecades;
					else if (i == -1)
						ticks = 3 * overridenScaleDecades;
					else if (i >= 2)
						ticks = (overridenScaleDecades * i) / numberOfDecadesPerMajorTick;
					else
						ticks = 0;

					double minorTickDeviation = Math.Abs(ticks - targetNumberOfMinorTicks);

					if (minorTickDeviation < smallestDeviation)
					{
						smallestDeviation = minorTickDeviation;
						optimalMinorTicks = i;
					}
				}
			}
			return optimalMinorTicks;
		}

		#endregion Functions to predict change of scale by tick snapping, grace, and OneLever
	}
}