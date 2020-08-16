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
  public abstract class DashPatternBase : IDashPattern, Main.IImmutable
  {
    #region Serialization

    protected static void SerializeV0(IDashPattern obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var parent = DashPatternListManager.Instance.GetParentList(obj);
      if (null != parent)
      {
        if (null == info.GetProperty(DashPatternList.GetSerializationRegistrationKey(parent)))
          info.AddValue("Set", parent);
        else
          info.AddValue("SetName", parent.Name);
      }
    }

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

    public abstract double this[int index] { get; set; }

    public abstract int Count { get; }

    public virtual double DashOffset { get { return 0; } }

    public object Clone() // Attention: although IDashPattern is immutable, different instances of the same pattern are neccessary to establish a membership into the DashPatternList!
    {
      return MemberwiseClone();
    }

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

    public virtual bool Equals(IDashPattern? other)
    {
      if (other is null)
        return false;

      if (object.ReferenceEquals(this, other))
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

    public static bool operator ==(DashPatternBase x, DashPatternBase y)
    {
      return x is { } _ ? x.Equals(y) : y is { } _ ? y.Equals(x) : true;
    }
    public static bool operator !=(DashPatternBase x, DashPatternBase y)
    {
      return !(x == y);
    }

    public override int GetHashCode()
    {
      return this.GetType().GetHashCode() + 5 * Count.GetHashCode() + 7 * DashOffset.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
      return Equals(obj as DashPatternBase);
    }
  }
}
