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

using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Main.Services;
using Altaxo.Scripting;
using System;
using System.Text;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	public enum FitFunctionContextMenuStyle
	{
		None,
		Edit,
		EditAndDelete
	}

	public interface IFitFunctionSelectionView
	{
		IFitFunctionSelectionViewEventSink Controller { get; set; }

		void ClearFitFunctionList();

		void AddFitFunctionList(string rootname, Altaxo.Main.Services.IFitFunctionInformation[] info, FitFunctionContextMenuStyle menustyle);

		void SetRtfDocumentation(string rtfString);

		NamedColor GetRtfBackgroundColor();

		void SelectFitFunction(IFitFunction func);
	}

	public interface IFitFunctionSelectionViewEventSink
	{
		void EhView_SelectionChanged(IFitFunctionInformation selectedtag);

		void EhView_EditItem(IFitFunctionInformation selectedtag);

		void EhView_CreateItemFromHere(IFitFunctionInformation selectedtag);

		void EhView_RemoveItem(IFitFunctionInformation selectedtag);
	}

	public interface IFitFunctionSelectionController : IMVCAController
	{
		void Refresh();
	}

	public class FitFunctionSelectionController : IFitFunctionSelectionViewEventSink, IMVCAController
	{
		private IFitFunction _doc;
		private IFitFunctionInformation _tempdoc;
		private IFitFunctionSelectionView _view;

		public FitFunctionSelectionController(IFitFunction doc)
		{
			_doc = doc;
			_tempdoc = null;
			Initialize();
		}

		public void Refresh()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (_view != null)
			{
				_view.ClearFitFunctionList();
				_view.AddFitFunctionList("Builtin functions", Current.FitFunctionService.GetBuiltinFitFunctions(), FitFunctionContextMenuStyle.None);
				_view.AddFitFunctionList("Application functions", Current.FitFunctionService.GetApplicationFitFunctions(), FitFunctionContextMenuStyle.Edit);
				_view.AddFitFunctionList("User functions", Current.FitFunctionService.GetUserDefinedFitFunctions(), FitFunctionContextMenuStyle.EditAndDelete);
				_view.AddFitFunctionList("Document functions", Current.FitFunctionService.GetDocumentFitFunctions(), FitFunctionContextMenuStyle.EditAndDelete);
			}
		}

		public void EhView_SelectionChanged(IFitFunctionInformation selectedtag)
		{
			_tempdoc = selectedtag;
		}

		public void EhView_EditItem(IFitFunctionInformation selectedtag)
		{
			EditItemOrItemCopy(selectedtag, false);
		}

		public void EhView_CreateItemFromHere(IFitFunctionInformation selectedtag)
		{
			EditItemOrItemCopy(selectedtag, true);
		}

		public void EditItemOrItemCopy(IFitFunctionInformation selectedtag, bool editItemCopy)
		{
			IFitFunction func = null;
			if (selectedtag is DocumentFitFunctionInformation)
			{
				func = selectedtag.CreateFitFunction();
			}
			else if (selectedtag is FileBasedFitFunctionInformation)
			{
				func = Altaxo.Main.Services.FitFunctionService.ReadUserDefinedFitFunction(selectedtag as Altaxo.Main.Services.FileBasedFitFunctionInformation);
			}

			if (null != func)
			{
				var editedFunc = Edit(func, editItemCopy);

				if (null != editedFunc)
				{
				}
			}
		}

		private FitFunctionScript Edit(IFitFunction func, bool editItemCopy)
		{
			if (null != func)
			{
				if (func is FitFunctionScript)
					editItemCopy = true; // for scripts, we always edit a copy

				object[] args = new object[] { (editItemCopy && func is ICloneable cfunc) ? cfunc.Clone() : func };
				if (Current.Gui.ShowDialog(args, "Edit fit function script"))
				{
					if (args[0] is FitFunctionScript editedScript)
					{
						var ctrl = new Altaxo.Gui.Scripting.FitFunctionNameAndCategoryController();
						ctrl.InitializeDocument(editedScript);
						if (Current.Gui.ShowDialog(ctrl, "Store?"))
						{
							// add the new script to the list
							var editedScript2 = Current.Project.FitFunctionScripts.Add(editedScript);

							if (!object.ReferenceEquals(editedScript, editedScript2))
								Current.Gui.InfoMessageBox("Edited fit function was not added because exactly the same fit function already exists.", "To your information");

							// Note: category and/or name can have changed now, so it is more save to
							// completely reinitialize the fit function tree
							Initialize();

							return editedScript2;
						}
					}
				}
			}
			return null;
		}

		public void EhView_RemoveItem(IFitFunctionInformation selectedtag)
		{
			if (selectedtag is DocumentFitFunctionInformation)
			{
				Current.Project.FitFunctionScripts.Remove(selectedtag.CreateFitFunction() as FitFunctionScript);
				Initialize();
			}
			else if (selectedtag is FileBasedFitFunctionInformation)
			{
				Current.FitFunctionService.RemoveUserDefinedFitFunction(selectedtag as Altaxo.Main.Services.FileBasedFitFunctionInformation);
				Initialize();
			}
		}

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
					_view.Controller = null;

				_view = value as IFitFunctionSelectionView;

				Initialize();

				if (_view != null)
					_view.Controller = this;
			}
		}

		public object ModelObject
		{
			get
			{
				return _doc;
			}
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			if (_tempdoc == null) // nothing selected, so return the original doc
				return true;

			try
			{
				_doc = _tempdoc.CreateFitFunction();
				return true;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("Can not create fit function. An exception was thrown: " + ex.Message);
			}
			return false;
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