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
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Graph;
namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for PlotStyleDialog.
	/// </summary>
	public class PlotStyleDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label m_lblDataSets;
		private System.Windows.Forms.TextBox m_txtDatasets;
		private System.Windows.Forms.ComboBox m_cbPlotType;
		private System.Windows.Forms.Label m_lblPlotType;
		private System.Windows.Forms.Label m_lblLineSymbolColor;
		private System.Windows.Forms.Button m_btLineSymbolColorDetails;
		private System.Windows.Forms.GroupBox m_gbLine;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox m_cbLineConnect;
		private System.Windows.Forms.ComboBox m_cbLineType;
		private System.Windows.Forms.ComboBox m_cbLineWidth;
		private System.Windows.Forms.CheckBox m_chkLineFillArea;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox m_cbLineFillDirection;
		private System.Windows.Forms.ComboBox m_cbLineFillColor;
		private System.Windows.Forms.Button m_btLineFillColorDetails;
		private System.Windows.Forms.CheckBox m_chkLineSymbolGap;
		private System.Windows.Forms.GroupBox m_gbSymbol;
		private System.Windows.Forms.Label m_lblSymbolShape;
		private System.Windows.Forms.Label m_lblSymbolStyle;
		private System.Windows.Forms.Label m_lblSymbolSize;
		private System.Windows.Forms.ComboBox m_cbSymbolShape;
		private System.Windows.Forms.ComboBox m_cbSymbolStyle;
		private System.Windows.Forms.ComboBox m_cbSymbolSize;
		private System.Windows.Forms.GroupBox m_gbSymbolDropLine;
		private System.Windows.Forms.CheckBox m_chkSymbolDropLineLeft;
		private System.Windows.Forms.CheckBox m_chkSymbolDropLineRight;
		private System.Windows.Forms.CheckBox m_chkSymbolDropLineTop;
		private System.Windows.Forms.CheckBox m_chkSymbolDropLineBottom;
		private System.Windows.Forms.CheckBox m_chkSymbolSkipPoints;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox m_edSymbolSkipFrequency;
		private System.Windows.Forms.Button m_btOK;
		private System.Windows.Forms.Button m_btCancel;
		private System.Windows.Forms.Button m_btWorksheet;
		private System.Windows.Forms.Button m_btRemove;


		protected PlotStyle m_PlotStyle;
		protected PlotGroup m_PlotGroup;
		protected PlotGroupStyle m_PlotGroupStyle;

		protected bool m_bEnableEvents=false;
		private System.Windows.Forms.ComboBox m_cbLineSymbolColor;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton m_rbtPlotGroupIndependent;
		private System.Windows.Forms.RadioButton m_rbtPlotGroupIncremental;
		private System.Windows.Forms.CheckBox m_chkPlotGroupColor;
		private System.Windows.Forms.CheckBox m_chkPlotGroupLineType;
		private System.Windows.Forms.CheckBox m_chkPlotGroupSymbol;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PlotStyleDialog(PlotStyle ps, PlotGroup plotGroup)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_bEnableEvents = false; // suspend the events


			// if this plotstyle belongs to a plot group of this layer,
			// use the master plot style instead of the plotstyle itself
			m_PlotGroup=plotGroup;
			if(null!=m_PlotGroup)
			{
				m_PlotStyle = (PlotStyle)((PlotStyle)m_PlotGroup.MasterItem.Style).Clone();
				m_PlotGroupStyle = m_PlotGroup.Style;
			}
			else // not member of a plotgroup
			{
				m_PlotStyle = (PlotStyle)ps.Clone();
				m_PlotGroupStyle = 0;
			}


			FillDialogElements(); // fill and set boxes without events
			m_bEnableEvents=true; // just now enable the events
		}

		public PlotStyle PlotStyle
		{
			get
			{
				// return only a clone of the plotstyle, since otherwise 
				// changes to the plotstyle can be made which are not
				// reflected by the dialog elements
				return null==m_PlotStyle?null:(PlotStyle)m_PlotStyle.Clone();
			}
		}

		public Graph.PlotGroupStyle PlotGroupStyle
		{
			get { return m_PlotGroupStyle; }
		}


		void FillDialogElements()
		{

			FillPlotTypeComboBox(this.m_cbPlotType);

			FillColorComboBox(this.m_cbLineSymbolColor);
			FillLineConnectComboBox(this.m_cbLineConnect);
			FillLineStyleComboBox(this.m_cbLineType);
			FillLineWidthComboBox(this.m_cbLineWidth);
			FillColorComboBox(this.m_cbLineFillColor);
			FillFillDirectionComboBox(this.m_cbLineFillDirection);

			FillSymbolShapeComboBox(this.m_cbSymbolShape);
			FillSymbolStyleComboBox(this.m_cbSymbolStyle);
			FillSymbolSizeComboBox(this.m_cbSymbolSize);


			// now we have to set all dialog elements to the right values
			SetPlotTypeComboBox(this.m_cbPlotType,this.m_PlotStyle);
			SetPlotStyleColorComboBox(this.m_cbLineSymbolColor, this.m_PlotStyle);
			SetLineSymbolGapCheckBox(this.m_chkLineSymbolGap, this.m_PlotStyle);

			SetDropLineLeftCheckBox(this.m_chkSymbolDropLineLeft, this.m_PlotStyle);
			SetDropLineRightCheckBox(this.m_chkSymbolDropLineRight, this.m_PlotStyle);
			SetDropLineTopCheckBox(this.m_chkSymbolDropLineTop, this.m_PlotStyle);
			SetDropLineBottomCheckBox(this.m_chkSymbolDropLineBottom, this.m_PlotStyle);


			SetLineConnectComboBox(this.m_cbLineConnect,this.m_PlotStyle);
			SetLineStyleComboBox(this.m_cbLineType,this.m_PlotStyle);
			SetLineWidthComboBox(this.m_cbLineWidth, m_PlotStyle);
			SetFillCheckBox(this.m_chkLineFillArea, m_PlotStyle);
			SetFillDirectionComboBox(this.m_cbLineFillDirection,m_PlotStyle);
			SetFillColorComboBox(this.m_cbLineFillColor, m_PlotStyle);
			EnableDisableFillElements(this,m_PlotStyle);
		
			SetSymbolShapeComboBox(this.m_cbSymbolShape,m_PlotStyle);
			SetSymbolStyleComboBox(this.m_cbSymbolStyle,m_PlotStyle);
			SetSymbolSizeComboBox(this.m_cbSymbolSize,m_PlotStyle);

			SetPlotGroupElements(m_PlotGroup);
		}

		private void SetPlotGroupElements(PlotGroup grp)
		{
			PlotGroupStyle Style = null!=grp ? grp.Style : 0;
			if(0==Style)
			{
				this.m_rbtPlotGroupIndependent.Checked=true;

				this.m_chkPlotGroupColor.Checked = (0!=(Style&PlotGroupStyle.Color));
				this.m_chkPlotGroupLineType.Checked = (0!=(Style & PlotGroupStyle.Line));
				this.m_chkPlotGroupSymbol.Checked = (0!=(Style & PlotGroupStyle.Symbol));

				this.m_rbtPlotGroupIndependent.Enabled=false;
				this.m_rbtPlotGroupIncremental.Enabled=false;
				this.m_chkPlotGroupColor.Enabled=false;
				this.m_chkPlotGroupLineType.Enabled=false;
				this.m_chkPlotGroupSymbol.Enabled=false;
			}
			else // style is not independent, i.e. incremental
			{
				this.m_rbtPlotGroupIncremental.Checked=true;

				this.m_chkPlotGroupColor.Checked = (0!=(Style&PlotGroupStyle.Color));
				this.m_chkPlotGroupLineType.Checked = (0!=(Style & PlotGroupStyle.Line));
				this.m_chkPlotGroupSymbol.Checked = (0!=(Style & PlotGroupStyle.Symbol));

				this.m_chkPlotGroupColor.Enabled=true;
				this.m_chkPlotGroupLineType.Enabled=true;
				this.m_chkPlotGroupSymbol.Enabled=true;
			}
		}


		private void GetPlotGroupElements()
		{
			m_PlotGroupStyle = 0;
			if(this.m_rbtPlotGroupIncremental.Checked)
			{
				if(this.m_chkPlotGroupColor.Checked)    m_PlotGroupStyle |= PlotGroupStyle.Color;
				if(this.m_chkPlotGroupLineType.Checked) m_PlotGroupStyle |= PlotGroupStyle.Color;
				if(this.m_chkPlotGroupSymbol.Checked)   m_PlotGroupStyle |= PlotGroupStyle.Color;
			}
		}


		void FillColorComboBox(System.Windows.Forms.ComboBox cb)
		{
			cb.Items.Add("Custom");

			foreach(Color c in PlotStyle.PlotColors)
			{
				string name = c.ToString();
				cb.Items.Add(name.Substring(7,name.Length-8));
			}
		}


		public static void SetPlotStyleColorComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			string name = "Custom"; // default

			if(null!=ps && null!=ps.LineStyle && null!=ps.LineStyle.PenHolder)
			{
				name = "Custom";
				if(ps.LineStyle.PenHolder.PenType == PenType.SolidColor)
				{
					name = PlotStyle.GetPlotColorName(ps.LineStyle.PenHolder.Color);
					if(null==name) name = "Custom";
				}
			}
			else if(null!=ps && null!=ps.ScatterStyle && null!=ps.ScatterStyle.Pen)
			{
				name = "Custom";
				if(ps.ScatterStyle.Pen.PenType == PenType.SolidColor)
				{
					name = PlotStyle.GetPlotColorName(ps.ScatterStyle.Pen.Color);
					if(null==name) name = "Custom";
				}
			}
			
			cb.SelectedItem = name;
		}




		static public void FillLineConnectComboBox(System.Windows.Forms.ComboBox cb)
		{
			string [] names = System.Enum.GetNames(typeof(LineStyles.ConnectionStyle));
			foreach(string name in names)
			{
				cb.Items.Add(name);
			}
		}

		static public void SetLineConnectComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			LineStyles.ConnectionStyle cn = LineStyles.ConnectionStyle.NoLine; // default

			if(ps!=null && ps.LineStyle!=null)
				cn = ps.LineStyle.Connection;

			string name = cn.ToString();
			cb.Text = name;
		}

		public static void SetLineSymbolGapCheckBox(System.Windows.Forms.CheckBox chk, PlotStyle ps)
		{
			bool bGap = ps.LineSymbolGap; // default
			chk.Checked = bGap;
		}

		public static void SetDropLineLeftCheckBox(System.Windows.Forms.CheckBox chk, PlotStyle ps)
		{
			chk.Checked = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Left);
		}
		public static void SetDropLineRightCheckBox(System.Windows.Forms.CheckBox chk, PlotStyle ps)
		{
			chk.Checked = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Right);
		}
		public static void SetDropLineTopCheckBox(System.Windows.Forms.CheckBox chk, PlotStyle ps)
		{
			chk.Checked = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Top);
		}
		public static void SetDropLineBottomCheckBox(System.Windows.Forms.CheckBox chk, PlotStyle ps)
		{
			chk.Checked = 0!=(ps.ScatterStyle.DropLine&ScatterStyles.DropLine.Bottom);
		}

		static public void FillLineStyleComboBox(System.Windows.Forms.ComboBox cb)
		{
			string [] names = System.Enum.GetNames(typeof(DashStyle));
			foreach(string name in names)
			{
				cb.Items.Add(name);
			}
		}

		static public void SetLineStyleComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			DashStyle ds = DashStyle.Solid; // default
			if(ps!=null && ps.LineStyle!=null && ps.LineStyle.PenHolder!=null)
				ds = ps.LineStyle.PenHolder.DashStyle;

			string name = ds.ToString();
			cb.SelectedItem=name;
		}


		static public void FillLineWidthComboBox(System.Windows.Forms.ComboBox cb)
		{
			float[] LineWidths = 
			{	0.2f,0.5f,1,1.5f,2,3,4,5 };

			foreach(float width in LineWidths)
			{
				cb.Items.Add(width.ToString());
			}
		}

		static public void SetLineWidthComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			float linewidth = 1; // default value
			if(null!=ps && null!=ps.LineStyle && null!=ps.LineStyle.PenHolder)
				linewidth = ps.LineStyle.PenHolder.Width;

			string name = linewidth.ToString();
			cb.SelectedItem = name;
		}


		public static void SetFillCheckBox(System.Windows.Forms.CheckBox chk, PlotStyle ps)
		{
			bool bFill = false; // default
			if(null!=ps && null!=ps.LineStyle)
				bFill = ps.LineStyle.FillArea;

			chk.Checked = bFill;

		}

		public static void EnableDisableFillElements(PlotStyleDialog dlg, PlotStyle ps)
		{
			bool bFill = ps.LineStyle.FillArea;
			dlg.m_cbLineFillColor.Enabled = bFill;
			dlg.m_cbLineFillDirection.Enabled = bFill;
		}

		public static void FillFillDirectionComboBox(System.Windows.Forms.ComboBox cb)
		{
			string [] names = System.Enum.GetNames(typeof(LineStyles.FillDirection));
			foreach(string name in names)
			{
				cb.Items.Add(name);
			}
		}

		public static void SetFillDirectionComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			LineStyles.FillDirection dir = LineStyles.FillDirection.Bottom; // default
			if(null!=ps && null!=ps.LineStyle)
				dir = ps.LineStyle.FillDirection;

			string name = dir.ToString();
			cb.SelectedItem = name;
		}

		public static void SetFillColorComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			string name = "Custom"; // default

			if(null!=ps && null!=ps.LineStyle && null!=ps.LineStyle.FillBrush)
			{
				name = "Custom";
				if(ps.LineStyle.FillBrush.BrushType==BrushType.SolidBrush) 
				{
					name = PlotStyle.GetPlotColorName(ps.LineStyle.FillBrush.Color);
					if(null==name) name = "Custom";
				}
			}
			cb.SelectedItem = name;
		}



		public static void FillSymbolShapeComboBox(System.Windows.Forms.ComboBox cb)
		{
			string [] names = System.Enum.GetNames(typeof(ScatterStyles.Shape));
			foreach(string name in names)
			{
				cb.Items.Add(name);
			}
		}
		public static void SetSymbolShapeComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			ScatterStyles.Shape sh = ScatterStyles.Shape.NoSymbol;
			if(null!=ps && null!=ps.ScatterStyle)
				sh = ps.ScatterStyle.Shape;

			string name = sh.ToString();
			cb.SelectedItem = name;
		}



		public static void FillSymbolStyleComboBox(System.Windows.Forms.ComboBox cb)
		{
			string [] names = System.Enum.GetNames(typeof(ScatterStyles.Style));
			foreach(string name in names)
			{
				cb.Items.Add(name);
			}
		}
		public static void SetSymbolStyleComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			ScatterStyles.Style sh = ScatterStyles.Style.Solid;
			if(null!=ps && null!=ps.ScatterStyle)
				sh = ps.ScatterStyle.Style;

			string name = sh.ToString();
			cb.SelectedItem = name;
		}

		static public void FillSymbolSizeComboBox(System.Windows.Forms.ComboBox cb)
		{
			float[] SymbolSizes = 
			{	0,1,3,5,8,12,15,18,24,30};

			foreach(float width in SymbolSizes)
			{
				cb.Items.Add(width.ToString());
			}
		}

		static public void SetSymbolSizeComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			float symbolsize = 1;
			if(null!=ps && null!=ps.ScatterStyle && null!=ps.ScatterStyle)
				symbolsize = ps.ScatterStyle.SymbolSize;

			string name = symbolsize.ToString();
			cb.SelectedItem = name;
		}

		static public void FillPlotTypeComboBox(System.Windows.Forms.ComboBox cb)
		{
			cb.Items.Add("Line");
			cb.Items.Add("Symbol");
			cb.Items.Add("Line_Symbol");
		}
		static public void SetPlotTypeComboBox(System.Windows.Forms.ComboBox cb, PlotStyle ps)
		{
			cb.Items.Add("Nothing");
			cb.Items.Add("Line");
			cb.Items.Add("Symbol");
			cb.Items.Add("Line_Symbol");

			if(null!=ps.LineStyle && null!=ps.ScatterStyle)
				cb.SelectedItem = "Line_Symbol";
			else if(null!=ps.LineStyle && null==ps.ScatterStyle)
				cb.SelectedItem = "Line";
			else if(null==ps.LineStyle && null!=ps.ScatterStyle)
				cb.SelectedItem = "Symbol";
			else
				cb.SelectedItem = "Nothing";
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
			this.m_lblDataSets = new System.Windows.Forms.Label();
			this.m_txtDatasets = new System.Windows.Forms.TextBox();
			this.m_cbPlotType = new System.Windows.Forms.ComboBox();
			this.m_lblPlotType = new System.Windows.Forms.Label();
			this.m_lblLineSymbolColor = new System.Windows.Forms.Label();
			this.m_cbLineSymbolColor = new System.Windows.Forms.ComboBox();
			this.m_btLineSymbolColorDetails = new System.Windows.Forms.Button();
			this.m_chkLineSymbolGap = new System.Windows.Forms.CheckBox();
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
			this.m_btOK = new System.Windows.Forms.Button();
			this.m_btCancel = new System.Windows.Forms.Button();
			this.m_btWorksheet = new System.Windows.Forms.Button();
			this.m_btRemove = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.m_chkPlotGroupSymbol = new System.Windows.Forms.CheckBox();
			this.m_chkPlotGroupLineType = new System.Windows.Forms.CheckBox();
			this.m_chkPlotGroupColor = new System.Windows.Forms.CheckBox();
			this.m_rbtPlotGroupIncremental = new System.Windows.Forms.RadioButton();
			this.m_rbtPlotGroupIndependent = new System.Windows.Forms.RadioButton();
			this.m_gbLine.SuspendLayout();
			this.m_gbSymbol.SuspendLayout();
			this.m_gbSymbolDropLine.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_lblDataSets
			// 
			this.m_lblDataSets.Location = new System.Drawing.Point(16, 8);
			this.m_lblDataSets.Name = "m_lblDataSets";
			this.m_lblDataSets.Size = new System.Drawing.Size(56, 16);
			this.m_lblDataSets.TabIndex = 0;
			this.m_lblDataSets.Text = "Dataset(s)";
			// 
			// m_txtDatasets
			// 
			this.m_txtDatasets.Location = new System.Drawing.Point(88, 8);
			this.m_txtDatasets.Name = "m_txtDatasets";
			this.m_txtDatasets.Size = new System.Drawing.Size(320, 20);
			this.m_txtDatasets.TabIndex = 1;
			this.m_txtDatasets.Text = "textBox1";
			// 
			// m_cbPlotType
			// 
			this.m_cbPlotType.Location = new System.Drawing.Point(88, 40);
			this.m_cbPlotType.Name = "m_cbPlotType";
			this.m_cbPlotType.Size = new System.Drawing.Size(320, 21);
			this.m_cbPlotType.TabIndex = 2;
			this.m_cbPlotType.Text = "comboBox1";
			// 
			// m_lblPlotType
			// 
			this.m_lblPlotType.Location = new System.Drawing.Point(16, 40);
			this.m_lblPlotType.Name = "m_lblPlotType";
			this.m_lblPlotType.Size = new System.Drawing.Size(56, 16);
			this.m_lblPlotType.TabIndex = 3;
			this.m_lblPlotType.Text = "Plot Type";
			// 
			// m_lblLineSymbolColor
			// 
			this.m_lblLineSymbolColor.Location = new System.Drawing.Point(16, 80);
			this.m_lblLineSymbolColor.Name = "m_lblLineSymbolColor";
			this.m_lblLineSymbolColor.Size = new System.Drawing.Size(100, 16);
			this.m_lblLineSymbolColor.TabIndex = 4;
			this.m_lblLineSymbolColor.Text = "Line/Symbol Color";
			// 
			// m_cbLineSymbolColor
			// 
			this.m_cbLineSymbolColor.Location = new System.Drawing.Point(120, 80);
			this.m_cbLineSymbolColor.Name = "m_cbLineSymbolColor";
			this.m_cbLineSymbolColor.Size = new System.Drawing.Size(104, 21);
			this.m_cbLineSymbolColor.TabIndex = 5;
			this.m_cbLineSymbolColor.Text = "comboBox1";
			this.m_cbLineSymbolColor.SelectedIndexChanged += new System.EventHandler(this.m_cbLineSymbolColor_SelectedIndexChanged);
			// 
			// m_btLineSymbolColorDetails
			// 
			this.m_btLineSymbolColorDetails.Location = new System.Drawing.Point(232, 80);
			this.m_btLineSymbolColorDetails.Name = "m_btLineSymbolColorDetails";
			this.m_btLineSymbolColorDetails.Size = new System.Drawing.Size(48, 24);
			this.m_btLineSymbolColorDetails.TabIndex = 6;
			this.m_btLineSymbolColorDetails.Text = "Adv..";
			this.m_btLineSymbolColorDetails.Click += new System.EventHandler(this.m_btLineSymbolColorDetails_Click);
			// 
			// m_chkLineSymbolGap
			// 
			this.m_chkLineSymbolGap.Location = new System.Drawing.Point(288, 80);
			this.m_chkLineSymbolGap.Name = "m_chkLineSymbolGap";
			this.m_chkLineSymbolGap.Size = new System.Drawing.Size(112, 24);
			this.m_chkLineSymbolGap.TabIndex = 7;
			this.m_chkLineSymbolGap.Text = "Line/Symbol Gap";
			this.m_chkLineSymbolGap.CheckedChanged += new System.EventHandler(this.m_chkLineSymbolGap_CheckedChanged);
			// 
			// m_gbLine
			// 
			this.m_gbLine.Controls.AddRange(new System.Windows.Forms.Control[] {
																																					 this.m_btLineFillColorDetails,
																																					 this.m_cbLineFillColor,
																																					 this.m_cbLineFillDirection,
																																					 this.label5,
																																					 this.label4,
																																					 this.m_chkLineFillArea,
																																					 this.m_cbLineWidth,
																																					 this.m_cbLineType,
																																					 this.m_cbLineConnect,
																																					 this.label3,
																																					 this.label2,
																																					 this.label1});
			this.m_gbLine.Location = new System.Drawing.Point(8, 120);
			this.m_gbLine.Name = "m_gbLine";
			this.m_gbLine.Size = new System.Drawing.Size(208, 232);
			this.m_gbLine.TabIndex = 8;
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
			this.m_btLineFillColorDetails.Click += new System.EventHandler(this.m_btLineFillColorDetails_Click);
			// 
			// m_cbLineFillColor
			// 
			this.m_cbLineFillColor.Location = new System.Drawing.Point(80, 176);
			this.m_cbLineFillColor.Name = "m_cbLineFillColor";
			this.m_cbLineFillColor.Size = new System.Drawing.Size(120, 21);
			this.m_cbLineFillColor.TabIndex = 10;
			this.m_cbLineFillColor.Text = "comboBox1";
			this.m_cbLineFillColor.SelectedIndexChanged += new System.EventHandler(this.m_cbLineFillColor_SelectedIndexChanged);
			// 
			// m_cbLineFillDirection
			// 
			this.m_cbLineFillDirection.Location = new System.Drawing.Point(80, 152);
			this.m_cbLineFillDirection.Name = "m_cbLineFillDirection";
			this.m_cbLineFillDirection.Size = new System.Drawing.Size(121, 21);
			this.m_cbLineFillDirection.TabIndex = 9;
			this.m_cbLineFillDirection.Text = "comboBox1";
			this.m_cbLineFillDirection.SelectedIndexChanged += new System.EventHandler(this.m_cbLineFillDirection_SelectedIndexChanged);
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
			this.m_chkLineFillArea.CheckedChanged += new System.EventHandler(this.m_chkLineFillArea_CheckedChanged);
			// 
			// m_cbLineWidth
			// 
			this.m_cbLineWidth.Location = new System.Drawing.Point(80, 80);
			this.m_cbLineWidth.Name = "m_cbLineWidth";
			this.m_cbLineWidth.Size = new System.Drawing.Size(121, 21);
			this.m_cbLineWidth.TabIndex = 5;
			this.m_cbLineWidth.Text = "comboBox1";
			this.m_cbLineWidth.SelectedIndexChanged += new System.EventHandler(this.m_cbLineWidth_SelectedIndexChanged);
			// 
			// m_cbLineType
			// 
			this.m_cbLineType.Location = new System.Drawing.Point(80, 48);
			this.m_cbLineType.Name = "m_cbLineType";
			this.m_cbLineType.Size = new System.Drawing.Size(121, 21);
			this.m_cbLineType.TabIndex = 4;
			this.m_cbLineType.Text = "comboBox1";
			this.m_cbLineType.SelectedIndexChanged += new System.EventHandler(this.m_cbLineType_SelectedIndexChanged);
			// 
			// m_cbLineConnect
			// 
			this.m_cbLineConnect.Location = new System.Drawing.Point(80, 16);
			this.m_cbLineConnect.Name = "m_cbLineConnect";
			this.m_cbLineConnect.Size = new System.Drawing.Size(121, 21);
			this.m_cbLineConnect.TabIndex = 3;
			this.m_cbLineConnect.Text = "comboBox1";
			this.m_cbLineConnect.SelectedIndexChanged += new System.EventHandler(this.m_cbLineConnect_SelectedIndexChanged);
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
			// m_gbSymbol
			// 
			this.m_gbSymbol.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						 this.m_edSymbolSkipFrequency,
																																						 this.label6,
																																						 this.m_chkSymbolSkipPoints,
																																						 this.m_gbSymbolDropLine,
																																						 this.m_cbSymbolSize,
																																						 this.m_cbSymbolStyle,
																																						 this.m_cbSymbolShape,
																																						 this.m_lblSymbolSize,
																																						 this.m_lblSymbolStyle,
																																						 this.m_lblSymbolShape});
			this.m_gbSymbol.Location = new System.Drawing.Point(224, 120);
			this.m_gbSymbol.Name = "m_gbSymbol";
			this.m_gbSymbol.Size = new System.Drawing.Size(216, 232);
			this.m_gbSymbol.TabIndex = 9;
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
			this.m_edSymbolSkipFrequency.TextChanged += new System.EventHandler(this.m_edSymbolSkipFrequency_TextChanged);
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
			this.m_chkSymbolSkipPoints.CheckedChanged += new System.EventHandler(this.m_chkSymbolSkipPoints_CheckedChanged);
			// 
			// m_gbSymbolDropLine
			// 
			this.m_gbSymbolDropLine.Controls.AddRange(new System.Windows.Forms.Control[] {
																																										 this.m_chkSymbolDropLineBottom,
																																										 this.m_chkSymbolDropLineTop,
																																										 this.m_chkSymbolDropLineRight,
																																										 this.m_chkSymbolDropLineLeft});
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
			this.m_chkSymbolDropLineBottom.CheckedChanged += new System.EventHandler(this.m_chkSymbolDropLineBottom_CheckedChanged);
			// 
			// m_chkSymbolDropLineTop
			// 
			this.m_chkSymbolDropLineTop.Location = new System.Drawing.Point(16, 48);
			this.m_chkSymbolDropLineTop.Name = "m_chkSymbolDropLineTop";
			this.m_chkSymbolDropLineTop.Size = new System.Drawing.Size(48, 16);
			this.m_chkSymbolDropLineTop.TabIndex = 2;
			this.m_chkSymbolDropLineTop.Text = "Top";
			this.m_chkSymbolDropLineTop.CheckedChanged += new System.EventHandler(this.m_chkSymbolDropLineTop_CheckedChanged);
			// 
			// m_chkSymbolDropLineRight
			// 
			this.m_chkSymbolDropLineRight.Location = new System.Drawing.Point(80, 24);
			this.m_chkSymbolDropLineRight.Name = "m_chkSymbolDropLineRight";
			this.m_chkSymbolDropLineRight.Size = new System.Drawing.Size(56, 16);
			this.m_chkSymbolDropLineRight.TabIndex = 1;
			this.m_chkSymbolDropLineRight.Text = "Right";
			this.m_chkSymbolDropLineRight.CheckedChanged += new System.EventHandler(this.m_chkSymbolDropLineRight_CheckedChanged);
			// 
			// m_chkSymbolDropLineLeft
			// 
			this.m_chkSymbolDropLineLeft.Location = new System.Drawing.Point(16, 24);
			this.m_chkSymbolDropLineLeft.Name = "m_chkSymbolDropLineLeft";
			this.m_chkSymbolDropLineLeft.Size = new System.Drawing.Size(48, 16);
			this.m_chkSymbolDropLineLeft.TabIndex = 0;
			this.m_chkSymbolDropLineLeft.Text = "Left";
			this.m_chkSymbolDropLineLeft.CheckedChanged += new System.EventHandler(this.m_chkSymbolDropLineLeft_CheckedChanged);
			// 
			// m_cbSymbolSize
			// 
			this.m_cbSymbolSize.Location = new System.Drawing.Point(56, 80);
			this.m_cbSymbolSize.Name = "m_cbSymbolSize";
			this.m_cbSymbolSize.Size = new System.Drawing.Size(152, 21);
			this.m_cbSymbolSize.TabIndex = 5;
			this.m_cbSymbolSize.Text = "comboBox1";
			this.m_cbSymbolSize.SelectedIndexChanged += new System.EventHandler(this.m_cbSymbolSize_SelectedIndexChanged);
			// 
			// m_cbSymbolStyle
			// 
			this.m_cbSymbolStyle.Location = new System.Drawing.Point(56, 48);
			this.m_cbSymbolStyle.Name = "m_cbSymbolStyle";
			this.m_cbSymbolStyle.Size = new System.Drawing.Size(152, 21);
			this.m_cbSymbolStyle.TabIndex = 4;
			this.m_cbSymbolStyle.Text = "comboBox1";
			this.m_cbSymbolStyle.SelectedIndexChanged += new System.EventHandler(this.m_cbSymbolStyle_SelectedIndexChanged);
			// 
			// m_cbSymbolShape
			// 
			this.m_cbSymbolShape.Location = new System.Drawing.Point(56, 16);
			this.m_cbSymbolShape.Name = "m_cbSymbolShape";
			this.m_cbSymbolShape.Size = new System.Drawing.Size(144, 21);
			this.m_cbSymbolShape.TabIndex = 3;
			this.m_cbSymbolShape.Text = "comboBox1";
			this.m_cbSymbolShape.SelectedIndexChanged += new System.EventHandler(this.m_cbSymbolShape_SelectedIndexChanged);
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
			// m_btOK
			// 
			this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btOK.Location = new System.Drawing.Point(456, 8);
			this.m_btOK.Name = "m_btOK";
			this.m_btOK.Size = new System.Drawing.Size(88, 24);
			this.m_btOK.TabIndex = 10;
			this.m_btOK.Text = "OK";
			// 
			// m_btCancel
			// 
			this.m_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btCancel.Location = new System.Drawing.Point(456, 40);
			this.m_btCancel.Name = "m_btCancel";
			this.m_btCancel.Size = new System.Drawing.Size(88, 24);
			this.m_btCancel.TabIndex = 11;
			this.m_btCancel.Text = "Cancel";
			// 
			// m_btWorksheet
			// 
			this.m_btWorksheet.Location = new System.Drawing.Point(456, 72);
			this.m_btWorksheet.Name = "m_btWorksheet";
			this.m_btWorksheet.Size = new System.Drawing.Size(88, 24);
			this.m_btWorksheet.TabIndex = 12;
			this.m_btWorksheet.Text = "Worksheet";
			// 
			// m_btRemove
			// 
			this.m_btRemove.Location = new System.Drawing.Point(456, 104);
			this.m_btRemove.Name = "m_btRemove";
			this.m_btRemove.Size = new System.Drawing.Size(88, 24);
			this.m_btRemove.TabIndex = 13;
			this.m_btRemove.Text = "Remove";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.m_chkPlotGroupSymbol,
																																						this.m_chkPlotGroupLineType,
																																						this.m_chkPlotGroupColor,
																																						this.m_rbtPlotGroupIncremental,
																																						this.m_rbtPlotGroupIndependent});
			this.groupBox1.Location = new System.Drawing.Point(448, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(96, 208);
			this.groupBox1.TabIndex = 14;
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
			this.m_chkPlotGroupSymbol.CheckedChanged += new System.EventHandler(this.OnPlotGroupSymbolChanged);
			// 
			// m_chkPlotGroupLineType
			// 
			this.m_chkPlotGroupLineType.Location = new System.Drawing.Point(8, 104);
			this.m_chkPlotGroupLineType.Name = "m_chkPlotGroupLineType";
			this.m_chkPlotGroupLineType.Size = new System.Drawing.Size(80, 16);
			this.m_chkPlotGroupLineType.TabIndex = 3;
			this.m_chkPlotGroupLineType.Text = "Line Type";
			this.m_chkPlotGroupLineType.CheckedChanged += new System.EventHandler(this.OnPlotGroupLineTypeChanged);
			// 
			// m_chkPlotGroupColor
			// 
			this.m_chkPlotGroupColor.Location = new System.Drawing.Point(8, 80);
			this.m_chkPlotGroupColor.Name = "m_chkPlotGroupColor";
			this.m_chkPlotGroupColor.Size = new System.Drawing.Size(72, 16);
			this.m_chkPlotGroupColor.TabIndex = 2;
			this.m_chkPlotGroupColor.Text = "Color";
			this.m_chkPlotGroupColor.CheckedChanged += new System.EventHandler(this.OnPlotGroupColorChanged);
			// 
			// m_rbtPlotGroupIncremental
			// 
			this.m_rbtPlotGroupIncremental.Location = new System.Drawing.Point(8, 48);
			this.m_rbtPlotGroupIncremental.Name = "m_rbtPlotGroupIncremental";
			this.m_rbtPlotGroupIncremental.Size = new System.Drawing.Size(88, 24);
			this.m_rbtPlotGroupIncremental.TabIndex = 1;
			this.m_rbtPlotGroupIncremental.Text = "Incremental";
			this.m_rbtPlotGroupIncremental.CheckedChanged += new System.EventHandler(this.OnPlotGroupIncrementalChanged);
			// 
			// m_rbtPlotGroupIndependent
			// 
			this.m_rbtPlotGroupIndependent.Location = new System.Drawing.Point(8, 24);
			this.m_rbtPlotGroupIndependent.Name = "m_rbtPlotGroupIndependent";
			this.m_rbtPlotGroupIndependent.Size = new System.Drawing.Size(88, 24);
			this.m_rbtPlotGroupIndependent.TabIndex = 0;
			this.m_rbtPlotGroupIndependent.Text = "Independent";
			this.m_rbtPlotGroupIndependent.CheckedChanged += new System.EventHandler(this.OnPlotGroupIndependentChanged);
			// 
			// PlotStyleDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(552, 358);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.groupBox1,
																																	this.m_btRemove,
																																	this.m_btWorksheet,
																																	this.m_btCancel,
																																	this.m_btOK,
																																	this.m_gbSymbol,
																																	this.m_gbLine,
																																	this.m_chkLineSymbolGap,
																																	this.m_btLineSymbolColorDetails,
																																	this.m_cbLineSymbolColor,
																																	this.m_lblLineSymbolColor,
																																	this.m_lblPlotType,
																																	this.m_cbPlotType,
																																	this.m_txtDatasets,
																																	this.m_lblDataSets});
			this.Name = "PlotStyleDialog";
			this.Text = "PlotDetailsDialog";
			this.m_gbLine.ResumeLayout(false);
			this.m_gbSymbol.ResumeLayout(false);
			this.m_gbSymbolDropLine.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void m_cbLineConnect_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)m_cbLineConnect.SelectedItem;
				m_PlotStyle.LineStyle.Connection = (LineStyles.ConnectionStyle)Enum.Parse(typeof(LineStyles.ConnectionStyle),str);
			}
			}

		private void m_cbLineType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbLineType.SelectedItem;
				m_PlotStyle.LineStyle.PenHolder.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),str);
			}
		}

		private void m_cbLineWidth_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbLineWidth.SelectedItem;
				float width = System.Convert.ToSingle(str);
				m_PlotStyle.LineStyle.PenHolder.Width = width;
				// m_PlotStyle.ScatterStyle.Pen.Width = width; // do not change scatterstyle pen width, since only SymbolSize influence the pen width of the scatter
			}
		
		}

		private void m_chkLineFillArea_CheckedChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				bool bFill = this.m_chkLineFillArea.Checked;
		
				m_PlotStyle.LineStyle.FillArea = bFill;

				EnableDisableFillElements(this,m_PlotStyle);
			}
		}

		private void m_cbLineFillDirection_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbLineFillDirection.SelectedItem;
				m_PlotStyle.LineStyle.FillDirection = (LineStyles.FillDirection)Enum.Parse(typeof(LineStyles.FillDirection),str);
			}
		}

		private void m_cbLineFillColor_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbLineFillColor.SelectedItem;
				if(str!="Custom")
					m_PlotStyle.LineStyle.FillBrush = new BrushHolder(Color.FromName(str));
			}
		}

		private void m_chkLineSymbolGap_CheckedChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				bool bGap = this.m_chkLineSymbolGap.Checked;
				m_PlotStyle.LineSymbolGap = bGap;
			}
		}

		private void m_cbSymbolShape_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbSymbolShape.SelectedItem;
				m_PlotStyle.ScatterStyle.Shape = (ScatterStyles.Shape)Enum.Parse(typeof(ScatterStyles.Shape),str);
			}
		}

		private void m_cbSymbolStyle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbSymbolStyle.SelectedItem;
				m_PlotStyle.ScatterStyle.Style = (ScatterStyles.Style)Enum.Parse(typeof(ScatterStyles.Style),str);
			}
		
		}

		private void m_cbSymbolSize_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbSymbolSize.SelectedItem;
				m_PlotStyle.ScatterStyle.SymbolSize = System.Convert.ToSingle(str);
			}
		
		}

		private void m_chkSymbolDropLineLeft_CheckedChanged(object sender, System.EventArgs e)
		{
			if(this.m_chkSymbolDropLineLeft.Checked) 
				m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Left;
			else
				m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Left);
		}

		private void m_chkSymbolDropLineRight_CheckedChanged(object sender, System.EventArgs e)
		{
			if(this.m_chkSymbolDropLineRight.Checked) 
				m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Right;
			else
				m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Right);
		}

		private void m_chkSymbolDropLineTop_CheckedChanged(object sender, System.EventArgs e)
		{
			if(this.m_chkSymbolDropLineTop.Checked) 
				m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Top;
			else
				m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Top);
		
		}

		private void m_chkSymbolDropLineBottom_CheckedChanged(object sender, System.EventArgs e)
		{
			if(this.m_chkSymbolDropLineBottom.Checked) 
				m_PlotStyle.ScatterStyle.DropLine |= ScatterStyles.DropLine.Bottom;
			else
				m_PlotStyle.ScatterStyle.DropLine &= (ScatterStyles.DropLine.All^ScatterStyles.DropLine.Bottom);
		
		}

		private void m_chkSymbolSkipPoints_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		private void m_edSymbolSkipFrequency_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void m_cbLineSymbolColor_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_bEnableEvents)
			{
				string str = (string)this.m_cbLineSymbolColor.SelectedItem;
				if(str!="Custom")
				{
					m_PlotStyle.LineStyle.PenHolder.Color = Color.FromName(str);
					m_PlotStyle.ScatterStyle.Pen.Color = Color.FromName(str);
				}
			}

		}

		private void m_btLineSymbolColorDetails_Click(object sender, System.EventArgs e)
		{
		
		}

		private void m_btLineFillColorDetails_Click(object sender, System.EventArgs e)
		{
		
		}

		private void OnPlotGroupIndependentChanged(object sender, System.EventArgs e)
		{
			this.m_chkPlotGroupColor.Enabled=false;
			this.m_chkPlotGroupLineType.Enabled=false;
			this.m_chkPlotGroupSymbol.Enabled=false;
			
			this.GetPlotGroupElements();
		}

		private void OnPlotGroupIncrementalChanged(object sender, System.EventArgs e)
		{
			this.m_chkPlotGroupColor.Enabled=true;
			this.m_chkPlotGroupLineType.Enabled=true;
			this.m_chkPlotGroupSymbol.Enabled=true;
			
			this.GetPlotGroupElements();
		}

		private void OnPlotGroupColorChanged(object sender, System.EventArgs e)
		{
		this.GetPlotGroupElements();
		}

		private void OnPlotGroupLineTypeChanged(object sender, System.EventArgs e)
		{
		this.GetPlotGroupElements();
		}

		private void OnPlotGroupSymbolChanged(object sender, System.EventArgs e)
		{
			this.GetPlotGroupElements();
		}			
	}
}
