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
using System.Drawing;

using Altaxo.Serialization;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph
{
  public class ObjectPositionAndSizeGlue : Component
  {
    #region Input/Output
    static void SetPositionText(TextBox t, double value)
    {
      if(t!=null)
        t.Text = GUIConversion.GetLengthMeasureText(value);
    }

    static bool GetPositionValue(TextBox t, ref double value)
    {
      return GUIConversion.GetLengthMeasureValue(t.Text, ref value);
    }

    static void SetSizeText(TextBox t, double value)
    {
      SetPositionText(t, value);
    }
    static bool GetSizeValue(TextBox t, ref double value)
    {
      return GetPositionValue(t, ref value);
    }



    #endregion

    #region Position
    double _positionX;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double PositionX
    {
      get { return _positionX; }
      set
      {
        _positionX = value;
        SetPositionText(_edPositionX, value);
      }
    }


    double _positionY;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double PositionY
    {
      get { return _positionY; }
      set
      {
        _positionY = value;
        SetPositionText(_edPositionY, value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointD2D Position
    {
      get
      {
        return new PointD2D(_positionX, _positionY);
      }
      set
      {
        PositionX = value.X;
        PositionY = value.Y;
      }
    }



    TextBox _edPositionX;
    public TextBox EdPositionX
    {
      get { return _edPositionX; }
      set
      {
        if (_edPositionX != null)
          _edPositionX.Validating -= EhPositionX_Validating;

        _edPositionX = value;

        if (_edPositionX != null)
          _edPositionX.Validating += EhPositionX_Validating;

        SetPositionText(_edPositionX, _positionX);
      }
    }

    TextBox _edPositionY;
    public TextBox EdPositionY
    {
      get { return _edPositionY; }
      set
      {
        if (_edPositionY != null)
          _edPositionY.Validating -= EhPositionY_Validating;

        _edPositionY = value;

        if (_edPositionY != null)
          _edPositionY.Validating += EhPositionY_Validating;

        SetPositionText(_edPositionY, _positionY);
      }
    }


    void EhPositionX_Validating(object sender, CancelEventArgs e)
    {
      if (!_edPositionX.Modified)
        return;

      if (GetPositionValue(_edPositionX, ref _positionX))
      {
        return;
      }
      else
      {
        e.Cancel = true;
      }
    }
    void EhPositionY_Validating(object sender, CancelEventArgs e)
    {
      if (!_edPositionY.Modified)
        return;
      if (GetPositionValue(_edPositionY, ref _positionY))
      {
        return;
      }
      else
      {
        e.Cancel = true;
      }
    }
    #endregion

    #region Size

    double _sizeX;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double SizeX
    {
      get { return _sizeX; }
      set
      {
        _sizeX = value;
        SetPositionText(_edSizeX, value);
      }
    }

    double _sizeY;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double SizeY
    {
      get { return _sizeY; }
      set
      {
        _sizeY = value;
        SetPositionText(_edSizeY, value);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointD2D Size
    {
      get
      {
        return new PointD2D(_sizeX, _sizeY);
      }
      set
      {
        SizeX = value.X;
        SizeY = value.Y;
      }
    }

    TextBox _edSizeX;
    TextBox _edSizeY;

    public TextBox EdSizeX
    {
      get { return _edSizeX; }
      set
      {
        if (_edSizeX != null)
          _edSizeX.Validating -= EhSizeX_Validating;

        _edSizeX = value;

        if (_edSizeX != null)
          _edSizeX.Validating += EhSizeX_Validating;

        SetPositionText(_edSizeX, _sizeX);
      }
    }
    public TextBox EdSizeY
    {
      get { return _edSizeY; }
      set
      {
        if (_edSizeY != null)
          _edSizeY.Validating -= EhSizeY_Validating;

        _edSizeY = value;

        if (_edSizeY != null)
          _edSizeY.Validating += EhSizeY_Validating;

        SetPositionText(_edSizeY, _sizeY);
      }
    }


    void EhSizeX_Validating(object sender, CancelEventArgs e)
    {
      if (!_edSizeX.Modified)
        return;

      if (GetPositionValue(_edSizeX, ref _sizeX))
      {
        return;
      }
      else
      {
        e.Cancel = true;
      }
    }
    void EhSizeY_Validating(object sender, CancelEventArgs e)
    {
      if (!_edSizeY.Modified)
        return;
      if (GetPositionValue(_edSizeY, ref _sizeY))
      {
        return;
      }
      else
      {
        e.Cancel = true;
      }
    }

    #endregion

    #region Rotation

    double _rotation;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double Rotation
    {
      get
      {
        if (_cbRotation != null)
          return _cbRotation.Rotation;
        else
        return _rotation;
      }
      set
      {
        _rotation = value;
        if (_cbRotation != null)
          _cbRotation.Rotation = (float)value;
      }
    }

    Altaxo.Gui.Common.Drawing.RotationComboBox _cbRotation;
    public Altaxo.Gui.Common.Drawing.RotationComboBox CbRotation
    {
      get { return _cbRotation; }
      set
      {
        if (_cbRotation != null)
        {
          _cbRotation.Validated -= EhRotation_Validated;
          _cbRotation.SelectionChangeCommitted -= EhRotation_Validated;
        }

        _cbRotation = value;

        if (_cbRotation != null)
        {
          _cbRotation.Validated += EhRotation_Validated;
          _cbRotation.SelectionChangeCommitted += EhRotation_Validated;

          _cbRotation.Rotation = (float)_rotation;
        }



      }
    }

    void EhRotation_Validated(object sender, EventArgs e)
    {
      _rotation = _cbRotation.Rotation;
    }
    #endregion


    #region Shear

    double _shear;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double Shear
    {
      get
      {
          return _shear;
      }
      set
      {
        _shear = value;
        if (_edShear != null)
          _edShear.Text = Altaxo.Serialization.GUIConversion.ToString(_shear);
      }
    }

    System.Windows.Forms.TextBox _edShear;
    public System.Windows.Forms.TextBox GuiShear
    {
      get { return _edShear; }
      set
      {
        if (_edShear != null)
        {
          _edShear.Validating -= EhShear_Validating;
          _edShear.Validated -= EhShear_Validated;
          //_edShear.SelectionChangeCommitted -= EhRotation_Validated;
        }

        _edShear = value;

        if (_edShear != null)
        {
          _edShear.Validating += EhShear_Validating;
          _edShear.Validated += EhShear_Validated;
          //_edShear.SelectionChangeCommitted += EhRotation_Validated;

          _edShear.Text = Altaxo.Serialization.GUIConversion.ToString(_shear);
        }
      }
    }
    void EhShear_Validating(object sender, CancelEventArgs e)
    {
      if (!Altaxo.Serialization.GUIConversion.IsDouble(_edShear.Text))
        e.Cancel = true;
    }
    void EhShear_Validated(object sender, EventArgs e)
    {
      double val;
      if (Altaxo.Serialization.GUIConversion.IsDouble(_edShear.Text, out val))
        _shear = val;
    }
    #endregion


    #region ScaleX

    double _scaleX;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double ScaleX
    {
      get
      {
          return _scaleX;
      }
      set
      {
        _scaleX = value;
        if (_edScaleX != null)
          _edScaleX.Text = Altaxo.Serialization.GUIConversion.ToString(_scaleX);
      }
    }

    System.Windows.Forms.TextBox _edScaleX;
    public System.Windows.Forms.TextBox GuiScaleX
    {
      get { return _edScaleX; }
      set
      {
        if (_edScaleX != null)
        {
          _edScaleX.Validating -= EhScaleX_Validating;
          _edScaleX.Validated -= EhScaleX_Validated;
          //_edScaleX.SelectionChangeCommitted -= EhRotation_Validated;
        }

        _edScaleX = value;

        if (_edScaleX != null)
        {
          _edScaleX.Validating += EhScaleX_Validating;
          _edScaleX.Validated += EhScaleX_Validated;
          //_edScaleX.SelectionChangeCommitted += EhRotation_Validated;

          _edScaleX.Text = Altaxo.Serialization.GUIConversion.ToString(_scaleX);
        }
      }
    }
    void EhScaleX_Validating(object sender, CancelEventArgs e)
    {
      if (!Altaxo.Serialization.GUIConversion.IsDouble(_edScaleX.Text))
        e.Cancel = true;
    }
    void EhScaleX_Validated(object sender, EventArgs e)
    {
      double val;
      if (Altaxo.Serialization.GUIConversion.IsDouble(_edScaleX.Text, out val))
        _scaleX = val;
    }
    #endregion

    #region ScaleY

    double _scaleY;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double ScaleY
    {
      get
      {
        return _scaleY;
      }
      set
      {
       _scaleY = value;
        if (_edScaleY != null)
          _edScaleY.Text = Altaxo.Serialization.GUIConversion.ToString(_scaleY);
      }
    }

    System.Windows.Forms.TextBox _edScaleY;
    public System.Windows.Forms.TextBox GuiScaleY
    {
      get { return _edScaleY; }
      set
      {
        if (_edScaleY != null)
        {
          _edScaleY.Validating -= EhScaleY_Validating;
          _edScaleY.Validated -= EhScaleY_Validated;
          //_edScaleY.SelectionChangeCommitted -= EhRotation_Validated;
        }

        _edScaleY = value;

        if (_edScaleY != null)
        {
          _edScaleY.Validating += EhScaleY_Validating;
          _edScaleY.Validated += EhScaleY_Validated;
          //_edScaleY.SelectionChangeCommitted += EhRotation_Validated;

          _edScaleY.Text = Altaxo.Serialization.GUIConversion.ToString(_scaleY);
        }
      }
    }
    void EhScaleY_Validating(object sender, CancelEventArgs e)
    {
      if (!Altaxo.Serialization.GUIConversion.IsDouble(_edScaleY.Text))
        e.Cancel = true;
    }
    void EhScaleY_Validated(object sender, EventArgs e)
    {
      double val;
      if (Altaxo.Serialization.GUIConversion.IsDouble(_edScaleY.Text, out val))
        _scaleY = val;
    }
    #endregion

  }
}
