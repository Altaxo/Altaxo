#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Altaxo.Geometry;

namespace Altaxo.Serialization.WITec
{

  /// <summary>
  /// Represents a space transformation defined in a WITec "TDSpaceTransformation" node.
  /// This class currently only exposes the underlying node; transformation logic, if any, is implemented in derived classes.
  /// </summary>
  public class TDSpaceTransformationClass : TDTransformationClass
  {
    /// <summary>
    /// Backing node for the "TDSpaceTransformation" child node.
    /// </summary>
    private WITecTreeNode _tdSpaceTransformation;

    /// <summary>
    /// Gets information about the line, if available.
    /// </summary>
    public LineInformation? LineInfo { get; private set; }

    /// <summary>
    /// Gets the 3D viewport information, if available.
    /// </summary>
    public ViewPort3D? ViewPort { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TDSpaceTransformationClass"/> class.
    /// </summary>
    /// <param name="node">The node representing this transformation in the WITec tree.</param>
    /// <param name="reader">The reader used to resolve referenced nodes if necessary.</param>
    public TDSpaceTransformationClass(WITecTreeNode node, WITecReader reader) : base(node, reader)
    {
      _tdSpaceTransformation = node.GetChild("TDSpaceTransformation");

      if (_tdSpaceTransformation.GetData<bool>("LineInformationValid"))
      {
        LineInfo = new LineInformation(_tdSpaceTransformation);
      }
      if (_tdSpaceTransformation.ChildNodes.TryGetValue("ViewPort3D", out var tdViewPort3D))
      {
        ViewPort = new ViewPort3D(tdViewPort3D);
      }
    }

    /// <summary>
    /// Gets the coordinates, given the two dimensions.
    /// </summary>
    /// <param name="dim1">The first dimension (number of points in x-direction).</param>
    /// <param name="dim2">The second dimension (number of points in y-direction).</param>
    /// <returns>The coordinates associated with the spectra.</returns>
    public PointD3D[,] GetCoordinates(int dim1, int dim2)
    {
      // calculate the positions of every point
      var PositionValues = new PointD3D[dim1, dim2];

      if (LineInfo is { } li)
      {
        if (li.NumberOfLinePoints != dim1 || dim2 != 1)
          throw new InvalidProgramException($"Obviously my assumption is wrong that dim1 is NumberOfLinePoints, and dim2=1. Please debug!");

        for (int i = 0; i < li.NumberOfLinePoints; ++i)
        {
          var r = i / (double)li.NumberOfLinePoints;
          var p = (1 - r) * li.LineStart + (r) * li.LineEnd;
          PositionValues[i, 0] = (PointD3D)p;
        }
      }
      else if (ViewPort is not null)
      {
        for (int i = 0; i < dim1; ++i)
        {
          for (int j = 0; j < dim2; ++j)
          {
            var x = new VectorD3D(i, j, 0);
            x = x - ViewPort.ModelOrigin;
            // TODO not sure whether first rotate then scale or vice versa
            x = ViewPort.Scale.Transform(x);
            x = ViewPort.Rotation.Transform(x);
            var p = ViewPort.WorldOrigin + x;
            PositionValues[i, j] = p;
          }
        }
      }
      else
      {
        throw new System.InvalidProgramException($"In TDSpaceTransformation, either the LineInfo or the ViewPort must be valid.");
      }
      return PositionValues;
    }
  }

  /// <summary>
  /// Represents information about a line, including its start and end points and the number of points along the line.
  /// </summary>
  public class LineInformation
  {
    /// <summary>
    /// Gets the start point of the line.
    /// </summary>
    public Altaxo.Geometry.VectorD3D LineStart { get; }
    /// <summary>
    /// Gets the end point of the line.
    /// </summary>
    public Altaxo.Geometry.VectorD3D LineEnd { get; }
    /// <summary>
    /// Gets the number of points along the line.
    /// </summary>
    public int NumberOfLinePoints { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineInformation"/> class from a WITec node.
    /// </summary>
    /// <param name="node">The node containing line information data.</param>
    public LineInformation(WITecTreeNode node)
    {
      if (node.Data.ContainsKey("LineStart_D"))
      {
        var start = node.GetData<double[]>("LineStart_D");
        LineStart = new Altaxo.Geometry.VectorD3D(start[0], start[1], start[2]);
      }
      else
      {
        var start = node.GetData<float[]>("LineStart");
        LineStart = new Altaxo.Geometry.VectorD3D(start[0], start[1], start[2]);
      }

      if (node.Data.ContainsKey("LineStop_D"))
      {
        var end = node.GetData<double[]>("LineStop_D");
        LineEnd = new Altaxo.Geometry.VectorD3D(end[0], end[1], end[2]);
      }
      else
      {
        var end = node.GetData<float[]>("LineStop");
        LineEnd = new Altaxo.Geometry.VectorD3D(end[0], end[1], end[2]);
      }

      NumberOfLinePoints = node.GetData<int>("NumberOfLinePoints");
    }
  }

  /// <summary>
  /// Represents a 3D viewport, including model and world origins, scale, and rotation.
  /// </summary>
  public class ViewPort3D
  {
    /// <summary>
    /// Gets or sets the model origin in 3D space.
    /// </summary>
    public Altaxo.Geometry.VectorD3D ModelOrigin { get; set; }
    /// <summary>
    /// Gets or sets the world origin in 3D space.
    /// </summary>
    public Altaxo.Geometry.PointD3D WorldOrigin { get; set; }
    /// <summary>
    /// Gets or sets the scale matrix for the viewport.
    /// </summary>
    public Altaxo.Geometry.Matrix3x3 Scale { get; set; }
    /// <summary>
    /// Gets or sets the rotation matrix for the viewport.
    /// </summary>
    public Altaxo.Geometry.Matrix3x3 Rotation { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewPort3D"/> class from a WITec node.
    /// </summary>
    /// <param name="node">The node containing viewport data.</param>
    public ViewPort3D(WITecTreeNode node)
    {

      var modelOrg = node.GetData<double[]>("ModelOrigin");
      var worldOrg = node.GetData<double[]>("WorldOrigin");
      var scale = node.GetData<double[]>("Scale");
      var rot = node.GetData<double[]>("Rotation");

      ModelOrigin = new Geometry.VectorD3D(modelOrg[0], modelOrg[1], modelOrg[2]);
      WorldOrigin = new Geometry.PointD3D(worldOrg[0], worldOrg[1], worldOrg[2]);
      Scale = new Geometry.Matrix3x3(scale[0], scale[1], scale[2], scale[3], scale[4], scale[5], scale[6], scale[7], scale[8]);
      Rotation = new Geometry.Matrix3x3(rot[0], rot[1], rot[2], rot[3], rot[4], rot[5], rot[6], rot[7], rot[8]);
    }
  }
}
