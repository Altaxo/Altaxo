#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

#endregion Copyright

namespace Altaxo.Calc.LinearAlgebra.Factorization
{
  public class PrincipalComponentAnalysis
  {


    public static (Matrix<double> factors, Matrix<double> loadings) BySVD(Matrix<double> data, int numberOfComponents)
    {
      int numberOfSpectra = data.RowCount;
      int spectralPoints = data.ColumnCount;

      var svd = data.Svd();

      var mfactors = CreateMatrix.Dense<double>(numberOfSpectra, numberOfComponents);
      for (int i = 0; i < numberOfComponents; i++)
      {
        var singularValue = svd.S[i];
        for (int r = 0; r < numberOfSpectra; r++)
        {
          mfactors[r, i] = svd.U[r, i] * singularValue;
        }
      }
      var mloads = svd.VT.SubMatrix(0, numberOfComponents, 0, spectralPoints);
      return (mfactors, mloads);
    }
  }
}
