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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public class BrushControlsGlue : Component
  {
    public BrushControlsGlue()
    {
      ToolStripMenuItem it = new ToolStripMenuItem();
      it.Text = "Custom Brush ...";
      it.Click += EhShowCustomBrushDialog;

      InsertContextMenuItem(it);
    }

    void EhShowCustomBrushDialog(object sender, EventArgs e)
    {
      BrushAllPropertiesControl ctrl = new BrushAllPropertiesControl();
      ctrl.Brush = this.Brush;
      if (Current.Gui.ShowDialog(ctrl, "Brush properties"))
      {
        this.Brush = ctrl.Brush;
        OnBrushChanged();
      }
    }

    BrushX _brush;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BrushX Brush
    {
      get { return _brush; }
      set 
      {
        _brush = value;

        if (_brush != null)
        {
          CbBrushType = _cbBrushType;
          CbHatchStyle = _cbHatchStyle;
          CbColor1 = _cbColor1;
          CbColor2 = _cbColor2;
          ChkExchangeColors = _chkExchangeColors;
          CbGradientMode = _cbGradientMode;
          CbGradientShape = _cbGradientShape;
          CbWrapMode = _cbWrapMode;
          CbGradientFocus = _cbGradientFocus;
          CbGradientScale = _cbGradientScale;
          CbTextureImage = _cbTextureImage;
          CbTextureScale = _cbTextureScale;
        }
      }
    }
    public event EventHandler BrushChanged;
    protected virtual void OnBrushChanged()
    {
      if (BrushChanged != null)
        BrushChanged(this, EventArgs.Empty);
    }


    ColorType _colorType = ColorType.KnownAndSystemColor;
    public ColorType ColorType
    {
      get
      {
        return _colorType;
      }
      set
      {
        _colorType = value;
        if (_cbColor1 != null)
          _cbColor1.ColorType = value; // only for color1
      }
    }

    #region BrushType
    BrushTypeComboBox _cbBrushType;

    public BrushTypeComboBox CbBrushType
    {
      get { return _cbBrushType; }
      set 
      {
        if (null != _cbBrushType)
          _cbBrushType.SelectionChangeCommitted -= EhBrushType_SelectionChangeCommitted;
        
        _cbBrushType = value;
        if (_brush != null && CbBrushType!=null)
          _cbBrushType.BrushType = _brush.BrushType;

        if (null != _cbBrushType)
          _cbBrushType.SelectionChangeCommitted += EhBrushType_SelectionChangeCommitted;
      
      
      }
    }

    void EhBrushType_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.BrushType = _cbBrushType.BrushType;
        
        OnBrushChanged();

        UpdateColor2State();
        UpdateExchangeColorsState();
        UpdateHatchStyleState();
        UpdateWrapModeState();
        UpdateGradientModeState();
        UpdateGradientShapeState();
        UpdateGradientFocusState();
        UpdateGradientScaleState();
        UpdateTextureImageState();
        UpdateTextureScaleState();
      }
    }

    #endregion

    #region HatchStyle
    HatchStyleComboBox _cbHatchStyle;
    public HatchStyleComboBox CbHatchStyle
    {
      get { return _cbHatchStyle; }
      set 
      {
        if (_cbHatchStyle != null)
          _cbHatchStyle.SelectionChangeCommitted -= EhHatchStyle_SelectionChangeCommitted;

        _cbHatchStyle = value;
        if (_brush != null && _cbHatchStyle!=null)
          _cbHatchStyle.HatchStyle = _brush.HatchStyle;

        if (_cbHatchStyle != null)
          _cbHatchStyle.SelectionChangeCommitted += EhHatchStyle_SelectionChangeCommitted;

        UpdateHatchStyleState();
      
      }
    }

    void EhHatchStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.HatchStyle = _cbHatchStyle.HatchStyle;
        OnBrushChanged();
      }
    }

    Control _lblHatchStyle;
    public Control LabelHatchStyle
    {
      get
      {
        return _lblHatchStyle;
      }
      set
      {
        _lblHatchStyle = value;
        UpdateHatchStyleState();
      }
    }
    void UpdateHatchStyleState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.HatchBrush);
        if (_lblHatchStyle != null)
          _lblHatchStyle.Visible = vis;
        if (_cbHatchStyle != null)
          _cbHatchStyle.Visible = vis;
      }
    }

    #endregion

    #region Color1

    ColorComboBox _cbColor1;
    public ColorComboBox CbColor1
    {
      get { return _cbColor1; }
      set 
      {
        if (_cbColor1 != null)
        {
          _cbColor1.ColorChoiceChanged -= EhColor1_ColorChoiceChanged;

          foreach (ToolStripItem item in _customContextMenuItems)
            _cbColor1.ContextMenuStrip.Items.Remove(item);
        }

        _cbColor1 = value;
        if (_cbColor1 != null)
          _cbColor1.ColorType = _colorType;
        if (_brush != null && _cbColor1 != null)
          _cbColor1.ColorChoice = _brush.Color;

        if (_cbColor1 != null)
        {
          _cbColor1.ColorChoiceChanged += EhColor1_ColorChoiceChanged;


          foreach (ToolStripItem item in _customContextMenuItems)
            _cbColor1.ContextMenuStrip.Items.Insert(0, item);
        }
      }
    }

    void EhColor1_ColorChoiceChanged(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.Color = _cbColor1.ColorChoice;
        OnBrushChanged();
      }
    }

    #endregion

    #region Color2
    ColorComboBox _cbColor2;

    public ColorComboBox CbColor2
    {
      get { return _cbColor2; }
      set 
      {
        if (_cbColor2 != null)
          _cbColor2.ColorChoiceChanged -= EhColor2_ColorChoiceChanged;

        _cbColor2 = value;
        if (_brush != null && _cbColor2 != null)
          _cbColor2.ColorChoice = _brush.BackColor;

        if (_cbColor2 != null)
          _cbColor2.ColorChoiceChanged += EhColor2_ColorChoiceChanged;

        UpdateColor2State();
      }
    }

    void EhColor2_ColorChoiceChanged(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.BackColor = _cbColor2.ColorChoice;
        OnBrushChanged();
      }
    }


    Control _lblColor2;
    public Control LabelColor2
    {
      get
      {
        return _lblColor2;
      }
      set
      {
        _lblColor2 = value;
        UpdateColor2State();
      }
    }
    void UpdateColor2State()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype != BrushType.SolidBrush) && (btype != BrushType.TextureBrush);
        if (_lblColor2 != null)
          _lblColor2.Visible = vis;
        if (_cbColor2 != null)
          _cbColor2.Visible = vis;
      }
    }


    #endregion

    #region ExchangeColors
    CheckBox _chkExchangeColors;

    public CheckBox ChkExchangeColors
    {
      get { return _chkExchangeColors; }
      set
      {
        if (_chkExchangeColors != null)
          _chkExchangeColors.CheckedChanged -= EhChkExchangeColors_CheckedChanged;

        _chkExchangeColors = value;
        if (_brush != null && _chkExchangeColors!=null)
          _chkExchangeColors.Checked = _brush.ExchangeColors;

        if (_chkExchangeColors != null)
          _chkExchangeColors.CheckedChanged += EhChkExchangeColors_CheckedChanged;

        UpdateExchangeColorsState();
      }
    }

    void EhChkExchangeColors_CheckedChanged(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.ExchangeColors = _chkExchangeColors.Checked;
        OnBrushChanged();
      }
    }


    Control _lblExchangeColors;
    public Control LabelExchangeColors
    {
      get
      {
        return _lblExchangeColors;
      }
      set
      {
        _lblExchangeColors = value;
        UpdateExchangeColorsState();
      }
    }
    void UpdateExchangeColorsState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.HatchBrush) || (btype == BrushType.LinearGradientBrush) || (btype==BrushType.PathGradientBrush);
        if (_lblExchangeColors != null)
          _lblExchangeColors.Visible = vis;
        if (_chkExchangeColors != null)
          _chkExchangeColors.Visible = vis;
      }
    }


    #endregion

    #region Wrap Mode
    WrapModeComboBox _cbWrapMode;
    public WrapModeComboBox CbWrapMode
    {
      get { return _cbWrapMode; }
      set
      {
        if (_cbWrapMode != null)
          _cbWrapMode.SelectionChangeCommitted -= EhWrapMode_SelectionChangeCommitted;

        _cbWrapMode = value;
        if (_brush != null && _cbWrapMode != null)
          _cbWrapMode.WrapMode = _brush.WrapMode;

        if (_cbWrapMode != null)
          _cbWrapMode.SelectionChangeCommitted += EhWrapMode_SelectionChangeCommitted;

        UpdateWrapModeState();

      }
    }

    void EhWrapMode_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.WrapMode = _cbWrapMode.WrapMode;
        OnBrushChanged();
      }
    }

    Control _lblWrapMode;
    public Control LabelWrapMode
    {
      get
      {
        return _lblWrapMode;
      }
      set
      {
        _lblWrapMode = value;
        UpdateWrapModeState();
      }
    }
    void UpdateWrapModeState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.LinearGradientBrush || btype == BrushType.PathGradientBrush || btype == BrushType.TextureBrush);
        if (_lblWrapMode != null)
          _lblWrapMode.Visible = vis;
        if (_cbWrapMode != null)
          _cbWrapMode.Visible = vis;
      }
    }
    #endregion

    #region Gradient Mode

    LinearGradientModeComboBox _cbGradientMode;
    public LinearGradientModeComboBox CbGradientMode
    {
      get { return _cbGradientMode; }
      set
      {
        if (_cbGradientMode != null)
          _cbGradientMode.SelectionChangeCommitted -= EhGradientMode_SelectionChangeCommitted;

        _cbGradientMode = value;
        if (_brush != null && _cbGradientMode != null)
          _cbGradientMode.LinearGradientMode = _brush.GradientMode;

        if (_cbGradientMode != null)
          _cbGradientMode.SelectionChangeCommitted += EhGradientMode_SelectionChangeCommitted;

        UpdateGradientModeState();

      }
    }

    void EhGradientMode_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.GradientMode = _cbGradientMode.LinearGradientMode;
        OnBrushChanged();
      }
    }

    Control _lblGradientMode;
    public Control LabelGradientMode
    {
      get
      {
        return _lblGradientMode;
      }
      set
      {
        _lblGradientMode = value;
        UpdateGradientModeState();
      }
    }
    void UpdateGradientModeState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.LinearGradientBrush);
        if (_lblGradientMode != null)
          _lblGradientMode.Visible = vis;
        if (_cbGradientMode != null)
          _cbGradientMode.Visible = vis;
      }
    }

    #endregion

    #region Gradient Shape
    LinearGradientShapeComboBox _cbGradientShape;
    public LinearGradientShapeComboBox CbGradientShape
    {
      get { return _cbGradientShape; }
      set
      {
        if (_cbGradientShape != null)
          _cbGradientShape.SelectionChangeCommitted -= EhGradientShape_SelectionChangeCommitted;

        _cbGradientShape = value;
        if (_brush != null && _cbGradientShape != null)
          _cbGradientShape.LinearGradientShape = _brush.GradientShape;

        if (_cbWrapMode != null)
          _cbGradientShape.SelectionChangeCommitted += EhGradientShape_SelectionChangeCommitted;

        UpdateGradientShapeState();

      }
    }

    void EhGradientShape_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.GradientShape = _cbGradientShape.LinearGradientShape;
        OnBrushChanged();
      }
    }

    Control _lblGradientShape;
    public Control LabelGradientShape
    {
      get
      {
        return _lblGradientShape;
      }
      set
      {
        _lblGradientShape = value;
        UpdateGradientShapeState();
      }
    }
    void UpdateGradientShapeState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.LinearGradientBrush);
        if (_lblGradientShape != null)
          _lblGradientShape.Visible = vis;
        if (_cbGradientShape != null)
          _cbGradientShape.Visible = vis;
      }
    }
    #endregion

    #region Gradient Focus

    GradientFocusComboBox _cbGradientFocus;
    public GradientFocusComboBox CbGradientFocus
    {
      get { return _cbGradientFocus; }
      set
      {
        if (_cbGradientFocus != null)
        {
          _cbGradientFocus.SelectionChangeCommitted -= EhGradientFocus_SelectionChangeCommitted;
          _cbGradientFocus.TextUpdate -= EhGradientFocus_TextChanged;
        }

        _cbGradientFocus = value;
        if (_brush != null && _cbGradientFocus != null)
          _cbGradientFocus.GradientFocus = _brush.GradientFocus;

        if (_cbGradientFocus != null)
        {
          _cbGradientFocus.SelectionChangeCommitted += EhGradientFocus_SelectionChangeCommitted;
          _cbGradientFocus.TextUpdate += EhGradientFocus_TextChanged;
        }

        UpdateGradientFocusState();
      }
    }

    void EhGradientFocus_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.GradientFocus = _cbGradientFocus.GradientFocus;
        OnBrushChanged();
      }
    }
    void EhGradientFocus_TextChanged(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.GradientFocus = _cbGradientFocus.GradientFocus;
        OnBrushChanged();
      }
    }

    Control _lblGradientFocus;
    public Control LabelGradientFocus
    {
      get
      {
        return _lblGradientFocus;
      }
      set
      {
        _lblGradientFocus = value;
        UpdateGradientFocusState();
      }
    }
    void UpdateGradientFocusState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.LinearGradientBrush);
        if (_lblGradientFocus != null)
          _lblGradientFocus.Visible = vis;
        if (_cbGradientFocus != null)
          _cbGradientFocus.Visible = vis;
      }
    }
    #endregion

    #region Gradient Scale
    GradientScaleComboBox _cbGradientScale;
    public GradientScaleComboBox CbGradientScale
    {
      get { return _cbGradientScale; }
      set
      {
        if (_cbGradientScale != null)
        {
          _cbGradientScale.SelectionChangeCommitted -= EhGradientScale_SelectionChangeCommitted;
          _cbGradientScale.TextUpdate -= EhGradientScale_TextChanged;
        }

        _cbGradientScale = value;
        if (_brush != null && _cbGradientScale != null)
          _cbGradientScale.GradientScale = _brush.GradientScale;

        if (_cbGradientScale != null)
        {
          _cbGradientScale.SelectionChangeCommitted += EhGradientScale_SelectionChangeCommitted;
          _cbGradientScale.TextUpdate += EhGradientScale_TextChanged;
        }

        UpdateGradientScaleState();
      }
    }

    void EhGradientScale_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.GradientScale = _cbGradientScale.GradientScale;
        OnBrushChanged();
      }
    }
    void EhGradientScale_TextChanged(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.GradientScale = _cbGradientScale.GradientScale;
        OnBrushChanged();
      }
    }

    Control _lblGradientScale;
    public Control LabelGradientScale
    {
      get
      {
        return _lblGradientScale;
      }
      set
      {
        _lblGradientScale = value;
        UpdateGradientScaleState();
      }
    }
    void UpdateGradientScaleState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.LinearGradientBrush);
        if (_lblGradientScale != null)
          _lblGradientScale.Visible = vis;
        if (_cbGradientScale != null)
          _cbGradientScale.Visible = vis;
      }
    }
    #endregion

    #region Texture Image

    TextureImageComboBox _cbTextureImage;
    public TextureImageComboBox CbTextureImage
    {
      get { return _cbTextureImage; }
      set
      {
        if (_cbTextureImage != null)
          _cbTextureImage.SelectionChangeCommitted -= EhTextureImage_SelectionChangeCommitted;

        _cbTextureImage = value;
        if (_brush != null && _cbTextureImage != null)
          _cbTextureImage.ChoosenTexture = _brush.TextureImage;

        if (_cbTextureImage != null)
          _cbTextureImage.SelectionChangeCommitted += EhTextureImage_SelectionChangeCommitted;

        UpdateTextureImageState();

      }
    }

    void EhTextureImage_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.TextureImage = _cbTextureImage.ChoosenTexture;
        OnBrushChanged();
      }
    }

    Control _lblTextureImage;
    public Control LabelTextureImage
    {
      get
      {
        return _lblTextureImage;
      }
      set
      {
        _lblTextureImage = value;
        UpdateTextureImageState();
      }
    }
    void UpdateTextureImageState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.TextureBrush);
        if (_lblTextureImage != null)
          _lblTextureImage.Visible = vis;
        if (_cbTextureImage != null)
          _cbTextureImage.Visible = vis;
      }
    }

    #endregion

    #region Texture Scale
    TextureScaleComboBox _cbTextureScale;
    public TextureScaleComboBox CbTextureScale
    {
      get { return _cbTextureScale; }
      set
      {
        if (_cbTextureScale != null)
        {
          _cbTextureScale.SelectionChangeCommitted -= EhTextureScale_SelectionChangeCommitted;
          _cbTextureScale.TextUpdate -= EhTextureScale_TextChanged;
        }

        _cbTextureScale = value;
        if (_brush != null && _cbTextureScale != null)
          _cbTextureScale.TextureScale = _brush.TextureScale;

        if (_cbTextureScale != null)
        {
          _cbTextureScale.SelectionChangeCommitted += EhTextureScale_SelectionChangeCommitted;
          _cbTextureScale.TextUpdate += EhTextureScale_TextChanged;
        }

        UpdateTextureScaleState();
      }
    }

    void EhTextureScale_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.TextureScale = _cbTextureScale.TextureScale;
        OnBrushChanged();
      }
    }
    void EhTextureScale_TextChanged(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.TextureScale = _cbTextureScale.TextureScale;
        OnBrushChanged();
      }
    }

    Control _lblTextureScale;
    public Control LabelTextureScale
    {
      get
      {
        return _lblTextureScale;
      }
      set
      {
        _lblTextureScale = value;
        UpdateTextureScaleState();
      }
    }
    void UpdateTextureScaleState()
    {
      if (_brush != null)
      {
        BrushType btype = _brush.BrushType;
        bool vis = (btype == BrushType.TextureBrush);
        if (_lblTextureScale != null)
          _lblTextureScale.Visible = vis;
        if (_cbTextureScale != null)
          _cbTextureScale.Visible = vis;
      }
    }
    #endregion

    #region Context Menu
    List<ToolStripItem> _customContextMenuItems = new List<ToolStripItem>();

    public void InsertContextMenuItem(ToolStripItem item)
    {
      if(_customContextMenuItems.Contains(item))
        return;

      _customContextMenuItems.Add(item);

      if(_cbColor1!=null)
        _cbColor1.ContextMenuStrip.Items.Insert(0,item);
    }
    public void RemoveContextMenuItem(ToolStripItem item)
    {
      if(!_customContextMenuItems.Contains(item))
        return;

      _customContextMenuItems.Remove(item);

      if(_cbColor1!=null)
        _cbColor1.ContextMenuStrip.Items.Remove(item);
    }

    #endregion

  }
}
