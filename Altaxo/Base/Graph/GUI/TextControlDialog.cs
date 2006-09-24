#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Gui.Common.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Shapes;



namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// Summary description for TextControlDialog.
  /// </summary>
  public class TextControlDialog : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Button m_btOK;
    private System.Windows.Forms.Button m_btCancel;
    private FontFamilyComboBox m_cbFonts;
    private System.Windows.Forms.ComboBox m_cbFontSize;
    private System.Windows.Forms.Label m_lblBackground;
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
    private XYPlotLayer m_Layer; // parent layer
    private TextGraphics m_TextObject;
    private float  m_PositionX; // original x position of textobject
    private float  m_PositionY; // original y position of textobject
    private float  m_Rotation; // original rotation of textobject
    private System.Windows.Forms.Button m_btNormal;
    private System.Windows.Forms.Button m_btStrikeout;
    private bool   m_bDialogInitialized=false;
    private Altaxo.Gui.Graph.BackgroundStyleControl _ctrlBackgroundStyle; // true if all dialog elements are initialized
    private Altaxo.Gui.Graph.BackgroundStyleController _backgroundStyleController;
    private Altaxo.Gui.Common.Drawing.ColorComboBox m_cbFontColor;
    private IContainer components;

    public TextControlDialog(XYPlotLayer layer, TextGraphics tgo)
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();


      m_Layer = layer;

      if(null==tgo)
        m_TextObject = new TextGraphics();
      else
        m_TextObject = tgo;


      FillDialogElements();

      // Focus the text box and select the text
      this.m_edText.Focus();
      this.m_edText.SelectAll();
    }


    public TextGraphics SimpleTextGraphics
    {
      get { return m_TextObject; }
    }

    public void FillDialogElements()
    {
      string name=null;
      _backgroundStyleController = new Altaxo.Gui.Graph.BackgroundStyleController(m_TextObject.Background);
      _backgroundStyleController.ViewObject = _ctrlBackgroundStyle;
      _ctrlBackgroundStyle.Controller = _backgroundStyleController;
      _backgroundStyleController.TemporaryModelObjectChanged += this.EhBackgroundChanged;

      this.m_edText.Text = m_TextObject.Text;

      
      // set some help values (rotation and position), since we have to change
      // them during painting of the SimpleTextGraphics
      m_PositionX = m_TextObject.Position.X;
      m_PositionY = m_TextObject.Position.Y;
      m_Rotation  = m_TextObject.Rotation;


      this.m_edPosX.Text = m_PositionX.ToString();
      this.m_edPosY.Text = m_PositionY.ToString();
    
    
      // fill the rotation combobox with some reasonable values
      this.m_cbRotation.Items.AddRange(new string[]{"0","45","90","135","180","225","270","315"});
      this.m_cbRotation.Text = m_TextObject.Rotation.ToString();

    
      // fill the font name combobox with all fonts
     
      this.m_cbFonts.FontFamilyDocument = m_TextObject.Font.FontFamily;

      // fill the font size combobox with reasonable values
      this.m_cbFontSize.Items.AddRange(new string[]{"8","9","10","11","12","14","16","18","20","22","24","26","28","36","48","72"});
      this.m_cbFontSize.Text = m_TextObject.Font.Size.ToString();


      // fill the color dialog box
      this.m_cbFontColor.Color = this.m_TextObject.Color;
    
    
      // indicate that all elements are now filled -
      // the following changed are due to the user
      this.m_bDialogInitialized = true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (_backgroundStyleController.Apply())
        m_TextObject.Background = (IBackgroundStyle)_backgroundStyleController.ModelObject;
      else
        e.Cancel = true;
      base.OnClosing(e);
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextControlDialog));
      this.m_btOK = new System.Windows.Forms.Button();
      this.m_btCancel = new System.Windows.Forms.Button();
      this.m_cbFonts = new FontFamilyComboBox();
      this.m_cbFontSize = new System.Windows.Forms.ComboBox();
      this.m_lblBackground = new System.Windows.Forms.Label();
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
      this.m_btNormal = new System.Windows.Forms.Button();
      this.m_btStrikeout = new System.Windows.Forms.Button();
      this.m_cbFontColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._ctrlBackgroundStyle = new Altaxo.Gui.Graph.BackgroundStyleControl();
      this.SuspendLayout();
      // 
      // m_btOK
      // 
      this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_btOK.Location = new System.Drawing.Point(368, 8);
      this.m_btOK.Name = "m_btOK";
      this.m_btOK.Size = new System.Drawing.Size(48, 24);
      this.m_btOK.TabIndex = 12;
      this.m_btOK.Text = "OK";
      // 
      // m_btCancel
      // 
      this.m_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_btCancel.Location = new System.Drawing.Point(368, 40);
      this.m_btCancel.Name = "m_btCancel";
      this.m_btCancel.Size = new System.Drawing.Size(48, 24);
      this.m_btCancel.TabIndex = 13;
      this.m_btCancel.Text = "Cancel";
      // 
      // m_cbFonts
      // 
      this.m_cbFonts.Location = new System.Drawing.Point(8, 8);
      this.m_cbFonts.Name = "m_cbFonts";
      this.m_cbFonts.Size = new System.Drawing.Size(128, 21);
      this.m_cbFonts.TabIndex = 1;
      this.m_cbFonts.Text = "Arial";
      this.m_cbFonts.SelectionChangeCommitted += new System.EventHandler(this.OncbFonts_TextChanged);
      // 
      // m_cbFontSize
      // 
      this.m_cbFontSize.Location = new System.Drawing.Point(142, 8);
      this.m_cbFontSize.Name = "m_cbFontSize";
      this.m_cbFontSize.Size = new System.Drawing.Size(64, 21);
      this.m_cbFontSize.TabIndex = 2;
      this.m_cbFontSize.Text = "comboBox1";
      this.m_cbFontSize.TextChanged += new System.EventHandler(this.OncbFontSize_TextChanged);
      // 
      // m_lblBackground
      // 
      this.m_lblBackground.Location = new System.Drawing.Point(22, 40);
      this.m_lblBackground.Name = "m_lblBackground";
      this.m_lblBackground.Size = new System.Drawing.Size(72, 16);
      this.m_lblBackground.TabIndex = 4;
      this.m_lblBackground.Text = "Background";
      // 
      // m_lblPosX
      // 
      this.m_lblPosX.Location = new System.Drawing.Point(8, 72);
      this.m_lblPosX.Name = "m_lblPosX";
      this.m_lblPosX.Size = new System.Drawing.Size(40, 16);
      this.m_lblPosX.TabIndex = 6;
      this.m_lblPosX.Text = "Pos.X";
      this.m_lblPosX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_edPosX
      // 
      this.m_edPosX.Location = new System.Drawing.Point(48, 72);
      this.m_edPosX.Name = "m_edPosX";
      this.m_edPosX.Size = new System.Drawing.Size(72, 20);
      this.m_edPosX.TabIndex = 7;
      this.m_edPosX.Text = "textBox1";
      this.m_edPosX.TextChanged += new System.EventHandler(this.OnedPosX_TextChanged);
      // 
      // m_lblPosY
      // 
      this.m_lblPosY.Location = new System.Drawing.Point(120, 72);
      this.m_lblPosY.Name = "m_lblPosY";
      this.m_lblPosY.Size = new System.Drawing.Size(40, 16);
      this.m_lblPosY.TabIndex = 8;
      this.m_lblPosY.Text = "Pos.Y";
      this.m_lblPosY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_edPosY
      // 
      this.m_edPosY.Location = new System.Drawing.Point(168, 72);
      this.m_edPosY.Name = "m_edPosY";
      this.m_edPosY.Size = new System.Drawing.Size(80, 20);
      this.m_edPosY.TabIndex = 9;
      this.m_edPosY.Text = "textBox1";
      this.m_edPosY.TextChanged += new System.EventHandler(this.OnedPosY_TextChanged);
      // 
      // m_lblRotation
      // 
      this.m_lblRotation.Location = new System.Drawing.Point(256, 72);
      this.m_lblRotation.Name = "m_lblRotation";
      this.m_lblRotation.Size = new System.Drawing.Size(32, 21);
      this.m_lblRotation.TabIndex = 10;
      this.m_lblRotation.Text = "Rot.";
      this.m_lblRotation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // m_cbRotation
      // 
      this.m_cbRotation.Location = new System.Drawing.Point(296, 73);
      this.m_cbRotation.Name = "m_cbRotation";
      this.m_cbRotation.Size = new System.Drawing.Size(66, 21);
      this.m_cbRotation.TabIndex = 11;
      this.m_cbRotation.Text = "comboBox1";
      this.m_cbRotation.TextChanged += new System.EventHandler(this.On_cbRotation_TextChanged);
      // 
      // m_btBold
      // 
      this.m_btBold.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_btBold.Image = ((System.Drawing.Image)(resources.GetObject("m_btBold.Image")));
      this.m_btBold.Location = new System.Drawing.Point(40, 112);
      this.m_btBold.Name = "m_btBold";
      this.m_btBold.Size = new System.Drawing.Size(32, 24);
      this.m_btBold.TabIndex = 15;
      this.m_btBold.Click += new System.EventHandler(this.OnbtBold_Click);
      // 
      // m_btItalic
      // 
      this.m_btItalic.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_btItalic.Image = ((System.Drawing.Image)(resources.GetObject("m_btItalic.Image")));
      this.m_btItalic.Location = new System.Drawing.Point(72, 112);
      this.m_btItalic.Name = "m_btItalic";
      this.m_btItalic.Size = new System.Drawing.Size(32, 24);
      this.m_btItalic.TabIndex = 16;
      this.m_btItalic.Click += new System.EventHandler(this.OnbtItalic_Click);
      // 
      // m_btUnderline
      // 
      this.m_btUnderline.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_btUnderline.Image = ((System.Drawing.Image)(resources.GetObject("m_btUnderline.Image")));
      this.m_btUnderline.Location = new System.Drawing.Point(104, 112);
      this.m_btUnderline.Name = "m_btUnderline";
      this.m_btUnderline.Size = new System.Drawing.Size(32, 24);
      this.m_btUnderline.TabIndex = 17;
      this.m_btUnderline.Click += new System.EventHandler(this.OnbtUnderline_Click);
      // 
      // m_btSupIndex
      // 
      this.m_btSupIndex.Image = ((System.Drawing.Image)(resources.GetObject("m_btSupIndex.Image")));
      this.m_btSupIndex.Location = new System.Drawing.Point(168, 112);
      this.m_btSupIndex.Name = "m_btSupIndex";
      this.m_btSupIndex.Size = new System.Drawing.Size(32, 24);
      this.m_btSupIndex.TabIndex = 19;
      this.m_btSupIndex.Click += new System.EventHandler(this.OnbtSupIndex_Click);
      // 
      // m_btSubIndex
      // 
      this.m_btSubIndex.Image = ((System.Drawing.Image)(resources.GetObject("m_btSubIndex.Image")));
      this.m_btSubIndex.Location = new System.Drawing.Point(200, 112);
      this.m_btSubIndex.Name = "m_btSubIndex";
      this.m_btSubIndex.Size = new System.Drawing.Size(32, 24);
      this.m_btSubIndex.TabIndex = 20;
      this.m_btSubIndex.Click += new System.EventHandler(this.OnbtSubIndex_Click);
      // 
      // m_btGreek
      // 
      this.m_btGreek.Font = new System.Drawing.Font("Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.m_btGreek.Image = ((System.Drawing.Image)(resources.GetObject("m_btGreek.Image")));
      this.m_btGreek.Location = new System.Drawing.Point(232, 112);
      this.m_btGreek.Name = "m_btGreek";
      this.m_btGreek.Size = new System.Drawing.Size(32, 24);
      this.m_btGreek.TabIndex = 21;
      this.m_btGreek.Click += new System.EventHandler(this.OnbtGreek_Click);
      // 
      // m_edText
      // 
      this.m_edText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_edText.Location = new System.Drawing.Point(8, 144);
      this.m_edText.Multiline = true;
      this.m_edText.Name = "m_edText";
      this.m_edText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.m_edText.Size = new System.Drawing.Size(400, 80);
      this.m_edText.TabIndex = 0;
      this.m_edText.Text = "textBox1";
      this.m_edText.TextChanged += new System.EventHandler(this.OnEditText_TextChanged);
      // 
      // m_pnPreview
      // 
      this.m_pnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_pnPreview.Location = new System.Drawing.Point(8, 240);
      this.m_pnPreview.Name = "m_pnPreview";
      this.m_pnPreview.Size = new System.Drawing.Size(408, 96);
      this.m_pnPreview.TabIndex = 22;
      this.m_pnPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPanelPreview_Paint);
      // 
      // m_btNormal
      // 
      this.m_btNormal.Image = ((System.Drawing.Image)(resources.GetObject("m_btNormal.Image")));
      this.m_btNormal.Location = new System.Drawing.Point(8, 112);
      this.m_btNormal.Name = "m_btNormal";
      this.m_btNormal.Size = new System.Drawing.Size(32, 24);
      this.m_btNormal.TabIndex = 14;
      this.m_btNormal.Click += new System.EventHandler(this.OnbtNormal_Click);
      // 
      // m_btStrikeout
      // 
      this.m_btStrikeout.Image = ((System.Drawing.Image)(resources.GetObject("m_btStrikeout.Image")));
      this.m_btStrikeout.Location = new System.Drawing.Point(136, 112);
      this.m_btStrikeout.Name = "m_btStrikeout";
      this.m_btStrikeout.Size = new System.Drawing.Size(32, 24);
      this.m_btStrikeout.TabIndex = 18;
      this.m_btStrikeout.Click += new System.EventHandler(this.OnbtStrikeout_Click);
      // 
      // m_cbFontColor
      // 
      this.m_cbFontColor.Color = System.Drawing.Color.Black;
      this.m_cbFontColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this.m_cbFontColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbFontColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbFontColor.FormattingEnabled = true;
      this.m_cbFontColor.ItemHeight = 15;
      this.m_cbFontColor.Items.AddRange(new object[] {
            System.Drawing.SystemColors.ActiveBorder,
            System.Drawing.SystemColors.ActiveCaption,
            System.Drawing.SystemColors.ActiveCaptionText,
            System.Drawing.SystemColors.AppWorkspace,
            System.Drawing.SystemColors.Control,
            System.Drawing.SystemColors.ControlDark,
            System.Drawing.SystemColors.ControlDarkDark,
            System.Drawing.SystemColors.ControlLight,
            System.Drawing.SystemColors.ControlLightLight,
            System.Drawing.SystemColors.ControlText,
            System.Drawing.SystemColors.Desktop,
            System.Drawing.SystemColors.GrayText,
            System.Drawing.SystemColors.Highlight,
            System.Drawing.SystemColors.HighlightText,
            System.Drawing.SystemColors.HotTrack,
            System.Drawing.SystemColors.InactiveBorder,
            System.Drawing.SystemColors.InactiveCaption,
            System.Drawing.SystemColors.InactiveCaptionText,
            System.Drawing.SystemColors.Info,
            System.Drawing.SystemColors.InfoText,
            System.Drawing.SystemColors.Menu,
            System.Drawing.SystemColors.MenuText,
            System.Drawing.SystemColors.ScrollBar,
            System.Drawing.SystemColors.Window,
            System.Drawing.SystemColors.WindowFrame,
            System.Drawing.SystemColors.WindowText,
            System.Drawing.Color.Transparent,
            System.Drawing.Color.AliceBlue,
            System.Drawing.Color.AntiqueWhite,
            System.Drawing.Color.Aqua,
            System.Drawing.Color.Aquamarine,
            System.Drawing.Color.Azure,
            System.Drawing.Color.Beige,
            System.Drawing.Color.Bisque,
            System.Drawing.Color.Black,
            System.Drawing.Color.BlanchedAlmond,
            System.Drawing.Color.Blue,
            System.Drawing.Color.BlueViolet,
            System.Drawing.Color.Brown,
            System.Drawing.Color.BurlyWood,
            System.Drawing.Color.CadetBlue,
            System.Drawing.Color.Chartreuse,
            System.Drawing.Color.Chocolate,
            System.Drawing.Color.Coral,
            System.Drawing.Color.CornflowerBlue,
            System.Drawing.Color.Cornsilk,
            System.Drawing.Color.Crimson,
            System.Drawing.Color.Cyan,
            System.Drawing.Color.DarkBlue,
            System.Drawing.Color.DarkCyan,
            System.Drawing.Color.DarkGoldenrod,
            System.Drawing.Color.DarkGray,
            System.Drawing.Color.DarkGreen,
            System.Drawing.Color.DarkKhaki,
            System.Drawing.Color.DarkMagenta,
            System.Drawing.Color.DarkOliveGreen,
            System.Drawing.Color.DarkOrange,
            System.Drawing.Color.DarkOrchid,
            System.Drawing.Color.DarkRed,
            System.Drawing.Color.DarkSalmon,
            System.Drawing.Color.DarkSeaGreen,
            System.Drawing.Color.DarkSlateBlue,
            System.Drawing.Color.DarkSlateGray,
            System.Drawing.Color.DarkTurquoise,
            System.Drawing.Color.DarkViolet,
            System.Drawing.Color.DeepPink,
            System.Drawing.Color.DeepSkyBlue,
            System.Drawing.Color.DimGray,
            System.Drawing.Color.DodgerBlue,
            System.Drawing.Color.Firebrick,
            System.Drawing.Color.FloralWhite,
            System.Drawing.Color.ForestGreen,
            System.Drawing.Color.Fuchsia,
            System.Drawing.Color.Gainsboro,
            System.Drawing.Color.GhostWhite,
            System.Drawing.Color.Gold,
            System.Drawing.Color.Goldenrod,
            System.Drawing.Color.Gray,
            System.Drawing.Color.Green,
            System.Drawing.Color.GreenYellow,
            System.Drawing.Color.Honeydew,
            System.Drawing.Color.HotPink,
            System.Drawing.Color.IndianRed,
            System.Drawing.Color.Indigo,
            System.Drawing.Color.Ivory,
            System.Drawing.Color.Khaki,
            System.Drawing.Color.Lavender,
            System.Drawing.Color.LavenderBlush,
            System.Drawing.Color.LawnGreen,
            System.Drawing.Color.LemonChiffon,
            System.Drawing.Color.LightBlue,
            System.Drawing.Color.LightCoral,
            System.Drawing.Color.LightCyan,
            System.Drawing.Color.LightGoldenrodYellow,
            System.Drawing.Color.LightGray,
            System.Drawing.Color.LightGreen,
            System.Drawing.Color.LightPink,
            System.Drawing.Color.LightSalmon,
            System.Drawing.Color.LightSeaGreen,
            System.Drawing.Color.LightSkyBlue,
            System.Drawing.Color.LightSlateGray,
            System.Drawing.Color.LightSteelBlue,
            System.Drawing.Color.LightYellow,
            System.Drawing.Color.Lime,
            System.Drawing.Color.LimeGreen,
            System.Drawing.Color.Linen,
            System.Drawing.Color.Magenta,
            System.Drawing.Color.Maroon,
            System.Drawing.Color.MediumAquamarine,
            System.Drawing.Color.MediumBlue,
            System.Drawing.Color.MediumOrchid,
            System.Drawing.Color.MediumPurple,
            System.Drawing.Color.MediumSeaGreen,
            System.Drawing.Color.MediumSlateBlue,
            System.Drawing.Color.MediumSpringGreen,
            System.Drawing.Color.MediumTurquoise,
            System.Drawing.Color.MediumVioletRed,
            System.Drawing.Color.MidnightBlue,
            System.Drawing.Color.MintCream,
            System.Drawing.Color.MistyRose,
            System.Drawing.Color.Moccasin,
            System.Drawing.Color.NavajoWhite,
            System.Drawing.Color.Navy,
            System.Drawing.Color.OldLace,
            System.Drawing.Color.Olive,
            System.Drawing.Color.OliveDrab,
            System.Drawing.Color.Orange,
            System.Drawing.Color.OrangeRed,
            System.Drawing.Color.Orchid,
            System.Drawing.Color.PaleGoldenrod,
            System.Drawing.Color.PaleGreen,
            System.Drawing.Color.PaleTurquoise,
            System.Drawing.Color.PaleVioletRed,
            System.Drawing.Color.PapayaWhip,
            System.Drawing.Color.PeachPuff,
            System.Drawing.Color.Peru,
            System.Drawing.Color.Pink,
            System.Drawing.Color.Plum,
            System.Drawing.Color.PowderBlue,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Red,
            System.Drawing.Color.RosyBrown,
            System.Drawing.Color.RoyalBlue,
            System.Drawing.Color.SaddleBrown,
            System.Drawing.Color.Salmon,
            System.Drawing.Color.SandyBrown,
            System.Drawing.Color.SeaGreen,
            System.Drawing.Color.SeaShell,
            System.Drawing.Color.Sienna,
            System.Drawing.Color.Silver,
            System.Drawing.Color.SkyBlue,
            System.Drawing.Color.SlateBlue,
            System.Drawing.Color.SlateGray,
            System.Drawing.Color.Snow,
            System.Drawing.Color.SpringGreen,
            System.Drawing.Color.SteelBlue,
            System.Drawing.Color.Tan,
            System.Drawing.Color.Teal,
            System.Drawing.Color.Thistle,
            System.Drawing.Color.Tomato,
            System.Drawing.Color.Turquoise,
            System.Drawing.Color.Violet,
            System.Drawing.Color.Wheat,
            System.Drawing.Color.White,
            System.Drawing.Color.WhiteSmoke,
            System.Drawing.Color.Yellow,
            System.Drawing.Color.YellowGreen,
            System.Drawing.SystemColors.ButtonFace,
            System.Drawing.SystemColors.ButtonHighlight,
            System.Drawing.SystemColors.ButtonShadow,
            System.Drawing.SystemColors.GradientActiveCaption,
            System.Drawing.SystemColors.GradientInactiveCaption,
            System.Drawing.SystemColors.MenuBar,
            System.Drawing.SystemColors.MenuHighlight});
      this.m_cbFontColor.Location = new System.Drawing.Point(232, 8);
      this.m_cbFontColor.Name = "m_cbFontColor";
      this.m_cbFontColor.Size = new System.Drawing.Size(130, 21);
      this.m_cbFontColor.TabIndex = 24;
      this.m_cbFontColor.SelectionChangeCommitted += new System.EventHandler(this.OncbFontColor_SelectedIndexChanged);
      // 
      // _ctrlBackgroundStyle
      // 
      this._ctrlBackgroundStyle.AutoSize = true;
      this._ctrlBackgroundStyle.Controller = null;
      this._ctrlBackgroundStyle.Location = new System.Drawing.Point(100, 36);
      this._ctrlBackgroundStyle.Margin = new System.Windows.Forms.Padding(0);
      this._ctrlBackgroundStyle.Name = "_ctrlBackgroundStyle";
      this._ctrlBackgroundStyle.Size = new System.Drawing.Size(262, 30);
      this._ctrlBackgroundStyle.TabIndex = 23;
      // 
      // TextControlDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(424, 342);
      this.Controls.Add(this.m_cbFontColor);
      this.Controls.Add(this._ctrlBackgroundStyle);
      this.Controls.Add(this.m_btStrikeout);
      this.Controls.Add(this.m_btNormal);
      this.Controls.Add(this.m_pnPreview);
      this.Controls.Add(this.m_edText);
      this.Controls.Add(this.m_edPosY);
      this.Controls.Add(this.m_edPosX);
      this.Controls.Add(this.m_btGreek);
      this.Controls.Add(this.m_btSubIndex);
      this.Controls.Add(this.m_btSupIndex);
      this.Controls.Add(this.m_btUnderline);
      this.Controls.Add(this.m_btItalic);
      this.Controls.Add(this.m_btBold);
      this.Controls.Add(this.m_cbRotation);
      this.Controls.Add(this.m_lblRotation);
      this.Controls.Add(this.m_lblPosY);
      this.Controls.Add(this.m_lblPosX);
      this.Controls.Add(this.m_lblBackground);
      this.Controls.Add(this.m_cbFontSize);
      this.Controls.Add(this.m_cbFonts);
      this.Controls.Add(this.m_btCancel);
      this.Controls.Add(this.m_btOK);
      this.Name = "TextControlDialog";
      this.Text = "Text Control";
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private void OnPanelPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      g.PageUnit = GraphicsUnit.Point;

      // set position and rotation to zero
      //    m_TextObject.Position=new PointF(0,0);
      //    m_TextObject.Rotation = 0;
      m_TextObject.Paint(g,m_Layer,true);
    
      // restore the original position and rotation values
      //      m_TextObject.Position = new PointF(m_PositionX,m_PositionY);
      //      m_TextObject.Rotation = _rotation;
    
    }

    private void OnEditText_TextChanged(object sender, System.EventArgs e)
    {
      this.m_TextObject.Text = this.m_edText.Text;
      this.m_pnPreview.Invalidate();
    }

    private void InsertBeforeAndAfterSelectedText(string insbefore, string insafter)
    {
      if(0!=this.m_edText.SelectionLength)
      {
        // insert \b( at beginning of selection and ) at the end of the selection
        int len   = m_edText.Text.Length;
        int start = m_edText.SelectionStart;
        int end   = m_edText.SelectionStart + m_edText.SelectionLength;
        m_edText.Text = m_edText.Text.Substring(0,start)+insbefore+m_edText.Text.Substring(start,end-start)+insafter+m_edText.Text.Substring(end,len-end);
      
        // now select the text plus the text before and after
        m_edText.Focus(); // necassary to show the selected area
        m_edText.Select(start,end - start + insbefore.Length + insafter.Length);
      }
    }

    private void OnbtBold_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\b(",")");
    }

    private void OnbtItalic_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\i(",")");
    
    }

    private void OnbtUnderline_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\u(",")");
    
    }

    private void OnbtSupIndex_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\+(",")");
    
    }

    private void OnbtSubIndex_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\-(",")");
    
    }

    private void OnbtGreek_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\g(",")");
    
    }

    private void OnbtNormal_Click(object sender, System.EventArgs e)
    {
      // remove a backslash x ( at the beginning and the closing brace at the end of the selection
      if(this.m_edText.SelectionLength>=4)
      {
        int len   = m_edText.Text.Length;
        int start = m_edText.SelectionStart;
        int end   = m_edText.SelectionStart + m_edText.SelectionLength;

        if(m_edText.Text[start]=='\\' && m_edText.Text[start+2]=='(' && m_edText.Text[end-1]==')')
        {
          m_edText.Text = m_edText.Text.Substring(0,start) 
            + m_edText.Text.Substring(start+3,end-start-4)
            + m_edText.Text.Substring(end,len-end);

          // now select again the rest of the text
          m_edText.Focus(); // neccessary to show the selected area
          m_edText.Select(start, end - start -4 );
        }
      }
    }

    private void OnbtStrikeout_Click(object sender, System.EventArgs e)
    {
      // insert \b( at beginning of selection and ) at the end of the selection
      InsertBeforeAndAfterSelectedText("\\s(",")");
    }

    private void OncbFonts_TextChanged(object sender, System.EventArgs e)
    {
      if(m_bDialogInitialized)
      {
        FontFamily ff = this.m_cbFonts.FontFamilyDocument;
        // make sure that regular style is available
        if(ff.IsStyleAvailable(FontStyle.Regular))
          this.m_TextObject.Font = new Font(ff,this.m_TextObject.Font.Size,FontStyle.Regular,GraphicsUnit.World);
        else if(ff.IsStyleAvailable(FontStyle.Bold))
          this.m_TextObject.Font = new Font(ff,this.m_TextObject.Font.Size,FontStyle.Bold,GraphicsUnit.World);
        else if(ff.IsStyleAvailable(FontStyle.Italic))
          this.m_TextObject.Font = new Font(ff,this.m_TextObject.Font.Size,FontStyle.Italic,GraphicsUnit.World);

        this.m_pnPreview.Invalidate();
      }   
    }

    private void OncbFontSize_TextChanged(object sender, System.EventArgs e)
    {
      if(m_bDialogInitialized)
      {

        try
        {
          string str = (string)this.m_cbFontSize.Text;
          float newSize = System.Convert.ToSingle(str);
          Font oldFont = this.m_TextObject.Font;
          this.m_TextObject.Font = new Font(oldFont.FontFamily,newSize,oldFont.Style,GraphicsUnit.World);
          this.m_pnPreview.Invalidate();
        }
        catch(Exception)
        {
        }
      }   
    }

    private void OncbFontColor_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      if(m_bDialogInitialized)
      {


        this.m_TextObject.Color = m_cbFontColor.Color;
          this.m_pnPreview.Invalidate();
       
      }   
    }


    private void EhBackgroundChanged(object sender, System.EventArgs e)
    {
      if(m_bDialogInitialized)
      {
        this.m_TextObject.Background = (IBackgroundStyle)_backgroundStyleController.TemporaryModelObject;
        this.m_pnPreview.Invalidate();
      }   
    }

    private void On_cbRotation_TextChanged(object sender, System.EventArgs e)
    {
      if(this.m_bDialogInitialized)
      {
        try 
        { 
          m_Rotation = System.Convert.ToSingle(this.m_cbRotation.Text);
          m_TextObject.Rotation = m_Rotation;
        }
        catch(Exception)
        {
        }
      }
    
    }

    private void OnedPosX_TextChanged(object sender, System.EventArgs e)
    {
      if(this.m_bDialogInitialized)
      {
        try 
        { 
          m_PositionX = System.Convert.ToSingle(this.m_edPosX.Text);
          m_TextObject.Position = new PointF(m_PositionX, m_PositionY);
        }
        catch(Exception)
        {
        }
      }
    
    
    }

    private void OnedPosY_TextChanged(object sender, System.EventArgs e)
    {
      if(this.m_bDialogInitialized)
      {
        try 
        { 
          m_PositionY = System.Convert.ToSingle(this.m_edPosY.Text);
          m_TextObject.Position = new PointF(m_PositionX, m_PositionY);
        }
        catch(Exception)
        {
        }
      }
    }


  }
}
