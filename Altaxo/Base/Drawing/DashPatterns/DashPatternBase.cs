#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Drawing.DashPatternManagement;

namespace Altaxo.Drawing.DashPatterns
{
  /// <summary>
  /// Base class for immutable dash patterns.
  /// </summary>
  public abstract class DashPatternBase : IDashPattern, Main.IImmutable
  {
    #region Serialization

    /// <summary>
    /// Serializes the common dash-pattern metadata used by version 0 surrogates.
    /// </summary>
    /// <param name="obj">The dash pattern to serialize.</param>
    /// <param name="info">The serialization info.</param>
    protected static void SerializeV0(IDashPattern obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var parent = DashPatternListManager.Instance.GetParentList(obj);
      if (parent is not null)
      {
        if (info.GetProperty(DashPatternList.GetSerializationRegistrationKey(parent)) is null)
          info.AddValue("Set", parent);
        else
          info.AddValue("SetName", parent.Name);
      }
    }

    /// <summary>
    /// Deserializes common dash-pattern metadata used by version 0 surrogates.
    /// </summary>
    /// <typeparam name="TItem">The dash-pattern type.</typeparam>
    /// <param name="instanceTemplate">The template instance to resolve.</param>
    /// <param name="info">The deserialization info.</param>
    /// <param name="parent">The parent object.</param>
    /// <returns>The resolved dash-pattern instance.</returns>
    protected static TItem DeserializeV0<TItem>(TItem instanceTemplate, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent) where TItem : IDashPattern
    {
      if (info.CurrentElementName == "Set")
      {
        var originalSet = (DashPatternList)info.GetValue("Set", parent);
        DashPatternListManager.Instance.TryRegisterList(info, originalSet, Main.ItemDefinitionLevel.Project, out var registeredSet);
        return (TItem)DashPatternListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, originalSet.Name); // Note: here we use the name of the original set, not of the registered set. Because the original name is translated during registering into the registered name
      }
      else if (info.CurrentElementName == "SetName")
      {
        string setName = info.GetString("SetName");
        return (TItem)DashPatternListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, setName);
      }
      else // nothing of both, thus symbol belongs to nothing
      {
        return instanceTemplate;
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public abstract double this[int index] { get; set; }

    /// <inheritdoc/>
    public abstract int Count { get; }

    /// <inheritdoc/>
    public virtual double DashOffset { get { return 0; } }

    /// <inheritdoc/>
    public object Clone() // Attention: although IDashPattern is immutable, different instances of the same pattern are neccessary to establish a membership into the DashPatternList!
    {
      return MemberwiseClone();
    }

    /// <inheritdoc/>
    public IEnumerator<double> GetEnumerator()
    {
      for (int i = 0; i < Count; ++i)
        yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < Count; ++i)
        yield return this[i];
    }

    /// <inheritdoc/>
    public virtual bool Equals(IDashPattern? other)
    {
      if (other is null)
        return false;

      if (ReferenceEquals(this, other))
        return true;

      if (this.GetType() != other.GetType())
        return false;

      if (this.DashOffset != other.DashOffset)
        return false;

      if (this.Count != other.Count)
        return false;

      for (int i = 0; i < Count; ++i)
      {
        if (this[i] != other[i])
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Determines whether two dash patterns are equal.
    /// </summary>
    public static bool operator ==(DashPatternBase x, DashPatternBase y)
    {
      return x is { } _ ? x.Equals(y) : y is { } _ ? y.Equals(x) : true;
    }
    /// <summary>
    /// Determines whether two dash patterns are not equal.
    /// </summary>
    public static bool operator !=(DashPatternBase x, DashPatternBase y)
    {
      return !(x == y);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return this.GetType().GetHashCode() + 5 * Count.GetHashCode() + 7 * DashOffset.GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return Equals(obj as DashPatternBase);
    }
  }
}
