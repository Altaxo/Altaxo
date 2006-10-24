using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public interface IBrushViewSimple
  {
    BrushX Brush { get; set; }
  }
  public interface IBrushViewAdvanced : IBrushViewSimple
  {
  }

  [UserControllerForObject(typeof(BrushX))]
  [ExpectedTypeOfView(typeof(IBrushViewSimple))]
  public class BrushControllerSimple : IMVCANController
  {
    BrushX _doc;
    UseDocument _usage;
    IBrushViewSimple _view;

    public BrushControllerSimple()
    {
    }

    public BrushControllerSimple(BrushX doc)
    {
      InitializeDocument(doc);
    }


    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;

      if (args[0] != null && !(args[0] is BrushX))
        return false;

      _doc =  (BrushX)args[0];

      return true;
    }

    void Initialize()
    {
      if (_view != null)
      {
        if (_doc != null)
          _view.Brush = _usage == UseDocument.Directly ? _doc : (BrushX)_doc.Clone();
        else
          _view.Brush = BrushX.Empty;
      }
    }

    public UseDocument UseDocumentCopy
    {
      set
      {
        _usage = value;
      }
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
        _view = value as IBrushViewSimple;
        Initialize();
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
      if (_doc != null || _view.Brush.IsVisible)
        _doc = _view.Brush;
      
      return true;
    }

    #endregion
  }


  [UserControllerForObject(typeof(BrushX))]
  [ExpectedTypeOfView(typeof(IBrushViewAdvanced))]
  public class BrushControllerAdvanced : BrushControllerSimple
  {
    public BrushControllerAdvanced()
    {
    }

    public BrushControllerAdvanced(BrushX doc)
      : base(doc)
    {
    }

  }
}
