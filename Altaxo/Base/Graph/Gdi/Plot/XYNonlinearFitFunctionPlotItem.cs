#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Plot
{
  using Altaxo.Calc.Regression.Nonlinear;
  using Altaxo.Data;
  using Altaxo.Main;
  using Data;
  using Graph.Plot.Data;
  using Styles;

  public class XYNonlinearFitFunctionPlotItem : XYFunctionPlotItem
  {
    #region Serialization

    /// <summary>
    /// 2016-12-22 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYNonlinearFitFunctionPlotItem), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYNonlinearFitFunctionPlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyles);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (XYNonlinearFitFunctionPlotItem)o ?? new XYNonlinearFitFunctionPlotItem(info);

        s.ChildSetMember(ref s._plotData, (XYNonlinearFitFunctionPlotData)info.GetValue("Data", null));
        s.Style = (G2DPlotStyleCollection)info.GetValue("Style", null);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">The information.</param>
    protected XYNonlinearFitFunctionPlotItem(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    public XYNonlinearFitFunctionPlotItem(XYNonlinearFitFunctionPlotItem from)
        : base(from)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XYNonlinearFitFunctionPlotItem"/> class.
    /// </summary>
    /// <param name="fitDocumentIdentifier">The fit document identifier.</param>
    /// <param name="fitDocument">The fit document. The document will be cloned before stored in this instance.</param>
    /// <param name="fitElementIndex">Index of the fit element.</param>
    /// <param name="dependentVariableIndex">Index of the dependent variable of the fit element.</param>
    /// <param name="dependentVariableTransformation">Transformation, which is applied to the result of the fit function to be then shown in the plot. Can be null.</param>
    /// <param name="ps">The ps.</param>
    public XYNonlinearFitFunctionPlotItem(string fitDocumentIdentifier, NonlinearFitDocument fitDocument, int fitElementIndex, int dependentVariableIndex, IVariantToVariantTransformation dependentVariableTransformation, int independentVariableIndex, IVariantToVariantTransformation independentVariableTransformation, G2DPlotStyleCollection ps)
        : base()
    {
      if (null == fitDocumentIdentifier)
        throw new ArgumentNullException(nameof(fitDocumentIdentifier));
      if (null == fitDocument)
        throw new ArgumentNullException(nameof(fitDocument));
      if (null == ps)
        throw new ArgumentNullException(nameof(ps));

      ChildSetMember(ref _plotData, new XYNonlinearFitFunctionPlotData(fitDocumentIdentifier, fitDocument, fitElementIndex, dependentVariableIndex, dependentVariableTransformation, independentVariableIndex, independentVariableTransformation));
      Style = ps;
    }

    public override object Clone()
    {
      return new XYNonlinearFitFunctionPlotItem(this);
    }

    /// <summary>
    /// Gets a copy of the fit document.
    /// </summary>
    /// <value>
    /// The copy of the fit document.
    /// </value>
    public NonlinearFitDocument FitDocumentCopy
    {
      get
      {
        return ((XYNonlinearFitFunctionPlotData)_plotData).FitDocumentCopy;
      }
    }

    /// <summary>
    /// A Guid string that is identical for all fit function elements with the same fit document.
    /// </summary>
    public string FitDocumentIdentifier
    {
      get
      {
        return ((XYNonlinearFitFunctionPlotData)_plotData).FitDocumentIdentifier;
      }
    }

    /// <summary>Index of the fit element of the <see cref="FitDocumentCopy"/> this function belongs to.</summary>
    public int FitElementIndex { get { return ((XYNonlinearFitFunctionPlotData)_plotData).FitElementIndex; } }

    /// <summary>
    /// Index of the the dependent variable of the fit element that is shown in this plot item.
    /// </summary>
    public int DependentVariableIndex
    {
      get { return ((XYNonlinearFitFunctionPlotData)_plotData).DependentVariableIndex; }
    }

    /// <summary>
    /// Gets the dependent variable column.
    /// </summary>
    /// <value>
    /// The dependent variable column.
    /// </value>
    public IReadableColumn DependentVariableColumn
    {
      get
      {
        return ((XYNonlinearFitFunctionPlotData)_plotData).DependentVariableColumn;
      }
    }

    public override IXYFunctionPlotData Data
    {
      get { return _plotData; }
      set
      {
        throw new InvalidOperationException("Data is read-only");
      }
    }

    public override void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      _plotData.VisitDocumentReferences(Report);
      base.VisitDocumentReferences(Report);
    }
  }
}
