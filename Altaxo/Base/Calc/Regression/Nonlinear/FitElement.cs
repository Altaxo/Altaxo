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

using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Selections;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Holds the fit function together with the data sources for the independent and
	/// dependent variables.
	/// </summary>
	public class FitElement
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IColumnPlotData,
		ICloneable
	{
		/// <summary>Fitting function. Can be null if no fitting function was actually chosen.</summary>
		private IFitFunction _fitFunction;

		/// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
		protected DataTableProxy _dataTable;

		/// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
		protected int _groupNumber;

		/// <summary>Holds the range of rows of the data source that are used for the fitting procedure.</summary>
		private IRowSelection _rangeOfRows;

		/// <summary>Array of columns that are used as data source for the independent variables.</summary>
		private IReadableColumnProxy[] _independentVariables;

		/// <summary>Array of columns that are used as data source for the dependent variables.</summary>
		private IReadableColumnProxy[] _dependentVariables;

		/// <summary>Holds for each dependent variable the kind of error evaluation (i.e. the kind of weighing of the difference between current dependent values and the calculated value of the fitting function)</summary>
		private IVarianceScaling[] _errorEvaluation;

		/// <summary>Array of the current parameter names. The length of this array should be equal to that of <see cref="P:IFitFunction.NumberOfParameters"/>. If an element of this array is null, the parameter name
		/// of the fit function is used. Otherwise, this value overrides the original parameter name of the fit function.</summary>
		private string[] _parameterNames = new string[0];

		/// <summary></summary>
		private string _parameterNameStart = string.Empty;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.FitElement", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version now allowed");
				/*
				FitElement s = (FitElement)obj;

				s.InternalCheckAndCorrectArraySize(true, false); // make sure the fit function has not changed unnoticed

				info.AddValue("FitFunction", s._fitFunction);
				info.AddValue("NumberOfRows", s._rangeOfRows.Count);
				info.AddValue("FirstRow", s._rangeOfRows.Start);

				info.AddArray("IndependentVariables", s._independentVariables, s._independentVariables.Length);
				info.AddArray("DependentVariables", s._dependentVariables, s._dependentVariables.Length);
				info.AddArray("VarianceEvaluation", s._errorEvaluation, s._errorEvaluation.Length);
				info.AddArray("ParameterNames", s._parameterNames, s._parameterNames.Length);
				info.AddValue("ParameterNameStart", s._parameterNameStart);
				*/
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				FitElement s = o != null ? (FitElement)o : new FitElement();

				s.ChildSetMemberAlt(ref s._fitFunction, (IFitFunction)info.GetValue("FitFunction", s));

				int numRows = info.GetInt32("NumberOfRows");
				int firstRow = info.GetInt32("FirstRow");
				s._rangeOfRows = RangeOfRowIndices.FromStartAndCount(firstRow, numRows);
				if (null != s._rangeOfRows) s._rangeOfRows.ParentObject = s;

				int arraycount = info.OpenArray();
				s._independentVariables = new IReadableColumnProxy[arraycount];
				for (int i = 0; i < arraycount; ++i)
				{
					s._independentVariables[i] = (IReadableColumnProxy)info.GetValue("e", s);
					if (null != s._independentVariables[i]) s._independentVariables[i].ParentObject = s;
				}
				info.CloseArray(arraycount);

				arraycount = info.OpenArray();
				s._dependentVariables = new IReadableColumnProxy[arraycount];
				for (int i = 0; i < arraycount; ++i)
				{
					s._dependentVariables[i] = (IReadableColumnProxy)info.GetValue("e", s);
					if (null != s._dependentVariables[i]) s._dependentVariables[i].ParentObject = s;
				}
				info.CloseArray(arraycount);

				arraycount = info.OpenArray();
				s._errorEvaluation = new IVarianceScaling[arraycount];
				for (int i = 0; i < arraycount; ++i)
					s._errorEvaluation[i] = (IVarianceScaling)info.GetValue("e", s);
				info.CloseArray(arraycount);

				info.GetArray("ParameterNames", out s._parameterNames);
				for (int i = 0; i < s._parameterNames.Length; ++i)
					if (s._parameterNames[i] == string.Empty)
						s._parameterNames[i] = null; // serialization can not distinguish between an empty string and a null string

				s._parameterNameStart = info.GetString("ParameterNameStart");

				// now some afterwork
				if (s.InternalCheckAndCorrectArraySize(false, false))
					Current.Console.WriteLine("Error: Fitelement array size mismatch");

				return s;
			}
		}

		/// <summary>
		/// 2016-10-05 Added DataTable, GroupNumber. Changed: NunberOfRows, FirstRow now is replaced by RowSelection
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitElement), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				FitElement s = (FitElement)obj;

				s.InternalCheckAndCorrectArraySize(true, false); // make sure the fit function has not changed unnoticed

				info.AddValue("FitFunction", s._fitFunction);

				info.AddValue("DataTable", s._dataTable);
				info.AddValue("GroupNumber", s._groupNumber);
				info.AddValue("RowSelection", s._rangeOfRows);

				info.AddArray("IndependentVariables", s._independentVariables, s._independentVariables.Length);
				info.AddArray("DependentVariables", s._dependentVariables, s._dependentVariables.Length);
				info.AddArray("VarianceEvaluation", s._errorEvaluation, s._errorEvaluation.Length);
				info.AddArray("ParameterNames", s._parameterNames, s._parameterNames.Length);
				info.AddValue("ParameterNameStart", s._parameterNameStart);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				FitElement s = o != null ? (FitElement)o : new FitElement();

				s.ChildSetMemberAlt(ref s._fitFunction, (IFitFunction)info.GetValue("FitFunction", s));

				s._dataTable = (DataTableProxy)info.GetValue("DataTable", s);
				if (null != s._dataTable) s._dataTable.ParentObject = s;

				s._groupNumber = info.GetInt32("GroupNumber");

				s._rangeOfRows = (IRowSelection)info.GetValue("RowSelection", s);
				if (null != s._rangeOfRows) s._rangeOfRows.ParentObject = s;

				int arraycount = info.OpenArray();
				s._independentVariables = new IReadableColumnProxy[arraycount];
				for (int i = 0; i < arraycount; ++i)
				{
					s._independentVariables[i] = (IReadableColumnProxy)info.GetValue("e", s);
					if (null != s._independentVariables[i]) s._independentVariables[i].ParentObject = s;
				}
				info.CloseArray(arraycount);

				arraycount = info.OpenArray();
				s._dependentVariables = new IReadableColumnProxy[arraycount];
				for (int i = 0; i < arraycount; ++i)
				{
					s._dependentVariables[i] = (IReadableColumnProxy)info.GetValue("e", s);
					if (null != s._dependentVariables[i]) s._dependentVariables[i].ParentObject = s;
				}
				info.CloseArray(arraycount);

				arraycount = info.OpenArray();
				s._errorEvaluation = new IVarianceScaling[arraycount];
				for (int i = 0; i < arraycount; ++i)
					s._errorEvaluation[i] = (IVarianceScaling)info.GetValue("e", s);
				info.CloseArray(arraycount);

				info.GetArray("ParameterNames", out s._parameterNames);
				for (int i = 0; i < s._parameterNames.Length; ++i)
					if (s._parameterNames[i] == string.Empty)
						s._parameterNames[i] = null; // serialization can not distinguish between an empty string and a null string

				s._parameterNameStart = info.GetString("ParameterNameStart");

				// now some afterwork
				if (s.InternalCheckAndCorrectArraySize(false, false))
					Current.Console.WriteLine("Error: Fitelement array size mismatch");

				return s;
			}
		}

		#endregion Serialization

		public FitElement()
		{
			_independentVariables = new IReadableColumnProxy[0];

			_dependentVariables = new IReadableColumnProxy[0];

			_errorEvaluation = new IVarianceScaling[0];

			_rangeOfRows = new AllRows();
		}

		public FitElement(FitElement from)
		{
			this._fitFunction = from._fitFunction;
			if (_fitFunction is ICloneable)
				this._fitFunction = (IFitFunction)((ICloneable)from.FitFunction).Clone();
			if (null != _fitFunction)
				_fitFunction.Changed += EhFitFunctionChanged;

			ChildCopyToMember(ref _dataTable, from._dataTable);
			this._groupNumber = from._groupNumber;
			ChildCloneToMember(ref _rangeOfRows, from._rangeOfRows);

			_independentVariables = new IReadableColumnProxy[from._independentVariables.Length];
			for (int i = 0; i < _independentVariables.Length; ++i)
			{
				if (from._independentVariables[i] != null)
					_independentVariables[i] = (IReadableColumnProxy)from._independentVariables[i].Clone();
			}

			_dependentVariables = new IReadableColumnProxy[from._dependentVariables.Length];
			for (int i = 0; i < _dependentVariables.Length; ++i)
			{
				if (from._dependentVariables[i] != null)
					_dependentVariables[i] = (IReadableColumnProxy)from._dependentVariables[i].Clone();
			}
			_errorEvaluation = new IVarianceScaling[from._errorEvaluation.Length];
			for (int i = 0; i < _errorEvaluation.Length; ++i)
			{
				if (from._errorEvaluation[i] != null)
					_errorEvaluation[i] = (IVarianceScaling)from._errorEvaluation[i].Clone();
			}

			_parameterNames = (string[])from._parameterNames.Clone();
			_parameterNameStart = from._parameterNameStart;
		}

		public FitElement(DataTable table, int groupNumber, IRowSelection rowSelection, IReadableColumn xColumn, IReadableColumn yColumn)
		{
			ChildSetMember(ref _dataTable, new DataTableProxy(table));
			_groupNumber = groupNumber;
			ChildCloneToMember(ref _rangeOfRows, rowSelection);

			_independentVariables = new IReadableColumnProxy[1];
			_independentVariables[0] = ReadableColumnProxyBase.FromColumn(xColumn);

			_dependentVariables = new IReadableColumnProxy[1];
			_dependentVariables[0] = ReadableColumnProxyBase.FromColumn(yColumn);

			_errorEvaluation = new IVarianceScaling[1];
			_errorEvaluation[0] = new ConstantVarianceScaling();
		}

		/// <summary>
		/// Gives the ith parameter name.
		/// </summary>
		/// <param name="i">Index of the parameter whose name is to be retrieved.</param>
		/// <returns>The ith parameter name. Returns null if the <see cref="_fitFunction"/> is <c>null</c>.
		/// Otherwise, if <see cref="_parameterNames"/>[i] is not null, returns this value.
		/// If <see cref="_parameterNames"/>[i] is null then the value of <see cref="_parameterNameStart"/> is concenated with the original parameter name of the fit function and that value is returned.
		/// </returns>
		public string ParameterName(int i)
		{
			if (null != _fitFunction)
			{
				if (null != _parameterNames[i])
					return _parameterNames[i];
				else
					return _parameterNameStart + _fitFunction.ParameterName(i);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Sets the ith parameter name.
		/// </summary>
		/// <param name="value">The new value of the parameter.</param>
		/// <param name="i">Index of the parameter.</param>
		public void SetParameterName(string value, int i)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value), "Parameter name must not be null");
			if (value.Length == 0)
				throw new ArgumentException("Parameter name is empty", nameof(value));

			string oldValue = _parameterNames[i];
			_parameterNames[i] = value;

			if (value != oldValue)
				EhSelfChanged(EventArgs.Empty);
		}

		public DataTable DataTable
		{
			get
			{
				return _dataTable?.Document;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (object.ReferenceEquals(DataTable, value))
					return;

				if (ChildSetMember(ref _dataTable, new DataTableProxy(value)))
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public int GroupNumber
		{
			get
			{
				return _groupNumber;
			}
			set
			{
				if (!(_groupNumber == value))
				{
					_groupNumber = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// The selection of data rows to be plotted.
		/// </summary>
		public IRowSelection DataRowSelection
		{
			get
			{
				return _rangeOfRows;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!_rangeOfRows.Equals(value))
				{
					ChildSetMember(ref _rangeOfRows, value);
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		public IEnumerable<Tuple<string, // Name of the column group, e.g. "X-Y-Data"
		IEnumerable<Tuple<
	string, // Column label
	IReadableColumn, // the column as it was at the time of this call
	string, // the name of the column (last part of the column proxies document path)
	Action<IReadableColumn, DataTable, int> // action to set the column during Apply of the controller
	>>>> GetAdditionallyUsedColumns()
		{
			yield return new Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>>>("Independent variables", GetIndependentVariables());
			yield return new Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>>>("Dependent variables", GetDependentVariables());
		}

		private IEnumerable<Tuple<
	string, // Column label
	IReadableColumn, // the column as it was at the time of this call
	string, // the name of the column (last part of the column proxies document path)
	Action<IReadableColumn, DataTable, int> // action to set the column during Apply of the controller
	>> GetIndependentVariables()
		{
			for (int i = 0; i < NumberOfIndependentVariables; ++i)
			{
				int k = i;

				string nameOfVariable = null != this.FitFunction && i < this.FitFunction.NumberOfIndependentVariables ? this.FitFunction.IndependentVariableName(i) : string.Empty;
				yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>(
					nameOfVariable,
					_independentVariables[k].Document,
					_independentVariables[k]?.DocumentPath?.LastPartOrDefault,
					(col, table, group) => { ChildSetMember(ref _independentVariables[k], ReadableColumnProxyBase.FromColumn(col)); DataTable = table; GroupNumber = group; }
					);
			}
		}

		private IEnumerable<Tuple<
	string, // Column label
	IReadableColumn, // the column as it was at the time of this call
	string, // the name of the column (last part of the column proxies document path)
	Action<IReadableColumn, DataTable, int> // action to set the column during Apply of the controller
	>> GetDependentVariables()
		{
			for (int i = 0; i < NumberOfDependentVariables; ++i)
			{
				int k = i;

				string nameOfVariable = null != this.FitFunction && k < this.FitFunction.NumberOfDependentVariables ? this.FitFunction.DependentVariableName(k) : string.Empty;
				yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>(
					nameOfVariable,
					_dependentVariables[k]?.Document,
					_dependentVariables[k]?.DocumentPath?.LastPartOrDefault,
					(col, table, group) => { ChildSetMember(ref _dependentVariables[k], ReadableColumnProxyBase.FromColumn(col)); DataTable = table; GroupNumber = group; }
					);
			}
		}

		/// <summary>
		/// Gets the maximum row index that can be deduced from the data columns. The calculation does <b>not</b> include the DataRowSelection.
		/// </summary>
		/// <returns>The maximum row index that can be deduced from the data columns.</returns>
		public int GetMaximumRowIndexExclusiveFromDataColumns()
		{
			int maxRowIndex = int.MaxValue;

			foreach (var proxy in _independentVariables.Concat(_dependentVariables))
			{
				var column = proxy?.Document;

				if (null != column && column.Count.HasValue)
					maxRowIndex = Math.Min(maxRowIndex, column.Count.Value);
			}
			// if both columns are indefinite long, we set the length to zero
			if (maxRowIndex == int.MaxValue || maxRowIndex < 0)
				maxRowIndex = 0;

			return maxRowIndex;
		}

		/// <summary>
		/// Returns the ith independent variable column. Can return <c>null</c> if the column was set properly, but was
		/// disposed in the mean time.
		/// </summary>
		/// <param name="i">Index.</param>
		/// <returns>The ith independent variable column, or <c>null if such a column is no longer available.</c></returns>
		public IReadableColumn IndependentVariables(int i)
		{
			return null == this._independentVariables[i] ? null : this._independentVariables[i].Document;
		}

		/// <summary>
		/// Sets the ith independent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
		/// </summary>
		/// <param name="i">Index.</param>
		/// <param name="col">Independent variable column to set.</param>
		public void SetIndependentVariable(int i, IReadableColumn col)
		{
			this._independentVariables[i] = ReadableColumnProxyBase.FromColumn(col);

			EhSelfChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Returns the ith dependent variable column. Can return <c>null</c> if the column was set properly, but was
		/// disposed in the mean time.
		/// </summary>
		/// <param name="i">Index.</param>
		/// <returns>The ith dependent variable column, or <c>null if such a column is no longer available.</c></returns>
		public IReadableColumn DependentVariables(int i)
		{
			return null == this._dependentVariables[i] ? null : this._dependentVariables[i].Document;
		}

		/// <summary>
		/// Sets the ith dependent variable column. The column is hold by a reference aware of disposed events, so that it can be null if retrieved afterwards.
		/// </summary>
		/// <param name="i">Index.</param>
		/// <param name="col">Dependent variable column to set.</param>
		public void SetDependentVariable(int i, IReadableColumn col)
		{
			this._dependentVariables[i] = ReadableColumnProxyBase.FromColumn(col);

			if (col != null)
			{
				if (this._errorEvaluation[i] == null)
					this._errorEvaluation[i] = new ConstantVarianceScaling();
			}
			else
			{
				this._errorEvaluation[i] = null;
			}
		}

		/// <summary>
		/// Gets the kind of error evaluation for the ith dependent variable.
		/// </summary>
		/// <param name="i">The index of the dependent variable.</param>
		/// <returns>Kind of error evaluation for the ith dependent variable.</returns>
		public IVarianceScaling GetErrorEvaluation(int i)
		{
			return this._errorEvaluation[i];
		}

		/// <summary>
		/// Gets the kind of error evaluation for the ith dependent variable.
		/// </summary>
		/// <param name="i">The index of the dependent variable.</param>
		/// <param name="val">Kind of error evaluation for the ith dependent variable.</param>
		public void SetErrorEvaluation(int i, IVarianceScaling val)
		{
			this._errorEvaluation[i] = val;
		}

		/// <summary>
		/// Returns true if the regression procedure has to include weights in the calculation.
		/// Otherwise, if weights are not used for all used dependent variables (ConstantVarianceScaling with Scaling==1), <c>false</c> is returned.
		/// </summary>
		public bool UseWeights
		{
			get
			{
				if (_errorEvaluation == null || _dependentVariables == null)
					return false;

				for (int i = 0; i < _errorEvaluation.Length; ++i)
				{
					if (_dependentVariables[i] != null)
					{
						if (_errorEvaluation[i] != null)
						{
							if (!(_errorEvaluation[i] is ConstantVarianceScaling) || !((ConstantVarianceScaling)_errorEvaluation[i]).IsDefault)
								return true;
						}
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Gets / sets the fitting function.
		/// </summary>
		public IFitFunction FitFunction
		{
			get
			{
				return _fitFunction;
			}
			set
			{
				if (null != _fitFunction)
				{
					_fitFunction.Changed -= EhFitFunctionChanged;
				}

				_fitFunction = value;

				if (null != _fitFunction)
				{
					_fitFunction.Changed += EhFitFunctionChanged;
				}

				InternalCheckAndCorrectArraySize(false, false);
			}
		}

		private void EhFitFunctionChanged(object sender, EventArgs e)
		{
			InternalCheckAndCorrectArraySize(false, true);
		}

		/// <summary>
		/// Checks the size of the arrays (independent variables, dependent variables, parameters) against the corresponding values of the fit function and throws an InvalidOperationException when a mismatch is detected.
		/// The most probably cause of this is when the fit function has changed the number of parameters (or dependent or independent) variables unnoticed by this FitElement.
		/// </summary>
		/// <param name="throwOnMismatch">If <c>true</c>, an InvalidOperationException is thrown if the corresponding number from the fit function and the array size mismatch.</param>
		/// <param name="forceChangedEvent">If <c>true</c>, the <see cref="E:Changed"/> event is fired even if no mismatch was detected.</param>
		/// <returns><c>True</c> if any mismatch occurred, so that the array size has changed. Otherwise, <c>False</c> is returned.</returns>
		private bool InternalCheckAndCorrectArraySize(bool throwOnMismatch, bool forceChangedEvent)
		{
			if (_fitFunction == null)
				return false;

			bool hasMismatch = false;

			if (_fitFunction.NumberOfIndependentVariables != _independentVariables.Length)
			{
				hasMismatch = true;
				if (throwOnMismatch)
					throw new InvalidOperationException("Mismatch between number of independent variables of the fit function and of the array. Probably the fit function was changed after assigning them to the fit element, and dit not fire the Changed event");
				else
					InternalReallocIndependentVariables(_fitFunction.NumberOfIndependentVariables);
			}
			if (_fitFunction.NumberOfDependentVariables != _dependentVariables.Length)
			{
				hasMismatch = true;
				if (throwOnMismatch)
					throw new InvalidOperationException("Mismatch between number of dependent variables of the fit function and of the array. Probably the fit function was changed after assigning them to the fit element, and dit not fire the Changed event");
				else
					InternalReallocDependentVariables(_fitFunction.NumberOfDependentVariables);
			}
			if (_fitFunction.NumberOfParameters != _parameterNames.Length)
			{
				hasMismatch = true;
				if (throwOnMismatch)
					throw new InvalidOperationException("Mismatch between number of parameters of the fit function and of the array. Probably the fit function was changed after assigning them to the fit element, and dit not fire the Changed event");
				else
					InternalReallocParameters(_fitFunction.NumberOfParameters);
			}

			if (hasMismatch | forceChangedEvent)
				EhSelfChanged(EventArgs.Empty);

			return hasMismatch;
		}

		private void InternalReallocIndependentVariables(int noIndep)
		{
			var oldArr = this._independentVariables;
			var newArr = new IReadableColumnProxy[noIndep];
			for (int i = Math.Min(newArr.Length, oldArr.Length) - 1; i >= 0; i--)
				newArr[i] = oldArr[i];

			this._independentVariables = newArr;
		}

		private void InternalReallocDependentVariables(int noDep)
		{
			{
				var oldArr = this._dependentVariables;
				var newArr = new IReadableColumnProxy[noDep];
				for (int i = Math.Min(newArr.Length, oldArr.Length) - 1; i >= 0; i--)
					newArr[i] = oldArr[i];
				this._dependentVariables = newArr;
			}
			{
				// do the same also with the error scaling

				IVarianceScaling[] oldArr = _errorEvaluation;
				IVarianceScaling[] newArr = new IVarianceScaling[noDep];
				for (int i = Math.Min(newArr.Length, oldArr.Length) - 1; i >= 0; i--)
					newArr[i] = oldArr[i];
				this._errorEvaluation = newArr;
			}
		}

		private void InternalReallocParameters(int noPar)
		{
			this._parameterNames = new string[noPar];
		}

		/// <summary>
		/// Gets the number of independent variables of this fit element.
		/// </summary>
		public int NumberOfIndependentVariables
		{
			get
			{
				return this._fitFunction != null ? _fitFunction.NumberOfIndependentVariables : 0;
			}
		}

		/// <summary>
		/// Gets the maximum possible number of dependent variables of this fit element. Please note that the actual used number of dependent variables can be less than this (see <see cref="NumberOfUsedDependentVariables"/>).
		/// </summary>
		public int NumberOfDependentVariables
		{
			get
			{
				return this._fitFunction != null ? _fitFunction.NumberOfDependentVariables : 0;
			}
		}

		/// <summary>
		/// Returns the number of dependent variables that are currently used for fitting.
		/// </summary>
		public int NumberOfUsedDependentVariables
		{
			get
			{
				int sum = 0;
				if (null != _dependentVariables)
				{
					int len = _dependentVariables.Length;
					if (null != _fitFunction)
						len = Math.Min(len, _fitFunction.NumberOfDependentVariables);

					for (int i = len - 1; i >= 0; --i)
						if (_dependentVariables[i] != null && _dependentVariables[i].Document != null)
							sum++;
				}
				return sum;
			}
		}

		/// <summary>
		/// Gets the number of parameters.
		/// </summary>
		public int NumberOfParameters
		{
			get
			{
				return this._fitFunction != null ? _fitFunction.NumberOfParameters : 0;
			}
		}

		/// <summary>
		/// Calculates the valid numeric rows of the data source, i.e. that rows that can be used for fitting. Both dependent and independent variable sources are considered. A row is considered valid, if
		/// all (independent and dependent) variables of this row have finite numeric values.
		/// </summary>
		/// <returns>The set of rows that can be used for fitting.</returns>
		public IAscendingIntegerCollection CalculateValidNumericRows()
		{
			// also obtain the valid rows both of the independent and of the dependent variables
			var cols = new IReadableColumn[_independentVariables.Length + _dependentVariables.Length];
			int i;
			AscendingIntegerCollection selectedCols = new AscendingIntegerCollection();
			// note: for a fitting session all independent variables columns must
			// be not null
			int maxLength = int.MaxValue;
			for (i = 0; i < _independentVariables.Length; i++)
			{
				cols[i] = _independentVariables[i].Document;
				selectedCols.Add(i);
				if (cols[i].Count.HasValue)
					maxLength = Math.Min(maxLength, cols[i].Count.Value);
			}

			// note: for a fitting session some of the dependent variables can be null
			for (int j = 0; j < _dependentVariables.Length; ++j, ++i)
			{
				if (_dependentVariables[j] != null && _dependentVariables[j].Document != null)
				{
					cols[i] = _dependentVariables[j].Document;
					selectedCols.Add(i);
					if (cols[i].Count.HasValue)
						maxLength = Math.Min(maxLength, cols[i].Count.Value);
				}
			}
			if (maxLength == int.MaxValue)
				maxLength = 0; // in case non of the columns has a defined length

			bool[] arr = Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetValidNumericRows(cols, selectedCols, _rangeOfRows.GetSelectedRowIndicesFromTo(0, maxLength, _dataTable?.Document.DataColumns, maxLength), maxLength);

			return Altaxo.Calc.LinearAlgebra.DataTableWrapper.GetCollectionOfValidNumericRows(arr);
		}

		/// <summary>
		/// Returns the parent table of the fit element.
		/// </summary>
		/// <returns>If all independent variables originate from the same table, the return value is that table. If not, the return value is null.</returns>
		public DataTable GetParentDataTable()
		{
			// now look for the parent table of the fit element
			// the parent table is defined if all independent variables originate from the same table
			Altaxo.Data.DataTable parentTable = null;
			for (int k = 0; k < NumberOfIndependentVariables; k++)
			{
				Altaxo.Data.DataColumn ncol = IndependentVariables(k) as Altaxo.Data.DataColumn;
				if (ncol == null)
					continue;
				Altaxo.Data.DataTable parent = Altaxo.Data.DataTable.GetParentDataTableOf(ncol);
				if (parent != null && parentTable == null)
					parentTable = parent;
				else if (parent != null && parentTable != null && !object.ReferenceEquals(parent, parentTable))
				{
					parentTable = null;
					break;
				}
			}

			return parentTable;
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new FitElement(this);
		}

		#endregion ICloneable Members

		#region Document node functions

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _independentVariables)
			{
				for (int i = 0; i < _independentVariables.Length; ++i)
				{
					if (null != _independentVariables[i])
						yield return new Main.DocumentNodeAndName(_independentVariables[i], "IndependentVariable" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}
			}

			if (null != _dependentVariables)
			{
				for (int i = 0; i < _dependentVariables.Length; ++i)
				{
					if (null != _dependentVariables[i])
						yield return new Main.DocumentNodeAndName(_dependentVariables[i], "DependentVariable" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}
			}

			if (_fitFunction is Main.IDocumentLeafNode)
				yield return new Main.DocumentNodeAndName((Main.IDocumentLeafNode)_fitFunction, () => _fitFunction = null, "FitFunction");
		}

		#endregion Document node functions
	}
}