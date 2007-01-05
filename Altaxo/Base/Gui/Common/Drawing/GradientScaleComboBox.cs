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
using System.Drawing.Drawing2D;
using System.ComponentModel;


namespace Altaxo.Gui.Common.Drawing
{
  public class GradientScaleComboBox : ComboBox
  {
    static List<float> _listValues;

    public GradientScaleComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDown;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public GradientScaleComboBox(float value)
      : this()
    {

      SetDataSource(value);
    }

    void SetDefaultValues()
    {
      _listValues = new List<float>();
      _listValues.AddRange(new float[] { 0.25f, 0.5f, 0.75f,  1 });
    }

    void SetDataSource(float value)
    {
      this.BeginUpdate();

      if (_listValues == null)
        SetDefaultValues();

      Items.Clear();

      if (!_listValues.Contains(value))
      {
        _listValues.Add(value);
        _listValues.Sort();
      }

      foreach (float val in _listValues)
        Items.Add(val);

      SelectedItem = value;

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float GradientScale
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

      float item = (float)Items[e.Index];
      using (LinearGradientBrush br = new LinearGradientBrush(rectColor, e.ForeColor, e.BackColor, LinearGradientMode.Horizontal))
      {
        br.SetBlendTriangularShape(0.5f,item);
        grfx.FillRectangle(br, rectColor);
      }

      using (SolidBrush foreColorBrush = new SolidBrush(e.ForeColor))
      {
        string text = Altaxo.Serialization.GUIConversion.ToString(item);
        grfx.DrawString(text, Font, foreColorBrush, rectText);
      }
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
