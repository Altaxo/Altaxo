﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Drawing.D3D.CrossSections
{
  using Geometry;

  public class Triangular : ICrossSectionOfLine
  {
    private double _size1By2, _size2By2;

    #region Serialization

    /// <summary>
    /// 2015-11-18 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Triangular), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Triangular)obj;

        info.AddValue("Size1", 2 * s._size1By2);
        info.AddValue("Size2", 2 * s._size2By2);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        double size1 = info.GetDouble("Size1");
        double size2 = info.GetDouble("Size2");
        return new Triangular(size1, size2);
      }
    }

    #endregion Serialization

    public Triangular()
      : this(1, 1)
    {
    }

    public Triangular(double size1, double size2)
    {
      if (!(size1 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size1), "must be >= 0");
      if (!(size2 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size2), "must be >= 0");
      if (0 == size1 && 0 == size2)
        throw new ArgumentOutOfRangeException(nameof(size2), "both size values are zero");

      _size1By2 = size1 / 2;
      _size2By2 = size2 / 2;
    }

    public double Size1
    {
      get
      {
        return _size1By2 * 2;
      }
    }

    public ICrossSectionOfLine WithSize1(double size1)
    {
      if (!(size1 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size1), "must be >= 0");

      var r = size1 / 2;

      if (r == _size1By2)
      {
        return this;
      }
      else
      {
        var result = (Triangular)MemberwiseClone();
        result._size1By2 = r;
        return result;
      }
    }

    public double Size2
    {
      get
      {
        return _size2By2 * 2;
      }
    }

    public ICrossSectionOfLine WithSize2(double size2)
    {
      if (!(size2 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size2), "must be >= 0");

      var r = size2 / 2;

      if (r == _size2By2)
      {
        return this;
      }
      else
      {
        var result = (Triangular)MemberwiseClone();
        result._size2By2 = r;
        return result;
      }
    }

    public ICrossSectionOfLine WithSize(double size1, double size2)
    {
      if (!(size1 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size1), "must be >= 0");
      if (!(size2 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size2), "must be >= 0");

      var r1 = size1 / 2;
      var r2 = size2 / 2;

      if (r1 == _size1By2 && r2 == _size2By2)
      {
        return this;
      }
      else
      {
        var result = (Triangular)MemberwiseClone();
        result._size1By2 = r1;
        result._size2By2 = r2;
        return result;
      }
    }

    public double GetMaximalDistanceFromCenter()
    {
      return Math.Sqrt(_size1By2 * _size1By2 + _size2By2 * _size2By2);
    }

    public bool IsVertexSharp(int idx)
    {
      return true;
    }

    public int NumberOfNormals
    {
      get
      {
        return 6;
      }
    }

    public VectorD2D Normals(int i)
    {
      switch (i)
      {
        case 0:
          return new VectorD2D(0, -1);

        case 1:
        case 2:
          return new VectorD2D(Cos30 * _size2By2, Sin30 * _size1By2).Normalized;

        case 3:
        case 4:
          return new VectorD2D(-Cos30 * _size2By2, Sin30 * _size1By2).Normalized;

        case 5:
          return new VectorD2D(0, -1);

        default:
          throw new IndexOutOfRangeException();
      }
    }

    public int NumberOfVertices
    {
      get
      {
        return 3;
      }
    }

    private static readonly double Cos30 = Math.Cos(Math.PI / 6);
    private static readonly double Sin30 = Math.Sin(Math.PI / 6);

    public PointD2D Vertices(int i)
    {
      switch (i)
      {
        case 0:
          return new PointD2D(_size1By2 * Cos30, -_size2By2 * Sin30);

        case 1:
          return new PointD2D(0, _size2By2);

        case 2:
          return new PointD2D(-_size1By2 * Cos30, -_size2By2 * Sin30);

        default:
          throw new IndexOutOfRangeException();
      }
    }
  }
}
