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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D.LineCaps
{
  public class Round : ContourShapedLineCapBase
  {
    private class RoundContour : ILineCapContour
    {
      private const int NumberOfPoints = 8;

      public int NumberOfNormals
      {
        get
        {
          return NumberOfPoints;
        }
      }

      public int NumberOfVertices
      {
        get
        {
          return NumberOfPoints;
        }
      }

      public bool IsVertexSharp(int idx)
      {
        return false;
      }

      public VectorD2D Normals(int idx)
      {
        if (idx == 0)
          return new VectorD2D(0, 1);
        else if (idx == (NumberOfPoints - 1))
          return new VectorD2D(1, 0);
        else
        {
          double phi = (0.5 * Math.PI * idx) / (NumberOfPoints - 1);
          return new VectorD2D(Math.Sin(phi), Math.Cos(phi));
        }
      }

      public PointD2D Vertices(int idx)
      {
        if (idx == 0)
          return new PointD2D(0, 1);
        else if (idx == (NumberOfPoints - 1))
          return new PointD2D(1, 0);
        else
        {
          double phi = (0.5 * Math.PI * idx) / (NumberOfPoints - 1);
          return new PointD2D(Math.Sin(phi), Math.Cos(phi));
        }
      }
    }

    private static RoundContour _contour = new RoundContour();

    #region Serialization

    /// <summary>
    /// 2016-05-02 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Round), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return new Round();
      }
    }

    #endregion Serialization

    public override double GetAbsoluteBaseInset(double thickness1, double thickness2)
    {
      return -0.5 * Math.Max(thickness1, thickness2);
    }

    public override void AddGeometry(Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int, bool> AddIndices, ref int vertexIndexOffset, bool isStartCap, PointD3D basePoint, VectorD3D eastVector, VectorD3D northVector, VectorD3D forwardVectorNormalized, ICrossSectionOfLine lineCrossSection, PointD3D[] baseCrossSectionPositions, VectorD3D[] baseCrossSectionNormals, ref object temporaryStorageSpace)
    {
      Add(
        AddPositionAndNormal,
        AddIndices,
        ref vertexIndexOffset,
        isStartCap,
        basePoint,
        eastVector,
        northVector,
        forwardVectorNormalized,
        lineCrossSection,
        baseCrossSectionPositions,
        baseCrossSectionNormals,
        ref temporaryStorageSpace,
        _contour);
    }
  }
}
