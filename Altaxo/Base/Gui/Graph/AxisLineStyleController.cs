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

using Altaxo.Collections;
using Altaxo.Graph.Gdi.Axis;

namespace Altaxo.Gui.Graph
{
  [UserControllerForObject(typeof(AxisLineStyle))]
  [ExpectedTypeOfView(typeof(IAxisLineStyleView))]
  public class AxisLineStyleController : IMVCAController
  {
    AxisLineStyle _doc;
    IAxisLineStyleView _view;


    public AxisLineStyleController(AxisLineStyle doc)
    {
      _doc = (AxisLineStyle)doc.Clone();
      Initialize(true);
    }


    #region IMVCController Members


    void Initialize(bool bInit)
    {
      if (_view != null)
      {
        _view.ShowLine = true;

        _view.LinePen = _doc.AxisPen;
        _view.MajorPen = _doc.MajorPen;
        _view.MinorPen = _doc.MinorPen;

        _view.MajorTickLength = _doc.MajorTickLength;
        _view.MinorTickLength = _doc.MinorTickLength;

        SelectableListNodeList list = new SelectableListNodeList();

        list.Clear();
        if (_doc.CachedAxisInformation != null)
        {
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, null, _doc.FirstDownMajorTicks));
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, null, _doc.FirstUpMajorTicks));
        }
        _view.MajorPenTicks = list;


        list.Clear();
        if (_doc.CachedAxisInformation != null)
        {
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, null, _doc.FirstDownMinorTicks));
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, null, _doc.FirstUpMinorTicks));
        }
        _view.MinorPenTicks = list;

      }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IAxisLineStyleView;
        Initialize(false);
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.AxisPen = _view.LinePen;
      _doc.MajorPen = _view.MajorPen;
      _doc.MinorPen = _view.MinorPen;
      _doc.MajorTickLength = _view.MajorTickLength;
      _doc.MinorTickLength = _view.MinorTickLength;

      SelectableListNodeList list;
      list = _view.MajorPenTicks;
      _doc.FirstDownMajorTicks = list[0].Selected;
      _doc.FirstUpMajorTicks = list[1].Selected;

      list = _view.MinorPenTicks;
      _doc.FirstDownMinorTicks = list[0].Selected;
      _doc.FirstUpMinorTicks = list[1].Selected;


      return true;

    }

    #endregion


  }
}
