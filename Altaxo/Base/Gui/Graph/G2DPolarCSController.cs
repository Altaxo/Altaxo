using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.CS;

namespace Altaxo.Gui.Graph
{
  [Main.GUI.UserControllerForObject(typeof(G2DPolarCoordinateSystem),101)]
  [Main.GUI.ExpectedTypeOfView(typeof(IG2DCartesicCSView))]
  public class G2DPolarCSController : Main.GUI.IMVCAController
  {
    IG2DCartesicCSView _view;
    G2DPolarCoordinateSystem _doc;

    public G2DPolarCSController(G2DPolarCoordinateSystem doc)
    {
      _doc = doc;
      Initialize(true);
    }

    #region IMVCController Members


    void Initialize(bool bInit)
    {
      if (_view != null)
      {
        _view.ExchangeXY = _doc.IsXYInterchanged;
        _view.ReverseX = _doc.IsXReverse;
        _view.ReverseY = _doc.IsYReverse;
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
        _view = value as IG2DCartesicCSView;
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
      _doc.IsXYInterchanged = _view.ExchangeXY;
      _doc.IsXReverse = _view.ReverseX;
      _doc.IsYReverse = _view.ReverseY;
      return true;

    }

    #endregion


  }
}
