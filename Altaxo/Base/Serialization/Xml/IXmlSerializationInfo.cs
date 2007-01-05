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
  /// <summary>
  /// Summary description for IXmlSerializationInfo.
  /// </summary>
  public interface IXmlSerializationInfo
  {
    void SetProperty(string propertyname, string propertyvalue);
    string GetProperty(string propertyname);

    void AddAttributeValue(string name, int val);
    
    void AddValue(string name, bool val);

    void AddValue(string name, int val);

    void AddValue(string name, float val);

    void AddValue(string name, double val);

    void AddValue(string name, string val);

    void AddValue(string name, DateTime val);

    void AddValue(string name, TimeSpan val);

    void AddValue(string name, System.IO.MemoryStream val);

    void AddEnum(string name, System.Enum val); // special name since otherwise _all_ enums would be serialized by that
 
    void SetNodeContent(string nodeContent); // sets Node content directly
  
    void CreateArray(string name, int count);

    void CommitArray();

    void AddArray(string name, int[] val, int count);
    void AddArray(string name, float[] val, int count);
    void AddArray(string name, double[] val, int count);
    void AddArray(string name, DateTime[] val, int count);
    void AddArray(string name, string[] val, int count);
    void AddArray(string name, object[] val, int count);
  
    void CreateElement(string name);
    void CommitElement();

    void AddValue(string name, object o);

    bool IsSerializable(object o);

    void AddBaseValueEmbedded(object o, System.Type basetype);
    void AddBaseValueStandalone(string name, object o, System.Type basetype);


    XmlArrayEncoding DefaultArrayEncoding   { get; set;   }
  }
}
