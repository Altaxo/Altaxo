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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Altaxo.Collections;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{
  public partial class AxisLineStyleControl : UserControl, IAxisLineStyleView
  {
    public AxisLineStyleControl()
    {
      InitializeComponent();
    }

    #region IAxisLineStyleView Members

    public PenX LinePen
    {
      get
      {
        return _linePenGlue.Pen;
      }
      set
      {
        _linePenGlue.Pen = value;
      }
    }

    public PenX MajorPen
    {
      get
      {
        return _majorPenGlue.Pen;
      }
      set
      {
        _majorPenGlue.Pen = value;
        if (value != null)
        {
          CustomMajorColor = !PenX.AreEqualUnlessWidth(value,_linePenGlue.Pen);
          CustomMajorThickness = (value.Width != _linePenGlue.Pen.Width);
        }
      }
    }

    public PenX MinorPen
    {
      get
      {
        return _minorPenGlue.Pen;
      }
      set
      {
        _minorPenGlue.Pen = value;
        if (value != null)
        {
          CustomMinorColor = !PenX.AreEqualUnlessWidth(value, _linePenGlue.Pen);
          CustomMinorThickness = (value.Width != _linePenGlue.Pen.Width);
        }
      }
    }

    public float MajorTickLength
    {
      get
      {
        return _lineMajorLength.PenWidthChoice;
      }
      set
      {
        _lineMajorLength.PenWidthChoice = value;
      }
    }

    public float MinorTickLength
    {
      get
      {
        return _lineMinorLength.PenWidthChoice;
      }
      set
      {
        _lineMinorLength.PenWidthChoice = value;
      }
    }
    public bool ShowLine
    {
      get
      {
        return _chkEnableLine.Checked;
      }
      set
      {
        _chkEnableLine.Checked = value;
      }
    }
   
    bool CustomMajorThickness
    {
      set
      {
        _chkCustomMajorThickness.Checked = value;
        _lineMajorThickness.Enabled = value ;

      }
    }
    bool CustomMinorThickness
    {
      set
      {
        _chkCustomMinorThickness.Checked = value;
        _lineMinorThickness.Enabled = value;

      }
    }

    bool CustomMajorColor
    {
      set
      {
        this._chkCustomMajorColor.Checked = value;
        this._majorLineColor.Enabled = value;
      }
    }
    bool CustomMinorColor
    {
      set
      {
        this._chkCustomMinorColor.Checked = value;
        this._minorLineColor.Enabled = value;
      }
    }

    public SelectableListNodeList MajorPenTicks
    {
      get
      {
        SelectableListNodeList list = new SelectableListNodeList();
        foreach (CheckBox chk in _majorWhichTicksLayout.Controls)
        {
          SelectableListNode n = new SelectableListNode(chk.Text, chk.Tag, chk.Checked);
          list.Add(n);
        }
        return list;
      }
      set
      {
        _majorWhichTicksLayout.Controls.Clear();
        foreach (SelectableListNode n in value)
        {
          CheckBox chk = new CheckBox();
          chk.Text = n.Name;
          chk.Tag = n.Item;
          chk.Checked = n.Selected;
          _majorWhichTicksLayout.Controls.Add(chk);
        }
      }
    }

    public SelectableListNodeList MinorPenTicks
    {
      get
      {
        SelectableListNodeList list = new SelectableListNodeList();
        foreach (CheckBox chk in _minorWhichTicksLayout.Controls)
        {
          SelectableListNode n = new SelectableListNode(chk.Text, chk.Tag, chk.Checked);
          list.Add(n);
        }
        return list;
      }
      set
      {
        _minorWhichTicksLayout.Controls.Clear();
        foreach (SelectableListNode n in value)
        {
          CheckBox chk = new CheckBox();
          chk.Text = n.Name;
          chk.Tag = n.Item;
          chk.Checked = n.Selected;
          _minorWhichTicksLayout.Controls.Add(chk);
        }
      }
    }

    #endregion

    private void EhIndividualMajorColor_CheckChanged(object sender, EventArgs e)
    {
      if (!_chkCustomMajorColor.Checked)
        _majorLineColor.ColorChoice = _lineBrushColor.ColorChoice;
      _majorLineColor.Enabled = _chkCustomMajorColor.Checked;
    }

    private void EhIndividualMinorColor_CheckChanged(object sender, EventArgs e)
    {
      if (!_chkCustomMinorColor.Checked)
        _minorLineColor.ColorChoice = _lineBrushColor.ColorChoice;
      _minorLineColor.Enabled = _chkCustomMinorColor.Checked;
    }

    private void EhIndividualMajorThickness_CheckChanged(object sender, EventArgs e)
    {
      if (!_chkCustomMajorThickness.Checked)
        _lineMajorThickness.PenWidthChoice = _lineLineThickness.PenWidthChoice;
      _lineMajorThickness.Enabled = _chkCustomMajorThickness.Checked;
    }

    private void EhIndividualMinorThickness_CheckChanged(object sender, EventArgs e)
    {
      if (!_chkCustomMinorThickness.Checked)
        _lineMinorThickness.PenWidthChoice = _lineLineThickness.PenWidthChoice;
      _lineMinorThickness.Enabled = _chkCustomMinorThickness.Checked;
    }

    private void EhLinePen_Changed(object sender, EventArgs e)
    {
      
      if (!_chkCustomMajorColor.Checked)
      {
        if(this._majorPenGlue.Pen!=null)
          this._majorPenGlue.Pen.BrushHolder = _linePenGlue.Pen.BrushHolder;
      }
      if (!_chkCustomMinorColor.Checked)
      {
        if(this._minorPenGlue.Pen!=null)
          this._minorPenGlue.Pen.BrushHolder = _linePenGlue.Pen.BrushHolder;
      }
    }

    private void EhLineThickness_Changed(object sender, EventArgs e)
    {
      if (!_chkCustomMajorThickness.Checked)
        _lineMajorThickness.PenWidthChoice = _lineLineThickness.PenWidthChoice;
      if (!_chkCustomMinorThickness.Checked)
        _lineMinorThickness.PenWidthChoice = _lineLineThickness.PenWidthChoice;
    }
  }

  public interface IAxisLineStyleView
  {
    bool ShowLine { get; set; }
    PenX LinePen { get; set; }
    PenX MajorPen{get; set; }
    PenX MinorPen{ get; set; }
    float MajorTickLength { get; set; }
    float MinorTickLength { get; set; }
    SelectableListNodeList MajorPenTicks { get; set; }
    SelectableListNodeList MinorPenTicks { get; set; }
  }
}
