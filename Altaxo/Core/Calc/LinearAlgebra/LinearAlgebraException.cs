/*
 * dnAException.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.Serialization;

namespace Altaxo.Calc.LinearAlgebra {
    ///<summary>Represents errors that occur when using the dnA library.</summary>
    [Serializable()]
    public abstract class LinearAlgebraException : System.SystemException {
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
