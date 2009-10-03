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

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Provides a control where you can enter minimum, maximum or a number between minimum and maximum of an integer number.
  /// </summary>
  public class MinMaxIntegerControl : System.Windows.Forms.UserControl
  {
    private System.Windows.Forms.RadioButton _rbMin;
    private System.Windows.Forms.RadioButton _rbMax;
    private System.Windows.Forms.RadioButton _rbEnterNumber;
    private System.Windows.Forms.NumericUpDown _edNumber;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    /// <summary>
    /// default constructor.
    /// </summary>
    public MinMaxIntegerControl()
      : this(0,int.MaxValue,0)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="min">The minimal allowed number to enter.</param>
    /// <param name="max">The maximal allowed number to enter.</param>
    /// <param name="startvalue">The starting value (default value).</param>
    public MinMaxIntegerControl(int min, int max, int startvalue)
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      _Minimum = min;
      _Maximum = max;
      _Value = startvalue;
      _StartValue = startvalue;

      _edNumber.Minimum = _Minimum;
      _edNumber.Maximum = _Maximum;

      if(startvalue==min)
        _rbMin.Checked=true;
      else if(startvalue==max)
        _rbMax.Checked=true;
      else
        _rbEnterNumber.Checked=true;

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
      this._rbMin = new System.Windows.Forms.RadioButton();
      this._rbMax = new System.Windows.Forms.RadioButton();
      this._rbEnterNumber = new System.Windows.Forms.RadioButton();
      this._edNumber = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this._edNumber)).BeginInit();
      this.SuspendLayout();
      // 
      // _rbMin
      // 
      this._rbMin.Location = new System.Drawing.Point(0, 0);
      this._rbMin.Name = "_rbMin";
      this._rbMin.TabIndex = 0;
      this._rbMin.Text = "Min";
      this._rbMin.CheckedChanged += new System.EventHandler(this._rbMin_CheckedChanged);
      // 
      // _rbMax
      // 
      this._rbMax.Location = new System.Drawing.Point(0, 24);
      this._rbMax.Name = "_rbMax";
      this._rbMax.TabIndex = 1;
      this._rbMax.Text = "Max";
      this._rbMax.CheckedChanged += new System.EventHandler(this._rbMax_CheckedChanged);
      // 
      // _rbEnterNumber
      // 
      this._rbEnterNumber.Location = new System.Drawing.Point(0, 48);
      this._rbEnterNumber.Name = "_rbEnterNumber";
      this._rbEnterNumber.TabIndex = 2;
      this._rbEnterNumber.Text = "Number:";
      this._rbEnterNumber.CheckedChanged += new System.EventHandler(this._rbEnterNumber_CheckedChanged);
      // 
      // _edNumber
      // 
      this._edNumber.Location = new System.Drawing.Point(0, 72);
      this._edNumber.Name = "_edNumber";
      this._edNumber.Size = new System.Drawing.Size(104, 20);
      this._edNumber.TabIndex = 3;
      this._edNumber.Validating += new System.ComponentModel.CancelEventHandler(this._edNumber_Validating);
      this._edNumber.ValueChanged += new System.EventHandler(this._edNumber_ValueChanged);
      // 
      // MinMaxIntegerControl
      // 
      this.Controls.Add(this._edNumber);
      this.Controls.Add(this._rbEnterNumber);
      this.Controls.Add(this._rbMax);
      this.Controls.Add(this._rbMin);
      this.Name = "MinMaxIntegerControl";
      this.Size = new System.Drawing.Size(104, 96);
      ((System.ComponentModel.ISupportInitialize)(this._edNumber)).EndInit();
      this.ResumeLayout(false);

    }
    #endregion

    int _Minimum;
    int _Maximum;
    int _Value;
    int _StartValue;

    private void _rbMin_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_rbMin.Checked)
      {
        _edNumber.Visible=false;
        _Value = _Minimum;
      }
    }

    private void _rbMax_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_rbMax.Checked)
      {
        _edNumber.Visible=false;
        _Value = _Maximum;
      }
    }

    private void _rbEnterNumber_CheckedChanged(object sender, System.EventArgs e)
    {
      if(_rbEnterNumber.Checked)
      {
        _edNumber.Visible=true;
        _Value = _StartValue;
        _edNumber.Value = _Value;
      }
    }

    private void _edNumber_ValueChanged(object sender, System.EventArgs e)
    {
      _Value = _StartValue = (int)_edNumber.Value;
    }

    private void _edNumber_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_rbEnterNumber.Checked)
      {
        _Value = _StartValue = (int)_edNumber.Value;
      }
    }

    /// <summary>
    /// Get/sets the value that the user entered.
    /// </summary>
    public int Value 
    {
      get 
      {
        return _Value; 
      }
      set 
      {
        _Value =  _StartValue = value;
        _edNumber.Value = value;

        if(_Value==_Minimum)
          _rbMin.Checked=true;
        else if(_Value==_Maximum)
          _rbMax.Checked=true;
        else
          _rbEnterNumber.Checked=true;
      }
    }

    /// <summary>
    /// Get/sets the minimum value allowed.
    /// </summary>
    public int Minimum
    {
      get { return _Minimum; }
      set
      {
        _Minimum = value;
        _edNumber.Minimum = value;
        this.Value = _Value;
      }
    }

    /// <summary>
    /// Get/sets the maximum value allowed.
    /// </summary>
    public int Maximum
    {
      get { return _Maximum; }
      set
      {
        _Maximum = value;
        _edNumber.Maximum = value;
        this.Value = _Value;
      }
    }

    /// <summary>
    /// Sets the label for the minimum radio button.
    /// </summary>
    public string MinLabel
    {
      set { _rbMin.Text = value; }
    }

    /// <summary>
    /// Sets the label for the maximum radio button.
    /// </summary>
    public string MaxLabel
    {
      set { _rbMax.Text = value; }
    }

    /// <summary>
    /// Sets the label for the Enter number radio button.
    /// </summary>
    public string EnterNumberLabel
    {
      set { _rbEnterNumber.Text = value; }
    }
  }
}
