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

using Altaxo.Graph;

namespace Altaxo.Gui.Common.Drawing
{
  public class PenControlsGlue : Component
  {
    private bool _userChangedStartCapSize;
    private bool _userChangedEndCapSize;


    PenHolder _pen;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PenHolder Pen
    {
      get
      {
      //  if (_pen != null)
      //    GetGuiElementValues(_pen);
        return _pen;
      }
      set
      {
        _pen = value;

        BrushGlue = _brushGlue;
        CbColor = _cbColor;
        CbLineThickness = _cbThickness;
        CbDashStyle = _cbDashStyle;
        CbDashCap = _cbDashCap;
        CbStartCap = _cbStartCap;
        CbStartCapSize = _cbStartCapSize;
        CbEndCap = _cbEndCap;
        CbEndCapSize = _cbEndCapSize;
        CbLineJoin = _cbLineJoin;
        CbMiterLimit = _cbMiterLimit;
      }
    }

    public event EventHandler PenChanged;
    protected virtual void OnPenChanged()
    {
      if (PenChanged != null)
        PenChanged(this, EventArgs.Empty);
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

        if (_cbColor != null)
          _cbColor.ColorType = _colorType;
        if (_brushGlue != null)
          _brushGlue.ColorType = _colorType;
      }
    }


   
    BrushControlsGlue _brushGlue;
    public BrushControlsGlue BrushGlue
    {
      get { return _brushGlue; }
      set
      {
        if (_brushGlue != null)
          _brushGlue.BrushChanged -= EhBrushChanged;
        
        _brushGlue = value;
        if (_brushGlue != null)
          _brushGlue.ColorType = _colorType;
        if (_pen != null && _brushGlue != null)
          _brushGlue.Brush = _pen.BrushHolder;


        if (_brushGlue != null)
          _brushGlue.BrushChanged += EhBrushChanged;

      }
    }

