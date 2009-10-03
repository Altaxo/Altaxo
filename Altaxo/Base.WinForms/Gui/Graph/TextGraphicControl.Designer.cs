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

namespace Altaxo.Gui.Graph
{
  partial class TextGraphicControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextGraphicControl));
      this._btStrikeout = new System.Windows.Forms.Button();
      this._btNormal = new System.Windows.Forms.Button();
      this.m_pnPreview = new System.Windows.Forms.Panel();
      this.m_edText = new System.Windows.Forms.TextBox();
      this.m_edPosY = new System.Windows.Forms.TextBox();
      this.m_edPosX = new System.Windows.Forms.TextBox();
      this._btGreek = new System.Windows.Forms.Button();
      this._btSubIndex = new System.Windows.Forms.Button();
      this._btSupIndex = new System.Windows.Forms.Button();
      this._btUnderline = new System.Windows.Forms.Button();
      this._btItalic = new System.Windows.Forms.Button();
      this._btBold = new System.Windows.Forms.Button();
      this.m_lblRotation = new System.Windows.Forms.Label();
      this.m_lblPosY = new System.Windows.Forms.Label();
      this.m_lblPosX = new System.Windows.Forms.Label();
      this._cbBackgroundStyle = new System.Windows.Forms.ComboBox();
      this.m_cbFontColor = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this._cbBackgroundBrush = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this.m_cbRotation = new Altaxo.Gui.Common.Drawing.RotationComboBox();
      this.m_cbFonts = new Altaxo.Gui.Common.Drawing.FontFamilyComboBox();
      this._positionSizeGlue = new Altaxo.Gui.Graph.ObjectPositionAndSizeGlue();
      this._backgroundGlue = new Altaxo.Gui.Graph.BackgroundControlsGlue();
      this.m_cbFontSize = new Altaxo.Gui.Common.Drawing.FontSizeComboBox();
      this.SuspendLayout();
      // 
      // _btStrikeout
      // 
      this._btStrikeout.Image = ((System.Drawing.Image)(resources.GetObject("_btStrikeout.Image")));
      this._btStrikeout.Location = new System.Drawing.Point(137, 113);
      this._btStrikeout.Name = "_btStrikeout";
      this._btStrikeout.Size = new System.Drawing.Size(32, 24);
      this._btStrikeout.TabIndex = 38;
      this._btStrikeout.Click += new System.EventHandler(this.EhStrikeout_Click);
      // 
      // _btNormal
      // 
      this._btNormal.Image = ((System.Drawing.Image)(resources.GetObject("_btNormal.Image")));
      this._btNormal.Location = new System.Drawing.Point(9, 113);
      this._btNormal.Name = "_btNormal";
      this._btNormal.Size = new System.Drawing.Size(32, 24);
      this._btNormal.TabIndex = 34;
      this._btNormal.Click += new System.EventHandler(this.EhNormal_Click);
      // 
      // m_pnPreview
      // 
      this.m_pnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_pnPreview.Location = new System.Drawing.Point(9, 241);
      this.m_pnPreview.Name = "m_pnPreview";
      this.m_pnPreview.Size = new System.Drawing.Size(354, 85);
      this.m_pnPreview.TabIndex = 42;
      this.m_pnPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.EhPreviewPanel_Paint);
      // 
      // m_edText
      // 
      this.m_edText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_edText.Location = new System.Drawing.Point(9, 145);
      this.m_edText.Multiline = true;
      this.m_edText.Name = "m_edText";
      this.m_edText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.m_edText.Size = new System.Drawing.Size(354, 80);
      this.m_edText.TabIndex = 25;
      this.m_edText.Text = "textBox1";
      this.m_edText.TextChanged += new System.EventHandler(this.EhEditText_TextChanged);
      // 
      // m_edPosY
      // 
      this.m_edPosY.Location = new System.Drawing.Point(169, 73);
      this.m_edPosY.Name = "m_edPosY";
      this.m_edPosY.Size = new System.Drawing.Size(80, 20);
      this.m_edPosY.TabIndex = 31;
      this.m_edPosY.Text = "0 pt";
      // 
      // m_edPosX
      // 
      this.m_edPosX.Location = new System.Drawing.Point(49, 73);
      this.m_edPosX.Name = "m_edPosX";
      this.m_edPosX.Size = new System.Drawing.Size(72, 20);
      this.m_edPosX.TabIndex = 29;
      this.m_edPosX.Text = "0 pt";
      // 
      // _btGreek
      // 
      this._btGreek.Font = new System.Drawing.Font("Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._btGreek.Image = ((System.Drawing.Image)(resources.GetObject("_btGreek.Image")));
      this._btGreek.Location = new System.Drawing.Point(233, 113);
      this._btGreek.Name = "_btGreek";
      this._btGreek.Size = new System.Drawing.Size(32, 24);
      this._btGreek.TabIndex = 41;
      this._btGreek.Click += new System.EventHandler(this.EhGreek_Click);
      // 
      // _btSubIndex
      // 
      this._btSubIndex.Image = ((System.Drawing.Image)(resources.GetObject("_btSubIndex.Image")));
      this._btSubIndex.Location = new System.Drawing.Point(201, 113);
      this._btSubIndex.Name = "_btSubIndex";
      this._btSubIndex.Size = new System.Drawing.Size(32, 24);
      this._btSubIndex.TabIndex = 40;
      this._btSubIndex.Click += new System.EventHandler(this.EhSubIndex_Click);
      // 
      // _btSupIndex
      // 
      this._btSupIndex.Image = ((System.Drawing.Image)(resources.GetObject("_btSupIndex.Image")));
      this._btSupIndex.Location = new System.Drawing.Point(169, 113);
      this._btSupIndex.Name = "_btSupIndex";
      this._btSupIndex.Size = new System.Drawing.Size(32, 24);
      this._btSupIndex.TabIndex = 39;
      this._btSupIndex.Click += new System.EventHandler(this.EhSupIndex_Click);
      // 
      // _btUnderline
      // 
      this._btUnderline.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._btUnderline.Image = ((System.Drawing.Image)(resources.GetObject("_btUnderline.Image")));
      this._btUnderline.Location = new System.Drawing.Point(105, 113);
      this._btUnderline.Name = "_btUnderline";
      this._btUnderline.Size = new System.Drawing.Size(32, 24);
      this._btUnderline.TabIndex = 37;
      this._btUnderline.Click += new System.EventHandler(this.EhUnderline_Click);
      // 
      // _btItalic
      // 
      this._btItalic.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._btItalic.Image = ((System.Drawing.Image)(resources.GetObject("_btItalic.Image")));
      this._btItalic.Location = new System.Drawing.Point(73, 113);
      this._btItalic.Name = "_btItalic";
      this._btItalic.Size = new System.Drawing.Size(32, 24);
      this._btItalic.TabIndex = 36;
      this._btItalic.Click += new System.EventHandler(this.EhItalic_Click);
      // 
      // _btBold
      // 
      this._btBold.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._btBold.Image = ((System.Drawing.Image)(resources.GetObject("_btBold.Image")));
      this._btBold.Location = new System.Drawing.Point(41, 113);
      this._btBold.Name = "_btBold";
      this._btBold.Size = new System.Drawing.Size(32, 24);
      this._btBold.TabIndex = 35;
      this._btBold.Click += new System.EventHandler(this.EhBold_Click);
      // 
      // m_lblRotation
      // 
      this.m_lblRotation.Location = new System.Drawing.Point(257, 73);
      this.m_lblRotation.Name = "m_lblRotation";
      this.m_lblRotation.Size = new System.Drawing.Size(32, 21);
      this.m_lblRotation.TabIndex = 32;
      this.m_lblRotation.Text = "Rot.";
      this.m_lblRotation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_lblPosY
      // 
      this.m_lblPosY.Location = new System.Drawing.Point(121, 73);
      this.m_lblPosY.Name = "m_lblPosY";
      this.m_lblPosY.Size = new System.Drawing.Size(40, 16);
      this.m_lblPosY.TabIndex = 30;
      this.m_lblPosY.Text = "Pos.Y";
      this.m_lblPosY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_lblPosX
      // 
      this.m_lblPosX.Location = new System.Drawing.Point(9, 73);
      this.m_lblPosX.Name = "m_lblPosX";
      this.m_lblPosX.Size = new System.Drawing.Size(40, 16);
      this.m_lblPosX.TabIndex = 28;
      this.m_lblPosX.Text = "Pos.X";
      this.m_lblPosX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // _cbBackgroundStyle
      // 
      this._cbBackgroundStyle.FormattingEnabled = true;
      this._cbBackgroundStyle.Location = new System.Drawing.Point(105, 36);
      this._cbBackgroundStyle.Name = "_cbBackgroundStyle";
      this._cbBackgroundStyle.Size = new System.Drawing.Size(121, 21);
      this._cbBackgroundStyle.TabIndex = 47;
      // 
      // m_cbFontColor
      // 
      this.m_cbFontColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this.m_cbFontColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbFontColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbFontColor.FormattingEnabled = true;
      this.m_cbFontColor.ItemHeight = 15;
      this.m_cbFontColor.Location = new System.Drawing.Point(233, 9);
      this.m_cbFontColor.Name = "m_cbFontColor";
      this.m_cbFontColor.Size = new System.Drawing.Size(130, 21);
      this.m_cbFontColor.TabIndex = 49;
      this.m_cbFontColor.SelectionChangeCommitted += new System.EventHandler(this.EhTextBrush_SelectionChangeCommitted);
      // 
      // _cbBackgroundBrush
      // 
      this._cbBackgroundBrush.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbBackgroundBrush.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbBackgroundBrush.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbBackgroundBrush.FormattingEnabled = true;
      this._cbBackgroundBrush.ItemHeight = 15;
      this._cbBackgroundBrush.Location = new System.Drawing.Point(233, 36);
      this._cbBackgroundBrush.Name = "_cbBackgroundBrush";
      this._cbBackgroundBrush.Size = new System.Drawing.Size(130, 21);
      this._cbBackgroundBrush.TabIndex = 46;
      // 
      // m_cbRotation
      // 
      this.m_cbRotation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbRotation.FormattingEnabled = true;
      this.m_cbRotation.ItemHeight = 13;
      this.m_cbRotation.Location = new System.Drawing.Point(295, 73);
      this.m_cbRotation.Name = "m_cbRotation";
      this.m_cbRotation.Size = new System.Drawing.Size(68, 19);
      this.m_cbRotation.TabIndex = 45;
      // 
      // m_cbFonts
      // 
      this.m_cbFonts.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbFonts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbFonts.ItemHeight = 13;
      this.m_cbFonts.Location = new System.Drawing.Point(9, 9);
      this.m_cbFonts.Name = "m_cbFonts";
      this.m_cbFonts.Size = new System.Drawing.Size(128, 19);
      this.m_cbFonts.TabIndex = 26;
      this.m_cbFonts.SelectionChangeCommitted += new System.EventHandler(this.EhFontFamilyChanged);
      // 
      // _positionSizeGlue
      // 
      this._positionSizeGlue.CbRotation = null;
      this._positionSizeGlue.EdPositionX = this.m_edPosX;
      this._positionSizeGlue.EdPositionY = this.m_edPosY;
      this._positionSizeGlue.EdSizeX = null;
      this._positionSizeGlue.EdSizeY = null;
      // 
      // _backgroundGlue
      // 
      this._backgroundGlue.CbBrush = this._cbBackgroundBrush;
      this._backgroundGlue.CbStyle = this._cbBackgroundStyle;
      this._backgroundGlue.LabelBrush = null;
      this._backgroundGlue.BackgroundStyleChanged += new System.EventHandler(this.EhBackgroundStyle_Changed);
      // 
      // m_cbFontSize
      // 
      this.m_cbFontSize.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbFontSize.FormattingEnabled = true;
      this.m_cbFontSize.ItemHeight = 15;
      this.m_cbFontSize.Location = new System.Drawing.Point(143, 9);
      this.m_cbFontSize.Name = "m_cbFontSize";
      this.m_cbFontSize.Size = new System.Drawing.Size(83, 21);
      this.m_cbFontSize.TabIndex = 50;
      this.m_cbFontSize.FontSizeChanged += new System.EventHandler(this.EhFontSize_Changed);
      // 
      // TextGraphicControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_cbFontSize);
      this.Controls.Add(this.m_cbFontColor);
      this.Controls.Add(this._cbBackgroundStyle);
      this.Controls.Add(this._cbBackgroundBrush);
      this.Controls.Add(this.m_cbRotation);
      this.Controls.Add(this._btStrikeout);
      this.Controls.Add(this._btNormal);
      this.Controls.Add(this.m_pnPreview);
      this.Controls.Add(this.m_edText);
      this.Controls.Add(this.m_edPosY);
      this.Controls.Add(this.m_edPosX);
      this.Controls.Add(this._btGreek);
      this.Controls.Add(this._btSubIndex);
      this.Controls.Add(this._btSupIndex);
      this.Controls.Add(this._btUnderline);
      this.Controls.Add(this._btItalic);
      this.Controls.Add(this._btBold);
      this.Controls.Add(this.m_lblRotation);
      this.Controls.Add(this.m_lblPosY);
      this.Controls.Add(this.m_lblPosX);
      this.Controls.Add(this.m_cbFonts);
      this.Name = "TextGraphicControl";
      this.Size = new System.Drawing.Size(370, 333);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _btStrikeout;
    private System.Windows.Forms.Button _btNormal;
    private System.Windows.Forms.Panel m_pnPreview;
    private System.Windows.Forms.TextBox m_edText;
    private System.Windows.Forms.TextBox m_edPosY;
    private System.Windows.Forms.TextBox m_edPosX;
    private System.Windows.Forms.Button _btGreek;
    private System.Windows.Forms.Button _btSubIndex;
    private System.Windows.Forms.Button _btSupIndex;
    private System.Windows.Forms.Button _btUnderline;
    private System.Windows.Forms.Button _btItalic;
    private System.Windows.Forms.Button _btBold;
    private System.Windows.Forms.Label m_lblRotation;
    private System.Windows.Forms.Label m_lblPosY;
    private System.Windows.Forms.Label m_lblPosX;
    private Altaxo.Gui.Common.Drawing.FontFamilyComboBox m_cbFonts;
    private Altaxo.Gui.Common.Drawing.RotationComboBox m_cbRotation;
    private Altaxo.Gui.Common.Drawing.BrushColorComboBox _cbBackgroundBrush;
    private ObjectPositionAndSizeGlue _positionSizeGlue;
    private BackgroundControlsGlue _backgroundGlue;
    private System.Windows.Forms.ComboBox _cbBackgroundStyle;
    private Altaxo.Gui.Common.Drawing.BrushColorComboBox m_cbFontColor;
    private Altaxo.Gui.Common.Drawing.FontSizeComboBox m_cbFontSize;
  }
}
