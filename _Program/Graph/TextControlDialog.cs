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
	/// Summary description for TextControlDialog.
	/// </summary>
	public class TextControlDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button m_btOK;
		private System.Windows.Forms.Button m_btCancel;
		private System.Windows.Forms.ComboBox m_cbFonts;
		private System.Windows.Forms.ComboBox m_cbFontSize;
		private System.Windows.Forms.ComboBox m_cbFontColor;
		private System.Windows.Forms.Label m_lblBackground;
		private System.Windows.Forms.ComboBox m_cbBackground;
		private System.Windows.Forms.Label m_lblPosX;
		private System.Windows.Forms.TextBox m_edPosX;
		private System.Windows.Forms.Label m_lblPosY;
		private System.Windows.Forms.TextBox m_edPosY;
		private System.Windows.Forms.Label m_lblRotation;
		private System.Windows.Forms.ComboBox m_cbRotation;
		private System.Windows.Forms.Button m_btBold;
		private System.Windows.Forms.Button m_btItalic;
		private System.Windows.Forms.Button m_btUnderline;
		private System.Windows.Forms.Button m_btSupIndex;
		private System.Windows.Forms.Button m_btSubIndex;
		private System.Windows.Forms.Button m_btGreek;
		private System.Windows.Forms.TextBox m_edText;
		private System.Windows.Forms.Panel m_pnPreview;
		private Altaxo.Graph.Layer m_Layer; // parent layer
		private ExtendedTextGraphObject m_TextObject;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TextControlDialog(Altaxo.Graph.Layer layer, ExtendedTextGraphObject tgo)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			m_Layer = layer;

			if(null==tgo)
				m_TextObject = new ExtendedTextGraphObject();
			else
				m_TextObject = tgo;

			FillDialogElements();
		}



		public void FillDialogElements()
		{
			this.m_edText.Text = m_TextObject.Text;
			this.m_edPosX.Text = m_TextObject.Position.X.ToString();
			this.m_edPosY.Text = m_TextObject.Position.Y.ToString();
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
			this.m_btOK = new System.Windows.Forms.Button();
			this.m_btCancel = new System.Windows.Forms.Button();
			this.m_cbFonts = new System.Windows.Forms.ComboBox();
			this.m_cbFontSize = new System.Windows.Forms.ComboBox();
			this.m_cbFontColor = new System.Windows.Forms.ComboBox();
			this.m_lblBackground = new System.Windows.Forms.Label();
			this.m_cbBackground = new System.Windows.Forms.ComboBox();
			this.m_lblPosX = new System.Windows.Forms.Label();
			this.m_edPosX = new System.Windows.Forms.TextBox();
			this.m_lblPosY = new System.Windows.Forms.Label();
			this.m_edPosY = new System.Windows.Forms.TextBox();
			this.m_lblRotation = new System.Windows.Forms.Label();
			this.m_cbRotation = new System.Windows.Forms.ComboBox();
			this.m_btBold = new System.Windows.Forms.Button();
			this.m_btItalic = new System.Windows.Forms.Button();
			this.m_btUnderline = new System.Windows.Forms.Button();
			this.m_btSupIndex = new System.Windows.Forms.Button();
			this.m_btSubIndex = new System.Windows.Forms.Button();
			this.m_btGreek = new System.Windows.Forms.Button();
			this.m_edText = new System.Windows.Forms.TextBox();
			this.m_pnPreview = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// m_btOK
			// 
			this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btOK.Location = new System.Drawing.Point(368, 8);
			this.m_btOK.Name = "m_btOK";
			this.m_btOK.Size = new System.Drawing.Size(48, 24);
			this.m_btOK.TabIndex = 0;
			this.m_btOK.Text = "OK";
			// 
			// m_btCancel
			// 
			this.m_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btCancel.Location = new System.Drawing.Point(368, 40);
			this.m_btCancel.Name = "m_btCancel";
			this.m_btCancel.Size = new System.Drawing.Size(48, 24);
			this.m_btCancel.TabIndex = 1;
			this.m_btCancel.Text = "Cancel";
			// 
			// m_cbFonts
			// 
			this.m_cbFonts.Location = new System.Drawing.Point(8, 8);
			this.m_cbFonts.Name = "m_cbFonts";
			this.m_cbFonts.Size = new System.Drawing.Size(160, 21);
			this.m_cbFonts.TabIndex = 2;
			this.m_cbFonts.Text = "comboBox1";
			// 
			// m_cbFontSize
			// 
			this.m_cbFontSize.Location = new System.Drawing.Point(184, 8);
			this.m_cbFontSize.Name = "m_cbFontSize";
			this.m_cbFontSize.Size = new System.Drawing.Size(64, 21);
			this.m_cbFontSize.TabIndex = 3;
			this.m_cbFontSize.Text = "comboBox1";
			// 
			// m_cbFontColor
			// 
			this.m_cbFontColor.Location = new System.Drawing.Point(264, 8);
			this.m_cbFontColor.Name = "m_cbFontColor";
			this.m_cbFontColor.Size = new System.Drawing.Size(88, 21);
			this.m_cbFontColor.TabIndex = 4;
			this.m_cbFontColor.Text = "comboBox1";
			// 
			// m_lblBackground
			// 
			this.m_lblBackground.Location = new System.Drawing.Point(176, 40);
			this.m_lblBackground.Name = "m_lblBackground";
			this.m_lblBackground.Size = new System.Drawing.Size(72, 16);
			this.m_lblBackground.TabIndex = 5;
			this.m_lblBackground.Text = "Background";
			// 
			// m_cbBackground
			// 
			this.m_cbBackground.Location = new System.Drawing.Point(264, 40);
			this.m_cbBackground.Name = "m_cbBackground";
			this.m_cbBackground.Size = new System.Drawing.Size(88, 21);
			this.m_cbBackground.TabIndex = 6;
			this.m_cbBackground.Text = "comboBox1";
			// 
			// m_lblPosX
			// 
			this.m_lblPosX.Location = new System.Drawing.Point(8, 72);
			this.m_lblPosX.Name = "m_lblPosX";
			this.m_lblPosX.Size = new System.Drawing.Size(40, 16);
			this.m_lblPosX.TabIndex = 7;
			this.m_lblPosX.Text = "Pos.X";
			this.m_lblPosX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_edPosX
			// 
			this.m_edPosX.Location = new System.Drawing.Point(48, 72);
			this.m_edPosX.Name = "m_edPosX";
			this.m_edPosX.Size = new System.Drawing.Size(56, 20);
			this.m_edPosX.TabIndex = 8;
			this.m_edPosX.Text = "textBox1";
			// 
			// m_lblPosY
			// 
			this.m_lblPosY.Location = new System.Drawing.Point(128, 72);
			this.m_lblPosY.Name = "m_lblPosY";
			this.m_lblPosY.Size = new System.Drawing.Size(40, 16);
			this.m_lblPosY.TabIndex = 9;
			this.m_lblPosY.Text = "Pos.Y";
			this.m_lblPosY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_edPosY
			// 
			this.m_edPosY.Location = new System.Drawing.Point(184, 72);
			this.m_edPosY.Name = "m_edPosY";
			this.m_edPosY.Size = new System.Drawing.Size(56, 20);
			this.m_edPosY.TabIndex = 10;
			this.m_edPosY.Text = "textBox1";
			// 
			// m_lblRotation
			// 
			this.m_lblRotation.Location = new System.Drawing.Point(256, 72);
			this.m_lblRotation.Name = "m_lblRotation";
			this.m_lblRotation.Size = new System.Drawing.Size(32, 21);
			this.m_lblRotation.TabIndex = 11;
			this.m_lblRotation.Text = "Rot.";
			this.m_lblRotation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_cbRotation
			// 
			this.m_cbRotation.Location = new System.Drawing.Point(296, 72);
			this.m_cbRotation.Name = "m_cbRotation";
			this.m_cbRotation.Size = new System.Drawing.Size(56, 21);
			this.m_cbRotation.TabIndex = 12;
			this.m_cbRotation.Text = "comboBox1";
			// 
			// m_btBold
			// 
			this.m_btBold.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_btBold.Location = new System.Drawing.Point(8, 112);
			this.m_btBold.Name = "m_btBold";
			this.m_btBold.Size = new System.Drawing.Size(32, 24);
			this.m_btBold.TabIndex = 13;
			this.m_btBold.Text = "B";
			// 
			// m_btItalic
			// 
			this.m_btItalic.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_btItalic.Location = new System.Drawing.Point(48, 112);
			this.m_btItalic.Name = "m_btItalic";
			this.m_btItalic.Size = new System.Drawing.Size(32, 24);
			this.m_btItalic.TabIndex = 14;
			this.m_btItalic.Text = "I";
			// 
			// m_btUnderline
			// 
			this.m_btUnderline.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_btUnderline.Location = new System.Drawing.Point(88, 112);
			this.m_btUnderline.Name = "m_btUnderline";
			this.m_btUnderline.Size = new System.Drawing.Size(24, 24);
			this.m_btUnderline.TabIndex = 15;
			this.m_btUnderline.Text = "U";
			// 
			// m_btSupIndex
			// 
			this.m_btSupIndex.Location = new System.Drawing.Point(120, 112);
			this.m_btSupIndex.Name = "m_btSupIndex";
			this.m_btSupIndex.Size = new System.Drawing.Size(40, 24);
			this.m_btSupIndex.TabIndex = 16;
			this.m_btSupIndex.Text = "X²";
			// 
			// m_btSubIndex
			// 
			this.m_btSubIndex.Location = new System.Drawing.Point(168, 112);
			this.m_btSubIndex.Name = "m_btSubIndex";
			this.m_btSubIndex.Size = new System.Drawing.Size(40, 24);
			this.m_btSubIndex.TabIndex = 17;
			this.m_btSubIndex.Text = "X2";
			// 
			// m_btGreek
			// 
			this.m_btGreek.Font = new System.Drawing.Font("Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.m_btGreek.Location = new System.Drawing.Point(216, 112);
			this.m_btGreek.Name = "m_btGreek";
			this.m_btGreek.Size = new System.Drawing.Size(40, 24);
			this.m_btGreek.TabIndex = 18;
			this.m_btGreek.Text = "G";
			// 
			// m_edText
			// 
			this.m_edText.Location = new System.Drawing.Point(8, 144);
			this.m_edText.Multiline = true;
			this.m_edText.Name = "m_edText";
			this.m_edText.Size = new System.Drawing.Size(400, 80);
			this.m_edText.TabIndex = 19;
			this.m_edText.Text = "textBox1";
			this.m_edText.TextChanged += new System.EventHandler(this.OnEditText_TextChanged);
			// 
			// m_pnPreview
			// 
			this.m_pnPreview.Location = new System.Drawing.Point(8, 240);
			this.m_pnPreview.Name = "m_pnPreview";
			this.m_pnPreview.Size = new System.Drawing.Size(408, 96);
			this.m_pnPreview.TabIndex = 20;
			this.m_pnPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPanelPreview_Paint);
			// 
			// TextControlDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 342);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.m_pnPreview,
																																	this.m_edText,
																																	this.m_btGreek,
																																	this.m_btSubIndex,
																																	this.m_btSupIndex,
																																	this.m_btUnderline,
																																	this.m_btItalic,
																																	this.m_btBold,
																																	this.m_cbRotation,
																																	this.m_lblRotation,
																																	this.m_edPosY,
																																	this.m_lblPosY,
																																	this.m_edPosX,
																																	this.m_lblPosX,
																																	this.m_cbBackground,
																																	this.m_lblBackground,
																																	this.m_cbFontColor,
																																	this.m_cbFontSize,
																																	this.m_cbFonts,
																																	this.m_btCancel,
																																	this.m_btOK});
			this.Name = "TextControlDialog";
			this.Text = "Text Control";
			this.ResumeLayout(false);

		}
		#endregion

		private void OnPanelPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		Graphics g = e.Graphics;
		g.PageUnit = GraphicsUnit.Point;

		g.TranslateTransform(20,20);
		m_TextObject.Paint(g,m_Layer);
		}

		private void OnEditText_TextChanged(object sender, System.EventArgs e)
		{
			this.m_TextObject.Text = this.m_edText.Text;
			this.m_pnPreview.Invalidate();
		}


	}
}
