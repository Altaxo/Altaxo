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

using Altaxo.Data;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Summary description for TableScriptControl.
  /// </summary>
  public class SDTableScriptControl : System.Windows.Forms.UserControl, ITableScriptView
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

    private ITableScriptController m_Controller;

    public SDTableScriptControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call

      this.ScriptName = System.Guid.NewGuid().ToString() + ".cs";
      this.edFormula.Document.TextEditorProperties.TabIndent=2;
      this.edFormulaWrapper.textAreaControl.InitializeFormatter();
      this.edFormulaWrapper.textAreaControl.TextEditorProperties.MouseWheelScrollDown=true;
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
      this.SuspendLayout();
      // 
      // edFormula
      // 
      this.edFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.edFormula.Location = new System.Drawing.Point(0, 0);
      //this.edFormula.Multiline = true;
      this.edFormula.Name = "edFormula";
      //this.edFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.edFormula.Size = new System.Drawing.Size(528, 336);
      this.edFormula.TabIndex = 23;
      this.edFormula.Text = "";
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
      // TableScriptControl
      // 
      this.Controls.Add(this.btCompile);
      this.Controls.Add(this.edFormula);
      this.Controls.Add(this.lbCompilerErrors);
      this.Controls.Add(this.btUpdate);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btDoIt);
      this.Name = "TableScriptControl";
      this.Size = new System.Drawing.Size(600, 428);
      this.ResumeLayout(false);

    }
    #endregion


    public ITableScriptController Controller
    {
      get { return m_Controller; }
      set { m_Controller = value; }
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


  }
}
