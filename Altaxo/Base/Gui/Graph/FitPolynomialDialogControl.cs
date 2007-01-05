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

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for FitPolynomialDialogControl.
  /// </summary>
  public class FitPolynomialDialogControl : System.Windows.Forms.UserControl, IFitPolynomialDialogControl
  {
    private IFitPolynomialDialogController m_Ctrl;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox _edOrder;
    private System.Windows.Forms.TextBox _edFitCurveXmin;
    private System.Windows.Forms.TextBox _edFitCurveXmax;
    private System.Windows.Forms.CheckBox _chkShowFormulaOnGraph;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public FitPolynomialDialogControl()
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
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this._edOrder = new System.Windows.Forms.TextBox();
      this._edFitCurveXmin = new System.Windows.Forms.TextBox();
      this._edFitCurveXmax = new System.Windows.Forms.TextBox();
      this._chkShowFormulaOnGraph = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
      this.label1.Location = new System.Drawing.Point(16, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(100, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Order (1=linear)";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 40);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Fit curve Xmin";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(16, 72);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(100, 16);
      this.label3.TabIndex = 2;
      this.label3.Text = "Fit curve Xmax";
      // 
      // _edOrder
      // 
      this._edOrder.Location = new System.Drawing.Point(144, 8);
      this._edOrder.Name = "_edOrder";
      this._edOrder.TabIndex = 3;
      this._edOrder.Text = "";
      // 
      // _edFitCurveXmin
      // 
      this._edFitCurveXmin.Location = new System.Drawing.Point(144, 40);
      this._edFitCurveXmin.Name = "_edFitCurveXmin";
      this._edFitCurveXmin.TabIndex = 4;
      this._edFitCurveXmin.Text = "textBox2";
      // 
      // _edFitCurveXmax
      // 
      this._edFitCurveXmax.Location = new System.Drawing.Point(144, 72);
      this._edFitCurveXmax.Name = "_edFitCurveXmax";
      this._edFitCurveXmax.TabIndex = 5;
      this._edFitCurveXmax.Text = "textBox3";
      // 
      // _chkShowFormulaOnGraph
      // 
      this._chkShowFormulaOnGraph.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this._chkShowFormulaOnGraph.Location = new System.Drawing.Point(16, 104);
      this._chkShowFormulaOnGraph.Name = "_chkShowFormulaOnGraph";
      this._chkShowFormulaOnGraph.Size = new System.Drawing.Size(144, 16);
      this._chkShowFormulaOnGraph.TabIndex = 6;
      this._chkShowFormulaOnGraph.Text = "Show formula on graph";
      // 
      // FitPolynomialDialogControl
      // 
      this.Controls.Add(this._chkShowFormulaOnGraph);
      this.Controls.Add(this._edFitCurveXmax);
      this.Controls.Add(this._edFitCurveXmin);
      this.Controls.Add(this._edOrder);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "FitPolynomialDialogControl";
      this.Size = new System.Drawing.Size(256, 136);
      this.ResumeLayout(false);

    }
    #endregion

    #region IFitPolynomialDialogControl Members

    public IFitPolynomialDialogController Controller
    {
      get
      {
        return m_Ctrl;
      }
      set
      {
        m_Ctrl = value;
      }
    }

    public Form Form
    {
      get
      {
        return this.ParentForm;
      }
    }

    public int Order 
    { 
      get 
      {
        int result=1;
        try
        {
          result = int.Parse(_edOrder.Text);
        }
        catch(Exception) 
        {
        }
        return result;
      }
      set
      {
        _edOrder.Text = value.ToString();
      }
    }
    public double FitCurveXmin 
    {
      get
      {
        double result;
        if(Altaxo.Serialization.NumberConversion.IsDouble(_edFitCurveXmin.Text,out result))
          return result;
        else return double.MinValue;
      }
      set
      {
        _edFitCurveXmin.Text = Altaxo.Serialization.NumberConversion.ToString(value);
      }
    }
    public double FitCurveXmax
    {
      get
      {
        double result;
        if(Altaxo.Serialization.NumberConversion.IsDouble(_edFitCurveXmax.Text,out result))
          return result;
        else return double.MaxValue;
      }
      set
      {
        _edFitCurveXmax.Text = Altaxo.Serialization.NumberConversion.ToString(value);
      }
    }

    public bool ShowFormulaOnGraph 
    {
      get
      {
        return _chkShowFormulaOnGraph.Checked;
      }
      set
      {
        _chkShowFormulaOnGraph.Checked = value;
      }
    }


    #endregion

    #region IMVCView Members

    public object ControllerObject
    {
      get
      {
        return Controller;
      }
      set
      {
        Controller = value as IFitPolynomialDialogController;
      }
    }

    #endregion
  }
}
