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

using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IFunctionPlotDataView
	{
		event EventHandler EditText;

		void InitializeFunctionText(string text, bool editable);
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(XYFunctionPlotData))]
	[ExpectedTypeOfView(typeof(IFunctionPlotDataView))]
	internal class FunctionPlotDataController : IMVCANController
	{
		private XYFunctionPlotData _doc, _originalDoc;
		private UseDocument _useDocumentCopy;
		private IFunctionPlotDataView _view;

		private IMVCAController _functionController;

		public FunctionPlotDataController()
		{
		}

		private void Initialize(bool initDoc)
		{
			if (initDoc)
			{
				// try to find a controller for the underlying function
				_functionController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.Function }, typeof(IMVCAController), UseDocument.Directly);
			}
			if (_view != null)
			{
				bool editable = null != _functionController;
				string text;
				if (_doc.Function is Altaxo.Scripting.IScriptText)
					text = ((Altaxo.Scripting.IScriptText)_doc.Function).ScriptText;
				else
					text = _doc.Function.ToString();

				_view.InitializeFunctionText(text, editable);
			}
		}

		private void EhView_EditText(object sender, EventArgs e)
		{
			if (Current.Gui.ShowDialog(_functionController, "Edit script"))
			{
				_doc.Function = (Altaxo.Calc.IScalarFunctionDD)_functionController.ModelObject;
				Initialize(false);
			}
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is XYFunctionPlotData))
				return false;
			_originalDoc = (XYFunctionPlotData)args[0];
			_doc = _useDocumentCopy == UseDocument.Directly ? _originalDoc : (XYFunctionPlotData)_originalDoc.Clone();
			Initialize(true); // initialize always because we have to update the temporary variables
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value; }
		}

		#endregion IMVCANController Members

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != null)
				{
					_view.EditText -= this.EhView_EditText;
				}

				_view = value as IFunctionPlotDataView;
				if (_view != null)
				{
					_view.EditText += this.EhView_EditText;
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			if (!object.ReferenceEquals(_doc, _originalDoc))
				_originalDoc.CopyFrom(_doc);

			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}