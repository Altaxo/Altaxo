using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
	/// <summary>
	/// Designates the mode for enumeration and mapping of matrix elements.
	/// </summary>
	public enum Zeros
	{
		/// <summary>
		/// Allow skipping zero entries, without enforcing it. When enumerating or mapping sparse or banded matrices, this can speed up operations.
		/// </summary>
		AllowSkip,

		/// <summary>
		/// Force applying the operation to all fields even if they are zero.
		/// </summary>
		Include,

		/// <summary>
		/// Force applying the operations to all fields in the diagonal even if they are zero, additionally to all non-zero fields.
		/// </summary>
		AllowSkipButIncludeDiagonal
	}
}