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
	internal class FunctionPlotDataController : MVCANControllerEditOriginalDocBase<XYFunctionPlotData, IFunctionPlotDataView>
	{
		private IMVCAController _functionController;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_functionController, () => _functionController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
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

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.EditText += this.EhView_EditText;
		}

		protected override void DetachView()
		{
			_view.EditText -= this.EhView_EditText;
			base.DetachView();
		}

		private void EhView_EditText(object sender, EventArgs e)
		{
			if (Current.Gui.ShowDialog(_functionController, "Edit script"))
			{
				_doc.Function = (Altaxo.Calc.IScalarFunctionDD)_functionController.ModelObject;
				Initialize(false);
			}
		}
	}
}