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
using System.Windows;
using System.Collections.Generic;
using Altaxo;
using Altaxo.Main;
using Altaxo.Worksheet;
using Altaxo.Gui.Worksheet.Viewing;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;

namespace Altaxo.Gui.SharpDevelop
{
	/// <summary>
	/// Extends the default worksheet controller with WinForms context menus.
	/// </summary>
	public class SDWorksheetControllerWpf : Altaxo.Gui.Worksheet.Viewing.WorksheetControllerWpf
	{
		public SDWorksheetControllerWpf(Altaxo.Gui.Worksheet.Viewing.IWorksheetController controller, WorksheetViewWpf view)
			: base(controller, view)
		{
			this.DataColumnHeaderRightClicked += EhDataColumnHeaderRightClicked;
			this.DataRowHeaderRightClicked += EhDataRowHeaderRightClicked;
			this.PropertyColumnHeaderRightClicked += EhPropertyColumnHeaderRightClicked;
			this.TableHeaderRightClicked += EhTableHeaderRightClicked;
			this.OutsideAllRightClicked += EhOutsideAllRightClicked;
		}

		#region Context menu handlers

		protected void EhDataColumnHeaderRightClicked(object sender, Altaxo.Gui.Worksheet.Viewing.ClickedCellInfoWpf clickedCell)
		{
			if (!(this.SelectedDataColumns.Contains(clickedCell.Column)) &&
					!(this.SelectedPropertyRows.Contains(clickedCell.Column)))
			{
				this.ClearAllSelections();
				this.SelectedDataColumns.Add(clickedCell.Column);
				this._view.TableAreaInvalidate();
			}
			MenuService.ShowContextMenu((UIElement)ViewObject,this.ViewObject,"/Altaxo/Views/Worksheet/DataColumnHeader/ContextMenu");
		}

		protected void EhDataRowHeaderRightClicked(object sender, Altaxo.Gui.Worksheet.Viewing.ClickedCellInfoWpf clickedCell)
		{
			if (!(this.SelectedDataRows.Contains(clickedCell.Row)))
			{
				this.ClearAllSelections();
				this.SelectedDataRows.Add(clickedCell.Row);
				this._view.TableAreaInvalidate();
			}
			MenuService.ShowContextMenu((UIElement)ViewObject,this.ViewObject, "/Altaxo/Views/Worksheet/DataRowHeader/ContextMenu");
		}

		protected void EhPropertyColumnHeaderRightClicked(object sender, Altaxo.Gui.Worksheet.Viewing.ClickedCellInfoWpf clickedCell)
		{
			if (!(this.SelectedPropertyColumns.Contains(clickedCell.Column)))
			{
				this.ClearAllSelections();
				this.SelectedPropertyColumns.Add(clickedCell.Column);
				this._view.TableAreaInvalidate();
			}
			MenuService.ShowContextMenu((UIElement)ViewObject,this.ViewObject, "/Altaxo/Views/Worksheet/PropertyColumnHeader/ContextMenu");
		}

		protected void EhTableHeaderRightClicked(object sender, Altaxo.Gui.Worksheet.Viewing.ClickedCellInfoWpf clickedCell)
		{
			MenuService.ShowContextMenu((UIElement)ViewObject,this.ViewObject, "/Altaxo/Views/Worksheet/DataTableHeader/ContextMenu");
		}

		protected void EhOutsideAllRightClicked(object sender, Altaxo.Gui.Worksheet.Viewing.ClickedCellInfoWpf clickedCell)
		{
			MenuService.ShowContextMenu((UIElement)ViewObject,this.ViewObject, "/Altaxo/Views/Worksheet/OutsideAll/ContextMenu");
		}


		#endregion

	}

	public class SDWorksheetViewContent : AbstractViewContent, Altaxo.Gui.IMVCControllerWrapper, IClipboardHandler
	{
		Altaxo.Gui.Worksheet.Viewing.WorksheetController _controller;

		#region Constructors
		/// <summary>
		/// Creates a GraphController which shows the <see cref="Altaxo.Graph.Gdi.GraphDocument"/> in the <c>layout</c>.    
		/// </summary>
		/// <param name="layout">The graph layout which holds the graph document.</param>
		public SDWorksheetViewContent(Altaxo.Worksheet.WorksheetLayout layout)
			: this(layout, false)
		{
		}

		/// <summary>
		/// Creates a WorksheetController which shows the table data into the 
		/// View <paramref name="view"/>.
		/// </summary>
		/// <param name="layout">The worksheet layout.</param>
		/// <param name="bDeserializationConstructor">If true, no layout has to be provided, since this is used as deserialization constructor.</param>
		protected SDWorksheetViewContent(Altaxo.Worksheet.WorksheetLayout layout, bool bDeserializationConstructor)
			:
				this(new Altaxo.Gui.Worksheet.Viewing.WorksheetController(layout))
		{
		}

		public SDWorksheetViewContent(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			_controller = ctrl;

			_controller.TitleNameChanged += EhTitleNameChanged;
			SetTitle();
		}

		void EhTitleNameChanged(object sender, EventArgs e)
		{
			SetTitle();
		}

		void SetTitle()
		{
			if (_controller != null && _controller.DataTable != null)
				this.TitleName = _controller.DataTable.Name;
		}


		#endregion

		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController Controller
		{
			get { return _controller; }
		}

		public Altaxo.Gui.IMVCController MVCController
		{
			get { return _controller; }
		}

		#region Abstract View Content overrides
		#region Required
		public override object Control
		{
			get { return _controller.ViewObject; }
		}
		#endregion

		#endregion

		#region IEditable Members

		public string Text
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		#endregion

		#region IClipboardHandler Members

		public bool EnableCut
		{
			get { return _controller.EnableCut; }
		}

		public bool EnableCopy
		{
			get { return _controller.EnableCopy; }
		}

		public bool EnablePaste
		{
			get { return _controller.EnablePaste; }
		}

		public bool EnableDelete
		{
			get { return _controller.EnableDelete; }
		}

		public bool EnableSelectAll
		{
			get { return _controller.EnableSelectAll; }
		}

		public void Cut()
		{
			_controller.Cut();
		}

		public void Copy()
		{
			_controller.Copy();
		}

		public void Paste()
		{
			_controller.Paste();
		}

		public void Delete()
		{
			_controller.Delete();
		}

		public void SelectAll()
		{
			_controller.SelectAll();
		}

		#endregion
	}
}
