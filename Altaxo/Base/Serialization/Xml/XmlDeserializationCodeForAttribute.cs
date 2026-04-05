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
  /// Identifies a deserialization helper method for a target type and version.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public class XmlDeserializationCodeForAttribute : Attribute
  {
    /// <summary>
    /// The version of the target type.
    /// </summary>
    protected int _version;

    /// <summary>
    /// The target serialization type, if available.
    /// </summary>
    protected System.Type? _serializationType;

    /// <summary>
    /// The short assembly name of the target type.
    /// </summary>
    protected string? _assemblyName;

    /// <summary>
    /// The full name of the target type.
    /// </summary>
    protected string? _typeName;

    /// <summary>
    /// Constructor. The class this attribute is applied provides a serialization surrogate for the type <code>serializationtype</code>, version <code>version.</code>.
    /// </summary>
    /// <param name="serializationtype">The type this class provides a surrogate for.</param>
    /// <param name="version">The version of the class for which this surrogate is intended.</param>
    public XmlDeserializationCodeForAttribute(Type serializationtype, int version)
    {
      _version = version;
      _serializationType = serializationtype;
    }

    /// <summary>
    /// Constructor. Used when the target type is deprecated and no longer available. The attributed method is then
    /// responsible for deserialization.
    /// </summary>
    /// <param name="assembly">The short assembly name.</param>
    /// <param name="typename">The full type name.</param>
    /// <param name="version">The supported serialization version.</param>
    public XmlDeserializationCodeForAttribute(string assembly, string typename, int version)
    {
      _version = version;
      _assemblyName = assembly;
      _typeName = typename;
    }

    /// <summary>
    /// Gets the version of the class for which the deserialization code is intended.
    /// </summary>
    public int Version
    {
      get { return _version; }
    }

    /// <summary>
    /// Gets the target type for which the attributed method provides deserialization code.
    /// </summary>
    public System.Type? SerializationType
    {
      get { return _serializationType; }
    }

    /// <summary>
    /// Gets the short assembly name of the target type.
    /// </summary>
    public string? AssemblyName
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
        else
        {
          return _assemblyName;
        }
      }
    }

    /// <summary>
    /// Gets the fully qualified name of the target type.
    /// </summary>
    public string TypeName
    {
      get
      {
        return _serializationType?.ToString() ?? _typeName ?? throw new InvalidOperationException();
      }
    }
  }
}
