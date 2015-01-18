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
using Altaxo.Main;
using Altaxo.Worksheet;
using System;
using System.ComponentModel;

namespace Altaxo.Gui.Worksheet.Viewing
{
	[UserControllerForObject(typeof(Altaxo.Worksheet.WorksheetLayout))]
	[ExpectedTypeOfView(typeof(IWorksheetView))]
	public abstract class WorksheetController : IWorksheetController, IDisposable
	{
		/// <summary>Holds the data table cached from the layout.</summary>
		protected Altaxo.Data.DataTable _table;

		protected Altaxo.Worksheet.WorksheetLayout _worksheetLayout;

		/// <summary>Fired if the title name changed.</summary>
		public event EventHandler TitleNameChanged;

		public WeakEventHandler _weakTableNameChangedHandler;

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
			if (null == layout)
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

		protected virtual void InternalInitializeWorksheetLayout(WorksheetLayout value)
		{
			if (null != _worksheetLayout)
				throw new ApplicationException("This controller is already controlling a layout");
			if (null != _table)
				throw new ApplicationException("This controller is already controlling a table");
			if (null == value)
				throw new ArgumentNullException("value");
			if (null == value.DataTable)
				throw new ApplicationException("The DataTable of the WorksheetLayout is null");

			_worksheetLayout = value;
			_table = _worksheetLayout.DataTable;
			_table.Changed += (_weakTableNameChangedHandler = new WeakEventHandler(this.EhTableNameChanged, x => _table.Changed -= x));
			OnTitleNameChanged();
		}

		public virtual void Dispose()
		{
			var view = ViewObject;
			this.ViewObject = null;
			if (view is IDisposable)
				((IDisposable)view).Dispose();

			if (null != _table)
			{
				_weakTableNameChangedHandler.Remove();
			}

			_table = null;
			_worksheetLayout = null; // removes also the event handler(s)
		}

		#endregion Constructors

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
				InternalInitializeWorksheetLayout(value);
			}
		}

		public abstract IndexSelection SelectedDataColumns { get; }

		public abstract IndexSelection SelectedDataRows { get; }

		public abstract IndexSelection SelectedPropertyColumns { get; }

		public abstract IndexSelection SelectedPropertyRows { get; }

		public abstract bool ArePropertyCellsSelected { get; }

		public abstract bool AreDataCellsSelected { get; }

		public abstract bool AreColumnsOrRowsSelected { get; }

		public abstract void ClearAllSelections();

		public abstract void TableAreaInvalidate();

		public abstract bool EnableCut { get; }

		public abstract bool EnableCopy { get; }

		public abstract bool EnablePaste { get; }

		public abstract bool EnableDelete { get; }

		public abstract bool EnableSelectAll { get; }

		public abstract void Cut();

		public abstract void Copy();

		public abstract void Paste();

		public abstract void Delete();

		public abstract void SelectAll();

		private void OnTitleNameChanged()
		{
			if (null != TitleNameChanged)
				TitleNameChanged(this, EventArgs.Empty);
		}

		private void EhTitleNameChanged(object sender, EventArgs e)
		{
			Current.Gui.Execute(EhTitleNameChanged_Unsynchronized, sender, e);
		}

		private void EhTitleNameChanged_Unsynchronized(object sender, EventArgs e)
		{
			if (null != TitleNameChanged)
				TitleNameChanged(this, e);
		}

		public void EhTableNameChanged(object sender, EventArgs e)
		{
			var eAsCCEA = e as Altaxo.Main.NamedObjectCollectionChangedEventArgs;
			if (null != eAsCCEA && object.ReferenceEquals(eAsCCEA.Item, _table))
			{
				if (eAsCCEA.WasItemRenamed)
				{
					Current.Gui.Execute(EhTableNameChanged_Unsynchronized, (INameOwner)eAsCCEA.Item, eAsCCEA.OldName);
				}
			}
		}

		private void EhTableNameChanged_Unsynchronized(INameOwner sender, string oldName)
		{
			var view = ViewObject as IWorksheetView;
			if (view != null)
				view.TableViewTitle = _table.Name;

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

		#endregion IWorksheetController Members

		#region IMVCController Members

		public abstract object ViewObject { get; set; }

		public object ModelObject
		{
			get
			{
				return new WorksheetViewLayout(_worksheetLayout);
			}
		}

		#endregion IMVCController Members

		public bool Apply(bool disposeController)
		{
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
	}
}