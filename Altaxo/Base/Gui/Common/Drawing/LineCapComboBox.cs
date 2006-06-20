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
using Altaxo.Graph;


namespace Altaxo.Gui.Common.Drawing
{



  public class LineCapComboBox : ComboBox
  {
    bool _isForEndCap;
    ContextMenuStrip _contextStrip;

    static List<LineCapEx> _lineCaps;

    public LineCapComboBox(bool isForEndCap)
      : this(isForEndCap, LineCapEx.Flat)
    {
    }

    public LineCapComboBox(bool isForEndCap, LineCapEx selected)
    {
      _isForEndCap = isForEndCap;
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;
      SetDataSource(selected);

      _contextStrip = new ContextMenuStrip();
      _contextStrip.Items.Add("Custom ..", null, this.EhChooseCustom);
      this.ContextMenuStrip = _contextStrip;
    }


    void SetDefaultValues()
    {
      _lineCaps = new List<LineCapEx>();

      foreach(LineCap cap in Enum.GetValues(typeof(LineCap)))
      _lineCaps.Add(new LineCapEx(cap));
    }

    void SetDataSource(LineCapEx selected)
    {
      if (_lineCaps == null)
        SetDefaultValues();
      if (!_lineCaps.Contains(selected))
        _lineCaps.Add(selected.Clone());

      this.BeginUpdate();

      Items.Clear();

      foreach (LineCapEx o in _lineCaps)
        Items.Add(o);

      SelectedItem = selected;

      this.EndUpdate();
    }

    public LineCapEx LineCapEx
    {
      get
      {
        return (LineCapEx)SelectedItem;
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

      //grfx.DrawRectangle(new Pen(e.ForeColor), rectColor);

      LineCapEx item = (LineCapEx)Items[e.Index];
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);

      Pen linePen = new Pen(foreColorBrush, (float)Math.Ceiling(0.125 * e.Bounds.Height));
      if (_isForEndCap)
      {
        item.SetPenEndCap(linePen);
        grfx.DrawLine(linePen,
          rectColor.Left, 0.5f * (rectColor.Top + rectColor.Bottom),
          rectColor.Right - 0.25f*rectColor.Width, 0.5f * (rectColor.Top + rectColor.Bottom));
      }
      else
      {
        item.SetPenStartCap(linePen);
        grfx.DrawLine(linePen,
          rectColor.Left+0.25f*rectColor.Width, 0.5f * (rectColor.Top + rectColor.Bottom),
          rectColor.Right, 0.5f * (rectColor.Top + rectColor.Bottom));
      }
      grfx.DrawString(item.ToString(), Font, foreColorBrush, rectText);
    }

    protected void EhChooseCustom(object sender, EventArgs e)
    {
      /*
      if (null == _customDialog)
        _customDialog = new ColorDialog();

      _customDialog.Color = this.Color;
      if (DialogResult.OK == _colorDialog.ShowDialog(this))
      {
        this.Color = _colorDialog.Color;
        OnSelectedItemChanged(EventArgs.Empty);
        OnSelectedValueChanged(EventArgs.Empty);
        OnSelectionChangeCommitted(EventArgs.Empty);
      }
       */
    }
  }
}
