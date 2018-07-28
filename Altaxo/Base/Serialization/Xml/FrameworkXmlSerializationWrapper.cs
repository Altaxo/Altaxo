#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// This element wraps an Altaxo element, which is serializable to an Xml document using the Altaxo built-in Xml serialization infrastructure,
  /// to an element which is serializable as Xml by the .NET framework Xml serialization infrastructure. Since the framework infrastructure does not support
  /// renaming of classes, this wrapper class must not be renamed or moved to another namespace.
  /// </summary>
  public class FrameworkXmlSerializationWrapper : System.Xml.Serialization.IXmlSerializable
  {
    private static Altaxo.Serialization.Xml.XmlStreamSerializationInfo _xmlWriting = new XmlStreamSerializationInfo();
    private static Altaxo.Serialization.Xml.XmlStreamDeserializationInfo _xmlReading = new XmlStreamDeserializationInfo();

    private object _wrappedObject;

    /// <summary>
    /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
    /// </returns>
    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    /// <summary>
    /// Generates an object from its XML representation.
    /// </summary>
    /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
    public void ReadXml(System.Xml.XmlReader reader)
    {
      reader.ReadStartElement();
      _xmlReading.BeginReading(reader);
      _wrappedObject = _xmlReading.GetValue("WrappedObject", null);
      _xmlReading.EndReading();
      reader.ReadEndElement();
    }

    /// <summary>
    /// Converts an object into its XML representation.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
      _xmlWriting.BeginWriting(writer);
      _xmlWriting.AddValue("WrappedObject", _wrappedObject);
      _xmlWriting.EndWriting();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkXmlSerializationWrapper" /> class. This empty constructur
    /// is intended for deserialization.
    /// </summary>
    public FrameworkXmlSerializationWrapper()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkXmlSerializationWrapper" /> class with the provided object to wrap.
    /// </summary>
    /// <param name="objectToWrap">The object to wrap.</param>
    public FrameworkXmlSerializationWrapper(object objectToWrap)
    {
      _wrappedObject = objectToWrap;
    }

    /// <summary>
    /// Gets the wrapped object.
    /// </summary>
    /// <value>
    /// The wrapped object.
    /// </value>
    public object WrappedObject
    {
      get
      {
        return _wrappedObject;
      }
    }

    /// <summary>
    /// Determines whether the specified object is serializable with the Altaxo xml serialization framework.
    /// </summary>
    /// <param name="o">The object to test</param>
    /// <returns>
    ///   <c>true</c> if the specified object is serializable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSerializable(object o)
    {
      return _xmlWriting.IsSerializable(o);
    }

    /// <summary>
    /// Determines whether instances of the provided type are serializable with the Altaxo Xml serialization framework.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if instances of the specified object are serializable with the Altaxo Xml serialization framework; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSerializableType(System.Type type)
    {
      return _xmlWriting.IsSerializableType(type);
    }
  }
}
