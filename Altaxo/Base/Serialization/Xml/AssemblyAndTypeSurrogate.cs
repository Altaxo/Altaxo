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
  /// Summary description for AssemblyAndTypeSurrogate.
  /// </summary>
  /// 
  public class AssemblyAndTypeSurrogate
  {
    string _assemblyName;
    string _typeName;


    #region Serialization
 

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AssemblyAndTypeSurrogate),0)]
      public class XmlSerializationSurrogate : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AssemblyAndTypeSurrogate s = (AssemblyAndTypeSurrogate)obj;
       
        info.AddValue("AssemblyName",s._assemblyName);
        info.AddValue("TypeName",s._typeName);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AssemblyAndTypeSurrogate s = o==null ? new AssemblyAndTypeSurrogate() : (AssemblyAndTypeSurrogate)o;

        s._assemblyName = info.GetString("AssemblyName");
        s._typeName = info.GetString("TypeName");

        return s;
      }
        
    }

   
    #endregion

    protected AssemblyAndTypeSurrogate()
    {
    }

    public AssemblyAndTypeSurrogate(object o)
    {
      if(o==null)
        throw new ArgumentNullException("To determine the type, the argument must not be null");
      this._assemblyName = o.GetType().Assembly.FullName;
      this._typeName = o.GetType().FullName;
    }

    public object CreateInstance()
    {
      try
      {
        System.Runtime.Remoting.ObjectHandle oh =  System.Activator.CreateInstance(_assemblyName,_typeName);
        return oh.Unwrap();
      }
      catch(Exception)
      {
      }
      return null;
    }
  }
}
