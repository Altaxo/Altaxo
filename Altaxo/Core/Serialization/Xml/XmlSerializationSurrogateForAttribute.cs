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

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Used to point to the target type for which this class provides a serialization surrogate.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class XmlSerializationSurrogateForAttribute : Attribute
  {
    protected int _version;
    protected System.Type? _serializationType;
    protected string? _assemblyName;
    protected string? _typeName;

    /// <summary>
    /// Constructor. The class this attribute is applied provides a serialization surrogate for the type <code>serializationtype</code>, version <code>version.</code>.
    /// </summary>
    /// <param name="serializationtype">The type this class provides a surrogate for.</param>
    /// <param name="version">The version of the class for which this surrogate is intended.</param>
    public XmlSerializationSurrogateForAttribute(Type serializationtype, int version)
    {
      _version = version;
      _serializationType = serializationtype;
    }

    /// <summary>
    /// Constructor. Used when the target type is deprecated and no longer available. The class this attribute is applied for is then
    /// responsible for deserialization
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typename"></param>
    /// <param name="version"></param>
    public XmlSerializationSurrogateForAttribute(string assembly, string typename, int version)
    {
      _version = version;
      _assemblyName = assembly;
      _typeName = typename;
    }

    /// <summary>
    /// returns the version of the class, for which the surrogate is intended
    /// </summary>
    public int Version
    {
      get { return _version; }
    }

    /// <summary>
    ///Returns the target type for which the class this attribute is applied for is the serialization surrogate.
    /// </summary>
    public System.Type? SerializationType
    {
      get { return _serializationType; }
    }

    /// <summary>
    /// Returns the assembly name (short form) of the target class type.
    /// </summary>
    public string AssemblyName
    {
      get
      {
        if (_serializationType is not null)
        {
          if (_serializationType.Assembly.FullName is { } fullName)
            return (_serializationType.Assembly.FullName.Split(new char[] { ',' }, 2))[0];
          else
            throw new InvalidOperationException($"No FullName available for assembly {_serializationType.Assembly}");
        }
        else if (_assemblyName is { } _)
        {
          return _assemblyName;
        }
        else
        {
          throw new InvalidOperationException("Either type or AssemblyName should be != null");
        }
      }
    }

    /// <summary>
    /// Returns the name of the target type (the full name inclusive namespaces).
    /// </summary>
    public string TypeName
    {
      get
      {
        return _serializationType?.ToString() ?? _typeName ?? throw new InvalidOperationException();
      }
    }
  } // end class SerializationSurrogateForAttribute
}
