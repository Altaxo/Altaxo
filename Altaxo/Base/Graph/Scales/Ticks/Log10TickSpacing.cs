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
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	using Altaxo.Calc;
	using Altaxo.Data;

	public class Log10TickSpacing : NumericTickSpacing
	{

		double _log10Org;
		double _log10End;

		/// <summary>Number of decades per major tick.</summary>
		int _decadesPerMajorTick; // how many decades is one major tick
		List<double> _majorTicks;
		List<double> _minorTicks;



		/// <summary>If set, gives the number of minor ticks choosen by the user.</summary>
		int? _userDefinedMinorTicks;

		/// <summary>If set, gives the physical value between two major ticks choosen by the user.</summary>
		int? _userDefinedMajorDecades;

    double _oneLever = 0;
		double _minGrace = 0;
		double _maxGrace = 0;
		int _targetNumberOfMajorTicks = 4;
		int _targetNumberOfMinorTicks = 9;

		double _transformationDivider = 1;
		bool _transformationOperationIsMultiply;
    double _transformationExponent = 1;

		/// <summary>If true, the boundaries will be set on a minor or major tick.</summary>
		BoundaryTickSnapping _snapOrgToTick;
		BoundaryTickSnapping _snapEndToTick;

    SuppressedTicks _suppressedMajorTicks;
    SuppressedTicks _suppressedMinorTicks;
    AdditionalTicks _additionalMajorTicks;
    AdditionalTicks _additionalMinorTicks;

		
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Log10TickSpacing), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Log10TickSpacing s = (Log10TickSpacing)obj;

        info.AddValue("OneLever", s._oneLever);
        info.AddValue("MinGrace", s._minGrace);
        info.AddValue("MaxGrace", s._maxGrace);
        info.AddEnum("SnapOrgToTick", s._snapOrgToTick);
        info.AddEnum("SnapEndToTick", s._snapEndToTick);

				info.AddValue("TargetNumberOfMajorTicks", s._targetNumberOfMajorTicks);
				info.AddValue("TargetNumberOfMinorTicks", s._targetNumberOfMinorTicks);
        info.AddValue("UserDefinedMajorDecades", s._userDefinedMajorDecades);
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
        s._minGrace = info.GetDouble("MinGrace");
        s._maxGrace = info.GetDouble("MaxGrace");
        s._snapOrgToTick = (BoundaryTickSnapping)info.GetEnum("SnapOrgToTick", typeof(BoundaryTickSnapping));
        s._snapEndToTick = (BoundaryTickSnapping)info.GetEnum("SnapEndToTick", typeof(BoundaryTickSnapping));
      
				s._targetNumberOfMajorTicks = info.GetInt32("TargetNumberOfMajorTicks");
				s._targetNumberOfMinorTicks = info.GetInt32("TargetNumberOfMinorTicks");
        s._userDefinedMajorDecades = info.GetNullableInt32("UserDefinedMajorDecades");
        s._userDefinedMinorTicks = info.GetNullableInt32("UserDefinedMinorTicks");

        s._transformationExponent = info.GetDouble("TransformationExponent");
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

		public Log10TickSpacing()
		{
			_decadesPerMajorTick = 1;
			_majorTicks = new List<double>();
			_minorTicks = new List<double>();
      _suppressedMajorTicks = new SuppressedTicks();
      _suppressedMinorTicks = new SuppressedTicks();
      _additionalMajorTicks = new AdditionalTicks();
      _additionalMinorTicks = new AdditionalTicks();
		}

		public Log10TickSpacing(Log10TickSpacing from)
			: this()
		{
			CopyFrom(from);
		}

		public void CopyFrom(Log10TickSpacing from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			_log10Org = from._log10Org;
			_log10End = from._log10End;
			_decadesPerMajorTick = from._decadesPerMajorTick;

			_majorTicks.Clear();
			_majorTicks.AddRange(from._majorTicks);

			_minorTicks.Clear();
			_minorTicks.AddRange(from._minorTicks);

      _suppressedMajorTicks = (SuppressedTicks)from._suppressedMajorTicks.Clone();
      _suppressedMinorTicks = (SuppressedTicks)from._suppressedMinorTicks.Clone();
      _additionalMajorTicks = (AdditionalTicks)from._additionalMajorTicks.Clone();
      _additionalMinorTicks = (AdditionalTicks)from._additionalMinorTicks.Clone();
		}


		public override object Clone()
		{
			return new Log10TickSpacing(this);
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
			// we don't want to change the boundaries, so we return false;
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

			InternalCalculateTicks(dorg, dend);
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

			_log10Org = Math.Log10(org);
			_log10End = Math.Log10(end);

			if (_log10Org == _log10End)
				return;

			if (!_log10Org.IsFinite())
				throw new ArgumentOutOfRangeException("Log10(org) is not finite");
			if (!_log10End.IsFinite())
				throw new ArgumentOutOfRangeException("Log10(end) is not finite");
			if(!(_log10Org<_log10End))
				throw new ArgumentException("Log10(org) is not less than Log10(end)");


			// calculate the number of decades between end and org
			double decades = Math.Abs(_log10End - _log10Org);

			// limit the number of major ticks to about 10

			_decadesPerMajorTick = (int)Math.Ceiling(decades / 10.0);

			InternalCalculateMajorTicks();
			InternalCalculateMinorTicks();
		}

		private void InternalCalculateMajorTicks()
		{
			_majorTicks.Clear();

			// calculate the number of major ticks

			int nFullDecades = (int)(1 + Math.Floor(_log10End) - Math.Ceiling(_log10Org));
			int nMajorTicks = (int)Math.Floor((nFullDecades + _decadesPerMajorTick - 1) / (double)_decadesPerMajorTick);


			int beg = (int)Math.Ceiling(_log10Org);
			int end = (int)Math.Floor(_log10End);

			for (int i = beg; i <= end; i += _decadesPerMajorTick)
			{
				_majorTicks.Add(Calc.RMath.Pow(10, i));
			}
		}

		private void InternalCalculateMinorTicks()
		{
			_minorTicks.Clear();

			double decadespan = Math.Abs(_log10Org - _log10End);

			// and calculate begin and end of minor ticks
			int majorcount = _majorTicks.Count;


			// guess from the span the tickiness (i.e. the increment of the multiplicator)
			// so that not more than 50 minor ticks are visible
			double minorsperdecade = 50.0 / decadespan;

			// when there is more than one decade per major tick,
			// then allow only minor ticks onto a full decade
			if (this._decadesPerMajorTick > 1)
			{
				int decadesPerMinor;
				for (decadesPerMinor = 1; decadesPerMinor < _decadesPerMajorTick; decadesPerMinor++)
				{
					if (0 != (_decadesPerMajorTick % decadesPerMinor))
						continue;
					double resultingMinors = decadespan / decadesPerMinor;
					if (resultingMinors < 50)
						break;
				}
				if (decadesPerMinor == _decadesPerMajorTick)
					return; // no minor ticks at all

				int log10firstmajor = (int)Math.Floor(Math.Log10(_majorTicks[0]) + 0.125);
				int beg = (int)Math.Ceiling((_log10Org - log10firstmajor) / decadesPerMinor);
				int end = (int)Math.Floor((_log10End - log10firstmajor) / decadesPerMinor);


				for (int i = beg, j = 0; i <= end; i++)
				{
					double result = Calc.RMath.Pow(10, log10firstmajor + i * decadesPerMinor);
					double logdiff = Math.Log10(result) - Math.Log10(_majorTicks[j]);
					if (Math.Abs(logdiff) < 0.125)
					{
						if ((j + 1) < majorcount)
							j++;
						continue;
					}
					_minorTicks.Add(result);
				}
			}
			else // decadesPerMajorTick==1
			{

				// do not allow more than 10 minors per decade than 
				if (decadespan > 0.3 && minorsperdecade > 9)
					minorsperdecade = 9;

				// if minorsperdecade is lesser than one, we dont have minors, so we can
				// return an empty field
				if (minorsperdecade <= 1)
					return; // no minor ticks at all


				// ensure the minorsperdecade are one of the following values
				// 3,9,..
				double dec = 1;
				double minormax = 1;
				for (int i = 0; ; i++)
				{
					double val;
					switch (i % 2)
					{
						default:
						case 0: val = 3 * dec; break;
						case 1: val = 9 * dec; dec *= 10; break;
					}
					if (val <= minorsperdecade)
					{
						minormax = val;
					}
					else
						break;
				}
				minorsperdecade = minormax;
				// now if minorsperdecade is at least 2, it is a good "even" value



				// of cause this increment is only valid in the decade between 1 and 10
				double minorincrement = 9 / (minorsperdecade);

				// there are two cases now, either we have at least one major tick,
				// then we have two different decades on left and right of the axis,
				// or there is no major tick, so the whole axis is in the same decade
				if (majorcount >= 1) // the "normal" case
				{
					int i, j, k;
					// count the ticks on left of the axis
					// note: we normalized so that the "lesser values" are on the left
					double org = Math.Pow(10, _log10Org);
					double firstmajor = _majorTicks[0];
					for (i = 1; firstmajor * (1 - i * minorincrement / 10) >= org; i++) { }
					int leftminorticks = i - 1;

					// count the ticks on the right of the axis
					double end = Math.Pow(10, _log10End);
					double lastmajor = _majorTicks[majorcount - 1];
					for (i = 1; lastmajor * (1 + i * minorincrement) <= end; i++) { }
					int rightminorticks = i - 1;


					// calculate the total minorticks count

					// now fill the array
					for (j = 0, i = leftminorticks; i > 0; j++, i--)
						_minorTicks.Add( firstmajor * (1 - i * minorincrement / 10) );

					for (k = 0; k < (majorcount - 1); k++)
					{
						for (i = 1; i < minorsperdecade; j++, i++)
							_minorTicks.Add( _majorTicks[k] * (1 + i * minorincrement) );
					}
					for (i = 1; i <= rightminorticks; j++, i++)
						_minorTicks.Add(lastmajor * (1 + i * minorincrement));

				}
				else // in case there is no major tick
				{

					// determine the upper decade (major tick)
					double firstmajor = Math.Pow(10, Math.Floor(_log10Org));
					double groundpow = Math.Floor(_log10Org);
					double norg = Math.Pow(10, _log10Org - groundpow);
					double nend = Math.Pow(10, _log10End - groundpow);

					// norg and nend now is between 1 and 10
					// so calculate directly the indices
					double firstidx = Math.Ceiling(norg / minorincrement);
					double lastidx = Math.Floor(nend / minorincrement);

					// do not do anything if something goes wrong
					if ((lastidx < firstidx) || ((lastidx - firstidx) > 100))
					{
						_minorTicks.Clear();
						return;
					}


					// fill the array
					int j;
					double di;
					for (j = 0, di = firstidx; di <= lastidx; j++, di += 1)
						_minorTicks.Add( firstmajor * (di * minorincrement) );
				}
			}
		}
	}
}
