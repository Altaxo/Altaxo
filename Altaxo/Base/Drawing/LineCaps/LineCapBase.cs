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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Altaxo.Drawing;

namespace Altaxo.Drawing.LineCaps
{

  [System.ComponentModel.ImmutableObject(true)]
  public abstract class LineCapBase : ILineCap
  {
    private double _minimumAbsoluteSizePt;
    private double _minimumRelativeSize;

    protected LineCapBase()
    {
      _minimumAbsoluteSizePt = DefaultMinimumAbsoluteSizePt;
      _minimumRelativeSize = DefaultMinimumRelativeSize;
    }

    protected LineCapBase(double minimumAbsoluteSizePt, double minimumRelativeSize)
    {
      _minimumAbsoluteSizePt = minimumAbsoluteSizePt;
      _minimumRelativeSize = minimumRelativeSize;
    }

    public virtual double MinimumAbsoluteSizePt
    {
      get { return _minimumAbsoluteSizePt; }
    }

    public virtual double MinimumRelativeSize
    {
      get { return _minimumRelativeSize; }
    }

    public abstract string Name { get; }

    /// <summary>Gets the default minimum absolute size in points (1/72 inch).</summary>
    public abstract double DefaultMinimumAbsoluteSizePt { get; }

    /// <summary>Gets the default minimum relative size of the cap.</summary>
    /// <value>The value is multiplied with the pen with to get the minimum absolute size of the cap.</value>
    public abstract double DefaultMinimumRelativeSize { get; }

    /// <summary>
    /// Gets a new instance of the line cap with the designated minimum absolute and relative sizes. Note that not all line cap types support one or both values; in this case, those values are ignored.
    /// </summary>
    /// <param name="minimumAbsoluteSizePt">The minimum absolute size pt.</param>
    /// <param name="minimumRelativeSize">Minimum size of the relative.</param>
    /// <returns>A new instance of the line cap with the designated minimum absolute and relative sizes.</returns>
    public ILineCap WithMinimumAbsoluteAndRelativeSize(double minimumAbsoluteSizePt, double minimumRelativeSize)
    {
      if (!(_minimumAbsoluteSizePt == minimumAbsoluteSizePt) || !(_minimumRelativeSize == minimumRelativeSize))
      {
        var result = (LineCapBase)MemberwiseClone();
        result._minimumAbsoluteSizePt = minimumAbsoluteSizePt;
        result._minimumRelativeSize = minimumRelativeSize;
        result.CoerceSizeValues();
        return result;
      }
      else
      {
        return this;
      }
    }

    protected virtual void CoerceSizeValues()
    {

    }


    public bool Equals(ILineCap other)
    {
      if (other is null)
        return false;
      if (object.ReferenceEquals(this, other))
        return true;

      return
        (MinimumAbsoluteSizePt == other.MinimumAbsoluteSizePt) &&
        (MinimumRelativeSize == other.MinimumRelativeSize) &&
        (Name == other.Name) &&
        (GetType() == other.GetType());


    }

    public override bool Equals(object? obj)
    {
      return obj is ILineCap lc && Equals(lc);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return Name.GetHashCode() + 5 * _minimumAbsoluteSizePt.GetHashCode() + 7 * _minimumRelativeSize.GetHashCode();
      }
    }

    public static bool operator ==(LineCapBase a, LineCapBase b)
    {
      return a is { } _ ? a.Equals(b) : b is { } _ ? b.Equals(a) : true;
    }

    public static bool operator !=(LineCapBase a, LineCapBase b)
    {
      return !(a == b);
    }

    #region Cap styles registry

    private static System.Collections.Generic.SortedDictionary<string, ILineCap> _registeredStyles;
    private static System.Collections.Generic.List<ILineCap> _registeredStylesSortedByName;
    private static System.Collections.Generic.SortedDictionary<string, ILineCap> _deprecatedGdiStyles;

    public static ILineCap Flat
    {
      get { return FlatCap.Instance; }
    }

    public static bool IsDefaultStyle(ILineCap cap)
    {
      return cap is FlatCap;
    }

    public static ILineCap FromName(string name)
    {
      if (_registeredStyles.TryGetValue(name, out var currentStyle))
      {
        return currentStyle;
      }
      else if (_deprecatedGdiStyles.TryGetValue(name, out currentStyle))
      {
        return currentStyle;
      }
      else
        throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
    }

    public static ILineCap FromNameAndAbsSize(string name, double sizePt)
    {

      if (_registeredStyles.TryGetValue(name, out var currentStyle))
      {
        return currentStyle.WithMinimumAbsoluteAndRelativeSize(sizePt, currentStyle is LineCapBase bc ? bc.DefaultMinimumRelativeSize : 4);
      }
      else if (_deprecatedGdiStyles.TryGetValue(name, out currentStyle))
      {
        return currentStyle;
      }
      else
        throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
    }

    public static ILineCap FromNameAndAbsAndRelSize(string name, double sizePt, double relSize)
    {

      if (_registeredStyles.TryGetValue(name, out var currentStyle))
      {
        return currentStyle.WithMinimumAbsoluteAndRelativeSize(sizePt, relSize);
      }
      else if (_deprecatedGdiStyles.TryGetValue(name, out currentStyle))
      {
        return currentStyle;
      }
      else
      {
        throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
      }
    }

    public static IEnumerable<ILineCap> GetRegisteredValues()
    {
      return _registeredStylesSortedByName;
    }



    bool IEquatable<ILineCap>.Equals(ILineCap? other)
    {
      throw new NotImplementedException();
    }

    static LineCapBase()
    {
      // first register the old deprecated Gdi styles
      _deprecatedGdiStyles = new System.Collections.Generic.SortedDictionary<string, ILineCap>();
      foreach (LineCap cap in Enum.GetValues(typeof(LineCap)))
      {
        switch (cap)
        {
          case LineCap.AnchorMask:
          case LineCap.Flat:
          case LineCap.NoAnchor:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, FlatCap.Instance);
            break;

          case LineCap.Square:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.SquareFLineCap(0, 1));
            break;

          case LineCap.Round:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.CircleFLineCap(0, 1));
            break;

          case LineCap.Triangle:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.DiamondFLineCap(0, 1));
            break;

          case LineCap.ArrowAnchor:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.ArrowF10LineCap(0, 2));
            break;

          case LineCap.SquareAnchor:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.SquareFLineCap(0, 1.5));
            break;

          case LineCap.RoundAnchor:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.CircleFLineCap(0, 2));
            break;

          case LineCap.DiamondAnchor:
            _deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap)!, new LineCaps.DiamondFLineCap(0, 2));
            break;
        }
      }

      // now the other linecaps
      _registeredStyles = new System.Collections.Generic.SortedDictionary<string, ILineCap>();
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ILineCap));
      foreach (var t in types)
      {
        var ex = (ILineCap?)Activator.CreateInstance(t);
        if (ex is not null)
          _registeredStyles.Add(ex.Name, ex);
      }

      // now sort them by name
      var nameList = new List<string>(_registeredStyles.Keys);
      nameList.Remove(FlatCap.Instance.Name);
      nameList.Sort();
      nameList.Insert(0, FlatCap.Instance.Name);
      _registeredStylesSortedByName = new List<ILineCap>(nameList.Select(x => _registeredStyles[x]));
    }

    #endregion Cap styles registry
  }
}
