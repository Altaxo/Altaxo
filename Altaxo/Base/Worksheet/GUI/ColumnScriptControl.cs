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

#if false
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Data;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for ColumnScriptControl.
  /// </summary>
  public class ColumnScriptControl : System.Windows.Forms.UserControl, IColumnScriptView
  {
    private System.Windows.Forms.TextBox edCodeHead;
    private System.Windows.Forms.TextBox edRowCondition;
    private System.Windows.Forms.TextBox edRowInc;
    private System.Windows.Forms.TextBox edFormula;
    private System.Windows.Forms.TextBox edRowTo;
    private System.Windows.Forms.TextBox edRowFrom;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lCodeStart;
    private System.Windows.Forms.Label lCodeTail;
    private System.Windows.Forms.ListBox lbCompilerErrors;
    private System.Windows.Forms.Button btUpdate;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox grpStyle;
    private System.Windows.Forms.RadioButton rbStyleFree;
    private System.Windows.Forms.RadioButton rbStyleSetColValues;
    private System.Windows.Forms.RadioButton rbStyleSetCol;
    private System.Windows.Forms.Button btDoIt;
    private System.Windows.Forms.Label label1;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;
    private System.Windows.Forms.Button btCompile;

    private IColumnScriptController m_Controller;

    public ColumnScriptControl()
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
      this.edCodeHead = new System.Windows.Forms.TextBox();
      this.edRowCondition = new System.Windows.Forms.TextBox();
      this.edRowInc = new System.Windows.Forms.TextBox();
      this.edFormula = new System.Windows.Forms.TextBox();
      this.edRowTo = new System.Windows.Forms.TextBox();
      this.edRowFrom = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.lCodeStart = new System.Windows.Forms.Label();
      this.lCodeTail = new System.Windows.Forms.Label();
      this.lbCompilerErrors = new System.Windows.Forms.ListBox();
      this.btUpdate = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.grpStyle = new System.Windows.Forms.GroupBox();
      this.rbStyleFree = new System.Windows.Forms.RadioButton();
      this.rbStyleSetColValues = new System.Windows.Forms.RadioButton();
      this.rbStyleSetCol = new System.Windows.Forms.RadioButton();
      this.btDoIt = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.btCompile = new System.Windows.Forms.Button();
      this.grpStyle.SuspendLayout();
      this.SuspendLayout();
      // 
      // edCodeHead
      // 
      this.edCodeHead.AcceptsReturn = true;
      this.edCodeHead.AcceptsTab = true;
      this.edCodeHead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.edCodeHead.Location = new System.Drawing.Point(152, 57);
      this.edCodeHead.Multiline = true;
      this.edCodeHead.Name = "edCodeHead";
      this.edCodeHead.ReadOnly = true;
      this.edCodeHead.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.edCodeHead.Size = new System.Drawing.Size(376, 80);
      this.edCodeHead.TabIndex = 35;
      this.edCodeHead.Text = "";
      this.edCodeHead.WordWrap = false;
      // 
      // edRowCondition
      // 
      this.edRowCondition.Location = new System.Drawing.Point(288, 33);
      this.edRowCondition.Name = "edRowCondition";
      this.edRowCondition.Size = new System.Drawing.Size(24, 20);
      this.edRowCondition.TabIndex = 34;
      this.edRowCondition.Text = "<";
      this.edRowCondition.TextChanged += new System.EventHandler(this.EhTextChanged_RowCondition);
      // 
      // edRowInc
      // 
      this.edRowInc.Location = new System.Drawing.Point(440, 33);
      this.edRowInc.Name = "edRowInc";
      this.edRowInc.Size = new System.Drawing.Size(56, 20);
      this.edRowInc.TabIndex = 33;
      this.edRowInc.Text = "++";
      this.edRowInc.TextChanged += new System.EventHandler(this.EhTextChanged_RowInc);
      // 
      // edFormula
      // 
      this.edFormula.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.edFormula.Location = new System.Drawing.Point(16, 145);
      this.edFormula.Multiline = true;
      this.edFormula.Name = "edFormula";
      this.edFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.edFormula.Size = new System.Drawing.Size(512, 128);
      this.edFormula.TabIndex = 23;
      this.edFormula.Text = "";
      // 
      // edRowTo
      // 
      this.edRowTo.Location = new System.Drawing.Point(312, 33);
      this.edRowTo.Name = "edRowTo";
      this.edRowTo.Size = new System.Drawing.Size(112, 20);
      this.edRowTo.TabIndex = 22;
      this.edRowTo.Tag = "nRowTo";
      this.edRowTo.Text = "0";
      this.edRowTo.TextChanged += new System.EventHandler(this.EhTextChanged_RowTo);
      // 
      // edRowFrom
      // 
      this.edRowFrom.Location = new System.Drawing.Point(200, 33);
      this.edRowFrom.Name = "edRowFrom";
      this.edRowFrom.Size = new System.Drawing.Size(64, 20);
      this.edRowFrom.TabIndex = 20;
      this.edRowFrom.Text = "0";
      this.edRowFrom.TextChanged += new System.EventHandler(this.EhTextChanged_RowFrom);
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(424, 33);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(16, 16);
      this.label3.TabIndex = 32;
      this.label3.Text = "; i";
      this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
      // 
      // lCodeStart
      // 
      this.lCodeStart.Location = new System.Drawing.Point(16, 121);
      this.lCodeStart.Name = "lCodeStart";
      this.lCodeStart.Size = new System.Drawing.Size(136, 24);
      this.lCodeStart.TabIndex = 31;
      // 
      // lCodeTail
      // 
      this.lCodeTail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lCodeTail.Location = new System.Drawing.Point(24, 281);
      this.lCodeTail.Name = "lCodeTail";
      this.lCodeTail.Size = new System.Drawing.Size(504, 40);
      this.lCodeTail.TabIndex = 30;
      // 
      // lbCompilerErrors
      // 
      this.lbCompilerErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lbCompilerErrors.HorizontalExtent = 4096;
      this.lbCompilerErrors.HorizontalScrollbar = true;
      this.lbCompilerErrors.Location = new System.Drawing.Point(16, 329);
      this.lbCompilerErrors.Name = "lbCompilerErrors";
      this.lbCompilerErrors.Size = new System.Drawing.Size(576, 82);
      this.lbCompilerErrors.TabIndex = 29;
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
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(152, 33);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(48, 16);
      this.label2.TabIndex = 26;
      this.label2.Text = "for i=";
      this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
      // 
      // grpStyle
      // 
      this.grpStyle.Controls.Add(this.rbStyleFree);
      this.grpStyle.Controls.Add(this.rbStyleSetColValues);
      this.grpStyle.Controls.Add(this.rbStyleSetCol);
      this.grpStyle.Location = new System.Drawing.Point(8, 17);
      this.grpStyle.Name = "grpStyle";
      this.grpStyle.Size = new System.Drawing.Size(144, 88);
      this.grpStyle.TabIndex = 25;
      this.grpStyle.TabStop = false;
      this.grpStyle.Text = "Style";
      // 
      // rbStyleFree
      // 
      this.rbStyleFree.Location = new System.Drawing.Point(16, 64);
      this.rbStyleFree.Name = "rbStyleFree";
      this.rbStyleFree.Size = new System.Drawing.Size(104, 16);
      this.rbStyleFree.TabIndex = 2;
      this.rbStyleFree.Text = "Free Style";
      this.rbStyleFree.CheckedChanged += new System.EventHandler(this.EhStyleFree_CheckedChanged);
      // 
      // rbStyleSetColValues
      // 
      this.rbStyleSetColValues.Location = new System.Drawing.Point(16, 16);
      this.rbStyleSetColValues.Name = "rbStyleSetColValues";
      this.rbStyleSetColValues.Size = new System.Drawing.Size(120, 16);
      this.rbStyleSetColValues.TabIndex = 1;
      this.rbStyleSetColValues.Text = "Set Column Values";
      this.rbStyleSetColValues.CheckedChanged += new System.EventHandler(this.EhStyleSetColValues_CheckedChanged);
      // 
      // rbStyleSetCol
      // 
      this.rbStyleSetCol.Location = new System.Drawing.Point(16, 40);
      this.rbStyleSetCol.Name = "rbStyleSetCol";
      this.rbStyleSetCol.Size = new System.Drawing.Size(104, 16);
      this.rbStyleSetCol.TabIndex = 0;
      this.rbStyleSetCol.Text = "Set Column";
      this.rbStyleSetCol.CheckedChanged += new System.EventHandler(this.EhStyleSetCol_CheckedChanged);
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
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(272, 33);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(16, 16);
      this.label1.TabIndex = 21;
      this.label1.Text = "; i";
      this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
      // 
      // btCompile
      // 
      this.btCompile.Location = new System.Drawing.Point(536, 80);
      this.btCompile.Name = "btCompile";
      this.btCompile.Size = new System.Drawing.Size(56, 32);
      this.btCompile.TabIndex = 36;
      this.btCompile.Text = "Compile";
      this.btCompile.Click += new System.EventHandler(this.EhCompile_Click);
      // 
      // ColumnScriptControl
      // 
      this.Controls.Add(this.btCompile);
      this.Controls.Add(this.edCodeHead);
      this.Controls.Add(this.edRowCondition);
      this.Controls.Add(this.edRowInc);
      this.Controls.Add(this.edFormula);
      this.Controls.Add(this.edRowTo);
      this.Controls.Add(this.edRowFrom);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.lCodeStart);
      this.Controls.Add(this.lCodeTail);
      this.Controls.Add(this.lbCompilerErrors);
      this.Controls.Add(this.btUpdate);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.grpStyle);
      this.Controls.Add(this.btDoIt);
      this.Controls.Add(this.label1);
      this.Name = "ColumnScriptControl";
      this.Size = new System.Drawing.Size(600, 428);
      this.grpStyle.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion


    public IColumnScriptController Controller
    {
      get { return m_Controller; }
      set { m_Controller = value; }
    }
  
    public void EnableRowFrom(bool bEnab)
    {
      this.edRowFrom.Enabled = bEnab;
    }
    public void EnableRowCondition(bool bEnab)
    {
      this.edRowCondition.Enabled =  bEnab;
    }
    public void EnableRowTo(bool bEnab)
    {
      this.edRowTo.Enabled =  bEnab;
    }
    public void EnableRowInc(bool bEnab)
    {
      this.edRowInc.Enabled =  bEnab;
    }

    public string RowFromText
    {
      set { this.edRowFrom.Text = value; }
    }
    public string RowConditionText
    {
      set { this.edRowCondition.Text = value; }
    }
    public string RowToText
    {
      set { this.edRowTo.Text = value; }
    }
    public string RowIncText
    {
      set { this.edRowInc.Text = value; }
    }
    public string FormulaText
    {
      get { return this.edFormula.Text; }
      set { this.edFormula.Text = value; }
    }
    public string CodeHeadText
    {
      set { this.edCodeHead.Text = value; }
    } 
    public string CodeStartText
    {
      set { this.lCodeStart.Text = value; }
    }
    public string CodeTailText
    {
      set { this.lCodeTail.Text = value; }
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

    public void InitializeScriptStyle(Altaxo.Data.ColumnScript.ScriptStyle style)
    {
      rbStyleSetColValues.Checked = style==ColumnScript.ScriptStyle.SetColumnValues;
      rbStyleSetCol.Checked = style==ColumnScript.ScriptStyle.SetColumn;
      rbStyleFree.Checked = style==ColumnScript.ScriptStyle.FreeStyle;
    }
  
    private void EhStyleSetColValues_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller && this.rbStyleSetColValues.Checked)
        Controller.EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle.SetColumnValues);
    }

    private void EhStyleSetCol_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller && this.rbStyleSetCol.Checked)
        Controller.EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle.SetColumn);
    }

    private void EhStyleFree_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=Controller && this.rbStyleFree.Checked)
        Controller.EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle.FreeStyle);
    }

    private void EhTextChanged_RowFrom(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_TextChanged_RowFrom(this.edRowFrom.Text);
    }

    private void EhTextChanged_RowTo(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_TextChanged_RowTo(this.edRowTo.Text);
    }

    private void EhTextChanged_RowInc(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_TextChanged_RowInc(this.edRowInc.Text);
    }

    private void EhTextChanged_RowCondition(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_TextChanged_RowCondition(this.edRowCondition.Text);
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
  }
}
#endif