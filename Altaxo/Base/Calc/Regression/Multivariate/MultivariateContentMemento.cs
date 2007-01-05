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

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// This class is for remembering the content of the PLS calibration and where to found the original data.
  /// </summary>
  public class MultivariateContentMemento
  {
    /// <summary>Represents that indices that build up one spectrum.</summary>
    public  Altaxo.Collections.IAscendingIntegerCollection SpectralIndices;

    /// <summary>
    /// Represents the indices of the measurements.
    /// </summary>
    public Altaxo.Collections.IAscendingIntegerCollection MeasurementIndices;

    /// <summary>
    /// Represents the indices of the concentrations.
    /// </summary>
    public Altaxo.Collections.IAscendingIntegerCollection ConcentrationIndices;

    /// <summary>
    /// True if the spectrum is horizontal oriented, i.e. is in one row. False if the spectrum is one column.
    /// </summary>
    public bool SpectrumIsRow;
    
    /// <summary>
    /// Get/sets the name of the table containing the original data.
    /// </summary>
    public string TableName;

    /// <summary>
    /// Number of factors for calculation and plotting.
    /// </summary>
    int _PreferredNumberOfFactors;

    /// <summary>
    /// Number of factors calculated.
    /// </summary>
    int _CalculatedNumberOfFactors;

    /// <summary>
    /// Mean number of observations included in Cross PRESS calculation (used to calculate F-Ratio).
    /// </summary>
    double _MeanNumberOfMeasurementsInCrossPRESSCalculation;

    /// <summary>
    /// Denotes how the cross validation is made (the exact method how the spectra are grouped and mutually excluded).
    /// </summary>
    CrossPRESSCalculationType _crossPRESSCalculationType;

    /// <summary>
    /// The name of the class used to analyse the data.
    /// </summary>
    string _ClassNameOfAnalysisClass;

    /// <summary>
    /// The instance of the class used to analyse the data.
    /// </summary>
    WorksheetAnalysis _InstanceOfAnalysisClass;


    /// <summary>
    /// What to do with the spectra before processing them.
    /// </summary>
    SpectralPreprocessingOptions _spectralPreprocessing = new SpectralPreprocessingOptions();


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Calc.Regression.PLS.PLSContentMemento",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        MultivariateContentMemento s = (MultivariateContentMemento)obj;
        info.AddValue("TableName",s.TableName); // name of the Table
        info.AddValue("SpectrumIsRow",s.SpectrumIsRow);
        info.AddValue("SpectralIndices",s.SpectralIndices);
        info.AddValue("ConcentrationIndices",s.ConcentrationIndices);
        info.AddValue("MeasurementIndices",s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        MultivariateContentMemento s = null!=o ? (MultivariateContentMemento)o : new MultivariateContentMemento();

        s.TableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection)info.GetValue("SpectralIndices",s);
        s.ConcentrationIndices = (IAscendingIntegerCollection)info.GetValue("ConcentrationIndices",s);
        s.MeasurementIndices = (IAscendingIntegerCollection)info.GetValue("MeasurementIndices",s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");
       
        // neccessary since version 2
        s.Analysis = new PLS2WorksheetAnalysis();

        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MultivariateContentMemento),1)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Calc.Regression.PLS.PLSContentMemento",1)]
      class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        MultivariateContentMemento s = (MultivariateContentMemento)obj;
        info.AddValue("TableName",s.TableName); // name of the Table
        info.AddValue("SpectrumIsRow",s.SpectrumIsRow);
        info.AddValue("SpectralIndices",s.SpectralIndices);
        info.AddValue("ConcentrationIndices",s.ConcentrationIndices);
        info.AddValue("MeasurementIndices",s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table

        // new in version 1
        info.AddArray("SpectralPreprocessingRegions",s._spectralPreprocessing.Regions,s._spectralPreprocessing.Regions.Length);
        info.AddEnum("SpectralPreprocessingMethod", s._spectralPreprocessing.Method);
        info.AddValue("SpectralPreprocessingDetrending", s._spectralPreprocessing.DetrendingOrder);
        info.AddValue("SpectralPreprocessingEnsembleScale",s._spectralPreprocessing.EnsembleScale);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        MultivariateContentMemento s = null!=o ? (MultivariateContentMemento)o : new MultivariateContentMemento();

        s.TableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection)info.GetValue("SpectralIndices",s);
        s.ConcentrationIndices = (IAscendingIntegerCollection)info.GetValue("ConcentrationIndices",s);
        s.MeasurementIndices = (IAscendingIntegerCollection)info.GetValue("MeasurementIndices",s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");

        // new in version 1
        if(info.CurrentElementName=="SpectralPreprocessingRegions")
        {
          int[] regions;
          info.GetArray("SpectralPreprocessingRegions", out regions);
          s._spectralPreprocessing.Regions = regions;
          s._spectralPreprocessing.Method  = (SpectralPreprocessingMethod)info.GetEnum("SpectralPreprocessingMethod",typeof(SpectralPreprocessingMethod));
          s._spectralPreprocessing.DetrendingOrder = info.GetInt32("SpectralPreprocessingDetrending");
          s._spectralPreprocessing.EnsembleScale = info.GetBoolean("SpectralPreprocessingEnsembleScale");
        }

        // neccessary since version 2
        s.Analysis = new PLS2WorksheetAnalysis();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MultivariateContentMemento),2)]
      class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        MultivariateContentMemento s = (MultivariateContentMemento)obj;
        info.AddValue("TableName",s.TableName); // name of the Table
        info.AddValue("SpectrumIsRow",s.SpectrumIsRow);
        info.AddValue("SpectralIndices",s.SpectralIndices);
        info.AddValue("ConcentrationIndices",s.ConcentrationIndices);
        info.AddValue("MeasurementIndices",s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table

        // new in version 1
        info.AddArray("SpectralPreprocessingRegions",s._spectralPreprocessing.Regions,s._spectralPreprocessing.Regions.Length);
        info.AddEnum("SpectralPreprocessingMethod", s._spectralPreprocessing.Method);
        info.AddValue("SpectralPreprocessingDetrending", s._spectralPreprocessing.DetrendingOrder);
        info.AddValue("SpectralPreprocessingEnsembleScale",s._spectralPreprocessing.EnsembleScale);

        // new in version 2
        info.AddValue("ClassNameOfAnalysisClass",s._ClassNameOfAnalysisClass);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        MultivariateContentMemento s = null!=o ? (MultivariateContentMemento)o : new MultivariateContentMemento();

        s.TableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection)info.GetValue("SpectralIndices",s);
        s.ConcentrationIndices = (IAscendingIntegerCollection)info.GetValue("ConcentrationIndices",s);
        s.MeasurementIndices = (IAscendingIntegerCollection)info.GetValue("MeasurementIndices",s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");

        // new in version 1
        int[] regions;
        info.GetArray("SpectralPreprocessingRegions", out regions);
        s._spectralPreprocessing.Regions = regions;
        s._spectralPreprocessing.Method  = (SpectralPreprocessingMethod)info.GetEnum("SpectralPreprocessingMethod",typeof(SpectralPreprocessingMethod));
        s._spectralPreprocessing.DetrendingOrder = info.GetInt32("SpectralPreprocessingDetrending");
        s._spectralPreprocessing.EnsembleScale = info.GetBoolean("SpectralPreprocessingEnsembleScale");

        // new in version 2
        s._ClassNameOfAnalysisClass = info.GetString("ClassNameOfAnalysisClass");

        return s;
      }
    }
    #endregion

    /// <summary>
    /// Gets the number of measurement = number of spectra
    /// </summary>
    public int NumberOfMeasurements
    {
      get { return MeasurementIndices.Count; }
    }

    /// <summary>
    /// Gets the number of spectral data per specta, i.e. number of wavelengths, frequencies etc.
    /// </summary>
    public int NumberOfSpectralData
    {
      get { return SpectralIndices.Count; }
    }

    /// <summary>
    /// Gets the number of concentration data, i.e. number of output variables.
    /// </summary>
    public int NumberOfConcentrationData
    {
      get { return ConcentrationIndices.Count; }
    }

    /// <summary>
    /// Get/sets the number of factors that were calculated during the analysis.
    /// </summary>
    public int NumberOfFactors
    {
      get { return _CalculatedNumberOfFactors; }
      set { _CalculatedNumberOfFactors = value; }
    }

    /// <summary>
    /// Get/sets the number of factors used  for calculation residuals, plotting etc.
    /// </summary>
    public int PreferredNumberOfFactors
    {
      get { return _PreferredNumberOfFactors; }
      set { _PreferredNumberOfFactors = value; }
    }

    /// <summary>
    /// Mean number of observations included in cross PRESS calculation (used to calculate F-Ratio).
    /// </summary>
    public double MeanNumberOfMeasurementsInCrossPRESSCalculation
    {
      get { return _MeanNumberOfMeasurementsInCrossPRESSCalculation; }
      set { _MeanNumberOfMeasurementsInCrossPRESSCalculation = value; }
    }


    public CrossPRESSCalculationType CrossValidationType
    {
      get { return this._crossPRESSCalculationType; }
      set { this._crossPRESSCalculationType = value; }
    }

    /// <summary>
    /// What to do with the spectra before processing them.
    /// </summary>
    public SpectralPreprocessingOptions SpectralPreprocessing
    {
      get { return _spectralPreprocessing; }
      set { _spectralPreprocessing = value; }
    }

    /// <summary>
    /// Returns the instance of analysis used to analyse the data
    /// </summary>
    public WorksheetAnalysis Analysis
    {
      get
      {
        if(_InstanceOfAnalysisClass!=null)
          return _InstanceOfAnalysisClass;
        else if(_ClassNameOfAnalysisClass!=null)
        {
          System.Type clstype = System.Type.GetType(_ClassNameOfAnalysisClass);
          if(clstype==null)
            throw new ApplicationException("Can not found the class used to analyse the data, the class type is: " + _ClassNameOfAnalysisClass);
      
          object instance = System.Activator.CreateInstance(clstype);
          if(instance==null)
            throw new ApplicationException("Can not create a instance of the analysis class (no empty constuctor?), class name: " + clstype.ToString());
          if(!(instance is WorksheetAnalysis))
            throw new ApplicationException("The current instance of the analysis class does not inherit from the WorksheetAnalysis class, class name: " + clstype.ToString());

          _InstanceOfAnalysisClass = (WorksheetAnalysis)instance;


          return _InstanceOfAnalysisClass;
        }
        else
          throw new ApplicationException("Neither the name of the analysis class nor an instance was set before, therefore the class used to analyse the data is unknown");
      }
      set
      {
        _InstanceOfAnalysisClass = value;
        _ClassNameOfAnalysisClass = value.GetType().FullName;
      }
    }
  }
}
