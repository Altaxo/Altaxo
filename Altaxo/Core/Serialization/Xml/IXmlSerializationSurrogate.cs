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

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Contract for XML serialization surrogates that serialize and deserialize specific types.
  /// Implementations should emit/read XML for the target type and handle base-type chaining when needed.
  /// </summary>
  public interface IXmlSerializationSurrogate
  {
    /// <summary>
    /// Serialize the provided object into XML using the supplied serialization info.
    /// </summary>
    /// <param name="o">The object to serialize.</param>
    /// <param name="info">The serialization info used to write values and structure to the XML output.</param>
    void Serialize(object o, IXmlSerializationInfo info);

    /// <summary>
    /// Deserialize an object from XML.
    /// </summary>
    /// <param name="o">This is <c>null</c> except when a base type is deserialized; in that case it is the instance of the super class to populate.</param>
    /// <param name="info">The deserialization info used to read values from the XML input.</param>
    /// <param name="parentobject">The parent object in the object hierarchy which was deserialized before the current object; may be <c>null</c> for top-level objects.</param>
    /// <returns>The deserialized object instance.</returns>
    /// <remarks>All deserialization code should check if object <c>o</c> is null. In this case it has to create an instance of the class which is about to
    /// be deserialized. If it is not null, the deserialization code of a super class has already created an instance. In this case the code must
    /// use that instance. It is recommended to use the following pattern (except for abstract and sealed classes):
    /// <code>
    /// public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    /// {
    ///   var s = o as Foo ?? new Foo();
    ///   // (Deserialization code follows here) ...
    /// }
    /// </code>
    /// </remarks>
    object? Deserialize(object? o, IXmlDeserializationInfo info, object? parentobject);
  }
}
