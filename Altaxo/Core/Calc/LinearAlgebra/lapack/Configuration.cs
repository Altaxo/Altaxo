#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
 * Configuration.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

#if !MANAGED
using System;

namespace Altaxo.Calc.LinearAlgebra.Lapack
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