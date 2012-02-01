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
#endregion

using System;
using System.ComponentModel;

using Altaxo.Worksheet;
using Altaxo.Collections;
using Altaxo.Main;

namespace Altaxo.Gui.Worksheet.Viewing
{
	[UserControllerForObject(typeof(Altaxo.Worksheet.WorksheetLayout))]
	[ExpectedTypeOfView(typeof(IWorksheetView))]
	public class WorksheetController : IWorksheetController, IDisposable
	{
		IWorksheetView _view;

		IGuiDependentWorksheetController _guiDependentController;


		/// <summary>Holds the data table cached from the layout.</summary>
		protected Altaxo.Data.DataTable _table;
		protected Altaxo.Worksheet.WorksheetLayout _worksheetLayout;

		/// <summary>Fired if the title name changed.</summary>
		public event EventHandler TitleNameChanged;


	

		#region Constructors

		/// <summary>Deserialization constructor.</summary>
		public WorksheetController()
		{
		}

		/// <summary>
		/// Creates a WorksheetController which shows the table data using the specified <paramref name="layout"/>.
		/// </summary>
		/// <param name="layout">The worksheet layout.</param>
		public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout)
		{
			if(null==layout)
				throw new ArgumentNullException("Leaving the layout null in constructor is not supported here");

				this.WorksheetLayout = layout;

		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0)
				return false;
			if (args[0] is WorksheetLayout)
				this.WorksheetLayout = (WorksheetLayout)args[0];
			else if (args[0] is WorksheetViewLayout)
				this.WorksheetLayout = ((WorksheetViewLayout)args[0]).WorksheetLayout;
			else
				return false;

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		public void Dispose()
		{
			var view = _view;
			this.ViewObject = null;
			if (view is IDisposable)
				((IDisposable)view).Dispose();

			if (null != _table)
			{
				_table.NameChanged -= EhTableNameChanged;
			}

			_table = null;
			_worksheetLayout = null; // removes also the event handler(s)
		}
	

		#endregion // Constructors

		#region IWorksheetController Members

		public Altaxo.Data.DataTable DataTable
		{
			get
			{
				return this._table;
			}
		}


		public WorksheetLayout WorksheetLayout
		{
			get { return _worksheetLayout; }
			
			set
			{
				if (null != _worksheetLayout)
					throw new ApplicationException("This controller is already controlling a layout");
				if (null == value)
					throw new ArgumentNullException("value");

				_worksheetLayout = value;

				Altaxo.Data.DataTable oldTable = _table;
				Altaxo.Data.DataTable newTable = null == _worksheetLayout ? null : _worksheetLayout.DataTable;

				if (null != oldTable)
				{
					oldTable.NameChanged -= this.EhTableNameChanged;
				}

				_table = newTable;
				if (null != newTable)
				{
					newTable.NameChanged += this.EhTableNameChanged;
					OnTitleNameChanged();
				}
			}
		}   

		public IndexSelection SelectedDataColumns
		{
			get { return _guiDependentController.SelectedDataColumns; }
		}

		public IndexSelection SelectedDataRows
		{
			get { return _guiDependentController.SelectedDataRows; }
		}

		public IndexSelection SelectedPropertyColumns
		{
			get { return _guiDependentController.SelectedPropertyColumns; }
		}

		public IndexSelection SelectedPropertyRows
		{
			get { return _guiDependentController.SelectedPropertyRows; }
		}

		public bool ArePropertyCellsSelected
		{
			get { return _guiDependentController.ArePropertyCellsSelected; }
		}

		public bool AreDataCellsSelected
		{
			get { return _guiDependentController.AreDataCellsSelected; }
		}

		public bool AreColumnsOrRowsSelected
		{
			get { return _guiDependentController.AreColumnsOrRowsSelected; }
		}

		public void ClearAllSelections()
		{
			_guiDependentController.ClearAllSelections();
		}

		public void UpdateTableView()
		{
			_guiDependentController.TableAreaInvalidate();
		}

		public bool EnableCut
		{
			get
			{
				return _guiDependentController.EnableCut;
			}
		}
		public bool EnableCopy
		{
			get
			{
				return _guiDependentController.EnableCopy;
			}
		}
		public bool EnablePaste
		{
			get
			{
				return _guiDependentController.EnablePaste;
			}
		}
		public bool EnableDelete
		{
			get
			{
				return _guiDependentController.EnableDelete;
			}
		}
		public bool EnableSelectAll
		{
			get
			{
				return _guiDependentController.EnableSelectAll;
			}
		}

		public void Cut()
		{
			_guiDependentController.Cut();
		}
		public void Copy()
		{
			_guiDependentController.Copy();
		}
		public void Paste()
		{
			_guiDependentController.Paste();
		}
		public void Delete()
		{
			_guiDependentController.Delete();
		}
		public void SelectAll()
		{
			_guiDependentController.SelectAll();
		}


		void OnTitleNameChanged()
		{
			if (null != TitleNameChanged)
				TitleNameChanged(this,EventArgs.Empty);
		}

		void EhTitleNameChanged(object sender, EventArgs e)
		{
			Current.Gui.Execute(EhTitleNameChanged_Unsynchronized, sender, e);
		}
		void EhTitleNameChanged_Unsynchronized(object sender, EventArgs e)
		{
			if (null != TitleNameChanged)
				TitleNameChanged(this, e);
		}

		public void EhTableNameChanged(INameOwner sender, string oldName)
		{
			Current.Gui.Execute(EhTableNameChanged_Unsynchronized, sender, oldName);
		}
		private void EhTableNameChanged_Unsynchronized(INameOwner sender, string oldName)
		{
			if (_view != null)
				_view.TableViewTitle = _table.Name;

			this.TitleName = _table.Name;
		}
		/// <summary>
		/// This is the whole name of the content, e.g. the file name or
		/// the url depending on the type of the content.
		/// </summary>
		public string TitleName
		{
			get
			{
				return _table.Name;
			}
			set
			{
				OnTitleNameChanged();
			}
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_guiDependentController = null;
					_view.Controller = null;
				}

				_view = value as IWorksheetView;

				if (null != _view)
				{
					_view.Controller = this;
					_guiDependentController = _view.GuiDependentController;
					_view.TableViewTitle = this.TitleName;
				}
			}
		}

		public object ModelObject
		{
			get 
			{
				return new WorksheetViewLayout(_worksheetLayout);
			}
		}

		#endregion



		

		public bool Apply()
		{
			return true;
		}
	}
}
