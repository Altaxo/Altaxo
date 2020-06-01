#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Exception that is thrown during deserialization if unexpeced values are being deserialized.
  /// </summary>
  /// <seealso cref="System.Exception" />
  [System.Serializable]
  public class DeserializationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationException"/> class.
    /// </summary>
    public DeserializationException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DeserializationException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The inner exception.</param>
    public DeserializationException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
    protected DeserializationException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }

  /// <summary>
  /// Exception that is thrown when an object was expected to be deserialized, but instead null was deserialized. 
  /// </summary>
  /// <seealso cref="Altaxo.Serialization.Xml.DeserializationException" />
  [System.Serializable]
  public class DeserializationNullException : DeserializationException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationNullException"/> class.
    /// </summary>
    public DeserializationNullException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationNullException"/> class.
    /// </summary>
    /// <param name="name">The name of the property being deserialized.</param>
    /// <param name="parent">The parent object that holds the property.</param>
    public DeserializationNullException(string name, object? parent) : base($"Name: {name}, parent:{parent}") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationNullException"/> class.
    /// </summary>
    /// <param name="name">The name of the property being deserialized.</param>
    /// <param name="parent">The parent object that holds the property.</param>
    /// <param name="inner">The inner exception.</param>
    public DeserializationNullException(string name, object? parent, Exception inner) : base($"Name: {name}, parent:{parent}", inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializationNullException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
    protected DeserializationNullException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }

}
