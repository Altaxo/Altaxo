using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Gui
{
	/// <summary>
	/// Summary description for SpinAndComboBoxControl.
	/// </summary>
	public class SpinAndComboBoxControl : System.Windows.Forms.UserControl, IIntegerAndComboBoxView
	{
		IIntegerAndComboBoxController m_Controller;
		private System.Windows.Forms.NumericUpDown m_IntegerUpDown;
		private System.Windows.Forms.ComboBox m_ComboBox;
		private System.Windows.Forms.Label m_IntegerLabel;
		private System.Windows.Forms.Label m_ComboBoxLabel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SpinAndComboBoxControl()
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
			this.m_IntegerUpDown = new System.Windows.Forms.NumericUpDown();
			this.m_ComboBox = new System.Windows.Forms.ComboBox();
			this.m_IntegerLabel = new System.Windows.Forms.Label();
			this.m_ComboBoxLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.m_IntegerUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// m_IntegerUpDown
			// 
			this.m_IntegerUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_IntegerUpDown.Location = new System.Drawing.Point(8, 64);
			this.m_IntegerUpDown.Minimum = new System.Decimal(new int[] {
																																		1,
																																		0,
																																		0,
																																		0});
			this.m_IntegerUpDown.Name = "m_IntegerUpDown";
			this.m_IntegerUpDown.Size = new System.Drawing.Size(216, 20);
			this.m_IntegerUpDown.TabIndex = 1;
			this.m_IntegerUpDown.Value = new System.Decimal(new int[] {
																																	1,
																																	0,
																																	0,
																																	0});
			this.m_IntegerUpDown.Validating += new System.ComponentModel.CancelEventHandler(this.EhInteger_Validating);
			// 
			// m_ComboBox
			// 
			this.m_ComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_ComboBox.Location = new System.Drawing.Point(8, 144);
			this.m_ComboBox.Name = "m_ComboBox";
			this.m_ComboBox.Size = new System.Drawing.Size(216, 21);
			this.m_ComboBox.TabIndex = 3;
			this.m_ComboBox.SelectionChangeCommitted += new System.EventHandler(this.EhComboBox_SelectionChangeCommit);
			// 
			// m_IntegerLabel
			// 
			this.m_IntegerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_IntegerLabel.Location = new System.Drawing.Point(8, 16);
			this.m_IntegerLabel.Name = "m_IntegerLabel";
			this.m_IntegerLabel.Size = new System.Drawing.Size(208, 40);
			this.m_IntegerLabel.TabIndex = 0;
			this.m_IntegerLabel.Text = "Lorem ipsum";
			this.m_IntegerLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// m_ComboBoxLabel
			// 
			this.m_ComboBoxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_ComboBoxLabel.Location = new System.Drawing.Point(8, 104);
			this.m_ComboBoxLabel.Name = "m_ComboBoxLabel";
			this.m_ComboBoxLabel.Size = new System.Drawing.Size(208, 32);
			this.m_ComboBoxLabel.TabIndex = 2;
			this.m_ComboBoxLabel.Text = "Lorem ipsum";
			this.m_ComboBoxLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// SpinAndComboBoxControl
			// 
			this.Controls.Add(this.m_ComboBoxLabel);
			this.Controls.Add(this.m_IntegerLabel);
			this.Controls.Add(this.m_ComboBox);
			this.Controls.Add(this.m_IntegerUpDown);
			this.Name = "SpinAndComboBoxControl";
			this.Size = new System.Drawing.Size(232, 176);
			((System.ComponentModel.ISupportInitialize)(this.m_IntegerUpDown)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region IIntegerAndComboBoxView Members

		public IIntegerAndComboBoxController Controller
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value;
			}
		}

		public Form Form
		{
			get
			{
				
				return this.ParentForm;
			}
		}

		public void ComboBox_Initialize(ListBoxEntry[] items, ListBoxEntry defaultItem)
		{
			m_ComboBox.Items.Clear();
			m_ComboBox.Items.AddRange(items);
			m_ComboBox.SelectedItem = defaultItem;
		}

		public void ComboBoxLabel_Initialize(string text)
		{
			m_ComboBoxLabel.Text = text;
		}

		public void IntegerEdit_Initialize(int min, int max, int val)
		{
			this.m_IntegerUpDown.Minimum	= min;
			this.m_IntegerUpDown.Maximum	= max;
			this.m_IntegerUpDown.Value		= val;
		}

		public void IntegerLabel_Initialize(string text)
		{
			this.m_IntegerLabel.Text = text;
		}

		#endregion

		private void EhInteger_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_IntegerChanged((int)this.m_IntegerUpDown.Value,ref bCancel);
				e.Cancel = bCancel;
			}
		}

		private void EhComboBox_SelectionChangeCommit(object sender, System.EventArgs e)
		{
		if(null!=Controller)
			Controller.EhView_ComboBoxSelectionChanged((ListBoxEntry)this.m_ComboBox.SelectedItem);
		}

		#region IMVCView Members

		public object ControllerObject
		{
			get
			{
				
				return Controller;
			}
			set
			{
				Controller = value as IIntegerAndComboBoxController;
			}
		}

		#endregion
	}
}
