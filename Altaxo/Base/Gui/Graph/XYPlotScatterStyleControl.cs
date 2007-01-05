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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for XYPlotScatterStyleControl.
  /// </summary>
  [UserControlForController(typeof(IXYPlotScatterStyleViewEventSink))]
  public class XYPlotScatterStyleControl : System.Windows.Forms.UserControl, IXYPlotScatterStyleView
  {
    private IXYPlotScatterStyleViewEventSink _controller;
    private bool _EnableDisableAll=false;
    private int m_SuppressEvents = 0;
    private System.Windows.Forms.TextBox m_edSymbolSkipFrequency;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox m_chkSymbolSkipPoints;
    private System.Windows.Forms.ComboBox m_cbSymbolSize;
    private System.Windows.Forms.ComboBox m_cbSymbolStyle;
    private System.Windows.Forms.ComboBox m_cbSymbolShape;
    private System.Windows.Forms.Label m_lblSymbolStyle;
    private System.Windows.Forms.Label m_lblSymbolShape;
    private System.Windows.Forms.CheckBox _chkIndependentColor;
    private System.Windows.Forms.CheckBox _chkIndependentSize;
    private Altaxo.Gui.Common.Drawing.ColorComboBox _cbColor;
    private Label _lblDropLine;
    private CheckedListBox _lbDropLine;
    private IContainer components;

    public XYPlotScatterStyleControl()
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
      this.components = new System.ComponentModel.Container();
      this._lblDropLine = new System.Windows.Forms.Label();
      this._lbDropLine = new System.Windows.Forms.CheckedListBox();
      this._cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._chkIndependentSize = new System.Windows.Forms.CheckBox();
      this.m_edSymbolSkipFrequency = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.m_chkSymbolSkipPoints = new System.Windows.Forms.CheckBox();
      this.m_cbSymbolSize = new System.Windows.Forms.ComboBox();
      this.m_cbSymbolStyle = new System.Windows.Forms.ComboBox();
      this.m_cbSymbolShape = new System.Windows.Forms.ComboBox();
      this.m_lblSymbolStyle = new System.Windows.Forms.Label();
      this.m_lblSymbolShape = new System.Windows.Forms.Label();
      this._chkIndependentColor = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // _lblDropLine
      // 
      this._lblDropLine.Location = new System.Drawing.Point(6, 197);
      this._lblDropLine.Name = "_lblDropLine";
      this._lblDropLine.Size = new System.Drawing.Size(64, 16);
      this._lblDropLine.TabIndex = 29;
      this._lblDropLine.Text = "Drop Lines:";
      this._lblDropLine.Click += new System.EventHandler(this.label1_Click);
      // 
      // _lbDropLine
      // 
      this._lbDropLine.CheckOnClick = true;
      this._lbDropLine.FormattingEnabled = true;
      this._lbDropLine.Location = new System.Drawing.Point(76, 197);
      this._lbDropLine.Name = "_lbDropLine";
      this._lbDropLine.Size = new System.Drawing.Size(128, 64);
      this._lbDropLine.TabIndex = 25;
      // 
      // _cbColor
      // 
      this._cbColor.ColorType = Altaxo.Graph.ColorType.PlotColor;
      this._cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColor.FormattingEnabled = true;
      this._cbColor.ItemHeight = 15;
      this._cbColor.Items.AddRange(new object[] {
            System.Drawing.Color.Black,
            System.Drawing.Color.Black,
            System.Drawing.Color.Black,
            System.Drawing.Color.Red,
            System.Drawing.Color.Green,
            System.Drawing.Color.Blue,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.Coral,
            System.Drawing.Color.Black,
            System.Drawing.Color.Red,
            System.Drawing.Color.Green,
            System.Drawing.Color.Blue,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.Coral,
            System.Drawing.Color.Black,
            System.Drawing.Color.Red,
            System.Drawing.Color.Green,
            System.Drawing.Color.Blue,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.Coral});
      this._cbColor.Location = new System.Drawing.Point(83, 72);
      this._cbColor.Name = "_cbColor";
      this._cbColor.Size = new System.Drawing.Size(121, 21);
      this._cbColor.TabIndex = 28;
      // 
      // _chkIndependentSize
      // 
      this._chkIndependentSize.Location = new System.Drawing.Point(9, 102);
      this._chkIndependentSize.Name = "_chkIndependentSize";
      this._chkIndependentSize.Size = new System.Drawing.Size(77, 24);
      this._chkIndependentSize.TabIndex = 27;
      this._chkIndependentSize.Text = "ind. Size:";
      // 
      // m_edSymbolSkipFrequency
      // 
      this.m_edSymbolSkipFrequency.Location = new System.Drawing.Point(132, 132);
      this.m_edSymbolSkipFrequency.Name = "m_edSymbolSkipFrequency";
      this.m_edSymbolSkipFrequency.Size = new System.Drawing.Size(72, 20);
      this.m_edSymbolSkipFrequency.TabIndex = 9;
      this.m_edSymbolSkipFrequency.Text = "textBox1";
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(6, 135);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(64, 16);
      this.label6.TabIndex = 8;
      this.label6.Text = "Skip Points";
      // 
      // m_chkSymbolSkipPoints
      // 
      this.m_chkSymbolSkipPoints.Location = new System.Drawing.Point(83, 136);
      this.m_chkSymbolSkipPoints.Name = "m_chkSymbolSkipPoints";
      this.m_chkSymbolSkipPoints.Size = new System.Drawing.Size(48, 16);
      this.m_chkSymbolSkipPoints.TabIndex = 7;
      this.m_chkSymbolSkipPoints.Text = "Freq";
      this.m_chkSymbolSkipPoints.CheckedChanged += new System.EventHandler(this.m_chkSymbolSkipPoints_CheckedChanged);
      // 
      // m_cbSymbolSize
      // 
      this.m_cbSymbolSize.Location = new System.Drawing.Point(83, 102);
      this.m_cbSymbolSize.Name = "m_cbSymbolSize";
      this.m_cbSymbolSize.Size = new System.Drawing.Size(121, 21);
      this.m_cbSymbolSize.TabIndex = 5;
      this.m_cbSymbolSize.Text = "comboBox1";
      // 
      // m_cbSymbolStyle
      // 
      this.m_cbSymbolStyle.Location = new System.Drawing.Point(52, 38);
      this.m_cbSymbolStyle.Name = "m_cbSymbolStyle";
      this.m_cbSymbolStyle.Size = new System.Drawing.Size(152, 21);
      this.m_cbSymbolStyle.TabIndex = 4;
      this.m_cbSymbolStyle.Text = "comboBox1";
      // 
      // m_cbSymbolShape
      // 
      this.m_cbSymbolShape.Location = new System.Drawing.Point(52, 6);
      this.m_cbSymbolShape.Name = "m_cbSymbolShape";
      this.m_cbSymbolShape.Size = new System.Drawing.Size(152, 21);
      this.m_cbSymbolShape.TabIndex = 3;
      this.m_cbSymbolShape.Text = "comboBox1";
      this.m_cbSymbolShape.SelectionChangeCommitted += new System.EventHandler(this.EhSymbolShape_SelectionChangeCommit);
      // 
      // m_lblSymbolStyle
      // 
      this.m_lblSymbolStyle.Location = new System.Drawing.Point(4, 46);
      this.m_lblSymbolStyle.Name = "m_lblSymbolStyle";
      this.m_lblSymbolStyle.Size = new System.Drawing.Size(40, 16);
      this.m_lblSymbolStyle.TabIndex = 1;
      this.m_lblSymbolStyle.Text = "Style";
      // 
      // m_lblSymbolShape
      // 
      this.m_lblSymbolShape.Location = new System.Drawing.Point(4, 14);
      this.m_lblSymbolShape.Name = "m_lblSymbolShape";
      this.m_lblSymbolShape.Size = new System.Drawing.Size(42, 16);
      this.m_lblSymbolShape.TabIndex = 0;
      this.m_lblSymbolShape.Text = "Shape";
      // 
      // _chkIndependentColor
      // 
      this._chkIndependentColor.Location = new System.Drawing.Point(9, 77);
      this._chkIndependentColor.Name = "_chkIndependentColor";
      this._chkIndependentColor.Size = new System.Drawing.Size(80, 16);
      this._chkIndependentColor.TabIndex = 26;
      this._chkIndependentColor.Text = "ind. Color:";
      this._chkIndependentColor.CheckedChanged += new System.EventHandler(this._chkIndependentColor_CheckedChanged);
      // 
      // XYPlotScatterStyleControl
      // 
      this.Controls.Add(this._lblDropLine);
      this.Controls.Add(this._lbDropLine);
      this.Controls.Add(this.m_cbSymbolStyle);
      this.Controls.Add(this._cbColor);
      this.Controls.Add(this._chkIndependentColor);
      this.Controls.Add(this.m_lblSymbolShape);
      this.Controls.Add(this.m_edSymbolSkipFrequency);
      this.Controls.Add(this.m_lblSymbolStyle);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.m_cbSymbolShape);
      this.Controls.Add(this.m_chkSymbolSkipPoints);
      this.Controls.Add(this.m_cbSymbolSize);
      this.Controls.Add(this._chkIndependentSize);
      this.Name = "XYPlotScatterStyleControl";
      this.Size = new System.Drawing.Size(214, 271);
      this.ResumeLayout(false);
      this.PerformLayout();

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

    public void EnableDisableMain(bool bEnable)
    {
      this._chkIndependentColor.Enabled = bEnable;
      this._chkIndependentSize.Enabled = bEnable;
      
      this._cbColor.Enabled = bEnable;
      this.m_cbSymbolSize.Enabled = bEnable;
      this.m_cbSymbolStyle.Enabled = bEnable;
      this.m_chkSymbolSkipPoints.Enabled = bEnable;
      this.m_edSymbolSkipFrequency.Enabled = bEnable;
    }

    bool ShouldEnableMain()
    {
      return this.m_cbSymbolShape.SelectedIndex != 0 ||
        this._lbDropLine.CheckedIndices.Count > 0;
          
    }
    public void SetEnableDisableMain(bool bActivate)
    {
      this._EnableDisableAll = bActivate;
      this.EnableDisableMain(_EnableDisableAll==false || this.ShouldEnableMain());
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

    public void InitializeIndependentSymbolSize(bool val)
    {
      this._chkIndependentSize.Checked = val;
    }
    public bool IndependentSymbolSize
    {
      get { return this._chkIndependentSize.Checked; }
    }


    public void InitializeSymbolShape(string[] arr, string sel)
    {
      InitComboBox(this.m_cbSymbolShape,arr,sel);
    }
    public string SymbolShape
    {
      get { return (string)m_cbSymbolShape.SelectedItem; }
    }

    public void InitializeDropLineConditions(List<SelectableListNode> names)
    {
      foreach (SelectableListNode node in names)
        this._lbDropLine.Items.Add(node, node.Selected);
    }

    public List<SelectableListNode> DropLines
    {
      get
      {
        List<SelectableListNode> names = new List<SelectableListNode>();
        for (int i = 0; i < _lbDropLine.Items.Count; i++)
        {
          if (_lbDropLine.GetItemChecked(i))
          {
            SelectableListNode node = (SelectableListNode)_lbDropLine.Items[i];
            names.Add(node);
          }
        }
        return names;
      }
    }
  
    public void InitializePlotStyleColor(Color sel)
    {
      _cbColor.ColorChoice = sel;
    }
    public Color SymbolColor
    {
      get { return _cbColor.ColorChoice; }
    }

    public void InitializeSkipPoints(int freq)
    {
      this.m_edSymbolSkipFrequency.Text = freq.ToString();
      this.m_edSymbolSkipFrequency.Enabled = (freq!=1);
      this.m_chkSymbolSkipPoints.Checked = (freq!=1) ;
    }

    public int SkipPoints
    {
      get
      {
        if(this.m_chkSymbolSkipPoints.Checked)
        {
          int val;
          if(Altaxo.Serialization.GUIConversion.IsInteger(this.m_edSymbolSkipFrequency.Text, out val))
            return val;
        }
        return 1;
      }
    }

    private void EhSymbolShape_SelectionChangeCommit(object sender, System.EventArgs e)
    {
      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());
    }

    public void InitializeIndependentColor(bool val)
    {
      this._chkIndependentColor.Checked = val;
      _chkIndependentColor_CheckedChanged(_chkIndependentColor.Checked,EventArgs.Empty);
    }
 
    public bool IndependentColor 
    {
      get
      {
        return this._chkIndependentColor.Checked; 
      }
    }

    private void _chkIndependentColor_CheckedChanged(object sender, System.EventArgs e)
    {
        
    }

    #endregion

    private void m_chkSymbolDropLineLeft_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());

    }

    private void m_chkSymbolDropLineRight_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());

    }

    private void m_chkSymbolDropLineTop_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());

    }

    private void m_chkSymbolDropLineBottom_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());

    }

    private void m_chkSymbolSkipPoints_CheckedChanged(object sender, System.EventArgs e)
    {
      if(!this.m_chkSymbolSkipPoints.Checked)
        this.m_edSymbolSkipFrequency.Text = "1";

      this.m_edSymbolSkipFrequency.Enabled = this.m_chkSymbolSkipPoints.Checked;
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

     
  }
}
