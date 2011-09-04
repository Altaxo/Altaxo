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

using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
	public class DateTimeTickSpacing : TickSpacing
	{
		#region Inner classes
		
		enum Unit { Span, Month, Years }

		struct SpanCompound
		{
			public Unit _unit;
			public TimeSpan _span;


			public SpanCompound(Unit unit, TimeSpan span)
			{
				_unit = unit;
				_span = span;
			}

			public SpanCompound(Unit unit, long val)
			{
				_unit = unit;
				_span = TimeSpan.FromTicks(val);
			}

			/// <summary>
			/// This returns the next number k with k greater or equal i, and k mod n == 0. 
			/// </summary>
			/// <param name="i">The number to round up.</param>
			/// <param name="n">The rounding step.</param>
			/// <returns></returns>
			public static int RoundUp(int i, int n)
			{
				n = Math.Abs(n);
				int r = i % n;
				return r == 0 ? i : i + n - r;
			}
			/// <summary>
			/// This returns the next number k with k lesser or equal i, and k mod n == 0. 
			/// </summary>
			/// <param name="i">The number to round down.</param>
			/// <param name="n">The rounding step.</param>
			/// <returns></returns>
			public static int RoundDown(int i, int n)
			{
				n = Math.Abs(n);
				int r = i % n;
				return r == 0 ? i : i - r;
			}

			public DateTime RoundUp(DateTime d)
			{
				switch (_unit)
				{
					case Unit.Span:
						return RoundUpSpan(d);
					case Unit.Month:
						return RoundUpMonths(d);
					case Unit.Years:
						return RoundUpYears(d);
				}
				return d;
			}

			public DateTime RoundDown(DateTime d)
			{
				switch (_unit)
				{
					case Unit.Span:
						return RoundDownSpan(d);
					case Unit.Month:
						return RoundDownMonths(d);
					case Unit.Years:
						return RoundDownYears(d);
				}
				return d;
			}


			DateTime RoundUpSpan(DateTime d)
			{
				long dd = (d - DateTime.MinValue).Ticks;
				long rn = _span.Ticks;
				long re = dd % rn;
				return DateTime.MinValue + TimeSpan.FromTicks(re == 0 ? dd : dd + rn - re);
			}
			DateTime RoundDownSpan(DateTime d)
			{
				long dd = (d - DateTime.MinValue).Ticks;
				long rn = _span.Ticks;
				long re = dd % rn;
				return DateTime.MinValue + TimeSpan.FromTicks(re == 0 ? dd : dd - re);
			}
			DateTime RoundUpMonths(DateTime d)
			{
				int m = (int)_span.Ticks;
				System.Diagnostics.Debug.Assert(m > 0 && m <= 12);
				for (DateTime td = new DateTime(d.Year, d.Month, 1); ; td = td.AddMonths(1))
				{
					if (td >= d && 0 == ((td.Month - 1) % m))
						return td;
				}
			}

			DateTime RoundDownMonths(DateTime d)
			{
				int m = (int)_span.Ticks;
				System.Diagnostics.Debug.Assert(m > 0 && m <= 12);
				for (DateTime td = new DateTime(d.Year, d.Month, 1); ; td = td.AddMonths(-1))
				{
					if (td <= d && 0 == ((td.Month - 1) % m))
						return td;
				}
			}

			DateTime RoundUpYears(DateTime d)
			{
				int m = (int)_span.Ticks;
				return new DateTime(Altaxo.Calc.Rounding.RoundUp(d.Year + 1, m), 1, 1);
			}
			DateTime RoundDownYears(DateTime d)
			{
				int m = (int)_span.Ticks;
				return new DateTime(Altaxo.Calc.Rounding.RoundDown(d.Year, m), 1, 1);
			}
		}

		#endregion

		#region static fields
		readonly static TimeSpan[] _fixedSpan =
      {
        TimeSpan.FromTicks(1),
        TimeSpan.FromTicks(2),
        TimeSpan.FromTicks(4),
        TimeSpan.FromTicks(5),
        TimeSpan.FromTicks(10),
        TimeSpan.FromTicks(20),
        TimeSpan.FromTicks(40),
        TimeSpan.FromTicks(50),
        TimeSpan.FromTicks(100),
        TimeSpan.FromTicks(200),
        TimeSpan.FromTicks(400),
        TimeSpan.FromTicks(500),
        TimeSpan.FromTicks(1000),
        TimeSpan.FromTicks(2000),
        TimeSpan.FromTicks(4000),
        TimeSpan.FromTicks(5000),
        TimeSpan.FromMilliseconds(1),
        TimeSpan.FromMilliseconds(2),
        TimeSpan.FromMilliseconds(4),
        TimeSpan.FromMilliseconds(5),
        TimeSpan.FromMilliseconds(10),
        TimeSpan.FromMilliseconds(20),
        TimeSpan.FromMilliseconds(40),
        TimeSpan.FromMilliseconds(50),
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(400),
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(20),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(4),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromMinutes(20),
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(4),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(12),
        TimeSpan.FromDays(1),
        TimeSpan.FromDays(2),
        TimeSpan.FromDays(4),
        TimeSpan.FromDays(5),
        TimeSpan.FromDays(10),
        TimeSpan.FromDays(20)
      };

		#endregion

		List<AltaxoVariant> _majorTicks;
		List<AltaxoVariant> _minorTicks;


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeTickSpacing), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DateTimeTickSpacing s = (DateTimeTickSpacing)obj;


			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DateTimeTickSpacing s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual DateTimeTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DateTimeTickSpacing s = null != o ? (DateTimeTickSpacing)o : new DateTimeTickSpacing();

				return s;
			}
		}
		#endregion



		public DateTimeTickSpacing()
		{
			_majorTicks = new List<AltaxoVariant>();
			_minorTicks = new List<AltaxoVariant>();
		}


		public DateTimeTickSpacing(DateTimeTickSpacing from)
		{
			_majorTicks = new List<AltaxoVariant>();
			_majorTicks.AddRange(from._majorTicks);

			_minorTicks = new List<AltaxoVariant>();
			_minorTicks.AddRange(from._minorTicks);
		}

		public override object Clone()
		{
			return new DateTimeTickSpacing(this);
		}

		public override bool PreProcessScaleBoundaries(ref AltaxoVariant org, ref AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
		{
			return false;
		}

		public override void FinalProcessScaleBoundaries(AltaxoVariant org, AltaxoVariant end, Scale scale)
		{
			DateTime dorg;
			DateTime dend;
			if (org.IsType(AltaxoVariant.Content.VDateTime))
				dorg = (DateTime)org;
			else if (org.CanConvertedToDouble)
				dorg = new DateTime((long)(org.ToDouble() * 1E7));
			else
				throw new ArgumentException("Variant org is not a DateTime nor a numeric value");

			if (end.IsType(AltaxoVariant.Content.VDateTime))
				dend = (DateTime)end;
			else if (end.CanConvertedToDouble)
				dend = new DateTime((long)(end.ToDouble() * 1E7));
			else
				throw new ArgumentException("Variant end is not a DateTime nor a numeric value");


			InternalFinalProcessScaleBoundaries(dorg, true, dend, true);
		}

		public override AltaxoVariant[] GetMajorTicksAsVariant()
		{
			return _majorTicks.ToArray();
		}

		public override AltaxoVariant[] GetMinorTicksAsVariant()
		{
			return _minorTicks.ToArray();
		}

		public void InternalFinalProcessScaleBoundaries(DateTime org, bool orgfixed, DateTime end, bool endfixed)
		{
			SpanCompound majorSpan;
			int numberOfMinorTicks;
			CalculateTicks(org, end, out majorSpan, out numberOfMinorTicks);
			InternalGetMajorTicks(org, end, majorSpan);
			InternalGetMinorTicks(org, end, majorSpan, numberOfMinorTicks);
		}


		void InternalGetMajorTicks(DateTime org, DateTime end, SpanCompound majorSpan)
		{
			_majorTicks.Clear();
			for (DateTime d = org; ; )
			{

				DateTime r = majorSpan.RoundUp(d);
				if (!(r <= end))
					break;
				_majorTicks.Add(r);
				d = r.AddTicks(1);
			}
		}

		void InternalGetMinorTicks(DateTime org, DateTime end, SpanCompound majorSpan, int numberOfMinorTicks)
		{
			_minorTicks.Clear();
		}

		static void CalculateTicks(
			DateTime min,                // Minimum of data 
			DateTime max,                // Maximum of data
			out SpanCompound majorspan,      // the span between two major ticks
			out int minorticks      // number of ticks in a major tick span 
			)
		{
			if (min > max) // should not happen, but can happen when there are no data and min and max are uninitialized 
			{
				min = max = DateTime.MinValue;
			}

			TimeSpan span = max - min; // span width between max and min

			if (0 == span.Ticks)
			{
				TimeSpan diff;
				// if span width is zero, then 1% of the velue, in case of min==max==0 we use 1
				if (DateTime.MinValue == max || DateTime.MinValue == min) // if one is null, the other should also be null, but to be secure...
					diff = TimeSpan.FromSeconds(1);
				else
					diff = TimeSpan.FromDays(1); // wir can be sure, that min==max, because span==0

				min -= diff;
				max += diff;

				span = max - min;
			} // if 0==span


			if (span > _fixedSpan[_fixedSpan.Length - 1])
			{
				if (span >= TimeSpan.FromDays(365 * 4 + 1))
				{
					minorticks = 0;
					majorspan = CalculateYearTicks(span);
				}
				else
				{
					minorticks = 0;
					majorspan = CalculateMonthTicks(span);
				}
			}
			else
			{
				int i = _fixedSpan.Length - 1;
				TimeSpan quarterspan = new TimeSpan(span.Ticks / 4);
				for (i = _fixedSpan.Length - 1; i >= 0; i--)
				{
					if (_fixedSpan[i] < quarterspan)
						break;
				}
				minorticks = 0;
				majorspan = new SpanCompound(Unit.Span, _fixedSpan[Math.Max(i, 0)]);
				if (span < TimeSpan.FromTicks(6 * majorspan._span.Ticks))
					minorticks = 1;
			}
		} // end of function


		static SpanCompound CalculateYearTicks(TimeSpan span)
		{
			double years = span.TotalDays / (4 * 365 + 1);
			long yearexpo = 1;
			long yearmantissa = 1;
			for (; ; )
			{
				if (years <= 4)
				{
					yearmantissa = 1;
					break;
				}
				else if (years <= 8)
				{
					yearmantissa = 2;
					break;
				}
				else if (years <= 16)
				{
					yearmantissa = 4;
					break;
				}
				else if (years <= 20)
				{
					yearmantissa = 4;
					break;
				}

				else
				{
					years /= 10;
					yearexpo *= 10;
				}
			}
			return new SpanCompound(Unit.Years, yearexpo * yearmantissa);
		}

		const double _DaysPerMonth = 31;
		static SpanCompound CalculateMonthTicks(TimeSpan span)
		{
			double months = span.TotalDays / _DaysPerMonth;
			long m;

			if (months <= 4)
				m = 1;
			else if (months <= 8)
				m = 2;
			else if (months <= 12)
				m = 3;
			else if (months <= 16)
				m = 4;
			else if (months <= 24)
				m = 6;
			else if (months <= 48)
				m = 12;
			else
				return CalculateYearTicks(span);

			return new SpanCompound(Unit.Month, m);
		}

	}
}
