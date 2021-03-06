﻿#region Copyright

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
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D.CrossSections
{
  /// <summary>
  /// Represents the cross section of a line. It is assumed here that the midpoint of the line is the point (0,0) and that all
  /// edges can be connected to the midpoint without leaving the cross section.
  /// </summary>
  public class CrossSectionOfLine
  {
    protected double _size1, _size2;

    protected PointD2D[] _vertices;
    protected bool[] _isVertexSharp;
    protected VectorD2D[] _normals;

    protected CrossSectionOfLine(int numberOfVertices, int numberOfNormals)
    {
      _vertices = new PointD2D[numberOfVertices];
      _isVertexSharp = new bool[numberOfVertices];
      _normals = new VectorD2D[numberOfNormals];
    }

    public double Size1 { get { return _size1; } }
    public double Size2 { get { return _size2; } }

    public PointD2D Vertices(int idx)
    {
      return _vertices[idx];
    }

    public bool IsVertexSharp(int idx)
    {
      return _isVertexSharp[idx];
    }

    public VectorD2D Normals(int idx)
    {
      return _normals[idx];
    }

    public int NumberOfVertices { get { return _vertices.Length; } }
    public int NumberOfNormals { get { return _normals.Length; } }

    public double GetMaximalDistanceFromCenter()
    {
      double result = 0;
      for (int i = 0; i < _vertices.Length; ++i)
      {
        var p = _vertices[i];
        double l = Math.Sqrt(p.X * p.X + p.Y * p.Y);
        result = Math.Max(result, l);
      }
      return result;
    }

    public double GetDistanceFromCenter(int i)
    {
      var p = _vertices[i];
      return Math.Sqrt(p.X * p.X + p.Y * p.Y);
    }

    public static CrossSectionOfLine GetRegularSharpPolygon(double radius, int edges)
    {
      var result = new CrossSectionOfLine(numberOfVertices: edges, numberOfNormals: 2 * edges);

      for (int i = 0; i < edges; ++i)
      {
        double phi = (Math.PI * (2 * i)) / edges;
        result._vertices[i] = new PointD2D(radius * Math.Cos(phi), radius * Math.Sin(phi));
        result._isVertexSharp[i] = true;
        double phim = (Math.PI * (2 * i - 1)) / edges;
        result._normals[2 * i] = new VectorD2D(Math.Cos(phim), Math.Sin(phim));
        double phip = (Math.PI * (2 * i + 1)) / edges;
        result._normals[2 * i + 1] = new VectorD2D(Math.Cos(phip), Math.Sin(phip));
      }

      // normals
      return result;
    }

    public static CrossSectionOfLine GetRegularRoundedPolygon(double radius, int edges)
    {
      var result = new CrossSectionOfLine(numberOfVertices: edges, numberOfNormals: edges);

      for (int i = 0; i < edges; ++i)
      {
        double phi = (2 * Math.PI * i) / edges;
        result._vertices[i] = new PointD2D(radius * Math.Cos(phi), radius * Math.Sin(phi));
        result._isVertexSharp[i] = false;
        result._normals[i] = new VectorD2D(Math.Cos(phi), Math.Sin(phi));
      }

      // normals
      return result;
    }

    public static CrossSectionOfLine GetStarShapedPolygon(double innerRadius, double outerRadius, int starArms)
    {
      int numVertices = starArms * 2;
      int numNormals = 2 * numVertices;
      var result = new CrossSectionOfLine(numberOfVertices: numVertices, numberOfNormals: numNormals);

      for (int i = 0; i < numVertices; ++i)
      {
        double phi = (Math.PI * (2 * i)) / numVertices;
        double radius = 0 == (i % 2) ? outerRadius : innerRadius;
        result._vertices[i] = new PointD2D(radius * Math.Cos(phi), radius * Math.Sin(phi));
        result._isVertexSharp[i] = true;
      }

      // now the normals, we calculate them using the cross product with the z-axis
      var zaxis = new VectorD3D(0, 0, 1);
      for (int i = 0; i < numVertices; ++i)
      {
        var line = (VectorD2D)(result._vertices[(i + 1) % numVertices] - result._vertices[i]);
        VectorD2D cross = new VectorD2D(line.Y, line.X).Normalized;
        result._normals[(2 * i + 1) % numNormals] = cross;
        result._normals[(2 * i + 2) % numNormals] = cross;
      }

      // normals
      return result;
    }

    public static CrossSectionOfLine GetQuadraticCrossSection(double width)
    {
      var result = new CrossSectionOfLine(numberOfVertices: 4, numberOfNormals: 8);
      double w2 = width / 2;

      result._vertices[0] = new PointD2D(w2, -w2);
      result._vertices[1] = new PointD2D(w2, w2);
      result._vertices[2] = new PointD2D(-w2, w2);
      result._vertices[3] = new PointD2D(-w2, -w2);

      result._isVertexSharp[0] = true;
      result._isVertexSharp[1] = true;
      result._isVertexSharp[2] = true;
      result._isVertexSharp[3] = true;

      result._normals[0] = new VectorD2D(0, -1);
      result._normals[1] = new VectorD2D(1, 0);
      result._normals[2] = new VectorD2D(1, 0);
      result._normals[3] = new VectorD2D(0, 1);
      result._normals[4] = new VectorD2D(0, 1);
      result._normals[5] = new VectorD2D(-1, 0);
      result._normals[6] = new VectorD2D(-1, 0);
      result._normals[7] = new VectorD2D(0, -1);

      return result;
    }

    public static CrossSectionOfLine GetSquareCrossSection(double width, double height)
    {
      var result = new CrossSectionOfLine(numberOfVertices: 4, numberOfNormals: 8);
      double w2 = width / 2;
      double h2 = height / 2;

      result._vertices[0] = new PointD2D(w2, -h2);
      result._vertices[1] = new PointD2D(w2, h2);
      result._vertices[2] = new PointD2D(-w2, h2);
      result._vertices[3] = new PointD2D(-w2, -h2);

      result._isVertexSharp[0] = true;
      result._isVertexSharp[1] = true;
      result._isVertexSharp[2] = true;
      result._isVertexSharp[3] = true;

      result._normals[0] = new VectorD2D(0, -1);
      result._normals[1] = new VectorD2D(1, 0);
      result._normals[2] = new VectorD2D(1, 0);
      result._normals[3] = new VectorD2D(0, 1);
      result._normals[4] = new VectorD2D(0, 1);
      result._normals[5] = new VectorD2D(-1, 0);
      result._normals[6] = new VectorD2D(-1, 0);
      result._normals[7] = new VectorD2D(0, -1);

      return result;
    }

    /// <summary>
    /// Gets the vertices of a cross section from a certain vertex index to another vertex index (including the last vertex index).
    /// </summary>
    /// <param name="crossSection">The cross section.</param>
    /// <param name="firstVertexIndex">The first vertex index to include. Must be greater than or equal to zero.</param>
    /// <param name="lastIncludedVertexIndex">The last vertex index to include. Must be greater than <paramref name="firstVertexIndex"/>, but can be greater than the number of vertices of the cross section in order to describe a polygon that 'wraps around'.</param>
    /// <returns>The vertices from the first index up to and including the last index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static IEnumerable<PointD2D> GetVerticesFromToIncluding(ICrossSectionOfLine crossSection, int firstVertexIndex, int lastIncludedVertexIndex)
    {
      if (!(firstVertexIndex <= lastIncludedVertexIndex))
        throw new ArgumentOutOfRangeException(nameof(lastIncludedVertexIndex) + " must be greater then or equal to " + nameof(firstVertexIndex));
      if (!(firstVertexIndex >= 0))
        throw new ArgumentException(nameof(firstVertexIndex) + "must be >=0");

      int vertexCount = crossSection.NumberOfVertices;
      for (int i = firstVertexIndex; i <= lastIncludedVertexIndex; ++i)
        yield return crossSection.Vertices(i % vertexCount);
    }
  }
}
