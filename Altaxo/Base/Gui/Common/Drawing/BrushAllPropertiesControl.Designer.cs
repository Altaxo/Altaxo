namespace Altaxo.Gui.Common.Drawing
{
  partial class BrushAllPropertiesControl
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
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this._lblBrushColor = new System.Windows.Forms.Label();
      this._lblBackColor = new System.Windows.Forms.Label();
      this._lblBrushType = new System.Windows.Forms.Label();
      this._lblHatchStyle = new System.Windows.Forms.Label();
      this._brushPreviewPanel = new System.Windows.Forms.Panel();
      this._cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._cbBrushType = new Altaxo.Gui.Common.Drawing.BrushTypeComboBox();
      this._cbBackColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._cbHatchStyle = new Altaxo.Gui.Common.Drawing.HatchStyleComboBox();
      this._brushGlue = new Altaxo.Gui.Common.Drawing.BrushControlsGlue();
      this.flowLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
      this.flowLayoutPanel1.Controls.Add(this._brushPreviewPanel);
      this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(393, 241);
      this.flowLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this._cbColor, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this._cbBrushType, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this._cbBackColor, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this._cbHatchStyle, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this._lblBrushColor, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this._lblBackColor, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this._lblBrushType, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this._lblHatchStyle, 2, 1);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(387, 60);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // _lblBrushColor
      // 
      this._lblBrushColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblBrushColor.AutoSize = true;
      this._lblBrushColor.Location = new System.Drawing.Point(28, 6);
      this._lblBrushColor.Name = "_lblBrushColor";
      this._lblBrushColor.Size = new System.Drawing.Size(34, 13);
      this._lblBrushColor.TabIndex = 4;
      this._lblBrushColor.Text = "Color:";
      // 
      // _lblBackColor
      // 
      this._lblBackColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblBackColor.AutoSize = true;
      this._lblBackColor.Location = new System.Drawing.Point(3, 31);
      this._lblBackColor.Name = "_lblBackColor";
      this._lblBackColor.Size = new System.Drawing.Size(59, 13);
      this._lblBackColor.TabIndex = 5;
      this._lblBackColor.Text = "BackColor:";
      // 
      // _lblBrushType
      // 
      this._lblBrushType.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblBrushType.AutoSize = true;
      this._lblBrushType.Location = new System.Drawing.Point(196, 6);
      this._lblBrushType.Name = "_lblBrushType";
      this._lblBrushType.Size = new System.Drawing.Size(61, 13);
      this._lblBrushType.TabIndex = 6;
      this._lblBrushType.Text = "BrushType:";
      // 
      // _lblHatchStyle
      // 
      this._lblHatchStyle.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._lblHatchStyle.AutoSize = true;
      this._lblHatchStyle.Location = new System.Drawing.Point(195, 31);
      this._lblHatchStyle.Name = "_lblHatchStyle";
      this._lblHatchStyle.Size = new System.Drawing.Size(62, 13);
      this._lblHatchStyle.TabIndex = 7;
      this._lblHatchStyle.Text = "HatchStyle:";
      // 
      // _brushPreviewPanel
      // 
      this._brushPreviewPanel.Location = new System.Drawing.Point(3, 69);
      this._brushPreviewPanel.Name = "_brushPreviewPanel";
      this._brushPreviewPanel.Size = new System.Drawing.Size(387, 169);
      this._brushPreviewPanel.TabIndex = 1;
      this._brushPreviewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.EhBrushPreview_Paint);
      // 
      // _cbColor
      // 
      this._cbColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColor.FormattingEnabled = true;
      this._cbColor.ItemHeight = 13;
      this._cbColor.Items.AddRange(new object[] {
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
      this._cbColor.Location = new System.Drawing.Point(68, 3);
      this._cbColor.Name = "_cbColor";
      this._cbColor.Size = new System.Drawing.Size(121, 19);
      this._cbColor.TabIndex = 0;
      // 
      // _cbBrushType
      // 
      this._cbBrushType.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbBrushType.BrushType = Altaxo.Graph.BrushType.SolidBrush;
      this._cbBrushType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbBrushType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbBrushType.FormattingEnabled = true;
      this._cbBrushType.ItemHeight = 13;
      this._cbBrushType.Items.AddRange(new object[] {
            Altaxo.Graph.BrushType.SolidBrush,
            Altaxo.Graph.BrushType.HatchBrush,
            Altaxo.Graph.BrushType.TextureBrush,
            Altaxo.Graph.BrushType.LinearGradientBrush,
            Altaxo.Graph.BrushType.PathGradientBrush});
      this._cbBrushType.Location = new System.Drawing.Point(263, 3);
      this._cbBrushType.Name = "_cbBrushType";
      this._cbBrushType.Size = new System.Drawing.Size(121, 19);
      this._cbBrushType.TabIndex = 1;
      // 
      // _cbBackColor
      // 
      this._cbBackColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbBackColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbBackColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbBackColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbBackColor.FormattingEnabled = true;
      this._cbBackColor.ItemHeight = 13;
      this._cbBackColor.Items.AddRange(new object[] {
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
      this._cbBackColor.Location = new System.Drawing.Point(68, 28);
      this._cbBackColor.Name = "_cbBackColor";
      this._cbBackColor.Size = new System.Drawing.Size(121, 19);
      this._cbBackColor.TabIndex = 2;
      // 
      // _cbHatchStyle
      // 
      this._cbHatchStyle.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbHatchStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbHatchStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbHatchStyle.FormattingEnabled = true;
      this._cbHatchStyle.HatchStyle = System.Drawing.Drawing2D.HatchStyle.Horizontal;
      this._cbHatchStyle.ItemHeight = 13;
      this._cbHatchStyle.Items.AddRange(new object[] {
            System.Drawing.Drawing2D.HatchStyle.Horizontal,
            System.Drawing.Drawing2D.HatchStyle.Horizontal,
            System.Drawing.Drawing2D.HatchStyle.Vertical,
            System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.LargeGrid,
            System.Drawing.Drawing2D.HatchStyle.LargeGrid,
            System.Drawing.Drawing2D.HatchStyle.LargeGrid,
            System.Drawing.Drawing2D.HatchStyle.DiagonalCross,
            System.Drawing.Drawing2D.HatchStyle.Percent05,
            System.Drawing.Drawing2D.HatchStyle.Percent10,
            System.Drawing.Drawing2D.HatchStyle.Percent20,
            System.Drawing.Drawing2D.HatchStyle.Percent25,
            System.Drawing.Drawing2D.HatchStyle.Percent30,
            System.Drawing.Drawing2D.HatchStyle.Percent40,
            System.Drawing.Drawing2D.HatchStyle.Percent50,
            System.Drawing.Drawing2D.HatchStyle.Percent60,
            System.Drawing.Drawing2D.HatchStyle.Percent70,
            System.Drawing.Drawing2D.HatchStyle.Percent75,
            System.Drawing.Drawing2D.HatchStyle.Percent80,
            System.Drawing.Drawing2D.HatchStyle.Percent90,
            System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.LightVertical,
            System.Drawing.Drawing2D.HatchStyle.LightHorizontal,
            System.Drawing.Drawing2D.HatchStyle.NarrowVertical,
            System.Drawing.Drawing2D.HatchStyle.NarrowHorizontal,
            System.Drawing.Drawing2D.HatchStyle.DarkVertical,
            System.Drawing.Drawing2D.HatchStyle.DarkHorizontal,
            System.Drawing.Drawing2D.HatchStyle.DashedDownwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DashedUpwardDiagonal,
            System.Drawing.Drawing2D.HatchStyle.DashedHorizontal,
            System.Drawing.Drawing2D.HatchStyle.DashedVertical,
            System.Drawing.Drawing2D.HatchStyle.SmallConfetti,
            System.Drawing.Drawing2D.HatchStyle.LargeConfetti,
            System.Drawing.Drawing2D.HatchStyle.ZigZag,
            System.Drawing.Drawing2D.HatchStyle.Wave,
            System.Drawing.Drawing2D.HatchStyle.DiagonalBrick,
            System.Drawing.Drawing2D.HatchStyle.HorizontalBrick,
            System.Drawing.Drawing2D.HatchStyle.Weave,
            System.Drawing.Drawing2D.HatchStyle.Plaid,
            System.Drawing.Drawing2D.HatchStyle.Divot,
            System.Drawing.Drawing2D.HatchStyle.DottedGrid,
            System.Drawing.Drawing2D.HatchStyle.DottedDiamond,
            System.Drawing.Drawing2D.HatchStyle.Shingle,
            System.Drawing.Drawing2D.HatchStyle.Trellis,
            System.Drawing.Drawing2D.HatchStyle.Sphere,
            System.Drawing.Drawing2D.HatchStyle.SmallGrid,
            System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard,
            System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard,
            System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond,
            System.Drawing.Drawing2D.HatchStyle.SolidDiamond});
      this._cbHatchStyle.Location = new System.Drawing.Point(263, 28);
      this._cbHatchStyle.Name = "_cbHatchStyle";
      this._cbHatchStyle.Size = new System.Drawing.Size(121, 19);
      this._cbHatchStyle.TabIndex = 3;
      // 
      // _brushGlue
      // 
      this._brushGlue.CbBrushType = this._cbBrushType;
      this._brushGlue.CbColor1 = this._cbColor;
      this._brushGlue.CbColor2 = this._cbBackColor;
      this._brushGlue.CbHatchStyle = this._cbHatchStyle;
      this._brushGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._brushGlue.LabelColor2 = this._lblBackColor;
      this._brushGlue.LabelHatchStyle = this._lblHatchStyle;
      this._brushGlue.BrushChanged += new System.EventHandler(this.EhBrushChanged);
      // 
      // BrushAllPropertiesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Name = "BrushAllPropertiesControl";
      this.Size = new System.Drawing.Size(396, 244);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private ColorComboBox _cbColor;
    private BrushTypeComboBox _cbBrushType;
    private ColorComboBox _cbBackColor;
    private HatchStyleComboBox _cbHatchStyle;
    private System.Windows.Forms.Label _lblBrushColor;
    private System.Windows.Forms.Label _lblBackColor;
    private System.Windows.Forms.Label _lblBrushType;
    private System.Windows.Forms.Label _lblHatchStyle;
    private System.Windows.Forms.Panel _brushPreviewPanel;
    private BrushControlsGlue _brushGlue;
  }
}
