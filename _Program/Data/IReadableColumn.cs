using System;

namespace Altaxo.Data
{


	/// <summary>
	/// This designates a vector structure, which holds elements. A single element at a given index can be read out
	/// by returning a AltaxoVariant.
	/// </summary>
	public interface IReadableColumn : ICloneable
	{

		/// <summary>
		/// The indexer property returns the element at index i as an AltaxoVariant.
		/// </summary>
		AltaxoVariant this[int i] 
		{
			get;
		}

		/// <summary>
		/// Returns true, if the value at index i of the column
		/// is null or invalid or in another state comparable to null or empty
		/// </summary>
		/// <param name="i">The index to the element.</param>
		/// <returns>true if element is null/empty, false if the element is valid</returns>
		bool IsElementEmpty(int i);

		/// <summary>
		/// FullName returns a descriptive name for a column
		/// for columns which belongs to a table, the table name and the column
		/// name, separated by a backslash, should be returned
		/// for other columns, a descriptive name should be returned so that the
		/// user knows the location of this column
		/// </summary>
		string FullName
		{
			get;
		}
	}

}
