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
  /// Stores the assembly name and type name of an object so that an instance of that type can later be recreated.
  /// </summary>
  public class AssemblyAndTypeSurrogate
  {
    /// <summary>
    /// The assembly name.
    /// </summary>
    private string _assemblyName;

    /// <summary>
    /// The type name.
    /// </summary>
    private string _typeName;

    #region Serialization

    #region Version 0

    /// <summary>
    /// Serialization surrogate for <see cref="AssemblyAndTypeSurrogate"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AssemblyAndTypeSurrogate), 0)]
    public class XmlSerializationSurrogate : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AssemblyAndTypeSurrogate)o;

        info.AddValue("AssemblyName", s._assemblyName);
        info.AddValue("TypeName", s._typeName);
      }

      /// <inheritdoc/>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAndTypeSurrogate"/> class for deserialization.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    /// <param name="version">The surrogate version to deserialize.</param>
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



    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAndTypeSurrogate"/> class from an existing object.
    /// </summary>
    /// <param name="o">The object whose assembly and type information should be stored.</param>
    public AssemblyAndTypeSurrogate(object o)
    {
      if (o is null)
        throw new ArgumentNullException("To determine the type, the argument must not be null");
      _assemblyName = o.GetType().Assembly.FullName ?? throw new InvalidOperationException($"Unable to determine full name of assembly of type {o.GetType()}");
      _typeName = o.GetType().FullName ?? throw new InvalidOperationException($"Unable to determine full name of type {o.GetType()}");
    }

    /// <summary>
    /// Creates an instance of the stored type.
    /// </summary>
    /// <returns>A newly created instance of the stored type, or <c>null</c> if instance creation fails.</returns>
    public object? CreateInstance()
    {
      try
      {
        var oh = System.Activator.CreateInstance(_assemblyName, _typeName);
        return oh?.Unwrap();
      }
      catch (Exception)
      {
      }
      return null;
    }
  }
}
