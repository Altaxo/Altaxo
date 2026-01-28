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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Geometry;

namespace Altaxo.Serialization.WITec
{
  /// <summary>
  /// Represents a graph data class (TDGraph) read from a WITec project node.
  /// This class extracts the X and Z values as well as metadata such as units and titles.
  /// </summary>
  public class TDGraphClass : TDataClass
  {
    /// <summary>
    /// Backing node for the TDGraph section of the data class.
    /// </summary>
    protected WITecTreeNode _tdGraph;

    /// <summary>
    /// Backing node for the GraphData child node inside the TDGraph node.
    /// </summary>
    protected WITecTreeNode _tdGraph_GraphData;

    /// <summary>
    /// Gets or sets the X axis values for the graph. This usually represents the spectral axis.
    /// </summary>
    public double[] XValues { get; set; }

    /// <summary>
    /// Gets or sets the collection of Z value arrays. Each entry represents one spectrum or series.
    /// </summary>
    public double[,][] ZValues { get; set; }

    /// <summary>
    /// Gets or sets the position in space that is associated with each spectrum. Only valid if this instance contains a <see cref="TDSpaceTransformationClass"/>.
    /// </summary>
    public PointD3D[,]? SpacePositionValues { get; set; }

    /// <summary>
    /// Gets the graph title.
    /// </summary>
    public string Title { get; } = string.Empty;

    /// <summary>
    /// Gets the unit shortcut for the X axis.
    /// </summary>
    public string XUnitShortcut { get; } = string.Empty;
    /// <summary>
    /// Gets the unit description for the X axis.
    /// </summary>
    public string XUnitDescription { get; } = string.Empty;

    /// <summary>
    /// Gets the unit shortcut for the Z values.
    /// </summary>
    public string ZUnitShortcut { get; } = string.Empty;
    /// <summary>
    /// Gets the unit description for the Z values.
    /// </summary>
    public string ZUnitDescription { get; } = string.Empty;

    /// <summary>
    /// Gets any informational RTF text associated with this graph.
    /// </summary>
    public string InformationRtfText { get; } = string.Empty;

    /// <summary>
    /// Type of graph represented by this class.
    /// </summary>
    public enum GraphClassType
    {
      /// <summary>
      /// Unknown graph type.
      /// </summary>
      Unknown,
      /// <summary>
      /// Spectral data (e.g. spectra).
      /// </summary>
      SpectralData,
      /// <summary>
      /// Time series data.
      /// </summary>
      TimeSeries,
    }

    /// <summary>
    /// Gets the determined graph type for this instance.
    /// </summary>
    public GraphClassType GraphType { get; }

    /// <summary>
    /// Metadata for Z values that may be shared or derived from other graphs.
    /// </summary>
    public class MetaData
    {
      /// <summary>
      /// Gets the unit shortcut for the Z values.
      /// </summary>
      public string ZUnitShortcut { get; init; } = string.Empty;
      /// <summary>
      /// Gets the unit description for the Z values.
      /// </summary>
      public string ZUnitDescription { get; init; } = string.Empty;

      /// <summary>
      /// Gets the Z values used as metadata (for example when the Z axis represents a measured parameter across spectra).
      /// </summary>
      public double[] ZValues { get; init; }
    }

    /// <summary>
    /// Optional metadata for Z values. May be null if no metadata was found or inferred.
    /// </summary>
    public MetaData? ZMetaData { get; }

