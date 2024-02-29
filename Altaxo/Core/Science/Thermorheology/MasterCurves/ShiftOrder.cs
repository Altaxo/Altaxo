#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;

namespace Altaxo.Science.Thermorheology.MasterCurves.ShiftOrder
{
  /// <summary>
  /// Designates the order in which the curves are tried to fit.
  /// </summary>
  public interface IShiftOrder : Main.IImmutable
  {
    /// <summary>
    /// Gets the indices of the curves in the order in which they should be shifted.
    /// Attention: the first returned index is the index of the curve that is fixed!
    /// </summary>
    /// <param name="numberOfCurves">The number of items.</param>
    /// <returns>Enumeration of curve indices in the order in which the curves should be fitted.
    /// Attention: the first returned index is the index of the curve that is fixed!
    /// </returns>
    IEnumerable<int> GetShiftOrderIndices(int numberOfCurves);

    /// <summary>
    /// Gets a value indicating whether this instance requires a pivot index to be set.
    /// </summary>
    bool IsPivotIndexRequired { get; }

    /// <summary>
    /// Gets /sets the pivot index.
    /// </summary>
    int? PivotIndex { get; init; }

    /// <summary>
    /// Returns a new instance of the same class with the pivot index set.
    /// If no pivot index is required (check by <see cref="IsPivotIndexRequired"/>), the same instance is returned./>
    /// </summary>
    IShiftOrder WithPivotIndex(int? index);
  }

  /// <summary>Fit by fixing the 1st curve, then adding the 2nd curve, 3rd curve, up to the Nth curve.</summary>
  public record FirstToLast : IShiftOrder
  {
    /// <inheritdoc/>
    public IEnumerable<int> GetShiftOrderIndices(int numberOfCurves)
    {
      for (int i = 0; i < numberOfCurves; i++)
      {
        yield return i;
      }
    }

    /// <inheritdoc/>
    public bool IsPivotIndexRequired => false;

    /// <inheritdoc/>
    public int? PivotIndex { get => null; init => Noop(); }

    /// <inheritdoc/>
    public IShiftOrder WithPivotIndex(int? index) => this;

