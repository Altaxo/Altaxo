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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Summary description for NonlinearFitControl.
  /// </summary>
  [UserControlForController(typeof(INonlinearFitViewEventSink))]
  public class NonlinearFitControl : System.Windows.Forms.UserControl, INonlinearFitView
  {
    INonlinearFitViewEventSink _controller;
    private System.Windows.Forms.TabControl _tabControl;
    private System.Windows.Forms.TabPage _tpSelectFunction;
    private System.Windows.Forms.TabPage _tpMakeFit;
    private System.Windows.Forms.Button _btSelect;
    private System.Windows.Forms.Button _btDoFit;
    private System.Windows.Forms.TabPage _tpFitEnsemble;
    private System.Windows.Forms.Button _btChiSqr;
    private System.Windows.Forms.TextBox _edChiSqr;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button _btNew;
    private System.Windows.Forms.Button _btDoSimplex;
    private ToolStrip _toolStripFit;
    private ToolStripButton _tsbCopyParameter;
    private ToolStripButton _tsbCopyParameterAll;
    private ToolStripButton _tsbCopyParameterValue;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public NonlinearFitControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();


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

    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NonlinearFitControl));
      this._tabControl = new System.Windows.Forms.TabControl();
      this._tpSelectFunction = new System.Windows.Forms.TabPage();
      this._btNew = new System.Windows.Forms.Button();
      this._btSelect = new System.Windows.Forms.Button();
      this._tpFitEnsemble = new System.Windows.Forms.TabPage();
      this._tpMakeFit = new System.Windows.Forms.TabPage();
      this._btDoSimplex = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this._edChiSqr = new System.Windows.Forms.TextBox();
      this._btChiSqr = new System.Windows.Forms.Button();
      this._btDoFit = new System.Windows.Forms.Button();
      this._toolStripFit = new System.Windows.Forms.ToolStrip();
      this._tsbCopyParameter = new System.Windows.Forms.ToolStripButton();
      this._tsbCopyParameterAll = new System.Windows.Forms.ToolStripButton();
      this._tsbCopyParameterValue = new System.Windows.Forms.ToolStripButton();
      this._tabControl.SuspendLayout();
      this._tpSelectFunction.SuspendLayout();
      this._tpMakeFit.SuspendLayout();
      this._toolStripFit.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tabControl
      // 
      this._tabControl.Controls.Add(this._tpSelectFunction);
      this._tabControl.Controls.Add(this._tpFitEnsemble);
      this._tabControl.Controls.Add(this._tpMakeFit);
      this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabControl.Location = new System.Drawing.Point(0, 0);
      this._tabControl.Name = "_tabControl";
      this._tabControl.SelectedIndex = 0;
      this._tabControl.Size = new System.Drawing.Size(432, 384);
      this._tabControl.TabIndex = 0;
      // 
      // _tpSelectFunction
      // 
      this._tpSelectFunction.Controls.Add(this._btNew);
      this._tpSelectFunction.Controls.Add(this._btSelect);
      this._tpSelectFunction.Location = new System.Drawing.Point(4, 22);
      this._tpSelectFunction.Name = "_tpSelectFunction";
      this._tpSelectFunction.Size = new System.Drawing.Size(424, 358);
      this._tpSelectFunction.TabIndex = 0;
      this._tpSelectFunction.Text = "Select fit func";
      // 
      // _btNew
      // 
      this._btNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btNew.Location = new System.Drawing.Point(120, 328);
      this._btNew.Name = "_btNew";
      this._btNew.Size = new System.Drawing.Size(75, 23);
      this._btNew.TabIndex = 1;
      this._btNew.Text = "New..";
      this._btNew.Click += new System.EventHandler(this._btNew_Click);
      // 
      // _btSelect
      // 
      this._btSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btSelect.Location = new System.Drawing.Point(32, 328);
      this._btSelect.Name = "_btSelect";
      this._btSelect.Size = new System.Drawing.Size(75, 23);
      this._btSelect.TabIndex = 0;
      this._btSelect.Text = "Select";
      this._btSelect.Click += new System.EventHandler(this._btSelectFitFunc_Click);
      // 
      // _tpFitEnsemble
      // 
      this._tpFitEnsemble.Location = new System.Drawing.Point(4, 22);
      this._tpFitEnsemble.Name = "_tpFitEnsemble";
      this._tpFitEnsemble.Size = new System.Drawing.Size(424, 358);
      this._tpFitEnsemble.TabIndex = 1;
      this._tpFitEnsemble.Text = "Details";
      // 
      // _tpMakeFit
      // 
      this._tpMakeFit.Controls.Add(this._toolStripFit);
      this._tpMakeFit.Controls.Add(this._btDoSimplex);
      this._tpMakeFit.Controls.Add(this.label1);
      this._tpMakeFit.Controls.Add(this._edChiSqr);
      this._tpMakeFit.Controls.Add(this._btChiSqr);
      this._tpMakeFit.Controls.Add(this._btDoFit);
      this._tpMakeFit.Location = new System.Drawing.Point(4, 22);
      this._tpMakeFit.Name = "_tpMakeFit";
      this._tpMakeFit.Size = new System.Drawing.Size(424, 358);
      this._tpMakeFit.TabIndex = 2;
      this._tpMakeFit.Text = "Fit";
      // 
      // _btDoSimplex
      // 
      this._btDoSimplex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btDoSimplex.Location = new System.Drawing.Point(152, 328);
      this._btDoSimplex.Name = "_btDoSimplex";
      this._btDoSimplex.Size = new System.Drawing.Size(56, 23);
      this._btDoSimplex.TabIndex = 4;
      this._btDoSimplex.Text = "Simplex!";
      this._btDoSimplex.Click += new System.EventHandler(this._btDoSimplex_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Location = new System.Drawing.Point(216, 328);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 16);
      this.label1.TabIndex = 3;
      this.label1.Text = "Chi ²:";
      // 
      // _edChiSqr
      // 
      this._edChiSqr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._edChiSqr.Location = new System.Drawing.Point(264, 328);
      this._edChiSqr.Name = "_edChiSqr";
      this._edChiSqr.Size = new System.Drawing.Size(144, 20);
      this._edChiSqr.TabIndex = 2;
      // 
      // _btChiSqr
      // 
      this._btChiSqr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btChiSqr.Location = new System.Drawing.Point(8, 328);
      this._btChiSqr.Name = "_btChiSqr";
      this._btChiSqr.Size = new System.Drawing.Size(48, 23);
      this._btChiSqr.TabIndex = 1;
      this._btChiSqr.Text = "ChiSqr";
      this._btChiSqr.Click += new System.EventHandler(this._btChiSqr_Click);
      // 
      // _btDoFit
      // 
      this._btDoFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btDoFit.Location = new System.Drawing.Point(80, 328);
      this._btDoFit.Name = "_btDoFit";
      this._btDoFit.Size = new System.Drawing.Size(64, 23);
      this._btDoFit.TabIndex = 0;
      this._btDoFit.Text = "Fit!";
      this._btDoFit.Click += new System.EventHandler(this._btDoFit_Click);
      // 
      // _toolStripFit
      // 
      this._toolStripFit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsbCopyParameter,
            this._tsbCopyParameterAll,
            this._tsbCopyParameterValue});
      this._toolStripFit.Location = new System.Drawing.Point(0, 0);
      this._toolStripFit.Name = "_toolStripFit";
      this._toolStripFit.Size = new System.Drawing.Size(424, 25);
      this._toolStripFit.TabIndex = 5;
      this._toolStripFit.Text = "toolStrip1";
      // 
      // _tsbCopyParameter
      // 
      this._tsbCopyParameter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this._tsbCopyParameter.Image = ((System.Drawing.Image)(resources.GetObject("_tsbCopyParameter.Image")));
      this._tsbCopyParameter.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._tsbCopyParameter.Name = "_tsbCopyParameter";
      this._tsbCopyParameter.Size = new System.Drawing.Size(52, 22);
      this._tsbCopyParameter.Text = "Copy NV";
      this._tsbCopyParameter.ToolTipText = "Copy parameter (name and value) to clipboard";
      this._tsbCopyParameter.Click += new System.EventHandler(this._tsbCopyParameter_Click);
      // 
      // _tsbCopyParameterAll
      // 
      this._tsbCopyParameterAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this._tsbCopyParameterAll.Image = ((System.Drawing.Image)(resources.GetObject("_tsbCopyParameterAll.Image")));
      this._tsbCopyParameterAll.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._tsbCopyParameterAll.Name = "_tsbCopyParameterAll";
      this._tsbCopyParameterAll.Size = new System.Drawing.Size(58, 22);
      this._tsbCopyParameterAll.Text = "Copy NVV";
      this._tsbCopyParameterAll.ToolTipText = "Copy Parameter (name, value, and variance))";
      this._tsbCopyParameterAll.Click += new System.EventHandler(this._tsbCopyParameterAll_Click);
      // 
      // _tsbCopyParameterValue
      // 
      this._tsbCopyParameterValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this._tsbCopyParameterValue.Image = ((System.Drawing.Image)(resources.GetObject("_tsbCopyParameterValue.Image")));
      this._tsbCopyParameterValue.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._tsbCopyParameterValue.Name = "_tsbCopyParameterValue";
      this._tsbCopyParameterValue.Size = new System.Drawing.Size(45, 22);
      this._tsbCopyParameterValue.Text = "Copy V";
      this._tsbCopyParameterValue.ToolTipText = "Copy Parameter Values only";
      this._tsbCopyParameterValue.Click += new System.EventHandler(this._tsbCopyParameterValue_Click);
      // 
      // NonlinearFitControl
      // 
      this.Controls.Add(this._tabControl);
      this.Name = "NonlinearFitControl";
      this.Size = new System.Drawing.Size(432, 384);
      this._tabControl.ResumeLayout(false);
      this._tpSelectFunction.ResumeLayout(false);
      this._tpMakeFit.ResumeLayout(false);
      this._tpMakeFit.PerformLayout();
      this._toolStripFit.ResumeLayout(false);
      this._toolStripFit.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    #region NonlinearFitView member

    public INonlinearFitViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }


    Control _setParameterControl;
    public void SetParameterControl(object control)
    {
      if(_setParameterControl!=null)
      {
        this._tpMakeFit.Controls.Remove(_setParameterControl);
      }

      _setParameterControl = (Control)control;
      _setParameterControl.Location = new Point(0,this._toolStripFit.Size.Height);
      _setParameterControl.Size = new Size(_tpMakeFit.ClientSize.Width, this._btDoFit.Location.Y - _toolStripFit.Size.Height - this._btDoFit.Height/2);
      _setParameterControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this._tpMakeFit.Controls.Add(_setParameterControl);

    }

    Control _funcSelControl;
    public void SetSelectFunctionControl(object control)
    {
      if(_funcSelControl!=null)
      {
        this._tpMakeFit.Controls.Remove(_funcSelControl);
      }

      _funcSelControl = (Control)control;
      _funcSelControl.Location = new Point(0,0);
      _funcSelControl.Size = new Size(this._tpSelectFunction.ClientSize.Width, _btSelect.Location.Y - _btSelect.Size.Height/2);
      _funcSelControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      this._tpSelectFunction.Controls.Add(_funcSelControl);
    }

    Control _fitEnsembleControl;
    public void SetFitEnsembleControl(object control)
    {
      if(_fitEnsembleControl!=null)
      {
        this._tpFitEnsemble.Controls.Remove(_fitEnsembleControl);
      }

      _fitEnsembleControl = (Control)control;
      _fitEnsembleControl.Size = new Size(this._tpFitEnsemble.ClientSize.Width, this._tpFitEnsemble.ClientSize.Height);
      _fitEnsembleControl.Location = new Point(0,0);
      _fitEnsembleControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      this._tpFitEnsemble.Controls.Add(_fitEnsembleControl);
    }

    public void SetChiSquare(double chiSquare)
    {
      this._edChiSqr.Text = Altaxo.Serialization.GUIConversion.ToString(chiSquare);
    }

    public void SwitchToFitEnsemblePage()
    {
      this._tabControl.SelectedTab = this._tpFitEnsemble;
    }

    #endregion

    private void _btDoFit_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_DoFit();
    
    }

    private void _btSelectFitFunc_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_SelectFitFunction();
    
    }

    private void _btChiSqr_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_EvaluateChiSqr();
    }

    private void _btNew_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_NewFitFunction();
    }

    private void _btDoSimplex_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_DoSimplex();
    }

    private void _tsbCopyParameter_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_CopyParameterNV();

    }

    private void _tsbCopyParameterAll_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_CopyParameterNVV();
    }

    private void _tsbCopyParameterValue_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_CopyParameterV();

    }
  }
}
