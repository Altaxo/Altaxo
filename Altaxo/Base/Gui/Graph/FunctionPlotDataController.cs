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

using Altaxo.Graph.Plot.Data;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IFunctionPlotDataView
  {
    event EventHandler EditText;
    void InitializeFunctionText(string text, bool editable);

  }
  #endregion

  [UserControllerForObject(typeof(XYFunctionPlotData))]
  [ExpectedTypeOfView(typeof(IFunctionPlotDataView))]
  class FunctionPlotDataController : IMVCANController
  {
    XYFunctionPlotData _doc, _originalDoc;
    UseDocument _useDocumentCopy;
    IFunctionPlotDataView _view;

    IMVCAController _functionController;

    public FunctionPlotDataController()
    {
    }

    void Initialize(bool initDoc)
    {
      if (initDoc)
      {
        // try to find a controller for the underlying function
        _functionController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.Function }, typeof(IMVCAController), UseDocument.Directly);
      }
      if (_view != null)
      {
        bool editable = null != _functionController;
        string text;
        if (_doc.Function is Altaxo.Scripting.IScriptText)
          text = ((Altaxo.Scripting.IScriptText)_doc.Function).ScriptText;
        else
          text = _doc.Function.ToString();
      
        _view.InitializeFunctionText(text,editable);

      }

    }

    void EhView_EditText(object sender, EventArgs e)
    {
      if (Current.Gui.ShowDialog(_functionController, "Edit script"))
      {
        _doc.Function = (Altaxo.Calc.IScalarFunctionDD)_functionController.ModelObject;
        Initialize(false);
      }
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is XYFunctionPlotData))
        return false;
      _originalDoc = (XYFunctionPlotData)args[0];
      _doc = _useDocumentCopy == UseDocument.Directly ? _originalDoc : (XYFunctionPlotData)_originalDoc.Clone();
      Initialize(true); // initialize always because we have to update the temporary variables
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { _useDocumentCopy = value; }
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
        if (_view != null)
        {
          _view.EditText -= this.EhView_EditText;
        }

        _view = value as IFunctionPlotDataView;
        if (_view != null)
        {
          _view.EditText += this.EhView_EditText;
          Initialize(false);
        }
      }
    }

    public object ModelObject
    {
      get { return _originalDoc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if (!object.ReferenceEquals(_doc, _originalDoc))
        _originalDoc.CopyFrom(_doc);

      return true;
    }

    #endregion
  }
}
