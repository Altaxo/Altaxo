#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

using Altaxo.Collections;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Utility class to retrieve the calibration model stored in an worksheet and export it
  /// either to a <see cref="PLS2CalibrationModel" /> or to a XML file.
  /// </summary>
  class PredictionModelExporter
  {
      
    Altaxo.Data.DataTable _table;
    MultivariateContentMemento _memento;
    // IMultivariateCalibrationModel _calibrationModel;
    System.Xml.XmlWriter _writer;

    int _numberOfFactors;
    int _numberOfX;
    int _numberOfY;


    

    public PredictionModelExporter(Altaxo.Data.DataTable table, int numberOfFactors)
    {
      _table = table;
      _memento = table.GetTableProperty("Content") as MultivariateContentMemento;
      _numberOfFactors = numberOfFactors;
    }

    

    public void Export(string filename)
    {
      _writer = new System.Xml.XmlTextWriter(filename,System.Text.Encoding.UTF8);
      _writer.WriteStartDocument();
      _writer.WriteStartElement("LinearPredictionModel");

      WriteProperties();
      WriteSpectralPreprocessing();
      WriteLinearPredictionData();

      _writer.WriteEndElement(); // PLSCalibrationModel
      _writer.WriteEndDocument();

      _writer.Close();
    }

    void WriteProperties()
    {
      _writer.WriteStartElement("Properties");


      _numberOfX = GetNumberOfX(_table);
      _numberOfY = GetNumberOfY(_table);
      _numberOfFactors = Math.Min(_numberOfFactors,_memento.NumberOfFactors);

      _writer.WriteElementString("NumberOfX",System.Xml.XmlConvert.ToString(_numberOfX));
      _writer.WriteElementString("NumberOfY",System.Xml.XmlConvert.ToString(_numberOfY));
      _writer.WriteElementString("NumberOfFactors",System.Xml.XmlConvert.ToString(_numberOfFactors));

      _writer.WriteEndElement(); // Properties
    }


    int GetNumberOfX(Altaxo.Data.DataTable table)
    {
     
      return _memento.NumberOfSpectralData;
    }

    int GetNumberOfY(Altaxo.Data.DataTable table)
    {
      return _memento.NumberOfConcentrationData;
    }
  

    void WriteSpectralPreprocessing()
    {
      _writer.WriteStartElement("SpectralPreprocessing");


      _memento.SpectralPreprocessing.Export(_writer);
    
          
      _writer.WriteEndElement(); 
    }


    void WriteLinearPredictionData()
    {
      _writer.WriteStartElement("Data");

      WriteBasicXData(true);
      WriteBasicYData(true);
      WritePredictionScores();

      _writer.WriteEndElement(); // Data
    }

    void WriteBasicXData(bool bWriteEndElement)
    {
      Altaxo.Data.DoubleColumn col=null;
      string colname;

      _writer.WriteStartElement("XData");

      colname = WorksheetAnalysis.GetXOfX_ColumnName();
      col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
      if(null==col) NotFound(colname);
      WriteVector("XOfX",col, _numberOfX);

      colname = WorksheetAnalysis.GetXMean_ColumnName();
      col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
      if(null==col) NotFound(colname);
      WriteVector("XMean",col, _numberOfX);

      colname = WorksheetAnalysis.GetXScale_ColumnName();
      col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
      if(null==col) NotFound(colname);
      WriteVector("XScale",col, _numberOfX);

      if(bWriteEndElement)
        _writer.WriteEndElement(); // XData
    }

   

    void WriteBasicYData(bool bWriteEndElement)
    {
      Altaxo.Data.DoubleColumn col=null;
      string colname;

      _writer.WriteStartElement("YData");

      colname = WorksheetAnalysis.GetYMean_ColumnName();
      col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
      if(null==col) NotFound(colname);
      WriteVector("YMean",col, _numberOfY);

      colname = WorksheetAnalysis.GetYScale_ColumnName();
      col = _table.DataColumns[colname] as Altaxo.Data.DoubleColumn;
      if(null==col) NotFound(colname);
      WriteVector("YScale",col, _numberOfY);

      if(bWriteEndElement)
        _writer.WriteEndElement(); // YData
    }


   

    void WritePredictionScores()
    {
        
      _writer.WriteStartElement("PredictionScores");
  
      IROMatrix predictionScores = _memento.Analysis.CalculatePredictionScores(
        _table,
        this._numberOfFactors);

        

      for(int i=0;i<_numberOfY;i++)
      {
        WriteVector("Score"+i.ToString(),MatrixMath.ColumnToROVector(predictionScores,i),_numberOfX);
      }
      _writer.WriteEndElement();
    }


    void WriteVector(string name, Altaxo.Data.DoubleColumn col, int numberOfData)
    {
      _writer.WriteStartElement(name);

      for(int i=0;i<numberOfData;i++)
      {
        _writer.WriteElementString("e",System.Xml.XmlConvert.ToString(col[i]));
      }


      _writer.WriteEndElement(); // name
    }

    void WriteVector(string name, IROVector col, int numberOfData)
    {
      _writer.WriteStartElement(name);

      for(int i=0;i<numberOfData;i++)
      {
        _writer.WriteElementString("e",System.Xml.XmlConvert.ToString(col[i]));
      }


      _writer.WriteEndElement(); // name
    }

    static void NotFound(string name)
    {
      throw new ArgumentException("Column " + name + " not found in the table.");
    }

  }

}
