using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for TitleFormatLayerControl.
	/// </summary>
	public class TitleFormatLayerControl : System.Windows.Forms.UserControl, ITitleFormatLayerView
		{
		protected ITitleFormatLayerController m_Ctrl;
		protected int m_SuppressEvents=0;
		private System.Windows.Forms.TextBox m_Format_edAxisPositionValue;
		private System.Windows.Forms.ComboBox m_Format_cbAxisPosition;
		private System.Windows.Forms.ComboBox m_Format_cbMinorTicks;
		private System.Windows.Forms.ComboBox m_Format_cbMajorTicks;
		private System.Windows.Forms.ComboBox m_Format_cbMajorTickLength;
		private System.Windows.Forms.ComboBox m_Format_cbThickness;
		private System.Windows.Forms.ComboBox m_Format_cbColor;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox m_Format_edTitle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox m_Format_chkShowAxis;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TitleFormatLayerControl()
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
			this.SuspendLayout();
			// 
			// m_Format_edAxisPositionValue
			// 
			this.m_Format_edAxisPositionValue.Location = new System.Drawing.Point(336, 160);
			this.m_Format_edAxisPositionValue.Name = "m_Format_edAxisPositionValue";
			this.m_Format_edAxisPositionValue.Size = new System.Drawing.Size(96, 20);
			this.m_Format_edAxisPositionValue.TabIndex = 33;
			this.m_Format_edAxisPositionValue.Text = "textBox1";
			this.m_Format_edAxisPositionValue.TextChanged += new System.EventHandler(this.EhAxisPositionValue_TextChanged);
			// 
			// m_Format_cbAxisPosition
			// 
			this.m_Format_cbAxisPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_Format_cbAxisPosition.Location = new System.Drawing.Point(336, 120);
			this.m_Format_cbAxisPosition.Name = "m_Format_cbAxisPosition";
			this.m_Format_cbAxisPosition.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbAxisPosition.TabIndex = 32;
			this.m_Format_cbAxisPosition.SelectionChangeCommitted += new System.EventHandler(this.EhAxisPosition_SelectionChangeCommit);
			// 
			// m_Format_cbMinorTicks
			// 
			this.m_Format_cbMinorTicks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_Format_cbMinorTicks.Location = new System.Drawing.Point(336, 80);
			this.m_Format_cbMinorTicks.Name = "m_Format_cbMinorTicks";
			this.m_Format_cbMinorTicks.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbMinorTicks.TabIndex = 31;
			this.m_Format_cbMinorTicks.SelectionChangeCommitted += new System.EventHandler(this.EhMinorTicks_SelectionChangeCommit);
			// 
			// m_Format_cbMajorTicks
			// 
			this.m_Format_cbMajorTicks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_Format_cbMajorTicks.Location = new System.Drawing.Point(336, 40);
			this.m_Format_cbMajorTicks.Name = "m_Format_cbMajorTicks";
			this.m_Format_cbMajorTicks.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbMajorTicks.TabIndex = 30;
			this.m_Format_cbMajorTicks.SelectionChangeCommitted += new System.EventHandler(this.EhMajorTicks_SelectionChangeCommit);
			// 
			// m_Format_cbMajorTickLength
			// 
			this.m_Format_cbMajorTickLength.Location = new System.Drawing.Point(56, 160);
			this.m_Format_cbMajorTickLength.Name = "m_Format_cbMajorTickLength";
			this.m_Format_cbMajorTickLength.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbMajorTickLength.TabIndex = 29;
			this.m_Format_cbMajorTickLength.Text = "10";
			this.m_Format_cbMajorTickLength.Validating += new System.ComponentModel.CancelEventHandler(this.EhMajorTickLength_Validating);
			this.m_Format_cbMajorTickLength.SelectionChangeCommitted += new System.EventHandler(this.EhMajorTickLength_SelectionChangeCommit);
			// 
			// m_Format_cbThickness
			// 
			this.m_Format_cbThickness.Location = new System.Drawing.Point(56, 120);
			this.m_Format_cbThickness.Name = "m_Format_cbThickness";
			this.m_Format_cbThickness.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbThickness.TabIndex = 28;
			this.m_Format_cbThickness.Text = "1";
			this.m_Format_cbThickness.Validating += new System.ComponentModel.CancelEventHandler(this.EhThickness_Validating);
			this.m_Format_cbThickness.SelectionChangeCommitted += new System.EventHandler(this.EhThickness_SelectionChangeCommit);
			// 
			// m_Format_cbColor
			// 
			this.m_Format_cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_Format_cbColor.Location = new System.Drawing.Point(56, 80);
			this.m_Format_cbColor.Name = "m_Format_cbColor";
			this.m_Format_cbColor.Size = new System.Drawing.Size(96, 21);
			this.m_Format_cbColor.TabIndex = 27;
			this.m_Format_cbColor.SelectionChangeCommitted += new System.EventHandler(this.EhColor_SelectionChangeCommit);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(240, 168);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(48, 16);
			this.label12.TabIndex = 26;
			this.label12.Text = "Value";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(240, 128);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(80, 16);
			this.label11.TabIndex = 25;
			this.label11.Text = "Axis Position";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(240, 88);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 16);
			this.label10.TabIndex = 24;
			this.label10.Text = "Minor Ticks";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(240, 48);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(64, 16);
			this.label9.TabIndex = 23;
			this.label9.Text = "Major Ticks";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(0, 160);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 32);
			this.label8.TabIndex = 22;
			this.label8.Text = "Major Tick Length";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(0, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 16);
			this.label7.TabIndex = 21;
			this.label7.Text = "Thickness";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 16);
			this.label6.TabIndex = 20;
			this.label6.Text = "Color";
			// 
			// m_Format_edTitle
			// 
			this.m_Format_edTitle.Location = new System.Drawing.Point(56, 40);
			this.m_Format_edTitle.Name = "m_Format_edTitle";
			this.m_Format_edTitle.TabIndex = 19;
			this.m_Format_edTitle.Text = "";
			this.m_Format_edTitle.TextChanged += new System.EventHandler(this.EhTitle_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 18;
			this.label1.Text = "Title";
			// 
			// m_Format_chkShowAxis
			// 
			this.m_Format_chkShowAxis.Location = new System.Drawing.Point(56, 8);
			this.m_Format_chkShowAxis.Name = "m_Format_chkShowAxis";
			this.m_Format_chkShowAxis.Size = new System.Drawing.Size(136, 16);
			this.m_Format_chkShowAxis.TabIndex = 17;
			this.m_Format_chkShowAxis.Text = "Show Axis && Ticks";
			this.m_Format_chkShowAxis.CheckedChanged += new System.EventHandler(this.EhShowAxis_CheckedChanged);
			// 
			// TitleFormatLayerControl
			// 
			this.Controls.Add(this.m_Format_edAxisPositionValue);
			this.Controls.Add(this.m_Format_cbAxisPosition);
			this.Controls.Add(this.m_Format_cbMinorTicks);
			this.Controls.Add(this.m_Format_cbMajorTicks);
			this.Controls.Add(this.m_Format_cbMajorTickLength);
			this.Controls.Add(this.m_Format_cbThickness);
			this.Controls.Add(this.m_Format_cbColor);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.m_Format_edTitle);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.m_Format_chkShowAxis);
			this.Name = "TitleFormatLayerControl";
			this.Size = new System.Drawing.Size(440, 192);
			this.ResumeLayout(false);

		}
		#endregion

		public static void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
		{
			box.Items.Clear();
			box.Items.AddRange(names);
			box.SelectedItem = name;
		}

		public void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, int sel)
		{
			++m_SuppressEvents;
			box.Items.Clear();
			box.Items.AddRange(names);
			box.SelectedIndex = sel;
			--m_SuppressEvents;
		}

		#region ITitleFormatLayerView Members

		public ITitleFormatLayerController Controller
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

		public object ControllerObject
		{
			get { return Controller; }
			set { Controller = (ITitleFormatLayerController)value; }
		}

		public System.Windows.Forms.Form Form
		{
			get
			{
				
				return this.ParentForm;
			}
		}

		public void InitializeShowAxis(bool bShow)
		{
			++m_SuppressEvents;
			this.m_Format_chkShowAxis.Checked = bShow;
			--m_SuppressEvents;
		}

		public void InitializeTitle(string txt)
		{
			++m_SuppressEvents;
			this.m_Format_edTitle.Text = txt;
			--m_SuppressEvents;
		}

		public void InitializeColor(string[] arr, string sel)
		{
			InitComboBox(this.m_Format_cbColor,arr,sel);
		}

		public void InitializeThickness(string[] arr, string sel)
		{
			InitComboBox(this.m_Format_cbThickness,arr,sel);
		}

		public void InitializeMajorTickLength(string[] arr, string sel)
		{
			InitComboBox(this.m_Format_cbMajorTickLength,arr,sel);
		}

		public void InitializeMajorTicks(string[] arr, int sel)
		{
			InitComboBox(this.m_Format_cbMajorTicks,arr,sel);
		}

		public void InitializeMinorTicks(string[] arr, int sel)
		{
			InitComboBox(this.m_Format_cbMinorTicks,arr,sel);
		}

		public void InitializeAxisPosition(string[] arr, int sel)
		{
			InitComboBox(this.m_Format_cbAxisPosition,arr,sel);
		}

		public void InitializeAxisPositionValue(string txt)
		{
			++m_SuppressEvents;
			this.m_Format_edAxisPositionValue.Text = txt;
			--m_SuppressEvents;
		}

		public void InitializeAxisPositionValueEnabled(bool b)
		{
			this.m_Format_edAxisPositionValue.Enabled = b;
		}

		#endregion

		private void EhShowAxis_CheckedChanged(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
		m_Ctrl.EhView_ShowAxisChanged(this.m_Format_chkShowAxis.Checked);
		}

		private void EhTitle_TextChanged(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_TitleChanged(this.m_Format_edTitle.Text);
		}

		private void EhColor_SelectionChangeCommit(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_ColorChanged((string)this.m_Format_cbColor.SelectedItem);
		}

		private void EhThickness_SelectionChangeCommit(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_ThicknessChanged((string)this.m_Format_cbThickness.SelectedItem);
		}

		private void EhThickness_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_ThicknessChanged((string)this.m_Format_cbThickness.Text);
		}

		private void EhMajorTickLength_SelectionChangeCommit(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_MajorTickLengthChanged((string)this.m_Format_cbMajorTickLength.SelectedItem);
		}

		private void EhMajorTickLength_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_MajorTickLengthChanged((string)this.m_Format_cbMajorTickLength.SelectedItem);
		}


		private void EhMajorTicks_SelectionChangeCommit(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_MajorTicksChanged(this.m_Format_cbMajorTicks.SelectedIndex);
		}

		private void EhMinorTicks_SelectionChangeCommit(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_MinorTicksChanged(this.m_Format_cbMinorTicks.SelectedIndex);
		}

		private void EhAxisPosition_SelectionChangeCommit(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_AxisPositionChanged(this.m_Format_cbAxisPosition.SelectedIndex);
		}

		private void EhAxisPositionValue_TextChanged(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl && 0==m_SuppressEvents)
				m_Ctrl.EhView_AxisPositionValueChanged(this.m_Format_edAxisPositionValue.Text);
		}


	}
}
