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

namespace Altaxo.Gui.Worksheet.Viewing
{
	[UserControllerForObject(typeof(Altaxo.Worksheet.WorksheetLayout))]
	[ExpectedTypeOfView(typeof(IWorksheetView))]
	public class WorksheetController : IWorksheetController
	{
		IWorksheetView _view;

		IGuiDependentWorksheetController _guiDependentController;


		/// <summary>Holds the data table cached from the layout.</summary>
		protected Altaxo.Data.DataTable _table;
		protected Altaxo.Worksheet.WorksheetLayout _worksheetLayout;

		/// <summary>Fired if the title name changed.</summary>
		public event EventHandler TitleNameChanged;


		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Worksheet.GUI.WorksheetController", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Worksheet.GUI.SDWorksheetController", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDWorksheetViewContent", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDWorksheetViewContent", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetController), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			Main.DocumentPath _PathToLayout;
			WorksheetController _TableController;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				WorksheetController s = (WorksheetController)obj;
				info.AddValue("Layout", Main.DocumentPath.GetAbsolutePath(s.WorksheetLayout));
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				WorksheetController s = null != o ? (WorksheetController)o : new WorksheetController();

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
				surr._TableController = s;
				if (info.CurrentElementName == "Controller")
				{
					info.OpenElement();
					surr._PathToLayout = (Main.DocumentPath)info.GetValue("Layout", s);
					info.CloseElement();
				}
				else
				{
					surr._PathToLayout = (Main.DocumentPath)info.GetValue("Layout", s);
				}
				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
			{

				if (null != _PathToLayout)
				{
					object o = Main.DocumentPath.GetObject(_PathToLayout, documentRoot, _TableController);
					if (o is Altaxo.Worksheet.WorksheetLayout)
					{
						_TableController.WorksheetLayout =  (Altaxo.Worksheet.WorksheetLayout)o;
						_PathToLayout = null;
					}
				}

				if (null == _PathToLayout)
				{
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>Deserialization constructor.</summary>
		private WorksheetController()
		{
		}

		/// <summary>
		/// Creates a WorksheetController which shows the table data into the 
		/// View <paramref name="view"/>.
		/// </summary>
		/// <param name="layout">The worksheet layout.</param>
		public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout)
		{
			if(null==layout)
				throw new ArgumentNullException("Leaving the layout null in constructor is not supported here");

				this.WorksheetLayout = layout;

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
					//oldTable.DataColumns.Changed -= new EventHandler(this.EhTableDataChanged);
					//oldTable.PropCols.Changed -= new EventHandler(this.EhPropertyDataChanged);
					oldTable.NameChanged -= this.EhTableNameChanged;
				}

				_table = newTable;
				if (null != newTable)
				{
					//newTable.DataColumns.Changed += new EventHandler(this.EhTableDataChanged);
					//newTable.PropCols.Changed += new EventHandler(this.EhPropertyDataChanged);
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

		public void EhTableNameChanged(Main.INameOwner sender, string oldName)
		{
			Current.Gui.Execute(EhTableNameChanged_Unsynchronized, sender, oldName);
		}
		private void EhTableNameChanged_Unsynchronized(Main.INameOwner sender, string oldName)
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
			get { return _worksheetLayout; }
		}

		#endregion
	}
}
