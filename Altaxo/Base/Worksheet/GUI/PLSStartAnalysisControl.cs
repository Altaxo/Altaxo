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

using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for PLSStartAnalysisControl.
  /// </summary>
  public class PLSStartAnalysisControl : System.Windows.Forms.UserControl
  {
    PLSStartAnalysisController _controller;

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.NumericUpDown edMaxNumFactors;
    private System.Windows.Forms.RadioButton rbCrossValidationNone;
    private System.Windows.Forms.RadioButton rbCrossValidationEvery;
    private System.Windows.Forms.RadioButton rbCrossValidationGroups;
    private System.Windows.Forms.ComboBox cbAnalysisMethod;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RadioButton rbCrossValidationHalfEnsemble;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public PLSStartAnalysisControl()
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
      this.label1 = new System.Windows.Forms.Label();
      this.edMaxNumFactors = new System.Windows.Forms.NumericUpDown();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rbCrossValidationGroups = new System.Windows.Forms.RadioButton();
      this.rbCrossValidationEvery = new System.Windows.Forms.RadioButton();
      this.rbCrossValidationNone = new System.Windows.Forms.RadioButton();
      this.cbAnalysisMethod = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.rbCrossValidationHalfEnsemble = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.edMaxNumFactors)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(16, 64);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(168, 32);
      this.label1.TabIndex = 0;
      this.label1.Text = "Maximum number of factors to calculate:";
      // 
      // edMaxNumFactors
      // 
      this.edMaxNumFactors.Location = new System.Drawing.Point(16, 96);
      this.edMaxNumFactors.Name = "edMaxNumFactors";
      this.edMaxNumFactors.Size = new System.Drawing.Size(168, 20);
      this.edMaxNumFactors.TabIndex = 1;
      this.edMaxNumFactors.Validated += new System.EventHandler(this.edMaxNumFactors_Validated);
      this.edMaxNumFactors.ValueChanged += new System.EventHandler(this.edMaxNumFactors_ValueChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.rbCrossValidationHalfEnsemble);
      this.groupBox1.Controls.Add(this.rbCrossValidationGroups);
      this.groupBox1.Controls.Add(this.rbCrossValidationEvery);
      this.groupBox1.Controls.Add(this.rbCrossValidationNone);
      this.groupBox1.Location = new System.Drawing.Point(16, 128);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(168, 144);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Cross validation calculation:";
      // 
      // rbCrossValidationGroups
      // 
      this.rbCrossValidationGroups.Location = new System.Drawing.Point(8, 72);
      this.rbCrossValidationGroups.Name = "rbCrossValidationGroups";
      this.rbCrossValidationGroups.Size = new System.Drawing.Size(136, 32);
      this.rbCrossValidationGroups.TabIndex = 2;
      this.rbCrossValidationGroups.Text = "Exclude groups of similar measurements";
      this.rbCrossValidationGroups.CheckedChanged += new System.EventHandler(this.rbCrossValidationGroups_CheckedChanged);
      // 
      // rbCrossValidationEvery
      // 
      this.rbCrossValidationEvery.Location = new System.Drawing.Point(8, 40);
      this.rbCrossValidationEvery.Name = "rbCrossValidationEvery";
      this.rbCrossValidationEvery.Size = new System.Drawing.Size(144, 32);
      this.rbCrossValidationEvery.TabIndex = 1;
      this.rbCrossValidationEvery.Text = "Exclude every measurement";
      this.rbCrossValidationEvery.CheckedChanged += new System.EventHandler(this.rbCrossValidationEvery_CheckedChanged);
      // 
      // rbCrossValidationNone
      // 
      this.rbCrossValidationNone.Location = new System.Drawing.Point(8, 16);
      this.rbCrossValidationNone.Name = "rbCrossValidationNone";
      this.rbCrossValidationNone.TabIndex = 0;
      this.rbCrossValidationNone.Text = "None";
      this.rbCrossValidationNone.CheckedChanged += new System.EventHandler(this.rbCrossValidationNone_CheckedChanged);
      // 
      // cbAnalysisMethod
      // 
      this.cbAnalysisMethod.Location = new System.Drawing.Point(16, 32);
      this.cbAnalysisMethod.Name = "cbAnalysisMethod";
      this.cbAnalysisMethod.Size = new System.Drawing.Size(168, 21);
      this.cbAnalysisMethod.TabIndex = 3;
      this.cbAnalysisMethod.Text = "comboBox1";
      this.cbAnalysisMethod.SelectionChangeCommitted += new System.EventHandler(this.cbAnalysisMethod_SelectionChangeCommitted);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 16);
      this.label2.TabIndex = 4;
      this.label2.Text = "Method:";
      // 
      // rbCrossValidationHalfEnsemble
      // 
      this.rbCrossValidationHalfEnsemble.Location = new System.Drawing.Point(8, 104);
      this.rbCrossValidationHalfEnsemble.Name = "rbCrossValidationHalfEnsemble";
      this.rbCrossValidationHalfEnsemble.Size = new System.Drawing.Size(136, 32);
      this.rbCrossValidationHalfEnsemble.TabIndex = 3;
      this.rbCrossValidationHalfEnsemble.Text = "Exclude half ensembly of measurements";
      this.rbCrossValidationHalfEnsemble.CheckedChanged += new System.EventHandler(this.rbCrossValidationHalfEnsemble_CheckedChanged);
      // 
      // PLSStartAnalysisControl
      // 
      this.Controls.Add(this.label2);
      this.Controls.Add(this.cbAnalysisMethod);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.edMaxNumFactors);
      this.Controls.Add(this.label1);
      this.Name = "PLSStartAnalysisControl";
      this.Size = new System.Drawing.Size(192, 280);
      ((System.ComponentModel.ISupportInitialize)(this.edMaxNumFactors)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion


    public PLSStartAnalysisController Controller
    {
      set { _controller = value; }
    }


    public void InitializeNumberOfFactors(int numFactors)
    {
      edMaxNumFactors.Minimum = 1;
      edMaxNumFactors.Value = numFactors;
    }

    public void InitializeAnalysisMethod(string[] methods, int actMethod)
    {
      cbAnalysisMethod.Items.Clear();
      cbAnalysisMethod.Items.AddRange(methods);
      cbAnalysisMethod.SelectedIndex = actMethod;
    }

    public void InitializeCrossPressCalculation(CrossPRESSCalculationType val)
    {
      switch(val)
      {
        case CrossPRESSCalculationType.None:
          rbCrossValidationNone.Checked = true;
          break;
        case CrossPRESSCalculationType.ExcludeEveryMeasurement:
          rbCrossValidationEvery.Checked = true;
          break;
        case CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements:
          rbCrossValidationGroups.Checked = true;
          break;
        case CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements:
          this.rbCrossValidationHalfEnsemble.Checked = true;
          break;
      }
    }


    private void edMaxNumFactors_ValueChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_MaxNumberOfFactorsChanged((int)this.edMaxNumFactors.Value);
    }

    private void edMaxNumFactors_Validated(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_MaxNumberOfFactorsChanged((int)this.edMaxNumFactors.Value);
    }
    private void rbCrossValidationNone_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_CrossValidationSelected(CrossPRESSCalculationType.None);
    }

    private void rbCrossValidationEvery_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_CrossValidationSelected(CrossPRESSCalculationType.ExcludeEveryMeasurement);
    
    }

    private void rbCrossValidationGroups_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_CrossValidationSelected(CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements);

    }

    private void rbCrossValidationHalfEnsemble_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_CrossValidationSelected(CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements);
    
    }


    private void cbAnalysisMethod_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_AnalysisMethodChanged(cbAnalysisMethod.SelectedIndex);
    }


 
  }
}
