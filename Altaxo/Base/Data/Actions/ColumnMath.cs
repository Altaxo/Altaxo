using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public static class ColumnMath
	{
		/// <summary>
		/// Determines if column c has monotonically increasing values.
		/// </summary>
		/// <param name="c">Column to test.</param>
		/// <param name="allowNaN">If true, NaN values will ignored. If false, the function will return false if there are NaN values present.</param>
		/// <returns>True if the values are monotonically increasing.</returns>
		public static bool IsMonotonicallyIncreasing(this DoubleColumn c, bool allowNaN)
		{
			if (c.Count == 0)
				return true;

			int i = 0;
			if (allowNaN)
			{
				for(i=0;i<c.Count;i++)
					if(!double.IsNaN(c[i]))
						break;
			}
			if (i >= c.Count)
				return false;
			
			double start = c[i];
			if (double.IsNaN(start))
				return false;

			for (i++; i < c.Count; i++)
			{
				double next = c[i];
				if (double.IsNaN(next))
				{
					if (allowNaN)
						continue;
					else return false;
				}
				if (!(start <= next))
					return false;
			}
			return true;
		}


			/// <summary>
		/// Determines if column c has monotonically increasing values.
		/// </summary>
		/// <param name="c">Column to test.</param>
		/// <returns>True if the values are monotonically increasing.</returns>
		public static bool IsMonotonicallyIncreasing(this DateTimeColumn c)
		{
			if (c.Count == 0)
				return true;

			DateTime prev = c[0];
			for (int i = 0; i < c.Count; i++)
			{
				DateTime next = c[i];

				if (next >= prev)
				{
					prev = next;
					continue;
				}
				else
					return false;
			}
			return true;
		}
	}
}
