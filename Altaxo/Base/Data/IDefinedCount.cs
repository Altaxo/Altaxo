using System;

namespace Altaxo.Data
{


	/// <summary>
	/// The interface to a column which has a definite number of elements.
	/// </summary>
	public interface IDefinedCount
	{
		/// <summary>
		/// Get the number of elements of the column.
		/// </summary>
		int Count
		{
			get;
		}
	}
}
