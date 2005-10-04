#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

using Altaxo.Data;
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Summary description for ParametrizedFunctionScriptControl.
  /// </summary>
  [UserControlForController(typeof(IFitFunctionScriptViewEventSink))]
  public class FitFunctionScriptControl : System.Windows.Forms.UserControl, IFitFunctionScriptView
  {

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox _cbNumberOfParameters;
    private System.Windows.Forms.CheckBox _chkUserDefinedParameters;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox _edParameterNames;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox _edIndependentVariables;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox _edDependentVariables;
    private System.Windows.Forms.Panel _panelScriptText;

    private IFitFunctionScriptViewEventSink m_Controller;

    public FitFunctionScriptControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

   

      this.InitializeNumberOfParameters();
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
      this.label1 = new System.Windows.Forms.Label();
      this._cbNumberOfParameters = new System.Windows.Forms.ComboBox();
      this._chkUserDefinedParameters = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this._edParameterNames = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this._edIndependentVariables = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this._edDependentVariables = new System.Windows.Forms.TextBox();
      this._panelScriptText = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(128, 16);
      this.label1.TabIndex = 37;
      this.label1.Text = "Number of Parameters:";
      // 
      // _cbNumberOfParameters
      // 
      this._cbNumberOfParameters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbNumberOfParameters.Location = new System.Drawing.Point(136, 3);
      this._cbNumberOfParameters.Name = "_cbNumberOfParameters";
      this._cbNumberOfParameters.Size = new System.Drawing.Size(80, 21);
      this._cbNumberOfParameters.TabIndex = 38;
      this._cbNumberOfParameters.SelectionChangeCommitted += new System.EventHandler(this._cbNumberOfParameters_SelectionChangeCommitted);
      // 
      // _chkUserDefinedParameters
      // 
      this._chkUserDefinedParameters.Location = new System.Drawing.Point(224, 8);
      this._chkUserDefinedParameters.Name = "_chkUserDefinedParameters";
      this._chkUserDefinedParameters.Size = new System.Drawing.Size(152, 16);
      this._chkUserDefinedParameters.TabIndex = 39;
      this._chkUserDefinedParameters.Text = "User defined parameters";
      this._chkUserDefinedParameters.CheckedChanged += new System.EventHandler(this._chkUserDefinedParameters_CheckedChanged);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 16);
      this.label2.TabIndex = 40;
      this.label2.Text = "Parameter names:";
      // 
      // _edParameterNames
      // 
      this._edParameterNames.Location = new System.Drawing.Point(136, 32);
      this._edParameterNames.Name = "_edParameterNames";
      this._edParameterNames.Size = new System.Drawing.Size(384, 20);
      this._edParameterNames.TabIndex = 41;
      this._edParameterNames.Text = "textBox1";
      this._edParameterNames.Validating += new System.ComponentModel.CancelEventHandler(this._edParameterNames_TextChanged);
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(8, 56);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(72, 16);
      this.label3.TabIndex = 42;
      this.label3.Text = "Ind.Var.";
      // 
      // _edIndependentVariables
      // 
      this._edIndependentVariables.Location = new System.Drawing.Point(136, 56);
      this._edIndependentVariables.Name = "_edIndependentVariables";
      this._edIndependentVariables.Size = new System.Drawing.Size(120, 20);
      this._edIndependentVariables.TabIndex = 43;
      this._edIndependentVariables.Text = "textBox1";
      this._edIndependentVariables.Validating += new System.ComponentModel.CancelEventHandler(this._edIndependentVariables_Validating);
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(264, 56);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(64, 16);
      this.label4.TabIndex = 44;
      this.label4.Text = "Dep.Var.";
      // 
      // _edDependentVariables
      // 
      this._edDependentVariables.Location = new System.Drawing.Point(320, 56);
      this._edDependentVariables.Name = "_edDependentVariables";
      this._edDependentVariables.Size = new System.Drawing.Size(200, 20);
      this._edDependentVariables.TabIndex = 45;
      this._edDependentVariables.Text = "textBox2";
      this._edDependentVariables.Validating += new System.ComponentModel.CancelEventHandler(this._edDependentVariables_Validating);
      // 
      // _panelScriptText
      // 
      this._panelScriptText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this._panelScriptText.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this._panelScriptText.Location = new System.Drawing.Point(0, 80);
      this._panelScriptText.Name = "_panelScriptText";
      this._panelScriptText.Size = new System.Drawing.Size(600, 344);
      this._panelScriptText.TabIndex = 46;
      // 
      // ParametrizedFunctionScriptControl
      // 
      this.Controls.Add(this._panelScriptText);
      this.Controls.Add(this._edDependentVariables);
      this.Controls.Add(this.label4);
      this.Controls.Add(this._edIndependentVariables);
      this.Controls.Add(this.label3);
      this.Controls.Add(this._edParameterNames);
      this.Controls.Add(this.label2);
      this.Controls.Add(this._chkUserDefinedParameters);
      this.Controls.Add(this._cbNumberOfParameters);
      this.Controls.Add(this.label1);
      this.Name = "ParametrizedFunctionScriptControl";
      this.Size = new System.Drawing.Size(600, 428);
      this.ResumeLayout(false);

    }
    #endregion


    public IFitFunctionScriptViewEventSink Controller
    {
      get { return m_Controller; }
      set { m_Controller = value; }
    }
  
    public void SetParameterText(string text, bool enable)
    {
      this._edParameterNames.Text = text;
      this._edParameterNames.Enabled = enable;
    }

    public void SetIndependentVariableText(string text)
    {
      this._edIndependentVariables.Text = text;
    }
    public void SetDependentVariableText(string text)
  {
    this._edDependentVariables.Text = text;
  }

    public void InitializeNumberOfParameters()
    {
      this._cbNumberOfParameters.BeginUpdate();
      this._cbNumberOfParameters.Items.Clear();
      for(int i=0;i<100;i++)
        this._cbNumberOfParameters.Items.Add(i.ToString());

      this._cbNumberOfParameters.EndUpdate();
    }

    public void SetCheckUseUserDefinedParameters(bool useUserDefParameters)
    {
      IFitFunctionScriptViewEventSink tempcontroller = m_Controller; // trick to suppress changed event
      m_Controller = null;

      this._chkUserDefinedParameters.Checked = useUserDefParameters;
      m_Controller = tempcontroller;
    }


    public void SetNumberOfParameters(int numberOfParameters, bool enable)
    {
      this._cbNumberOfParameters.SelectedIndex = numberOfParameters;
      this._cbNumberOfParameters.Enabled = enable;
    }

  

    Control _scriptView;
    public void SetScriptView(object viewAsObject)
    {
      if(object.ReferenceEquals(_scriptView,viewAsObject) )
        return;

      if(null!=_scriptView)
      {
        this._panelScriptText.Controls.Remove(_scriptView);
      }
      _scriptView = (Control)viewAsObject;
      if(null!=_scriptView)
      {
        _scriptView.Location = new Point(0,0);
        _scriptView.Dock = DockStyle.Fill;
        this._panelScriptText.Controls.Add(_scriptView);
      }

    }
  
   
    public void Close(bool withOK)
    {
      this.ParentForm.DialogResult = withOK ? DialogResult.OK : DialogResult.Cancel;
      this.ParentForm.Close();
    }

 

   
   



  
    private void _edParameterNames_TextChanged(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_UserDefinedParameterTextChanged(this._edParameterNames.Text);
    }

    private void _cbNumberOfParameters_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if (null != Controller)
        Controller.EhView_NumberOfParameterChanged(this._cbNumberOfParameters.SelectedIndex);
    }

    private void _chkUserDefinedParameters_CheckedChanged(object sender, System.EventArgs e)
    {
      if (null != Controller)
        Controller.EhView_UserDefinedParameterCheckChanged(_chkUserDefinedParameters.Checked);

      this._cbNumberOfParameters.Enabled = !_chkUserDefinedParameters.Checked;
      this._edParameterNames.Enabled = _chkUserDefinedParameters.Checked;
    }

    private void _edIndependentVariables_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
    if(null!=Controller)
      Controller.EhView_IndependentVariableTextChanged(this._edIndependentVariables.Text);
    }

    private void _edDependentVariables_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_DependentVariableTextChanged(this._edDependentVariables.Text);
    
    }
  }
}
