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
 * Potrs.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
*/

#if !MANAGED
using System;
using System.Runtime.InteropServices;



namespace Altaxo.Calc.LinearAlgebra.Lapack{
  [System.Security.SuppressUnmanagedCodeSecurityAttribute]
  internal sealed class Potrs {
    private Potrs() {}

    private static void ArgumentCheck(UpLo uplo, int n, int nrhs, object A, int lda, object B, int ldb){
      if (n<0) 
        throw new ArgumentException("n must be at least zero.", "n");
      if (nrhs<0) 
        throw new ArgumentException("nrhs must be at least zero.", "nrhs");
      if (A==null) 
        throw new ArgumentNullException("A","A cannot be null.");
      if (lda<n || lda<1) 
        throw new ArgumentException("lda must be at least max(1,n)");
      if (B==null) 
        throw new ArgumentNullException("B","B cannot be null.");
      if (ldb<n || ldb<1) 
        throw new ArgumentException("ldb must be at least max(1,n)", "ldb");
    }
    
    internal static int Compute( UpLo uplo, int n, int nrhs, float[] A, int lda, float[] B, int ldb  ){
      ArgumentCheck(uplo, n, nrhs, A, lda, B, ldb);
      return dna_lapack_spotrs(uplo, n, nrhs, A, lda, B, ldb);
    }

    internal static int Compute( UpLo uplo, int n, int nrhs, double[] A, int lda, double[] B, int ldb  ){
      ArgumentCheck(uplo, n, nrhs, A, lda, B, ldb);
      return dna_lapack_dpotrs(uplo, n, nrhs, A, lda, B, ldb);
    }

    internal static int Compute( UpLo uplo, int n, int nrhs, ComplexFloat[] A, int lda, ComplexFloat[] B, int ldb  ){
      ArgumentCheck(uplo, n, nrhs, A, lda, B, ldb);
      return dna_lapack_cpotrs(uplo, n, nrhs, A, lda, B, ldb);
    }

    internal static int Compute( UpLo uplo, int n, int nrhs, Complex[] A, int lda, Complex[] B, int ldb  ){
      ArgumentCheck(uplo, n, nrhs, A, lda, B, ldb);
      return dna_lapack_zpotrs(uplo, n, nrhs, A, lda, B, ldb);
    }

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_spotrs( UpLo uplo, int n, int nrsh, [In,Out]float[] A, int lda, [In,Out]float[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_dpotrs( UpLo uplo, int n, int nrsh, [In,Out]double[] A, int lda, [In,Out]double[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_cpotrs( UpLo uplo, int n, int nrsh, [In,Out]ComplexFloat[] A, int lda, [In,Out]ComplexFloat[] B, int ldb );

    [DllImport(Configuration.BLASLibrary, ExactSpelling=true, SetLastError=false)]
    private static extern int dna_lapack_zpotrs( UpLo uplo, int n, int nrsh, [In,Out]Complex[] A, int lda, [In,Out]Complex[] B, int ldb );
  }
}
#endif