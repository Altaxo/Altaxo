// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Runtime.Serialization;
using Altaxo;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Is thrown when the ServiceManager cannot find a required service.
  /// </summary>
  [Serializable()]
  public class ServiceNotFoundException : BaseException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    public ServiceNotFoundException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="serviceType">The required service type that could not be found.</param>
    public ServiceNotFoundException(Type serviceType) : base("Required service not found: " + serviceType.FullName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ServiceNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class from serialized data.
    /// </summary>
    /// <param name="info">The serialization information.</param>
    /// <param name="context">The streaming context.</param>
    protected ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
