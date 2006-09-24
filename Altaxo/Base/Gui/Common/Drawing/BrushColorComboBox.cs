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
using System.ComponentModel;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public class BrushColorComboBox : ComboBox
  {

    ContextMenuStrip _contextStrip;
    static ColorDialog _colorDialog;
    ColorType _colorType;


    public BrushColorComboBox()
     
    {
      ColorComboBox.SetComboBoxProperties(this);
      _contextStrip = new ContextMenuStrip();
      FillMenu();
      this.ContextMenuStrip = _contextStrip;
      _colorType = ColorType.KnownAndSystemColor;
    }

    public BrushColorComboBox(BrushHolder selectedBrush)
      : this()
    {
      SetDataSource(selectedBrush);
    }


    public ColorType ColorType
    {
      get
      {
        return _colorType;
      }
      set
      {
        _colorType = value;
        SetDataSource(this.Brush);
      }
    }

    public virtual void FillMenu()
    {
      AddCustomBrushContextMenu(_contextStrip, this.EhShowCustomBrushDialog);
      ColorComboBox.AddCustomColorContextMenu(_contextStrip, this.EhChooseCustomColor);
      ColorComboBox.AddTransparencyContextMenu(_contextStrip, this.EhChooseTransparencyValue);
    }

    public static void AddCustomBrushContextMenu(ContextMenuStrip menu, EventHandler eh)
    {
      menu.Items.Add("Custom Brush..", null, eh);
    }

    void EhShowCustomBrushDialog(object sender, EventArgs e)
    {
      BrushAllPropertiesControl ctrl = new BrushAllPropertiesControl();
      ctrl.Brush = (BrushHolder)this.Brush.Clone();
      if (Current.Gui.ShowDialog(ctrl, "Brush properties"))
      {
        this.Brush = ctrl.Brush;
        OnSelectedItemChanged(EventArgs.Empty);
        OnSelectedValueChanged(EventArgs.Empty);
        OnSelectionChangeCommitted(EventArgs.Empty);
      }
    }

    void SetDataSource(BrushHolder selectedBrush)
    {
      this.BeginUpdate();

      Items.Clear();
      bool knownColorBrush=false;
      if(selectedBrush.BrushType == BrushType.SolidBrush)
      {
      if (!ColorDictionary.IsColorOfType(selectedBrush.Color, _colorType))
        Items.Add(selectedBrush.Color);
      else
        knownColorBrush = true;
      }
      else
      {
        Items.Add(selectedBrush);
      }
    
      foreach (Color c in ColorDictionary.GetColorsOfType(_colorType))
        Items.Add(c);

      if (selectedBrush.BrushType == BrushType.SolidBrush)
        SelectedItem = ColorDictionary.GetNormalizedColor(selectedBrush.Color,_colorType);
      else
         SelectedItem = selectedBrush;

      this.EndUpdate();
    }
  
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BrushHolder Brush
    {
      get
      {
        if (SelectedItem is Color)
          return new BrushHolder((Color)SelectedItem);
        else if (SelectedItem is BrushHolder)
          return (BrushHolder)SelectedItem;
        else
          return new BrushHolder(Color.Black);
      }
      set
      {
        if(value!=null)
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

      grfx.DrawRectangle(new Pen(e.ForeColor), rectColor);

      string text = string.Empty;
      if (e.Index >= 0)
      {
        if (Items[e.Index] is Color)
        {
          Color itemColor = e.Index < 0 ? Color.Black : (Color)Items[e.Index];
          grfx.FillRectangle(new SolidBrush(itemColor), rectColor);
          if (itemColor.IsNamedColor)
          {
            text = itemColor.Name;
          }
          else if (ColorDictionary.IsBaseColorNamed(itemColor))
          {
            int transparency = ((255 - itemColor.A) * 100) / 255;
            text = "T" + transparency.ToString() + ColorDictionary.GetBaseColorName(itemColor);
          }
          else
          {
            text = "Custom Color";
          }
        }
        else
        {
          BrushHolder itemBrush = (BrushHolder)Items[e.Index];
          itemBrush.Rectangle = rectColor;
          grfx.FillRectangle(itemBrush, rectColor);
          text = "Custom Brush";
        }
      }
      grfx.DrawString(text, Font, new SolidBrush(e.ForeColor), rectText);
    }

    protected void EhChooseCustomColor(object sender, EventArgs e)
    {
      if (null == _colorDialog)
        _colorDialog = new ColorDialog();

      _colorDialog.Color = this.Brush.Color;
      if (DialogResult.OK == _colorDialog.ShowDialog(this))
      {
        this.Brush = new BrushHolder(_colorDialog.Color);
        OnSelectedItemChanged(EventArgs.Empty);
        OnSelectedValueChanged(EventArgs.Empty);
        OnSelectionChangeCommitted(EventArgs.Empty);
      }
    }

    protected void EhChooseTransparencyValue(object sender, EventArgs e)
    {
      ToolStripItem item = (ToolStripItem)sender;
      int alpha = (int)item.Tag;
      this.Brush = new BrushHolder(Color.FromArgb(alpha, this.Brush.Color));
      OnSelectedItemChanged(EventArgs.Empty);
      OnSelectedValueChanged(EventArgs.Empty);
      OnSelectionChangeCommitted(EventArgs.Empty);
    }
  }
}
