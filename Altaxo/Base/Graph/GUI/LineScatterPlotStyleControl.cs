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

namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// Summary description for LineScatterPlotStyleControl.
  /// </summary>
  public class LineScatterPlotStyleControl : System.Windows.Forms.UserControl, ILineScatterPlotStyleView
  {
    private ILineScatterPlotStyleController m_Controller;
    private int m_SuppressEvents=0;

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox m_chkPlotGroupSymbol;
    private System.Windows.Forms.CheckBox m_chkPlotGroupLineType;
    private System.Windows.Forms.CheckBox m_chkPlotGroupColor;
    private System.Windows.Forms.RadioButton m_rbtPlotGroupIncremental;
    private System.Windows.Forms.RadioButton m_rbtPlotGroupIndependent;
    private System.Windows.Forms.Button m_btRemove;
    private System.Windows.Forms.Button m_btWorksheet;
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
    private System.Windows.Forms.Label m_lblPlotType;
    private System.Windows.Forms.ComboBox m_cbPlotType;
    private System.Windows.Forms.TextBox m_txtDatasets;
    private System.Windows.Forms.Label m_lblDataSets;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public LineScatterPlotStyleControl()
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.m_chkPlotGroupSymbol = new System.Windows.Forms.CheckBox();
      this.m_chkPlotGroupLineType = new System.Windows.Forms.CheckBox();
      this.m_chkPlotGroupColor = new System.Windows.Forms.CheckBox();
      this.m_rbtPlotGroupIncremental = new System.Windows.Forms.RadioButton();
      this.m_rbtPlotGroupIndependent = new System.Windows.Forms.RadioButton();
      this.m_btRemove = new System.Windows.Forms.Button();
      this.m_btWorksheet = new System.Windows.Forms.Button();
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
      this.m_lblPlotType = new System.Windows.Forms.Label();
      this.m_cbPlotType = new System.Windows.Forms.ComboBox();
      this.m_txtDatasets = new System.Windows.Forms.TextBox();
      this.m_lblDataSets = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      this.m_gbSymbol.SuspendLayout();
      this.m_gbSymbolDropLine.SuspendLayout();
      this.m_gbLine.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.m_chkPlotGroupSymbol);
      this.groupBox1.Controls.Add(this.m_chkPlotGroupLineType);
      this.groupBox1.Controls.Add(this.m_chkPlotGroupColor);
      this.groupBox1.Controls.Add(this.m_rbtPlotGroupIncremental);
      this.groupBox1.Controls.Add(this.m_rbtPlotGroupIndependent);
      this.groupBox1.Location = new System.Drawing.Point(448, 120);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(96, 232);
      this.groupBox1.TabIndex = 29;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Plot Group";
      // 
      // m_chkPlotGroupSymbol
      // 
      this.m_chkPlotGroupSymbol.Location = new System.Drawing.Point(8, 128);
      this.m_chkPlotGroupSymbol.Name = "m_chkPlotGroupSymbol";
      this.m_chkPlotGroupSymbol.Size = new System.Drawing.Size(72, 16);
      this.m_chkPlotGroupSymbol.TabIndex = 4;
      this.m_chkPlotGroupSymbol.Text = "Symbol";
      // 
      // m_chkPlotGroupLineType
      // 
      this.m_chkPlotGroupLineType.Location = new System.Drawing.Point(8, 104);
      this.m_chkPlotGroupLineType.Name = "m_chkPlotGroupLineType";
      this.m_chkPlotGroupLineType.Size = new System.Drawing.Size(80, 16);
      this.m_chkPlotGroupLineType.TabIndex = 3;
      this.m_chkPlotGroupLineType.Text = "Line Type";
      // 
      // m_chkPlotGroupColor
      // 
      this.m_chkPlotGroupColor.Location = new System.Drawing.Point(8, 80);
      this.m_chkPlotGroupColor.Name = "m_chkPlotGroupColor";
      this.m_chkPlotGroupColor.Size = new System.Drawing.Size(72, 16);
      this.m_chkPlotGroupColor.TabIndex = 2;
      this.m_chkPlotGroupColor.Text = "Color";
      // 
      // m_rbtPlotGroupIncremental
      // 
      this.m_rbtPlotGroupIncremental.Location = new System.Drawing.Point(8, 48);
      this.m_rbtPlotGroupIncremental.Name = "m_rbtPlotGroupIncremental";
      this.m_rbtPlotGroupIncremental.Size = new System.Drawing.Size(88, 24);
      this.m_rbtPlotGroupIncremental.TabIndex = 1;
      this.m_rbtPlotGroupIncremental.Text = "Incremental";
      this.m_rbtPlotGroupIncremental.Click += new System.EventHandler(this.EhPlotGroupIndependent_Changed);
      // 
      // m_rbtPlotGroupIndependent
      // 
      this.m_rbtPlotGroupIndependent.Location = new System.Drawing.Point(8, 24);
      this.m_rbtPlotGroupIndependent.Name = "m_rbtPlotGroupIndependent";
      this.m_rbtPlotGroupIndependent.Size = new System.Drawing.Size(88, 24);
      this.m_rbtPlotGroupIndependent.TabIndex = 0;
      this.m_rbtPlotGroupIndependent.Text = "Independent";
      this.m_rbtPlotGroupIndependent.Click += new System.EventHandler(this.EhPlotGroupIndependent_Changed);
      // 
      // m_btRemove
      // 
      this.m_btRemove.Location = new System.Drawing.Point(448, 80);
      this.m_btRemove.Name = "m_btRemove";
      this.m_btRemove.Size = new System.Drawing.Size(96, 24);
      this.m_btRemove.TabIndex = 28;
      this.m_btRemove.Text = "Remove";
      // 
      // m_btWorksheet
      // 
      this.m_btWorksheet.Location = new System.Drawing.Point(448, 48);
      this.m_btWorksheet.Name = "m_btWorksheet";
      this.m_btWorksheet.Size = new System.Drawing.Size(96, 24);
      this.m_btWorksheet.TabIndex = 27;
      this.m_btWorksheet.Text = "Worksheet";
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
      this.m_gbSymbol.Location = new System.Drawing.Point(224, 120);
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
      this.m_chkLineSymbolGap.Location = new System.Drawing.Point(288, 80);
      this.m_chkLineSymbolGap.Name = "m_chkLineSymbolGap";
      this.m_chkLineSymbolGap.Size = new System.Drawing.Size(112, 24);
      this.m_chkLineSymbolGap.TabIndex = 22;
      this.m_chkLineSymbolGap.Text = "Line/Symbol Gap";
      // 
      // m_btLineSymbolColorDetails
      // 
      this.m_btLineSymbolColorDetails.Location = new System.Drawing.Point(232, 80);
      this.m_btLineSymbolColorDetails.Name = "m_btLineSymbolColorDetails";
      this.m_btLineSymbolColorDetails.Size = new System.Drawing.Size(48, 24);
      this.m_btLineSymbolColorDetails.TabIndex = 21;
      this.m_btLineSymbolColorDetails.Text = "Adv..";
      // 
      // m_cbLineSymbolColor
      // 
      this.m_cbLineSymbolColor.Location = new System.Drawing.Point(120, 80);
      this.m_cbLineSymbolColor.Name = "m_cbLineSymbolColor";
      this.m_cbLineSymbolColor.Size = new System.Drawing.Size(104, 21);
      this.m_cbLineSymbolColor.TabIndex = 20;
      this.m_cbLineSymbolColor.Text = "comboBox1";
      // 
      // m_lblLineSymbolColor
      // 
      this.m_lblLineSymbolColor.Location = new System.Drawing.Point(16, 80);
      this.m_lblLineSymbolColor.Name = "m_lblLineSymbolColor";
      this.m_lblLineSymbolColor.Size = new System.Drawing.Size(100, 16);
      this.m_lblLineSymbolColor.TabIndex = 19;
      this.m_lblLineSymbolColor.Text = "Line/Symbol Color";
      // 
      // m_lblPlotType
      // 
      this.m_lblPlotType.Location = new System.Drawing.Point(16, 40);
      this.m_lblPlotType.Name = "m_lblPlotType";
      this.m_lblPlotType.Size = new System.Drawing.Size(56, 16);
      this.m_lblPlotType.TabIndex = 18;
      this.m_lblPlotType.Text = "Plot Type";
      // 
      // m_cbPlotType
      // 
      this.m_cbPlotType.Location = new System.Drawing.Point(88, 40);
      this.m_cbPlotType.Name = "m_cbPlotType";
      this.m_cbPlotType.Size = new System.Drawing.Size(320, 21);
      this.m_cbPlotType.TabIndex = 17;
      this.m_cbPlotType.Text = "comboBox1";
      // 
      // m_txtDatasets
      // 
      this.m_txtDatasets.Location = new System.Drawing.Point(88, 8);
      this.m_txtDatasets.Name = "m_txtDatasets";
      this.m_txtDatasets.Size = new System.Drawing.Size(320, 20);
      this.m_txtDatasets.TabIndex = 16;
      this.m_txtDatasets.Text = "textBox1";
      // 
      // m_lblDataSets
      // 
      this.m_lblDataSets.Location = new System.Drawing.Point(16, 8);
      this.m_lblDataSets.Name = "m_lblDataSets";
      this.m_lblDataSets.Size = new System.Drawing.Size(56, 16);
      this.m_lblDataSets.TabIndex = 15;
      this.m_lblDataSets.Text = "Dataset(s)";
      // 
      // LineScatterPlotStyleControl
      // 
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.m_btRemove);
      this.Controls.Add(this.m_btWorksheet);
      this.Controls.Add(this.m_gbSymbol);
      this.Controls.Add(this.m_gbLine);
      this.Controls.Add(this.m_chkLineSymbolGap);
      this.Controls.Add(this.m_btLineSymbolColorDetails);
      this.Controls.Add(this.m_cbLineSymbolColor);
      this.Controls.Add(this.m_lblLineSymbolColor);
      this.Controls.Add(this.m_lblPlotType);
      this.Controls.Add(this.m_cbPlotType);
      this.Controls.Add(this.m_txtDatasets);
      this.Controls.Add(this.m_lblDataSets);
      this.Name = "LineScatterPlotStyleControl";
      this.Size = new System.Drawing.Size(560, 368);
      this.groupBox1.ResumeLayout(false);
      this.m_gbSymbol.ResumeLayout(false);
      this.m_gbSymbolDropLine.ResumeLayout(false);
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

    #region ILineScatterPlotStyleView Members

    public ILineScatterPlotStyleController Controller
    {
      get { return m_Controller; }
      set { m_Controller = value; }
    }

    public System.Windows.Forms.Form Form
    {
      get { return this.ParentForm; }
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

    public void InitializePlotType(string[] arr, string sel)
    {
      InitComboBox(this.m_cbPlotType,arr,sel);
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

    private void EhSymbolShape_SelectionChangeCommit(object sender, System.EventArgs e)
    {
      if(null!=this.Controller)
        Controller.EhView_SymbolShapeChanged(this.m_cbSymbolShape.SelectedIndex);
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
  

    public void InitializePlotGroupConditions(bool bMemberOfPlotGroup, bool bIndependent, bool bColor, bool bLineType, bool bSymbol)
    {
      this.m_rbtPlotGroupIndependent.Checked=  bIndependent;
      this.m_rbtPlotGroupIncremental.Checked= !bIndependent;


      this.m_chkPlotGroupColor.Checked = bColor;
      this.m_chkPlotGroupLineType.Checked = bLineType;
      this.m_chkPlotGroupSymbol.Checked = bSymbol;

      this.m_rbtPlotGroupIndependent.Enabled=  bMemberOfPlotGroup;
      this.m_rbtPlotGroupIncremental.Enabled=  bMemberOfPlotGroup;
      this.m_chkPlotGroupColor.Enabled=       !bIndependent;
      this.m_chkPlotGroupLineType.Enabled=    !bIndependent;
      this.m_chkPlotGroupSymbol.Enabled=      !bIndependent;
    }

    private void EhPlotGroupIndependent_Changed(object sender, System.EventArgs e)
    {
      bool bIndependent = this.m_rbtPlotGroupIndependent.Checked;

      if(Controller!=null)
        Controller.EhView_PlotGroupIndependent_Changed(bIndependent);

      this.m_chkPlotGroupColor.Enabled=       !bIndependent;
      this.m_chkPlotGroupLineType.Enabled=    !bIndependent;
      this.m_chkPlotGroupSymbol.Enabled=      !bIndependent;
    }

    public bool PlotGroupIncremental
    {
      get { return m_rbtPlotGroupIncremental.Checked; }
    }

    public bool PlotGroupColor
    {
      get { return m_chkPlotGroupColor.Checked; }
    }
    public bool PlotGroupLineType
    {
      get { return m_chkPlotGroupLineType.Checked; }
    }
    public bool PlotGroupSymbol
    {
      get { return m_chkPlotGroupSymbol.Checked; }
    }



    #endregion
  }
}
