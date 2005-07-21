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
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for XYPlotScatterStyleControl.
  /// </summary>
  	[UserControlForController(typeof(IXYPlotScatterStyleViewEventSink))]
  public class XYPlotScatterStyleControl : System.Windows.Forms.UserControl, IXYPlotScatterStyleView
  {
    private IXYPlotScatterStyleViewEventSink _controller;
    private int m_SuppressEvents=0;
    private System.Windows.Forms.GroupBox m_gbSymbol;
    private System.Windows.Forms.TextBox m_edSymbolSkipFrequency;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox m_chkSymbolSkipPoints;
    private System.Windows.Forms.GroupBox m_gbSymbolDropLine;
    private System.Windows.Forms.CheckBox m_chkSymbolDropLineBottom;
    private System.Windows.Forms.CheckBox m_chkSymbolDropLineTop;
    private System.Windows.Forms.CheckBox m_chkSymbolDropLineRight;
    private System.Windows.Forms.CheckBox m_chkSymbolDropLineLeft;
    private System.Windows.Forms.ComboBox m_cbSymbolSize;
    private System.Windows.Forms.ComboBox m_cbSymbolStyle;
    private System.Windows.Forms.ComboBox m_cbSymbolShape;
    private System.Windows.Forms.Label m_lblSymbolSize;
    private System.Windows.Forms.Label m_lblSymbolStyle;
    private System.Windows.Forms.Label m_lblSymbolShape;
    private System.Windows.Forms.Button m_btLineSymbolColorDetails;
    private System.Windows.Forms.ComboBox m_cbLineSymbolColor;
    private System.Windows.Forms.Label m_lblLineSymbolColor;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public XYPlotScatterStyleControl()
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
      this.m_gbSymbol = new System.Windows.Forms.GroupBox();
      this.m_edSymbolSkipFrequency = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.m_chkSymbolSkipPoints = new System.Windows.Forms.CheckBox();
      this.m_gbSymbolDropLine = new System.Windows.Forms.GroupBox();
      this.m_chkSymbolDropLineBottom = new System.Windows.Forms.CheckBox();
      this.m_chkSymbolDropLineTop = new System.Windows.Forms.CheckBox();
      this.m_chkSymbolDropLineRight = new System.Windows.Forms.CheckBox();
      this.m_chkSymbolDropLineLeft = new System.Windows.Forms.CheckBox();
      this.m_cbSymbolSize = new System.Windows.Forms.ComboBox();
      this.m_cbSymbolStyle = new System.Windows.Forms.ComboBox();
      this.m_cbSymbolShape = new System.Windows.Forms.ComboBox();
      this.m_lblSymbolSize = new System.Windows.Forms.Label();
      this.m_lblSymbolStyle = new System.Windows.Forms.Label();
      this.m_lblSymbolShape = new System.Windows.Forms.Label();
      this.m_btLineSymbolColorDetails = new System.Windows.Forms.Button();
      this.m_cbLineSymbolColor = new System.Windows.Forms.ComboBox();
      this.m_lblLineSymbolColor = new System.Windows.Forms.Label();
      this.m_gbSymbol.SuspendLayout();
      this.m_gbSymbolDropLine.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_gbSymbol
      // 
      this.m_gbSymbol.Controls.Add(this.m_edSymbolSkipFrequency);
      this.m_gbSymbol.Controls.Add(this.label6);
      this.m_gbSymbol.Controls.Add(this.m_chkSymbolSkipPoints);
      this.m_gbSymbol.Controls.Add(this.m_gbSymbolDropLine);
      this.m_gbSymbol.Controls.Add(this.m_cbSymbolSize);
      this.m_gbSymbol.Controls.Add(this.m_cbSymbolStyle);
      this.m_gbSymbol.Controls.Add(this.m_cbSymbolShape);
      this.m_gbSymbol.Controls.Add(this.m_lblSymbolSize);
      this.m_gbSymbol.Controls.Add(this.m_lblSymbolStyle);
      this.m_gbSymbol.Controls.Add(this.m_lblSymbolShape);
      this.m_gbSymbol.Location = new System.Drawing.Point(8, 56);
      this.m_gbSymbol.Name = "m_gbSymbol";
      this.m_gbSymbol.Size = new System.Drawing.Size(216, 232);
      this.m_gbSymbol.TabIndex = 24;
      this.m_gbSymbol.TabStop = false;
      this.m_gbSymbol.Text = "Symbol";
      // 
      // m_edSymbolSkipFrequency
      // 
      this.m_edSymbolSkipFrequency.Location = new System.Drawing.Point(128, 200);
      this.m_edSymbolSkipFrequency.Name = "m_edSymbolSkipFrequency";
      this.m_edSymbolSkipFrequency.Size = new System.Drawing.Size(72, 20);
      this.m_edSymbolSkipFrequency.TabIndex = 9;
      this.m_edSymbolSkipFrequency.Text = "textBox1";
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(8, 200);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(64, 16);
      this.label6.TabIndex = 8;
      this.label6.Text = "Skip Points";
      // 
      // m_chkSymbolSkipPoints
      // 
      this.m_chkSymbolSkipPoints.Location = new System.Drawing.Point(80, 200);
      this.m_chkSymbolSkipPoints.Name = "m_chkSymbolSkipPoints";
      this.m_chkSymbolSkipPoints.Size = new System.Drawing.Size(48, 16);
      this.m_chkSymbolSkipPoints.TabIndex = 7;
      this.m_chkSymbolSkipPoints.Text = "Freq";
      // 
      // m_gbSymbolDropLine
      // 
      this.m_gbSymbolDropLine.Controls.Add(this.m_chkSymbolDropLineBottom);
      this.m_gbSymbolDropLine.Controls.Add(this.m_chkSymbolDropLineTop);
      this.m_gbSymbolDropLine.Controls.Add(this.m_chkSymbolDropLineRight);
      this.m_gbSymbolDropLine.Controls.Add(this.m_chkSymbolDropLineLeft);
      this.m_gbSymbolDropLine.Location = new System.Drawing.Point(24, 112);
      this.m_gbSymbolDropLine.Name = "m_gbSymbolDropLine";
      this.m_gbSymbolDropLine.Size = new System.Drawing.Size(184, 80);
      this.m_gbSymbolDropLine.TabIndex = 6;
      this.m_gbSymbolDropLine.TabStop = false;
      this.m_gbSymbolDropLine.Text = "Drop Line";
      // 
      // m_chkSymbolDropLineBottom
      // 
      this.m_chkSymbolDropLineBottom.Location = new System.Drawing.Point(80, 48);
      this.m_chkSymbolDropLineBottom.Name = "m_chkSymbolDropLineBottom";
      this.m_chkSymbolDropLineBottom.Size = new System.Drawing.Size(64, 16);
      this.m_chkSymbolDropLineBottom.TabIndex = 3;
      this.m_chkSymbolDropLineBottom.Text = "Bottom";
      // 
      // m_chkSymbolDropLineTop
      // 
      this.m_chkSymbolDropLineTop.Location = new System.Drawing.Point(16, 48);
      this.m_chkSymbolDropLineTop.Name = "m_chkSymbolDropLineTop";
      this.m_chkSymbolDropLineTop.Size = new System.Drawing.Size(48, 16);
      this.m_chkSymbolDropLineTop.TabIndex = 2;
      this.m_chkSymbolDropLineTop.Text = "Top";
      // 
      // m_chkSymbolDropLineRight
      // 
      this.m_chkSymbolDropLineRight.Location = new System.Drawing.Point(80, 24);
      this.m_chkSymbolDropLineRight.Name = "m_chkSymbolDropLineRight";
      this.m_chkSymbolDropLineRight.Size = new System.Drawing.Size(56, 16);
      this.m_chkSymbolDropLineRight.TabIndex = 1;
      this.m_chkSymbolDropLineRight.Text = "Right";
      // 
      // m_chkSymbolDropLineLeft
      // 
      this.m_chkSymbolDropLineLeft.Location = new System.Drawing.Point(16, 24);
      this.m_chkSymbolDropLineLeft.Name = "m_chkSymbolDropLineLeft";
      this.m_chkSymbolDropLineLeft.Size = new System.Drawing.Size(48, 16);
      this.m_chkSymbolDropLineLeft.TabIndex = 0;
      this.m_chkSymbolDropLineLeft.Text = "Left";
      // 
      // m_cbSymbolSize
      // 
      this.m_cbSymbolSize.Location = new System.Drawing.Point(56, 80);
      this.m_cbSymbolSize.Name = "m_cbSymbolSize";
      this.m_cbSymbolSize.Size = new System.Drawing.Size(152, 21);
      this.m_cbSymbolSize.TabIndex = 5;
      this.m_cbSymbolSize.Text = "comboBox1";
      // 
      // m_cbSymbolStyle
      // 
      this.m_cbSymbolStyle.Location = new System.Drawing.Point(56, 48);
      this.m_cbSymbolStyle.Name = "m_cbSymbolStyle";
      this.m_cbSymbolStyle.Size = new System.Drawing.Size(152, 21);
      this.m_cbSymbolStyle.TabIndex = 4;
      this.m_cbSymbolStyle.Text = "comboBox1";
      // 
      // m_cbSymbolShape
      // 
      this.m_cbSymbolShape.Location = new System.Drawing.Point(56, 16);
      this.m_cbSymbolShape.Name = "m_cbSymbolShape";
      this.m_cbSymbolShape.Size = new System.Drawing.Size(144, 21);
      this.m_cbSymbolShape.TabIndex = 3;
      this.m_cbSymbolShape.Text = "comboBox1";
      this.m_cbSymbolShape.SelectionChangeCommitted += new System.EventHandler(this.EhSymbolShape_SelectionChangeCommit);
      // 
      // m_lblSymbolSize
      // 
      this.m_lblSymbolSize.Location = new System.Drawing.Point(8, 80);
      this.m_lblSymbolSize.Name = "m_lblSymbolSize";
      this.m_lblSymbolSize.Size = new System.Drawing.Size(40, 16);
      this.m_lblSymbolSize.TabIndex = 2;
      this.m_lblSymbolSize.Text = "Size";
      // 
      // m_lblSymbolStyle
      // 
      this.m_lblSymbolStyle.Location = new System.Drawing.Point(8, 48);
      this.m_lblSymbolStyle.Name = "m_lblSymbolStyle";
      this.m_lblSymbolStyle.Size = new System.Drawing.Size(40, 16);
      this.m_lblSymbolStyle.TabIndex = 1;
      this.m_lblSymbolStyle.Text = "Style";
      // 
      // m_lblSymbolShape
      // 
      this.m_lblSymbolShape.Location = new System.Drawing.Point(8, 24);
      this.m_lblSymbolShape.Name = "m_lblSymbolShape";
      this.m_lblSymbolShape.Size = new System.Drawing.Size(64, 16);
      this.m_lblSymbolShape.TabIndex = 0;
      this.m_lblSymbolShape.Text = "Shape";
      // 
      // m_btLineSymbolColorDetails
      // 
      this.m_btLineSymbolColorDetails.Location = new System.Drawing.Point(176, 24);
      this.m_btLineSymbolColorDetails.Name = "m_btLineSymbolColorDetails";
      this.m_btLineSymbolColorDetails.Size = new System.Drawing.Size(48, 24);
      this.m_btLineSymbolColorDetails.TabIndex = 21;
      this.m_btLineSymbolColorDetails.Text = "Adv..";
      // 
      // m_cbLineSymbolColor
      // 
      this.m_cbLineSymbolColor.Location = new System.Drawing.Point(8, 24);
      this.m_cbLineSymbolColor.Name = "m_cbLineSymbolColor";
      this.m_cbLineSymbolColor.Size = new System.Drawing.Size(160, 21);
      this.m_cbLineSymbolColor.TabIndex = 20;
      this.m_cbLineSymbolColor.Text = "comboBox1";
      // 
      // m_lblLineSymbolColor
      // 
      this.m_lblLineSymbolColor.Location = new System.Drawing.Point(8, 8);
      this.m_lblLineSymbolColor.Name = "m_lblLineSymbolColor";
      this.m_lblLineSymbolColor.Size = new System.Drawing.Size(100, 16);
      this.m_lblLineSymbolColor.TabIndex = 19;
      this.m_lblLineSymbolColor.Text = "Line/Symbol Color";
      // 
      // XYPlotScatterStyleControl
      // 
      this.Controls.Add(this.m_gbSymbol);
      this.Controls.Add(this.m_btLineSymbolColorDetails);
      this.Controls.Add(this.m_cbLineSymbolColor);
      this.Controls.Add(this.m_lblLineSymbolColor);
      this.Name = "XYPlotScatterStyleControl";
      this.Size = new System.Drawing.Size(232, 296);
      this.m_gbSymbol.ResumeLayout(false);
      this.m_gbSymbolDropLine.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion


    public void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      ++m_SuppressEvents;
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
      --m_SuppressEvents;
    }

    #region IXYPlotScatterStyleView Members

    public IXYPlotScatterStyleViewEventSink Controller
    {
      get { return _controller; }
      set { _controller = value; }
    }

    



    public void InitializeSymbolStyle(string[] arr, string sel)
    {
      InitComboBox(this.m_cbSymbolStyle,arr,sel);
    }

    public string SymbolStyle
    {
      get { return (string)m_cbSymbolStyle.SelectedItem; }
    }

    public void InitializeSymbolSize(string[] arr, string sel)
    {
      InitComboBox(this.m_cbSymbolSize,arr,sel);
    }
    public string SymbolSize
    {
      get { return (string)m_cbSymbolSize.SelectedItem; }
    }


    public void InitializeSymbolShape(string[] arr, string sel)
    {
      InitComboBox(this.m_cbSymbolShape,arr,sel);
    }
    public string SymbolShape
    {
      get { return (string)m_cbSymbolShape.SelectedItem; }
    }

    public void InitializeDropLineConditions(bool bLeft, bool bBottom, bool bRight, bool bTop)
    {
      this.m_chkSymbolDropLineLeft.Checked = bLeft;
      this.m_chkSymbolDropLineBottom.Checked = bBottom;
      this.m_chkSymbolDropLineRight.Checked = bRight;
      this.m_chkSymbolDropLineTop.Checked = bTop;
    }

    public bool DropLineLeft
    {
      get { return m_chkSymbolDropLineLeft.Checked; }
    }
    public bool DropLineBottom
    {
      get { return m_chkSymbolDropLineBottom.Checked; }
    }
    public bool DropLineRight
    {
      get { return m_chkSymbolDropLineRight.Checked; }
    }
    public bool DropLineTop
    {
      get { return m_chkSymbolDropLineTop.Checked; }
    }

  
    public void InitializePlotStyleColor(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineSymbolColor,arr,sel);
    }
    public string SymbolColor
    {
      get { return (string)m_cbLineSymbolColor.SelectedItem; }
    }

  
    private void EhSymbolShape_SelectionChangeCommit(object sender, System.EventArgs e)
    {
    }

 
  




    #endregion
  }
}
