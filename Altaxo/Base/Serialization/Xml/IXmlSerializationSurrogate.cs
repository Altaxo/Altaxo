#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

using System;
using System.Xml;

namespace Altaxo.Serialization.Xml
{

  /// <summary>
  /// This interface has to be implemented by all XmlSerializationSurrogates.
  /// </summary>
  public interface IXmlSerializationSurrogate
  {
    /// <summary>
    /// Serialize the object <code>o</code> into xml.
    /// </summary>
    /// <param name="o">The object to serialize.</param>
    /// <param name="info">The serialization info used to serialize the object.</param>
    void Serialize(object o, IXmlSerializationInfo info);

    /// <summary>
    /// Deserialize the object from xml.
    /// </summary>
    /// <param name="o">Is null except when a base type is deserialized, in this case this is the object instance of a super class.</param>
    /// <param name="info">The serialization info used to deserialize the stream.</param>
    /// <param name="parentobject">The object which is serialized before in the object hierarchie.</param>
    /// <returns>The object which is deserialized.</returns>
    /// <remarks>All deserialization code should check if object o is null. In this case it has to create a instance of the class which is about to
    /// be deserialized. If it is not null, the deserialization code of a super class has already created a instance. In this case the code had to 
    /// use this instance! It is recommended to use always the following code (except abstract and sealed classes):
    /// <code>
    /// public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    /// {
    /// Foo s = null!=o ? (Foo)o : new Foo();
    /// // (Deserialization code follows here) ...
    /// }
    ///</code>
    ///</remarks>
    object Deserialize(object o, IXmlDeserializationInfo info, object parentobject);
    
  }
}
