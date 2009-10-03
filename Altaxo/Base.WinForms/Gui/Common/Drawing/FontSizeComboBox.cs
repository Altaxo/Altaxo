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
  public class FontSizeComboBox : ComboBox
  {
    static List<float> _itemValues;

    public FontSizeComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDown;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public FontSizeComboBox(float thickness)
      : this()
    {

      SetDataSource(thickness);
    }

    void SetDefaultValues()
    {
      _itemValues = new List<float>();
      _itemValues.AddRange(new float[] { 6, 8,  10, 12, 14, 16, 18, 20, 24, 28, 32, 48, 72 });
    }

    void SetDataSource(float thickness)
    {
      this.BeginUpdate();

      if (_itemValues == null)
        SetDefaultValues();

      Items.Clear();

      if (!_itemValues.Contains(thickness))
      {
        _itemValues.Add(thickness);
        _itemValues.Sort();
      }

      foreach (float val in _itemValues)
        Items.Add(val);

      SelectedItem = thickness;

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float FontSize
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
    public event EventHandler FontSizeChanged;
    protected void OnFontSizeChanged()
    {
      if (FontSizeChanged!=null)
        FontSizeChanged(this, EventArgs.Empty);
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


      float itemThickness = (float)Items[e.Index];
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);

      if (itemThickness >= rectColor.Height)
        grfx.FillRectangle(foreColorBrush, rectColor);
      else
        grfx.DrawLine(new Pen(foreColorBrush, itemThickness),
       rectColor.Left, 0.5f * (rectColor.Top + rectColor.Bottom),
       rectColor.Right, 0.5f * (rectColor.Top + rectColor.Bottom));

      string text = Altaxo.Serialization.GUIConversion.ToString(itemThickness);
      grfx.DrawString(text, Font, foreColorBrush, rectText);
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

      if (e.Cancel == false)
        OnFontSizeChanged();
    }

    protected override void OnSelectionChangeCommitted(EventArgs e)
    {
      base.OnSelectionChangeCommitted(e);

      OnFontSizeChanged();
    }


  }
}
