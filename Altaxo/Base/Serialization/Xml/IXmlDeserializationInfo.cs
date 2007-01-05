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

namespace Altaxo.Serialization.Xml
{
  public interface IXmlDeserializationInfo
  {
    /// <summary>Returns the name of the current xml element.</summary>
    string CurrentElementName { get; }

    bool GetBoolean();
    bool GetBoolean(string name);

    int GetInt32();
    int GetInt32(string name);

    float GetSingle();
    float GetSingle(string name);

    double GetDouble();
    double GetDouble(string name);

    string GetString();
    string GetString(string name);

    DateTime GetDateTime(string name);

    TimeSpan GetTimeSpan(string name);

    System.IO.MemoryStream GetMemoryStream(string name);

    object GetEnum(string name, System.Type type); // see remarks on serialization

    string GetNodeContent(); // gets the inner text of the node directly

    int GetInt32Attribute(string name);
  
    int OpenArray(); // get Number of Array elements
    int OpenArray(string name);
    void CloseArray(int count);

    void GetArray(out float[] val);
    
    /// <summary>
    /// Deserializes an array of double values. The array is allocated automatically.
    /// </summary>
    /// <param name="name">Name of the array.</param>
    /// <param name="val">The resulting deserialized array.</param>
    void GetArray(string name, out double[] val);

    void GetArray(string name, out int[] val);

    /// <summary>
    /// Deserializes an array of double value. The xml node must be opened before with <see cref="OpenArray()" />
    /// </summary>
    /// <param name="val">The array, must be at least of length <c>count</c>.</param>
    /// <param name="count">The number of elements to deserialize. If this is less than the number of elements in the xml stream, the other elements are safely ignored.</param>
    void GetArray(double[] val, int count);
    void GetArray(DateTime[] val, int count);
    void GetArray(string name, out string[] val);
    void GetArray(string[] val, int count);


    void OpenElement();
    void CloseElement();
    
    /// <summary>Retrieves the name of the current node</summary>
    /// <returns>The name of the current node.</returns>
    string GetNodeName();

    object GetValue(object parent);
    object GetValue(string name, object parent);

    void GetBaseValueEmbedded(object instance, System.Type basetype, object parent);
    void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent);


    /// <summary>
    /// This event is called if the deserialization process of all objects is finished and
    /// the deserialized objects are sorted into the document. Then the application should
    /// call AllFinished, which fires this event. The purpose of this event is to 
    /// resolve the references in the deserialized objects. This resolving process can be successfully
    /// done only if the objects are put in the right places in the document, so that
    /// the document paths can be resolved to the right objects.
    /// </summary>
    event XmlDeserializationCallbackEventHandler DeserializationFinished;
  }
}
