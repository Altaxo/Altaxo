using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo
{
	/// <summary>
	/// Differnent types of norms of a matrix.
	/// </summary>
	public enum MatrixNorm
	{
		/// <summary> 1-norm, the largest column sum of the absolute values of the matrix.</summary>
		M1Norm,

		/// <summary>2-norm, the largest singular value of the matrix</summary>
		M2Norm,

		/// <summary>Frobenius norm of the matrix A, i.e. sqrt (sum (diag (A' * A)))</summary>
		MFroebeniusNorm,

		/// <summary>Infinity norm, the largest row sum of the absolute values of the matrix.</summary>
		MInfinityNorm
	}
}