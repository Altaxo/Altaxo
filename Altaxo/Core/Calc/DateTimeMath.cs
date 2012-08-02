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
using System.Diagnostics;

namespace Altaxo.Calc
{
	public static class DateTimeMath
	{
		/// <summary>
		/// The date x is rounded down to the start of a year. If x designates exactly the start of a year, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded down to the start of a year. If <paramref name="x"/> designates exactly the start of a year, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundDownToStartOfYear(DateTime x)
		{
			return new DateTime(x.Year, 1, 1, 0, 0, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded up to the start of a year. If x designates exactly the start of a year, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded up to the next start of a year. If <paramref name="x"/> designates exactly the start of a year, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundUpToStartOfYear(DateTime x)
		{
			if (x == RoundDownToStartOfYear(x))
				return x;
			else
				return new DateTime(x.Year + 1, 1, 1, 0, 0, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded down to the start of a month. If x designates exactly the start of a month, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded down to the start of a month. If <paramref name="x"/> designates exactly the start of a month, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundDownToStartOfMonth(DateTime x)
		{
			return new DateTime(x.Year, x.Month, 1, 0, 0, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded up to the start of a month. If x designates exactly the start of a month, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded up to the next start of a month. If <paramref name="x"/> designates exactly the start of a month, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundUpToStartOfMonth(DateTime x)
		{
			if (x == RoundDownToStartOfMonth(x))
				return x;
			var month = x.Month + 1;
			return new DateTime(x.Year + (month > 12 ? 1 : 0), month > 12 ? month - 12 : month, 1, 1, 0, 0, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded down to the start of a day. If x designates exactly the start of a day, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded down to the start of a day. If <paramref name="x"/> designates exactly the start of a day, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundDownToStartOfDay(DateTime x)
		{
			return new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded up to the start of a day. If x designates exactly the start of a day, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded up to the next start of a day. If <paramref name="x"/> designates exactly the start of a day, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundUpToStartOfDay(DateTime x)
		{
			DateTime start = RoundDownToStartOfDay(x);
			if (x == start)
				return x;
			return start.AddDays(1);
		}

		/// <summary>
		/// The date x is rounded down to the start of an hour. If x designates exactly the start of an hour, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded down to the start of an hour. If <paramref name="x"/> designates exactly the start of an hour, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundDownToStartOfHour(DateTime x)
		{
			return new DateTime(x.Year, x.Month, x.Day, x.Hour, 0, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded up to the start of an hour. If x designates exactly the start of an hour, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded up to the next start of an hour. If <paramref name="x"/> designates exactly the start of an hour, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundUpToStartOfHour(DateTime x)
		{
			DateTime start = RoundDownToStartOfHour(x);
			if (x == start)
				return x;
			return start.AddHours(1);
		}

		/// <summary>
		/// The date x is rounded down to the start of a minute. If x designates exactly the start of a minute, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded down to the start of a minute. If <paramref name="x"/> designates exactly the start of a minute, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>
		public static DateTime RoundDownToStartOfMinute(DateTime x)
		{
			return new DateTime(x.Year, x.Month, x.Day, x.Hour, x.Minute, 0, x.Kind);
		}

		/// <summary>
		/// The date x is rounded up to the start of a minute. If x designates exactly the start of a minute, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded up to the next start of a minute. If <paramref name="x"/> designates exactly the start of a minute, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundUpToStartOfMinute(DateTime x)
		{
			DateTime start = RoundDownToStartOfMinute(x);
			if (x == start)
				return x;
			return start.AddMinutes(1);
		}

		/// <summary>
		/// The date x is rounded down to the start of a second. If x designates exactly the start of a second, then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded down to the start of a second. If <paramref name="x"/> designates exactly the start of a second, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundDownToStartOfSecond(DateTime x)
		{
			return new DateTime(x.Year, x.Month, x.Day, x.Hour, x.Minute, x.Second, x.Kind);
		}

		/// <summary>
		/// The date x is rounded up to the start of a second. If x designates exactly the start of a second , then x is returned unchanged.
		/// </summary>
		/// <param name="x">The date argument.</param>
		/// <returns>
		///  The date x is rounded up to the next start of a second. If <paramref name="x"/> designates exactly the start of a second, then x is returned unchanged. 
		/// The time zone information of the return value is copied from <paramref name="x"/>.
		/// </returns>	
		public static DateTime RoundUpToStartOfSecond(DateTime x)
		{
			DateTime start = RoundDownToStartOfSecond(x);
			if (x == start)
				return x;
			return start.AddSeconds(1);
		}

		/// <summary>Rounds down the date so that it starts at the first day of a year and is less than or equal to <paramref name="x"/>. 
		/// The resulting year number then is k*<paramref name="n"/>, with k being an integer.</summary>
		/// <param name="x">The date.</param>
		/// <param name="n">The number of years to round down. Must be greater than or equal to 1.</param>
		/// <returns>A date &lt;=<paramref name="x"/>, and with a year number which is k*<paramref name="n"/>.</returns>
		public static DateTime RoundDownYears(DateTime x, int n)
		{
			return new DateTime(Altaxo.Calc.Rounding.RoundDown(x.Year, n), 1, 1, 0, 0, 0, x.Kind);
		}



		/// <summary>Rounds up the date so that it starts at the first day of a year and is greater than or equal to <paramref name="x"/>. 
		/// The resulting year number then is k*<paramref name="n"/>, with k being an integer.</summary>
		/// <param name="x">The date.</param>
		/// <param name="n">The number of years to round up. Must be greater than or equal to 1.</param>
		/// <returns>A date &gt;=<paramref name="x"/>, and with a year number which is k*<paramref name="n"/>, with k being an integer.</returns>
		public static DateTime RoundUpYears(DateTime x, int n)
		{
			var beginningOfThisYear = RoundDownToStartOfYear(x);
			return new DateTime(Altaxo.Calc.Rounding.RoundUp(x.Year + (x == beginningOfThisYear ? 0 : 1), n), 1, 1, 0, 0, 0, x.Kind);
		}



		/// <summary>Rounds down the date so that it starts at the first day of a month and is less than or equal to <paramref name="x"/>. 
		/// The resulting month number then is 1 + k*<paramref name="n"/>, with k being an integer.</summary>
		/// <param name="x">The date.</param>
		/// <param name="n">The number of months to round down. Must be greater than or equal to 1.</param>
		/// <returns>A date &lt;=<paramref name="x"/>, and with a month number which is 1 + k*<paramref name="month"/>. If the resulting month is &lt;1, the year number is decreased accordingly.</returns>
		public static DateTime RoundDownMonths(DateTime x, int n)
		{
			if (n <= 0)
				return x;

			int totalMonth = Altaxo.Calc.Rounding.RoundDown((x.Year - 1) * 12 + (x.Month - 1), n);

			if (totalMonth < 0)
				return DateTime.MinValue;
			else
				return new DateTime(1+(totalMonth/12), 1+(totalMonth%12), 1, 0, 0, 0, x.Kind);
		}



		/// <summary>Rounds up the date so that it starts at the first day of a month and is greater than or equal to <paramref name="x"/>. 
		/// The resulting month number then is 1 + k*<paramref name="n"/>, with k being an integer.</summary>
		/// <param name="x">The date.</param>
		/// <param name="n">The number of months to round up. Must be greater than or equal to 1.</param>
		/// <returns>A date &gt;=<paramref name="x"/>, and with a month number which is 1 + k*<paramref name="month"/>. If the resulting month is &gt;12, the year number is increased accordingly.</returns>
		public static DateTime RoundUpMonths(DateTime x, int n)
		{
			int totalMonth = Altaxo.Calc.Rounding.RoundUp((x.Year - 1) * 12 + (x.Month - 1), n);

			if (totalMonth < 0)
				return DateTime.MinValue;
			else
				return new DateTime(1 + (totalMonth / 12), 1 + (totalMonth % 12), 1, 0, 0, 0, x.Kind);
		}


		private static DateTime GetStartOffset(DateTime d, TimeSpan span)
		{
			DateTime start;

			if (span >= TimeSpan.FromDays(7))
				start = RoundDownToStartOfYear(d);
			else if (span >= TimeSpan.FromDays(1))
				start = RoundDownToStartOfMonth(d);
			else if (span >= TimeSpan.FromHours(1))
				start = RoundDownToStartOfDay(d);
			else if (span >= TimeSpan.FromMinutes(1))
				start = RoundDownToStartOfHour(d);
			else if (span >= TimeSpan.FromSeconds(1))
				start = RoundDownToStartOfMinute(d);
			else
				start = RoundDownToStartOfSecond(d);
			return start;
		}


		public static DateTime RoundUpSpan(DateTime d, TimeSpan span)
		{
			//if span is a day or greater, it should be rounded with respect to the start of the month
			// if span is an hour or greater, it should be rounded with respect to midnight
			// if span is a minute or greater, it should be rounded with respect to the beginning of the hour
			// if span is a second or greater, it should be rounded with respect to the beginning of the minute
			// else it should be rounded with respect to the beginning of the second

			DateTime start = GetStartOffset(d, span);
			long tickdiff = (d - start).Ticks;
			long roundedTicks = Altaxo.Calc.Rounding.RoundUp(tickdiff, span.Ticks);
			return start + TimeSpan.FromTicks(roundedTicks);
		}


		public static DateTime RoundDownSpan(DateTime d, TimeSpan span)
		{
			//if span is a day or greater, it should be rounded with respect to the start of the month
			// if span is an hour or greater, it should be rounded with respect to midnight
			// if span is a minute or greater, it should be rounded with respect to the beginning of the hour
			// if span is a second or greater, it should be rounded with respect to the beginning of the minute
			// else it should be rounded with respect to the beginning of the second

			DateTime start = GetStartOffset(d, span);
			long tickdiff = (d - start).Ticks;
			long roundedTicks = Altaxo.Calc.Rounding.RoundDown(tickdiff, span.Ticks);
			return start + TimeSpan.FromTicks(roundedTicks);
		}

	}
}
