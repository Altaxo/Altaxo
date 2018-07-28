#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph
{
  using Altaxo.Units;
  using AUL = Altaxo.Units.Length;

  public class ObjectPositionAndSizeGlue : FrameworkElement
  {
    #region Input/Output

    private static void SetPositionText(TextBox t, double value)
    {
      if (t != null)
        t.Text = GUIConversion.GetLengthMeasureText(value);
    }

    private static bool GetPositionValue(TextBox t, ref double value)
    {
      return GUIConversion.GetLengthMeasureValue(t.Text, ref value);
    }

    private static void SetSizeText(TextBox t, double value)
    {
      SetPositionText(t, value);
    }

    private static bool GetSizeValue(TextBox t, ref double value)
    {
      return GetPositionValue(t, ref value);
    }

    #endregion Input/Output

    #region Position

    private double _positionX;

    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double PositionX
    {
      get
      {
        if (null != _edPositionX)
          return _edPositionX.SelectedQuantity.AsValueIn(AUL.Point.Instance);
        else
          return _positionX;
      }
      set
      {
        _positionX = value;
        if (null != _edPositionX)
          _edPositionX.SelectedQuantity = new DimensionfulQuantity(value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit);
      }
    }

    private double _positionY;

    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double PositionY
    {
      get
      {
        if (null != _edPositionY)
          return _edPositionY.SelectedQuantity.AsValueIn(AUL.Point.Instance);
        else
          return _positionY;
      }
      set
      {
        _positionY = value;
        if (null != _edPositionY)
          _edPositionY.SelectedQuantity = new DimensionfulQuantity(value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit);
      }
    }

    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointD2D Position
    {
      get
      {
        return new PointD2D(PositionX, PositionY);
      }
      set
      {
        PositionX = value.X;
        PositionY = value.Y;
      }
    }

    private Altaxo.Gui.Common.QuantityWithUnitTextBox _edPositionX;

    public Altaxo.Gui.Common.QuantityWithUnitTextBox EdPositionX
    {
      get { return _edPositionX; }
      set
      {
        _edPositionX = value;

        if (_edPositionX != null)
        {
          _edPositionX.UnitEnvironment = PositionEnvironment.Instance;
          _edPositionX.SelectedQuantity = new DimensionfulQuantity(_positionX, AUL.Point.Instance);
        }
      }
    }

    private Altaxo.Gui.Common.QuantityWithUnitTextBox _edPositionY;

    public Altaxo.Gui.Common.QuantityWithUnitTextBox EdPositionY
    {
      get { return _edPositionY; }
      set
      {
        _edPositionY = value;

        if (_edPositionY != null)
        {
          _edPositionY.UnitEnvironment = PositionEnvironment.Instance;
          _edPositionY.SelectedQuantity = new DimensionfulQuantity(_positionY, AUL.Point.Instance);
        }
      }
    }

    #endregion Position

    #region Size

    /// <summary>Size in point units.</summary>
    private double _sizeX;

    /// <summary>Gets/sets the size in point units.</summary>
    public double SizeX
    {
      get
      {
        if (null != _edSizeX)
          return _edSizeX.SelectedQuantity.AsValueIn(AUL.Point.Instance);
        else
          return _sizeX;
      }
      set
      {
        _sizeX = value;
        if (null != _edSizeX)
          _edSizeX.SelectedQuantity = new DimensionfulQuantity(value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit);
      }
    }

    private double _sizeY;

    public double SizeY
    {
      get
      {
        if (null != _edSizeY)
          return _edSizeY.SelectedQuantity.AsValueIn(AUL.Point.Instance);
        else
          return _sizeY;
      }
      set
      {
        _sizeY = value;
        if (null != _edSizeY)
          _edSizeY.SelectedQuantity = new DimensionfulQuantity(value, AUL.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit);
      }
    }

    public PointD2D Size
    {
      get
      {
        return new PointD2D(SizeX, SizeY);
      }
      set
      {
        SizeX = value.X;
        SizeY = value.Y;
      }
    }

    private Altaxo.Gui.Common.QuantityWithUnitTextBox _edSizeX;
    private Altaxo.Gui.Common.QuantityWithUnitTextBox _edSizeY;

    public Altaxo.Gui.Common.QuantityWithUnitTextBox EdSizeX
    {
      get { return _edSizeX; }
      set
      {
        _edSizeX = value;

        if (_edSizeX != null)
        {
          _edSizeX.UnitEnvironment = PositionEnvironment.Instance;
          _edSizeX.SelectedQuantity = new DimensionfulQuantity(_sizeX, AUL.Point.Instance);
        }
      }
    }

    public Altaxo.Gui.Common.QuantityWithUnitTextBox EdSizeY
    {
      get { return _edSizeY; }
      set
      {
        _edSizeY = value;

        if (_edSizeY != null)
        {
          _edSizeY.UnitEnvironment = PositionEnvironment.Instance;
          _edSizeY.SelectedQuantity = new DimensionfulQuantity(_sizeY, AUL.Point.Instance);
        }
      }
    }

    #endregion Size

    #region Rotation

    private double _rotation;

    public double Rotation
    {
      get
      {
        if (null != _cbRotation)
          return _cbRotation.SelectedQuantityAsValueInDegrees;
        else
          return _rotation;
      }
      set
      {
        _rotation = value;
        if (_cbRotation != null)
          _cbRotation.SelectedQuantityAsValueInDegrees = value;
      }
    }

    private Altaxo.Gui.Common.Drawing.RotationComboBox _cbRotation;

    public Altaxo.Gui.Common.Drawing.RotationComboBox CbRotation
    {
      get { return _cbRotation; }
      set
      {
        _cbRotation = value;
        if (_cbRotation != null)
        {
          _cbRotation.SelectedQuantityAsValueInDegrees = _rotation;
        }
      }
    }

    #endregion Rotation

    #region Shear

    private double _shear;

    public double Shear
    {
      get
      {
        if (null != _edShear)
          return _edShear.SelectedQuantityInSIUnits;
        else
          return _shear;
      }
      set
      {
        _shear = value;
        if (_edShear != null)
          _edShear.SelectedQuantityInSIUnits = value;
      }
    }

    private Altaxo.Gui.Common.Drawing.ShearComboBox _edShear;

    public Altaxo.Gui.Common.Drawing.ShearComboBox GuiShear
    {
      get { return _edShear; }
      set
      {
        _edShear = value;

        if (_edShear != null)
        {
          _edShear.SelectedQuantityInSIUnits = _shear;
        }
      }
    }

    #endregion Shear

    #region Scale

    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointD2D Scale
    {
      get
      {
        return new PointD2D(ScaleX, ScaleY);
      }
      set
      {
        ScaleX = value.X;
        ScaleY = value.Y;
      }
    }

    #endregion Scale

    #region ScaleX

    private double _scaleX = 1;

    public double ScaleX
    {
      get
      {
        if (_edScaleX != null)
          return _edScaleX.SelectedQuantityInSIUnits;
        else
          return _scaleX;
      }
      set
      {
        _scaleX = value;
        if (_edScaleX != null)
          _edScaleX.SelectedQuantityInSIUnits = value;
      }
    }

    private Altaxo.Gui.Common.Drawing.ScaleComboBox _edScaleX;

    public Altaxo.Gui.Common.Drawing.ScaleComboBox GuiScaleX
    {
      get { return _edScaleX; }
      set
      {
        _edScaleX = value;

        if (_edScaleX != null)
        {
          _edScaleX.SelectedQuantityInSIUnits = _scaleX;
        }
      }
    }

    #endregion ScaleX

    #region ScaleY

    private double _scaleY = 1;

    public double ScaleY
    {
      get
      {
        if (_edScaleY != null)
          return _edScaleY.SelectedQuantityInSIUnits;
        else
          return _scaleY;
      }
      set
      {
        _scaleY = value;
        if (_edScaleY != null)
          _edScaleY.SelectedQuantityInSIUnits = value;
      }
    }

    private Altaxo.Gui.Common.Drawing.ScaleComboBox _edScaleY;

    public Altaxo.Gui.Common.Drawing.ScaleComboBox GuiScaleY
    {
      get { return _edScaleY; }
      set
      {
        _edScaleY = value;
        if (_edScaleY != null)
        {
          _edScaleY.SelectedQuantityInSIUnits = _scaleY;
        }
      }
    }

    #endregion ScaleY
  }
}
