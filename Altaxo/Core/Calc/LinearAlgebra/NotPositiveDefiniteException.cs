/*
 * NotPositiveDefiniteException.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Runtime.Serialization;

namespace Altaxo.Calc.LinearAlgebra {
    ///<summary>The exception is thrown when a none positive definite matrix is passed a method not expecting one.</summary>
    [Serializable()]
  public sealed class NotPositiveDefiniteException : MatrixException
  {

        ///<summary>Initializes a new instance of the <c>NotPositiveDefiniteException</c> class.</summary>
        public NotPositiveDefiniteException () : base () {}

        ///<summary>Initializes a new instance of the <c>NotPositiveDefiniteException</c> class with a specified error message.</summary>
        ///<param name="message">The error message that explains the reason for the exception.</param>
        public NotPositiveDefiniteException (String message) : base (message) {}

        ///<summary>Initializes a new instance of the <c>NotPositiveDefiniteException</c> class with a specified error message 
        ///and a reference to the inner exception that is the cause of this exception.</summary>
        ///<param name="message">The error message that explains the reason for the exception.</param>
        ///<param name="inner">The exception that is the cause of the current exception. 
        ///If the innerException parameter is not a null reference, the current exception is raised in a <c>catch</c> block 
        ///that handles the inner exception.</param>
        public NotPositiveDefiniteException (String message, System.Exception inner) : base(message,inner) {}  

        ///<summary>Initializes a new instance of the <c>NotPositiveDefiniteException</c> class with serialized data.</summary>
        ///<param name="info">The error message that explains the reason for the exception.</param>
        ///<param name="context">The error message that explains the reason for the exception.</param>
        private NotPositiveDefiniteException (SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}