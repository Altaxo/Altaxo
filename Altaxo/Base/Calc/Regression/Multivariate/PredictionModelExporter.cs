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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Utility class to retrieve the calibration model stored in an worksheet and export it
  /// either to a <see cref="PLS2CalibrationModel" /> or to a XML file.
  /// </summary>
  internal class PredictionModelExporter
  {
    private Altaxo.Data.DataTable _table;
    private MultivariateContentMemento _memento;
    private int _numberOfFactors;
    private int _numberOfX;
    private int _numberOfY;

    public PredictionModelExporter(Altaxo.Data.DataTable table, int numberOfFactors)
    {
      _table = table ?? throw new ArgumentNullException(nameof(table));
      _memento = (table.GetTableProperty("Content") as MultivariateContentMemento) ?? throw new InvalidOperationException($"Data table {table.Name} does not contain a multivariate model.");
      _numberOfFactors = numberOfFactors;
    }

    public void Export(string filename)
    {
      var writer = new System.Xml.XmlTextWriter(filename, System.Text.Encoding.UTF8);
      writer.WriteStartDocument();
      writer.WriteStartElement("LinearPredictionModel");

      WriteProperties(writer);
      WriteSpectralPreprocessing(writer);
      WriteLinearPredictionData(writer);

      writer.WriteEndElement(); // PLSCalibrationModel
      writer.WriteEndDocument();

      writer.Close();
    }

    private void WriteProperties(System.Xml.XmlWriter writer)
    {
      writer.WriteStartElement("Properties");

      _numberOfX = GetNumberOfX(_table);
      _numberOfY = GetNumberOfY(_table);
      _numberOfFactors = Math.Min(_numberOfFactors, _memento.NumberOfFactors);

      writer.WriteElementString("NumberOfX", System.Xml.XmlConvert.ToString(_numberOfX));
      writer.WriteElementString("NumberOfY", System.Xml.XmlConvert.ToString(_numberOfY));
      writer.WriteElementString("NumberOfFactors", System.Xml.XmlConvert.ToString(_numberOfFactors));

      writer.WriteEndElement(); // Properties
    }

    private int GetNumberOfX(Altaxo.Data.DataTable table)
    {
      return _memento.NumberOfSpectralData;
    }

    private int GetNumberOfY(Altaxo.Data.DataTable table)
    {
      return _memento.NumberOfConcentrationData;
    }

    private void WriteSpectralPreprocessing(System.Xml.XmlWriter writer)
    {
      writer.WriteStartElement("SpectralPreprocessing");

      _memento.SpectralPreprocessing.Export(writer);

      writer.WriteEndElement();
    }

    private void WriteLinearPredictionData(System.Xml.XmlWriter writer)
    {
      writer.WriteStartElement("Data");

      WriteBasicXData(writer, true);
      WriteBasicYData(writer, true);
      WritePredictionScores(writer);

      writer.WriteEndElement(); // Data
    }

    private void WriteBasicXData(System.Xml.XmlWriter writer, bool bWriteEndElement)
    {
      Altaxo.Data.DoubleColumn? col = null;
      string colname;

      writer.WriteStartElement("XData");

      colname = WorksheetAnalysis.GetXOfX_ColumnName();
      col = _table.DataColumns.TryGetColumn(colname) as Altaxo.Data.DoubleColumn;
      if (col is null)
        NotFound(colname);
      WriteVector(writer, "XOfX", col, _numberOfX);

      colname = WorksheetAnalysis.GetXMean_ColumnName();
      col = _table.DataColumns.TryGetColumn(colname) as Altaxo.Data.DoubleColumn;
      if (col is null)
        NotFound(colname);
      WriteVector(writer, "XMean", col, _numberOfX);

      colname = WorksheetAnalysis.GetXScale_ColumnName();
      col = _table.DataColumns.TryGetColumn(colname) as Altaxo.Data.DoubleColumn;
      if (col is null)
        NotFound(colname);
      WriteVector(writer, "XScale", col, _numberOfX);

      if (bWriteEndElement)
        writer.WriteEndElement(); // XData
    }

    private void WriteBasicYData(System.Xml.XmlWriter writer, bool bWriteEndElement)
    {
      Altaxo.Data.DoubleColumn? col = null;
      string colname;

      writer.WriteStartElement("YData");

      colname = WorksheetAnalysis.GetYMean_ColumnName();
      col = _table.DataColumns.TryGetColumn(colname) as Altaxo.Data.DoubleColumn;
      if (col is null)
        NotFound(colname);
      WriteVector(writer, "YMean", col, _numberOfY);

      colname = WorksheetAnalysis.GetYScale_ColumnName();
      col = _table.DataColumns.TryGetColumn(colname) as Altaxo.Data.DoubleColumn;
      if (col is null)
        NotFound(colname);
      WriteVector(writer, "YScale", col, _numberOfY);

      if (bWriteEndElement)
        writer.WriteEndElement(); // YData
    }

    private void WritePredictionScores(System.Xml.XmlWriter writer)
    {
      writer.WriteStartElement("PredictionScores");

      var predictionScores = _memento.Analysis.CalculatePredictionScores(
        _table,
        _numberOfFactors);

      for (int i = 0; i < _numberOfY; i++)
      {
        WriteVector(writer, "Score" + i.ToString(), MatrixMath.ColumnToROVector(predictionScores, i), _numberOfX);
      }
      writer.WriteEndElement();
    }

    private void WriteVector(System.Xml.XmlWriter writer, string name, Altaxo.Data.DoubleColumn col, int numberOfData)
    {
      writer.WriteStartElement(name);

      for (int i = 0; i < numberOfData; i++)
      {
        writer.WriteElementString("e", System.Xml.XmlConvert.ToString(col[i]));
      }

      writer.WriteEndElement(); // name
    }

    private void WriteVector(System.Xml.XmlWriter writer, string name, IReadOnlyList<double> col, int numberOfData)
    {
      writer.WriteStartElement(name);

      for (int i = 0; i < numberOfData; i++)
      {
        writer.WriteElementString("e", System.Xml.XmlConvert.ToString(col[i]));
      }

      writer.WriteEndElement(); // name
    }

    [DoesNotReturn]
    private static void NotFound(string name)
    {
      throw new ArgumentException("Column " + name + " not found in the table.");
    }
  }
}
