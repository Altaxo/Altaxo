using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Altaxo.Main.GUI;

namespace Altaxo.Calc.Regression.Nonlinear
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
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public NonlinearFitControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call

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
      this._tabControl = new System.Windows.Forms.TabControl();
      this._tpSelectFunction = new System.Windows.Forms.TabPage();
      this._btSelect = new System.Windows.Forms.Button();
      this._tpMakeFit = new System.Windows.Forms.TabPage();
      this._btDoFit = new System.Windows.Forms.Button();
      this._tpFitEnsemble = new System.Windows.Forms.TabPage();
      this._tabControl.SuspendLayout();
      this._tpSelectFunction.SuspendLayout();
      this._tpMakeFit.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tabControl
      // 
      this._tabControl.Controls.Add(this._tpSelectFunction);
      this._tabControl.Controls.Add(this._tpMakeFit);
      this._tabControl.Controls.Add(this._tpFitEnsemble);
      this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabControl.Location = new System.Drawing.Point(0, 0);
      this._tabControl.Name = "_tabControl";
      this._tabControl.SelectedIndex = 0;
      this._tabControl.Size = new System.Drawing.Size(432, 384);
      this._tabControl.TabIndex = 0;
      // 
      // _tpSelectFunction
      // 
      this._tpSelectFunction.Controls.Add(this._btSelect);
      this._tpSelectFunction.Location = new System.Drawing.Point(4, 22);
      this._tpSelectFunction.Name = "_tpSelectFunction";
      this._tpSelectFunction.Size = new System.Drawing.Size(424, 358);
      this._tpSelectFunction.TabIndex = 0;
      this._tpSelectFunction.Text = "Select fit func";
      // 
      // _btSelect
      // 
      this._btSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btSelect.Location = new System.Drawing.Point(32, 328);
      this._btSelect.Name = "_btSelect";
      this._btSelect.TabIndex = 0;
      this._btSelect.Text = "Select";
      this._btSelect.Click += new System.EventHandler(this._btSelectFitFunc_Click);
      // 
      // _tpMakeFit
      // 
      this._tpMakeFit.Controls.Add(this._btDoFit);
      this._tpMakeFit.Location = new System.Drawing.Point(4, 22);
      this._tpMakeFit.Name = "_tpMakeFit";
      this._tpMakeFit.Size = new System.Drawing.Size(424, 358);
      this._tpMakeFit.TabIndex = 1;
      this._tpMakeFit.Text = "Fit";
      // 
      // _btDoFit
      // 
      this._btDoFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._btDoFit.Location = new System.Drawing.Point(32, 328);
      this._btDoFit.Name = "_btDoFit";
      this._btDoFit.TabIndex = 0;
      this._btDoFit.Text = "Fit!";
      this._btDoFit.Click += new System.EventHandler(this._btDoFit_Click);
      // 
      // _tpFitEnsemble
      // 
      this._tpFitEnsemble.Location = new System.Drawing.Point(4, 22);
      this._tpFitEnsemble.Name = "_tpFitEnsemble";
      this._tpFitEnsemble.Size = new System.Drawing.Size(424, 358);
      this._tpFitEnsemble.TabIndex = 2;
      this._tpFitEnsemble.Text = "Details";
      // 
      // NonlinearFitControl
      // 
      this.Controls.Add(this._tabControl);
      this.Name = "NonlinearFitControl";
      this.Size = new System.Drawing.Size(432, 384);
      this._tabControl.ResumeLayout(false);
      this._tpSelectFunction.ResumeLayout(false);
      this._tpMakeFit.ResumeLayout(false);
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
      _setParameterControl.Location = new Point(0,0);
      _setParameterControl.Size = new Size(_tpMakeFit.ClientSize.Width, this._tpMakeFit.Location.Y - this._tpMakeFit.Size.Height/2);
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
      _fitEnsembleControl.Location = new Point(0,0);
      _fitEnsembleControl.Size = new Size(this._tpSelectFunction.ClientSize.Width, _btSelect.Location.Y - _btSelect.Size.Height/2);
      _fitEnsembleControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      this._tpFitEnsemble.Controls.Add(_fitEnsembleControl);
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
  }
}
