#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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


namespace Altaxo.Calc.Regression.PLS
{
	/// <summary>
  /// This class is for remembering the content of the PLS calibration and where to found the original data.
  /// </summary>
  public class PLSContentMemento
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


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PLSContentMemento),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo  info)
      {
        PLSContentMemento s = (PLSContentMemento)obj;
        info.AddValue("TableName",s.TableName); // name of the Table
        info.AddValue("SpectrumIsRow",s.SpectrumIsRow);
        info.AddValue("SpectralIndices",s.SpectralIndices);
        info.AddValue("ConcentrationIndices",s.ConcentrationIndices);
        info.AddValue("MeasurementIndices",s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo  info, object parent)
      {
        PLSContentMemento s = null!=o ? (PLSContentMemento)o : new PLSContentMemento();

        s.TableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection)info.GetValue("SpectralIndices",s);
        s.ConcentrationIndices = (IAscendingIntegerCollection)info.GetValue("ConcentrationIndices",s);
        s.MeasurementIndices = (IAscendingIntegerCollection)info.GetValue("MeasurementIndices",s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");

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
    /// Get/sets the number of factors used  for calculation residuals, plotting etc.
    /// </summary>
    public int PreferredNumberOfFactors
    {
      get { return _PreferredNumberOfFactors; }
      set { _PreferredNumberOfFactors = value; }
    }
  }


}
