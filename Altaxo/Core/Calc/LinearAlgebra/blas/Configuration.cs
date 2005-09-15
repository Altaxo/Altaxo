/*
 * Configuration.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

#if !MANAGED
using System;

namespace Altaxo.Calc.LinearAlgebra.Blas
{
	
  ///<summary>Contains configuration information for dnA library</summary>
  ///<remarks>
  /// This class contains members that are used to configure the operation
  /// of the dnA library.  The members define the default use of native
  /// or managed code, and how exceptions from unmanaged code are handled.
  /// </remarks>
  public sealed class Configuration 
  {
    private static int blockSize = 16;
		
    internal const string BLASLibrary = "dnA.Wrapper.dll";
		
    /// <summary>
    /// Defines the block size for blocked LAPACK algorithms. The value is 
    /// machine dependent (typically, 16 to 64).
    /// </summary>
    /// <remarks>Defaults to 16. The value must be greater than 0.</remarks>
    public static int BlockSize
    {
      get
      {
        return blockSize;
      }
      set
      {
        if(value <= 0)
        {
          throw new ArgumentException("Blocksize must be positive.");
        }
        blockSize = value;
      }
    }
		
    private Configuration() {}
  }
}
#endif