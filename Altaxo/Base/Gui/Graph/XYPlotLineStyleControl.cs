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
  /// Summary description for XYPlotLineStyleControl.
  /// </summary>
    	[UserControlForController(typeof(IXYPlotLineStyleViewEventSink))]
  public class XYPlotLineStyleControl : System.Windows.Forms.UserControl, IXYPlotLineStyleView
  {
    private IXYPlotLineStyleViewEventSink _controller;
    private int m_SuppressEvents=0;
    private System.Windows.Forms.GroupBox m_gbLine;
    private System.Windows.Forms.Button m_btLineFillColorDetails;
    private System.Windows.Forms.ComboBox m_cbLineFillColor;
    private System.Windows.Forms.ComboBox m_cbLineFillDirection;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox m_chkLineFillArea;
    private System.Windows.Forms.ComboBox m_cbLineWidth;
    private System.Windows.Forms.ComboBox m_cbLineType;
    private System.Windows.Forms.ComboBox m_cbLineConnect;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox m_chkLineSymbolGap;
    private System.Windows.Forms.Button m_btLineSymbolColorDetails;
    private System.Windows.Forms.ComboBox m_cbLineSymbolColor;
    private System.Windows.Forms.Label m_lblLineSymbolColor;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public XYPlotLineStyleControl()
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
      this.m_gbLine = new System.Windows.Forms.GroupBox();
      this.m_btLineFillColorDetails = new System.Windows.Forms.Button();
      this.m_cbLineFillColor = new System.Windows.Forms.ComboBox();
      this.m_cbLineFillDirection = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.m_chkLineFillArea = new System.Windows.Forms.CheckBox();
      this.m_cbLineWidth = new System.Windows.Forms.ComboBox();
      this.m_cbLineType = new System.Windows.Forms.ComboBox();
      this.m_cbLineConnect = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.m_chkLineSymbolGap = new System.Windows.Forms.CheckBox();
      this.m_btLineSymbolColorDetails = new System.Windows.Forms.Button();
      this.m_cbLineSymbolColor = new System.Windows.Forms.ComboBox();
      this.m_lblLineSymbolColor = new System.Windows.Forms.Label();
      this.m_gbLine.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_gbLine
      // 
      this.m_gbLine.Controls.Add(this.m_btLineFillColorDetails);
      this.m_gbLine.Controls.Add(this.m_cbLineFillColor);
      this.m_gbLine.Controls.Add(this.m_cbLineFillDirection);
      this.m_gbLine.Controls.Add(this.label5);
      this.m_gbLine.Controls.Add(this.label4);
      this.m_gbLine.Controls.Add(this.m_chkLineFillArea);
      this.m_gbLine.Controls.Add(this.m_cbLineWidth);
      this.m_gbLine.Controls.Add(this.m_cbLineType);
      this.m_gbLine.Controls.Add(this.m_cbLineConnect);
      this.m_gbLine.Controls.Add(this.label3);
      this.m_gbLine.Controls.Add(this.label2);
      this.m_gbLine.Controls.Add(this.label1);
      this.m_gbLine.Location = new System.Drawing.Point(8, 120);
      this.m_gbLine.Name = "m_gbLine";
      this.m_gbLine.Size = new System.Drawing.Size(208, 232);
      this.m_gbLine.TabIndex = 23;
      this.m_gbLine.TabStop = false;
      this.m_gbLine.Text = "Line";
      // 
      // m_btLineFillColorDetails
      // 
      this.m_btLineFillColorDetails.Location = new System.Drawing.Point(8, 200);
      this.m_btLineFillColorDetails.Name = "m_btLineFillColorDetails";
      this.m_btLineFillColorDetails.Size = new System.Drawing.Size(40, 23);
      this.m_btLineFillColorDetails.TabIndex = 11;
      this.m_btLineFillColorDetails.Text = "Adv..";
      // 
      // m_cbLineFillColor
      // 
      this.m_cbLineFillColor.Location = new System.Drawing.Point(80, 176);
      this.m_cbLineFillColor.Name = "m_cbLineFillColor";
      this.m_cbLineFillColor.Size = new System.Drawing.Size(120, 21);
      this.m_cbLineFillColor.TabIndex = 10;
      this.m_cbLineFillColor.Text = "comboBox1";
      // 
      // m_cbLineFillDirection
      // 
      this.m_cbLineFillDirection.Location = new System.Drawing.Point(80, 152);
      this.m_cbLineFillDirection.Name = "m_cbLineFillDirection";
      this.m_cbLineFillDirection.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineFillDirection.TabIndex = 9;
      this.m_cbLineFillDirection.Text = "comboBox1";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(8, 176);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(72, 16);
      this.label5.TabIndex = 8;
      this.label5.Text = "Fill Color";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(8, 152);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(72, 16);
      this.label4.TabIndex = 7;
      this.label4.Text = "Fill Direction";
      // 
      // m_chkLineFillArea
      // 
      this.m_chkLineFillArea.Location = new System.Drawing.Point(16, 120);
      this.m_chkLineFillArea.Name = "m_chkLineFillArea";
      this.m_chkLineFillArea.Size = new System.Drawing.Size(176, 16);
      this.m_chkLineFillArea.TabIndex = 6;
      this.m_chkLineFillArea.Text = "Fill Area";
      this.m_chkLineFillArea.CheckedChanged += new System.EventHandler(this.EhLineFillArea_CheckedChanged);
      // 
      // m_cbLineWidth
      // 
      this.m_cbLineWidth.Location = new System.Drawing.Point(80, 80);
      this.m_cbLineWidth.Name = "m_cbLineWidth";
      this.m_cbLineWidth.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineWidth.TabIndex = 5;
      this.m_cbLineWidth.Text = "comboBox1";
      // 
      // m_cbLineType
      // 
      this.m_cbLineType.Location = new System.Drawing.Point(80, 48);
      this.m_cbLineType.Name = "m_cbLineType";
      this.m_cbLineType.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineType.TabIndex = 4;
      this.m_cbLineType.Text = "comboBox1";
      // 
      // m_cbLineConnect
      // 
      this.m_cbLineConnect.Location = new System.Drawing.Point(80, 16);
      this.m_cbLineConnect.Name = "m_cbLineConnect";
      this.m_cbLineConnect.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineConnect.TabIndex = 3;
      this.m_cbLineConnect.Text = "comboBox1";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(16, 80);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 16);
      this.label3.TabIndex = 2;
      this.label3.Text = "Width";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Type";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(16, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(48, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Connect";
      // 
      // m_chkLineSymbolGap
      // 
      this.m_chkLineSymbolGap.Location = new System.Drawing.Point(8, 8);
      this.m_chkLineSymbolGap.Name = "m_chkLineSymbolGap";
      this.m_chkLineSymbolGap.Size = new System.Drawing.Size(112, 24);
      this.m_chkLineSymbolGap.TabIndex = 22;
      this.m_chkLineSymbolGap.Text = "Line/Symbol Gap";
      // 
      // m_btLineSymbolColorDetails
      // 
      this.m_btLineSymbolColorDetails.Location = new System.Drawing.Point(168, 56);
      this.m_btLineSymbolColorDetails.Name = "m_btLineSymbolColorDetails";
      this.m_btLineSymbolColorDetails.Size = new System.Drawing.Size(48, 24);
      this.m_btLineSymbolColorDetails.TabIndex = 21;
      this.m_btLineSymbolColorDetails.Text = "Adv..";
      // 
      // m_cbLineSymbolColor
      // 
      this.m_cbLineSymbolColor.Location = new System.Drawing.Point(8, 56);
      this.m_cbLineSymbolColor.Name = "m_cbLineSymbolColor";
      this.m_cbLineSymbolColor.Size = new System.Drawing.Size(152, 21);
      this.m_cbLineSymbolColor.TabIndex = 20;
      this.m_cbLineSymbolColor.Text = "comboBox1";
      // 
      // m_lblLineSymbolColor
      // 
      this.m_lblLineSymbolColor.Location = new System.Drawing.Point(8, 40);
      this.m_lblLineSymbolColor.Name = "m_lblLineSymbolColor";
      this.m_lblLineSymbolColor.Size = new System.Drawing.Size(100, 16);
      this.m_lblLineSymbolColor.TabIndex = 19;
      this.m_lblLineSymbolColor.Text = "Line/Symbol Color";
      // 
      // XYPlotLineStyleControl
      // 
      this.Controls.Add(this.m_gbLine);
      this.Controls.Add(this.m_chkLineSymbolGap);
      this.Controls.Add(this.m_btLineSymbolColorDetails);
      this.Controls.Add(this.m_cbLineSymbolColor);
      this.Controls.Add(this.m_lblLineSymbolColor);
      this.Name = "XYPlotLineStyleControl";
      this.Size = new System.Drawing.Size(224, 368);
      this.m_gbLine.ResumeLayout(false);
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

    #region IXYPlotLineStyleView Members

    public IXYPlotLineStyleViewEventSink Controller
    {
      get { return _controller; }
      set { _controller = value; }
    }

    public void InitializeLineSymbolGapCondition(bool bGap)
    {
      ++m_SuppressEvents;
      this.m_chkLineSymbolGap.Checked = bGap;
      --m_SuppressEvents;
    }

    public bool LineSymbolGap
    {
      get { return m_chkLineSymbolGap.Checked; }
    }

    public void InitializePlotStyleColor(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineSymbolColor,arr,sel);
    }
    public string SymbolColor
    {
      get { return (string)m_cbLineSymbolColor.SelectedItem; }
    }

    public void InitializeLineConnect(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineConnect,arr,sel);
    }
    public string LineConnect
    {
      get { return (string)m_cbLineConnect.SelectedItem; }
    }
    
    public void InitializeLineStyle(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineType,arr,sel);
    }
    public string LineType
    {
      get { return (string)m_cbLineType.SelectedItem; }
    }
  
    public void InitializeLineWidth(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineWidth,arr,sel);
    }
    public string LineWidth
    {
      get { return (string)m_cbLineWidth.SelectedItem; }
    }
  

    public void InitializeFillCondition(bool bFill)
    {
      this.m_chkLineFillArea.Checked = bFill;
      this.m_cbLineFillColor.Enabled = bFill;
      this.m_cbLineFillDirection.Enabled = bFill;
    }

    public bool LineFillArea 
    {
      get { return m_chkLineFillArea.Checked; }
    }

    private void EhLineFillArea_CheckedChanged(object sender, System.EventArgs e)
    {
      bool bFill = m_chkLineFillArea.Checked;
      this.m_cbLineFillColor.Enabled = bFill;
      this.m_cbLineFillDirection.Enabled = bFill;
    }

   

    public void InitializeFillColor(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineFillColor,arr,sel);
    }
    public string LineFillColor
    {
      get { return (string)m_cbLineFillColor.SelectedItem; }
    }
  
    public void InitializeFillDirection(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineFillDirection,arr,sel);
    }
    public string LineFillDirection
    {
      get { return (string)m_cbLineFillDirection.SelectedItem; }
    }
  




    #endregion
  }
}
