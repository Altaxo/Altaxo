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



  public class BrushTypeComboBox : ComboBox
  {
    public BrushTypeComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public BrushTypeComboBox(BrushType selected)
      : this()
    {
      SetDataSource(selected);
    }




    void SetDataSource(BrushType selected)
    {
      this.BeginUpdate();

      Items.Clear();
      foreach (BrushType o in Enum.GetValues(typeof(BrushType)))
        Items.Add(o);

      SelectedItem = selected;

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BrushType BrushType
    {
      get
      {
        return SelectedItem == null ? BrushType.SolidBrush : (BrushType)SelectedItem;
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

      BrushType item = e.Index>=0 ? (BrushType)Items[e.Index] : BrushType.SolidBrush;
    
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);
      Brush fillbrush = foreColorBrush;

      switch (item)
      {
        case BrushType.SolidBrush:
          fillbrush = new SolidBrush(e.ForeColor);
          break;
        case BrushType.HatchBrush:
          fillbrush = new HatchBrush(HatchStyle.Cross, e.BackColor, e.ForeColor);
          break;
        case BrushType.LinearGradientBrush:
          fillbrush = new LinearGradientBrush(rectColor, e.BackColor, e.ForeColor, LinearGradientMode.ForwardDiagonal);
          break;
        case BrushType.PathGradientBrush:
          GraphicsPath gp = new GraphicsPath();
          gp.AddRectangle(rectColor);
          PathGradientBrush pgb = new PathGradientBrush(gp);
          pgb.CenterColor = e.ForeColor;
          fillbrush = pgb;
          break;
      }

      grfx.FillRectangle(fillbrush, rectColor);
      grfx.DrawString(item.ToString(), Font, foreColorBrush, rectText);
    }


  }
}
