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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Altaxo.Calc;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{







	/// <summary>
	/// Tick settings for a linear scale.
	/// </summary>
	public class LinearTickSpacing : NumericTickSpacing
	{
		// primary values
		/// <summary>Current axis origin divided by the major tick span value.</summary>
		protected double _axisOrgByMajor = 0;
		/// <summary>Current axis end divided by the major tick span value.</summary>
		protected double _axisEndByMajor = 5;
		/// <summary>Physical span value between two major ticks.</summary>
		protected double _majorSpan = 0.2; // physical span value between two major ticks
		/// <summary>Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!</summary>
		protected int _numberOfMinorTicks = 2;

    /// <summary>Maximum allowed number of ticks in case manual tick input will produce a big amount of ticks.</summary>
    protected static readonly int _maxSafeNumberOfTicks=10000;



		/// <summary>If set, gives the number of minor ticks choosen by the user.</summary>
		int? _userDefinedMinorTicks;

		/// <summary>If set, gives the physical value between two major ticks choosen by the user.</summary>
		double? _userDefinedMajorSpan;

		double _zeroLever = 0.25;
		double _minGrace = 1 / 16.0;
		double _maxGrace = 1 / 16.0;

		int _targetNumberOfMajorTicks = 4;
		int _targetNumberOfMinorTicks = 2;

		double _transformationDivider = 1;
		bool _transformationOperationIsMultiply;
		double _transformationOffset = 0;

		/// <summary>If true, the boundaries will be set on a minor or major tick.</summary>
		BoundaryTickSnapping _snapOrgToTick;
		BoundaryTickSnapping _snapEndToTick;

		SuppressedTicks _suppressedMajorTicks;
		SuppressedTicks _suppressedMinorTicks;
    AdditionalTicks _additionalMajorTicks;
    AdditionalTicks _additionalMinorTicks;

		List<double> _majorTicks;
		List<double> _minorTicks;

		class CachedMajorMinor
		{
			public double Org, End;
			public double MajorSpan;
			public int MinorTicks;

			public CachedMajorMinor(double org, double end, double major, int minor)
			{
				Org = org;
				End = end;
				MajorSpan = major;
				MinorTicks = minor;
			}
		}

		CachedMajorMinor _cachedMajorMinor;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearTickSpacing), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinearTickSpacing s = (LinearTickSpacing)obj;

				info.AddValue("ZeroLever", s._zeroLever);
				info.AddValue("MinGrace", s._minGrace);
				info.AddValue("MaxGrace", s._maxGrace);
        info.AddEnum("SnapOrgToTick", s._snapOrgToTick);
        info.AddEnum("SnapEndToTick", s._snapEndToTick);

				info.AddValue("TargetNumberOfMajorTicks", s._targetNumberOfMajorTicks);
				info.AddValue("TargetNumberOfMinorTicks", s._targetNumberOfMinorTicks);
        info.AddValue("UserDefinedMajorSpan", s._userDefinedMajorSpan);
        info.AddValue("UserDefinedMinorTicks", s._userDefinedMinorTicks);

				info.AddValue("TransformationOffset", s._transformationOffset);
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


        if(s._additionalMajorTicks.IsEmpty)
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
				LinearTickSpacing s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual LinearTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinearTickSpacing s = null != o ? (LinearTickSpacing)o : new LinearTickSpacing();
				s._zeroLever = info.GetDouble("ZeroLever");
				s._minGrace = info.GetDouble("MinGrace");
				s._maxGrace = info.GetDouble("MaxGrace");
        s._snapOrgToTick = (BoundaryTickSnapping)info.GetEnum("SnapOrgToTick", typeof(BoundaryTickSnapping));
        s._snapEndToTick = (BoundaryTickSnapping)info.GetEnum("SnapEndToTick", typeof(BoundaryTickSnapping));
        
        
        s._targetNumberOfMajorTicks = info.GetInt32("TargetNumberOfMajorTicks");
				s._targetNumberOfMinorTicks = info.GetInt32("TargetNumberOfMinorTicks");
        s._userDefinedMajorSpan = info.GetNullableDouble("UserDefinedMajorSpan");
        s._userDefinedMinorTicks = info.GetNullableInt32("UserDefinedMinorTicks");


				s._transformationOffset = info.GetDouble("TransformationOffset");
				s._transformationDivider = info.GetDouble("TransformationDivider");
				s._transformationOperationIsMultiply = info.GetBoolean("TransformationIsMultiply");


				s._suppressedMajorTicks = (SuppressedTicks)info.GetValue("SuppressedMajorTicks", s);
        s._suppressedMinorTicks = (SuppressedTicks)info.GetValue("SuppressedMinorTicks", s);
        s._additionalMajorTicks = (AdditionalTicks)info.GetValue("AdditionalMajorTicks", s);
        s._additionalMinorTicks = (AdditionalTicks)info.GetValue("AdditionalMinorTicks", s);

        if (s._suppressedMajorTicks == null)
          s._suppressedMajorTicks = new SuppressedTicks();
        if (s._suppressedMinorTicks == null)
          s._suppressedMinorTicks = new SuppressedTicks();

        if (s._additionalMajorTicks == null)
          s._additionalMajorTicks = new AdditionalTicks();
        if (s._additionalMinorTicks == null)
          s._additionalMinorTicks = new AdditionalTicks();

				return s;
			}
		}
		#endregion


		public LinearTickSpacing()
		{
			_majorTicks = new List<double>();
			_minorTicks = new List<double>();
      _suppressedMajorTicks = new SuppressedTicks();
      _suppressedMinorTicks = new SuppressedTicks();
      _additionalMajorTicks = new AdditionalTicks();
      _additionalMinorTicks = new AdditionalTicks();
		}

		public LinearTickSpacing(LinearTickSpacing from)
			: this()
		{
			CopyFrom(from);
		}



		public virtual void CopyFrom(LinearTickSpacing from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this._axisEndByMajor = from._axisEndByMajor;
			this._axisOrgByMajor = from._axisOrgByMajor;
			this._majorSpan = from._majorSpan;
			this._numberOfMinorTicks = from._numberOfMinorTicks;

			_majorTicks.Clear();
			_majorTicks.AddRange(from._majorTicks);

			_minorTicks.Clear();
			_minorTicks.AddRange(from._minorTicks);

			_userDefinedMajorSpan = from._userDefinedMajorSpan;
			_userDefinedMinorTicks = from._userDefinedMinorTicks;

			_zeroLever = from._zeroLever;
			_minGrace = from._minGrace;
			_maxGrace = from._maxGrace;

			_snapOrgToTick = from._snapOrgToTick;
			_snapEndToTick = from._snapEndToTick;

			_targetNumberOfMajorTicks = from._targetNumberOfMajorTicks;
			_targetNumberOfMinorTicks = from._targetNumberOfMinorTicks;

      _suppressedMajorTicks = (SuppressedTicks)from._suppressedMajorTicks.Clone();
      _suppressedMinorTicks = (SuppressedTicks)from._suppressedMinorTicks.Clone();
      _additionalMajorTicks = (AdditionalTicks)from._additionalMajorTicks.Clone();
      _additionalMinorTicks = (AdditionalTicks)from._additionalMinorTicks.Clone();

			_transformationOffset = from._transformationOffset;
			_transformationDivider = from._transformationDivider;
			_transformationOperationIsMultiply = from._transformationOperationIsMultiply;
		}

		public override object Clone()
		{
			return new LinearTickSpacing(this);
		}

		public double ZeroLever
		{
			get
			{
				return _zeroLever;
			}
			set
			{
				_zeroLever = value;
			}
		}

		public double MinGrace
		{
			get
			{
				return _minGrace;
			}
			set
			{
				_minGrace = value;
			}
		}

		public double MaxGrace
		{
			get
			{
				return _maxGrace;
			}
			set
			{
				_maxGrace = value;
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
        _snapOrgToTick = value;
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
        _snapEndToTick = value;
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
        _targetNumberOfMajorTicks = value;
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
        _targetNumberOfMinorTicks = value;
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
				_transformationDivider = value;
			}
		}

		public double TransformationOffset
		{
			get
			{
				return _transformationOffset;
			}
			set
			{
				_transformationOffset = value;
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
				_transformationOperationIsMultiply = value;
			}
		}

		private double TransformOriginalToModified(double x)
		{
			if (_transformationOperationIsMultiply)
				return _transformationOffset + x * _transformationDivider;
			else
				return _transformationOffset + x / _transformationDivider;
		}

		private double TransformModifiedToOriginal(double y)
		{
			if (_transformationOperationIsMultiply)
				return (y - _transformationOffset) / _transformationDivider;
			else
				return (y - _transformationOffset) * _transformationDivider;
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


		public int? MinorTicks
		{
			get
			{
				return _userDefinedMinorTicks;
			}
			set
			{
				_userDefinedMinorTicks = value;
			}
		}

		public double? MajorTick
		{
			get
			{
				return _userDefinedMajorSpan;
			}
			set
			{
				_userDefinedMajorSpan = value;
			}
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

			InternalCalculateTicks(dorg, dend);
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

		bool InternalPreProcessScaleBoundaries(ref double xorg, ref double xend, bool isOrgExtendable, bool isEndExtendable)
		{
			_cachedMajorMinor = null;

			if (!(xend >= xorg))
				throw new ArgumentOutOfRangeException("xend is not greater or equal than xorg");

			if (xend == xorg)
				ApplyGraceAndZeroLever(xorg, xend, isOrgExtendable, isEndExtendable, out xorg, out xend);

			double xorg2, xend2;
			ApplyGraceAndZeroLever(xorg, xend, isOrgExtendable, isEndExtendable, out xorg2, out xend2);

			double propOrg, propEnd, propOrg2, propEnd2, majorSpan, majorSpan2;
			int minorTicks, minorTicks2;
			CalculateBoundaries(xend - xorg, xorg, xend, isOrgExtendable, isEndExtendable, out propOrg, out propEnd, out majorSpan, out minorTicks);
			CalculateBoundaries(xend2 - xorg2, xorg, xend, isOrgExtendable, isEndExtendable, out propOrg2, out propEnd2, out majorSpan2, out minorTicks2);

			if ((propEnd2 - propOrg2) < (propEnd - propOrg))
			{
				propOrg = propOrg2;
				propEnd = propEnd2;
				majorSpan = majorSpan2;
				minorTicks = minorTicks2;
			}

			bool hasChanged = xorg != propOrg || xend != propEnd;
			xorg = propOrg;
			xend = propEnd;

			_cachedMajorMinor = new CachedMajorMinor(propOrg, propEnd, majorSpan, minorTicks);

			return hasChanged;
		}








		/// <summary>
		/// calculates the axis org and end using the databounds
		/// the org / end is adjusted only if it is not fixed
		/// and the DataBound object contains valid data
		/// </summary>
		private void InternalCalculateTicks(double org, double end)
		{
			_minorTicks.Clear();
			_majorTicks.Clear();

			if (!(end > org))
				return;


			if (!org.IsFinite())
				throw new ArgumentOutOfRangeException("org is not finite");
			if (!end.IsFinite())
				throw new ArgumentOutOfRangeException("end is not finite");

			if (_cachedMajorMinor != null && _cachedMajorMinor.Org == org && _cachedMajorMinor.End == end)
			{
				_majorSpan = _cachedMajorMinor.MajorSpan;
				_numberOfMinorTicks = _cachedMajorMinor.MinorTicks;
			}
			else
			{
				double propOrg, propEnd;
				CalculateBoundaries(end - org, org, end, false, false, out propOrg, out propEnd, out _majorSpan, out _numberOfMinorTicks);
			}

			_axisOrgByMajor = org / _majorSpan;
			_axisEndByMajor = end / _majorSpan;

			// supress rounding errors
			double spanByMajor = Math.Abs((end - org) / _majorSpan);
			if (_axisOrgByMajor - Math.Floor(_axisOrgByMajor) < 1e-7 * spanByMajor)
				_axisOrgByMajor = Math.Floor(_axisOrgByMajor);
			if (Math.Ceiling(_axisEndByMajor) - _axisEndByMajor < 1e-7 * spanByMajor)
				_axisEndByMajor = Math.Ceiling(_axisEndByMajor);


			InternalCalculateMajorTicks();
			InternalCalculateMinorTicks();
		}


		void InternalCalculateMajorTicks()
		{
			double beg = System.Math.Ceiling(_axisOrgByMajor);
			double end = System.Math.Floor(_axisEndByMajor);
      double llen = end - beg;
      // limit the length to 10000 to limit the amount of space required
      llen = Math.Max(0,Math.Min(llen, _maxSafeNumberOfTicks));
      int len = (int)llen;

			for (int i = 0; i <= len; i++)
			{
				double v = (i+beg) * _majorSpan;

				if (!_suppressedMajorTicks.IsEmpty)
				{
          if (_suppressedMajorTicks.ByValues.Contains(v))
						continue;
          if (_suppressedMajorTicks.ByNumbers.Contains(i))
						continue;
					if (_suppressedMajorTicks.ByNumbers.Contains(i - len - 1))
						continue;
				}

				_majorTicks.Add(v);
			}

			if (!_additionalMajorTicks.IsEmpty)
			{
				foreach (AltaxoVariant v in _additionalMajorTicks.ByValues)
        {
					_majorTicks.Add(v);
        }
			}

		}

		void InternalCalculateMinorTicks()
		{
			if (_numberOfMinorTicks < 2)
				return; // below 2 there are no minor ticks per definition

			double beg = System.Math.Ceiling(_axisOrgByMajor);
			double end = System.Math.Floor(_axisEndByMajor);
			int majorticks = 1 + (int)(end - beg);
			beg = System.Math.Ceiling(_axisOrgByMajor * _numberOfMinorTicks);
			end = System.Math.Floor(_axisEndByMajor * _numberOfMinorTicks);
      double llen = end - beg;
      // limit the length to 10000 to limit the amount of space and time required
      llen = Math.Max(0, Math.Min(llen, _maxSafeNumberOfTicks));
      int len = (int)llen;

      int shift = (int)(beg % _numberOfMinorTicks);

			for (int i = 0; i <= len; i++)
			{
				if ((i+shift) % _numberOfMinorTicks == 0)
					continue;

				double v = (i+beg) * _majorSpan / _numberOfMinorTicks;

				if (!_suppressedMinorTicks.IsEmpty)
				{
          if (_suppressedMinorTicks.ByValues.Contains(v))
						continue;
          if (_suppressedMinorTicks.ByNumbers.Contains(i))
						continue;
          if (_suppressedMinorTicks.ByNumbers.Contains(i - len - 1))
						continue;
				}
				_minorTicks.Add(v);
			}


      if (!_additionalMinorTicks.IsEmpty)
			{
        foreach (AltaxoVariant v in _additionalMinorTicks.ByValues)
        {
          _minorTicks.Add(v);
        }
			}
		}


		static readonly double[] _majorSpanValues = new double[] { 1, 1.5, 2, 2.5, 3, 4, 5, 10 };
		static readonly double[] _minorSpanValues = new double[] { 1, 1.5, 2, 2.5, 3, 4, 5, 10 };

		/// <summary>
		/// Calculates the major span from the scale span, taking into account the setting for targetMajorTicks.
		/// </summary>
		/// <param name="scaleSpan">Scale span (end-origin).</param>
		/// <param name="targetNumberOfMajorTicks">Target number of major ticks.</param>
		public static double CalculateMajorSpan(double scaleSpan, int targetNumberOfMajorTicks)
		{
			if (targetNumberOfMajorTicks <= 0)
				throw new ArgumentOutOfRangeException("targetNumberOfMajorTicks is <= 0");
      if (!(scaleSpan > 0))
        throw new ArgumentOutOfRangeException("scaleSpan must be >0");

			double rawMajorSpan = scaleSpan / targetNumberOfMajorTicks;
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

		/// <summary>
		/// Calculates the number of minor ticks from the major span value and the target number of minor ticks.
		/// </summary>
		/// <param name="majorSpan">Major span value.</param>
		/// <param name="targetNumberOfMinorTicks">Target number of minor ticks.</param>
		/// <returns></returns>
		public static int CalculateMinorTicks(double majorSpan, int targetNumberOfMinorTicks)
		{
			if (targetNumberOfMinorTicks <= 0)
				return 1;
			majorSpan = Math.Abs(majorSpan);
			if (!majorSpan.IsFinite())
				return 1;

			double rawMinorSpan = majorSpan / targetNumberOfMinorTicks;
			int log10RawMinorSpan = (int)Math.Floor(Math.Log10(rawMinorSpan));

			double normMinorSpan = rawMinorSpan / RMath.Pow(10, log10RawMinorSpan); // number between 1 and 10
			for (int i = _minorSpanValues.Length - 1; i >= 0; i--)
			{
				double span = _minorSpanValues[i];
				if (span <= normMinorSpan)
				{
					normMinorSpan = span;
					break;
				}
			}
			double minorSpan = normMinorSpan * RMath.Pow(10, log10RawMinorSpan);
			int result = (int)Math.Round(majorSpan / minorSpan);
			return result < 1 ? 1 : result;
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
		public static double SnapToTick(double x, double majorSpan, int minorTicks, BoundaryTickSnapping snapping, bool upwards)
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
						if(upwards)
							rel = Math.Ceiling(rel);
						else
							rel = Math.Floor(rel);

						if (Math.IEEERemainder(rel, 1) != 0 && minorTicks>1)
							rel = upwards ? rel + 1 : rel - 1;

						return rel * majorSpan/minorTicks;
					}
				case BoundaryTickSnapping.SnapToMinorOrMajor:
					{
						double rel = x * minorTicks / (majorSpan);
						if (upwards)
							return Math.Ceiling(rel) * majorSpan/minorTicks;
						else
							return Math.Floor(rel) * majorSpan/minorTicks;
					}
			}
		}

		/// <summary>
		/// Applies the value for <see cref="MinGrace"/>, <see cref="MaxGrace"/> and <see cref="ZeroLever"/> to the scale and calculated proposed values for the boundaries.
		/// </summary>
		/// <param name="scaleOrg">Scale origin.</param>
		/// <param name="scaleEnd">Scale end.</param>
		/// <param name="isOrgExtendable">True if the scale org can be extended.</param>
		/// <param name="isEndExtendable">True if the scale end can be extended.</param>
		/// <param name="propOrg">Returns the proposed value of the scale origin.</param>
		/// <param name="propEnd">Returns the proposed value of the scale end.</param>
		public void ApplyGraceAndZeroLever(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
		{
			double scaleSpan = Math.Abs(scaleEnd - scaleOrg);
			propOrg = scaleOrg;
			if(isOrgExtendable)
				propOrg -= Math.Abs(_minGrace * scaleSpan);

			propEnd = scaleEnd;
			if(isEndExtendable)
				propEnd += Math.Abs(_maxGrace * scaleSpan);

			double lever = Math.Abs(_zeroLever * scaleSpan);
			double propOrg2 = scaleOrg - lever;
			double propEnd2 = scaleEnd + lever;

			if (isOrgExtendable && propOrg > 0 && propOrg2 <= 0)
				propOrg = 0;

			if (isEndExtendable && propEnd < 0 && propEnd2 >= 0)
				propEnd = 0;


			double range = propEnd - propOrg;
			if (range == 0) // Emergency plan if range is zero

			{
				double extend = propOrg == 0 ? 0.5 : Math.Abs(propOrg / 10);
				if (isOrgExtendable && isEndExtendable)
				{
					propOrg -= extend;
					propEnd += extend;
				}
				else if (isOrgExtendable)
				{
					propOrg -= 2 * extend;
				}
				else if (isEndExtendable)
				{
					propEnd += 2 * extend;
				}
			}
		}

		

		/// <summary>
		/// Calculate the proposed org and end of the scale by using a certain scale span to calculate the major span value and the number of minor ticks.
		/// </summary>
		/// <param name="overrideScaleSpan">Scale span value used to calculate <paramref name="majorSpan"/> and <paramref name="minorTicks"/>.</param>
		/// <param name="scaleOrg">Scale origin.</param>
		/// <param name="scaleEnd">Scale end.</param>
		/// <param name="isOrgExtendable">True if the scale org can be extended.</param>
		/// <param name="isEndExtendable">True if the scale end can be extended.</param>
		/// <param name="propOrg">Returns the proposed value of the scale origin.</param>
		/// <param name="propEnd">Returns the proposed value of the scale end.</param>
		/// <param name="majorSpan">Returns the proposed value of the major span.</param>
		/// <param name="minorTicks">Returns the proposed value of the number of minor ticks.</param>
		public void CalculateBoundaries(double overrideScaleSpan, double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd, out double majorSpan, out int minorTicks)
		{
      if (null != _userDefinedMajorSpan)
        majorSpan = (double)_userDefinedMajorSpan;
      else
      {
        if (!(overrideScaleSpan > 0))
          throw new ArgumentOutOfRangeException("overrideScaleSpan must be >0");

        majorSpan = CalculateMajorSpan(overrideScaleSpan, _targetNumberOfMajorTicks);

      }

			if (null != _userDefinedMinorTicks)
				minorTicks = (int)_userDefinedMinorTicks;
			else
				minorTicks = CalculateMinorTicks(majorSpan, _targetNumberOfMinorTicks);

			CalculateBoundaries(scaleOrg, scaleEnd, isOrgExtendable, isEndExtendable, majorSpan, minorTicks, out propOrg, out propEnd);
		}

		/// <summary>
		/// Calculates the scale boundaries using defined values for majorSpan and minorTicks, taking into accound the settings
		/// for ZeroLever, MinGrace, MaxGrace and the Snapping to ticks
		/// </summary>
		/// <param name="scaleOrg">Scale origin.</param>
		/// <param name="scaleEnd">Scale end.</param>
		/// <param name="isOrgExtendable">True if the scale org can be extended.</param>
		/// <param name="isEndExtendable">True if the scale end can be extended.</param>
		/// <param name="majorSpan">The major span value used for calculation.</param>
		/// <param name="minorTicks">The number of minor ticks used for calculation.</param>
		/// <param name="propOrg">Returns the proposed value of the scale origin.</param>
		/// <param name="propEnd">Returns the proposed value of the scale end.</param>
		public void CalculateBoundaries(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, double majorSpan, int minorTicks, out double propOrg, out double propEnd)
		{
			ApplyGraceAndZeroLever(scaleOrg, scaleEnd, isOrgExtendable, isEndExtendable, out propOrg, out propEnd);

			if (isOrgExtendable)
			{
				double propOrg2 = SnapToTick(scaleOrg, majorSpan, minorTicks, _snapOrgToTick, false);
				double propOrg3 = SnapToTick(propOrg, majorSpan, minorTicks, _snapOrgToTick, false);
				if (propOrg2 <= propOrg)
					propOrg = propOrg2;
				else if (propOrg3 <= propOrg)
					propOrg = propOrg3;
			}

			if (isEndExtendable)
			{
				double propEnd2 = SnapToTick(scaleEnd, majorSpan, minorTicks, _snapEndToTick, true);
				double propEnd3 = SnapToTick(propEnd, majorSpan, minorTicks, _snapEndToTick, true);
				if (propEnd2 >= propEnd)
					propEnd = propEnd2;
				else if (propEnd3 >= propEnd)
					propEnd = propEnd3;
			}
		}


		

	}
}
