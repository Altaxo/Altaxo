namespace Altaxo.Gui.Common.Drawing
{
  partial class PenAllPropertiesControl
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PenAllPropertiesControl));
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this._lblColor = new System.Windows.Forms.Label();
      this._lblStyle = new System.Windows.Forms.Label();
      this._cbDashStyle = new Altaxo.Gui.Common.Drawing.DashStyleComboBox();
      this._cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._cbDashCap = new Altaxo.Gui.Common.Drawing.DashCapComboBox();
      this._lblDashCap = new System.Windows.Forms.Label();
      this._lblStartCap = new System.Windows.Forms.Label();
      this._lblEndCap = new System.Windows.Forms.Label();
      this._lblJoin = new System.Windows.Forms.Label();
      this._lblMiter = new System.Windows.Forms.Label();
      this._cbLineJoin = new Altaxo.Gui.Common.Drawing.LineJoinComboBox();
      this._cbMiterLimit = new Altaxo.Gui.Common.Drawing.MiterLimitComboBox();
      this._cbThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._lblThickness = new System.Windows.Forms.Label();
      this.lineCapComboBox1 = new Altaxo.Gui.Common.Drawing.LineCapComboBox();
      this._cbStartCap = new Altaxo.Gui.Common.Drawing.LineCapComboBox();
      this._cbEndCap = new Altaxo.Gui.Common.Drawing.LineCapComboBox();
      this._LineDesignPanel = new System.Windows.Forms.Panel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this._lblColor, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this._lblStyle, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this._cbDashStyle, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this._cbColor, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this._cbDashCap, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this._lblDashCap, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this._lblStartCap, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this._lblEndCap, 2, 4);
      this.tableLayoutPanel1.Controls.Add(this._lblJoin, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this._lblMiter, 2, 5);
      this.tableLayoutPanel1.Controls.Add(this._cbLineJoin, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this._cbMiterLimit, 3, 5);
      this.tableLayoutPanel1.Controls.Add(this._cbThickness, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this._lblThickness, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this._cbStartCap, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this._cbEndCap, 3, 4);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 6;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(379, 145);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // _lblColor
      // 
      this._lblColor.AutoSize = true;
      this._lblColor.Location = new System.Drawing.Point(3, 0);
      this._lblColor.Name = "_lblColor";
      this._lblColor.Size = new System.Drawing.Size(34, 13);
      this._lblColor.TabIndex = 0;
      this._lblColor.Text = "Color:";
      // 
      // _lblStyle
      // 
      this._lblStyle.AutoSize = true;
      this._lblStyle.Location = new System.Drawing.Point(3, 70);
      this._lblStyle.Name = "_lblStyle";
      this._lblStyle.Size = new System.Drawing.Size(33, 13);
      this._lblStyle.TabIndex = 1;
      this._lblStyle.Text = "Style:";
      // 
      // _cbDashStyle
      // 
      this._cbDashStyle.DashStyleEx = ((Altaxo.Graph.DashStyleEx)(resources.GetObject("_cbDashStyle.DashStyleEx")));
      this._cbDashStyle.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbDashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbDashStyle.FormattingEnabled = true;
      this._cbDashStyle.ItemHeight = 13;
      this._cbDashStyle.Items.AddRange(new object[] {
            ((object)(resources.GetObject("_cbDashStyle.Items"))),
            ((object)(resources.GetObject("_cbDashStyle.Items1"))),
            ((object)(resources.GetObject("_cbDashStyle.Items2"))),
            ((object)(resources.GetObject("_cbDashStyle.Items3"))),
            ((object)(resources.GetObject("_cbDashStyle.Items4"))),
            ((object)(resources.GetObject("_cbDashStyle.Items5"))),
            ((object)(resources.GetObject("_cbDashStyle.Items6"))),
            ((object)(resources.GetObject("_cbDashStyle.Items7"))),
            ((object)(resources.GetObject("_cbDashStyle.Items8"))),
            ((object)(resources.GetObject("_cbDashStyle.Items9")))});
      this._cbDashStyle.Location = new System.Drawing.Point(68, 73);
      this._cbDashStyle.Name = "_cbDashStyle";
      this._cbDashStyle.Size = new System.Drawing.Size(121, 19);
      this._cbDashStyle.TabIndex = 2;
      // 
      // _cbColor
      // 
      this._cbColor.Color = System.Drawing.Color.Black;
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
            System.Drawing.SystemColors.MenuHighlight,
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
      this._cbColor.TabIndex = 3;
      // 
      // _cbDashCap
      // 
      this._cbDashCap.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
      this._cbDashCap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbDashCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbDashCap.FormattingEnabled = true;
      this._cbDashCap.ItemHeight = 13;
      this._cbDashCap.Items.AddRange(new object[] {
            System.Drawing.Drawing2D.DashCap.Flat,
            System.Drawing.Drawing2D.DashCap.Round,
            System.Drawing.Drawing2D.DashCap.Triangle,
            System.Drawing.Drawing2D.DashCap.Flat,
            System.Drawing.Drawing2D.DashCap.Round,
            System.Drawing.Drawing2D.DashCap.Triangle});
      this._cbDashCap.Location = new System.Drawing.Point(255, 73);
      this._cbDashCap.Name = "_cbDashCap";
      this._cbDashCap.Size = new System.Drawing.Size(121, 19);
      this._cbDashCap.TabIndex = 4;
      // 
      // _lblDashCap
      // 
      this._lblDashCap.AutoSize = true;
      this._lblDashCap.Location = new System.Drawing.Point(195, 70);
      this._lblDashCap.Name = "_lblDashCap";
      this._lblDashCap.Size = new System.Drawing.Size(54, 13);
      this._lblDashCap.TabIndex = 5;
      this._lblDashCap.Text = "DashCap:";
      // 
      // _lblStartCap
      // 
      this._lblStartCap.AutoSize = true;
      this._lblStartCap.Location = new System.Drawing.Point(3, 95);
      this._lblStartCap.Name = "_lblStartCap";
      this._lblStartCap.Size = new System.Drawing.Size(32, 13);
      this._lblStartCap.TabIndex = 6;
      this._lblStartCap.Text = "Start:";
      // 
      // _lblEndCap
      // 
      this._lblEndCap.AutoSize = true;
      this._lblEndCap.Location = new System.Drawing.Point(195, 95);
      this._lblEndCap.Name = "_lblEndCap";
      this._lblEndCap.Size = new System.Drawing.Size(29, 13);
      this._lblEndCap.TabIndex = 7;
      this._lblEndCap.Text = "End:";
      // 
      // _lblJoin
      // 
      this._lblJoin.AutoSize = true;
      this._lblJoin.Location = new System.Drawing.Point(3, 120);
      this._lblJoin.Name = "_lblJoin";
      this._lblJoin.Size = new System.Drawing.Size(29, 13);
      this._lblJoin.TabIndex = 8;
      this._lblJoin.Text = "Join:";
      // 
      // _lblMiter
      // 
      this._lblMiter.AutoSize = true;
      this._lblMiter.Location = new System.Drawing.Point(195, 120);
      this._lblMiter.Name = "_lblMiter";
      this._lblMiter.Size = new System.Drawing.Size(33, 13);
      this._lblMiter.TabIndex = 9;
      this._lblMiter.Text = "Miter:";
      // 
      // _cbLineJoin
      // 
      this._cbLineJoin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbLineJoin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbLineJoin.FormattingEnabled = true;
      this._cbLineJoin.ItemHeight = 13;
      this._cbLineJoin.Items.AddRange(new object[] {
            System.Drawing.Drawing2D.LineJoin.Miter,
            System.Drawing.Drawing2D.LineJoin.Bevel,
            System.Drawing.Drawing2D.LineJoin.Round,
            System.Drawing.Drawing2D.LineJoin.MiterClipped});
      this._cbLineJoin.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
      this._cbLineJoin.Location = new System.Drawing.Point(68, 123);
      this._cbLineJoin.Name = "_cbLineJoin";
      this._cbLineJoin.Size = new System.Drawing.Size(121, 19);
      this._cbLineJoin.TabIndex = 10;
      // 
      // _cbMiterLimit
      // 
      this._cbMiterLimit.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbMiterLimit.FormattingEnabled = true;
      this._cbMiterLimit.ItemHeight = 13;
      this._cbMiterLimit.Items.AddRange(new object[] {
            0F,
            0.125F,
            0.25F,
            0.5F,
            1F,
            2F,
            3F,
            5F,
            10F});
      this._cbMiterLimit.Location = new System.Drawing.Point(255, 123);
      this._cbMiterLimit.MiterValue = 1F;
      this._cbMiterLimit.Name = "_cbMiterLimit";
      this._cbMiterLimit.Size = new System.Drawing.Size(121, 19);
      this._cbMiterLimit.TabIndex = 11;
      // 
      // _cbThickness
      // 
      this._cbThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbThickness.FormattingEnabled = true;
      this._cbThickness.ItemHeight = 13;
      this._cbThickness.Items.AddRange(new object[] {
            0F,
            0.125F,
            0.25F,
            0.5F,
            1F,
            2F,
            3F,
            5F,
            10F});
      this._cbThickness.Location = new System.Drawing.Point(68, 48);
      this._cbThickness.Name = "_cbThickness";
      this._cbThickness.Size = new System.Drawing.Size(121, 19);
      this._cbThickness.TabIndex = 12;
      this._cbThickness.Thickness = 1F;
      // 
      // _lblThickness
      // 
      this._lblThickness.AutoSize = true;
      this._lblThickness.Location = new System.Drawing.Point(3, 45);
      this._lblThickness.Name = "_lblThickness";
      this._lblThickness.Size = new System.Drawing.Size(59, 13);
      this._lblThickness.TabIndex = 13;
      this._lblThickness.Text = "Thickness:";
      // 
      // lineCapComboBox1
      // 
      this.lineCapComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.lineCapComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.lineCapComboBox1.ItemHeight = 13;
      this.lineCapComboBox1.Location = new System.Drawing.Point(0, 0);
      this.lineCapComboBox1.Name = "lineCapComboBox1";
      this.lineCapComboBox1.Size = new System.Drawing.Size(121, 19);
      this.lineCapComboBox1.TabIndex = 0;
      // 
      // _cbStartCap
      // 
      this._cbStartCap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbStartCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbStartCap.FormattingEnabled = true;
      this._cbStartCap.ItemHeight = 13;

      this._cbStartCap.LineCapEx = Altaxo.Graph.LineCapEx.Flat;
      this._cbStartCap.Location = new System.Drawing.Point(68, 98);
      this._cbStartCap.Name = "_cbStartCap";
      this._cbStartCap.Size = new System.Drawing.Size(121, 19);
      this._cbStartCap.TabIndex = 14;
      // 
      // _cbEndCap
      // 
      this._cbEndCap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbEndCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbEndCap.FormattingEnabled = true;
      this._cbEndCap.ItemHeight = 13;

      this._cbEndCap.LineCapEx = Altaxo.Graph.LineCapEx.Flat;
      this._cbEndCap.Location = new System.Drawing.Point(255, 98);
      this._cbEndCap.Name = "_cbEndCap";
      this._cbEndCap.Size = new System.Drawing.Size(121, 19);
      this._cbEndCap.TabIndex = 15;
      // 
      // _LineDesignPanel
      // 
      this._LineDesignPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this._LineDesignPanel.Location = new System.Drawing.Point(0, 280);
      this._LineDesignPanel.Name = "_LineDesignPanel";
      this._LineDesignPanel.Size = new System.Drawing.Size(380, 120);
      this._LineDesignPanel.TabIndex = 1;
      // 
      // PenAllPropertiesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this._LineDesignPanel);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "PenAllPropertiesControl";
      this.Size = new System.Drawing.Size(385, 403);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label _lblColor;
    private System.Windows.Forms.Label _lblStyle;
    private DashStyleComboBox _cbDashStyle;
    private ColorComboBox _cbColor;
    private DashCapComboBox _cbDashCap;
    private System.Windows.Forms.Label _lblDashCap;
    private System.Windows.Forms.Label _lblStartCap;
    private System.Windows.Forms.Label _lblEndCap;
    private System.Windows.Forms.Label _lblJoin;
    private System.Windows.Forms.Label _lblMiter;
    private LineJoinComboBox _cbLineJoin;
    private MiterLimitComboBox _cbMiterLimit;
    private LineThicknessComboBox _cbThickness;
    private System.Windows.Forms.Label _lblThickness;
    private LineCapComboBox lineCapComboBox1;
    private LineCapComboBox _cbStartCap;
    private LineCapComboBox _cbEndCap;
    private System.Windows.Forms.Panel _LineDesignPanel;
  }
}
