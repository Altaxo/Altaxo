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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Altaxo;
using Altaxo.Data;

namespace Altaxo.Worksheet.GUI
{
	/// <summary>
	/// Summary description for WksSetColumnValuesDialog.
	/// </summary>
	public class SetColumnValuesDialog : System.Windows.Forms.Form
	{

		private System.Windows.Forms.TextBox edRowFrom;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox edRowTo;
		private System.Windows.Forms.TextBox edFormula;
		private System.Windows.Forms.Button btDoIt;
		
		private Altaxo.Data.DataTable dataTable;
		private Altaxo.Data.DataColumn dataColumn;
		public ColumnScript columnScript;

		private System.Windows.Forms.GroupBox grpStyle;
		private System.Windows.Forms.RadioButton rbStyleSetCol;
		private System.Windows.Forms.RadioButton rbStyleSetColValues;
		private System.Windows.Forms.RadioButton rbStyleFree;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btUpdate;
		private System.Windows.Forms.ListBox lbCompilerErrors;
		private System.Windows.Forms.Label lCodeTail;
		private System.Windows.Forms.Label lCodeStart;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox edRowInc;
		private System.Windows.Forms.TextBox edRowCondition;
		private System.Windows.Forms.TextBox edCodeHead;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SetColumnValuesDialog(Altaxo.Data.DataTable dataTable, Altaxo.Data.DataColumn dataColumn, ColumnScript _columnScript)
		{
			this.dataTable = dataTable;
			this.dataColumn = dataColumn;

			if(null!=_columnScript)
			{
				this.columnScript = (ColumnScript)_columnScript.Clone();
			}
			else
			{
				this.columnScript = new ColumnScript();
			}


			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			this.SuspendLayout();

			rbStyleSetColValues.Checked = columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			rbStyleSetCol.Checked = columnScript.Style==ColumnScript.ScriptStyle.SetColumn;
			rbStyleFree.Checked = columnScript.Style==ColumnScript.ScriptStyle.FreeStyle;

			if(null==columnScript.ForFrom)
				columnScript.ForFrom = "0";
			if(null==columnScript.ForCondition)
				columnScript.ForCondition = "<";
			if(null==columnScript.ForEnd)
				columnScript.ForEnd = "col.RowCount";
			if(null==columnScript.ForInc)
				columnScript.ForInc = "++";
			if(null==columnScript.ScriptBody)
				columnScript.ScriptBody="";

			this.edRowFrom.Enabled =  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			this.edRowCondition.Enabled =  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			this.edRowTo.Enabled =  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			this.edRowInc.Enabled =  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;

			edRowFrom.Text = columnScript.ForFrom;
			edRowCondition.Text = columnScript.ForCondition;
			edRowTo.Text = columnScript.ForEnd;
			edRowInc.Text = columnScript.ForInc;
			edFormula.Text=columnScript.ScriptBody;

			SetCodeParts();

			ResumeLayout();

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
			this.edRowFrom = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.edRowTo = new System.Windows.Forms.TextBox();
			this.edFormula = new System.Windows.Forms.TextBox();
			this.btDoIt = new System.Windows.Forms.Button();
			this.grpStyle = new System.Windows.Forms.GroupBox();
			this.rbStyleFree = new System.Windows.Forms.RadioButton();
			this.rbStyleSetColValues = new System.Windows.Forms.RadioButton();
			this.rbStyleSetCol = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.btUpdate = new System.Windows.Forms.Button();
			this.lbCompilerErrors = new System.Windows.Forms.ListBox();
			this.lCodeTail = new System.Windows.Forms.Label();
			this.lCodeStart = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.edRowInc = new System.Windows.Forms.TextBox();
			this.edRowCondition = new System.Windows.Forms.TextBox();
			this.edCodeHead = new System.Windows.Forms.TextBox();
			this.grpStyle.SuspendLayout();
			this.SuspendLayout();
			// 
			// edRowFrom
			// 
			this.edRowFrom.Location = new System.Drawing.Point(192, 16);
			this.edRowFrom.Name = "edRowFrom";
			this.edRowFrom.Size = new System.Drawing.Size(64, 20);
			this.edRowFrom.TabIndex = 1;
			this.edRowFrom.Text = "0";
			this.edRowFrom.TextChanged += new System.EventHandler(this.OnTextChanged_RowFrom);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(264, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "; i";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// edRowTo
			// 
			this.edRowTo.Location = new System.Drawing.Point(304, 16);
			this.edRowTo.Name = "edRowTo";
			this.edRowTo.Size = new System.Drawing.Size(112, 20);
			this.edRowTo.TabIndex = 3;
			this.edRowTo.Tag = "nRowTo";
			this.edRowTo.Text = "0";
			this.edRowTo.TextChanged += new System.EventHandler(this.OnTextChanged_RowTo);
			// 
			// edFormula
			// 
			this.edFormula.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.edFormula.Location = new System.Drawing.Point(8, 128);
			this.edFormula.Multiline = true;
			this.edFormula.Name = "edFormula";
			this.edFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.edFormula.Size = new System.Drawing.Size(512, 128);
			this.edFormula.TabIndex = 5;
			this.edFormula.Text = "";
			// 
			// btDoIt
			// 
			this.btDoIt.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btDoIt.Location = new System.Drawing.Point(528, 8);
			this.btDoIt.Name = "btDoIt";
			this.btDoIt.Size = new System.Drawing.Size(56, 32);
			this.btDoIt.TabIndex = 6;
			this.btDoIt.Text = "Do It!";
			this.btDoIt.Click += new System.EventHandler(this.btDoIt_Click);
			// 
			// grpStyle
			// 
			this.grpStyle.Controls.AddRange(new System.Windows.Forms.Control[] {
																																					 this.rbStyleFree,
																																					 this.rbStyleSetColValues,
																																					 this.rbStyleSetCol});
			this.grpStyle.Name = "grpStyle";
			this.grpStyle.Size = new System.Drawing.Size(144, 88);
			this.grpStyle.TabIndex = 7;
			this.grpStyle.TabStop = false;
			this.grpStyle.Text = "Style";
			// 
			// rbStyleFree
			// 
			this.rbStyleFree.Location = new System.Drawing.Point(16, 64);
			this.rbStyleFree.Name = "rbStyleFree";
			this.rbStyleFree.Size = new System.Drawing.Size(104, 16);
			this.rbStyleFree.TabIndex = 2;
			this.rbStyleFree.Text = "Free Style";
			this.rbStyleFree.CheckedChanged += new System.EventHandler(this.rbStyleFree_CheckedChanged);
			// 
			// rbStyleSetColValues
			// 
			this.rbStyleSetColValues.Location = new System.Drawing.Point(16, 16);
			this.rbStyleSetColValues.Name = "rbStyleSetColValues";
			this.rbStyleSetColValues.Size = new System.Drawing.Size(120, 16);
			this.rbStyleSetColValues.TabIndex = 1;
			this.rbStyleSetColValues.Text = "Set Column Values";
			this.rbStyleSetColValues.CheckedChanged += new System.EventHandler(this.rbStyleSetColValues_CheckedChanged);
			// 
			// rbStyleSetCol
			// 
			this.rbStyleSetCol.Location = new System.Drawing.Point(16, 40);
			this.rbStyleSetCol.Name = "rbStyleSetCol";
			this.rbStyleSetCol.Size = new System.Drawing.Size(104, 16);
			this.rbStyleSetCol.TabIndex = 0;
			this.rbStyleSetCol.Text = "Set Column";
			this.rbStyleSetCol.CheckedChanged += new System.EventHandler(this.rbStyleSetCol_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "for i=";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// btCancel
			// 
			this.btCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btCancel.Location = new System.Drawing.Point(528, 64);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(56, 32);
			this.btCancel.TabIndex = 9;
			this.btCancel.Text = "Cancel";
			// 
			// btUpdate
			// 
			this.btUpdate.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btUpdate.Location = new System.Drawing.Point(528, 128);
			this.btUpdate.Name = "btUpdate";
			this.btUpdate.Size = new System.Drawing.Size(56, 32);
			this.btUpdate.TabIndex = 10;
			this.btUpdate.Text = "Update";
			// 
			// lbCompilerErrors
			// 
			this.lbCompilerErrors.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lbCompilerErrors.HorizontalExtent = 4096;
			this.lbCompilerErrors.HorizontalScrollbar = true;
			this.lbCompilerErrors.Location = new System.Drawing.Point(8, 312);
			this.lbCompilerErrors.Name = "lbCompilerErrors";
			this.lbCompilerErrors.Size = new System.Drawing.Size(576, 82);
			this.lbCompilerErrors.TabIndex = 11;
			// 
			// lCodeTail
			// 
			this.lCodeTail.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lCodeTail.Location = new System.Drawing.Point(16, 264);
			this.lCodeTail.Name = "lCodeTail";
			this.lCodeTail.Size = new System.Drawing.Size(504, 40);
			this.lCodeTail.TabIndex = 12;
			// 
			// lCodeStart
			// 
			this.lCodeStart.Location = new System.Drawing.Point(8, 104);
			this.lCodeStart.Name = "lCodeStart";
			this.lCodeStart.Size = new System.Drawing.Size(136, 24);
			this.lCodeStart.TabIndex = 14;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(416, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 16);
			this.label3.TabIndex = 15;
			this.label3.Text = "; i";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// edRowInc
			// 
			this.edRowInc.Location = new System.Drawing.Point(432, 16);
			this.edRowInc.Name = "edRowInc";
			this.edRowInc.Size = new System.Drawing.Size(56, 20);
			this.edRowInc.TabIndex = 16;
			this.edRowInc.Text = "++";
			this.edRowInc.TextChanged += new System.EventHandler(this.OnTextChanged_RowInc);
			// 
			// edRowCondition
			// 
			this.edRowCondition.Location = new System.Drawing.Point(280, 16);
			this.edRowCondition.Name = "edRowCondition";
			this.edRowCondition.Size = new System.Drawing.Size(24, 20);
			this.edRowCondition.TabIndex = 17;
			this.edRowCondition.Text = "<";
			this.edRowCondition.TextChanged += new System.EventHandler(this.edRowCondition_TextChanged);
			// 
			// edCodeHead
			// 
			this.edCodeHead.AcceptsReturn = true;
			this.edCodeHead.AcceptsTab = true;
			this.edCodeHead.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.edCodeHead.Location = new System.Drawing.Point(144, 40);
			this.edCodeHead.Multiline = true;
			this.edCodeHead.Name = "edCodeHead";
			this.edCodeHead.ReadOnly = true;
			this.edCodeHead.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.edCodeHead.Size = new System.Drawing.Size(376, 80);
			this.edCodeHead.TabIndex = 19;
			this.edCodeHead.Text = "";
			this.edCodeHead.WordWrap = false;
			// 
			// SetColumnValuesDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 398);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.edCodeHead,
																																	this.edRowCondition,
																																	this.edRowInc,
																																	this.label3,
																																	this.lCodeStart,
																																	this.lCodeTail,
																																	this.lbCompilerErrors,
																																	this.btUpdate,
																																	this.btCancel,
																																	this.label2,
																																	this.grpStyle,
																																	this.btDoIt,
																																	this.edFormula,
																																	this.edRowTo,
																																	this.label1,
																																	this.edRowFrom});
			this.Name = "SetColumnValuesDialog";
			this.Text = "WksSetColumnValuesDialog";
			this.grpStyle.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion





		private void btDoIt_Click(object sender, System.EventArgs e)
		{
			columnScript.ScriptBody = this.edFormula.Text;

			lbCompilerErrors.Items.Clear();

			bool bSucceeded = columnScript.Compile();

			if(!bSucceeded)
			{
				foreach(string s in columnScript.Errors)
					this.lbCompilerErrors.Items.Add(s);

				System.Windows.Forms.MessageBox.Show(this, "There were compilation errors","No success");
				return;
			}

			bSucceeded = columnScript.ExecuteWithSuspendedNotifications(dataColumn);
			if(!bSucceeded)
			{
				foreach(string s in columnScript.Errors)
					this.lbCompilerErrors.Items.Add(s);

				System.Windows.Forms.MessageBox.Show(this, "There were execution errors","No success");
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void SetCodeParts()
		{
			this.edCodeHead.Text = columnScript.CodeHeader;
			this.lCodeStart.Text= columnScript.CodeStart;
			this.lCodeTail.Text = columnScript.CodeTail;
		}



		void ReadStyleFromButtons()
		{
			if(this.rbStyleSetColValues.Checked) 
			{
				this.columnScript.Style=ColumnScript.ScriptStyle.SetColumnValues;
			}
			else if(this.rbStyleSetCol.Checked)
			{
				this.columnScript.Style=ColumnScript.ScriptStyle.SetColumn;
			}
			else 
			{
				this.columnScript.Style=ColumnScript.ScriptStyle.FreeStyle;
			}
			edRowFrom.Enabled = columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			edRowCondition.Enabled = columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			edRowTo.Enabled = columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;
			edRowInc.Enabled = columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues;

			}


		private void rbStyleSetColValues_CheckedChanged(object sender, System.EventArgs e)
		{
			ReadStyleFromButtons();
			SetCodeParts();
		}

		private void rbStyleSetCol_CheckedChanged(object sender, System.EventArgs e)
		{
			ReadStyleFromButtons();
			SetCodeParts();
		}

		private void rbStyleFree_CheckedChanged(object sender, System.EventArgs e)
		{
			ReadStyleFromButtons();
			SetCodeParts();
		}

		private void OnTextChanged_RowFrom(object sender, System.EventArgs e)
		{
		columnScript.ForFrom = this.edRowFrom.Text;
		SetCodeParts();
		}

		private void OnTextChanged_RowTo(object sender, System.EventArgs e)
		{
			columnScript.ForEnd = this.edRowTo.Text;
			SetCodeParts();
		}

		private void OnTextChanged_RowInc(object sender, System.EventArgs e)
		{
			columnScript.ForInc = this.edRowInc.Text;
			SetCodeParts();
		}

		private void edRowCondition_TextChanged(object sender, System.EventArgs e)
		{
			columnScript.ForCondition = this.edRowCondition.Text;
			SetCodeParts();
		}
	}
}
