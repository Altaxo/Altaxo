/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for LayerDialog.
	/// </summary>
	public class LayerDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl m_PropTabCtrl;
		private System.Windows.Forms.TabPage m_Tab_Scale;
		private System.Windows.Forms.TabPage m_Tab_TitleAndFormat;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private Layer m_Layer;

		public enum Tab { Scale=0, TitleAndFormat=1, Contents=2, Layer=3, Grid=4, MajorLabels=5, MinorLabels=6, EndTab=7 };
		private EdgeType m_CurrentEdge;
		private System.Windows.Forms.TextBox m_Scale_edFrom;
		private System.Windows.Forms.TextBox m_Scale_edTo;
		private System.Windows.Forms.ComboBox m_Scale_cbType;
		private System.Windows.Forms.ComboBox m_Scale_cbRescale;

		private const int pgScale=0;
		private const int pgFormat=1;
		private const int pgContent=2;
		private const int pgLayer=3;
		private const int pgGrid=4;
		private const int pgMajorLabels=5;
		private const int pgMinorLabels=6;
		private const int pgLastPage=7;




		protected enum PageState { Uninitialized, Initialized, Dirty };
		protected delegate int ApplyValuesHandler();
		protected delegate void InitValuesHandler();


		protected struct PageProperties
		{
			private PageState m_State;
			public ApplyValuesHandler ApplyValues;
			public InitValuesHandler  InitValues;
		
		
			public bool IsUninitialized() { return m_State==PageState.Uninitialized; }
			public bool IsInitialized() { return m_State==PageState.Initialized; }
			public bool IsDirty() { return m_State==PageState.Dirty; }
			
			public void SetUninitialized() { m_State=PageState.Uninitialized; }
			public void SetInitialized() { m_State=PageState.Initialized; }
			public void SetDirty() { m_State=PageState.Dirty; }
			
		}

		protected PageProperties[] m_PProp;
		private System.Windows.Forms.ListBox m_Common_lbEdges;
		private System.Windows.Forms.CheckBox m_Format_chkShowAxis;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox m_Format_edTitle;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox m_Format_cbColor;
		private System.Windows.Forms.ComboBox m_Format_cbThickness;
		private System.Windows.Forms.ComboBox m_Format_cbMajorTickLength;
		private System.Windows.Forms.ComboBox m_Format_cbMajorTicks;
		private System.Windows.Forms.ComboBox m_Format_cbMinorTicks;
		private System.Windows.Forms.ComboBox m_Format_cbAxisPosition;
		private System.Windows.Forms.TextBox m_Format_edAxisPositionValue;
		private System.Windows.Forms.TabPage m_Tab_Contents;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TreeView m_Content_tvDataAvail;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button m_Contents_btPutData;
		private System.Windows.Forms.Button m_Contents_btPullData;
		private System.Windows.Forms.ListBox m_Contents_lbContents;
		private System.Windows.Forms.Button m_Main_btOK;
		private System.Windows.Forms.Button m_Main_btCancel;
		private System.Windows.Forms.Button m_Main_btApply;
		private System.Windows.Forms.Button m_Contents_btListSelUp;
		private System.Windows.Forms.Button m_Contents_btListSelDown;
		private System.Windows.Forms.Button m_Contents_btPlotAssociations;
		private System.Windows.Forms.Button m_Contents_btGroup;
		private System.Windows.Forms.Button m_Contents_btUngroup;
		private System.Windows.Forms.Button m_Contents_btEditRange;
		private System.Windows.Forms.CheckBox m_Contents_chkShowRange;
		private System.Windows.Forms.TabPage m_Tab_Layer;
		private System.Windows.Forms.TabPage m_Tab_MajorLabels;
		private System.Windows.Forms.TabPage m_Tab_MinorLabels;
		private System.Windows.Forms.TabPage m_Tab_Grid;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox m_Layer_edLeftPosition;
		private System.Windows.Forms.TextBox m_Layer_edTopPosition;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox m_Layer_edWidth;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox m_Layer_edHeight;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox m_Layer_edRotation;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox m_Layer_edScale;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.ComboBox m_Layer_cbLinkedLayer;
		private System.Windows.Forms.ComboBox m_Layer_cbLeftType;
		private System.Windows.Forms.ComboBox m_Layer_cbHeightType;
		private System.Windows.Forms.GroupBox m_Layer_gbXAxis;
		private System.Windows.Forms.GroupBox m_Layer_gbYAxis;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkXAxisNone;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkXAxisStraight;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkXAxisCustom;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkYAxisNone;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkYAxisStraight;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkYAxisCustom;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.TextBox m_Layer_edLinkYAxisEndB;
		private System.Windows.Forms.TextBox m_Layer_edLinkYAxisEndA;
		private System.Windows.Forms.TextBox m_Layer_edLinkYAxisOrgB;
		private System.Windows.Forms.TextBox m_Layer_edLinkYAxisOrgA;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisEndB;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisEndA;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisOrgB;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisOrgA;
		private System.Windows.Forms.ComboBox m_Layer_cbTopType;
		private System.Windows.Forms.ComboBox m_Layer_cbWidthType;



		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LayerDialog(Layer layer, Tab initialtab, EdgeType initialEdge)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			m_Layer = layer;
		
		
			// now init the page properties of all pages
			m_PProp = new PageProperties[pgLastPage];
			m_PProp[pgScale].InitValues = new InitValuesHandler(InitTabScale);
			m_PProp[pgScale].ApplyValues = new ApplyValuesHandler(ApplyTabScale);
			m_PProp[pgFormat].InitValues = new InitValuesHandler(InitTabFormat);
			m_PProp[pgFormat].ApplyValues = new ApplyValuesHandler(ApplyTabFormat);
			m_PProp[pgContent].InitValues = new InitValuesHandler(InitTabContents);
			m_PProp[pgContent].ApplyValues = new ApplyValuesHandler(ApplyTabContents);

			m_PProp[pgLayer].InitValues = new InitValuesHandler(InitTabLayer);
			m_PProp[pgLayer].ApplyValues = new ApplyValuesHandler(ApplyTabLayer);

			// Force showing the initial tab, so that
			// the tab updates it's elements
		
			this.m_CurrentEdge = initialEdge;

			// now either switch to the choosen page if not already choosen
			if(this.m_PropTabCtrl.SelectedIndex!=(int)initialtab)
			{
				this.m_PropTabCtrl.SelectedIndex=(int)initialtab; // inittab is called implicitely
			}
			else
			{
				m_PProp[(int)initialtab].InitValues(); // inittab must be called explicitely here
			}

		}

		public int GetTabIndex(Tab a)
		{
			switch(a)
			{
				case Tab.Scale:
					return 0;
				case Tab.TitleAndFormat:
					return 1;
			}

			return 0; // default tab
		}

		public int SelectedPage 
		{
			get { return m_PropTabCtrl.SelectedIndex; }
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LayerDialog));
			this.m_PropTabCtrl = new System.Windows.Forms.TabControl();
			this.m_Tab_Scale = new System.Windows.Forms.TabPage();
			this.m_Scale_cbRescale = new System.Windows.Forms.ComboBox();
			this.m_Scale_cbType = new System.Windows.Forms.ComboBox();
			this.m_Scale_edTo = new System.Windows.Forms.TextBox();
			this.m_Scale_edFrom = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.m_Tab_TitleAndFormat = new System.Windows.Forms.TabPage();
			this.m_Format_edAxisPositionValue = new System.Windows.Forms.TextBox();
			this.m_Format_cbAxisPosition = new System.Windows.Forms.ComboBox();
			this.m_Format_cbMinorTicks = new System.Windows.Forms.ComboBox();
			this.m_Format_cbMajorTicks = new System.Windows.Forms.ComboBox();
			this.m_Format_cbMajorTickLength = new System.Windows.Forms.ComboBox();
			this.m_Format_cbThickness = new System.Windows.Forms.ComboBox();
			this.m_Format_cbColor = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.m_Format_edTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.m_Format_chkShowAxis = new System.Windows.Forms.CheckBox();
			this.m_Tab_Contents = new System.Windows.Forms.TabPage();
			this.m_Contents_chkShowRange = new System.Windows.Forms.CheckBox();
			this.m_Contents_btEditRange = new System.Windows.Forms.Button();
			this.m_Contents_btUngroup = new System.Windows.Forms.Button();
			this.m_Contents_btGroup = new System.Windows.Forms.Button();
			this.m_Contents_btPlotAssociations = new System.Windows.Forms.Button();
			this.m_Contents_btListSelDown = new System.Windows.Forms.Button();
			this.m_Contents_btListSelUp = new System.Windows.Forms.Button();
			this.m_Contents_lbContents = new System.Windows.Forms.ListBox();
			this.m_Contents_btPullData = new System.Windows.Forms.Button();
			this.m_Contents_btPutData = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.m_Content_tvDataAvail = new System.Windows.Forms.TreeView();
			this.label13 = new System.Windows.Forms.Label();
			this.m_Tab_Layer = new System.Windows.Forms.TabPage();
			this.m_Layer_gbYAxis = new System.Windows.Forms.GroupBox();
			this.m_Layer_edLinkYAxisEndB = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkYAxisEndA = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkYAxisOrgB = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkYAxisOrgA = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.m_Layer_rbLinkYAxisCustom = new System.Windows.Forms.RadioButton();
			this.m_Layer_rbLinkYAxisStraight = new System.Windows.Forms.RadioButton();
			this.m_Layer_rbLinkYAxisNone = new System.Windows.Forms.RadioButton();
			this.m_Layer_gbXAxis = new System.Windows.Forms.GroupBox();
			this.label26 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.m_Layer_edLinkXAxisEndB = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkXAxisEndA = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkXAxisOrgB = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkXAxisOrgA = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.m_Layer_rbLinkXAxisCustom = new System.Windows.Forms.RadioButton();
			this.m_Layer_rbLinkXAxisStraight = new System.Windows.Forms.RadioButton();
			this.m_Layer_rbLinkXAxisNone = new System.Windows.Forms.RadioButton();
			this.m_Layer_cbHeightType = new System.Windows.Forms.ComboBox();
			this.m_Layer_cbWidthType = new System.Windows.Forms.ComboBox();
			this.m_Layer_cbTopType = new System.Windows.Forms.ComboBox();
			this.m_Layer_cbLeftType = new System.Windows.Forms.ComboBox();
			this.m_Layer_cbLinkedLayer = new System.Windows.Forms.ComboBox();
			this.label21 = new System.Windows.Forms.Label();
			this.m_Layer_edScale = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.m_Layer_edRotation = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.m_Layer_edHeight = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.m_Layer_edWidth = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.m_Layer_edTopPosition = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.m_Layer_edLeftPosition = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.m_Tab_MajorLabels = new System.Windows.Forms.TabPage();
			this.m_Tab_MinorLabels = new System.Windows.Forms.TabPage();
			this.m_Tab_Grid = new System.Windows.Forms.TabPage();
			this.m_Common_lbEdges = new System.Windows.Forms.ListBox();
			this.m_Main_btOK = new System.Windows.Forms.Button();
			this.m_Main_btCancel = new System.Windows.Forms.Button();
			this.m_Main_btApply = new System.Windows.Forms.Button();
			this.m_PropTabCtrl.SuspendLayout();
			this.m_Tab_Scale.SuspendLayout();
			this.m_Tab_TitleAndFormat.SuspendLayout();
			this.m_Tab_Contents.SuspendLayout();
			this.m_Tab_Layer.SuspendLayout();
			this.m_Layer_gbYAxis.SuspendLayout();
			this.m_Layer_gbXAxis.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_PropTabCtrl
			// 
			this.m_PropTabCtrl.Controls.AddRange(new System.Windows.Forms.Control[] {
																																								this.m_Tab_Scale,
																																								this.m_Tab_TitleAndFormat,
																																								this.m_Tab_Contents,
																																								this.m_Tab_Layer,
																																								this.m_Tab_MajorLabels,
																																								this.m_Tab_MinorLabels,
																																								this.m_Tab_Grid});
			this.m_PropTabCtrl.Location = new System.Drawing.Point(88, 32);
			this.m_PropTabCtrl.Name = "m_PropTabCtrl";
			this.m_PropTabCtrl.SelectedIndex = 0;
			this.m_PropTabCtrl.Size = new System.Drawing.Size(464, 320);
			this.m_PropTabCtrl.TabIndex = 0;
			this.m_PropTabCtrl.SelectedIndexChanged += new System.EventHandler(this.m_PropTabCtrl_SelectedIndexChanged);
			// 
			// m_Tab_Scale
			// 
			this.m_Tab_Scale.Controls.AddRange(new System.Windows.Forms.Control[] {
																																							this.m_Scale_cbRescale,
																																							this.m_Scale_cbType,
																																							this.m_Scale_edTo,
																																							this.m_Scale_edFrom,
																																							this.label5,
																																							this.label4,
																																							this.label3,
																																							this.label2});
			this.m_Tab_Scale.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_Scale.Name = "m_Tab_Scale";
			this.m_Tab_Scale.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_Scale.TabIndex = 0;
			this.m_Tab_Scale.Text = "Scale";
			// 
			// m_Scale_cbRescale
			// 
			this.m_Scale_cbRescale.Location = new System.Drawing.Point(152, 128);
			this.m_Scale_cbRescale.Name = "m_Scale_cbRescale";
			this.m_Scale_cbRescale.Size = new System.Drawing.Size(121, 21);
			this.m_Scale_cbRescale.TabIndex = 9;
			this.m_Scale_cbRescale.Text = "comboBox2";
			this.m_Scale_cbRescale.SelectedIndexChanged += new System.EventHandler(this.OnScale_cbRescale_SelectedIndexChanged);
			// 
			// m_Scale_cbType
			// 
			this.m_Scale_cbType.Location = new System.Drawing.Point(152, 96);
			this.m_Scale_cbType.Name = "m_Scale_cbType";
			this.m_Scale_cbType.Size = new System.Drawing.Size(121, 21);
			this.m_Scale_cbType.TabIndex = 8;
			this.m_Scale_cbType.Text = "comboBox1";
			this.m_Scale_cbType.SelectedIndexChanged += new System.EventHandler(this.OnScale_cbType_SelectedIndexChanged);
			// 
			// m_Scale_edTo
			// 
			this.m_Scale_edTo.Location = new System.Drawing.Point(152, 64);
			this.m_Scale_edTo.Name = "m_Scale_edTo";
			this.m_Scale_edTo.Size = new System.Drawing.Size(120, 20);
			this.m_Scale_edTo.TabIndex = 7;
			this.m_Scale_edTo.Text = "textBox2";
			this.m_Scale_edTo.TextChanged += new System.EventHandler(this.m_Scale_edTo_TextChanged);
			// 
			// m_Scale_edFrom
			// 
			this.m_Scale_edFrom.Location = new System.Drawing.Point(152, 32);
			this.m_Scale_edFrom.Name = "m_Scale_edFrom";
			this.m_Scale_edFrom.Size = new System.Drawing.Size(120, 20);
			this.m_Scale_edFrom.TabIndex = 6;
			this.m_Scale_edFrom.Text = "textBox1";
			this.m_Scale_edFrom.TextChanged += new System.EventHandler(this.m_Scale_edFrom_TextChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(88, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 16);
			this.label5.TabIndex = 5;
			this.label5.Text = "Rescale";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(88, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(32, 16);
			this.label4.TabIndex = 4;
			this.label4.Text = "Type";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(88, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 16);
			this.label3.TabIndex = 3;
			this.label3.Text = "To";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(88, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "From";
			// 
			// m_Tab_TitleAndFormat
			// 
			this.m_Tab_TitleAndFormat.Controls.AddRange(new System.Windows.Forms.Control[] {
																																											 this.m_Format_edAxisPositionValue,
																																											 this.m_Format_cbAxisPosition,
																																											 this.m_Format_cbMinorTicks,
																																											 this.m_Format_cbMajorTicks,
																																											 this.m_Format_cbMajorTickLength,
																																											 this.m_Format_cbThickness,
																																											 this.m_Format_cbColor,
																																											 this.label12,
																																											 this.label11,
																																											 this.label10,
																																											 this.label9,
																																											 this.label8,
																																											 this.label7,
																																											 this.label6,
																																											 this.m_Format_edTitle,
																																											 this.label1,
																																											 this.m_Format_chkShowAxis});
			this.m_Tab_TitleAndFormat.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_TitleAndFormat.Name = "m_Tab_TitleAndFormat";
			this.m_Tab_TitleAndFormat.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_TitleAndFormat.TabIndex = 1;
			this.m_Tab_TitleAndFormat.Text = "Title & Format";
			// 
			// m_Format_edAxisPositionValue
			// 
			this.m_Format_edAxisPositionValue.Location = new System.Drawing.Point(344, 160);
			this.m_Format_edAxisPositionValue.Name = "m_Format_edAxisPositionValue";
			this.m_Format_edAxisPositionValue.Size = new System.Drawing.Size(96, 20);
			this.m_Format_edAxisPositionValue.TabIndex = 16;
			this.m_Format_edAxisPositionValue.Text = "textBox1";
			this.m_Format_edAxisPositionValue.TextChanged += new System.EventHandler(this.OnFormat_edAxisPositionValue_TextChanged);
			// 
			// m_Format_cbAxisPosition
			// 
			this.m_Format_cbAxisPosition.Location = new System.Drawing.Point(344, 120);
			this.m_Format_cbAxisPosition.Name = "m_Format_cbAxisPosition";
			this.m_Format_cbAxisPosition.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbAxisPosition.TabIndex = 15;
			this.m_Format_cbAxisPosition.Text = "comboBox1";
			this.m_Format_cbAxisPosition.TextChanged += new System.EventHandler(this.OnFormat_cbAxisPosition_TextChanged);
			// 
			// m_Format_cbMinorTicks
			// 
			this.m_Format_cbMinorTicks.Location = new System.Drawing.Point(344, 80);
			this.m_Format_cbMinorTicks.Name = "m_Format_cbMinorTicks";
			this.m_Format_cbMinorTicks.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbMinorTicks.TabIndex = 14;
			this.m_Format_cbMinorTicks.Text = "comboBox1";
			this.m_Format_cbMinorTicks.TextChanged += new System.EventHandler(this.OnFormat_cbMinorTicks_TextChanged);
			// 
			// m_Format_cbMajorTicks
			// 
			this.m_Format_cbMajorTicks.Location = new System.Drawing.Point(344, 40);
			this.m_Format_cbMajorTicks.Name = "m_Format_cbMajorTicks";
			this.m_Format_cbMajorTicks.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbMajorTicks.TabIndex = 13;
			this.m_Format_cbMajorTicks.Text = "comboBox1";
			this.m_Format_cbMajorTicks.TextChanged += new System.EventHandler(this.OnFormat_cbMajorTicks_TextChanged);
			// 
			// m_Format_cbMajorTickLength
			// 
			this.m_Format_cbMajorTickLength.Location = new System.Drawing.Point(64, 160);
			this.m_Format_cbMajorTickLength.Name = "m_Format_cbMajorTickLength";
			this.m_Format_cbMajorTickLength.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbMajorTickLength.TabIndex = 12;
			this.m_Format_cbMajorTickLength.Text = "comboBox1";
			this.m_Format_cbMajorTickLength.TextChanged += new System.EventHandler(this.OnFormat_cbMajorTickLength_TextChanged);
			// 
			// m_Format_cbThickness
			// 
			this.m_Format_cbThickness.Location = new System.Drawing.Point(64, 120);
			this.m_Format_cbThickness.Name = "m_Format_cbThickness";
			this.m_Format_cbThickness.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbThickness.TabIndex = 11;
			this.m_Format_cbThickness.Text = "comboBox1";
			this.m_Format_cbThickness.TextChanged += new System.EventHandler(this.OnFormat_cbThickness_TextChanged);
			// 
			// m_Format_cbColor
			// 
			this.m_Format_cbColor.Location = new System.Drawing.Point(64, 80);
			this.m_Format_cbColor.Name = "m_Format_cbColor";
			this.m_Format_cbColor.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbColor.TabIndex = 10;
			this.m_Format_cbColor.Text = "comboBox1";
			this.m_Format_cbColor.TextChanged += new System.EventHandler(this.OnFormat_cbColor_TextChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(248, 168);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(48, 16);
			this.label12.TabIndex = 9;
			this.label12.Text = "Value";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(248, 128);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(80, 16);
			this.label11.TabIndex = 8;
			this.label11.Text = "Axis Position";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(248, 88);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 16);
			this.label10.TabIndex = 7;
			this.label10.Text = "Minor Ticks";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(248, 48);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(64, 16);
			this.label9.TabIndex = 6;
			this.label9.Text = "Major Ticks";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 160);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(56, 32);
			this.label8.TabIndex = 5;
			this.label8.Text = "Major Tick Length";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 4;
			this.label7.Text = "Thickness";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(40, 16);
			this.label6.TabIndex = 3;
			this.label6.Text = "Color";
			// 
			// m_Format_edTitle
			// 
			this.m_Format_edTitle.Location = new System.Drawing.Point(64, 40);
			this.m_Format_edTitle.Name = "m_Format_edTitle";
			this.m_Format_edTitle.TabIndex = 2;
			this.m_Format_edTitle.Text = "textBox1";
			this.m_Format_edTitle.TextChanged += new System.EventHandler(this.OnFormat_edTitle_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Title";
			// 
			// m_Format_chkShowAxis
			// 
			this.m_Format_chkShowAxis.Location = new System.Drawing.Point(64, 8);
			this.m_Format_chkShowAxis.Name = "m_Format_chkShowAxis";
			this.m_Format_chkShowAxis.Size = new System.Drawing.Size(136, 16);
			this.m_Format_chkShowAxis.TabIndex = 0;
			this.m_Format_chkShowAxis.Text = "Show Axis && Ticks";
			this.m_Format_chkShowAxis.TextChanged += new System.EventHandler(this.OnFormat_chkShowAxis_TextChanged);
			this.m_Format_chkShowAxis.CheckedChanged += new System.EventHandler(this.OnFormat_chkShowAxis_CheckedChanged);
			// 
			// m_Tab_Contents
			// 
			this.m_Tab_Contents.Controls.AddRange(new System.Windows.Forms.Control[] {
																																								 this.m_Contents_chkShowRange,
																																								 this.m_Contents_btEditRange,
																																								 this.m_Contents_btUngroup,
																																								 this.m_Contents_btGroup,
																																								 this.m_Contents_btPlotAssociations,
																																								 this.m_Contents_btListSelDown,
																																								 this.m_Contents_btListSelUp,
																																								 this.m_Contents_lbContents,
																																								 this.m_Contents_btPullData,
																																								 this.m_Contents_btPutData,
																																								 this.label14,
																																								 this.m_Content_tvDataAvail,
																																								 this.label13});
			this.m_Tab_Contents.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_Contents.Name = "m_Tab_Contents";
			this.m_Tab_Contents.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_Contents.TabIndex = 2;
			this.m_Tab_Contents.Text = "Contents";
			// 
			// m_Contents_chkShowRange
			// 
			this.m_Contents_chkShowRange.Location = new System.Drawing.Point(344, 240);
			this.m_Contents_chkShowRange.Name = "m_Contents_chkShowRange";
			this.m_Contents_chkShowRange.TabIndex = 12;
			this.m_Contents_chkShowRange.Text = "Show Range";
			this.m_Contents_chkShowRange.CheckedChanged += new System.EventHandler(this.OnContents_chkShowRange_CheckedChange);
			// 
			// m_Contents_btEditRange
			// 
			this.m_Contents_btEditRange.Location = new System.Drawing.Point(344, 208);
			this.m_Contents_btEditRange.Name = "m_Contents_btEditRange";
			this.m_Contents_btEditRange.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btEditRange.TabIndex = 11;
			this.m_Contents_btEditRange.Text = "Edit Range...";
			this.m_Contents_btEditRange.Click += new System.EventHandler(this.OnContents_btEditRange_Click);
			// 
			// m_Contents_btUngroup
			// 
			this.m_Contents_btUngroup.Location = new System.Drawing.Point(344, 176);
			this.m_Contents_btUngroup.Name = "m_Contents_btUngroup";
			this.m_Contents_btUngroup.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btUngroup.TabIndex = 10;
			this.m_Contents_btUngroup.Text = "Ungroup";
			this.m_Contents_btUngroup.Click += new System.EventHandler(this.OnContents_btUngroup_Click);
			// 
			// m_Contents_btGroup
			// 
			this.m_Contents_btGroup.Location = new System.Drawing.Point(344, 144);
			this.m_Contents_btGroup.Name = "m_Contents_btGroup";
			this.m_Contents_btGroup.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btGroup.TabIndex = 9;
			this.m_Contents_btGroup.Text = "Group";
			this.m_Contents_btGroup.Click += new System.EventHandler(this.OnContents_btGroup_Click);
			// 
			// m_Contents_btPlotAssociations
			// 
			this.m_Contents_btPlotAssociations.Location = new System.Drawing.Point(344, 112);
			this.m_Contents_btPlotAssociations.Name = "m_Contents_btPlotAssociations";
			this.m_Contents_btPlotAssociations.Size = new System.Drawing.Size(104, 24);
			this.m_Contents_btPlotAssociations.TabIndex = 8;
			this.m_Contents_btPlotAssociations.Text = "PlotAssociations...";
			this.m_Contents_btPlotAssociations.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.m_Contents_btPlotAssociations.Click += new System.EventHandler(this.OnContents_btPlotAssociations_Click);
			// 
			// m_Contents_btListSelDown
			// 
			this.m_Contents_btListSelDown.Image = ((System.Drawing.Bitmap)(resources.GetObject("m_Contents_btListSelDown.Image")));
			this.m_Contents_btListSelDown.Location = new System.Drawing.Point(344, 72);
			this.m_Contents_btListSelDown.Name = "m_Contents_btListSelDown";
			this.m_Contents_btListSelDown.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btListSelDown.TabIndex = 7;
			this.m_Contents_btListSelDown.Click += new System.EventHandler(this.OnContents_btListSelDown_Click);
			// 
			// m_Contents_btListSelUp
			// 
			this.m_Contents_btListSelUp.Image = ((System.Drawing.Bitmap)(resources.GetObject("m_Contents_btListSelUp.Image")));
			this.m_Contents_btListSelUp.Location = new System.Drawing.Point(344, 32);
			this.m_Contents_btListSelUp.Name = "m_Contents_btListSelUp";
			this.m_Contents_btListSelUp.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btListSelUp.TabIndex = 6;
			this.m_Contents_btListSelUp.Click += new System.EventHandler(this.OnContent_btListSelUp_Click);
			// 
			// m_Contents_lbContents
			// 
			this.m_Contents_lbContents.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.m_Contents_lbContents.Location = new System.Drawing.Point(200, 24);
			this.m_Contents_lbContents.Name = "m_Contents_lbContents";
			this.m_Contents_lbContents.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.m_Contents_lbContents.Size = new System.Drawing.Size(136, 238);
			this.m_Contents_lbContents.TabIndex = 5;
			this.m_Contents_lbContents.DoubleClick += new System.EventHandler(this.OnContents_lbContents_DoubleClick);
			this.m_Contents_lbContents.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.OnContents_lbContents_MeasureItem);
			this.m_Contents_lbContents.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.OnContents_lbContents_DrawItem);
			this.m_Contents_lbContents.SelectedIndexChanged += new System.EventHandler(this.OnContents_lbContents_SelectedIndexChanged);
			// 
			// m_Contents_btPullData
			// 
			this.m_Contents_btPullData.Image = ((System.Drawing.Bitmap)(resources.GetObject("m_Contents_btPullData.Image")));
			this.m_Contents_btPullData.Location = new System.Drawing.Point(160, 72);
			this.m_Contents_btPullData.Name = "m_Contents_btPullData";
			this.m_Contents_btPullData.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btPullData.TabIndex = 4;
			this.m_Contents_btPullData.Click += new System.EventHandler(this.OnContents_PullData_Click);
			// 
			// m_Contents_btPutData
			// 
			this.m_Contents_btPutData.Image = ((System.Drawing.Bitmap)(resources.GetObject("m_Contents_btPutData.Image")));
			this.m_Contents_btPutData.Location = new System.Drawing.Point(160, 32);
			this.m_Contents_btPutData.Name = "m_Contents_btPutData";
			this.m_Contents_btPutData.Size = new System.Drawing.Size(32, 32);
			this.m_Contents_btPutData.TabIndex = 3;
			this.m_Contents_btPutData.Click += new System.EventHandler(this.OnContents_btPutData_Click);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(200, 8);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(88, 16);
			this.label14.TabIndex = 2;
			this.label14.Text = "Layer Contents";
			// 
			// m_Content_tvDataAvail
			// 
			this.m_Content_tvDataAvail.CheckBoxes = true;
			this.m_Content_tvDataAvail.ImageIndex = -1;
			this.m_Content_tvDataAvail.Location = new System.Drawing.Point(0, 24);
			this.m_Content_tvDataAvail.Name = "m_Content_tvDataAvail";
			this.m_Content_tvDataAvail.SelectedImageIndex = -1;
			this.m_Content_tvDataAvail.Size = new System.Drawing.Size(152, 240);
			this.m_Content_tvDataAvail.TabIndex = 1;
			this.m_Content_tvDataAvail.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnContents_tvDataAvailBeforeExpand);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(0, 8);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 16);
			this.label13.TabIndex = 0;
			this.label13.Text = "Available data";
			// 
			// m_Tab_Layer
			// 
			this.m_Tab_Layer.Controls.AddRange(new System.Windows.Forms.Control[] {
																																							this.m_Layer_gbYAxis,
																																							this.m_Layer_gbXAxis,
																																							this.m_Layer_cbHeightType,
																																							this.m_Layer_cbWidthType,
																																							this.m_Layer_cbTopType,
																																							this.m_Layer_cbLeftType,
																																							this.m_Layer_cbLinkedLayer,
																																							this.label21,
																																							this.m_Layer_edScale,
																																							this.label20,
																																							this.m_Layer_edRotation,
																																							this.label19,
																																							this.m_Layer_edHeight,
																																							this.label18,
																																							this.m_Layer_edWidth,
																																							this.label17,
																																							this.m_Layer_edTopPosition,
																																							this.label16,
																																							this.m_Layer_edLeftPosition,
																																							this.label15});
			this.m_Tab_Layer.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_Layer.Name = "m_Tab_Layer";
			this.m_Tab_Layer.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_Layer.TabIndex = 3;
			this.m_Tab_Layer.Text = "Layer";
			// 
			// m_Layer_gbYAxis
			// 
			this.m_Layer_gbYAxis.Controls.AddRange(new System.Windows.Forms.Control[] {
																																									this.m_Layer_edLinkYAxisEndB,
																																									this.m_Layer_edLinkYAxisEndA,
																																									this.m_Layer_edLinkYAxisOrgB,
																																									this.m_Layer_edLinkYAxisOrgA,
																																									this.label30,
																																									this.label29,
																																									this.label28,
																																									this.label27,
																																									this.m_Layer_rbLinkYAxisCustom,
																																									this.m_Layer_rbLinkYAxisStraight,
																																									this.m_Layer_rbLinkYAxisNone});
			this.m_Layer_gbYAxis.Location = new System.Drawing.Point(296, 144);
			this.m_Layer_gbYAxis.Name = "m_Layer_gbYAxis";
			this.m_Layer_gbYAxis.Size = new System.Drawing.Size(152, 136);
			this.m_Layer_gbYAxis.TabIndex = 20;
			this.m_Layer_gbYAxis.TabStop = false;
			this.m_Layer_gbYAxis.Text = "Link Y Axis:";
			// 
			// m_Layer_edLinkYAxisEndB
			// 
			this.m_Layer_edLinkYAxisEndB.Location = new System.Drawing.Point(96, 112);
			this.m_Layer_edLinkYAxisEndB.Name = "m_Layer_edLinkYAxisEndB";
			this.m_Layer_edLinkYAxisEndB.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkYAxisEndB.TabIndex = 17;
			this.m_Layer_edLinkYAxisEndB.Text = "";
			// 
			// m_Layer_edLinkYAxisEndA
			// 
			this.m_Layer_edLinkYAxisEndA.Location = new System.Drawing.Point(40, 112);
			this.m_Layer_edLinkYAxisEndA.Name = "m_Layer_edLinkYAxisEndA";
			this.m_Layer_edLinkYAxisEndA.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkYAxisEndA.TabIndex = 16;
			this.m_Layer_edLinkYAxisEndA.Text = "";
			// 
			// m_Layer_edLinkYAxisOrgB
			// 
			this.m_Layer_edLinkYAxisOrgB.Location = new System.Drawing.Point(96, 88);
			this.m_Layer_edLinkYAxisOrgB.Name = "m_Layer_edLinkYAxisOrgB";
			this.m_Layer_edLinkYAxisOrgB.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkYAxisOrgB.TabIndex = 15;
			this.m_Layer_edLinkYAxisOrgB.Text = "";
			// 
			// m_Layer_edLinkYAxisOrgA
			// 
			this.m_Layer_edLinkYAxisOrgA.Location = new System.Drawing.Point(40, 88);
			this.m_Layer_edLinkYAxisOrgA.Name = "m_Layer_edLinkYAxisOrgA";
			this.m_Layer_edLinkYAxisOrgA.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkYAxisOrgA.TabIndex = 14;
			this.m_Layer_edLinkYAxisOrgA.Text = "";
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(8, 112);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(24, 16);
			this.label30.TabIndex = 13;
			this.label30.Text = "End";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(8, 88);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(24, 16);
			this.label29.TabIndex = 12;
			this.label29.Text = "Org";
			this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(96, 72);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(16, 8);
			this.label28.TabIndex = 11;
			this.label28.Text = "b";
			this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label27
			// 
			this.label27.Location = new System.Drawing.Point(48, 72);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(16, 8);
			this.label27.TabIndex = 10;
			this.label27.Text = "a";
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_rbLinkYAxisCustom
			// 
			this.m_Layer_rbLinkYAxisCustom.Location = new System.Drawing.Point(8, 48);
			this.m_Layer_rbLinkYAxisCustom.Name = "m_Layer_rbLinkYAxisCustom";
			this.m_Layer_rbLinkYAxisCustom.TabIndex = 3;
			this.m_Layer_rbLinkYAxisCustom.Text = "Custom (a+bx)";
			this.m_Layer_rbLinkYAxisCustom.CheckedChanged += new System.EventHandler(this.OnLayer_LinkYAxisCheckedChanged);
			// 
			// m_Layer_rbLinkYAxisStraight
			// 
			this.m_Layer_rbLinkYAxisStraight.Location = new System.Drawing.Point(8, 32);
			this.m_Layer_rbLinkYAxisStraight.Name = "m_Layer_rbLinkYAxisStraight";
			this.m_Layer_rbLinkYAxisStraight.TabIndex = 2;
			this.m_Layer_rbLinkYAxisStraight.Text = "Straight (1:1)";
			this.m_Layer_rbLinkYAxisStraight.CheckedChanged += new System.EventHandler(this.OnLayer_LinkYAxisCheckedChanged);
			// 
			// m_Layer_rbLinkYAxisNone
			// 
			this.m_Layer_rbLinkYAxisNone.Location = new System.Drawing.Point(8, 16);
			this.m_Layer_rbLinkYAxisNone.Name = "m_Layer_rbLinkYAxisNone";
			this.m_Layer_rbLinkYAxisNone.TabIndex = 1;
			this.m_Layer_rbLinkYAxisNone.Text = "None";
			this.m_Layer_rbLinkYAxisNone.CheckedChanged += new System.EventHandler(this.OnLayer_LinkYAxisCheckedChanged);
			// 
			// m_Layer_gbXAxis
			// 
			this.m_Layer_gbXAxis.Controls.AddRange(new System.Windows.Forms.Control[] {
																																									this.label26,
																																									this.label25,
																																									this.label24,
																																									this.m_Layer_edLinkXAxisEndB,
																																									this.m_Layer_edLinkXAxisEndA,
																																									this.m_Layer_edLinkXAxisOrgB,
																																									this.m_Layer_edLinkXAxisOrgA,
																																									this.label23,
																																									this.m_Layer_rbLinkXAxisCustom,
																																									this.m_Layer_rbLinkXAxisStraight,
																																									this.m_Layer_rbLinkXAxisNone});
			this.m_Layer_gbXAxis.Location = new System.Drawing.Point(296, 0);
			this.m_Layer_gbXAxis.Name = "m_Layer_gbXAxis";
			this.m_Layer_gbXAxis.Size = new System.Drawing.Size(152, 136);
			this.m_Layer_gbXAxis.TabIndex = 19;
			this.m_Layer_gbXAxis.TabStop = false;
			this.m_Layer_gbXAxis.Text = "Link X Axis:";
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(96, 72);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(16, 8);
			this.label26.TabIndex = 10;
			this.label26.Text = "b";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(48, 72);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(16, 8);
			this.label25.TabIndex = 9;
			this.label25.Text = "a";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(8, 112);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(24, 16);
			this.label24.TabIndex = 8;
			this.label24.Text = "End";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_edLinkXAxisEndB
			// 
			this.m_Layer_edLinkXAxisEndB.Location = new System.Drawing.Point(88, 112);
			this.m_Layer_edLinkXAxisEndB.Name = "m_Layer_edLinkXAxisEndB";
			this.m_Layer_edLinkXAxisEndB.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisEndB.TabIndex = 7;
			this.m_Layer_edLinkXAxisEndB.Text = "";
			// 
			// m_Layer_edLinkXAxisEndA
			// 
			this.m_Layer_edLinkXAxisEndA.Location = new System.Drawing.Point(40, 112);
			this.m_Layer_edLinkXAxisEndA.Name = "m_Layer_edLinkXAxisEndA";
			this.m_Layer_edLinkXAxisEndA.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisEndA.TabIndex = 6;
			this.m_Layer_edLinkXAxisEndA.Text = "";
			// 
			// m_Layer_edLinkXAxisOrgB
			// 
			this.m_Layer_edLinkXAxisOrgB.Location = new System.Drawing.Point(88, 88);
			this.m_Layer_edLinkXAxisOrgB.Name = "m_Layer_edLinkXAxisOrgB";
			this.m_Layer_edLinkXAxisOrgB.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisOrgB.TabIndex = 5;
			this.m_Layer_edLinkXAxisOrgB.Text = "";
			// 
			// m_Layer_edLinkXAxisOrgA
			// 
			this.m_Layer_edLinkXAxisOrgA.Location = new System.Drawing.Point(40, 88);
			this.m_Layer_edLinkXAxisOrgA.Name = "m_Layer_edLinkXAxisOrgA";
			this.m_Layer_edLinkXAxisOrgA.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisOrgA.TabIndex = 4;
			this.m_Layer_edLinkXAxisOrgA.Text = "";
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(8, 88);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(24, 16);
			this.label23.TabIndex = 3;
			this.label23.Text = "Org";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_rbLinkXAxisCustom
			// 
			this.m_Layer_rbLinkXAxisCustom.Location = new System.Drawing.Point(8, 48);
			this.m_Layer_rbLinkXAxisCustom.Name = "m_Layer_rbLinkXAxisCustom";
			this.m_Layer_rbLinkXAxisCustom.TabIndex = 2;
			this.m_Layer_rbLinkXAxisCustom.Text = "Custom (a+bx)";
			this.m_Layer_rbLinkXAxisCustom.CheckedChanged += new System.EventHandler(this.OnLayer_LinkXAxisCheckedChanged);
			// 
			// m_Layer_rbLinkXAxisStraight
			// 
			this.m_Layer_rbLinkXAxisStraight.Location = new System.Drawing.Point(8, 32);
			this.m_Layer_rbLinkXAxisStraight.Name = "m_Layer_rbLinkXAxisStraight";
			this.m_Layer_rbLinkXAxisStraight.TabIndex = 1;
			this.m_Layer_rbLinkXAxisStraight.Text = "Straight (1:1)";
			this.m_Layer_rbLinkXAxisStraight.CheckedChanged += new System.EventHandler(this.OnLayer_LinkXAxisCheckedChanged);
			// 
			// m_Layer_rbLinkXAxisNone
			// 
			this.m_Layer_rbLinkXAxisNone.Location = new System.Drawing.Point(8, 16);
			this.m_Layer_rbLinkXAxisNone.Name = "m_Layer_rbLinkXAxisNone";
			this.m_Layer_rbLinkXAxisNone.TabIndex = 0;
			this.m_Layer_rbLinkXAxisNone.Text = "None";
			this.m_Layer_rbLinkXAxisNone.CheckedChanged += new System.EventHandler(this.OnLayer_LinkXAxisCheckedChanged);
			// 
			// m_Layer_cbHeightType
			// 
			this.m_Layer_cbHeightType.Location = new System.Drawing.Point(96, 128);
			this.m_Layer_cbHeightType.Name = "m_Layer_cbHeightType";
			this.m_Layer_cbHeightType.Size = new System.Drawing.Size(168, 21);
			this.m_Layer_cbHeightType.TabIndex = 17;
			this.m_Layer_cbHeightType.Text = "comboBox1";
			// 
			// m_Layer_cbWidthType
			// 
			this.m_Layer_cbWidthType.Location = new System.Drawing.Point(96, 104);
			this.m_Layer_cbWidthType.Name = "m_Layer_cbWidthType";
			this.m_Layer_cbWidthType.Size = new System.Drawing.Size(168, 21);
			this.m_Layer_cbWidthType.TabIndex = 16;
			this.m_Layer_cbWidthType.Text = "comboBox1";
			// 
			// m_Layer_cbTopType
			// 
			this.m_Layer_cbTopType.Location = new System.Drawing.Point(96, 72);
			this.m_Layer_cbTopType.Name = "m_Layer_cbTopType";
			this.m_Layer_cbTopType.Size = new System.Drawing.Size(168, 21);
			this.m_Layer_cbTopType.TabIndex = 15;
			this.m_Layer_cbTopType.Text = "comboBox1";
			// 
			// m_Layer_cbLeftType
			// 
			this.m_Layer_cbLeftType.Location = new System.Drawing.Point(96, 48);
			this.m_Layer_cbLeftType.Name = "m_Layer_cbLeftType";
			this.m_Layer_cbLeftType.Size = new System.Drawing.Size(168, 21);
			this.m_Layer_cbLeftType.TabIndex = 14;
			this.m_Layer_cbLeftType.Text = "comboBox1";
			this.m_Layer_cbLeftType.SelectionChangeCommitted += new System.EventHandler(this.OnLayer_cbLeftType_SelectionChangeCommitted);
			// 
			// m_Layer_cbLinkedLayer
			// 
			this.m_Layer_cbLinkedLayer.Location = new System.Drawing.Point(96, 8);
			this.m_Layer_cbLinkedLayer.Name = "m_Layer_cbLinkedLayer";
			this.m_Layer_cbLinkedLayer.Size = new System.Drawing.Size(168, 21);
			this.m_Layer_cbLinkedLayer.TabIndex = 13;
			this.m_Layer_cbLinkedLayer.Text = "None";
			this.m_Layer_cbLinkedLayer.SelectedValueChanged += new System.EventHandler(this.OnLayer_cbLinkedLayer_SelectedValueChanged);
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(8, 8);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(88, 16);
			this.label21.TabIndex = 12;
			this.label21.Text = "Linked to layer:";
			// 
			// m_Layer_edScale
			// 
			this.m_Layer_edScale.Location = new System.Drawing.Point(56, 192);
			this.m_Layer_edScale.Name = "m_Layer_edScale";
			this.m_Layer_edScale.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edScale.TabIndex = 11;
			this.m_Layer_edScale.Text = "";
			this.m_Layer_edScale.TextChanged += new System.EventHandler(this.OnLayer_edScale_TextChanged);
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(8, 192);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(48, 16);
			this.label20.TabIndex = 10;
			this.label20.Text = "Scale";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_Layer_edRotation
			// 
			this.m_Layer_edRotation.Location = new System.Drawing.Point(56, 168);
			this.m_Layer_edRotation.Name = "m_Layer_edRotation";
			this.m_Layer_edRotation.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edRotation.TabIndex = 9;
			this.m_Layer_edRotation.Text = "";
			this.m_Layer_edRotation.TextChanged += new System.EventHandler(this.OnLayer_edRotation_TextChanged);
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(8, 168);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(48, 16);
			this.label19.TabIndex = 8;
			this.label19.Text = "Rotation";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_Layer_edHeight
			// 
			this.m_Layer_edHeight.Location = new System.Drawing.Point(40, 128);
			this.m_Layer_edHeight.Name = "m_Layer_edHeight";
			this.m_Layer_edHeight.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edHeight.TabIndex = 7;
			this.m_Layer_edHeight.Text = "";
			this.m_Layer_edHeight.TextChanged += new System.EventHandler(this.OnLayer_edHeight_TextChanged);
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(0, 128);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(40, 16);
			this.label18.TabIndex = 6;
			this.label18.Text = "Height";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_edWidth
			// 
			this.m_Layer_edWidth.Location = new System.Drawing.Point(40, 104);
			this.m_Layer_edWidth.Name = "m_Layer_edWidth";
			this.m_Layer_edWidth.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edWidth.TabIndex = 5;
			this.m_Layer_edWidth.Text = "";
			this.m_Layer_edWidth.TextChanged += new System.EventHandler(this.OnLayer_edWidth_TextChanged);
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(0, 104);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(40, 16);
			this.label17.TabIndex = 4;
			this.label17.Text = "Width";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_edTopPosition
			// 
			this.m_Layer_edTopPosition.Location = new System.Drawing.Point(40, 72);
			this.m_Layer_edTopPosition.Name = "m_Layer_edTopPosition";
			this.m_Layer_edTopPosition.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edTopPosition.TabIndex = 3;
			this.m_Layer_edTopPosition.Text = "";
			this.m_Layer_edTopPosition.TextChanged += new System.EventHandler(this.OnLayer_edTopPosition_TextChanged);
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(16, 72);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(24, 16);
			this.label16.TabIndex = 2;
			this.label16.Text = "Top";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_Layer_edLeftPosition
			// 
			this.m_Layer_edLeftPosition.Location = new System.Drawing.Point(40, 48);
			this.m_Layer_edLeftPosition.Name = "m_Layer_edLeftPosition";
			this.m_Layer_edLeftPosition.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLeftPosition.TabIndex = 1;
			this.m_Layer_edLeftPosition.Text = "";
			this.m_Layer_edLeftPosition.TextChanged += new System.EventHandler(this.OnLayer_edLeftPosition_TextChanged);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(16, 48);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(24, 16);
			this.label15.TabIndex = 0;
			this.label15.Text = "Left";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_Tab_MajorLabels
			// 
			this.m_Tab_MajorLabels.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_MajorLabels.Name = "m_Tab_MajorLabels";
			this.m_Tab_MajorLabels.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_MajorLabels.TabIndex = 4;
			this.m_Tab_MajorLabels.Text = "Major Labels";
			// 
			// m_Tab_MinorLabels
			// 
			this.m_Tab_MinorLabels.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_MinorLabels.Name = "m_Tab_MinorLabels";
			this.m_Tab_MinorLabels.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_MinorLabels.TabIndex = 5;
			this.m_Tab_MinorLabels.Text = "Minor Labels";
			// 
			// m_Tab_Grid
			// 
			this.m_Tab_Grid.Location = new System.Drawing.Point(4, 22);
			this.m_Tab_Grid.Name = "m_Tab_Grid";
			this.m_Tab_Grid.Size = new System.Drawing.Size(456, 294);
			this.m_Tab_Grid.TabIndex = 6;
			this.m_Tab_Grid.Text = "Grid";
			// 
			// m_Common_lbEdges
			// 
			this.m_Common_lbEdges.Location = new System.Drawing.Point(8, 56);
			this.m_Common_lbEdges.Name = "m_Common_lbEdges";
			this.m_Common_lbEdges.Size = new System.Drawing.Size(64, 121);
			this.m_Common_lbEdges.TabIndex = 1;
			this.m_Common_lbEdges.SelectedIndexChanged += new System.EventHandler(this.OnCommonLbEdgesSelIndexChanged);
			// 
			// m_Main_btOK
			// 
			this.m_Main_btOK.Location = new System.Drawing.Point(88, 360);
			this.m_Main_btOK.Name = "m_Main_btOK";
			this.m_Main_btOK.Size = new System.Drawing.Size(56, 24);
			this.m_Main_btOK.TabIndex = 2;
			this.m_Main_btOK.Text = "OK";
			this.m_Main_btOK.Click += new System.EventHandler(this.OnMain_btOK_Click);
			// 
			// m_Main_btCancel
			// 
			this.m_Main_btCancel.Location = new System.Drawing.Point(192, 360);
			this.m_Main_btCancel.Name = "m_Main_btCancel";
			this.m_Main_btCancel.Size = new System.Drawing.Size(48, 24);
			this.m_Main_btCancel.TabIndex = 3;
			this.m_Main_btCancel.Text = "Cancel";
			this.m_Main_btCancel.Click += new System.EventHandler(this.OnMain_btCancel_Click);
			// 
			// m_Main_btApply
			// 
			this.m_Main_btApply.Location = new System.Drawing.Point(280, 360);
			this.m_Main_btApply.Name = "m_Main_btApply";
			this.m_Main_btApply.Size = new System.Drawing.Size(48, 24);
			this.m_Main_btApply.TabIndex = 4;
			this.m_Main_btApply.Text = "Apply";
			this.m_Main_btApply.Click += new System.EventHandler(this.OnMain_btApply_Click);
			// 
			// LayerDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(560, 386);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.m_Main_btApply,
																																	this.m_Main_btCancel,
																																	this.m_Main_btOK,
																																	this.m_Common_lbEdges,
																																	this.m_PropTabCtrl});
			this.Name = "LayerDialog";
			this.Text = "LayerDialog";
			this.m_PropTabCtrl.ResumeLayout(false);
			this.m_Tab_Scale.ResumeLayout(false);
			this.m_Tab_TitleAndFormat.ResumeLayout(false);
			this.m_Tab_Contents.ResumeLayout(false);
			this.m_Tab_Layer.ResumeLayout(false);
			this.m_Layer_gbYAxis.ResumeLayout(false);
			this.m_Layer_gbXAxis.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void m_PropTabCtrl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(			m_PProp[m_PropTabCtrl.SelectedIndex].IsUninitialized() 
				&&	null!=m_PProp[m_PropTabCtrl.SelectedIndex].InitValues)
				m_PProp[m_PropTabCtrl.SelectedIndex].InitValues();
		}


		private void ApplyValues()
		{
			for(int i=0;i<pgLastPage;i++)
			{
				if(this.m_PProp[i].IsDirty())
				{
					if(null!=this.m_PProp[i].ApplyValues)
						this.m_PProp[i].ApplyValues();
				}
			}

		}

		private void SetCurrentEdge(EdgeType newedge)
		{
			if(newedge==this.m_CurrentEdge)
				return; // nothing is to do if nothing changed

			// see if some page changed

			bool bSomePageIsDirty=false;
			int i;
			for(i=0;i<pgLastPage;i++)
			{
				if(this.m_PProp[i].IsDirty())
					bSomePageIsDirty=true;
			}

			if(bSomePageIsDirty)
			{
				// show dialog box to confirm the changes
				DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this,"Do you want to apply the changed values?","Confirmation",MessageBoxButtons.YesNo);
			
				if(dlgres == DialogResult.Yes)
				{
					ApplyValues();
				}
				else // no confirmation, so set back all tabs to not initialized
				{
					for(i=0;i<pgLastPage;i++)
					{
						this.m_PProp[i].SetUninitialized();
					}
				}
			}

			// now set the new edge
			m_CurrentEdge = newedge;
			// and initialize the values in every case, since the edge changed
			if(null!=m_PProp[SelectedPage].InitValues)
				m_PProp[SelectedPage].InitValues();

		}

		private void OnMain_btOK_Click(object sender, System.EventArgs e)
		{
			ApplyValues();
			this.DialogResult=DialogResult.OK;
			this.Close();
		}

		private void OnMain_btCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void OnMain_btApply_Click(object sender, System.EventArgs e)
		{
			ApplyValues();
		}


		private void OnCommonLbEdgesSelIndexChanged(object sender, System.EventArgs e)
		{
			string selitm = this.m_Common_lbEdges.SelectedItem.ToString();

			switch(selitm)
			{
				case "Horizontal":
					if(m_CurrentEdge!=EdgeType.Bottom && m_CurrentEdge!=EdgeType.Top)
						SetCurrentEdge(EdgeType.Bottom);
					break;
				case "Vertical":
					if(m_CurrentEdge!=EdgeType.Left && m_CurrentEdge!=EdgeType.Right)
						SetCurrentEdge(EdgeType.Left);
					break;
				case "Bottom":
					SetCurrentEdge(EdgeType.Bottom);
					break;
				case "Left":
					SetCurrentEdge(EdgeType.Left);
					break;
				case "Top":
					SetCurrentEdge(EdgeType.Top);
					break;
				case "Right":
					SetCurrentEdge(EdgeType.Right);
					break;
			} // end switch
		
		}


		#region Tab_Scale_Methods



		private bool m_Scale_FromOrToChanged=false; // is true if From or To was changed
		private void InitTabScale()
		{
			// first set the current listbox item
			// 0==Horizontal, 1==Vertikal
			int selidx=0;
			if(m_CurrentEdge==EdgeType.Left || m_CurrentEdge==EdgeType.Right)
				selidx = 1;

			this.m_Common_lbEdges.Items.Clear();
			this.m_Common_lbEdges.Items.Add("Horizontal");
			this.m_Common_lbEdges.Items.Add("Vertical");
			this.m_Common_lbEdges.SelectedIndex=selidx;
		


			Axis currAxis = 0==selidx ? m_Layer.XAxis : m_Layer.YAxis;
			m_Scale_edFrom.Text = currAxis.Org.ToString();
			m_Scale_edTo.Text   = currAxis.End.ToString();


			// fill the 
			this.m_Scale_cbRescale.Items.Clear();
			this.m_Scale_cbRescale.Items.Add("automatic");
			this.m_Scale_cbRescale.Items.Add("org fixed");
			this.m_Scale_cbRescale.Items.Add("end fixed");
			this.m_Scale_cbRescale.Items.Add("both fixed");

			if(!currAxis.OrgFixed && !currAxis.EndFixed)
				this.m_Scale_cbRescale.SelectedIndex = 0;
			else if(currAxis.OrgFixed && !currAxis.EndFixed)
				this.m_Scale_cbRescale.SelectedIndex = 1;
			else if(!currAxis.OrgFixed && currAxis.EndFixed)
				this.m_Scale_cbRescale.SelectedIndex = 2;
			else 
				this.m_Scale_cbRescale.SelectedIndex = 3;
				

			// axis types
			this.m_Scale_cbType.Items.Clear();
			string curraxisname=null;
			foreach(string axs in Axis.AvailableAxes.Keys)
			{
				this.m_Scale_cbType.Items.Add(axs);
				if(currAxis.GetType()==Axis.AvailableAxes[axs])
					curraxisname = axs;
			}
			// set the selection to the current axis
			if(null!=curraxisname)
			{
				this.m_Scale_cbType.SelectedItem = curraxisname;
			}
			else
			{
				// if the current axis is not a item in the list, add the item to the list first
				// and then set the selected item
				this.m_Scale_cbType.Items.Add(currAxis.GetType().Name);
				this.m_Scale_cbType.SelectedItem = currAxis.GetType().Name;
			}

			// now the page is initialized, so set its state
			m_PProp[pgScale].SetInitialized();
			this.m_Scale_FromOrToChanged=false;
		}

		protected int ApplyTabScale()
		{
			Axis currAxis = (m_CurrentEdge==EdgeType.Bottom || m_CurrentEdge==EdgeType.Top) ? m_Layer.XAxis : m_Layer.YAxis;

			switch(this.m_Scale_cbRescale.SelectedIndex)
			{
				default:
				case 0:
					currAxis.OrgFixed = false; currAxis.EndFixed=false;
					break;
				case 1:
					currAxis.OrgFixed = true; currAxis.EndFixed=false;
					break;
				case 2:
					currAxis.OrgFixed = false; currAxis.EndFixed=true;
					break;
				case 3:
					currAxis.OrgFixed = true; currAxis.EndFixed=true;
					break;
			} // end switch

			double org = System.Convert.ToDouble(m_Scale_edFrom.Text);
			double end = System.Convert.ToDouble(m_Scale_edTo.Text);


			// retrieve the axis type from the dialog box and compare it
			// with the current type
			string axisname = this.m_Scale_cbType.SelectedItem.ToString();
			System.Type axistype = (System.Type)Axis.AvailableAxes[axisname];
			if(null!=axistype)
			{
				if(axistype!=currAxis.GetType())
				{
					// replace the current axis by a new axis of the type axistype
					currAxis = (Axis)System.Activator.CreateInstance(axistype);

					if((m_CurrentEdge==EdgeType.Bottom || m_CurrentEdge==EdgeType.Top))
						m_Layer.XAxis = currAxis;
					else
						m_Layer.YAxis = currAxis;
				}
			}

			if(this.m_Scale_FromOrToChanged)
				currAxis.ProcessDataBounds(org,true,end,true);
			return 0; // all ok
		}


		private void m_Scale_edFrom_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgScale].SetDirty();	
			this.m_Scale_FromOrToChanged=true;
		}

		private void m_Scale_edTo_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgScale].SetDirty();	
			this.m_Scale_FromOrToChanged=true;
		}

		private void OnScale_cbType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgScale].SetDirty();	
		}

		private void OnScale_cbRescale_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgScale].SetDirty();	
		}


		#endregion

		#region Tab_Format_Methods

		private void InitTabFormat()
		{
			string name;
			string [] names;

			XYLayerAxisStyle axstyle=null;
			string title=null;
			bool bAxisEnabled=false;
			switch(m_CurrentEdge)
			{
				case EdgeType.Left:
					axstyle = m_Layer.LeftAxisStyle;
					title   = m_Layer.LeftAxisTitleString;
					bAxisEnabled = m_Layer.LeftAxisEnabled;
					break;
				case EdgeType.Right:
					axstyle = m_Layer.RightAxisStyle;
					title   = m_Layer.RightAxisTitleString;
					bAxisEnabled = m_Layer.RightAxisEnabled;
					break;
				case EdgeType.Bottom:
					axstyle = m_Layer.BottomAxisStyle;
					title   = m_Layer.BottomAxisTitleString;
					bAxisEnabled = m_Layer.BottomAxisEnabled;
					break;
				case EdgeType.Top:
					axstyle = m_Layer.TopAxisStyle;
					title   = m_Layer.TopAxisTitleString;
					bAxisEnabled = m_Layer.TopAxisEnabled;
					break;
			}

			this.m_Format_chkShowAxis.Checked = bAxisEnabled;


			this.m_Common_lbEdges.Items.Clear();
			names = System.Enum.GetNames(typeof(Graph.EdgeType));
			foreach(string edgename in names)
			{
				this.m_Common_lbEdges.Items.Add(edgename);
			}
			this.m_Common_lbEdges.SelectedIndex = (int)m_CurrentEdge;

			// fill axis title box
			this.m_Format_edTitle.Text = null!=title ? title : "";

			// fill axis thickness combo box
			this.m_Format_cbThickness.Items.Clear();
			object [] thicknesses = new object[]{0.0,0.2,0.5,1.0,1.5,2.0,3.0,4.0,5.0};
			this.m_Format_cbThickness.Items.AddRange(thicknesses);
			this.m_Format_cbThickness.Text = axstyle.Thickness.ToString();
			
			// fill major tick lenght combo box
			this.m_Format_cbMajorTickLength.Items.Clear();
			object [] ticklengths = new object[]{3,4,5,6,8,10,12,15,18,24,32};
			this.m_Format_cbMajorTickLength.Items.AddRange(ticklengths);
			this.m_Format_cbMajorTickLength.Text = axstyle.MajorTickLength.ToString();

			// fill the color dialog box
			this.m_Format_cbColor.Items.Clear();
			this.m_Format_cbColor.Items.Add("Custom");
			foreach(Color c in PlotStyle.PlotColors)
			{
				name = c.ToString();
				this.m_Format_cbColor.Items.Add(name.Substring(7,name.Length-8));
			}
			name = PlotStyle.GetPlotColorName(axstyle.Color);
			if(null==name)
				name = "Custom";
			this.m_Format_cbColor.SelectedItem = name;


			// fill the Major Ticks combo box
			this.m_Format_cbMajorTicks.Items.Clear();
			this.m_Format_cbMajorTicks.Items.AddRange(new Object[]{"None","In","Out","In&Out"});
			this.m_Format_cbMajorTicks.SelectedIndex = (axstyle.InnerMajorTicks?1:0) + (axstyle.OuterMajorTicks?2:0); 

			// fill the Minor Ticks combo box
			this.m_Format_cbMinorTicks.Items.Clear();
			this.m_Format_cbMinorTicks.Items.AddRange(new Object[]{"None","In","Out","In&Out"});
			this.m_Format_cbMinorTicks.SelectedIndex = (axstyle.InnerMinorTicks?1:0) + (axstyle.OuterMinorTicks?2:0); 
		

			// fill the position combo box
			this.m_Format_cbAxisPosition.Items.Clear();
			this.m_Format_cbAxisPosition.Items.Add(m_CurrentEdge.ToString());
			this.m_Format_cbAxisPosition.Items.Add("% from " + m_CurrentEdge.ToString());
			this.m_Format_cbAxisPosition.Items.Add("At position =");

			if(axstyle.Position.Value==0)
			{
				this.m_Format_cbAxisPosition.SelectedIndex=0;
				this.m_Format_edAxisPositionValue.Text = "";
				this.m_Format_edAxisPositionValue.Enabled = false;
			}
			else if(axstyle.Position.IsRelative)
			{
				this.m_Format_cbAxisPosition.SelectedIndex=1;
				this.m_Format_edAxisPositionValue.Text = (100.0*axstyle.Position.Value).ToString();
				this.m_Format_edAxisPositionValue.Enabled = true;
			}
			else
			{
				this.m_Format_cbAxisPosition.SelectedIndex=2;
				this.m_Format_edAxisPositionValue.Text = axstyle.Position.Value.ToString();
				this.m_Format_edAxisPositionValue.Enabled = true;
			}


			m_PProp[pgFormat].SetInitialized();
		}


		protected int ApplyTabFormat()
		{
			XYLayerAxisStyle axstyle=null;
			switch(m_CurrentEdge)
			{
				case EdgeType.Left:
					axstyle = m_Layer.LeftAxisStyle;
					break;
				case EdgeType.Right:
					axstyle = m_Layer.RightAxisStyle;
					break;
				case EdgeType.Bottom:
					axstyle = m_Layer.BottomAxisStyle;
					break;
				case EdgeType.Top:
					axstyle = m_Layer.TopAxisStyle;
					break;
			}

			try
			{

				// read axis title
				string title = this.m_Format_edTitle.Text;
				if(title.Length==0)
					title=null;

				// read axis color		
				string str = (string)this.m_Format_cbColor.SelectedItem;
				if(str!="Custom")
					axstyle.Color = Color.FromName(str);

				// read axis thickness
				axstyle.Thickness = System.Convert.ToSingle(this.m_Format_cbThickness.Text);

				// read major thick length
				axstyle.MajorTickLength = System.Convert.ToSingle(this.m_Format_cbMajorTickLength.Text);

				// read major ticks
				int selidx = 	this.m_Format_cbMajorTicks.SelectedIndex;
				axstyle.InnerMajorTicks = 0!=(1&selidx);
				axstyle.OuterMajorTicks = 0!=(2&selidx);
 
				// read minor ticks
				selidx = 	this.m_Format_cbMinorTicks.SelectedIndex;
				axstyle.InnerMinorTicks = 0!=(1&selidx);
				axstyle.OuterMinorTicks = 0!=(2&selidx);

			
				// read axis position
				double posval;
				switch(this.m_Format_cbAxisPosition.SelectedIndex)
				{
					case 0:
						axstyle.Position = new Calc.RelativeOrAbsoluteValue(0,false);
						break;
					case 1:
						posval = System.Convert.ToDouble(this.m_Format_edAxisPositionValue.Text);
						axstyle.Position = new Calc.RelativeOrAbsoluteValue(posval/100,true);
						break;
					case 2:
						posval = System.Convert.ToDouble(this.m_Format_edAxisPositionValue.Text);
						axstyle.Position = new Calc.RelativeOrAbsoluteValue(posval,false);
						break;
				}

				// read axis enabled
				bool bAxisEnabled = this.m_Format_chkShowAxis.Checked;
				// set axis enabled on the layer
				switch(m_CurrentEdge)
				{
					case EdgeType.Left:
						m_Layer.LeftAxisEnabled = bAxisEnabled;
						m_Layer.LeftAxisTitleString = title;
						break;
					case EdgeType.Right:
						m_Layer.RightAxisEnabled = bAxisEnabled;
						m_Layer.RightAxisTitleString = title;
						break;
					case EdgeType.Bottom:
						m_Layer.BottomAxisEnabled = bAxisEnabled;
						m_Layer.BottomAxisTitleString = title;
						break;
					case EdgeType.Top:
						m_Layer.TopAxisEnabled = bAxisEnabled;
						m_Layer.TopAxisTitleString = title;
						break;
				}

			}
			catch(Exception)
			{
				return 1; // failed
			}


			return 0; // all ok
		}

		private void OnFormat_cbMajorTickLength_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();	
		}
		private void OnFormat_cbThickness_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();	
		}
		private void OnFormat_cbColor_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}

		private void OnFormat_cbMajorTicks_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}

		private void OnFormat_cbMinorTicks_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}

		private void OnFormat_cbAxisPosition_TextChanged(object sender, System.EventArgs e)
		{
			// enable/disable 
			switch(this.m_Format_cbAxisPosition.SelectedIndex)
			{
				case 0:
					this.m_Format_edAxisPositionValue.Enabled = false;
					break;
				case 1:
					this.m_Format_edAxisPositionValue.Enabled = true;
					break;
				case 2:
					this.m_Format_edAxisPositionValue.Enabled = true;
					break;
			}


			m_PProp[pgFormat].SetDirty();
		}

		private void OnFormat_edAxisPositionValue_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}

		private void OnFormat_edTitle_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}

		private void OnFormat_chkShowAxis_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}
		private void OnFormat_chkShowAxis_CheckedChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgFormat].SetDirty();
		}


		#endregion

		#region Tab_Contents_Methods

		
		private class PLCon
		{
			public string table, column; // holds either name of table and column if freshly added
			public PlotItem plotassociation;   // or the plot association itself in case of existing PlotAssociations
			
			// m_Group holds (in PLCon items) information about the group members
			public System.Collections.ArrayList m_Group; 
			//  m_OriginalGroup is set to the original plot group in case it exists before
			public PlotGroup m_OriginalGroup=null;

			public PLCon(string table, string column)
			{
				this.table=table;
				this.column=column;
				this.plotassociation=null;
				this.m_Group=null;
			}
			public PLCon(PlotItem pa)
			{
				this.table=null;
				this.column=null;
				this.plotassociation=pa;
				this.m_Group=null;
			}

			public PLCon(PLCon[] array)
			{
				this.table=null;
				this.column=null;
				this.plotassociation=null;
				this.m_OriginalGroup = null;
				this.m_Group = new System.Collections.ArrayList();
				this.m_Group.AddRange(array);
			}

			public PLCon(PlotGroup grp)
			{
				this.table=null;
				this.column=null;
				this.plotassociation=null;

				this.m_OriginalGroup = grp;
				this.m_Group = new System.Collections.ArrayList(grp.Count);
				for(int i=0;i<grp.Count;i++)
					m_Group.Add(grp[i]);
			}


			public bool IsValid
			{
				get { return null!=plotassociation || (null!=table && null!=column); }
			}

			public bool IsSingleNewItem
			{
				get { return null==plotassociation && null==m_Group; }
			}
			public bool IsSingleKnownItem
			{
				get { return null!=plotassociation && null==m_Group; }
			}
			public bool IsGroup
			{
				get { return null!=m_Group && m_Group.Count>0; }
			}
			public bool IsUnchangedOldGroup
			{
				get 
				{
					if(!IsGroup)
						return false;
				
					if(null==m_OriginalGroup)
						return false; // a original group must exist

					// and the counts of the original group and the m_Group Collection have to match
					if(m_OriginalGroup.Count!=m_Group.Count)
						return false;

					// and all items in that original group have to match the items in
					// the m_Group Collection
					for(int i=0;i<m_OriginalGroup.Count;i++)
					{
						if(!object.ReferenceEquals(m_OriginalGroup[i],((PLCon)m_Group[i]).plotassociation))
							return false;
					}
					
					return true; // if all conditions fullfilled, it is unchanged
				}
			}

			public bool IsChangedOldGroup
			{
				get
				{
					return IsGroup && (null!=m_OriginalGroup) && (!IsUnchangedOldGroup);
				}
			}
			public bool IsNewGroup
			{
				get { return IsGroup && (null==m_OriginalGroup); }
			}

			public override string ToString()
			{
				if(null!=plotassociation)
				{
					return plotassociation.GetName(0);
				}
				else
					return "<no more available>";
					
			}
		} // end class PLCon



		
		
		private void InitTabContents()
		{

			this.m_Content_tvDataAvail.BeginUpdate();

			// Clear the TreeView each time the method is called.
			this.m_Content_tvDataAvail.Nodes.Clear();

			// first stage - add all available tables to the nodes collection
			
			
			foreach(Data.DataTable dt in App.Current.Doc.DataSet)
			{
				this.m_Content_tvDataAvail.Nodes.Add(new TreeNode(dt.TableName,new TreeNode[1]{new TreeNode()}));
			}

			this.m_Content_tvDataAvail.EndUpdate();


			// now fill the list box with all plot associations currently inside
			this.m_Contents_lbContents.BeginUpdate();
			this.m_Contents_lbContents.Items.Clear();
			System.Collections.Hashtable addedItems = new System.Collections.Hashtable();
			for(int i=0;i<m_Layer.PlotItems.Count;i++)
			{
				PlotItem pa = m_Layer.PlotItems[i];
				
				if(!addedItems.ContainsKey(pa)) // if not already added to the list box
				{
					PlotGroup grp = m_Layer.PlotGroups.GetPlotGroupOf(pa);
				
					if(null!=grp)
					{
						// add only one item to the list box, namely a PLCon group item with
						// all the members of that group
						PLCon plitem = new PLCon(grp); 
						this.m_Contents_lbContents.Items.Add(plitem);
						// add all the items in the group also to the list of added items 
						for(int j=0;j<grp.Count;j++)
						{
							addedItems.Add(grp[j],null);
						}
					}
					else // item is not in a plot group
					{
						this.m_Contents_lbContents.Items.Add(new PLCon(pa));
						addedItems.Add(pa,null);
					}
				}				
			}
			this.m_Contents_lbContents.EndUpdate();

			m_PProp[pgContent].SetInitialized();
		}


		private PlotItem NewPlotAssociationFromPLCon(PLCon item)
		{
			if(!item.IsSingleNewItem)
				return null;

			// create a new plotassociation from the column
			// first, get the y column from table and name
			Data.DataTable tab = App.Current.Doc.DataSet[item.table];
			if(null!=tab)
			{
				Data.DataColumn ycol = tab[item.column];
				if(null!=ycol)
				{
					Data.DataColumn xcol = tab.FindXColumnOfGroup(ycol.Group);
					if(null==xcol)
						xcol=ycol;
					return  new Graph.XYDataPlot(new PlotAssociation(xcol,ycol),new LineScatterPlotStyle());
					// now enter the plotassociation back into the layer's plot association list
				}
			}
			return null;
		}

		protected int ApplyTabContents()
		{


			m_Layer.PlotItems.Clear();
			m_Layer.PlotGroups.Clear();

			// now we must get all items out of the listbox and look
			// for which items are new or changed
			for(int i=0;i<this.m_Contents_lbContents.Items.Count;i++)
			{
				PLCon item = (PLCon)this.m_Contents_lbContents.Items[i];
				PlotItem pa=null;
				
				if(item.IsSingleNewItem)
				{
					pa = this.NewPlotAssociationFromPLCon(item);
					if(null!=pa)
					{
						m_Layer.PlotItems.Add(pa);
					}
				}
				else if(item.IsSingleKnownItem)
				{
					pa = item.plotassociation;
					m_Layer.PlotItems.Add(pa);
				}
				else if(item.IsUnchangedOldGroup)
				{
					// if the group was not changed, add all group members to the
					// plotassociation collection and add the group to the group list
					for(int j=0;j<item.m_Group.Count;j++)
					{
						PLCon member = (PLCon)item.m_Group[j];
						m_Layer.PlotItems.Add(member.plotassociation);
					} // end for
					m_Layer.PlotGroups.Add(item.m_OriginalGroup); // add the unchanged group back to the layer
				} // if item.IsUnchangedOldGroup
				else if(item.IsChangedOldGroup) // group exists before, but was changed
				{
					item.m_OriginalGroup.Clear();
					for(int j=0;j<item.m_Group.Count;j++)
					{
						PLCon member = (PLCon)item.m_Group[j];
						if(member.IsSingleKnownItem)
						{
							m_Layer.PlotItems.Add(member.plotassociation);
							item.m_OriginalGroup.Add(member.plotassociation);
						}
						else // than it is a single new item
						{
							pa = this.NewPlotAssociationFromPLCon(member);
							if(null!=pa)
							{
								m_Layer.PlotItems.Add(member.plotassociation);
								item.m_OriginalGroup.Add(pa);
							}
						}
					} // end for
					m_Layer.PlotGroups.Add(item.m_OriginalGroup); // add the plot group back to the layer
				} // else if item.IsChangedOldGroup
				else if(item.IsNewGroup) // if it is a new group
				{
					// 1st) create a new PlotGroup
					PlotGroup newplotgrp = new PlotGroup(PlotGroupStyle.All);
					// if the group was not changed, add all group members to the
					// plotassociation collection and add the group to the group list
					for(int j=0;j<item.m_Group.Count;j++)
					{
						PLCon member = (PLCon)item.m_Group[j];
						if(member.IsSingleKnownItem)
						{
							m_Layer.PlotItems.Add(member.plotassociation);
							newplotgrp.Add(member.plotassociation);
						}
						else // than it is a single new item
						{
							pa = this.NewPlotAssociationFromPLCon(member);
							if(null!=pa)
							{
								m_Layer.PlotItems.Add(pa);
								newplotgrp.Add(pa);
							}
						}
					} // for all items in that new group
					m_Layer.PlotGroups.Add(newplotgrp); // add the new plot group to the layer
				} // if it was a new group
			} // end for all items in the list box
			
			return 0; // all ok
		}

		
		private void OnContents_tvDataAvailBeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			Data.DataTable dt = App.Current.Doc.DataSet[e.Node.Text];
			if(null!=dt)
			{
				this.m_Content_tvDataAvail.BeginUpdate();
				e.Node.Nodes.Clear();
				for(int i=0;i<dt.ColumnCount;i++)
				{
					e.Node.Nodes.Add(new TreeNode(dt[i].ColumnName));
				}
				this.m_Content_tvDataAvail.EndUpdate();
			}
		}



		private void OnContents_btPutData_Click(object sender, System.EventArgs e)
		{

			// first, put the selected node into the list, even if it is not checked
			TreeNode sn=this.m_Content_tvDataAvail.SelectedNode;
			if(null!=sn && null!=sn.Parent)
			{
				this.m_Contents_lbContents.Items.Add(new PLCon(sn.Parent.Text,sn.Text));
				sn.Checked=false;
				m_PProp[pgContent].SetDirty();
			}

			// put all checked nodes into the list box, but be carefull not
			// to put the table nodes into the list box
			foreach(TreeNode ptn in m_Content_tvDataAvail.Nodes) // for all table nodes
			{
				foreach(TreeNode tn in ptn.Nodes) // for all column nodes
				{
					if(tn.Checked && tn.Parent!=null)
					{
						this.m_Contents_lbContents.Items.Add(new PLCon(tn.Parent.Text,tn.Text));
						tn.Checked=false;
						m_PProp[pgContent].SetDirty();

					}
				}
			}
		}

		private void OnContents_PullData_Click(object sender, System.EventArgs e)
		{
			// for each selected item in the list, 
			// remove it from the list
			int cnt = this.m_Contents_lbContents.SelectedIndices.Count;
			if(0!=cnt)
			{
				int[] selidxs = new int[cnt];
				this.m_Contents_lbContents.SelectedIndices.CopyTo(selidxs,0);
				for(int i=selidxs.Length-1;i>=0;i--)
					this.m_Contents_lbContents.Items.RemoveAt(selidxs[i]);

				m_PProp[pgContent].SetDirty();
			}
		}

		
		private void OnContents_lbContents_DoubleClick(object sender, System.EventArgs e)
		{
			if(this.m_Contents_lbContents.SelectedItems.Count==1)
			{
				PlotItem pa = ((PLCon)this.m_Contents_lbContents.SelectedItems[0]).plotassociation;
				if(null!=pa)
				{
					PlotGroup plotGroup = m_Layer.PlotGroups.GetPlotGroupOf(pa);
					// open the plot style dialog of the selected item
					PlotStyleDialog dlg = new PlotStyleDialog(null==plotGroup ? (PlotStyle)pa.Style : (PlotStyle)plotGroup.MasterItem.Style,plotGroup);
					DialogResult dr = dlg.ShowDialog(this);
					if(dr==DialogResult.OK)
					{
						if(null!=plotGroup)
						{
							plotGroup.Style = dlg.PlotGroupStyle;
							if(plotGroup.IsIndependent)
								pa.Style = dlg.PlotStyle;
							else
								plotGroup.MasterItem.Style = dlg.PlotStyle;
						}
						else // pa was not member of a plot group
						{
							pa.Style = dlg.PlotStyle;
						}
						this.Invalidate(); // renew the picture
					}
				}
			}
		}


		private void OnContent_btListSelUp_Click(object sender, System.EventArgs e)
		{
			// move the selected items upwards in the list
			ContentsListBox_MoveUpDown(-1);
			m_PProp[pgContent].SetDirty();
		}

		private void OnContents_btListSelDown_Click(object sender, System.EventArgs e)
		{
			// move the selected items downwards in the list
			ContentsListBox_MoveUpDown(1);
			m_PProp[pgContent].SetDirty();
		}

		private void ContentsListBox_MoveUpDown(int iDelta)
		{
			int i;

			if(iDelta!=1 && iDelta!=-1)
				return;

			// retrieve the selected items
			int cnt = this.m_Contents_lbContents.SelectedIndices.Count;
			if(0==cnt)
				return; // we cannot move anything if nothing is selected
			int[] selidxs = new int[cnt];
			this.m_Contents_lbContents.SelectedIndices.CopyTo(selidxs,0);

			if(iDelta==-1 ) // move one position upwards
			{
				if(selidxs[0]==0) // if the first item is selected, we can't move upwards
				{
					return;
				}

				this.m_Contents_lbContents.BeginUpdate();
				for(i=0;i<cnt;i++)
				{
					object helpSeg;
					int iSeg=selidxs[i];

					helpSeg = this.m_Contents_lbContents.Items[iSeg-1];
					this.m_Contents_lbContents.Items[iSeg-1] = this.m_Contents_lbContents.Items[iSeg];
					this.m_Contents_lbContents.Items[iSeg] = helpSeg;

					this.m_Contents_lbContents.SetSelected(iSeg-1,true); // select upper item,
					this.m_Contents_lbContents.SetSelected(iSeg,false); // deselect lower item
				}
				this.m_Contents_lbContents.EndUpdate();
			} // end if iDelta==-1
			else if(iDelta==1) // move one position down
			{
				if(selidxs[cnt-1]==this.m_Contents_lbContents.Items.Count-1)		// if last item is selected, we can't move downwards
				{
					return;
				}

				this.m_Contents_lbContents.BeginUpdate();
				for(i=cnt-1;i>=0;i--)
				{
					object helpSeg;
					int iSeg=selidxs[i];

					helpSeg = this.m_Contents_lbContents.Items[iSeg+1];
					this.m_Contents_lbContents.Items[iSeg+1]=this.m_Contents_lbContents.Items[iSeg];
					this.m_Contents_lbContents.Items[iSeg]=helpSeg;

					this.m_Contents_lbContents.SetSelected(iSeg+1,true);
					this.m_Contents_lbContents.SetSelected(iSeg,false);
				}
				this.m_Contents_lbContents.EndUpdate();
			} // end if iDelta==1
		}

		private void OnContents_lbContents_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// make sure that all items of a single group are
			// either selected or deselected
			// so selection state in one group must be the same for all items

		}

		private void OnContents_lbContents_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			PLCon item = (PLCon)this.m_Contents_lbContents.Items[e.Index];
			if(item.IsGroup)
				e.ItemHeight *= item.m_Group.Count;

			System.Console.WriteLine("MeasureItem {0}, {1}",e.Index,e.ItemHeight);	
		}

		private void OnContents_lbContents_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			int height = e.Bounds.Height;
			e.DrawBackground();
			PLCon item = (PLCon)this.m_Contents_lbContents.Items[e.Index];
			using(Brush brush = new SolidBrush(e.ForeColor))
			{
				if(item.IsGroup) // item is a group
				{
					height /= item.m_Group.Count;
					for(int i=0;i<item.m_Group.Count;i++)
					{
						string str = string.Format("g{0} {1}",e.Index,item.m_Group[i].ToString());
						e.Graphics.DrawString(str,
							e.Font,brush,0,e.Bounds.Top+i*height);
					}
				}
				else // item is not a group
				{
					e.Graphics.DrawString(item.ToString(),
						e.Font,brush,0,e.Bounds.Top);
				}
			}

		}

		private void OnContents_btPlotAssociations_Click(object sender, System.EventArgs e)
		{
		
		}

		private void OnContents_btGroup_Click(object sender, System.EventArgs e)
		{
			// retrieve the selected items
			int cnt = this.m_Contents_lbContents.SelectedIndices.Count;
			if(cnt<2)
				return; // we cannot group anything if no or only one item is selected
		
			
			this.m_Contents_lbContents.BeginUpdate();
			
			int[] selidxs = new int[cnt];
			this.m_Contents_lbContents.SelectedIndices.CopyTo(selidxs,0);

			// look, if one of the selected items is a plot group
			// if found, use this group and add the remaining items to this
			PLCon foundgroup=null;
			int   foundindex=-1;
			int i;
			for(i=0;i<selidxs.Length;i++)
			{
				foundgroup = (PLCon)m_Contents_lbContents.Items[selidxs[i]];
				if(foundgroup.IsGroup)
				{
					foundindex = selidxs[i];
					break;
				}
				foundgroup=null; // set to null to indicate not a group item
			}


			// if a group was found use this to add the remaining items
			// else use a new PLCon to add the items to
			PLCon addgroup = null!=foundgroup ? foundgroup : new PLCon(new PLCon[0]); 
			// now add the remaining selected items to the found group
			for(i=0;i<selidxs.Length;i++)
			{
				if(selidxs[i]==foundindex) continue; // don't add the found group to itself
				PLCon item = (PLCon)m_Contents_lbContents.Items[selidxs[i]];
				if(item.IsGroup) // if it is a group, add the members of the group to avoid more than one recursion
				{
					for(int j=0;j<item.m_Group.Count;j++)
						addgroup.m_Group.Add(item.m_Group[i]);
				}
				else // item to add is not a group
				{
					addgroup.m_Group.Add(item);
				}
			} // end for

			// now all items are in the new group

			// so update the list box:
			// delete all items except of the found group

			if(null!=foundgroup)
			{
				for(i=selidxs.Length-1;i>=0;i--) // step from end of list because items shift away if removing some items
				{
					if(selidxs[i]==foundindex)
					{
						this.m_Contents_lbContents.Items[selidxs[i]]=addgroup; // this is only a trick to force measuring the item again
						continue; // don't add the found group to itself
					}
					this.m_Contents_lbContents.Items.RemoveAt(selidxs[i]);
				}
			}
			else // if no previous group was found, replace first selected item by the group
			{
				this.m_Contents_lbContents.Items[selidxs[0]]= addgroup;
				// remove the remaining items
				for(i=selidxs.Length-1;i>=1;i--)
				{
					this.m_Contents_lbContents.Items.RemoveAt(selidxs[i]);
				}
			}
			this.m_Contents_lbContents.EndUpdate();
			m_PProp[pgContent].SetDirty();
		}

		private void OnContents_btUngroup_Click(object sender, System.EventArgs e)
		{
			// retrieve the selected items
			int cnt = this.m_Contents_lbContents.SelectedIndices.Count;
			if(cnt<1)
				return; // we cannot ungroup anything if nothing selected

			int[] selidxs = new int[cnt];
			this.m_Contents_lbContents.SelectedIndices.CopyTo(selidxs,0);
			
			this.m_Contents_lbContents.BeginUpdate();


			for(int i=cnt-1;i>=0;i--)
			{
				PLCon item = (PLCon)this.m_Contents_lbContents.Items[selidxs[i]];
			
				if(item.IsGroup)
				{
					// insert all items contained in that group in the next position
					for(int j=item.m_Group.Count-1;j>=1;j--)
					{
						this.m_Contents_lbContents.Items.Insert(selidxs[i]+1,item.m_Group[j]);
					}
					// and replace the group item by the first item of that group
					this.m_Contents_lbContents.Items[selidxs[i]] = item.m_Group[0];
				}
			} // end for
			this.m_Contents_lbContents.EndUpdate();
			m_PProp[pgContent].SetDirty();
		}

		private void OnContents_btEditRange_Click(object sender, System.EventArgs e)
		{
		}

		private void OnContents_chkShowRange_CheckedChange(object sender, System.EventArgs e)
		{
		}


		#endregion

		#region Tab_Layer_Methods
		private void InitTabLayer()
		{

			this.m_Layer_edHeight.Text = m_Layer.UserHeight.ToString();
			this.m_Layer_edWidth.Text  = m_Layer.UserWidth.ToString();

			this.m_Layer_edLeftPosition.Text = m_Layer.UserXPosition.ToString();
			this.m_Layer_edTopPosition.Text = m_Layer.UserYPosition.ToString();

			this.m_Layer_edRotation.Text = m_Layer.Rotation.ToString();
			this.m_Layer_edScale.Text = m_Layer.Scale.ToString();


			// Fill the comboboxes of the x and y position with possible values
			this.m_Layer_cbLeftType.Items.Clear();
			this.m_Layer_cbTopType.Items.Clear();
			foreach(string name in Enum.GetNames(typeof(Layer.PositionType)))
			{
				this.m_Layer_cbLeftType.Items.Add(name);
				this.m_Layer_cbTopType.Items.Add(name);
			}
			this.m_Layer_cbLeftType.SelectedItem =Enum.GetName(typeof(Layer.PositionType),m_Layer.UserXPositionType);
			this.m_Layer_cbTopType.SelectedItem =Enum.GetName(typeof(Layer.PositionType),m_Layer.UserYPositionType);


			// Fill the comboboxes of the width  and height with possible values
			this.m_Layer_cbWidthType.Items.Clear();
			this.m_Layer_cbHeightType.Items.Clear();
			foreach(string name in Enum.GetNames(typeof(Layer.SizeType)))
			{
				this.m_Layer_cbWidthType.Items.Add(name);
				this.m_Layer_cbHeightType.Items.Add(name);
			}
			this.m_Layer_cbWidthType.SelectedItem =Enum.GetName(typeof(Layer.SizeType),m_Layer.UserWidthType);
			this.m_Layer_cbHeightType.SelectedItem =Enum.GetName(typeof(Layer.SizeType),m_Layer.UserHeightType);


			// Fill the combobox of linked layer with possible values
			this.m_Layer_cbLinkedLayer.Items.Clear();
			this.m_Layer_cbLinkedLayer.Items.Add("None");
			if(null!=m_Layer.ParentLayerList)
			{
				for(int i=0;i<m_Layer.ParentLayerList.Count;i++)
				{
					if(!m_Layer.IsLayerDependentOnMe(m_Layer.ParentLayerList[i]))
						this.m_Layer_cbLinkedLayer.Items.Add("Layer " + i.ToString());
				}
			}

			// now if we have a linked layer, set the selected item to the right value
			if(null==m_Layer.LinkedLayer)
				this.m_Layer_cbLinkedLayer.SelectedItem = "None";
			else
				this.m_Layer_cbLinkedLayer.SelectedItem = "Layer " + m_Layer.LinkedLayer.Number;


			// initialize the axis link properties
			if(m_Layer.XAxisLinkType==Layer.AxisLinkType.None)
				this.m_Layer_rbLinkXAxisNone.Checked=true;
			else if(m_Layer.XAxisLinkType==Layer.AxisLinkType.Straight)
				this.m_Layer_rbLinkXAxisStraight.Checked=true;
			else
				this.m_Layer_rbLinkXAxisCustom.Checked=true;


			if(m_Layer.YAxisLinkType==Layer.AxisLinkType.None)
				this.m_Layer_rbLinkYAxisNone.Checked=true;
			else if(m_Layer.YAxisLinkType==Layer.AxisLinkType.Straight)
				this.m_Layer_rbLinkYAxisStraight.Checked=true;
			else
				this.m_Layer_rbLinkYAxisCustom.Checked=true;

			this.m_Layer_edLinkXAxisOrgA.Text = m_Layer.LinkXAxisOrgA.ToString();
			this.m_Layer_edLinkXAxisEndA.Text = m_Layer.LinkXAxisEndA.ToString();
			this.m_Layer_edLinkXAxisOrgB.Text = m_Layer.LinkXAxisOrgB.ToString();
			this.m_Layer_edLinkXAxisEndB.Text = m_Layer.LinkXAxisEndB.ToString();


			bool bEnab= m_Layer.IsLinked==true && m_Layer.XAxisLinkType!=Layer.AxisLinkType.None;
			this.m_Layer_edLinkXAxisOrgA.Enabled=bEnab;
			this.m_Layer_edLinkXAxisEndA.Enabled=bEnab;
			this.m_Layer_edLinkXAxisOrgB.Enabled=bEnab;
			this.m_Layer_edLinkXAxisEndB.Enabled=bEnab;


			this.m_Layer_edLinkYAxisOrgA.Text = m_Layer.LinkYAxisOrgA.ToString();
			this.m_Layer_edLinkYAxisEndA.Text = m_Layer.LinkYAxisEndA.ToString();
			this.m_Layer_edLinkYAxisOrgB.Text = m_Layer.LinkYAxisOrgB.ToString();
			this.m_Layer_edLinkYAxisEndB.Text = m_Layer.LinkYAxisEndB.ToString();

			bEnab= m_Layer.IsLinked==true && m_Layer.YAxisLinkType!=Layer.AxisLinkType.None;
			this.m_Layer_edLinkYAxisOrgA.Enabled=bEnab;
			this.m_Layer_edLinkYAxisEndA.Enabled=bEnab;
			this.m_Layer_edLinkYAxisOrgB.Enabled=bEnab;
			this.m_Layer_edLinkYAxisEndB.Enabled=bEnab;


			// indicate this tab is now initialized
			m_PProp[pgLayer].SetInitialized();
		}

		protected int ApplyTabLayer()
		{
			try
			{

				int linkedlayernumber=-1;

				string selitem = (string)m_Layer_cbLinkedLayer.SelectedItem;
				if(selitem.StartsWith("Layer "))
					linkedlayernumber= System.Convert.ToInt32(selitem.Substring(6));


				m_Layer.LinkedLayer = linkedlayernumber<0 ? null : m_Layer.ParentLayerList[linkedlayernumber];

			
				// now update the layer
				float angle  = System.Convert.ToSingle(this.m_Layer_edRotation.Text);
				float scale  = System.Convert.ToSingle(this.m_Layer_edScale.Text);
				m_Layer.Rotation = angle;
				m_Layer.Scale    = scale;


				double width  = System.Convert.ToSingle(this.m_Layer_edWidth.Text);
				Layer.SizeType widthtype = (Layer.SizeType)Enum.Parse(typeof(Layer.SizeType),this.m_Layer_cbWidthType.Text);
				double height = System.Convert.ToSingle(this.m_Layer_edHeight.Text);
				Layer.SizeType heighttype = (Layer.SizeType)Enum.Parse(typeof(Layer.SizeType),this.m_Layer_cbHeightType.Text);
				m_Layer.SetSize(width,widthtype,height,heighttype);


				double leftpos = System.Convert.ToSingle(this.m_Layer_edLeftPosition.Text);
				Layer.PositionType leftpostype = (Layer.PositionType)Enum.Parse(typeof(Layer.PositionType),this.m_Layer_cbLeftType.Text);
				double toppos  = System.Convert.ToSingle(this.m_Layer_edTopPosition.Text);
				Layer.PositionType toppostype = (Layer.PositionType)Enum.Parse(typeof(Layer.PositionType),this.m_Layer_cbTopType.Text);
				m_Layer.SetPosition(leftpos,leftpostype,toppos,toppostype);

			
				// update the x axis link properties
				if(m_Layer.LinkedLayer!=null && false==this.m_Layer_rbLinkXAxisNone.Checked)
				{
					double orgA = System.Convert.ToDouble(this.m_Layer_edLinkXAxisOrgA.Text);
					double orgB = System.Convert.ToDouble(this.m_Layer_edLinkXAxisOrgB.Text);
					double endA = System.Convert.ToDouble(this.m_Layer_edLinkXAxisEndA.Text);
					double endB = System.Convert.ToDouble(this.m_Layer_edLinkXAxisEndB.Text);
					m_Layer.SetXAxisLinkParameter(orgA,orgB,endA,endB);
				}
				// update the y axis link properties
				if(m_Layer.LinkedLayer!=null && false==this.m_Layer_rbLinkYAxisNone.Checked)
				{
					double orgA = System.Convert.ToDouble(this.m_Layer_edLinkYAxisOrgA.Text);
					double orgB = System.Convert.ToDouble(this.m_Layer_edLinkYAxisOrgB.Text);
					double endA = System.Convert.ToDouble(this.m_Layer_edLinkYAxisEndA.Text);
					double endB = System.Convert.ToDouble(this.m_Layer_edLinkYAxisEndB.Text);
					m_Layer.SetYAxisLinkParameter(orgA,orgB,endA,endB);
				}
			}
			catch(Exception)
			{
				return 1; // indicate that something failed
			}

			return 0; // all ok
		}
		private void OnLayer_edLeftPosition_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}

		private void OnLayer_edTopPosition_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();		
		}

		private void OnLayer_edWidth_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}

		private void OnLayer_edHeight_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}

		private void OnLayer_edRotation_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}

		private void OnLayer_edScale_TextChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}

		private void OnLayer_cbLinkedLayer_SelectedValueChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}

		private void OnLayer_LinkXAxisCheckedChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();

			bool bEnab = m_Layer_rbLinkXAxisCustom.Checked;
			this.m_Layer_edLinkXAxisOrgA.Enabled=bEnab;
			this.m_Layer_edLinkXAxisEndA.Enabled=bEnab;
			this.m_Layer_edLinkXAxisOrgB.Enabled=bEnab;
			this.m_Layer_edLinkXAxisEndB.Enabled=bEnab;
			
			if(m_Layer_rbLinkXAxisStraight.Checked)
			{
				this.m_Layer_edLinkXAxisOrgA.Text = "0";
				this.m_Layer_edLinkXAxisEndA.Text = "0";
				this.m_Layer_edLinkXAxisOrgB.Text = "1";
				this.m_Layer_edLinkXAxisEndB.Text = "1";
			}
		}

		private void OnLayer_LinkYAxisCheckedChanged(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();

			bool bEnab = m_Layer_rbLinkYAxisCustom.Checked;
			this.m_Layer_edLinkYAxisOrgA.Enabled=bEnab;
			this.m_Layer_edLinkYAxisEndA.Enabled=bEnab;
			this.m_Layer_edLinkYAxisOrgB.Enabled=bEnab;
			this.m_Layer_edLinkYAxisEndB.Enabled=bEnab;

			if(m_Layer_rbLinkYAxisStraight.Checked)
			{
				this.m_Layer_edLinkYAxisOrgA.Text = "0";
				this.m_Layer_edLinkYAxisEndA.Text = "0";
				this.m_Layer_edLinkYAxisOrgB.Text = "1";
				this.m_Layer_edLinkYAxisEndB.Text = "1";
			}
		}

		private void OnLayer_cbLeftType_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
			m_PProp[pgLayer].SetDirty();
		}


		#endregion // Tab_Layer_Methods
	}
}
