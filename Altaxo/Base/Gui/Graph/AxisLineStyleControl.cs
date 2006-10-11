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
        CustomMajorColor = (value != _linePenGlue.Pen);
        CustomMajorThickness = (value.Width != _linePenGlue.Pen.Width);
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
        CustomMinorColor = (value != _linePenGlue.Pen);
        CustomMinorThickness = (value.Width != _linePenGlue.Pen.Width);
      }
    }

    public float MajorTickLength
    {
      get
      {
        return _lineMajorLength.Thickness;
      }
      set
      {
        _lineMajorLength.Thickness = value;
      }
    }

    public float MinorTickLength
    {
      get
      {
        return _lineMinorLength.Thickness;
      }
      set
      {
        _lineMinorLength.Thickness = value;
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
    public bool ShowMajor
    {
      get
      {
        return _chkEnableMajor.Checked;
      }
      set
      {
        _chkEnableMajor.Checked = value;
      }
    }
    public bool ShowMinor
    {
      get
      {
        return _chkEnableMinor.Checked;
      }
      set
      {
        _chkEnableMinor.Checked = value;
      }
    }
    bool CustomMajorThickness
    {
      set
      {
        _chkCustomMajorThickness.Checked = value;
        _lineMajorThickness.Enabled = (value && ShowMajor);

      }
    }
    bool CustomMinorThickness
    {
      set
      {
        _chkCustomMinorThickness.Checked = value;
        _lineMinorThickness.Enabled = (value && ShowMinor);

      }
    }

    bool CustomMajorColor
    {
      set
      {
        this._chkCustomMajorColor.Checked = value;
        this._majorLineColor.Enabled = (value && ShowMajor);
      }
    }
    bool CustomMinorColor
    {
      set
      {
        this._chkCustomMinorColor.Checked = value;
        this._minorLineColor.Enabled = (value && ShowMinor);
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
          _majorWhichTicksLayout.Controls.Add(chk);
        }
      }
    }

    #endregion
  }

  public interface IAxisLineStyleView
  {
    bool ShowLine { get; set; }
    bool ShowMajor { get; set; }
    bool ShowMinor { get; set; }
    PenX LinePen { get; set; }
    PenX MajorPen{get; set; }
    PenX MinorPen{ get; set; }
    float MajorTickLength { get; set; }
    float MinorTickLength { get; set; }
    SelectableListNodeList MajorPenTicks { get; set; }
    SelectableListNodeList MinorPenTicks { get; set; }
  }
}
