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
using Altaxo.Collections;

namespace Altaxo.Gui.Common.Drawing
{



  public class FontFamilyComboBox : ComboBox
  {
    public FontFamilyComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public FontFamilyComboBox(FontFamily selected)
      : this()
    {
      SetDataSource(selected);
    }




    void SetDataSource(FontFamily selected)
    {
      this.BeginUpdate();

      Items.Clear();

      foreach (FontFamily ff in FontFamily.Families)
      {
        if (!ff.IsStyleAvailable(FontStyle.Regular))
          continue;
        if (!ff.IsStyleAvailable(FontStyle.Bold))
          continue;
        if (!ff.IsStyleAvailable(FontStyle.Italic))
          continue;

        Items.Add(new NamedItem<FontFamily>(ff,ff.Name));
      }

      SelectedItem = new NamedItem<FontFamily>(selected,selected.Name);

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public FontFamily FontFamilyDocument
    {
      get
      {
        return SelectedItem == null ? FontFamily.GenericSansSerif : ((NamedItem<FontFamily>)SelectedItem).Item;
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

      NamedItem<FontFamily> item = e.Index >= 0 ? (NamedItem<FontFamily>)Items[e.Index] : new NamedItem<FontFamily>(FontFamily.GenericSansSerif,FontFamily.GenericSansSerif.Name);
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);
    
      Font font = new Font(item.Item,Font.Size,FontStyle.Regular);
   
      
      grfx.DrawString("Abc", font, foreColorBrush, rectColor);

      grfx.DrawString(item.ToString(), Font, foreColorBrush, rectText);
    }


  }
}
