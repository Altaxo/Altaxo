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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Worksheet.Viewing
{
	/// <summary>
	/// Interaction logic for WorksheetViewWpf.xaml
	/// </summary>
	public partial class WorksheetViewWpf : UserControl, Altaxo.Gui.Worksheet.Viewing.IWorksheetView
	{
		IWorksheetController _controller;
		WorksheetControllerWpf _guiController;

		/// <summary>Function that creates the Gui dependent controller.</summary>
		Func<Altaxo.Gui.Worksheet.Viewing.IWorksheetController, WorksheetViewWpf, WorksheetControllerWpf> _createGuiDependentController;


		public WorksheetViewWpf()
		{
			InitializeComponent();

			_createGuiDependentController = (x, y) => new WorksheetControllerWpf(x, y);
		}

			public WorksheetViewWpf(Func<Altaxo.Gui.Worksheet.Viewing.IWorksheetController, WorksheetViewWpf, WorksheetControllerWpf> createController)
		{
			if (null == createController)
				throw new ArgumentNullException("createController");

			InitializeComponent();
			_createGuiDependentController = createController;
		}

		#region Altaxo.Gui.Worksheet.Viewing.IWorksheetView

		public IWorksheetController Controller
		{
			set
			{
				if (null != value)
				{
					_controller = value;
					_guiController = _createGuiDependentController(_controller, this);
				}
				else
				{
					_controller = null;
					_guiController = null;
				}
			}
		}

		public IGuiDependentWorksheetController GuiDependentController
		{
			get { return _guiController; }
		}

		public void TableAreaInvalidate()
		{
			if (null != _guiController)
				_guiController.TableAreaInvalidate();

		}

		public string TableViewTitle
		{
			set
			{
			
			}
		}

		/// <summary>
		/// Returns the control that should be focused initially.
		/// </summary>
		public object GuiInitiallyFocusedElement
		{
			get { return _worksheetPanel; }
		}

		#endregion

		#region Functions called from GuiController

		public Canvas Canvas
		{
			get { return _worksheetPanel; }
		}

		public Canvas LowerCanvas
		{
			get { return _lowerPanel; }
		}

		public Size TableAreaSize
		{
			get
			{
				return new Size(_worksheetPanel.ActualWidth, _worksheetPanel.ActualHeight);
			}
		}

		public int TableViewHorzScrollValue
		{
			get
			{
				return (int)_horzScrollBar.Value;
			}
			set
			{
				_horzScrollBar.Value = value;
			}
		}

		public int TableViewVertScrollValue
		{
			get
			{
				return (int)_vertScrollBar.Value;
			}
			set
			{
				_vertScrollBar.Value = value;
			}
		}

		public int TableViewHorzScrollMaximum
		{
			get
			{
				return (int)_horzScrollBar.Maximum;
			}
			set
			{
				// A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
				// the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
				_horzScrollBar.Maximum = value + _horzScrollBar.LargeChange - 1;
				// TODO (Wpf) _horzScrollBar.Refresh();
			}
		}

		public int TableViewVertScrollMaximum
		{
			get
			{
				return (int)_vertScrollBar.Maximum;
			}
			set
			{
				// A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
				// the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
				_vertScrollBar.Maximum = value + _vertScrollBar.LargeChange - 1;
				// TODO (Wpf) _vertScrollBar.Refresh();
			}
		}

		public bool TableAreaCapture
		{
			get { return this._worksheetPanel.IsMouseCaptured; }
			set
			{
				if (_worksheetPanel.IsMouseCaptured == value)
					return;

				if (value == true)
					this._worksheetPanel.CaptureMouse();
				else
					this._worksheetPanel.ReleaseMouseCapture();
			}
		}

		public Cursor TableAreaCursor
		{
			get { return this._worksheetPanel.Cursor; }
			set { this._worksheetPanel.Cursor = value; }
		}

		#endregion

		#region Helpers

		private Point GetPosition(MouseEventArgs e)
		{
			return e.GetPosition(_worksheetPanel);
		}

		#endregion


		#region Event handlers

		private void EhWP_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if(!_worksheetPanel.IsFocused)
				_worksheetPanel.Focus();


			if (_guiController != null)
			{
				_guiController.EhView_TableAreaMouseDown(GetPosition(e), e);
			}
		}

		private void EhWP_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (_guiController != null)
				_guiController.EhView_TableAreaMouseUp(GetPosition(e), e);

			if (e.ChangedButton == MouseButton.Left && e.ClickCount >= 2)
				_guiController.EhView_TableAreaMouseDoubleClick(GetPosition(e), e);
			else if (e.ClickCount == 1)
				_guiController.EhView_TableAreaMouseClick(GetPosition(e), e);

		}

		private void EhWP_MouseMove(object sender, MouseEventArgs e)
		{
			if (_guiController != null)
				_guiController.EhView_TableAreaMouseMove(GetPosition(e), e);
		}

		private void EhWP_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (_guiController != null)
				_guiController.EhView_TableAreaMouseWheel(GetPosition(e), e);
		}

		private void EhWP_KeyDown(object sender, KeyEventArgs e)
		{
			if (_guiController != null)
				_guiController.EhView_KeyDown(e);
		}


		private void EhWP_HorzScroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			if (null != _guiController)
				_guiController.EhView_HorzScrollBarScroll(e);
		}

		private void EhWP_VertScroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
			if (null != _guiController)
				_guiController.EhView_VertScrollBarScroll(e);
		}

		#endregion

		private void EhWP_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (null != _guiController)
				_guiController.EhView_TableAreaSizeChanged(e);
		}

		private void EhEnableCmdCopy(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _guiController && _guiController.EnableCopy;
			e.Handled = true;
		}

		private void EhEnableCmdPaste(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _guiController && _guiController.EnablePaste;
			e.Handled = true;
		}

		private void EhCmdCopy(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _guiController)
			{
				_guiController.Copy();
				e.Handled = true;
			}
		}

		private void EhCmdPaste(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _guiController)
			{
				_guiController.Paste();
				e.Handled = true;
			}
		}

		private void EhEnableCmdCut(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _guiController && _guiController.EnableCut;
			e.Handled = true;
		}

		private void EhCmdCut(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _guiController)
			{
				_guiController.Cut();
				e.Handled = true;
			}
		}

		private void EhEnableCmdDelete(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _guiController && _guiController.EnableDelete;
			e.Handled = true;
		}

		private void EhCmdDelete(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _guiController)
			{
				_guiController.Delete();
				e.Handled = true;
			}
		}

		private void EhEnableCmdSelectAll(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = null != _guiController && _guiController.EnableSelectAll;
			e.Handled = true;
		}

		private void EhCmdSelectAll(object sender, ExecutedRoutedEventArgs e)
		{
			if (null != _guiController)
			{
				_guiController.SelectAll();
				e.Handled = true;
			}
		}

	}
}
