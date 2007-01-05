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
using Altaxo.Graph.Gdi;


namespace Altaxo.Gui.Common.Drawing
{



  public class FontComboBox : ComboBox
  {
    private class FontAndStyle
    {
      public FontFamily Family;
      public FontStyle Style;
      public FontAndStyle(FontFamily fam, FontStyle style)
      {
        Family = fam;
        Style = style;
      }
      public override bool Equals(object obj)
      {
        if(obj==null || !(obj is FontAndStyle))
          return false;
        FontAndStyle from = (FontAndStyle)obj;

        return this.Family == from.Family && this.Style == from.Style;
      }

      public override int GetHashCode()
      {
        return Family.GetHashCode() + Style.GetHashCode();
      }

      public override string ToString()
      {
        return Family.Name + "-" + Style.ToString();
      }

      public static FontAndStyle Default
      {
        get { return new FontAndStyle(FontFamily.GenericSansSerif, FontStyle.Regular); }
      }
    }

    public FontComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public FontComboBox(Font selected)
      : this()
    {
      SetDataSource(selected);
    }


    protected override void OnDropDown(EventArgs e)
    {
      if (Items.Count <= 1)
      {
        Font selected = this.Font;

        this.BeginUpdate();

        Items.Clear();

        foreach (FontFamily ff in FontFamily.Families)
        {
          foreach (FontStyle style in Enum.GetValues(typeof(FontStyle)))
          {
            if (ff.IsStyleAvailable(style))
              Items.Add(new FontAndStyle(ff, style));
          }
        }

        SelectedItem = new FontAndStyle(selected.FontFamily, selected.Style);

        this.EndUpdate();
      }

      base.OnDropDown(e);
    }

    void SetDataSource(Font selected)
    {
      if (Items.Count > 1)
        Items.Clear();
      if(Items.Count==0)
        Items.Add(new FontAndStyle(selected.FontFamily,selected.Style));
      else
        Items[0] = new FontAndStyle(selected.FontFamily,selected.Style);

      SelectedIndex = 0;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Font FontDocument
    {
      get
      {
          FontAndStyle fas = SelectedItem != null ? (FontAndStyle)SelectedItem : FontAndStyle.Default;
          return new Font(fas.Family, 12, fas.Style);
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

      FontAndStyle item = e.Index >= 0 ? (FontAndStyle)Items[e.Index] : FontAndStyle.Default;
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);

      Font font = new Font(item.Family, Font.Size, item.Style);
      grfx.DrawString("Abc", font, foreColorBrush, rectColor);
      grfx.DrawString(item.ToString(), Font, foreColorBrush, rectText);

      font.Dispose();
    }


  }
}
