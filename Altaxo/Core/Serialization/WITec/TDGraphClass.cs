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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Serialization.WITec
{
  public class TDGraphClass : TDataClass
  {
    protected WITecTreeNode _tdGraph;
    protected WITecTreeNode _tdGraph_GraphData;


    public double[] XValues { get; set; }

    public List<double[]> ZValues { get; set; }

    public string Title { get; } = string.Empty;

    public string XUnitShortcut { get; } = string.Empty;
    public string XUnitDescription { get; } = string.Empty;

    public string ZUnitShortcut { get; } = string.Empty;
    public string ZUnitDescription { get; } = string.Empty;

    public string InformationRtfText { get; } = string.Empty;

    public enum GraphClassType
    {
      Unknown,
      SpectralData,
      TimeSeries,
    }

    public GraphClassType GraphType { get; }

    public class MetaData
    {
      public string ZUnitShortcut { get; init; } = string.Empty;
      public string ZUnitDescription { get; init; } = string.Empty;

      public double[] ZValues { get; init; }
    }


    public MetaData? ZMetaData { get; }


    public string KindText { get; } = string.Empty;

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

        if (KindText.Contains("Spec.Data"))
          GraphType = GraphClassType.SpectralData;
        else if (KindText.Contains("Elapsed Time"))
          GraphType = GraphClassType.TimeSeries;
      }



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


      var ranges = _tdGraph_GraphData.GetData<int[]>("Ranges");
      var dataType = _tdGraph_GraphData.GetData<int>("DataType");
      var data = _tdGraph_GraphData.GetData<byte[]>("Data");

      double[][] yArrays;
      int dim1;
      int dim2;
      int spectralDimension;

      if (ranges.Length == 1)
      {
        yArrays = Enumerable.Range(0, 1).Select(i => new double[ranges[0]]).ToArray();
        dim1 = 1;
        dim2 = spectralDimension = ranges[0];
      }
      else if (ranges.Length == 2)
      {
        yArrays = Enumerable.Range(0, ranges[0]).Select(i => new double[ranges[1]]).ToArray();
        dim1 = ranges[0];
        dim2 = spectralDimension = ranges[1];
      }
      else
      {
        throw new NotImplementedException($"Array dimensions of 3 or more are currently not supported");
      }

      switch (dataType)
      {
        case 9: // float array
          {
            int ptr = 0;
            for (int i = 0; i < dim1; ++i)
            {
              for (int j = 0; j < dim2; ++j)
              {
                yArrays[i][j] = BitConverter.ToSingle(data, ptr);
                ptr += sizeof(Single);
              }
            }
          }
          break;
        default:
          throw new NotImplementedException();
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
      ZValues = new List<double[]>(yArrays);

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
        if (previousGraph.GraphType == GraphClassType.TimeSeries && previousGraph.ZValues.Count == 1 && previousGraph.ZValues[0].Length == yArrays.Length)
        {
          ZMetaData = new MetaData
          {
            ZUnitShortcut = previousGraph.ZUnitShortcut,
            ZUnitDescription = previousGraph.KindText,
            ZValues = previousGraph.ZValues[0],
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
