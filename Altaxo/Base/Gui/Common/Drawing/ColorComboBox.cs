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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public class ColorComboBox : ComboBox
  {
    ColorType _colorType;
    ContextMenuStrip _contextStrip;
    static ColorDialog _colorDialog;
    public event EventHandler ColorChoiceChanged;

    public ColorComboBox()
    {
      SetComboBoxProperties(this);
      _contextStrip = new ContextMenuStrip();
      FillMenu();
      this.ContextMenuStrip = _contextStrip;
      _colorType = ColorType.KnownAndSystemColor;
    }

    public ColorComboBox(Color selectedColor)
      : this()
    {
      SetDataSource(selectedColor);
    }

    public static void SetComboBoxProperties(ComboBox box)
    {
      box.DropDownStyle = ComboBoxStyle.DropDownList;
      box.DrawMode = DrawMode.OwnerDrawFixed;
      box.ItemHeight = box.Font.Height;
    
    }

    public static void AddCustomColorContextMenu(ContextMenuStrip menu, EventHandler eh)
    {
      menu.Items.Add("Custom Color..", null, eh);
    }
    public static void AddTransparencyContextMenu(ContextMenuStrip menu, EventHandler eh)
    {
      menu.Items.Add("-");
      for (int i = 0; i <= 100; i += 10)
      {
        ToolStripItem item = new ToolStripMenuItem();
        item.Text = "Transparency: " + i.ToString() + "%";
        item.Tag = (int)(255 - 2.55 * i);
        item.Click += eh;
        menu.Items.Add(item);
      }
    }

    public virtual void FillMenu()
    {
      AddCustomColorContextMenu(_contextStrip, this.EhChooseCustomColor);
      AddTransparencyContextMenu(_contextStrip, this.EhChooseTransparencyValue);
    }

    public ColorType ColorType
    {
      get
      {
        return _colorType;
      }
      set
      {
        ColorType oldvalue = _colorType;
        _colorType = value;

        if (oldvalue != value)
        {
          SetDataSource(this.ColorChoice);
        }
      }
    }

    protected virtual void OnColorChoiceChanged()
    {
      if (null != ColorChoiceChanged)
        ColorChoiceChanged(this, EventArgs.Empty);
    }

    protected override void OnSelectionChangeCommitted(EventArgs e)
    {
      base.OnSelectionChangeCommitted(e);
      OnColorChoiceChanged();
    }

    protected override void OnDropDown(EventArgs e)
    {
      if (Items.Count <= 1)
      {
        Color selectedColor = this.ColorChoice;

        this.BeginUpdate();

        Items.Clear();

        if (!ColorDictionary.IsColorOfType(selectedColor, _colorType))
          Items.Add(selectedColor);

        foreach (Color c in ColorDictionary.GetColorsOfType(_colorType))
          Items.Add(c);

        SelectedItem = ColorDictionary.GetNormalizedColor(selectedColor, _colorType);

        this.EndUpdate();
      }

      base.OnDropDown(e);
    }

    void SetDataSource(Color selectedColor)
    {
      if (Items.Count > 1)
        Items.Clear();

      if (Items.Count == 0)
        Items.Add(selectedColor);
      else
        Items[0] = selectedColor;

      SelectedIndex = 0;

      OnColorChoiceChanged();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color ColorChoice
    {
      get
      {
        if (SelectedItem is Color)
          return (Color)SelectedItem;
        else
          return Color.Black;
      }
      set
      {
        if ((SelectedItem is Color) && ((Color)SelectedItem) == value)
          return;
        else
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
      string text = "Custom";
      if (itemColor.IsNamedColor)
      {
        text = itemColor.Name;
      }
      else if (ColorDictionary.IsBaseColorNamed(itemColor))
      {
        int transparency = ((255 - itemColor.A) * 100) / 255;
        text = "T" + transparency.ToString() + ColorDictionary.GetBaseColorName(itemColor);
      }

      grfx.DrawString(text, Font, new SolidBrush(e.ForeColor), rectText);
    }

    protected void EhChooseCustomColor(object sender, EventArgs e)
    {
      if(null==_colorDialog)
        _colorDialog = new ColorDialog();

      _colorDialog.Color = this.ColorChoice;
      if (DialogResult.OK == _colorDialog.ShowDialog(this))
      {
        this.ColorChoice = _colorDialog.Color;
        OnSelectedItemChanged(EventArgs.Empty);
        OnSelectedValueChanged(EventArgs.Empty);
        OnSelectionChangeCommitted(EventArgs.Empty);
      }
    }

    protected void EhChooseTransparencyValue(object sender, EventArgs e)
    {
      ToolStripItem item = (ToolStripItem)sender;
      int alpha = (int)item.Tag;
      this.ColorChoice = Color.FromArgb(alpha, this.ColorChoice);
      OnSelectedItemChanged(EventArgs.Empty);
      OnSelectedValueChanged(EventArgs.Empty);
      OnSelectionChangeCommitted(EventArgs.Empty);
    }
  }
}
