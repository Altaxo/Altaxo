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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui.Worksheet.Viewing
{
    /// <summary>
    /// Interaction logic for WorksheetViewWpf.xaml
    /// </summary>
    public partial class WorksheetViewWpf : UserControl, Altaxo.Gui.Worksheet.Viewing.IWorksheetView
    {
        private WorksheetController _guiController;

        protected VisualHost _visualHost;
        protected TextBox _cellEditControl;

        #region Cell edit

        public event Action CellEdit_LostFocus;

        public event Action<AltaxoKeyboardKey, HandledEventArgs> CellEdit_PreviewKeyPressed;

        public event Action CellEdit_TextChanged;

        private bool _cellEdit_IsArmed;

        private void InitializeCellEditControl()
        {
            _cellEditControl = new TextBox();
            _cellEditControl.AcceptsTab = true;
            _cellEditControl.BorderThickness = new Thickness(0);
            _cellEditControl.AcceptsReturn = true;
            _cellEditControl.Name = "_cellEditControl";
            _cellEditControl.TabIndex = 0;
            _cellEditControl.Text = "";
            _cellEditControl.PreviewKeyDown += (s, e) => { var ee = new HandledEventArgs(false); CellEdit_PreviewKeyPressed?.Invoke(GuiHelper.ToAltaxo(e.Key), ee); e.Handled = ee.Handled; };
            _cellEditControl.TextChanged += (s, e) => CellEdit_TextChanged?.Invoke();
            _cellEditControl.LostKeyboardFocus += (s, e) => CellEdit_LostFocus?.Invoke();
            _cellEditControl.Visibility = Visibility.Hidden;

            _visualHost = new VisualHost(EhView_TableAreaPaint);

            this.LowerCanvas.Children.Add(_visualHost);
            this.Canvas.Children.Add(_cellEditControl);
        }

        public int CellEdit_SelectionStart
        {
            get
            {
                return _cellEditControl.SelectionStart;
            }
            set
            {
                _cellEditControl.SelectionStart = value;
            }
        }

        public int CellEdit_SelectionLength
        {
            get
            {
                return _cellEditControl.SelectionLength;
            }
            set
            {
                _cellEditControl.SelectionLength = value;
            }
        }

        public void CellEdit_Cut()
        {
            _cellEditControl.Cut();
        }

        public void CellEdit_Copy()
        {
            _cellEditControl.Copy();
        }

        public void CellEdit_Paste()
        {
            _cellEditControl.Paste();
        }

        public void CellEdit_Clear()
        {
            _cellEditControl.Clear();
        }

        public string CellEdit_Text
        {
            get
            {
                return _cellEditControl.Text;
            }
            set
            {
                _cellEditControl.Text = value;
            }
        }

        public void CellEdit_Hide()
        {
            _cellEditControl.Visibility = Visibility.Hidden;
            _cellEdit_IsArmed = false;
            this.Canvas.Focus();
        }

        public void CellEdit_Show()
        {
            if (!this.Canvas.Children.Contains(_cellEditControl))
                this.Canvas.Children.Add(_cellEditControl);
            _cellEditControl.Visibility = Visibility.Visible;
            _cellEditControl.Focus();
            _cellEdit_IsArmed = true;
        }

        public void CellEdit_SetTextAlignmentAndSelectAll(bool textAlignmentRight)
        {
            _cellEditControl.TextAlignment = textAlignmentRight ? TextAlignment.Right : TextAlignment.Left;
            _cellEditControl.SelectAll();
        }

        public RectangleD2D CellEdit_Location
        {
            set
            {
                _cellEditControl.SetValue(Canvas.LeftProperty, value.Left);
                _cellEditControl.SetValue(Canvas.TopProperty, value.Top);
                _cellEditControl.Width = value.Width;
                _cellEditControl.Height = value.Height;
            }
        }

        #endregion Cell edit

        #region Cursor

        public void Cursor_SetToArrow()
        {
            this.TableAreaCursor = Cursors.Arrow;
        }

        public void Cursor_SetToResizeWestEast()
        {
            this.TableAreaCursor = Cursors.SizeWE;
        }

        #endregion Cursor

        public WorksheetViewWpf()
        {
            InitializeComponent();

            InitializeCellEditControl();
        }

        public WorksheetViewWpf(Func<Altaxo.Gui.Worksheet.Viewing.IWorksheetController, WorksheetViewWpf, WorksheetController> createController)
        {
            if (null == createController)
                throw new ArgumentNullException("createController");

            InitializeComponent();

            InitializeCellEditControl();
        }

        #region Altaxo.Gui.Worksheet.Viewing.IWorksheetView

        public IWorksheetController Controller
        {
            set
            {
                _guiController = value as WorksheetController;
            }
        }

        /// <summary>
        /// Returns the control that should be focused initially.
        /// </summary>
        public object GuiInitiallyFocusedElement
        {
            get { return _worksheetPanel; }
        }

        #endregion Altaxo.Gui.Worksheet.Viewing.IWorksheetView

        #region Functions called from GuiController

        public Canvas Canvas
        {
            get { return _worksheetPanel; }
        }

        public Canvas LowerCanvas
        {
            get { return _lowerPanel; }
        }

        public PointD2D TableArea_Size
        {
            get
            {
                return new PointD2D(_worksheetPanel.ActualWidth, _worksheetPanel.ActualHeight);
            }
        }

        public double TableViewHorzScrollValue
        {
            get
            {
                return _horzScrollBar.Value;
            }
            set
            {
                _horzScrollBar.Value = value;
            }
        }

        public double TableViewHorzViewPortSize
        {
            set
            {
                _horzScrollBar.ViewportSize = value;
            }
        }

        public double TableViewVertScrollValue
        {
            get
            {
                return _vertScrollBar.Value;
            }
            set
            {
                _vertScrollBar.Value = value;
            }
        }

        public double TableViewVertViewPortSize
        {
            set
            {
                _vertScrollBar.ViewportSize = value;
            }
        }

        public double TableViewHorzScrollMaximum
        {
            get
            {
                return _horzScrollBar.Maximum;
            }
            set
            {
                // A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
                // the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
                _horzScrollBar.Maximum = value + _horzScrollBar.LargeChange - 1;
                // TODO (Wpf) _horzScrollBar.Refresh();
            }
        }

        public double TableViewVertScrollMaximum
        {
            get
            {
                return _vertScrollBar.Maximum;
            }
            set
            {
                // A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
                // the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
                _vertScrollBar.Maximum = value + _vertScrollBar.LargeChange - 1;
                // TODO (Wpf) _vertScrollBar.Refresh();
            }
        }

        public bool TableArea_IsCaptured
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

        #endregion Functions called from GuiController

        #region Helpers

        private Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition(_worksheetPanel);
        }

        #endregion Helpers

        #region Event handlers

        public void TableArea_TriggerRedrawing()
        {
            _visualHost.InvalidateDrawing();
        }

        public void EhView_TableAreaPaint(DrawingContext unused)
        {
            if (Canvas == null || _guiController == null || _guiController.DataTable == null)
                return;

            using (var dc = _visualHost.OpenDrawingContext())
            {
                Rect clipRect = new Rect(0, 0, this.Canvas.ActualWidth, this.Canvas.ActualHeight);

                WorksheetPaintingWpf.PaintTableArea(dc, _guiController.WorksheetLayout, clipRect.Size, clipRect,
                    _guiController.SelectedDataColumns, _guiController.SelectedDataRows, _guiController.SelectedPropertyColumns, _guiController.SelectedPropertyRows,
                    _guiController.HorzScrollPos, _guiController.VertScrollPos);

                dc.Close();
            }
            // now create the resizing positions
            _guiController.CreateResizingPositions();
        }

        private void EhWP_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _worksheetPanel.Focus();

            if (_guiController != null)
            {
                _guiController.EhView_TableAreaMouseDown(GuiHelper.ToAltaxo(GetPosition(e)));
            }
        }

        private void EhWP_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_guiController != null)
                _guiController.EhView_TableAreaMouseUp(GuiHelper.ToAltaxo(GetPosition(e)));

            if (e.ChangedButton == MouseButton.Left && e.ClickCount >= 2)
                _guiController.EhView_TableAreaMouseDoubleClick(GuiHelper.ToAltaxo(GetPosition(e)));
            else if (e.ClickCount == 1)
                _guiController.EhView_TableAreaMouseClick(GuiHelper.ToAltaxo(GetPosition(e)), GuiHelper.ToAltaxo(e.ChangedButton), GuiHelper.ToAltaxo(Keyboard.Modifiers));
        }

        private void EhWP_MouseMove(object sender, MouseEventArgs e)
        {
            if (_guiController != null)
                _guiController.EhView_TableAreaMouseMove(GuiHelper.ToAltaxo(GetPosition(e)));
        }

        private void EhWP_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_guiController != null)
                _guiController.EhView_TableAreaMouseWheel(GuiHelper.ToAltaxo(GetPosition(e)), e.Delta);
        }

        private void EhWP_KeyDown(object sender, KeyEventArgs e)
        {
            if (_guiController != null)
                _guiController.EhView_KeyDown(GuiHelper.ToAltaxo(e.Key));
        }

        private void EhWP_HorzScroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (null != _guiController)
                _guiController.EhView_HorzScrollBarScroll((int)e.NewValue);
        }

        private void EhWP_VertScroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (null != _guiController)
                _guiController.EhView_VertScrollBarScroll((int)e.NewValue);
        }

        #endregion Event handlers

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