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

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for ParametrizedFunctionScriptControl.
  /// </summary>
  [UserControlForController(typeof(IParametrizedFunctionScriptViewEventSink))]
  public class ParametrizedFunctionScriptControl : System.Windows.Forms.UserControl, IParametrizedFunctionScriptView
  {
    //private System.Windows.Forms.TextBox edFormula;
    private ICSharpCode.TextEditor.TextEditorControl edFormula;
    private ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper edFormulaWrapper;
    private System.Windows.Forms.ListBox lbCompilerErrors;
    private System.Windows.Forms.Button btUpdate;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btDoIt;
    private System.Windows.Forms.Button btCompile;

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

    private IParametrizedFunctionScriptViewEventSink m_Controller;

    public ParametrizedFunctionScriptControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call

      this.ScriptName = System.Guid.NewGuid().ToString() + ".cs";
      this.edFormula.Document.TextEditorProperties.TabIndent=2;
      this.edFormulaWrapper.textAreaControl.InitializeFormatter();
      this.edFormulaWrapper.textAreaControl.TextEditorProperties.MouseWheelScrollDown=true;

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
      this.edFormulaWrapper = new ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper();
      this.edFormula = edFormulaWrapper.TextEditorControl;

      this.lbCompilerErrors = new System.Windows.Forms.ListBox();
      this.btUpdate = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.btDoIt = new System.Windows.Forms.Button();
      this.btCompile = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this._cbNumberOfParameters = new System.Windows.Forms.ComboBox();
      this._chkUserDefinedParameters = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this._edParameterNames = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this._edIndependentVariables = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this._edDependentVariables = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // lbCompilerErrors
      // 
      this.lbCompilerErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lbCompilerErrors.HorizontalExtent = 4096;
      this.lbCompilerErrors.HorizontalScrollbar = true;
      this.lbCompilerErrors.Location = new System.Drawing.Point(0, 344);
      this.lbCompilerErrors.Name = "lbCompilerErrors";
      this.lbCompilerErrors.Size = new System.Drawing.Size(592, 82);
      this.lbCompilerErrors.TabIndex = 29;
      this.lbCompilerErrors.DoubleClick += new System.EventHandler(this.EhCompilerErrors_DoubleClick);
      // 
      // btUpdate
      // 
      this.btUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btUpdate.Location = new System.Drawing.Point(536, 136);
      this.btUpdate.Name = "btUpdate";
      this.btUpdate.Size = new System.Drawing.Size(56, 32);
      this.btUpdate.TabIndex = 28;
      this.btUpdate.Text = "Update";
      this.btUpdate.Click += new System.EventHandler(this.EhUpdate_Click);
      // 
      // btCancel
      // 
      this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btCancel.Location = new System.Drawing.Point(536, 240);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(56, 32);
      this.btCancel.TabIndex = 27;
      this.btCancel.Text = "Cancel";
      this.btCancel.Click += new System.EventHandler(this.EhCancel_Click);
      // 
      // btDoIt
      // 
      this.btDoIt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btDoIt.Location = new System.Drawing.Point(536, 25);
      this.btDoIt.Name = "btDoIt";
      this.btDoIt.Size = new System.Drawing.Size(56, 32);
      this.btDoIt.TabIndex = 24;
      this.btDoIt.Text = "Do It!";
      this.btDoIt.Click += new System.EventHandler(this.EhDoIt_Click);
      // 
      // btCompile
      // 
      this.btCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btCompile.Location = new System.Drawing.Point(536, 80);
      this.btCompile.Name = "btCompile";
      this.btCompile.Size = new System.Drawing.Size(56, 32);
      this.btCompile.TabIndex = 36;
      this.btCompile.Text = "Compile";
      this.btCompile.Click += new System.EventHandler(this.EhCompile_Click);
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
      this._edParameterNames.TextChanged += new System.EventHandler(this._edParameterNames_TextChanged);
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
      // 
      // edFormula
      // 
      this.edFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.edFormula.Location = new System.Drawing.Point(0, 80);
      //this.edFormula.Multiline = true;
      this.edFormula.Name = "edFormula";
      //this.edFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.edFormula.Size = new System.Drawing.Size(528, 336-80);
      this.edFormula.TabIndex = 23;
      this.edFormula.Text = "";

      // 
      // ParametrizedFunctionScriptControl
      // 
      this.Controls.Add(this.edFormula);
      this.Controls.Add(this._edDependentVariables);
      this.Controls.Add(this.label4);
      this.Controls.Add(this._edIndependentVariables);
      this.Controls.Add(this.label3);
      this.Controls.Add(this._edParameterNames);
      this.Controls.Add(this.label2);
      this.Controls.Add(this._chkUserDefinedParameters);
      this.Controls.Add(this._cbNumberOfParameters);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btCompile);
      this.Controls.Add(this.lbCompilerErrors);
      this.Controls.Add(this.btUpdate);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btDoIt);
      this.Name = "ParametrizedFunctionScriptControl";
      this.Size = new System.Drawing.Size(600, 428);
      this.ResumeLayout(false);

    }
    #endregion


    public IParametrizedFunctionScriptViewEventSink Controller
    {
      get { return m_Controller; }
      set { m_Controller = value; }
    }
  
    public void SetParameterText(string text, bool enable)
    {
      this._edParameterNames.Text = text;
      this._edParameterNames.Enabled = enable;
    }

    public void InitializeNumberOfParameters()
    {
      this._cbNumberOfParameters.BeginUpdate();
      this._cbNumberOfParameters.Items.Clear();
      for(int i=0;i<100;i++)
        this._cbNumberOfParameters.Items.Add(i.ToString());

      this._cbNumberOfParameters.EndUpdate();
    }
    public void SetNumberOfParameters(int numberOfParameters, bool enable)
    {
      this._cbNumberOfParameters.SelectedIndex = numberOfParameters;
      this._cbNumberOfParameters.Enabled = enable;
    }

    public string ScriptText
    {
      get 
      {
        return this.edFormula.Text; 
      }
      set 
      {
        this.edFormula.Text = value;
      }
    }

    public string ScriptName
    {
      set
      {
        edFormulaWrapper.TextEditorControl.FileName = value;
        edFormulaWrapper.TitleName = value;
        edFormulaWrapper.FileName = value;
      }
    }

    public int ScriptCursorLocation
    {
      set
      {
        System.Drawing.Point point = edFormulaWrapper.textAreaControl.Document.OffsetToPosition(value);
        this.edFormulaWrapper.JumpTo(point.Y,point.X);
      }

    }

    public void SetScriptCursorLocation(int line, int column)
    {
      this.edFormulaWrapper.JumpTo(line,column);
    }

    public void MarkText(int pos1, int pos2)
    {

    }

    public object EditableContent
    {
      get
      { 
        return this.edFormulaWrapper; 
      }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
      return base.ProcessDialogKey (keyData);
    }


    public System.Windows.Forms.Form Form
    {
      get { return this.ParentForm; }
    }

    public void ClearCompilerErrors()
    {
      lbCompilerErrors.Items.Clear();
    }

    public void AddCompilerError(string s)
    {
      this.lbCompilerErrors.Items.Add(s);
    }

    private void EhDoIt_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_Execute();
    
    }

    private void EhCompile_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_Compile();
    
    }

    private void EhUpdate_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_Update();
    
    }

    private void EhCancel_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_Cancel();
    
    }

    private void EhCompilerErrors_DoubleClick(object sender, System.EventArgs e)
    {
      string msg = lbCompilerErrors.SelectedItem as string;

      if(null!=Controller && null!=msg)
        Controller.EhView_GotoCompilerError(msg);
    }

    private void _edParameterNames_TextChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_ParameterChanged(this._cbNumberOfParameters.SelectedIndex, this._chkUserDefinedParameters.Checked, this._edParameterNames.Text);
    }

    private void _cbNumberOfParameters_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
     _edParameterNames_TextChanged(sender,e);
    }

    private void _chkUserDefinedParameters_CheckedChanged(object sender, System.EventArgs e)
    {
      _edParameterNames_TextChanged(sender,e);
    }
  }
}
