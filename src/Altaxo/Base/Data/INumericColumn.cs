using System;

namespace Altaxo.Data
{


	/// <summary>
	/// This is a column with elements, which can be treated as numeric values. This is truly the case
	/// for columns which hold integer values or floating point values. Also true for DateTime columns, since they
	/// can converted in seconds since a given reference date.
	/// </summary>
	public interface INumericColumn : IReadableColumn, ICloneable
	{
		/// <summary>
		/// Returns the value of a column element at index i as numeric value (double).
		/// </summary>
		/// <param name="i">The index to the column element.</param>
		/// <returns>The value of the column element as double value.</returns>
		double GetDoubleAt(int i);
	}

}
