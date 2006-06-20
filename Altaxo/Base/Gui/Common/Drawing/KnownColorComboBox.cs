#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common.Drawing
{
  public class KnownColorComboBox : ComboBox
  {
    ContextMenuStrip _contextStrip;
    static ColorDialog _colorDialog;

    public KnownColorComboBox()
     : this(Color.Black)
    {
    }

    public KnownColorComboBox(Color selectedColor)
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
      SetDataSource(selectedColor);

      _contextStrip = new ContextMenuStrip();
      _contextStrip.Items.Add("Custom Color..", null, this.EhChooseCustomColor);
      this.ContextMenuStrip = _contextStrip;
    }

    void SetDataSource(Color selectedColor)
    {
      this.BeginUpdate();

      Items.Clear();

      if(!selectedColor.IsKnownColor)
        Items.Add(selectedColor);

      foreach (object o in Enum.GetValues(typeof(KnownColor)))
        Items.Add(Color.FromKnownColor((KnownColor)o));

      SelectedItem = selectedColor;

      this.EndUpdate();
    }

    public Color Color
    {
      get
      {
        return  (Color)SelectedItem;
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

      if(this.Enabled)
        e.DrawBackground();

      grfx.DrawRectangle(new Pen(e.ForeColor), rectColor);
      
      Color itemColor = e.Index < 0 ? Color.Black : (Color)Items[e.Index];
      grfx.FillRectangle(new SolidBrush(itemColor), rectColor);
      string text = itemColor.IsNamedColor ? itemColor.Name : "Custom";
      grfx.DrawString(text, Font, new SolidBrush(e.ForeColor), rectText);
    }

    protected void EhChooseCustomColor(object sender, EventArgs e)
    {
      if(null==_colorDialog)
        _colorDialog = new ColorDialog();

      _colorDialog.Color = this.Color;
      if (DialogResult.OK == _colorDialog.ShowDialog(this))
      {
        this.Color = _colorDialog.Color;
        OnSelectedItemChanged(EventArgs.Empty);
        OnSelectedValueChanged(EventArgs.Empty);
        OnSelectionChangeCommitted(EventArgs.Empty);
      }
    }
  }
}
