#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Drawing;

namespace Altaxo.Graph.Plot.Data
{
  using Altaxo.Calc.Regression.Nonlinear;
  using Altaxo.Data;
  using Altaxo.Main;
  using Gdi.Plot.Data;

  /// <summary>
  /// Summary description for XYFunctionPlotData.
  /// </summary>
  [Serializable]
  public class XYNonlinearFitFunctionPlotData : XYFunctionPlotData
  {
    /// <summary>
    /// A Guid string that is identical for all fit function elements with the same fit document.
    /// </summary>
    protected string _fitDocumentIdentifier;

    /// <summary>The nonlinear fit this function belongs to.</summary>
    private NonlinearFitDocument _fitDocument;

    /// <summary>Index of the fit element this function belongs to.</summary>
    private int _fitElementIndex;

    /// <summary>
    /// Index of the the dependent variable of the fit element that is shown in this plot item.
    /// </summary>
    private int _dependentVariableIndex;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYNonlinearFitFunctionPlotData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYNonlinearFitFunctionPlotData)obj;

        info.AddValue("Function", s._function);
        info.AddValue("FitDocumentIdentifier", s._fitDocumentIdentifier);
        info.AddValue("FitDocument", s._fitDocument);
        info.AddValue("FitElementIndex", s._fitElementIndex);
        info.AddValue("DependentVariableIndex", s._dependentVariableIndex);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (XYNonlinearFitFunctionPlotData)o : new XYNonlinearFitFunctionPlotData();

        s.Function = (Altaxo.Calc.IScalarFunctionDD)info.GetValue("Function", s);
        s._fitDocumentIdentifier = info.GetString("FitDocumentIdentifier");
        s.ChildSetMember(ref s._fitDocument, (NonlinearFitDocument)info.GetValue("FitDocument", s));
        s._fitElementIndex = info.GetInt32("FitElementIndex");
        s._dependentVariableIndex = info.GetInt32("DependentVariableIndex");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    protected XYNonlinearFitFunctionPlotData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XYNonlinearFitFunctionPlotData"/> class.
    /// </summary>
    /// <param name="fitDocumentIdentifier">The fit document identifier.</param>
    /// <param name="fitDocument">The fit document. The document will be cloned before stored in this instance.</param>
    /// <param name="fitElementIndex">Index of the fit element.</param>
    /// <param name="dependentVariableIndex">Index of the dependent variable of the fit element.</param>
    /// <param name="dependentVariableTransformation">Transformation, which is applied to the result of the fit function to be then shown in the plot. Can be null.</param>
    /// <param name="independentVariableIndex">Index of the independent variable of the fit element.</param>
    /// <param name="independentVariableTransformation">Transformation, which is applied to the x value before it is applied to the fit function. Can be null.</param>
    public XYNonlinearFitFunctionPlotData(string fitDocumentIdentifier, NonlinearFitDocument fitDocument, int fitElementIndex, int dependentVariableIndex, IVariantToVariantTransformation dependentVariableTransformation, int independentVariableIndex, IVariantToVariantTransformation independentVariableTransformation)
    {
      if (null == fitDocumentIdentifier)
        throw new ArgumentNullException(nameof(fitDocumentIdentifier));
      if (null == fitDocument)
        throw new ArgumentNullException(nameof(fitDocument));

      ChildCloneToMember(ref _fitDocument, fitDocument); // clone here, because we want to have a local copy which can not change.
      _fitDocumentIdentifier = fitDocumentIdentifier;
      _fitElementIndex = fitElementIndex;
      _dependentVariableIndex = dependentVariableIndex;
      Function = new FitFunctionToScalarFunctionDDWrapper(_fitDocument.FitEnsemble[fitElementIndex].FitFunction, dependentVariableIndex, dependentVariableTransformation, independentVariableIndex, independentVariableTransformation, _fitDocument.GetParametersForFitElement(fitElementIndex));
    }

    public XYNonlinearFitFunctionPlotData(XYNonlinearFitFunctionPlotData from)
      : base(from)
    {
    }

    public override object Clone()
    {
      return new XYNonlinearFitFunctionPlotData(this);
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var copied = base.CopyFrom(obj);
      if (copied && obj is XYNonlinearFitFunctionPlotData from)
      {
        _fitDocumentIdentifier = from._fitDocumentIdentifier;
        ChildCopyToMember(ref _fitDocument, from._fitDocument);
        _fitElementIndex = from._fitElementIndex;
        _dependentVariableIndex = from._dependentVariableIndex;
      }
      return copied;
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _function && Function is Main.IDocumentLeafNode)
        yield return new Main.DocumentNodeAndName((Main.IDocumentLeafNode)_function, "Function");

      if (null != _fitDocument)
        yield return new Main.DocumentNodeAndName(_fitDocument, () => _fitDocument = null, "FitDocument");
    }



    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public override void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      if (_fitDocument is { } fdoc)
        fdoc.VisitDocumentReferences(Report);
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
        return (NonlinearFitDocument)_fitDocument?.Clone();
      }
    }

    /// <summary>
    /// A Guid string that is identical for all fit function elements with the same fit document.
    /// </summary>
    public string FitDocumentIdentifier
    {
      get
      {
        return _fitDocumentIdentifier;
      }
    }

    /// <summary>Index of the fit element of the <see cref="FitDocumentCopy"/> this function belongs to.</summary>
    public int FitElementIndex { get { return _fitElementIndex; } }

    /// <summary>
    /// Index of the the dependent variable of the fit element that is shown in this plot item.
    /// </summary>
    public int DependentVariableIndex
    {
      get { return _dependentVariableIndex; }
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
        var fitEle = _fitDocument.FitEnsemble[FitElementIndex];
        return fitEle.DependentVariables(DependentVariableIndex);
      }
    }
  }
}
