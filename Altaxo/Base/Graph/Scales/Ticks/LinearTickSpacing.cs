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
		/// <summary>Proposed value of axis origin, proposed either by the lower physical boundary or by the user (if axis org is fixed).</summary>
		protected double _baseOrg = 0; // proposed value of org
		/// <summary>Proposed value of axis end, proposed either by the upper physical boundary or by the user (if axis end is fixed).</summary>
		protected double _baseEnd = 1; // proposed value of end
		/// <summary>Current axis origin divided by the major tick span value.</summary>
		protected double _axisOrgByMajor = 0;
		/// <summary>Current axis end divided by the major tick span value.</summary>
		protected double _axisEndByMajor = 5;
		/// <summary>Physical span value between two major ticks.</summary>
		protected double _majorSpan = 0.2; // physical span value between two major ticks
		/// <summary>Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!</summary>
		protected int _numberOfMinorTicks = 2;



		// cached values
		/// <summary>Current axis origin (cached value).</summary>
		protected double _cachedAxisOrg = 0;
		/// <summary>Current axis end (cached value).</summary>
		protected double _cachedAxisEnd = 1;
		/// <summary>Current axis span (i.e. end-org) (cached value).</summary>
		protected double _cachedAxisSpan = 1;
		/// <summary>Current inverse of axis span (cached value).</summary>
		protected double _cachedOneByAxisSpan = 1;


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
			this._cachedAxisEnd = from._cachedAxisEnd;
			this._axisEndByMajor = from._axisEndByMajor;
			this._cachedAxisOrg = from._cachedAxisOrg;
			this._axisOrgByMajor = from._axisOrgByMajor;
			this._cachedAxisSpan = from._cachedAxisSpan;
			this._baseEnd = from._baseEnd;
			this._baseOrg = from._baseOrg;
			this._majorSpan = from._majorSpan;
			this._numberOfMinorTicks = from._numberOfMinorTicks;
			this._cachedOneByAxisSpan = from._cachedOneByAxisSpan;

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
			bool hasBoundaryChanged = false;

			if (!(xend > xorg))
				throw new ArgumentOutOfRangeException("xend is not greater than xorg");

			double range = xend - xorg;

			// This is the zero-lever test.  If xorg is within the zero lever fraction
			// of the data range, then use zero.

			if (isOrgExtendable && xorg > 0 && Math.Abs(xorg / range) < ZeroLever)
			{
				xorg = 0;
				isOrgExtendable = false;
				hasBoundaryChanged = true;
			}

			// Zero-lever test for cases where the xend value is less than zero
			if (isEndExtendable && xend < 0 && Math.Abs(xend / range) < ZeroLever)
			{
				xend = 0;
				isEndExtendable = false;
				hasBoundaryChanged = true;
			}

			range = xend - xorg;

			if (range == 0)
			{
				double extend = xorg == 0 ? 0.5 : Math.Abs(xorg / 10);
				if (isOrgExtendable && isEndExtendable)
				{
					xorg -= extend;
					xend += extend;
					hasBoundaryChanged = true;
				}
				else if (isOrgExtendable)
				{
					xorg -= 2 * extend;
					hasBoundaryChanged = true;
				}
				else if (isEndExtendable)
				{
					xend += 2 * extend;
					hasBoundaryChanged = true;
				}
			}

			range = xend - xorg;



			if (range > 0)
			{
				double majorSpan;
				int minorTicks;
				CalculateTicks(xorg, xend, out majorSpan, out minorTicks);
				double axisOrgByMajor;
				double axisEndByMajor;

				if (isOrgExtendable)
				{
					axisOrgByMajor = System.Math.Floor(minorTicks * xorg / majorSpan) / minorTicks;
					double norg = axisOrgByMajor * majorSpan;

					// Compare this new org with a value adjusted by grace
					// Do not let the grace value extend the axis below zero when all the values were positive
					double gorg = xorg;
					if ((xorg < 0 || xorg - _minGrace * range >= 0.0))
						gorg = xorg - _minGrace * range;

					if (gorg < norg) // if the grace adjusted value exceeds the tick adjusted limit, than we adjust the limit further
						axisOrgByMajor = System.Math.Floor(minorTicks * gorg / majorSpan) / minorTicks;

					xorg = axisOrgByMajor * majorSpan;
					hasBoundaryChanged = true;
				}
				else
				{
					axisOrgByMajor = xorg / majorSpan;
				}

				if (isEndExtendable)
				{
					axisEndByMajor = System.Math.Ceiling(minorTicks * xend / majorSpan) / minorTicks;
					double nend = axisEndByMajor * majorSpan;

					// Compare this new end with a value adjusted by grace
					// Do not let the grace value extend the axis above zero when all the values are negative
					double gend = xend;
					if (xend > 0 || xend + _maxGrace * range <= 0.0)
						gend = xend + _maxGrace * range;

					if (gend > nend) // if the grace adjusted value exceeds the tick adjusted limit, than we adjust the limit further
						axisEndByMajor = System.Math.Ceiling(minorTicks * gend / majorSpan) / minorTicks;

					xend = axisEndByMajor * majorSpan;
					hasBoundaryChanged = true;
				}
				else
				{
					axisEndByMajor = xend / majorSpan;
				}

			}


			return hasBoundaryChanged;
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


			CalculateTicks(org, end, out _majorSpan, out _numberOfMinorTicks);

      if (_userDefinedMajorSpan != null)
        _majorSpan = (double)_userDefinedMajorSpan;
      if (_userDefinedMinorTicks != null)
        _numberOfMinorTicks = (int)_userDefinedMinorTicks;

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


		public void InternalCalculateMajorTicks()
		{
			double i, beg, end;
			beg = System.Math.Ceiling(_axisOrgByMajor);
			end = System.Math.Floor(_axisEndByMajor);
			for (i = beg; i <= end; i += 1)
			{
				double v = i * _majorSpan;

				if (!_suppressedMajorTicks.IsEmpty)
				{
          if (_suppressedMajorTicks.ByValues.Contains(v))
						continue;
          if (_suppressedMajorTicks.ByNumbers.Contains((int)(i - beg)))
						continue;
					if (_suppressedMajorTicks.ByNumbers.Contains((int)(i - end - 1)))
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

		public void InternalCalculateMinorTicks()
		{
			double i, beg, end;
			if (_numberOfMinorTicks < 2)
				return; // below 2 there are no minor ticks per definition


			beg = System.Math.Ceiling(_axisOrgByMajor);
			end = System.Math.Floor(_axisEndByMajor);
			int majorticks = 1 + (int)(end - beg);
			beg = System.Math.Ceiling(_axisOrgByMajor * _numberOfMinorTicks);
			end = System.Math.Floor(_axisEndByMajor * _numberOfMinorTicks);
			for (i = beg; i <= end; i += 1)
			{
				if (i % _numberOfMinorTicks == 0)
					continue;

				double v = i * _majorSpan / _numberOfMinorTicks;

				if (!_suppressedMinorTicks.IsEmpty)
				{
          if (_suppressedMinorTicks.ByValues.Contains(v))
						continue;
          if (_suppressedMinorTicks.ByNumbers.Contains((int)(i - beg)))
						continue;
          if (_suppressedMinorTicks.ByNumbers.Contains((int)(i - end - 1)))
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




		static void CalculateTicks(
		double min,                // Minimum of data 
		double max,                // Maximum of data
		out double majorspan,      // the span between two major ticks
		out int minorticks      // number of ticks in a major tick span 
		)
		{
			// Make sure that minVal and maxVal are legitimate values
			if (Double.IsInfinity(min) || Double.IsNaN(min) || min == Double.MaxValue)
				min = 0.0;
			if (Double.IsInfinity(max) || Double.IsNaN(max) || max == Double.MaxValue)
				max = 0.0;

			if (min > max) // should not happen, but can happen when there are no data and min and max are uninitialized 
			{
				min = max = 0;
			}

			double span = max - min; // span width between max and min

			if (0 == span)
			{
				double diff;
				// if span width is zero, then 1% of the velue, in case of min==max==0 we use 1
				if (0 == max || 0 == min) // if one is null, the other should also be null, but to be secure...
					diff = 1;
				else
					diff = System.Math.Abs(min / 100); // we can be sure, that min==max, because span==0

				min -= diff;
				max += diff;

				span = max - min;
			} // if 0==span


			// we have to norm span in that way, that 100<=normspan<1000
			int nSpanPotCorr = (int)(System.Math.Floor(System.Math.Log10(span))) - 2; // nSpanPotCorr will be 0 if 100<=span<1000 
			double normspan = span / TenToThePowerOf(nSpanPotCorr);

			// we divide normspan by 10, 20, 25, 50, 100, 200 and calculate the
			// number of major ticks this will give
			// we can break if the number of major ticks is below 10
			int majornormspan = 1;
			int minornormspan = 1;
			for (int finep = 0; finep <= 5; finep++)
			{
				switch (finep)
				{
					case 0:
						majornormspan = 10;
						minornormspan = 5;
						break;
					case 1:
						majornormspan = 20;
						minornormspan = 10;
						break;
					case 2:
						majornormspan = 25;
						minornormspan = 5;
						break;
					case 3:
						majornormspan = 50;
						minornormspan = 25;
						break;
					case 4:
						majornormspan = 100;
						minornormspan = 50;
						break;
					case 5:
					default:
						majornormspan = 200;
						minornormspan = 100;
						break;
				} // end of switch
				double majorticks = 1 + System.Math.Floor(normspan / majornormspan);
				if (majorticks <= 10)
					break;
			}
			majorspan = majornormspan * TenToThePowerOf(nSpanPotCorr);
			minorticks = (int)(majornormspan / minornormspan);
		} // end of function


		static double TenToThePowerOf(int ii)
		{
			if (ii == 0)
				return 1;
			else if (ii == 1)
				return 10;
			else
			{
				int i = System.Math.Abs(ii);
				int halfi = i / 2;
				double hret = TenToThePowerOf(halfi);
				double ret = (halfi + halfi) == i ? hret * hret : 10 * hret * hret;
				return ii < 0 ? 1 / ret : ret;
			}
		}

	}
}