    /// <summary>
    /// Gets the kind text (suffix) extracted from the caption that describes the data kind (for example "Spec.Data" or "Elapsed Time").
    /// </summary>
    public string KindText { get; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TDGraphClass"/> class using the provided node and reader.
    /// The constructor extracts graph data, units, titles and optional metadata from the WITec nodes.
    /// </summary>
    /// <param name="node">The node representing the data class entry in the WITec tree.</param>
    /// <param name="reader">The reader used to resolve referenced nodes by their identifiers.</param>
    public TDGraphClass(WITecTreeNode node, WITecReader reader)
      : base(node)
    {
      _tdGraph = node.GetChild("TDGraph");
      _tdGraph_GraphData = _tdGraph.GetChild("GraphData");

      var idx = Caption.LastIndexOf("_");
      if (idx >= 0)
      {
        Title = Caption[..idx];
        KindText = Caption[(idx + 1)..];
      }
      if (Caption.Contains("Spec.Data"))
        GraphType = GraphClassType.SpectralData;
      else if (Caption.Contains("Elapsed Time"))
        GraphType = GraphClassType.TimeSeries;



      var xtransformationID = _tdGraph.GetData<int>("XTransformationID");
      TDTransformationClass? xtransformation = null;
      if (xtransformationID != 0)
      {
        xtransformation = reader.BuildNodeWithID<TDTransformationClass>(xtransformationID);
        XUnitShortcut = xtransformation.StandardUnit;
        XUnitDescription = xtransformation.StandardUnit;
      }

      var xinterpretationID = _tdGraph.GetData<int>("XInterpretationID");
      TDInterpretationClass? xinterpretation = null;
      if (xinterpretationID != 0)
      {
        xinterpretation = reader.BuildNodeWithID<TDInterpretationClass>(xinterpretationID);
        if (!string.IsNullOrEmpty(xinterpretation.UnitShortCut))
          XUnitShortcut = xinterpretation.UnitShortCut;
        if (!string.IsNullOrEmpty(xinterpretation.UnitDescription))
          XUnitDescription = xinterpretation.UnitDescription;
      }


      var zinterpretationId = _tdGraph.GetData<int>("ZInterpretationID");
      TDInterpretationClass? zinterpretation = null;
      if (0 != zinterpretationId)
      {
        zinterpretation = reader.BuildNodeWithID<TDInterpretationClass>(zinterpretationId);
        ZUnitShortcut = zinterpretation.UnitShortCut;
        ZUnitDescription = zinterpretation.UnitDescription;

        if (zinterpretation is TDZInterpretationClass tdzi)
        {
          if (string.IsNullOrEmpty(ZUnitShortcut) && !string.IsNullOrEmpty(tdzi.UnitName))
            ZUnitShortcut = tdzi.UnitName;
          if (string.IsNullOrEmpty(ZUnitDescription) && !string.IsNullOrEmpty(tdzi.UnitName))
            ZUnitDescription = tdzi.UnitName;
        }
      }

      var spaceTransformationId = _tdGraph.GetData<int>("SpaceTransformationID");
      TDTransformationClass? spaceTransformation = null;
      if (0 != spaceTransformationId)
      {
        spaceTransformation = reader.BuildNodeWithID<TDTransformationClass>(spaceTransformationId);
      }


      var ranges = _tdGraph_GraphData.GetData<int[]>("Ranges");
      var dataType = _tdGraph_GraphData.GetData<int>("DataType");
      var data = _tdGraph_GraphData.GetData<byte[]>("Data");

      int dim1; // x-dimension, e.g. line scan
      int dim2; // y-dimension, e.g. depth in line scans with different depths, or in x-y-scans
      int spectralDimension; // number of spectral points

      if (ranges.Length == 1)
      {
        dim1 = 1;
        dim2 = 1;
        spectralDimension = _tdGraph.GetData<int>("SizeGraph");
        if (!(ranges[0] == spectralDimension))
          throw new NotImplementedException("Please debug. Compare Sizes in _tdGraph with sizes in _tdGraph_GraphData");
      }
      else if (ranges.Length == 2)
      {
        dim1 = _tdGraph.GetData<int>("SizeX");
        dim2 = _tdGraph.GetData<int>("SizeY");
        spectralDimension = _tdGraph.GetData<int>("SizeGraph");
        if (dim1 != ranges[0])
          throw new NotImplementedException("Please debug. Compare Sizes in _tdGraph with sizes in _tdGraph_GraphData");
      }
      else
      {
        throw new NotImplementedException($"Array dimensions of 3 or more are currently not supported");
      }

      if (spaceTransformation is TDSpaceTransformationClass spt)
      {
        SpacePositionValues = spt.GetCoordinates(dim1, dim2);
      }


      // allocate the arrays that accommodate the spectral values
      var yArrays = new double[dim1, dim2][];
      for (int i = 0; i < dim1; ++i)
        for (int j = 0; j < dim2; ++j)
          yArrays[i, j] = new double[spectralDimension];

      switch (dataType)
      {
        case 2: // unsigned byte array
          {
            int ptr = 0;
            for (int i = 0; i < dim1; ++i)
            {
              for (int j = 0; j < dim2; ++j)
              {
                for (int k = 0; k < spectralDimension; ++k)
                {
                  yArrays[i, j][k] = data[ptr];
                  ptr += sizeof(byte);
                }
              }
            }
          }
          break;
        case 6: // ushort array
          {
            int ptr = 0;
            for (int i = 0; i < dim1; ++i)
            {
              for (int j = 0; j < dim2; ++j)
              {
                for (int k = 0; k < spectralDimension; ++k)
                {
                  yArrays[i, j][k] = BitConverter.ToUInt16(data, ptr);
                  ptr += sizeof(ushort);
                }
              }
            }
          }
          break;
        case 9: // float array
          {
            int ptr = 0;
            for (int i = 0; i < dim1; ++i)
            {
              for (int j = 0; j < dim2; ++j)
              {
                for (int k = 0; k < spectralDimension; ++k)
                {
                  yArrays[i, j][k] = BitConverter.ToSingle(data, ptr);
                  ptr += sizeof(Single);
                }
              }
            }
          }
          break;
        default:
          throw new NotImplementedException("The data type {dataType} is not implemented yet, but debug this and then implement it.");
      }


      double[] xArray;
      if (xtransformation is TDSpectralTransformationClass specTrans)
      {
        xArray = specTrans.Transform(Enumerable.Range(0, spectralDimension).Select(x => (double)x)).ToArray();

        if (xinterpretation is TDSpectralInterpretationClass spectralInterpretation &&
              spectralInterpretation.UnitIndex == 3 &&
              specTrans.StandardUnit == "nm" && spectralInterpretation.ExcitationWaveLength > 0)
        {
          VectorMath.Map(xArray, wl => 1E7 * (1 / spectralInterpretation.ExcitationWaveLength - 1 / wl));
        }
      }
      else
      {
        xArray = Enumerable.Range(0, spectralDimension).Select(x => (double)x).ToArray();
      }
      XValues = xArray;
      ZValues = yArrays;

      // now some optional things

      // if we have more than one spectrum, then is is probably a time series?. At least we look for a secondary trans
      var secondaryTransformationID = _tdGraph.GetData<int>("SecondaryTransformationID");
      if (secondaryTransformationID != 0)
      {
        // we don't need to build this secondary transformation.
        // instead, it would be sensible to look for another graph that has this transformation
      }

      if (yArrays.Length > 1 && reader.Spectra.Count > 0)
      {
        // look if the previous graph is a time series graph
        var previousGraph = reader.Spectra[^1];
        if (previousGraph.GraphType == GraphClassType.TimeSeries && previousGraph.ZValues.GetLength(0) == 1 && previousGraph.ZValues.GetLength(1) == 1 && previousGraph.ZValues[0, 0].Length == yArrays.Length)
        {
          ZMetaData = new MetaData
          {
            ZUnitShortcut = previousGraph.ZUnitShortcut,
            ZUnitDescription = previousGraph.KindText,
            ZValues = previousGraph.ZValues[0, 0],
          };
        }
      }

      {
        // we can also look for a TDText node. Caveat: the text node seems not to be coupled with the TDGraph node here.
        // the only possiblity I see is to extract the sample name, and then search for a text node with appended 'Information'
        var parts = Node.Name.Split(' ');
        var index = int.Parse(parts[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

        var dataNode = Node.Parent;

        for (int i = index + 1; i < dataNode.ChildNodes.Count; i++)
        {
          var className = dataNode.GetData<string>($"DataClassName {i}");
          if (className == "TDGraph")
            break; // don't go beyond a TDGraph node
          if (className == "TDText")
          {
            var textNode = new TDTextClass(dataNode.GetChild($"Data {i}"));
            if (!string.IsNullOrEmpty(Title) && textNode.Caption.StartsWith(Title))
            {
              InformationRtfText = textNode.TextRtfFormat;
            }
          }
        }
      }
    }
  }
}
