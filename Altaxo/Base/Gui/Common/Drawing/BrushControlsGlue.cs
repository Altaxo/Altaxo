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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Graph;

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

    BrushHolder _brush;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BrushHolder Brush
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
        UpdateHatchStyleState();
      }
    }


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
          _cbColor1.SelectionChangeCommitted -= EhColor1_SelectionChangeCommitted;

          foreach (ToolStripItem item in _customContextMenuItems)
            _cbColor1.ContextMenuStrip.Items.Remove(item);
        }

        _cbColor1 = value;
        if (_cbColor1 != null)
          _cbColor1.ColorType = _colorType;
        if (_brush != null && _cbColor1 != null)
          _cbColor1.Color = _brush.Color;

        if (_cbColor1 != null)
        {
          _cbColor1.SelectionChangeCommitted += EhColor1_SelectionChangeCommitted;


          foreach (ToolStripItem item in _customContextMenuItems)
            _cbColor1.ContextMenuStrip.Items.Insert(0, item);
        }
      }
    }

    void EhColor1_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.Color = _cbColor1.Color;
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
          _cbColor2.SelectionChangeCommitted -= EhColor2_SelectionChangeCommitted;

        _cbColor2 = value;
        if (_brush != null && _cbColor2 != null)
          _cbColor2.Color = _brush.BackColor;

        if (_cbColor2 != null)
          _cbColor2.SelectionChangeCommitted += EhColor2_SelectionChangeCommitted;

        UpdateColor2State();
      }
    }

    void EhColor2_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_brush != null)
      {
        _brush.BackColor = _cbColor2.Color;
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

  }
}
