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
  /// Apply this attribute to a surrogate class to indicate the type (or the assembly/type name)
  /// and version for which the surrogate handles serialization/deserialization.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class XmlSerializationSurrogateForAttribute : Attribute
  {
    protected int _version;
    protected System.Type? _serializationType;
    protected string? _assemblyName;
    protected string? _typeName;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializationSurrogateForAttribute"/> class.
    /// The class this attribute is applied to provides a serialization surrogate for the specified <paramref name="serializationtype"/>, version <paramref name="version"/>.
    /// </summary>
    /// <param name="serializationtype">The type this class provides a surrogate for.</param>
    /// <param name="version">The version of the class for which this surrogate is intended.</param>
    public XmlSerializationSurrogateForAttribute(Type serializationtype, int version)
    {
      _version = version;
      _serializationType = serializationtype;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializationSurrogateForAttribute"/> class.
    /// Use this constructor when the target type is deprecated and the original type is no longer available. In that case the class this attribute is applied to
    /// is responsible for deserialization of the deprecated type identified by the assembly and type name.
    /// </summary>
    /// <param name="assembly">The short assembly name containing the deprecated type.</param>
    /// <param name="typename">The fully qualified type name (including namespace) of the deprecated type.</param>
    /// <param name="version">The version of the deprecated type for which this surrogate is intended.</param>
    public XmlSerializationSurrogateForAttribute(string assembly, string typename, int version)
    {
      _version = version;
      _assemblyName = assembly;
      _typeName = typename;
    }

    /// <summary>
    /// Gets the version of the class for which the surrogate is intended.
    /// </summary>
    public int Version
    {
      get { return _version; }
    }

    /// <summary>
    /// Gets the target type for which the class this attribute is applied to is the serialization surrogate.
    /// The value is <c>null</c> when the surrogate targets a deprecated type specified by assembly and type name instead.
    /// </summary>
    public System.Type? SerializationType
    {
      get { return _serializationType; }
    }

    /// <summary>
    /// Gets the assembly short name of the target class type.
    /// If the <see cref="SerializationType"/> is set, the assembly name is derived from that type; otherwise the explicitly provided assembly name is returned.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when neither <see cref="SerializationType"/> nor the assembly name were provided.</exception>
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
    /// Gets the name of the target type (the full name inclusive of namespaces).
    /// If the <see cref="SerializationType"/> is set, its full name is returned; otherwise the supplied type name is returned.
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
