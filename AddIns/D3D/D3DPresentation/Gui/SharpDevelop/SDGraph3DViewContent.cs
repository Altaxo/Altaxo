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

using Altaxo.Graph3D;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.SharpDevelop
{
	public class SDGraph3DViewContent : AbstractViewContent, Altaxo.Gui.IMVCControllerWrapper, IClipboardHandler
	{
		private Altaxo.Gui.Graph3D.Viewing.Graph3DController _controller;

		#region Constructors

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		public SDGraph3DViewContent(GraphDocument3D graphdoc)
			: this(graphdoc, false)
		{
		}

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		/// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
		protected SDGraph3DViewContent(GraphDocument3D graphdoc, bool bDeserializationConstructor)
			: this(new Altaxo.Gui.Graph3D.Viewing.Graph3DControllerWpf(graphdoc))
		{
		}

		public SDGraph3DViewContent(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			_controller = ctrl;
			ctrl.TitleNameChanged += new WeakEventHandler(this.EhTitleNameChanged, x => ctrl.TitleNameChanged -= x);
			SetTitle();
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
			if (_controller != null && _controller.Doc != null)
				this.TitleName = _controller.Doc.Name;
		}

		#endregion Constructors

		public static implicit operator Altaxo.Gui.Graph3D.Viewing.Graph3DController(SDGraph3DViewContent ctrl)
		{
			return ctrl._controller;
		}

		public Altaxo.Gui.Graph3D.Viewing.Graph3DController Controller
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
				return (_controller.ViewObject as Altaxo.Gui.Graph3D.Viewing.IGraph3DView).GuiInitiallyFocusedElement;
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
			get { return true; }
		}

		public bool EnableCopy
		{
			get { return true; }
		}

		public bool EnablePaste
		{
			get { return true; }
		}

		public bool EnableDelete
		{
			get { return true; }
		}

		public bool EnableSelectAll
		{
			get { return false; }
		}

		public void Cut()
		{
			_controller.CutSelectedObjectsToClipboard();
		}

		public void Copy()
		{
			_controller.CopySelectedObjectsToClipboard();
		}

		public void Paste()
		{
			_controller.PasteObjectsFromClipboard();
		}

		public void Delete()
		{
			if (_controller.SelectedObjects.Count > 0)
			{
				_controller.RemoveSelectedObjects();
			}
			else
			{
				throw new NotImplementedException("Please implement according to next line");
				// nothing is selected, we assume that the user wants to delete the worksheet itself
				//Current.ProjectService.DeleteGraphDocument(_controller.Doc, false);
			}
		}

		public void SelectAll()
		{
		}

		#endregion IClipboardHandler Members
	}
}