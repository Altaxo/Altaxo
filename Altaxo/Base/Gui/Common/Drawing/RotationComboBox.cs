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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Altaxo.Gui.Common.Drawing
{
  public class RotationComboBox : ComboBox
  {
    static List<float> _rotationValues;
    bool _textIsDirty;

    public RotationComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDown;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public RotationComboBox(float rotation)
      : this()
    {

      SetDataSource(rotation,false);
    }

    void SetDefaultValues()
    {
      _rotationValues = new List<float>();
      _rotationValues.AddRange(new float[] { 0, 45, 90, 135, 180, 225, 270, 315 });
    }

    void SetDataSource(float rotation, bool manuallyEnteredByUser)
    {
      this.BeginUpdate();

      if (_rotationValues == null)
        SetDefaultValues();

      Items.Clear();

      rotation = (float)Math.IEEERemainder(rotation, 360);

      if (manuallyEnteredByUser && !_rotationValues.Contains(rotation))
      {
        _rotationValues.Add(rotation);
        _rotationValues.Sort();
      }

      foreach (float val in _rotationValues)
        Items.Add(val);

      if (_rotationValues.Contains(rotation))
        SelectedItem = rotation;
      else
        this.Text = rotation.ToString();

      this.EndUpdate();
      
      _textIsDirty = false;

    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float Rotation
    {
      get
      {
        if (SelectedItem != null)
          return (float)SelectedItem;
        double v;
        if (Altaxo.Serialization.GUIConversion.IsDouble(this.Text, out v))
          return (float)v;
        return 1;
      }
      set
      {
        SetDataSource(value,false);
      }
    }

    public event EventHandler RotationChanged;
    protected virtual void OnRotationChanged()
    {
      if (null != RotationChanged)
        RotationChanged(this, EventArgs.Empty);
    }


    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      Graphics grfx = e.Graphics;
      Rectangle rectColor = new Rectangle(e.Bounds.Left, e.Bounds.Top, 2 * e.Bounds.Height, e.Bounds.Height);
      rectColor.Inflate(-1, -1);

      Rectangle rectText = new Rectangle(e.Bounds.Left + 2 * e.Bounds.Height,
        e.Bounds.Top,
        e.Bounds.Width - 2 * e.Bounds.Height,
        e.Bounds.Height);

      if (this.Enabled)
        e.DrawBackground();

     
      int minSize = Math.Min(rectColor.Height,rectColor.Width);
      PointF middle = new PointF(rectColor.X+minSize/2,rectColor.Y+minSize/2);


      float rot = (float)Items[e.Index];
      PointF dest = new PointF(
        middle.X+(float)(0.5*minSize*Math.Cos(rot*Math.PI/180)),
        middle.Y-(float)(0.5*minSize*Math.Sin(rot*Math.PI/180)));
      

      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);
      Pen foreColorPen = new Pen(e.ForeColor);

      grfx.DrawEllipse(foreColorPen, new Rectangle(rectColor.X, rectColor.Y, minSize, minSize));
      grfx.DrawLine(foreColorPen, middle, dest);

      string text = Altaxo.Serialization.GUIConversion.ToString(rot);
      grfx.DrawString(text, Font, foreColorBrush, rectText);
    }

    protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
    {
      double w;
      if (Altaxo.Serialization.GUIConversion.IsDouble(this.Text, out w))
      {
        this.SetDataSource((float)w, _textIsDirty);
      }
      else
      {
        e.Cancel = true;
      }

      base.OnValidating(e);

      if (e.Cancel == false)
        OnRotationChanged();
    }
    protected override void OnSelectionChangeCommitted(EventArgs e)
    {
      base.OnSelectionChangeCommitted(e);
      OnRotationChanged();
    }

    protected override void OnTextChanged(EventArgs e)
    {
      _textIsDirty = true;
      base.OnTextChanged(e);
    }


  }
}
