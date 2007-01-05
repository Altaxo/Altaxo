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
using Altaxo.Gui;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for SavitzkyGolayParameterControl.
  /// </summary>
  [UserControlForController(typeof(ISavitzkyGolayParameterViewEventSink))]
  public class SavitzkyGolayParameterControl : System.Windows.Forms.UserControl, ISavitzkyGolayParameterView
  {


    private System.Windows.Forms.NumericUpDown _edNumberOfPoints;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown _edPolynomialOrder;
    private System.Windows.Forms.NumericUpDown _edDerivativeOrder;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ISavitzkyGolayParameterViewEventSink _controller;

    public ISavitzkyGolayParameterViewEventSink Controller
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

    public SavitzkyGolayParameterControl()
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
      this._edNumberOfPoints = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this._edPolynomialOrder = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this._edDerivativeOrder = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this._edNumberOfPoints)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this._edPolynomialOrder)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this._edDerivativeOrder)).BeginInit();
      this.SuspendLayout();
      // 
      // _edNumberOfPoints
      // 
      this._edNumberOfPoints.Increment = new System.Decimal(new int[] {
                                                                        2,
                                                                        0,
                                                                        0,
                                                                        0});
      this._edNumberOfPoints.Location = new System.Drawing.Point(8, 36);
      this._edNumberOfPoints.Maximum = new System.Decimal(new int[] {
                                                                      1000000001,
                                                                      0,
                                                                      0,
                                                                      0});
      this._edNumberOfPoints.Minimum = new System.Decimal(new int[] {
                                                                      3,
                                                                      0,
                                                                      0,
                                                                      0});
      this._edNumberOfPoints.Name = "_edNumberOfPoints";
      this._edNumberOfPoints.TabIndex = 0;
      this._edNumberOfPoints.Value = new System.Decimal(new int[] {
                                                                    7,
                                                                    0,
                                                                    0,
                                                                    0});
      this._edNumberOfPoints.Validating += new System.ComponentModel.CancelEventHandler(this._edNumberOfPoints_Validating);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(100, 20);
      this.label1.TabIndex = 1;
      this.label1.Text = "Number of points";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 72);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 20);
      this.label2.TabIndex = 2;
      this.label2.Text = "Polynomial order:";
      // 
      // _edPolynomialOrder
      // 
      this._edPolynomialOrder.Location = new System.Drawing.Point(8, 92);
      this._edPolynomialOrder.Name = "_edPolynomialOrder";
      this._edPolynomialOrder.TabIndex = 3;
      this._edPolynomialOrder.Validating += new System.ComponentModel.CancelEventHandler(this._edPolynomialOrder_Validating);
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(8, 128);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(184, 20);
      this.label3.TabIndex = 4;
      this.label3.Text = "Derivative order (0->smothing):";
      // 
      // _edDerivativeOrder
      // 
      this._edDerivativeOrder.Location = new System.Drawing.Point(8, 148);
      this._edDerivativeOrder.Name = "_edDerivativeOrder";
      this._edDerivativeOrder.TabIndex = 5;
      this._edDerivativeOrder.Validating += new System.ComponentModel.CancelEventHandler(this._edDerivativeOrder_Validating);
      // 
      // SavitzkyGolayParameterControl
      // 
      this.Controls.Add(this._edDerivativeOrder);
      this.Controls.Add(this.label3);
      this.Controls.Add(this._edPolynomialOrder);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this._edNumberOfPoints);
      this.Name = "SavitzkyGolayParameterControl";
      this.Size = new System.Drawing.Size(176, 176);
      ((System.ComponentModel.ISupportInitialize)(this._edNumberOfPoints)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this._edPolynomialOrder)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this._edDerivativeOrder)).EndInit();
      this.ResumeLayout(false);

    }
    #endregion

    #region ISavitzkyGolayParameterView Members

    public void InitializeNumberOfPoints(int val, int max)
    {
      _edNumberOfPoints.Maximum = max;
      _edNumberOfPoints.Value = val;
    }

    public void InitializeDerivativeOrder(int val, int max)
    {
      _edDerivativeOrder.Maximum = max;
      _edDerivativeOrder.Value = val;
    }

    public void InitializePolynomialOrder(int val, int max)
    {
      _edPolynomialOrder.Maximum = max;
      _edPolynomialOrder.Value = val;
    }

    public int GetNumberOfPoints()
    {
      
      return (int)_edNumberOfPoints.Value;
    }

    public int GetDerivativeOrder()
    {
      
      return (int)_edDerivativeOrder.Value;
    }

    public int GetPolynomialOrder()
    {
      
      return (int)_edPolynomialOrder.Value;
    }

    #endregion

    private void _edNumberOfPoints_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        Controller.EhValidatingNumberOfPoints((int)_edNumberOfPoints.Value);
    
    }

    private void _edPolynomialOrder_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        Controller.EhValidatingPolynomialOrder((int)_edPolynomialOrder.Value);
    
    }

    private void _edDerivativeOrder_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        Controller.EhValidatingDerivativeOrder((int)_edDerivativeOrder.Value);
    
    }
  }
}
