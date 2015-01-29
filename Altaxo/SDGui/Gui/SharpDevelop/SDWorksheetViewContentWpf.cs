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

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.SharpDevelop
{
	public class SDWorksheetViewContent : AbstractViewContent, Altaxo.Gui.IMVCControllerWrapper, IClipboardHandler
	{
		private Altaxo.Gui.Worksheet.Viewing.WorksheetController _controller;

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
		/// Creates a wrapper around a <see cref="Altaxo.Gui.Worksheet.Viewing.WorksheetController"/> which shows the table layout.
		/// </summary>
		/// <param name="layout">The worksheet layout.</param>
		/// <param name="bDeserializationConstructor">If true, no layout has to be provided, since this is used as deserialization constructor.</param>
		protected SDWorksheetViewContent(Altaxo.Worksheet.WorksheetLayout layout, bool bDeserializationConstructor)
			: this(CreateControllerForLayout(layout))
		{
		}

		public SDWorksheetViewContent(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			_controller = ctrl;
			_controller.TitleNameChanged += new WeakEventHandler(this.EhTitleNameChanged, x => _controller.TitleNameChanged -= x);
			SetTitle();
		}

		private static Altaxo.Gui.Worksheet.Viewing.WorksheetControllerWpf CreateControllerForLayout(Altaxo.Worksheet.WorksheetLayout layout)
		{
			var ctrl = new Altaxo.Gui.Worksheet.Viewing.WorksheetControllerWpf();
			ctrl.InitializeDocument(layout);
			return ctrl;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (null != _controller)
			{
				_controller.Dispose();
				_controller = null;
			}
		}

		private void EhTitleNameChanged(object sender, EventArgs e)
		{
			SetTitle();
		}

		private void SetTitle()
		{
			if (_controller != null && _controller.DataTable != null)
				this.TitleName = _controller.DataTable.Name;
		}

		#endregion Constructors

		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController Controller
		{
			get { return _controller; }
		}

		public Altaxo.Gui.IMVCANController MVCController
		{
			get { return _controller; }
		}

		#region Abstract View Content overrides

		#region Required

		public override object Control
		{
			get { return _controller.ViewObject; }
		}

		public override object InitiallyFocusedControl
		{
			get
			{
				return (_controller.ViewObject as Altaxo.Gui.Worksheet.Viewing.IWorksheetView).GuiInitiallyFocusedElement;
			}
		}

		#endregion Required

		#endregion Abstract View Content overrides

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

		#endregion IEditable Members

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

		#endregion IClipboardHandler Members
	}
}