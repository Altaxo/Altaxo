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



  public class LinearGradientShapeComboBox : ComboBox
  {
    public LinearGradientShapeComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
    }

    public LinearGradientShapeComboBox(LinearGradientShape selected)
      : this()
    {
      SetDataSource(selected);
    }




    void SetDataSource(LinearGradientShape selected)
    {
      this.BeginUpdate();

      Items.Clear();
      foreach (LinearGradientShape o in Enum.GetValues(typeof(LinearGradientShape)))
        Items.Add(o);

      SelectedItem = selected;

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LinearGradientShape LinearGradientShape
    {
      get
      {
        return SelectedItem == null ? LinearGradientShape.Linear : (LinearGradientShape)SelectedItem;
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

      LinearGradientShape item = e.Index >= 0 ? (LinearGradientShape)Items[e.Index] : LinearGradientShape.Linear;

      Rectangle rectShape = new Rectangle(rectColor.X + rectColor.Width / 4, rectColor.Y, rectColor.Width / 2, rectColor.Height);
      using (LinearGradientBrush br = new LinearGradientBrush(rectShape, e.ForeColor, e.BackColor, LinearGradientMode.Horizontal))
      {
        if (item == LinearGradientShape.Triangular)
          br.SetBlendTriangularShape(0.5f);
        else if (item == LinearGradientShape.SigmaBell)
          br.SetSigmaBellShape(0.5f);

        grfx.FillRectangle(br, rectColor);
      }
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);
      grfx.DrawString(item.ToString(), Font, foreColorBrush, rectText);
    }


  }
}
