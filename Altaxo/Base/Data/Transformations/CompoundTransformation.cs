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
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Data.Transformations
{
  public class CompoundTransformation : IDoubleToDoubleTransformation
  {
    /// <summary>
    /// The transformations. The innermost (i.e. first transformation to carry out, the rightmost transformation) is located at index 0.
    /// </summary>
    private List<IVariantToVariantTransformation> _transformations = new List<IVariantToVariantTransformation>();

    #region Serialization

    /// <summary>
    /// 2016-06-25 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CompoundTransformation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CompoundTransformation)obj;
        info.CreateArray("Transformations", s._transformations.Count);
        foreach (var t in s._transformations)
          info.AddValue("e", t);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int count = info.OpenArray("Transformations");
        var arr = new List<IVariantToVariantTransformation>(count);
        for (int i = 0; i < count; ++i)
        {
          arr.Add((IVariantToVariantTransformation)info.GetValue("e", null));
        }
        info.CloseArray(count);
        return new CompoundTransformation() { _transformations = arr };
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public Type InputValueType { get { return _transformations[0].InputValueType; } }

    /// <inheritdoc/>
    public Type OutputValueType { get { return _transformations[_transformations.Count - 1].OutputValueType; } }

    private CompoundTransformation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompoundTransformation"/> class.
    /// </summary>
    /// <param name="transformations">The transformations.</param>
    /// <exception cref="System.ArgumentException">
    /// Enumeration contains no items
    /// or
    /// Enumeration contains only one item. Please use this item directly.
    /// </exception>
    public CompoundTransformation(IEnumerable<IVariantToVariantTransformation> transformations)
    {
      _transformations = new List<IVariantToVariantTransformation>(transformations);

      if (_transformations.Count == 0)
        throw new ArgumentException("Enumeration contains no items", nameof(transformations));
      if (_transformations.Count == 1)
        throw new ArgumentException("Enumeration contains only one item. Please use this item directly.", nameof(transformations));
    }

    /// <summary>
    /// Try to get a compound transformation. Use this function when in doubt how many transformations the enumeration yields.
    /// The behavior is as follows: if the enumeration is null or empty, the return value is null. If the enumeration contains only one
    /// element, the return value is that element. If the enumeration contains multiple elements, the return value is a compound transformation
    /// with all elements.
    /// </summary>
    /// <param name="transformations">Enumeration of transformations.</param>
    /// <returns>If the enumeration is null or empty, the return value is null. If the enumeration contains only one
    /// element, the return value is that element. If the enumeration contains multiple elements, the return value is a compound transformation
    /// with all elements.</returns>
    public static IVariantToVariantTransformation? TryGetCompoundTransformation(IEnumerable<IVariantToVariantTransformation> transformations)
    {
      if (transformations is null)
        return null;

      var transformationList = new List<IVariantToVariantTransformation>(transformations);

      if (transformationList.Count == 0)
        return null;
      else if (transformationList.Count == 1)
        return transformationList[0];
      else
        return new CompoundTransformation(transformationList);
    }

    public static IVariantToVariantTransformation? TryGetCompoundTransformationWithSimplification(IEnumerable<(IVariantToVariantTransformation Transformation, bool UseBackTransformation)> transformations)
    {
      if (transformations is null)
        return null;

      var transformationList = new List<IVariantToVariantTransformation>();
      foreach (var item in transformations)
      {
        AddTransformationToFlattenedList(item.Transformation, item.UseBackTransformation, transformationList);
      }

      if (transformationList.Count == 0)
      {
        return null;
      }
      else if (transformationList.Count == 1)
      {
        return transformationList[0];
      }
      else
      {
        SimplifyTransformationList(transformationList);

        if (transformationList.Count == 0)
        {
          return null;
        }
        else if (transformationList.Count == 1)
        {
          return transformationList[0];
        }
        else
        {
          return new CompoundTransformation(transformationList);
        }
      }
    }

    public static IVariantToVariantTransformation? TryGetCompoundTransformationWithSimplification(IEnumerable<IVariantToVariantTransformation> transformations)
    {
      if (transformations is null)
        return null;

      var transformationList = new List<IVariantToVariantTransformation>();
      foreach (var transfo in transformations)
        AddTransformationToFlattenedList(transfo, false, transformationList);

      if (transformationList.Count == 0)
      {
        return null;
      }
      else if (transformationList.Count == 1)
      {
        return transformationList[0];
      }
      else
      {
        SimplifyTransformationList(transformationList);

        if (transformationList.Count == 0)
          return null;
        else
          return new CompoundTransformation(transformationList);
      }
    }

    /// <summary>
    /// Adds a transformation to a flattened list. If the provided transformation is a <see cref="CompoundTransformation"/>, the transformation is unpacked before added to the list.
    /// </summary>
    /// <param name="transformation">The transformation.</param>
    /// <param name="list">The list.</param>
    private static void AddTransformationToFlattenedList(IVariantToVariantTransformation transformation, bool useBackTransformation, List<IVariantToVariantTransformation> list)
    {
      if (transformation is CompoundTransformation ct)
      {
        if (useBackTransformation)
        {
          for(int i=ct._transformations.Count-1;i>=0;--i)
          {
            AddTransformationToFlattenedList(ct._transformations[i], useBackTransformation, list);
          }
        }
        else
        {
          foreach (var trans in ct._transformations)
          {
            AddTransformationToFlattenedList(trans, useBackTransformation, list);
          }
        }
      }
      else if (transformation is not null)
      {
        if (useBackTransformation)
        {
          list.Add(transformation.BackTransformation ?? throw new InvalidOperationException($"Backtransformation of transformation {transformation} is not available"));
        }
        else
        {
          list.Add(transformation);
        }
      }
    }

    /// <summary>
    /// Simplifies the transformation list by cancelling transformation / backtransformation pairs.
    /// </summary>
    /// <param name="list">The list to simplify.</param>
    private static void SimplifyTransformationList(List<IVariantToVariantTransformation> list)
    {
      bool hasChanged;

      // Cancel factors and offsets
      do
      {
        hasChanged = false;
        for (int i = list.Count - 2; i >= 0; --i)
        {
          if (list[i].BackTransformation.Equals(list[i + 1]))
          {
            hasChanged = true;
            list.RemoveAt(i + 1);
            list.RemoveAt(i);
            --i;
          }
          else if (list[i+1] is ScaleTransformation sc1 && list[i] is ScaleTransformation sc2)
          {
            hasChanged = true;
            list.RemoveAt(i + 1);
            var newScale = sc1.Scale + sc2.Scale;
            if (newScale != 1)
            {
              list[i] = new ScaleTransformation(newScale);
            }
            else
            {
              list.RemoveAt(i);
              --i;
            }
          }
          else if (list[i + 1] is OffsetTransformation of1 && list[i] is OffsetTransformation of2)
          {
            hasChanged = true;
            list.RemoveAt(i + 1);
            var newOffset = of1.Offset + of2.Offset;
            if (newOffset != 0)
            {
              list[i] = new OffsetTransformation(newOffset);
            }
            else
            {
              list.RemoveAt(i);
              --i;
            }
          }
        }
      } while (hasChanged);
    }

    public AltaxoVariant Transform(AltaxoVariant value)
    {
      foreach (var item in _transformations)
        value = item.Transform(value);
      return value;
    }

    public double Transform(double value)
    {
      foreach (var item in _transformations)
        value = item.Transform(value);
      return value;
    }

    /// <inheritdoc/>
    public (double ytrans, double dydxtrans) Derivative(double y, double dydx)
    {
      foreach (var item in _transformations)
      {
        if (item is IDoubleToDoubleTransformation ddt)
        {
          (y, dydx) = ddt.Derivative(y, dydx);
        }
        else
        {
          throw new InvalidOperationException($"Can not calculate derivative of compound transformation because the member {item} does not implement {nameof(IDoubleToDoubleTransformation)}");
        }
      }
      return (y, dydx);
    }

    public string RepresentationAsFunction
    {
      get { return GetRepresentationAsFunction("x"); }
    }

    public string GetRepresentationAsFunction(string arg)
    {
      var x = arg;
      foreach (var item in _transformations)
        x = item.GetRepresentationAsFunction(x);
      return x;
    }

    public string RepresentationAsOperator
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        for (int i = _transformations.Count - 1; i >= 0; --i)
        {
          stb.Append(_transformations[i].RepresentationAsOperator);
          if (i != 0)
            stb.Append(" ");
        }
        return stb.ToString();
      }
    }

    public IVariantToVariantTransformation BackTransformation
    {
      get
      {
        return new CompoundTransformation(GetTransformationsInReverseOrder().Select(transfo => transfo.BackTransformation));
      }
    }

    public CompoundTransformation WithPrependedTransformation(IVariantToVariantTransformation transformation)
    {
      if (transformation is null)
        throw new ArgumentNullException(nameof(transformation));

      var result = new CompoundTransformation
      {
        _transformations = new List<IVariantToVariantTransformation>(_transformations)
      };
      if (transformation is CompoundTransformation)
      {
        result._transformations.AddRange(((CompoundTransformation)transformation)._transformations);
      }
      else
      {
        result._transformations.Add(transformation);
      }
      return result;
    }

    public CompoundTransformation WithAppendedTransformation(IVariantToVariantTransformation transformation)
    {
      if (transformation is null)
        throw new ArgumentNullException(nameof(transformation));

      var result = new CompoundTransformation
      {
        _transformations = new List<IVariantToVariantTransformation>()
      };
      if (transformation is CompoundTransformation)
      {
        result._transformations.AddRange(((CompoundTransformation)transformation)._transformations);
      }
      else
      {
        result._transformations.Add(transformation);
      }

      result._transformations.AddRange(_transformations);
      return result;
    }

    private IEnumerable<IVariantToVariantTransformation> GetTransformationsInReverseOrder()
    {
      for (int i = _transformations.Count - 1; i >= 0; --i)
        yield return _transformations[i];
    }

    public override bool Equals(object? obj)
    {
      if (!(obj is CompoundTransformation from))
        return false;

      if (_transformations.Count != from._transformations.Count)
        return false;

      for (int i = 0; i < _transformations.Count; ++i)
        if (!_transformations[i].Equals(from._transformations[i]))
          return false;

      return true;
    }

    public override int GetHashCode()
    {
      int len = Math.Min(3, _transformations.Count);
      int result = GetType().GetHashCode();
      for (int i = 0; i < len; ++i)
        result += _transformations[i].GetHashCode();

      return result;
    }

    public bool IsEditable { get { return true; } }
  }
}
