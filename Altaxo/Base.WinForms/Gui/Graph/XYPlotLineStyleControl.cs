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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Collections;

using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;
namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for XYPlotLineStyleControl.
  /// </summary>
  [UserControlForController(typeof(IXYPlotLineStyleViewEventSink))]
  public class XYPlotLineStyleControl : System.Windows.Forms.UserControl, IXYPlotLineStyleView
  {
    private IXYPlotLineStyleViewEventSink _controller;
    private bool _EnableDisableAll=false;
    private int m_SuppressEvents = 0;
    private System.Windows.Forms.ComboBox m_cbLineFillDirection;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox m_chkLineFillArea;
    private System.Windows.Forms.ComboBox m_cbLineConnect;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox m_chkLineSymbolGap;
    private System.Windows.Forms.CheckBox _chkIndependentColor;
    private Altaxo.Gui.Common.Drawing.BrushColorComboBox m_cbLineFillColor;
    private Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl _ctrlColorTypeThickness;
    private CheckBox _chkIndependentFillColor;
    private CheckBox _chkConnectCircular;
    private IContainer components;

    public XYPlotLineStyleControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();


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
      this.components = new System.ComponentModel.Container();
      this._chkConnectCircular = new System.Windows.Forms.CheckBox();
      this._chkIndependentFillColor = new System.Windows.Forms.CheckBox();
      this._ctrlColorTypeThickness = new Altaxo.Gui.Common.Drawing.ColorTypeThicknessPenControl();
      this.m_cbLineFillColor = new Altaxo.Gui.Common.Drawing.BrushColorComboBox();
      this.m_cbLineFillDirection = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.m_chkLineFillArea = new System.Windows.Forms.CheckBox();
      this.m_cbLineConnect = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this._chkIndependentColor = new System.Windows.Forms.CheckBox();
      this.m_chkLineSymbolGap = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // _chkConnectCircular
      // 
      this._chkConnectCircular.AutoSize = true;
      this._chkConnectCircular.Location = new System.Drawing.Point(95, 33);
      this._chkConnectCircular.Name = "_chkConnectCircular";
      this._chkConnectCircular.Size = new System.Drawing.Size(117, 17);
      this._chkConnectCircular.TabIndex = 28;
      this._chkConnectCircular.Text = "Circular connection";
      this._chkConnectCircular.UseVisualStyleBackColor = true;
      // 
      // _chkIndependentFillColor
      // 
      this._chkIndependentFillColor.AutoSize = true;
      this._chkIndependentFillColor.Location = new System.Drawing.Point(5, 209);
      this._chkIndependentFillColor.Name = "_chkIndependentFillColor";
      this._chkIndependentFillColor.Size = new System.Drawing.Size(87, 17);
      this._chkIndependentFillColor.TabIndex = 27;
      this._chkIndependentFillColor.Text = "ind. Fill color:";
      this._chkIndependentFillColor.UseVisualStyleBackColor = true;
      // 
      // _ctrlColorTypeThickness
      // 
      this._ctrlColorTypeThickness.AutoSize = true;
      this._ctrlColorTypeThickness.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._ctrlColorTypeThickness.ColorType = Altaxo.Graph.ColorType.PlotColor;
      this._ctrlColorTypeThickness.Controller = null;
      this._ctrlColorTypeThickness.DocPen = null;
      this._ctrlColorTypeThickness.Location = new System.Drawing.Point(25, 68);
      this._ctrlColorTypeThickness.Name = "_ctrlColorTypeThickness";
      this._ctrlColorTypeThickness.Size = new System.Drawing.Size(198, 87);
      this._ctrlColorTypeThickness.TabIndex = 26;
      // 
      // m_cbLineFillColor
      // 
      this.m_cbLineFillColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this.m_cbLineFillColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.m_cbLineFillColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbLineFillColor.FormattingEnabled = true;
      this.m_cbLineFillColor.ItemHeight = 15;
      this.m_cbLineFillColor.Items.AddRange(new object[] {
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
      this.m_cbLineFillColor.Location = new System.Drawing.Point(96, 207);
      this.m_cbLineFillColor.Name = "m_cbLineFillColor";
      this.m_cbLineFillColor.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineFillColor.TabIndex = 25;
      // 
      // m_cbLineFillDirection
      // 
      this.m_cbLineFillDirection.Location = new System.Drawing.Point(96, 236);
      this.m_cbLineFillDirection.Name = "m_cbLineFillDirection";
      this.m_cbLineFillDirection.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineFillDirection.TabIndex = 9;
      this.m_cbLineFillDirection.Text = "comboBox1";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(17, 239);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(75, 16);
      this.label4.TabIndex = 7;
      this.label4.Text = "Fill Direction";
      // 
      // m_chkLineFillArea
      // 
      this.m_chkLineFillArea.Location = new System.Drawing.Point(5, 187);
      this.m_chkLineFillArea.Name = "m_chkLineFillArea";
      this.m_chkLineFillArea.Size = new System.Drawing.Size(74, 16);
      this.m_chkLineFillArea.TabIndex = 6;
      this.m_chkLineFillArea.Text = "Fill Area";
      this.m_chkLineFillArea.CheckedChanged += new System.EventHandler(this.EhLineFillArea_CheckedChanged);
      // 
      // m_cbLineConnect
      // 
      this.m_cbLineConnect.Location = new System.Drawing.Point(96, 6);
      this.m_cbLineConnect.Name = "m_cbLineConnect";
      this.m_cbLineConnect.Size = new System.Drawing.Size(121, 21);
      this.m_cbLineConnect.TabIndex = 3;
      this.m_cbLineConnect.Text = "comboBox1";
      this.m_cbLineConnect.SelectionChangeCommitted += new System.EventHandler(this.m_cbLineConnect_SelectionChangeCommitted);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(5, 10);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(48, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Connect";
      // 
      // _chkIndependentColor
      // 
      this._chkIndependentColor.Location = new System.Drawing.Point(8, 76);
      this._chkIndependentColor.Name = "_chkIndependentColor";
      this._chkIndependentColor.Size = new System.Drawing.Size(45, 16);
      this._chkIndependentColor.TabIndex = 24;
      this._chkIndependentColor.Text = "ind.";
      this._chkIndependentColor.CheckedChanged += new System.EventHandler(this._chkIndependentColor_CheckedChanged);
      // 
      // m_chkLineSymbolGap
      // 
      this.m_chkLineSymbolGap.Location = new System.Drawing.Point(95, 45);
      this.m_chkLineSymbolGap.Name = "m_chkLineSymbolGap";
      this.m_chkLineSymbolGap.Size = new System.Drawing.Size(112, 24);
      this.m_chkLineSymbolGap.TabIndex = 22;
      this.m_chkLineSymbolGap.Text = "Line/Symbol Gap";
      // 
      // XYPlotLineStyleControl
      // 
      this.Controls.Add(this._chkIndependentColor);
      this.Controls.Add(this._chkConnectCircular);
      this.Controls.Add(this._chkIndependentFillColor);
      this.Controls.Add(this._ctrlColorTypeThickness);
      this.Controls.Add(this.m_chkLineSymbolGap);
      this.Controls.Add(this.m_cbLineFillColor);
      this.Controls.Add(this.m_cbLineFillDirection);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.m_cbLineConnect);
      this.Controls.Add(this.m_chkLineFillArea);
      this.Name = "XYPlotLineStyleControl";
      this.Size = new System.Drawing.Size(226, 271);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion


    public void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      ++m_SuppressEvents;
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
      --m_SuppressEvents;
    }

    public void InitComboBox(System.Windows.Forms.ComboBox box, List<ListNode> names, int sel)
    {
      ++m_SuppressEvents;
      box.Items.Clear();
      box.Items.AddRange(names.ToArray());
      
      if(sel>=0 && sel<box.Items.Count)
        box.SelectedIndex = sel;

      --m_SuppressEvents;
    }

    public void EnableDisableMain(bool bEnable)
    {
      this._chkIndependentColor.Enabled = bEnable;
      
      this._ctrlColorTypeThickness.Enabled = bEnable;
      this.m_cbLineFillColor.Enabled = bEnable;
      this.m_chkLineSymbolGap.Enabled = bEnable;
    }

    bool ShouldEnableMain()
    {
      return this.m_cbLineConnect.SelectedIndex!=0 || this.m_chkLineFillArea.Checked;
    }

    public void SetEnableDisableMain(bool bActivate)
    {
      this._EnableDisableAll = bActivate;
      this.EnableDisableMain(_EnableDisableAll==false || this.ShouldEnableMain());
    }

    #region IXYPlotLineStyleView Members

    public IXYPlotLineStyleViewEventSink Controller
    {
      get { return _controller; }
      set { _controller = value; }
    }

    public void InitializeLineSymbolGapCondition(bool bGap)
    {
      ++m_SuppressEvents;
      this.m_chkLineSymbolGap.Checked = bGap;
      --m_SuppressEvents;
    }

    public bool LineSymbolGap
    {
      get { return m_chkLineSymbolGap.Checked; }
    }

    public void InitializePen(IColorTypeThicknessPenController controller)
    {
      controller.ViewObject = this._ctrlColorTypeThickness;
    }

  
 

    public void InitializeLineConnect(string[] arr, string sel)
    {
      InitComboBox(this.m_cbLineConnect,arr,sel);
    }
    public string LineConnect
    {
      get { return (string)m_cbLineConnect.SelectedItem; }
    }
    
  

    public void InitializeFillCondition(bool bFill)
    {
      this.m_chkLineFillArea.Checked = bFill;
      this.m_cbLineFillColor.Enabled = bFill;
      this.m_cbLineFillDirection.Enabled = bFill;
    }

    public bool LineFillArea 
    {
      get { return m_chkLineFillArea.Checked; }
    }

  


    private void EhLineFillArea_CheckedChanged(object sender, System.EventArgs e)
    {
      bool bFill = m_chkLineFillArea.Checked;
      this.m_cbLineFillColor.Enabled = bFill;
      this.m_cbLineFillDirection.Enabled = bFill;

      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());
    }

   

    public void InitializeFillColor(BrushX sel)
    {
      m_cbLineFillColor.Brush = sel;
    }

    public BrushX LineFillColor
    {
      get { return m_cbLineFillColor.Brush; }
    }
  
    public void InitializeFillDirection(List<ListNode> list, int sel)
    {
      InitComboBox(this.m_cbLineFillDirection,list,sel);
    }

       
      
    public ListNode LineFillDirection
    {
      get { return (ListNode)m_cbLineFillDirection.SelectedItem; }
    }
  
    public void InitializeIndependentColor(bool val)
    {
      this._chkIndependentColor.Checked = val;
      _chkIndependentColor_CheckedChanged(_chkIndependentColor.Checked,EventArgs.Empty);
    }

    public bool IndependentColor 
    {
      get
      {
        return this._chkIndependentColor.Checked; 
      }
    }

    public bool ConnectCircular 
    {
      get { return _chkConnectCircular.Checked; }
      set { _chkConnectCircular.Checked = value; }
    }

    public bool IndependentFillColor
    {
      get { return _chkIndependentFillColor.Checked; }
      set { _chkIndependentFillColor.Checked = value; }
    }

    private void _chkIndependentColor_CheckedChanged(object sender, System.EventArgs e)
    {
     
    }


       


    #endregion

    private void m_cbLineConnect_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if(this._EnableDisableAll)
        EnableDisableMain(this.ShouldEnableMain());
    }
  }
}
