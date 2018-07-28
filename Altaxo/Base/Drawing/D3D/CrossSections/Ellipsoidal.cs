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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D.CrossSections
{
  public class Ellipsoidal : ICrossSectionOfLine
  {
    private double _radius1;
    private double _radius2;

    private const int _numberOfVertices = 16;

    #region Serialization

    /// <summary>
    /// 2016-04-30 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Ellipsoidal), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Ellipsoidal)obj;

        info.AddValue("Size1", 2 * s._radius1);
        info.AddValue("Size2", 2 * s._radius2);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        double size1 = info.GetDouble("Radius1");
        double size2 = info.GetDouble("Radius2");
        return new Ellipsoidal(size1, size2);
      }
    }

    #endregion Serialization

    public Ellipsoidal()
      : this(1, 1)
    {
    }

    public Ellipsoidal(double size1, double size2)
    {
      if (!(size1 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size1), "must be >= 0");
      if (!(size2 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size2), "must be >= 0");
      if (0 == size1 && 0 == size2)
        throw new ArgumentOutOfRangeException(nameof(size2), "both size values are zero");

      _radius1 = size1 / 2;
      _radius2 = size2 / 2;
    }

    public int NumberOfNormals
    {
      get
      {
        return _numberOfVertices;
      }
    }

    public int NumberOfVertices
    {
      get
      {
        return _numberOfVertices;
      }
    }

    public double Size1
    {
      get
      {
        return _radius1 * 2;
      }
    }

    public ICrossSectionOfLine WithSize1(double size1)
    {
      if (!(size1 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size1), "must be >= 0");

      var r = size1 / 2;

      if (r == _radius1)
      {
        return this;
      }
      else
      {
        var result = (Ellipsoidal)MemberwiseClone();
        result._radius1 = r;
        return result;
      }
    }

    public double Size2
    {
      get
      {
        return _radius2 * 2;
      }
    }

    public ICrossSectionOfLine WithSize2(double size2)
    {
      if (!(size2 >= 0))
        throw new ArgumentOutOfRangeException(nameof(size2), "must be >= 0");

      var r = size2 / 2;

      if (r == _radius2)
      {
        return this;
      }
      else
      {
        var result = (Ellipsoidal)MemberwiseClone();
        result._radius2 = r;
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

      if (r1 == _radius1 && r2 == _radius2)
      {
        return this;
      }
      else
      {
        var result = (Ellipsoidal)MemberwiseClone();
        result._radius1 = r1;
        result._radius2 = r2;
        return result;
      }
    }

    public double GetMaximalDistanceFromCenter()
    {
      return Math.Max(_radius1, _radius2);
    }

    public bool IsVertexSharp(int idx)
    {
      return false;
    }

    public VectorD2D Normals(int i)
    {
      double phi = i * (2 * Math.PI / _numberOfVertices);
      return VectorD2D.CreateNormalized(_radius2 * Math.Cos(phi), _radius1 * Math.Sin(phi));
    }

    public PointD2D Vertices(int i)
    {
      double phi = i * (2 * Math.PI / _numberOfVertices);
      return new PointD2D(_radius1 * Math.Cos(phi), _radius2 * Math.Sin(phi));
    }
  }
}
