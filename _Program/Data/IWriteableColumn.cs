using System;

namespace Altaxo.Data
{
	
	/// <summary>
	/// A column, for which the elements can be set by assigning a AltaxoVariant to a element at index i.
	/// </summary>
	public interface IWriteableColumn : ICloneable
	{
		/// <summary>
		/// Indexer property for setting the element at index i by a AltaxoVariant.
		/// This function should throw an exeption, if the type of the variant do not match
		/// the type of the column.
		/// </summary>
		AltaxoVariant this[int i] 
		{
			set;
		}
	}

}
