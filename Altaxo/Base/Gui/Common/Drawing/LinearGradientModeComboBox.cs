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



  public class LinearGradientModeComboBox : ComboBox
  {
    public LinearGradientModeComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public LinearGradientModeComboBox(LinearGradientMode selected)
      : this()
    {
      SetDataSource(selected);
    }




    void SetDataSource(LinearGradientMode selected)
    {
      this.BeginUpdate();

      Items.Clear();
      foreach (LinearGradientMode o in Enum.GetValues(typeof(LinearGradientMode)))
        Items.Add(o);

      SelectedItem = selected;

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LinearGradientMode LinearGradientMode
    {
      get
      {
        return SelectedItem == null ? LinearGradientMode.Horizontal : (LinearGradientMode)SelectedItem;
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

      LinearGradientMode item = e.Index >= 0 ? (LinearGradientMode)Items[e.Index] : LinearGradientMode.Horizontal;
      
      using (LinearGradientBrush br = new LinearGradientBrush(rectColor, e.ForeColor, e.BackColor, item))
      {
        grfx.FillRectangle(br, rectColor);
      }

      using (SolidBrush foreColorBrush = new SolidBrush(e.ForeColor))
      {
        grfx.DrawString(item.ToString(), Font, foreColorBrush, rectText);
      }
    }


  }
}
