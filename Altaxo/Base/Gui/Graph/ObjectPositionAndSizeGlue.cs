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
using System.Drawing;

using Altaxo.Graph;

namespace Altaxo.Gui.Graph
{
  public class ObjectPositionAndSizeGlue : Component
  {
    static LengthUnit _lastUnit = LengthUnit.Point;

    #region Input/Output
    static void SetPositionText(TextBox t, double value)
    {
      LengthUnit lastUnit = _lastUnit;
      double v = lastUnit.ConvertFrom(value,LengthUnit.Point);
      string txt = Altaxo.Serialization.GUIConversion.ToString(v,"G5") + " " + lastUnit.Shortcut;

      if (t != null)
        t.Text = txt;
    }
    static bool GetPositionValue(TextBox t, ref double value)
    {
      string txt = t.Text.Trim().ToLower();

      LengthUnit unit = _lastUnit;
      foreach (string end in LengthUnit.Shortcuts)
      {
        if (txt.EndsWith(end))
        {
          unit = LengthUnit.FromShortcut(end);
          txt = txt.Substring(0, txt.Length - end.Length).TrimEnd();
          break;
        }
      }


      double v;
      if (Altaxo.Serialization.GUIConversion.IsDouble(txt, out v))
      {
        value = LengthUnit.Point.ConvertFrom(v, unit);
        _lastUnit = unit;
        return true;
      }
      else
      {
        return false;
      }
    }

    static void SetSizeText(TextBox t, double value)
    {
      SetPositionText(t, value);
    }
    static bool GetSizeValue(TextBox t, ref double value)
    {
      return GetPositionValue(t, ref value);
    }
    static void SetRotationText(TextBox t, double value)
    {
      string txt = Altaxo.Serialization.GUIConversion.ToString(value);
      if (t != null)
        t.Text = txt;
    }
    static bool GetRotationValue(TextBox t, ref double value)
    {
      double v;
      if (Altaxo.Serialization.GUIConversion.IsDouble(t.Text, out v))
      {
        value = v;
        return true;
      }
      else
      {
        return false;
      }
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
    public PointF Position
    {
      get
      {
        return new PointF((float)_positionX, (float)_positionY); 
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
    public SizeF Size
    {
      get
      {
        return new SizeF((float)_sizeX, (float)_sizeY);
      }
      set
      {
        SizeX = value.Width;
        SizeY = value.Height;
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
      get { return _rotation; }
      set
      {
        _rotation = value;
        SetRotationText(_edRotation, value);
      }
    }

    TextBox _edRotation;
    public TextBox EdRotation
    {
      get { return _edRotation; }
      set 
      {
        if (_edRotation != null)
          _edRotation.Validating -= EhRotation_Validating;

        _edRotation = value;

        if (_edRotation != null)
          _edRotation.Validating += EhRotation_Validating;

        SetPositionText(_edRotation, _rotation);
      
      }
    }

    void EhRotation_Validating(object sender, CancelEventArgs e)
    {
      if (!_edRotation.Modified)
        return;
      if (GetRotationValue(_edRotation, ref _rotation))
      {
        return;
      }
      else
      {
        e.Cancel = true;
      }
    }
    #endregion
  }
}
