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
  /// Summary description for SpectralPreprocessingControl.
  /// </summary>
  public class SpectralPreprocessingControl : System.Windows.Forms.UserControl
  {
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.RadioButton _rbMethodNone;
    private System.Windows.Forms.RadioButton _rbMethodMSC;
    private System.Windows.Forms.RadioButton _rbMethodSNV;
    private System.Windows.Forms.RadioButton _rbMethod1stDer;
    private System.Windows.Forms.RadioButton _rbMethod2ndDer;
    private System.Windows.Forms.RadioButton _rbDetrendingNone;
    private System.Windows.Forms.RadioButton _rbDetrendingZero;
    private System.Windows.Forms.RadioButton _rbDetrending1st;
    private System.Windows.Forms.RadioButton _rbDetrending2nd;
    private System.Windows.Forms.CheckBox _chkEnsembleScale;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public SpectralPreprocessingController Controller;

    public SpectralPreprocessingControl()
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this._rbMethod2ndDer = new System.Windows.Forms.RadioButton();
      this._rbMethod1stDer = new System.Windows.Forms.RadioButton();
      this._rbMethodSNV = new System.Windows.Forms.RadioButton();
      this._rbMethodMSC = new System.Windows.Forms.RadioButton();
      this._rbMethodNone = new System.Windows.Forms.RadioButton();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this._rbDetrending2nd = new System.Windows.Forms.RadioButton();
      this._rbDetrending1st = new System.Windows.Forms.RadioButton();
      this._rbDetrendingZero = new System.Windows.Forms.RadioButton();
      this._rbDetrendingNone = new System.Windows.Forms.RadioButton();
      this._chkEnsembleScale = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this._rbMethod2ndDer);
      this.groupBox1.Controls.Add(this._rbMethod1stDer);
      this.groupBox1.Controls.Add(this._rbMethodSNV);
      this.groupBox1.Controls.Add(this._rbMethodMSC);
      this.groupBox1.Controls.Add(this._rbMethodNone);
      this.groupBox1.Location = new System.Drawing.Point(8, 8);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(120, 136);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Method";
      // 
      // _rbMethod2ndDer
      // 
      this._rbMethod2ndDer.Location = new System.Drawing.Point(8, 112);
      this._rbMethod2ndDer.Name = "_rbMethod2ndDer";
      this._rbMethod2ndDer.Size = new System.Drawing.Size(104, 16);
      this._rbMethod2ndDer.TabIndex = 4;
      this._rbMethod2ndDer.Text = "2nd Derivative";
      this._rbMethod2ndDer.CheckedChanged += new System.EventHandler(this._rbMethod2ndDer_CheckedChanged);
      // 
      // _rbMethod1stDer
      // 
      this._rbMethod1stDer.Location = new System.Drawing.Point(8, 88);
      this._rbMethod1stDer.Name = "_rbMethod1stDer";
      this._rbMethod1stDer.Size = new System.Drawing.Size(104, 16);
      this._rbMethod1stDer.TabIndex = 3;
      this._rbMethod1stDer.Text = "1st Derivative";
      this._rbMethod1stDer.CheckedChanged += new System.EventHandler(this._rbMethod1stDer_CheckedChanged);
      // 
      // _rbMethodSNV
      // 
      this._rbMethodSNV.Location = new System.Drawing.Point(8, 64);
      this._rbMethodSNV.Name = "_rbMethodSNV";
      this._rbMethodSNV.Size = new System.Drawing.Size(104, 16);
      this._rbMethodSNV.TabIndex = 2;
      this._rbMethodSNV.Text = "SNV";
      this._rbMethodSNV.CheckedChanged += new System.EventHandler(this._rbMethodSNV_CheckedChanged);
      // 
      // _rbMethodMSC
      // 
      this._rbMethodMSC.Location = new System.Drawing.Point(8, 40);
      this._rbMethodMSC.Name = "_rbMethodMSC";
      this._rbMethodMSC.Size = new System.Drawing.Size(104, 16);
      this._rbMethodMSC.TabIndex = 1;
      this._rbMethodMSC.Text = "MSC";
      this._rbMethodMSC.CheckedChanged += new System.EventHandler(this._rbMethodMSC_CheckedChanged);
      // 
      // _rbMethodNone
      // 
      this._rbMethodNone.Location = new System.Drawing.Point(8, 16);
      this._rbMethodNone.Name = "_rbMethodNone";
      this._rbMethodNone.Size = new System.Drawing.Size(104, 16);
      this._rbMethodNone.TabIndex = 0;
      this._rbMethodNone.Text = "None";
      this._rbMethodNone.CheckedChanged += new System.EventHandler(this._rbMethodNone_CheckedChanged);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this._rbDetrending2nd);
      this.groupBox2.Controls.Add(this._rbDetrending1st);
      this.groupBox2.Controls.Add(this._rbDetrendingZero);
      this.groupBox2.Controls.Add(this._rbDetrendingNone);
      this.groupBox2.Location = new System.Drawing.Point(136, 8);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(128, 136);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Detrending";
      // 
      // _rbDetrending2nd
      // 
      this._rbDetrending2nd.Location = new System.Drawing.Point(16, 88);
      this._rbDetrending2nd.Name = "_rbDetrending2nd";
      this._rbDetrending2nd.Size = new System.Drawing.Size(104, 16);
      this._rbDetrending2nd.TabIndex = 3;
      this._rbDetrending2nd.Text = "quadratic";
      this._rbDetrending2nd.CheckedChanged += new System.EventHandler(this._rbDetrending2nd_CheckedChanged);
      // 
      // _rbDetrending1st
      // 
      this._rbDetrending1st.Location = new System.Drawing.Point(16, 64);
      this._rbDetrending1st.Name = "_rbDetrending1st";
      this._rbDetrending1st.Size = new System.Drawing.Size(104, 16);
      this._rbDetrending1st.TabIndex = 2;
      this._rbDetrending1st.Text = "linear";
      this._rbDetrending1st.CheckedChanged += new System.EventHandler(this._rbDetrending1st_CheckedChanged);
      // 
      // _rbDetrendingZero
      // 
      this._rbDetrendingZero.Location = new System.Drawing.Point(16, 40);
      this._rbDetrendingZero.Name = "_rbDetrendingZero";
      this._rbDetrendingZero.Size = new System.Drawing.Size(104, 16);
      this._rbDetrendingZero.TabIndex = 1;
      this._rbDetrendingZero.Text = "spectrum mean";
      this._rbDetrendingZero.CheckedChanged += new System.EventHandler(this._rbDetrendingZero_CheckedChanged);
      // 
      // _rbDetrendingNone
      // 
      this._rbDetrendingNone.Location = new System.Drawing.Point(16, 16);
      this._rbDetrendingNone.Name = "_rbDetrendingNone";
      this._rbDetrendingNone.Size = new System.Drawing.Size(104, 16);
      this._rbDetrendingNone.TabIndex = 0;
      this._rbDetrendingNone.Text = "None";
      this._rbDetrendingNone.CheckedChanged += new System.EventHandler(this._rbDetrendingNone_CheckedChanged);
      // 
      // _chkEnsembleScale
      // 
      this._chkEnsembleScale.Location = new System.Drawing.Point(8, 152);
      this._chkEnsembleScale.Name = "_chkEnsembleScale";
      this._chkEnsembleScale.Size = new System.Drawing.Size(248, 24);
      this._chkEnsembleScale.TabIndex = 2;
      this._chkEnsembleScale.Text = "Variance scale (spectral ensemble)";
      this._chkEnsembleScale.CheckedChanged += new System.EventHandler(this._chkEnsembleScale_CheckedChanged);
      // 
      // SpectralPreprocessingControl
      // 
      this.Controls.Add(this._chkEnsembleScale);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "SpectralPreprocessingControl";
      this.Size = new System.Drawing.Size(272, 176);
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion


    #region Control logic

    public void InitializeMethod(SpectralPreprocessingMethod method)
    {
      switch(method)
      {
        case SpectralPreprocessingMethod.None:
          this._rbMethodNone.Checked = true;
          break;
        case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
          this._rbMethodMSC.Checked = true;
          break;
        case SpectralPreprocessingMethod.StandardNormalVariate:
          this._rbMethodSNV.Checked = true;
          break;
        case SpectralPreprocessingMethod.FirstDerivative:
          this._rbMethod1stDer.Checked = true;
          break;
        case SpectralPreprocessingMethod.SecondDerivative:
          this._rbMethod2ndDer.Checked = true;
          break;
      }
    }

    public void InitializeDetrending(int detrending)
    {
      switch(detrending)
      {
        case 0:
          this._rbDetrendingZero.Checked = true;
          break;
        case 1:
          this._rbDetrending1st.Checked = true;
          break;
        case 2:
          this._rbDetrending2nd.Checked = true;
          break;
        default:
          this._rbDetrendingNone.Checked = true;
          break;
      }
    }

    public void InitializeEnsembleScale(bool ensScale)
    {
      this._chkEnsembleScale.Checked = ensScale;
    }


    #endregion

    #region Event logic

    private void _rbMethodNone_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbMethodNone.Checked)
        Controller.EhView_MethodChanged(SpectralPreprocessingMethod.None);
    }

    private void _rbMethodMSC_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbMethodMSC.Checked)
        Controller.EhView_MethodChanged(SpectralPreprocessingMethod.MultiplicativeScatteringCorrection);
    }

    private void _rbMethodSNV_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbMethodSNV.Checked)
        Controller.EhView_MethodChanged(SpectralPreprocessingMethod.StandardNormalVariate);
    }

    private void _rbMethod1stDer_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbMethod1stDer.Checked)
        Controller.EhView_MethodChanged(SpectralPreprocessingMethod.FirstDerivative);
    }

    private void _rbMethod2ndDer_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbMethod2ndDer.Checked)
        Controller.EhView_MethodChanged(SpectralPreprocessingMethod.SecondDerivative);
    }

    private void _rbDetrendingNone_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbDetrendingNone.Checked)
        Controller.EhView_DetrendingChanged(-1);
    }

    private void _rbDetrendingZero_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbDetrendingZero.Checked)
        Controller.EhView_DetrendingChanged(0);
    }

    private void _rbDetrending1st_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbDetrending1st.Checked)
        Controller.EhView_DetrendingChanged(1);
    }

    private void _rbDetrending2nd_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null && _rbDetrending2nd.Checked)
        Controller.EhView_DetrendingChanged(2);
    }

    private void _chkEnsembleScale_CheckedChanged(object sender, System.EventArgs e)
    {
      if(Controller!=null)
        Controller.EhView_EnsembleScaleChanged(_chkEnsembleScale.Checked);
    }

    #endregion
  }
}
