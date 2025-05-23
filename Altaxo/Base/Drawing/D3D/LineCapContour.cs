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
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// The contour for a line cap. A list of contour points describes the shape of the cap. The x component of each contour point corresponds to the advance of the cap in line direction,
  /// whereas the y-component of each contour point is multiplied with the cross section of the line (i.e. it is the relative thickness of the cap at this point).
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public interface ILineCapContour : Main.IImmutable
  {
    /// <summary>
    /// Returns the vertex at index <paramref name="idx"/>. The x component of the returned value is responsible for the advance of the cap in the direction of the line, the y component is multiplied with the cross section of the line.
    /// </summary>
    /// <param name="idx">The index. The range is 0 .. <see cref="NumberOfVertices"/> - 1. </param>
    /// <returns>The vertex at index <paramref name="idx"/>.</returns>
    PointD2D Vertices(int idx);

    /// <summary>
    /// Determines whether the vertex at index <paramref name="idx"/> is a sharp vertex.
    /// </summary>
    /// <param name="idx">The index. The range is 0 .. <see cref="NumberOfVertices"/> - 1. </param>
    /// <returns>True if the vertex is sharp; otherwise false.</returns>
    bool IsVertexSharp(int idx);

    /// <summary>
    /// Returns the normal at index <paramref name="idx"/>.
    /// </summary>
    /// <param name="idx">The index. The range is 0 .. <see cref="NumberOfNormals"/> - 1. </param>
    /// <returns>The normal at index <paramref name="idx"/>.</returns>
    VectorD2D Normals(int idx);

    /// <summary>
    /// Gets the number of normals. The number of normals is calculated from the number of vertices, plus the number of all sharp vertices.
    /// </summary>
    /// <value>
    /// The number of normals.
    /// </value>
    int NumberOfNormals { get; }

    /// <summary>
    /// Gets the number of vertices.
    /// </summary>
    /// <value>
    /// The number of vertices.
    /// </value>
    int NumberOfVertices { get; }
  }

  public class LineCapContour
  {
    private PointD2D[] _vertices;
    private bool[] _isVertexSharp;
    private VectorD2D[] _normals;

    protected LineCapContour(int numberOfVertices)
    {
      _vertices = new PointD2D[numberOfVertices];
      _isVertexSharp = new bool[numberOfVertices];
      _normals = new VectorD2D[numberOfVertices];
    }

    protected LineCapContour(int numberOfVertices, int numberOfIsVertexSharp, int numberOfNormals)
    {
      _vertices = new PointD2D[numberOfVertices];
      _isVertexSharp = new bool[numberOfIsVertexSharp];
      _normals = new VectorD2D[numberOfNormals];
    }

    public PointD2D[] Vertices { get { return _vertices; } }
    public bool[] IsVertexSharp { get { return _isVertexSharp; } }
    public VectorD2D[] Normals { get { return _normals; } }

    public int NumberOfVertices { get { return _vertices.Length; } }
    public int NumberOfNormals { get { return _normals.Length; } }

    private void NormalizeNormals()
    {
      for (int i = 0; i < _normals.Length; ++i)
        _normals[i] = _normals[i].Normalized;
    }

    public static LineCapContour Ball
    {
      get
      {
        const int segments = 4;
        var r = new LineCapContour(2 + segments);
        r._vertices[0] = new PointD2D(0, 0);
        r._isVertexSharp[0] = false;
        r._normals[0] = new VectorD2D(0, -1);

        for (int i = 0; i < segments; ++i)
        {
          double phi = i * (0.5 * Math.PI) / segments;
          r._vertices[i + 1] = new PointD2D(Math.Cos(phi), Math.Sin(phi));
          r._normals[i + 1] = new VectorD2D(Math.Cos(phi), Math.Sin(phi));
          r._isVertexSharp[i + 1] = false;
        }

        r._vertices[segments + 1] = new PointD2D(0, 1);
        r._normals[segments + 1] = new VectorD2D(0, 1);
        r._isVertexSharp[segments + 1] = false;

        return r;
      }
    }

    public static LineCapContour Triangle
    {
      get
      {
        var r = new LineCapContour(numberOfVertices: 3, numberOfIsVertexSharp: 2, numberOfNormals: 2);
        r._vertices[0] = new PointD2D(0, 1);
        r._vertices[1] = new PointD2D(1, 0);
        r._isVertexSharp[0] = true;
        r._isVertexSharp[1] = true;
        r._normals[0] = VectorD2D.CreateNormalized(1, 1);
        r._normals[1] = VectorD2D.CreateNormalized(1, 1);
        return r;
      }
    }

    public static LineCapContour Arrow
    {
      get
      {
        var r = new LineCapContour(numberOfVertices: 3, numberOfIsVertexSharp: 3, numberOfNormals: 4);
        r._vertices[0] = new PointD2D(0, 0);
        r._vertices[1] = new PointD2D(1, 0);
        r._vertices[2] = new PointD2D(0, 1);

        r._isVertexSharp[0] = false;
        r._isVertexSharp[1] = true;
        r._isVertexSharp[2] = false;

        r._normals[0] = new VectorD2D(0, -1);
        r._normals[1] = new VectorD2D(0, -1);
        r._normals[2] = new VectorD2D(1, 1);
        r._normals[3] = new VectorD2D(1, 1);

        r.NormalizeNormals();

        return r;
      }
    }

    public static LineCapContour BigArrow
    {
      get
      {
        var r = new LineCapContour(numberOfVertices: 3, numberOfIsVertexSharp: 3, numberOfNormals: 4);

        r._vertices[0] = new PointD2D(0, 0);
        r._vertices[1] = new PointD2D(2, 0);
        r._vertices[2] = new PointD2D(0, 4);

        r._isVertexSharp[0] = false;
        r._isVertexSharp[1] = true;
        r._isVertexSharp[2] = false;

        r._normals[0] = new VectorD2D(0, -1);
        r._normals[1] = new VectorD2D(0, -1);
        r._normals[2] = new VectorD2D(4, 2);
        r._normals[3] = new VectorD2D(4, 2);

        r.NormalizeNormals();

        return r;
      }
    }

    public static LineCapContour Block
    {
      get
      {
        var r = new LineCapContour(numberOfVertices: 4, numberOfIsVertexSharp: 4, numberOfNormals: 6);

        r._vertices[0] = new PointD2D(0, 0);
        r._vertices[1] = new PointD2D(1, 0);
        r._vertices[2] = new PointD2D(1, 1);
        r._vertices[3] = new PointD2D(0, 1);

        r._isVertexSharp[0] = false;
        r._isVertexSharp[1] = true;
        r._isVertexSharp[2] = true;
        r._isVertexSharp[3] = false;

        r._normals[0] = new VectorD2D(0, -1);
        r._normals[1] = new VectorD2D(0, -1);
        r._normals[2] = new VectorD2D(1, 0);
        r._normals[3] = new VectorD2D(1, 0);
        r._normals[4] = new VectorD2D(0, 1);
        r._normals[5] = new VectorD2D(0, 1);

        r.NormalizeNormals();

        return r;
      }
    }
  }
}