    static void Noop() { }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FirstToLast), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new FirstToLast();
      }
    }
    #endregion
  }

  /// <summary>Fit by fixing the Nth curve, then adding the N-1 curve, N-2 curve, up to the 0th curve.</summary>
  public record LastToFirst : IShiftOrder
  {
    /// <inheritdoc/>
    public IEnumerable<int> GetShiftOrderIndices(int numberOfCurves)
    {
      for (int i = numberOfCurves - 1; i >= 0; i--)
      {
        yield return i;
      }
    }

    /// <inheritdoc/>
    public bool IsPivotIndexRequired => false;

    /// <inheritdoc/>
    public int? PivotIndex { get => null; init => Noop(); }

    /// <inheritdoc/>
    public IShiftOrder WithPivotIndex(int? index) => this;

    static void Noop() { }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LastToFirst), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new LastToFirst();
      }
    }
    #endregion
  }

  /// <summary>Fit by fixing the Rth curve, then adding the R+1 curve, .. Nth curve. Then adding the R-1 curve, down to the 0th curve.</summary>
  public record PivotToLastThenToFirst : IShiftOrder
  {
    int? _pivotIndex;

    /// <inheritdoc/>
    public IEnumerable<int> GetShiftOrderIndices(int numberOfCurves)
    {
      if (_pivotIndex is null)
        throw new InvalidOperationException($"{nameof(PivotIndex)} must be set before calling {nameof(GetShiftOrderIndices)}");
      var refIndex = Math.Max(0, Math.Min(_pivotIndex.Value, numberOfCurves - 1));

      for (int i = refIndex; i < numberOfCurves; ++i)
        yield return i;
      for (int i = refIndex - 1; i >= 0; --i)
        yield return i;
    }

    /// <inheritdoc/>
    public bool IsPivotIndexRequired => true;

    /// <inheritdoc/>
    public int? PivotIndex { get => _pivotIndex; init => _pivotIndex = value is null || value.Value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(PivotIndex)); }

    /// <inheritdoc/>
    public IShiftOrder WithPivotIndex(int? index) => this with { PivotIndex = index };

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PivotToLastThenToFirst), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PivotToLastThenToFirst)obj;
        info.AddValue("PivotIndex", s._pivotIndex);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pivotIndex = info.GetNullableInt32("PivotIndex");
        return new PivotToLastThenToFirst() { _pivotIndex = pivotIndex };
      }
    }
    #endregion
  }

  /// <summary>Fit by fixing the Rth curve, then adding the R-1 curve, .. 0th curve. Then adding the R+1 curve, up to the Nth curve.</summary>
  public record PivotToFirstThenToLast : IShiftOrder
  {
    int? _pivotIndex;

    /// <inheritdoc/>
    public IEnumerable<int> GetShiftOrderIndices(int numberOfCurves)
    {
      if (_pivotIndex is null)
        throw new InvalidOperationException($"{nameof(PivotIndex)} must be set before calling {nameof(GetShiftOrderIndices)}");
      var refIndex = Math.Max(0, Math.Min(_pivotIndex.Value, numberOfCurves - 1));

      for (int i = refIndex; i >= 0; --i)
        yield return i;
      for (int i = refIndex + 1; i < numberOfCurves; ++i)
        yield return i;
    }

    /// <inheritdoc/>
    public bool IsPivotIndexRequired => true;

    /// <inheritdoc/>
    public int? PivotIndex { get => _pivotIndex; init => _pivotIndex = value is null || value.Value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(PivotIndex)); }

    /// <inheritdoc/>
    public IShiftOrder WithPivotIndex(int? index) => this with { PivotIndex = index };

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PivotToFirstThenToLast), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PivotToFirstThenToLast)obj;
        info.AddValue("PivotIndex", s._pivotIndex);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pivotIndex = info.GetNullableInt32("PivotIndex");
        return new PivotToFirstThenToLast() { _pivotIndex = pivotIndex };
      }
    }
    #endregion
  }

  /// <summary>Fit by fixing the Rth curve, then adding the R+1 curve, R-1 curve, up and down to the Nth and 0th curve.</summary>
  public record PivotToLastAlternating : IShiftOrder
  {
    int? _pivotIndex;

    /// <inheritdoc/>
    public IEnumerable<int> GetShiftOrderIndices(int numberOfCurves)
    {
      if (_pivotIndex is null)
        throw new InvalidOperationException($"{nameof(PivotIndex)} must be set before calling {nameof(GetShiftOrderIndices)}");
      var refIndex = Math.Max(0, Math.Min(_pivotIndex.Value, numberOfCurves - 1));

      yield return refIndex;
      for (int i = 1; (refIndex - i) >= 0 || (refIndex + i) < numberOfCurves; ++i)
      {
        if ((refIndex + i) < numberOfCurves)
          yield return (refIndex + i);
        if ((refIndex - i) >= 0)
          yield return (refIndex - i);
      }
    }

    /// <inheritdoc/>
    public bool IsPivotIndexRequired => true;

    /// <inheritdoc/>
    public int? PivotIndex { get => _pivotIndex; init => _pivotIndex = value is null || value.Value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(PivotIndex)); }

    /// <inheritdoc/>
    public IShiftOrder WithPivotIndex(int? index) => this with { PivotIndex = index };

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PivotToLastAlternating), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PivotToLastAlternating)obj;
        info.AddValue("PivotIndex", s._pivotIndex);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pivotIndex = info.GetNullableInt32("PivotIndex");
        return new PivotToLastAlternating() { _pivotIndex = pivotIndex };
      }
    }
    #endregion
  }

  /// <summary>Fit by fixing the Rth curve, then adding the R-1 curve, R+1 curve, down and up to the 0th and Nth curve.</summary>
  public record PivotToFirstAlternating : IShiftOrder
  {
    int? _pivotIndex;

    /// <inheritdoc/>
    public IEnumerable<int> GetShiftOrderIndices(int numberOfCurves)
    {
      if (_pivotIndex is null)
        throw new InvalidOperationException($"{nameof(PivotIndex)} must be set before calling {nameof(GetShiftOrderIndices)}");
      var refIndex = Math.Max(0, Math.Min(_pivotIndex.Value, numberOfCurves - 1));

      yield return refIndex;
      for (int i = 1; (refIndex - i) >= 0 || (refIndex + i) < numberOfCurves; ++i)
      {
        if ((refIndex - i) >= 0)
          yield return (refIndex - i);
        if ((refIndex + i) < numberOfCurves)
          yield return (refIndex + i);
      }
    }

    /// <inheritdoc/>
    public bool IsPivotIndexRequired => true;

    /// <inheritdoc/>
    public int? PivotIndex { get => _pivotIndex; init => _pivotIndex = value is null || value.Value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(PivotIndex)); }

    /// <inheritdoc/>
    public IShiftOrder WithPivotIndex(int? index) => this with { PivotIndex = index };

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PivotToFirstAlternating), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PivotToFirstAlternating)obj;
        info.AddValue("PivotIndex", s._pivotIndex);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pivotIndex = info.GetNullableInt32("PivotIndex");
        return new PivotToFirstAlternating() { _pivotIndex = pivotIndex };
      }
    }
    #endregion
  }
}

