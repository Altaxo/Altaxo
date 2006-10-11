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
    AxisLineStyle _tempDoc;
    IAxisLineStyleView _view;


    public AxisLineStyleController(AxisLineStyle doc)
    {
      _doc = doc;
      _tempDoc = (AxisLineStyle)doc.Clone();
      Initialize(true);
    }


    #region IMVCController Members


    void Initialize(bool bInit)
    {
      if (_view != null)
      {
        _view.ShowLine = true;
        _view.ShowMajor = _tempDoc.MajorTickLength > 0;
        _view.ShowMinor = _tempDoc.MinorTickLength > 0;

        _view.LinePen = _tempDoc.AxisPen;
        _view.MajorPen = _tempDoc.MajorPen;
        _view.MinorPen = _tempDoc.MinorPen;

        _view.MajorTickLength = _tempDoc.MajorTickLength;
        _view.MinorTickLength = _tempDoc.MinorTickLength;

        SelectableListNodeList list = new SelectableListNodeList();

        list.Clear();
        list.Add(new SelectableListNode("Left", null, _tempDoc.LeftSideMajorTicks));
        list.Add(new SelectableListNode("Right", null, _tempDoc.RightSideMajorTicks));

        _view.MajorPenTicks = list;


        list.Clear();
        list.Add(new SelectableListNode("Left", null, _tempDoc.LeftSideMinorTicks));
        list.Add(new SelectableListNode("Right", null, _tempDoc.RightSideMinorTicks));
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
      _doc.LeftSideMajorTicks = list[0].Selected;
      _doc.RightSideMajorTicks = list[1].Selected;

      list = _view.MinorPenTicks;
      _doc.LeftSideMinorTicks = list[0].Selected;
      _doc.RightSideMinorTicks = list[1].Selected;


      return true;

    }

    #endregion


  }
}