    void EhBrushChanged(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.BrushHolder = _brushGlue.Brush;
        OnPenChanged();
      }
    }

    ColorComboBox _cbColor;
    public ColorComboBox CbColor
    {
      get { return _cbColor; }
      set
      {
        if (_cbColor != null)
          _cbColor.SelectionChangeCommitted -= EhColor_SelectionChangeCommitted;

        _cbColor = value;
        if (_cbColor != null)
          _cbColor.ColorType = _colorType;
        if (_pen != null && _cbColor != null)
          _cbColor.Color = _pen.Color;

        if (_cbColor != null)
        {
          _cbColor.SelectionChangeCommitted += EhColor_SelectionChangeCommitted;

          System.Windows.Forms.ToolStripItem tsi = new System.Windows.Forms.ToolStripMenuItem();
          tsi.Text = "Custom Pen ...";
          tsi.Click += EhShowCustomPenDialog;
          _cbColor.ContextMenuStrip.Items.Insert(0, tsi);
        }


      }
    }

    void EhColor_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.Color = _cbColor.Color;
        OnPenChanged();
      }
    }


    DashStyleComboBox _cbDashStyle;
    public DashStyleComboBox CbDashStyle
    {
      get { return _cbDashStyle; }
      set 
      {
        if (_cbDashStyle != null)
          _cbDashStyle.SelectionChangeCommitted -= EhDashStyle_SelectionChangeCommitted;

        _cbDashStyle = value;
        if (_pen != null && _cbDashStyle != null)
          _cbDashStyle.DashStyleEx = _pen.DashStyleEx;

        if (_cbDashStyle != null)
          _cbDashStyle.SelectionChangeCommitted += EhDashStyle_SelectionChangeCommitted;
      }
    }

    void EhDashStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.DashStyleEx = _cbDashStyle.DashStyleEx;
        OnPenChanged();
      }
    }

    DashCapComboBox _cbDashCap;
    public DashCapComboBox CbDashCap
    {
      get { return _cbDashCap; }
      set 
      {
        if (_cbDashCap != null)
          _cbDashCap.SelectionChangeCommitted -= EhDashCap_SelectionChangeCommitted;

        _cbDashCap = value;
        if (_pen != null && _cbDashCap != null)
          _cbDashCap.DashCap = _pen.DashCap;

        if (_cbDashCap != null)
          _cbDashCap.SelectionChangeCommitted += EhDashCap_SelectionChangeCommitted;
      }
    }

    void EhDashCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.DashCap = _cbDashCap.DashCap;
        OnPenChanged();
      }
    }

    LineThicknessComboBox _cbThickness;
    public LineThicknessComboBox CbLineThickness
    {
      get { return _cbThickness; }
      set 
      {
        if (_cbThickness != null)
        {
          _cbThickness.SelectionChangeCommitted -= EhThickness_SelectionChangeCommitted;
          _cbThickness.TextUpdate -= EhThickness_TextChanged;
        }

        _cbThickness = value;
        if (_pen != null && _cbThickness != null)
          _cbThickness.Thickness = _pen.Width;

        if (_cbThickness != null)
        {
          _cbThickness.SelectionChangeCommitted += EhThickness_SelectionChangeCommitted;
          _cbThickness.TextUpdate += EhThickness_TextChanged;
        }
      }
    }

    void EhThickness_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.Width = _cbThickness.Thickness;
        OnPenChanged();
      }
    }
    void EhThickness_TextChanged(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.Width = _cbThickness.Thickness;
        OnPenChanged();
      }
    }

    LineCapComboBox _cbStartCap;

    public LineCapComboBox CbStartCap
    {
      get { return _cbStartCap; }
      set 
      {
        if (_cbStartCap != null)
          _cbStartCap.SelectionChangeCommitted -= EhStartCap_SelectionChangeCommitted;

        _cbStartCap = value;
        if (_pen != null && _cbStartCap != null)
          _cbStartCap.LineCapEx = _pen.StartCap;

        if (_cbStartCap != null)
          _cbStartCap.SelectionChangeCommitted += EhStartCap_SelectionChangeCommitted;
      }
    }

    void EhStartCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_cbStartCapSize != null)
      {
        if (!_userChangedStartCapSize)
          _cbStartCapSize.Thickness = _cbStartCap.LineCapEx.Size;
      }
    }

    LineCapComboBox _cbEndCap;

    public LineCapComboBox CbEndCap
    {
      get { return _cbEndCap; }
      set 
      {
        if (_cbEndCap != null)
          _cbEndCap.SelectionChangeCommitted -= EhEndCap_SelectionChangeCommitted;

        _cbEndCap = value;
        if (_pen != null && _cbEndCap != null)
          _cbEndCap.LineCapEx = _pen.EndCap;

        if (_cbEndCap != null)
          _cbEndCap.SelectionChangeCommitted += EhEndCap_SelectionChangeCommitted;
      }
    }

  

    void EhEndCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_cbEndCapSize != null)
      {
        if (!_userChangedEndCapSize)
          _cbEndCapSize.Thickness = _cbEndCap.LineCapEx.Size;
      }
    }

    LineCapSizeComboBox _cbStartCapSize;

    public LineCapSizeComboBox CbStartCapSize
    {
      get { return _cbStartCapSize; }
      set 
      {
        if (_cbStartCapSize != null)
        {

          _cbStartCapSize.SelectionChangeCommitted -= EhStartCapSize_SelectionChangeCommitted;
          _cbStartCapSize.TextUpdate -= EhStartCapSize_TextChanged;
        }
        _cbStartCapSize = value;
        if (_pen != null && _cbStartCapSize != null)
          _cbStartCapSize.Thickness = _pen.StartCap.Size;

        if (_cbStartCapSize != null)
        {
          _cbStartCapSize.SelectionChangeCommitted += EhStartCapSize_SelectionChangeCommitted;
          _cbStartCapSize.TextUpdate += EhStartCapSize_TextChanged;
        }
      }
    }

    void EhStartCapSize_SelectionChangeCommitted(object sender, EventArgs e)
    {
      _userChangedStartCapSize = true;

    }
    void EhStartCapSize_TextChanged(object sender, EventArgs e)
    {
      _userChangedStartCapSize = true;
    }

    LineCapSizeComboBox _cbEndCapSize;

    public LineCapSizeComboBox CbEndCapSize
    {
      get { return _cbEndCapSize; }
      set 
      {
        if (_cbEndCapSize != null)
        {
          _cbEndCapSize.SelectionChangeCommitted -= EhEndCapSize_SelectionChangeCommitted;
          _cbEndCapSize.TextUpdate -= EhEndCapSize_TextChanged;
        }

        _cbEndCapSize = value;
        if (_pen != null && _cbEndCapSize != null)
          _cbEndCapSize.Thickness = _pen.EndCap.Size;

        if (_cbEndCapSize != null)
        {
          _cbEndCapSize.SelectionChangeCommitted += EhEndCapSize_SelectionChangeCommitted;
          _cbEndCapSize.TextUpdate += EhEndCapSize_TextChanged;
        }
      }
    }

    void EhEndCapSize_SelectionChangeCommitted(object sender, EventArgs e)
    {
      _userChangedEndCapSize = true;

    }
    void EhEndCapSize_TextChanged(object sender, EventArgs e)
    {
      _userChangedEndCapSize = true;
    }


    LineJoinComboBox _cbLineJoin;

    public LineJoinComboBox CbLineJoin
    {
      get { return _cbLineJoin; }
      set 
      {
        if (_cbLineJoin != null)
          _cbLineJoin.SelectionChangeCommitted -= EhLineJoin_SelectionChangeCommitted;
        

        _cbLineJoin = value;
        if (_pen != null && _cbLineJoin != null)
          _cbLineJoin.LineJoin = _pen.LineJoin;

        if (_cbLineJoin != null)
          _cbLineJoin.SelectionChangeCommitted += EhLineJoin_SelectionChangeCommitted;
         
      }
    }

    void EhLineJoin_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.LineJoin = _cbLineJoin.LineJoin;
        OnPenChanged();
      }
    }
    MiterLimitComboBox _cbMiterLimit;

    public MiterLimitComboBox CbMiterLimit
    {
      get { return _cbMiterLimit; }
      set
      {
        if (_cbMiterLimit != null)
        {
          _cbMiterLimit.SelectionChangeCommitted -= EhMiterLimit_SelectionChangeCommitted;
          _cbMiterLimit.TextUpdate -= EhMiterLimit_TextChanged;
        }

        _cbMiterLimit = value;
        if (_pen != null && _cbMiterLimit != null)
          _cbMiterLimit.MiterValue = _pen.MiterLimit;

        if (_cbLineJoin != null)
        {
          _cbMiterLimit.SelectionChangeCommitted += EhMiterLimit_SelectionChangeCommitted;
          _cbMiterLimit.TextUpdate += EhMiterLimit_TextChanged;
        }
      }
    }

    void EhMiterLimit_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.MiterLimit = _cbMiterLimit.MiterValue;
        OnPenChanged();
      }
    }
    void EhMiterLimit_TextChanged(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.MiterLimit = _cbMiterLimit.MiterValue;
        OnPenChanged();
      }
    }




    void EhShowCustomPenDialog(object sender, EventArgs e)
    {
      PenAllPropertiesControl ctrl = new PenAllPropertiesControl();
      ctrl.Pen = this.Pen;
      if (Current.Gui.ShowDialog(ctrl, "Pen properties"))
      {
        this.Pen = ctrl.Pen;
      }
    }

   



    /*
        void SetGuiElementValues(PenHolder doc)
        {
          if (_cbColor != null)
            _cbColor.Color = doc.Color;

          if (_cbDashCap != null)
            _cbDashCap.DashCap = doc.DashCap;

          if (_cbDashStyle != null)
            _cbDashStyle.DashStyleEx = doc.DashStyleEx;

          if (_cbEndCap != null)
            _cbEndCap.LineCapEx = doc.EndCap;

          if (_cbEndCapSize != null)
            _cbEndCapSize.Thickness = doc.EndCap.Size;

          if (_cbStartCap != null)
            _cbStartCap.LineCapEx = doc.StartCap;

          if (_cbStartCapSize != null)
            _cbStartCapSize.Thickness = doc.StartCap.Size;

          if (_cbThickness != null)
            _cbThickness.Thickness = doc.Width;

          if (_cbLineJoin != null)
            _cbLineJoin.LineJoin = doc.LineJoin;

          if (_cbMiterLimit != null)
            _cbMiterLimit.MiterValue = doc.MiterLimit;
        }


        void GetGuiElementValues(PenHolder doc)
        {
          if (_cbColor != null)
            doc.Color = _cbColor.Color;
          if (_cbThickness != null)
            doc.Width = _cbThickness.Thickness;

          if (_cbDashStyle != null)
            doc.DashStyleEx = _cbDashStyle.DashStyleEx;

          if (_cbDashCap != null)
            doc.DashCap = _cbDashCap.DashCap;

          if (_cbStartCap != null)
          {
            LineCapEx startCap = _cbStartCap.LineCapEx;

            if (_cbStartCapSize != null)
              startCap.Size = _cbStartCapSize.Thickness;
            doc.StartCap = startCap;
          }
          if (_cbEndCap != null)
          {
            LineCapEx endCap = _cbEndCap.LineCapEx;
            if (_cbEndCapSize != null)
              endCap.Size = _cbEndCapSize.Thickness;
            doc.EndCap = endCap;
          }
        }
     */
  }
}
