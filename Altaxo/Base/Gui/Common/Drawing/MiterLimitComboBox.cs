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
  public class MiterLimitComboBox : ComboBox
  {
    static List<float> _miterValues;

    public MiterLimitComboBox()
     
    {
      DropDownStyle = ComboBoxStyle.DropDown;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public MiterLimitComboBox(float miterLimit)
      : this()
    {
      SetDataSource(miterLimit);
    }

    void SetDefaultValues()
    {
      _miterValues = new List<float>();
      _miterValues.AddRange(new float[] { 0, 0.125f, 0.25f, 0.5f, 1, 2, 3, 5, 10 });
    }

    void SetDataSource(float miterLimit)
    {
      this.BeginUpdate();

      if (_miterValues == null)
        SetDefaultValues();

      Items.Clear();

      if (!_miterValues.Contains(miterLimit))
      {
        _miterValues.Add(miterLimit);
        _miterValues.Sort();
      }

      foreach (float val in _miterValues)
        Items.Add(val);

      SelectedItem = miterLimit;

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float MiterValue
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
        SetDataSource(value);
      }
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

      //grfx.DrawRectangle(new Pen(e.ForeColor), rectColor);


      float miterValue = (float)Items[e.Index];
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);


      PointF[] points = new PointF[]{
        new PointF(rectColor.Right,rectColor.Top+0.125f*rectColor.Height),
        new PointF(rectColor.Right-0.5f*rectColor.Width,0.5f*(rectColor.Top+rectColor.Bottom)),
        new PointF(rectColor.Right,rectColor.Bottom-0.125f*rectColor.Height)
      };

      Pen pen = new Pen(foreColorBrush, (float)Math.Ceiling(0.375 * e.Bounds.Height));
      pen.MiterLimit = miterValue;

      grfx.DrawLines(pen,points);

      string text = Altaxo.Serialization.GUIConversion.ToString(miterValue);
      grfx.DrawString(text, Font, foreColorBrush, rectText);

      pen.Dispose();
      foreColorBrush.Dispose();
    }

    protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
    {
      double w;
      if (Altaxo.Serialization.GUIConversion.IsDouble(this.Text, out w))
      {
        this.SetDataSource((float)w);
      }
      else
      {
        e.Cancel = true;
      }

      base.OnValidating(e);
    }


  }
}
