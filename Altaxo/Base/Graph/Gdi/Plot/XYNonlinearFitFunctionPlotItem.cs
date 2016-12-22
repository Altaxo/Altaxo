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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Gdi.Plot
{
	using Altaxo.Calc.Regression.Nonlinear;
	using Altaxo.Data;
	using Data;
	using Graph.Plot.Data;
	using Styles;

	public class XYNonlinearFitFunctionPlotItem : XYFunctionPlotItem
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
				info.AddValue("FitDocumentIdentifier", s._fitDocumentIdentifier);
				info.AddValue("FitDocument", s._fitDocument);
				info.AddValue("FitElementIndex", s._fitElementIndex);
				info.AddValue("DependentVariableIndex", s._dependentVariableIndex);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (XYNonlinearFitFunctionPlotItem)o ?? new XYNonlinearFitFunctionPlotItem(info);

				s.Data = (XYFunctionPlotData)info.GetValue("Data", null);
				s.Style = (G2DPlotStyleCollection)info.GetValue("Style", null);
				s._fitDocumentIdentifier = info.GetString("FitDocumentIdentifier");
				s.ChildSetMember(ref s._fitDocument, (NonlinearFitDocument)info.GetValue("FitDocument", s));
				s._fitElementIndex = info.GetInt32("FitElementIndex");
				s._dependentVariableIndex = info.GetInt32("DependentVariableIndex");

				return s;
			}
		}

		#endregion Serialization

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var copied = base.CopyFrom(obj);
			if (copied && obj is XYNonlinearFitFunctionPlotItem from)
			{
				this._fitDocumentIdentifier = from._fitDocumentIdentifier;
				ChildCopyToMember(ref this._fitDocument, from._fitDocument);
				this._fitElementIndex = from._fitElementIndex;
				this._dependentVariableIndex = from._dependentVariableIndex;
			}
			return copied;
		}

		private System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetLocalDocumentNodeChildrenWithName()
		{
			if (null != _fitDocument)
				yield return new Main.DocumentNodeAndName(_fitDocument, () => _fitDocument = null, "FitDocument");
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			return GetLocalDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info">The information.</param>
		protected XYNonlinearFitFunctionPlotItem(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XYNonlinearFitFunctionPlotItem"/> class.
		/// </summary>
		/// <param name="fitDocumentIdentifier">The fit document identifier.</param>
		/// <param name="fitDocument">The fit document. The document will be cloned before stored in this instance.</param>
		/// <param name="fitElementIndex">Index of the fit element.</param>
		/// <param name="dependentVariableIndex">Index of the dependent variable of the fit element.</param>
		/// <param name="ps">The ps.</param>
		public XYNonlinearFitFunctionPlotItem(string fitDocumentIdentifier, NonlinearFitDocument fitDocument, int fitElementIndex, int dependentVariableIndex, G2DPlotStyleCollection ps)
			: base()
		{
			ChildCloneToMember(ref _fitDocument, fitDocument); // clone here, because we want to have a local copy which can not change.
			_fitDocumentIdentifier = fitDocumentIdentifier;
			_fitElementIndex = fitElementIndex;
			_dependentVariableIndex = dependentVariableIndex;
			ChildSetMember(ref _plotData, new XYFunctionPlotData(new FitFunctionToScalarFunctionDDWrapper(_fitDocument.FitEnsemble[fitElementIndex].FitFunction, dependentVariableIndex, _fitDocument.GetParametersForFitElement(fitElementIndex))));
			Style = ps;
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

		public override XYFunctionPlotData Data
		{
			get { return _plotData; }
			set
			{
				throw new InvalidOperationException("Data is read-only");
			}
		}
	}
}