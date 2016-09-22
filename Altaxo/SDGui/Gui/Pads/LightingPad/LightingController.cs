#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Pads.LightingPad
{
	/// <summary>
	/// Responsible for showing the notes of worksheets and graph windows.
	/// </summary>
	public class LightingController : ICSharpCode.SharpDevelop.Gui.IPadContent
	{
		private LightingControl _view;

		/// <summary>The currently active view content to which the text belongs.</summary>
		private WeakReference _currentActiveViewContent = new WeakReference(null);

		public LightingController()
		{
			_view = new LightingControl();

			//			_view.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			//			_view.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;

			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += EhActiveWorkbenchWindowChanged;
			_view.LostFocus += new System.Windows.RoutedEventHandler(_view_LostFocus);
			_view.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(_view_LostKeyboardFocus);
			_view.LightingChanged += EhView_LightingChanged;
			_view.IsEnabled = false;

			EhActiveWorkbenchWindowChanged(this, EventArgs.Empty); // Find out if the active workbench window is a Graph3d
		}

		private void EhView_LightingChanged(object sender, EventArgs e)
		{
			if (_currentActiveViewContent.IsAlive && object.ReferenceEquals(_currentActiveViewContent.Target, Current.Workbench.ActiveViewContent))
			{
				var ctrl = ActiveViewContentAsGraph3DController;
				if (null != ctrl)
				{
					ctrl.Doc.Lighting = _view.Lighting;
				}
			}
		}

		private void _view_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
		}

		private void _view_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
		}

		private Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController ActiveViewContentAsGraph3DController
		{
			get
			{
				var cnt = Current.Workbench.ActiveViewContent as IMVCControllerWrapper;
				var ctrl = cnt?.MVCController as Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController;
				return ctrl;
			}
		}

		private void EhActiveWorkbenchWindowChanged(object sender, EventArgs e)
		{
			_currentActiveViewContent = new WeakReference(Current.Workbench.ActiveViewContent);

			bool enable = true;

			var ctrl = ActiveViewContentAsGraph3DController;

			if (null != ctrl)
			{
				_view.Lighting = ctrl.Doc.Lighting;
				enable = true;
			}
			else
			{
				_view.Lighting = null;
				enable = false;
			}

			_view.IsEnabled = enable;

			// uncomment the following if we want to activate the lighting pad each time a Graph3D window is activated
			/*
			if (enable && _view.Lighting != null)
			{
				ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow ww = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

				var pad = WorkbenchSingleton.Workbench.GetPad(this.GetType());
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(pad);

				// now focus back to the formerly active workbench window.
				ww.SelectWindow();
			}
			*/
		}

		/// <summary>
		/// Stores the text in the text control back to the graph document or worksheet.
		/// </summary>

		#region IPadContent Members

		public object Control
		{
			get
			{
				return this._view;
			}
		}

		public object InitiallyFocusedControl
		{
			get
			{
				return null;
			}
		}

		#endregion IPadContent Members

		#region IDisposable Members

		public void Dispose()
		{
			if (_view != null)
			{
				_view = null;
			}
		}

		#endregion IDisposable Members
	}
}