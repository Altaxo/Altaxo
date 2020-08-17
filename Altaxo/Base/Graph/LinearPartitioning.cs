#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
  public class LinearPartitioning
    :
    Main.SuspendableDocumentLeafNodeWithSetOfEventArgs,
    IList<RADouble>,
    ICollection<RADouble>,
    IEnumerable<RADouble>,
    IList,
    ICollection,
    IEnumerable,
    INotifyCollectionChanged, INotifyPropertyChanged,
    ICloneable
  {
    private System.Collections.ObjectModel.ObservableCollection<RADouble> _innerList;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2013-09-25 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearPartitioning), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearPartitioning)obj;

        info.CreateArray("Partitioning", s.Count);
        foreach (var v in s)
          info.AddValue("e", v);
        info.CommitArray();
      }

      protected virtual LinearPartitioning SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinearPartitioning?)o ?? new LinearPartitioning();

        int count = info.OpenArray("Partitioning");
        for (int i = 0; i < count; ++i)
          s.Add((RADouble)info.GetValue("e", s));
        info.CloseArray(count);

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    public LinearPartitioning()
    {
      _innerList = new System.Collections.ObjectModel.ObservableCollection<RADouble>();
      _innerList.CollectionChanged += EhInnerList_CollectionChanged;
      ((INotifyPropertyChanged)_innerList).PropertyChanged += EhInnerList_PropertyChanged;
    }

    public LinearPartitioning(IEnumerable<RADouble> list)
    {
      _innerList = new System.Collections.ObjectModel.ObservableCollection<RADouble>(list);
      _innerList.CollectionChanged += EhInnerList_CollectionChanged;
      ((INotifyPropertyChanged)_innerList).PropertyChanged += EhInnerList_PropertyChanged;
    }

    public object Clone()
    {
      return new LinearPartitioning(_innerList);
    }

    #region event handling

    private void EhInnerList_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      EhSelfChanged(e);
    }

    private void EhInnerList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      EhSelfChanged(e);
    }

    protected override void OnChanged(EventArgs e)
    {
      var e1 = e as NotifyCollectionChangedEventArgs;
      if (null != e1)
      {
        var ev = CollectionChanged;
        if (null != ev)
          ev(this, e1);
      }

      var e2 = e as PropertyChangedEventArgs;
      if (null != e2)
      {
        var ev = PropertyChanged;
        if (null != ev)
          ev(this, e2);
      }

      base.OnChanged(e);
    }

    #endregion event handling

    /// <summary>
    /// Gets the absolute partition positions.
    /// </summary>
    /// <param name="totalSize">The total size of the partition.</param>
    /// <returns>Array containing the absolute partition positions.</returns>
    public double[] GetPartitionPositions(double totalSize)
    {
      double relSum = this.Sum(x => x.IsRelative ? x.Value : 0);
      double absSum = this.Sum(x => x.IsAbsolute ? x.Value : 0);

      double absValuePerRelativeValue = (totalSize - absSum) / relSum;

      int i = -1;
      double position = 0;
      double[] result = new double[Count];
      foreach (var x in this)
      {
        position += x.IsAbsolute ? x.Value : x.Value * absValuePerRelativeValue;
        result[++i] = position;
      }
      return result;
    }

    /// <summary>
    /// Normalizes the relative values in the partition, so that their sum is 1.
    /// </summary>
    public void NormalizeRelativeValues()
    {
      double relSum = this.Sum(x => x.IsRelative ? x.Value : 0);
      if (0 == relSum)
        return;

      double factor = 1 / relSum;

      if (!Altaxo.Calc.RMath.IsFinite(factor))
        return;

      for (int i = 0; i < _innerList.Count; ++i)
        if (_innerList[i].IsRelative)
          _innerList[i] = RADouble.NewRel(_innerList[i].Value * factor);
    }

    /// <summary>
    /// Adjusts the value at index <paramref name="idx"/> to match the absolute position given in <paramref name="absolutePosition"/>
    /// </summary>
    /// <param name="totalSize">The total size of the layer.</param>
    /// <param name="idx">The index of the value to modify.</param>
    /// <param name="absolutePosition">The new absolute position of the partition at the index <paramref name="idx"/>.</param>
    public void AdjustIndexToMatchPosition(double totalSize, int idx, double absolutePosition)
    {
      double relSum = this.Sum(x => x.IsRelative ? x.Value : 0);
      double absSum = this.Sum(x => x.IsAbsolute ? x.Value : 0);
      double absValuePerRelativeValue = (totalSize - absSum) / relSum;

      double position = 0;
      for (int i = 0; i < idx; ++i)
      {
        var x = this[i];
        position += x.IsAbsolute ? x.Value : x.Value * absValuePerRelativeValue;
      }

      double nextTwoCellsWidth =
                            (_innerList[idx].IsAbsolute ? _innerList[idx].Value : _innerList[idx].Value * absValuePerRelativeValue) +
                            (_innerList[idx + 1].IsAbsolute ? _innerList[idx + 1].Value : _innerList[idx + 1].Value * absValuePerRelativeValue);

      double absNew = Math.Max(0, absolutePosition - position); // should not be smaller than
      absNew = Math.Min(absNew, nextTwoCellsWidth);
      if (_innerList[idx].IsAbsolute)
      {
        double absOld = _innerList[idx].Value;
        double deltaAbs = absNew - absOld;

        if (this[idx + 1].IsRelative)
        {
          _innerList[idx] = RADouble.NewAbs(absNew);
          _innerList[idx + 1] = RADouble.NewRel(_innerList[idx + 1].Value - deltaAbs * relSum / (totalSize - absSum));
        }
        else // both idx and idx+1 are absolute
        {
          _innerList[idx] = RADouble.NewAbs(absNew);
          _innerList[idx + 1] = RADouble.NewAbs(_innerList[idx + 1].Value - deltaAbs);
        }
      }
      else
      {
        var relOld = _innerList[idx].Value;
        var absOld = relOld * absValuePerRelativeValue;

        if (this[idx + 1].IsRelative)
        {
          _innerList[idx] = new RADouble(absNew / absValuePerRelativeValue, true);
          _innerList[idx + 1] = new RADouble(this[idx + 1].Value + (relOld - this[idx].Value), true);
        }
        else // idx+1 is absolute
        {
          double deltaAbs = absNew - absOld;
          _innerList[idx] = RADouble.NewRel(_innerList[idx].Value + deltaAbs * relSum / (totalSize - absSum));
          _innerList[idx + 1] = RADouble.NewAbs(_innerList[idx + 1].Value - deltaAbs);
        }
      }
    }

    /// <summary>
    /// Gets the partition position. A relative value of 0 gives the absolute position 0, a value of 1 gives the size of the first partition, a value of two the size of the first plus second partition and so on.
    /// </summary>
    /// <param name="totalSize">The total size.</param>
    /// <param name="gridIndex">The grid index that designates a position in the partition.</param>
    /// <returns>The absolute position that belongs to the provided grid index.</returns>
    public double GetAbsolutePositionFromGridIndex(double totalSize, double gridIndex)
    {
      var partPositions = GetPartitionPositions(totalSize);

      if (gridIndex < 0)
      {
        return gridIndex * totalSize;
      }
      else if (gridIndex < partPositions.Length)
      {
        int rlower = (int)Math.Floor(gridIndex);
        double r = gridIndex - rlower;
        double pl = rlower == 0 ? 0 : partPositions[rlower - 1];
        double pu = partPositions[rlower];
        return pl * (1 - r) + r * pu;
      }
      else if (gridIndex >= partPositions.Length)
      {
        return (gridIndex - partPositions.Length) * totalSize + totalSize;
      }
      else
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// Gets the relative index from the absolute position.
    /// </summary>
    /// <param name="totalSize">The total size of the grid.</param>
    /// <param name="absolutePosition">The absolute position inside (or outside) the grid.</param>
    /// <returns>The relative index that corresponds to the provided absolute position.</returns>
    public double GetGridIndexFromAbsolutePosition(double totalSize, double absolutePosition)
    {
      var partPositions = GetPartitionPositions(totalSize);

      if (absolutePosition < 0) // on the left side outside the grid
      {
        return absolutePosition / totalSize;
      }

      double prevPosition = 0;
      for (int i = 0; i < partPositions.Length; ++i)
      {
        double nextPosition = partPositions[i];
        if (absolutePosition <= nextPosition)
        {
          var delta = nextPosition - prevPosition;
          if (0 == delta)
            return i;
          else
            return i + 1 + (absolutePosition - nextPosition) / delta;
        }
        prevPosition = nextPosition;
      }

      // else we are at the right side outside the grid
      return partPositions.Length + (absolutePosition - totalSize) / totalSize;
    }

    /// <summary>
    /// Gets the absolute start position and absolute size by providing a grid start index and grid span.
    /// </summary>
    /// <param name="totalSize">The total size of the partition.</param>
    /// <param name="gridIndex">The grid index that designates the tile position.</param>
    /// <param name="gridSpan">The grid span that designates the tile size.</param>
    /// <param name="absoluteTilePosition">Result: the absolute position of the tile.</param>
    /// <param name="absoluteTileSize">Result: the absolute size of the tile.</param>
    public void GetAbsolutePositionAndSizeFromGridIndexAndSpan(double totalSize, double gridIndex, double gridSpan, out double absoluteTilePosition, out double absoluteTileSize)
    {
      absoluteTilePosition = GetAbsolutePositionFromGridIndex(totalSize, gridIndex);
      absoluteTileSize = GetAbsolutePositionFromGridIndex(totalSize, gridIndex + gridSpan) - absoluteTilePosition;
    }

    /// <summary>
    /// Gets the sum of all relative values.
    /// </summary>
    /// <returns>Sum of all relative values.</returns>
    public double GetSumRelativeValues()
    {
      return this.Sum(x => x.IsRelative ? x.Value : 0);
    }

    #region ICollection<RADouble>

    public void Add(RADouble item)
    {
      _innerList.Add(item);
    }

    public void Clear()
    {
      _innerList.Clear();
    }

    public bool Contains(RADouble item)
    {
      return _innerList.Contains(item);
    }

    public void CopyTo(RADouble[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _innerList.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(RADouble item)
    {
      return _innerList.Remove(item);
    }

    public IEnumerator<RADouble> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion ICollection<RADouble>

    #region INotifyCollectionChanged

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    #endregion INotifyCollectionChanged

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion INotifyPropertyChanged

    #region IList<RADouble>

    public int IndexOf(RADouble item)
    {
      return _innerList.IndexOf(item);
    }

    public void Insert(int index, RADouble item)
    {
      _innerList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      RemoveAt(index);
    }

    public RADouble this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        _innerList[index] = value;
      }
    }

    #endregion IList<RADouble>

    #region IList

    public int Add(object? value)
    {
      return ((IList)_innerList).Add(value);
    }

    public bool Contains(object? value)
    {
      return ((IList)_innerList).Contains(value);
    }

    public int IndexOf(object? value)
    {
      return ((IList)_innerList).IndexOf(value);
    }

    public void Insert(int index, object? value)
    {
      ((IList)_innerList).Insert(index, value);
    }

    public bool IsFixedSize
    {
      get { return ((IList)_innerList).IsFixedSize; }
    }

    public void Remove(object? value)
    {
      ((IList)_innerList).Remove(value);
    }

    object? IList.this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        ((IList)_innerList)[index] = value;
      }
    }

    public void CopyTo(Array array, int index)
    {
      ((IList)_innerList).CopyTo(array, index);
    }

    public bool IsSynchronized
    {
      get { return ((IList)_innerList).IsSynchronized; }
    }

    public object SyncRoot
    {
      get { return ((IList)_innerList).SyncRoot; }
    }

    #endregion IList
  }
}
