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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Serialization;

namespace Altaxo.Gui.Worksheet.Viewing
{
  /// <summary>
  /// WorksheetView is our class for visualizing data tables.
  /// </summary>
  public class WorksheetView : System.Windows.Forms.UserControl, Altaxo.Gui.Worksheet.Viewing.IWorksheetView
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.VScrollBar _vertScrollBar;
    private System.Windows.Forms.HScrollBar _horzScrollBar;
    private WorksheetPanel _worksheetPanel;

    [Browsable(false)]
    private MainMenu _menu;

    /// <summary>
    /// The controller that controls this view
    /// </summary>
    private WinFormsWorksheetController _winFormsController;

		private Altaxo.Gui.Worksheet.Viewing.IWorksheetController _controller;

		/// <summary>Function that creates the Gui dependent controller.</summary>
		Func<Altaxo.Gui.Worksheet.Viewing.IWorksheetController, WorksheetView, WinFormsWorksheetController> _createGuiDependentController;
    
    public WorksheetView()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

			_createGuiDependentController = (x, y) => new WinFormsWorksheetController(x, y);
    }

		public WorksheetView(Func<Altaxo.Gui.Worksheet.Viewing.IWorksheetController, WorksheetView, WinFormsWorksheetController> createController)
		{
			if (null == createController)
				throw new ArgumentNullException("createController");

			InitializeComponent();
			_createGuiDependentController = createController;
		}

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code
  
    
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this._vertScrollBar = new System.Windows.Forms.VScrollBar();
      this._horzScrollBar = new System.Windows.Forms.HScrollBar();
      this._worksheetPanel = new WorksheetPanel();
      this.SuspendLayout();
      // 
      // m_VertScrollBar
      // 
      this._vertScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
      this._vertScrollBar.Location = new System.Drawing.Point(276, 0);
      this._vertScrollBar.Name = "m_VertScrollBar";
      this._vertScrollBar.Size = new System.Drawing.Size(16, 266);
      this._vertScrollBar.TabIndex = 2;
      this._vertScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EhVertScrollBar_Scroll);
      // 
      // m_HorzScrollBar
      // 
      this._horzScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
      this._horzScrollBar.Location = new System.Drawing.Point(0, 250);
      this._horzScrollBar.Name = "m_HorzScrollBar";
      this._horzScrollBar.Size = new System.Drawing.Size(276, 16);
      this._horzScrollBar.TabIndex = 3;
      this._horzScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EhHorzScrollBar_Scroll);
      // 
      // m_GridPanel
      // 
      this._worksheetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this._worksheetPanel.Location = new System.Drawing.Point(0, 0);
      this._worksheetPanel.Name = "m_GridPanel";
      this._worksheetPanel.Size = new System.Drawing.Size(276, 250);
      this._worksheetPanel.TabIndex = 4;
      this._worksheetPanel.Click += new System.EventHandler(this.EhTableArea_Click);
      this._worksheetPanel.SizeChanged += new System.EventHandler(this.EhTableArea_SizeChanged);
      this._worksheetPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseUp);
      this._worksheetPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.EhTableArea_Paint);
      this._worksheetPanel.DoubleClick += new System.EventHandler(this.EhTableArea_DoubleClick);
      this._worksheetPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseMove);
      this._worksheetPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseDown);
      this._worksheetPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseWheel);
      this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseWheel);
      //this.ParentForm.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseWheel);
      
      // 
      // WorksheetView
      // 
      //this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Controls.Add(this._worksheetPanel);
      this.Controls.Add(this._horzScrollBar);
      this.Controls.Add(this._vertScrollBar);
      this.Name = "WorksheetView";
      this.Text = "WorksheetView";
      this.ResumeLayout(false);

    }
    #endregion

    #region Event handlers

    protected override void OnParentChanged(EventArgs e)
    {
      base.OnParentChanged (e);

      if(_menu != null)
        this.TableViewMenu = _menu;
    }



    private void EhVertScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_VertScrollBarScroll(e);
    }

    private void EhHorzScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_HorzScrollBarScroll(e);
    }


    private void EhTableArea_Click(object sender, System.EventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaMouseClick(e);
    }

    private void EhTableArea_DoubleClick(object sender, System.EventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaMouseDoubleClick(e);
    }

    private void EhTableArea_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaPaint(e);
    }

    private void EhTableArea_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaMouseDown(e);
    }

    private void EhTableArea_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaMouseMove(e);
    }

    private void EhTableArea_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaMouseUp(e);
    }

    private void EhTableArea_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaMouseWheel(e);
    }

    private void EhTableArea_SizeChanged(object sender, System.EventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_TableAreaSizeChanged(e);
    }
  
  
    #endregion

    #region IWorksheetView Members

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control TableViewWindow
    {
      get
      {
        return this;
      }
    }

		/// <summary>
		/// Returns the control that should be focused initially.
		/// </summary>
		public object GuiInitiallyFocusedElement { get { return _worksheetPanel; } }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Form TableViewForm
    {
      get
      {
        return this.ParentForm;
      }
    }

  

    public void TakeFocus()
    {
      this.Parent.Show();
      this.Show();
      this.Focus();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MainMenu TableViewMenu
    {
      set
      {
        _menu = value;
#if FormerGuiState
        if(this.ParentForm is WorkbenchForm && null!=m_Menu)
        {
          if(null!=this.ParentForm.Menu)
            this.ParentForm.Menu.MergeMenu( m_Menu ); // do not clone the menu
          else
            this.ParentForm.Menu = m_Menu; // do not clone the menu
        }
#endif
      }
    }

    public int TableViewHorzScrollMaximum
    {
      get
      {
        return _horzScrollBar.Maximum;
      }
      set
      {
        // A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
        // the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
        _horzScrollBar.Maximum = value + _horzScrollBar.LargeChange-1;
        _horzScrollBar.Refresh();
      }
    }

    /// <summary>
    /// Sets the title of this view.
    /// </summary>
    public string TableViewTitle
    {
      set { this.Text = value; }
    }

    public int TableViewVertScrollMaximum
    {
      get
      {
        return _vertScrollBar.Maximum;
      }
      set
      {
        // A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
        // the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
        _vertScrollBar.Maximum = value + _vertScrollBar.LargeChange-1;
        _vertScrollBar.Refresh();
      }
    }

    public int TableViewHorzScrollValue
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

    public int TableViewVertScrollValue
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

    public Graphics TableAreaCreateGraphics()
    {
      return _worksheetPanel.CreateGraphics();
    }

    public void TableAreaInvalidate()
    {
      _worksheetPanel.Invalidate();
    }


    public Size TableAreaSize
    {
      get
      {
        return _worksheetPanel.Size;
      }
    }

    public bool TableAreaCapture
    {
      get { return this._worksheetPanel.Capture; }
      set { this._worksheetPanel.Capture = value; }
    }

    public System.Windows.Forms.Cursor TableAreaCursor
    {
      get { return this._worksheetPanel.Cursor; }
      set { this._worksheetPanel.Cursor = value; }
    }

    #endregion

		#region New IWorksheetView members

		public Altaxo.Gui.Worksheet.Viewing.IWorksheetController Controller
		{
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				_controller = value;
				_winFormsController = _createGuiDependentController(_controller, this);
			}
		}

		public Altaxo.Gui.Worksheet.Viewing.IGuiDependentWorksheetController GuiDependentController
		{
			get
			{
				return _winFormsController;
			}
		}

		#endregion
	}
}
