using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.Plot.Groups;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IWaterfallTransformView
  {
    string XScale { get; set; }
    string YScale { get; set; }
    bool UseClipping { get; set; }
  }

  #endregion

  [UserControllerForObject(typeof(WaterfallTransform))]
  [ExpectedTypeOfView(typeof(IWaterfallTransformView))]
  public class WaterfallTransformController : IMVCANController
  {
    IWaterfallTransformView _view;
    WaterfallTransform _doc;

    void Initialize(bool a)
    {
      if (_view != null)
      {
        _view.XScale = Altaxo.Serialization.GUIConversion.ToString(_doc.XScale);
        _view.YScale = Altaxo.Serialization.GUIConversion.ToString(_doc.YScale);
        _view.UseClipping = _doc.UseClipping;
      }
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;
      WaterfallTransform doc = args[0] as WaterfallTransform;
      if (doc == null)
        return false;
      _doc = doc;
      Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }


    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IWaterfallTransformView;
        if (_view != null)
        {
          Initialize(false);
        }
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
      _doc.UseClipping = _view.UseClipping;

      double xscale, yscale;

      if (Altaxo.Serialization.GUIConversion.IsDouble(_view.XScale, out xscale))
      {
        _doc.XScale = xscale;
      }
      else
      {
        Current.Gui.ErrorMessageBox("XScale must contain a valid number");
        return false;
      }
      if (Altaxo.Serialization.GUIConversion.IsDouble(_view.YScale, out yscale))
      {
        _doc.YScale = yscale;
      }
      else
      {
        Current.Gui.ErrorMessageBox("YScale must contain a valid number");
        return false;
      }


      return true;
    }

    #endregion
  }
}
