#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Stores additional tick values for a scale.
  /// </summary>
  public class AdditionalTicks
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    Main.ICopyFrom
  {
    private List<AltaxoVariant> _additionalTicks;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AdditionalTicks), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AdditionalTicks)obj;

        info.CreateArray("ByValues", s._additionalTicks.Count);
        foreach (AltaxoVariant v in s._additionalTicks)
          info.AddValue("e", (object)v);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AdditionalTicks?)o ?? new AdditionalTicks();

        int count;

        count = info.OpenArray("ByValues");
        for (int i = 0; i < count; i++)
          s._additionalTicks.Add((AltaxoVariant)info.GetValue("e", s));
        info.CloseArray(count);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalTicks"/> class.
    /// </summary>
    public AdditionalTicks()
    {
      _additionalTicks = new List<AltaxoVariant>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalTicks"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public AdditionalTicks(AdditionalTicks from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies the additional tick values from another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    [MemberNotNull(nameof(_additionalTicks))]
    protected void CopyFrom(AdditionalTicks from)
    {
      _additionalTicks = new List<AltaxoVariant>(from._additionalTicks);
      EhSelfChanged();
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is AdditionalTicks from)
      {
        CopyFrom(from);

        return true;
      }

      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new AdditionalTicks(this);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      else if (!(obj is AdditionalTicks ticks))
        return false;
      else
      {
        var from = ticks;
        return _additionalTicks.SequenceEqual(from._additionalTicks);
      }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return _additionalTicks.GetHashCode();
    }

    /// <summary>
    /// Gets a value indicating whether no additional ticks are stored.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return _additionalTicks.Count == 0;
      }
    }

    /// <summary>
    /// Gets or sets an additional tick value by index.
    /// </summary>
    /// <param name="idx">The zero-based index of the additional tick.</param>
    public AltaxoVariant this[int idx]
    {
      get
      {
        return _additionalTicks[idx];
      }
      set
      {
        var oldValue = _additionalTicks[idx];
        _additionalTicks[idx] = value;
        if (value != oldValue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Clears all additional ticks.
    /// </summary>
    public void Clear()
    {
      var oldCount = _additionalTicks.Count;
      _additionalTicks.Clear();
      if (0 != oldCount)
        EhSelfChanged();
    }

    /// <summary>
    /// Adds an additional tick value.
    /// </summary>
    /// <param name="additionalTick">The additional tick value to add.</param>
    public void Add(AltaxoVariant additionalTick)
    {
      _additionalTicks.Add(additionalTick);
      EhSelfChanged();
    }

    /// <summary>
    /// Gets the stored additional tick values.
    /// </summary>
    public IEnumerable<AltaxoVariant> Values
    {
      get
      {
        return _additionalTicks;
      }
    }
  }
}
