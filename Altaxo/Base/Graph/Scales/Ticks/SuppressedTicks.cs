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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Stores tick values or tick indices that should be suppressed.
  /// </summary>
  public class SuppressedTicks
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    Main.ICopyFrom
  {
    private ObservableCollection<AltaxoVariant> _suppressedTickValues;
    private ObservableCollection<int> _suppressedTicksByNumber;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SuppressedTicks), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SuppressedTicks)o;

        info.CreateArray("ByValues", s._suppressedTickValues.Count);
        foreach (AltaxoVariant v in s._suppressedTickValues)
          info.AddValue("e", (object)v);
        info.CommitArray();

        info.CreateArray("ByNumbers", s._suppressedTicksByNumber.Count);
        foreach (int v in s._suppressedTicksByNumber)
          info.AddValue("e", v);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s =  (SuppressedTicks?)o ?? new SuppressedTicks();

        int count;

        count = info.OpenArray("ByValues");
        for (int i = 0; i < count; i++)
          s._suppressedTickValues.Add((AltaxoVariant)info.GetValue("e", s));
        info.CloseArray(count);

        count = info.OpenArray("ByNumbers");
        for (int i = 0; i < count; i++)
          s._suppressedTicksByNumber.Add(info.GetInt32("e"));
        info.CloseArray(count);

        return s;
      }

      
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SuppressedTicks"/> class.
    /// </summary>
    public SuppressedTicks()
    {
      _suppressedTickValues = new ObservableCollection<AltaxoVariant>();
      _suppressedTicksByNumber = new ObservableCollection<int>();

      _suppressedTicksByNumber.CollectionChanged += EhCollectionChanged;
      _suppressedTickValues.CollectionChanged += EhCollectionChanged;
    }

    /// <summary>
    /// Handles collection changes of the suppressed-tick lists.
    /// </summary>
    private void EhCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      EhSelfChanged();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuppressedTicks"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public SuppressedTicks(SuppressedTicks from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies values from another <see cref="SuppressedTicks"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    [MemberNotNull(nameof(_suppressedTickValues), nameof(_suppressedTicksByNumber))]
    protected void CopyFrom(SuppressedTicks from)
    {
      _suppressedTickValues = new ObservableCollection<AltaxoVariant>(from._suppressedTickValues);
      _suppressedTicksByNumber = new ObservableCollection<int>(from._suppressedTicksByNumber);
      _suppressedTicksByNumber.CollectionChanged += EhCollectionChanged;
      _suppressedTickValues.CollectionChanged += EhCollectionChanged;
      EhSelfChanged();
    }

    /// <inheritdoc/>
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is SuppressedTicks from)
      {
        CopyFrom(from);
        return true;
      }

      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new SuppressedTicks(this);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      else if (!(obj is SuppressedTicks ticks))
        return false;
      else
      {
        var from = ticks;
        if (!_suppressedTicksByNumber.SequenceEqual(from._suppressedTicksByNumber))
          return false;

        if (!_suppressedTickValues.SequenceEqual(from._suppressedTickValues))
          return false;
      }
      return true;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode() + 17 * _suppressedTicksByNumber.Count + 31 * _suppressedTickValues.Count;
    }

    /// <summary>
    /// Gets a value indicating whether no suppressed ticks are stored.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return _suppressedTickValues.Count == 0 && _suppressedTicksByNumber.Count == 0;
      }
    }

    /// <summary>
    /// Gets the suppressed tick values.
    /// </summary>
    public IList<AltaxoVariant> ByValues
    {
      get
      {
        return _suppressedTickValues;
      }
    }

    /// <summary>
    /// Gets the suppressed tick numbers.
    /// </summary>
    public IList<int> ByNumbers
    {
      get
      {
        return _suppressedTicksByNumber;
      }
    }

    private class DescendingIntComparer : IComparer<int>
    {
      private Comparer<int> _comparer = Comparer<int>.Default;

      public int Compare(int x, int y)
      {
        return _comparer.Compare(y, x);
      }
    }

    /// <summary>Gets all suppressed tick numbers in descending order with negative numbers transformed to positive numbers.</summary>
    /// <param name="totalNumberOfTicks">The total number of ticks. This is used to transform negative numbers to real, positive, tick numbers.</param>
    /// <returns>A list of suppressed tick numbers (all positive) in descending order.</returns>
    public ICollection<int> GetValidTickNumbersDecendingWithNegativeNumbersTransformed(int totalNumberOfTicks)
    {
      var result = new SortedSet<int>(new DescendingIntComparer());
      foreach (int idx in _suppressedTicksByNumber)
      {
        if (idx >= 0 && idx < totalNumberOfTicks)
          result.Add(idx);
        else if (idx < 0 && totalNumberOfTicks + idx >= 0)
          result.Add(totalNumberOfTicks + idx);
      }
      return result;
    }

    /// <summary>Removes the suppressed ticks from the list given as argument.</summary>
    /// <param name="ticks">The tick list. At return, the suppressed ticks are removed from that list.</param>
    public void RemoveSuppressedTicks(IList<double> ticks)
    {
      // Remove suppressed ticks
      if (_suppressedTicksByNumber.Count > 0)
      {
        var suppressedTicksDescending = GetValidTickNumbersDecendingWithNegativeNumbersTransformed(ticks.Count);
        foreach (var i in suppressedTicksDescending)
        {
          ticks.RemoveAt(i);
        }
      }

      if (_suppressedTickValues.Count > 0)
      {
        for (int i = ticks.Count - 1; i >= 0; --i)
        {
          if (_suppressedTickValues.Contains(ticks[i]))
            ticks.RemoveAt(i);
        }
      }
    }

    /// <summary>Removes the suppressed ticks from the list given as argument.</summary>
    /// <param name="ticks">The tick list. At return, the suppressed ticks are removed from that list.</param>
    public void RemoveSuppressedTicks(IList<AltaxoVariant> ticks)
    {
      // Remove suppressed ticks
      if (_suppressedTicksByNumber.Count > 0)
      {
        var suppressedTicksDescending = GetValidTickNumbersDecendingWithNegativeNumbersTransformed(ticks.Count);
        foreach (var i in suppressedTicksDescending)
        {
          ticks.RemoveAt(i);
        }
      }

      if (_suppressedTickValues.Count > 0)
      {
        for (int i = ticks.Count - 1; i >= 0; --i)
        {
          if (_suppressedTickValues.Contains(ticks[i]))
            ticks.RemoveAt(i);
        }
      }
    }
  }
}
