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



  public class LineCapComboBox : ComboBox
  {
    bool _isForEndCap;
    ContextMenuStrip _contextStrip;

    static SortedDictionary<string, LineCapEx> _lineCaps;

    public LineCapComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = Font.Height;

      _contextStrip = new ContextMenuStrip();
      _contextStrip.Items.Add("Custom ..", null, this.EhChooseCustom);
      this.ContextMenuStrip = _contextStrip;
    }

    public LineCapComboBox(bool isForEndCap)
      : this()
    {
      _isForEndCap = isForEndCap;
    }

    public LineCapComboBox(bool isForEndCap, LineCapEx selected)
      : this()
    {
      _isForEndCap = isForEndCap;
      SetDataSource(selected);
    }


    void SetDefaultValues()
    {
      _lineCaps = new SortedDictionary<string, LineCapEx>();

      foreach (LineCapEx cap in LineCapEx.GetValues())
      {
          _lineCaps.Add(cap.Name, cap);
      }
    }

    void SetDataSource(LineCapEx selected)
    {
      if (_lineCaps == null)
        SetDefaultValues();
      if (!_lineCaps.ContainsKey(selected.Name))
        _lineCaps.Add(selected.Name, selected);

      this.BeginUpdate();

      Items.Clear();

      foreach (KeyValuePair<string,LineCapEx> o in _lineCaps)
        Items.Add(o.Value);

      SelectedItem = LineCapEx.FromName(selected.Name);

      this.EndUpdate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LineCapEx LineCapEx
    {
      get
      {
        return null==SelectedItem ? LineCapEx.Flat : (LineCapEx)SelectedItem;
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

      LineCapEx item = e.Index>=0 ? (LineCapEx)Items[e.Index] : LineCapEx.Flat;
      SolidBrush foreColorBrush = new SolidBrush(e.ForeColor);

      Pen linePen = new Pen(foreColorBrush, (float)Math.Ceiling(0.4 * e.Bounds.Height));
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
