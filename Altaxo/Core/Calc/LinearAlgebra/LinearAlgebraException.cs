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
 * dnAException.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.Serialization;

namespace Altaxo.Calc.LinearAlgebra 
{
  ///<summary>Represents errors that occur when using the dnA library.</summary>
  [Serializable()]
  public abstract class LinearAlgebraException : System.SystemException 
  {
    ///<summary>Initializes a new instance of the <c>LinearAlgebraException</c> class.</summary>
    protected LinearAlgebraException () : base () {}

    ///<summary>Initializes a new instance of the <c>LinearAlgebraException</c> class with a specified error message.</summary>
    ///<param name="message">The error message that explains the reason for the exception.</param>
    protected LinearAlgebraException (String message) : base (message) {}

    ///<summary>Initializes a new instance of the <c>LinearAlgebraException</c> class with a specified error message 
    ///and a reference to the inner exception that is the cause of this exception.</summary>
    ///<param name="message">The error message that explains the reason for the exception.</param>
    ///<param name="inner">The exception that is the cause of the current exception. 
    ///If the innerException parameter is not a null reference, the current exception is raised in a <c>catch</c> block 
    ///that handles the inner exception.</param>
    protected LinearAlgebraException (String message, System.Exception inner) : base(message,inner) {}

    ///<summary>Initializes a new instance of the <c>LinearAlgebraException</c> class with serialized data.</summary>
    ///<param name="info">The error message that explains the reason for the exception.</param>
    ///<param name="context">The error message that explains the reason for the exception.</param>
    protected LinearAlgebraException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
