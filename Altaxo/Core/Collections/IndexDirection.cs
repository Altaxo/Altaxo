using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	/// <summary>
	/// Direction in which list elements will be handled.
	/// </summary>
	public enum IndexDirection
	{
		/// <summary>
		/// List elements will be handled from the lowest index to the highest index.
		/// </summary>
		Ascending,

		/// <summary>
		/// List elements will be handled from the highest index to the lowest index.
		/// </summary>
		Descending
	}
}