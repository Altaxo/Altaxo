#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Worksheet.Viewing
{
	public class WorksheetController : IWorksheetController
	{
		IGuiDependentWorksheetController _guiDependentController;
		

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Worksheet.GUI.WorksheetController", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetController),1)]
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

				WorksheetController s = null != o ? (WorksheetController)o : new WorksheetController(null, true);

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
				surr._TableController = s;
				surr._PathToLayout = (Main.DocumentPath)info.GetValue("Layout", s);
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
						_TableController._guiDependentController.InternalInitializeWorksheetLayout( o as Altaxo.Worksheet.WorksheetLayout );
						_PathToLayout = null;
					}
				}

				if (null == _PathToLayout)
				{
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}


		#region Constructors


		public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout)
			: this(layout, false)
		{
		}

		/// <summary>
		/// Creates a WorksheetController which shows the table data into the 
		/// View <paramref name="view"/>.
		/// </summary>
		/// <param name="layout">The worksheet layout.</param>
		/// <param name="bDeserializationConstructor">If true, no layout has to be provided, since this is used as deserialization constructor.</param>
		public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout, bool bDeserializationConstructor)
		{
			Current.Gui.InstrumentControllerWithGuiDependentFunctions(this);
			if (null == _guiDependentController)
				throw new ApplicationException("Gui dependent worksheet controller was not set - it is null!");

			if (null != layout)
				this._guiDependentController.InternalInitializeWorksheetLayout( layout ); // Using DataTable here wires the event chain also
			else if (!bDeserializationConstructor)
				throw new ArgumentNullException("Leaving the layout null in constructor is not supported here");
		}

		/// <summary>
		/// This function is intended for internal purposes only. It sets the Gui dependent controller instance that will do all the work.
		/// </summary>
		/// <param name="guiController">The gui dependent controller.</param>
		public void InternalSetGuiController(IGuiDependentWorksheetController guiController)
		{
			if (null == guiController)
				throw new ArgumentNullException("guiController");

			_guiDependentController = guiController;
		}

		public IGuiDependentWorksheetController InternalGetGuiController()
		{
			return _guiDependentController;
		}

		#endregion // Constructors

		#region IWorksheetController Members

		public Altaxo.Data.DataTable Doc
		{
			get { return _guiDependentController.Doc; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Altaxo.Data.DataTable DataTable
		{
			get
			{
				return _guiDependentController.DataTable;
			}
		}

		public WorksheetLayout WorksheetLayout
		{
			get { return _guiDependentController.WorksheetLayout; }
		}   


		public Altaxo.Worksheet.IndexSelection SelectedDataColumns
		{
			get { return _guiDependentController.SelectedDataColumns; }
		}

		public Altaxo.Worksheet.IndexSelection SelectedDataRows
		{
			get { return _guiDependentController.SelectedDataRows; }
		}

		public Altaxo.Worksheet.IndexSelection SelectedPropertyColumns
		{
			get { return _guiDependentController.SelectedPropertyColumns; }
		}

		public Altaxo.Worksheet.IndexSelection SelectedPropertyRows
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
			_guiDependentController.UpdateTableView();
		}

		#endregion
	}
}
