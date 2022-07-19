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

using System;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Contains the data used in <see cref="DimensionReductionAndRegressionPredictionDataSource"/> to predict data using a prediction model, and some spectral data.
  /// </summary>
  public class DimensionReductionAndRegressionPredictionProcessData : Altaxo.Main.SuspendableDocumentLeafNodeWithEventArgs, ICloneable
  {
    /// <summary>
    /// Proxy to the table containing the model. This table should contain a <see cref="DimensionReductionAndRegressionDataSource"/>.
    /// </summary>
    public DataTableProxy TableWithModel { get; set; }

    /// <summary>
    /// Gets the data to predict.
    /// </summary>
    public DataTableMatrixProxyWithMultipleColumnHeaderColumns DataToPredict { get; set; }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-07-19 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionAndRegressionPredictionProcessData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionAndRegressionPredictionProcessData)obj;

        info.AddValue("ModelTable", s.TableWithModel);
        info.AddValue("DataToPredict", s.DataToPredict);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var modelProxy = (DataTableProxy)info.GetValue("ModelTable", parent);
        var dataToPredict = (DataTableMatrixProxyWithMultipleColumnHeaderColumns)info.GetValue("DataToPredict", parent);
        return new DimensionReductionAndRegressionPredictionProcessData(modelProxy, dataToPredict);
      }
    }
    #endregion

    #endregion  

    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionAndRegressionPredictionProcessData"/> class.
    /// </summary>
    /// <param name="tableWithModel">The table with model.</param>
    /// <param name="dataToPredict">The data to predict.</param>
    public DimensionReductionAndRegressionPredictionProcessData(DataTableProxy tableWithModel, DataTableMatrixProxyWithMultipleColumnHeaderColumns dataToPredict)
    {
      TableWithModel = tableWithModel;
      DataToPredict = dataToPredict;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DimensionReductionAndRegressionPredictionProcessData(
        (DataTableProxy)TableWithModel.Clone(),
        (DataTableMatrixProxyWithMultipleColumnHeaderColumns)DataToPredict.Clone()
      );
    }
  }
}
