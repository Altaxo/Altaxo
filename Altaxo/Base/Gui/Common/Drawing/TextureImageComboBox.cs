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
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Altaxo.Graph;



namespace Altaxo.Gui.Common.Drawing
{
  public class TextureImageComboBox : ComboBox
  {
    public TextureImageComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;

      this.ContextMenuStrip = new ContextMenuStrip();
      this.ContextMenuStrip.Items.Add("From file ...", null, EhLoadFromFile);
    }

    public TextureImageComboBox(ImageProxy selected)
      : this()
    {
      SetDataSource(selected);
    }


    void EhLoadFromFile(object sender, EventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      if (DialogResult.OK == dlg.ShowDialog(Current.MainWindow))
      {
        ImageProxy img = ImageProxy.FromFile(dlg.FileName);
        if (img.IsValid)
        {
          SetDataSource(img);
          OnSelectedItemChanged(EventArgs.Empty);
          OnSelectedValueChanged(EventArgs.Empty);
          OnSelectionChangeCommitted(EventArgs.Empty);
        }
      }
    }

    void SetDataSource(ImageProxy selected)
    {
      this.BeginUpdate();

      Items.Clear();

      Items.Add(ImageProxy.FromResource("Altaxo.Textures.Marbel.Marbel01.jpg"));

      if (selected != null)
      {
        Items.Add(selected);
        SelectedItem = selected;
      }

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ImageProxy TextureImage
    {
      get
      {
        return SelectedItem == null ? null : (ImageProxy)SelectedItem;
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

      ImageProxy item = e.Index >= 0 ? (ImageProxy)Items[e.Index] : null;
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);
      grfx.DrawString(item==null?"<No image>":item.ToString(), Font, foreColorBrush, rectText);
    }


  }
}
