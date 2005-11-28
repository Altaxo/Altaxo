#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

/*
 * Potrf.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Potrf {
    private Potrf() {}

    private static void ArgumentCheck(UpLo uplo, int n, object A, int lda){
      if (n<0){ 
        throw new ArgumentException("n must be at least zero.", "n");
      }
      if (A==null){ 
        throw new ArgumentNullException("A","A cannot be null.");
      }
      if (lda<n || lda<1){
        throw new ArgumentException("lda must be at least max(1,n)", "lda");
      }
    }
    
    internal static int Compute( UpLo uplo, int n, float[] A, int lda  ){
      ArgumentCheck(uplo, n, A, lda);
      return dna_lapack_spotrf(uplo, n, A, lda);
    }

    internal static int Compute( UpLo uplo, int n, double[] A, int lda  ){
      ArgumentCheck(uplo, n, A, lda);
      return dna_lapack_dpotrf(uplo, n, A, lda);
    }

    internal static int Compute( UpLo uplo, int n, ComplexFloat[] A, int lda  ){
      ArgumentCheck(uplo, n, A, lda);
      return dna_lapack_cpotrf(uplo, n, A, lda);
    }

    internal static int Compute( UpLo uplo, int n, Complex[] A, int lda  ){
      ArgumentCheck(uplo, n, A, lda);
      return dna_lapack_zpotrf(uplo, n, A, lda);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_spotrf( UpLo uplo, int n, [In,Out]float[] A, int lda );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dpotrf( UpLo uplo, int n, [In,Out]double[] A, int lda );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cpotrf( UpLo uplo, int n, [In,Out]ComplexFloat[] A, int lda );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zpotrf( UpLo uplo, int n, [In,Out]Complex[] A, int lda );
  }
}
#endif
