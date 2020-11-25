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
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Summary description for AssemblyAndTypeSurrogate.
  /// </summary>
  ///
  public class AssemblyAndTypeSurrogate
  {
    private string _assemblyName;
    private string _typeName;

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AssemblyAndTypeSurrogate), 0)]
    public class XmlSerializationSurrogate : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AssemblyAndTypeSurrogate)obj;

        info.AddValue("AssemblyName", s._assemblyName);
        info.AddValue("TypeName", s._typeName);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is AssemblyAndTypeSurrogate s)
          s.DeserializeSurrogate0(info);
        else
          s = new AssemblyAndTypeSurrogate(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_assemblyName), nameof(_typeName))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _assemblyName = info.GetString("AssemblyName");
      _typeName = info.GetString("TypeName");
    }

    #endregion Version 0

    protected AssemblyAndTypeSurrogate(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization



    public AssemblyAndTypeSurrogate(object o)
    {
      if (o is null)
        throw new ArgumentNullException("To determine the type, the argument must not be null");
      _assemblyName = o.GetType().Assembly.FullName ?? throw new InvalidOperationException($"Unable to determine full name of assembly of type {o.GetType()}");
      _typeName = o.GetType().FullName ?? throw new InvalidOperationException($"Unable to determine full name of type {o.GetType()}");
    }

    public object? CreateInstance()
    {
      try
      {
#if NETFRAMEWORK
        System.Runtime.Remoting.ObjectHandle oh = System.Activator.CreateInstance(_assemblyName, _typeName);
        return oh.Unwrap();
#else
        throw new NotImplementedException("Need to find Core function for line below");
#endif
      }
      catch (Exception)
      {
      }
      return null;
    }
  }
}
