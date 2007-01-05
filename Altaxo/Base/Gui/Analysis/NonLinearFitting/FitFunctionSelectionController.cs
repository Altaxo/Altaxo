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
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Reflection;
using Altaxo.Scripting;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public enum FitFunctionContextMenuStyle
  {
    None, 
    Edit,
    EditAndDelete
  }

  public interface IFitFunctionSelectionView
  {
    IFitFunctionSelectionViewEventSink Controller { get; set; }
    void ClearFitFunctionList();
    void AddFitFunctionList(string rootname, Altaxo.Main.Services.IFitFunctionInformation[] info, FitFunctionContextMenuStyle menustyle);

    void SetRtfDocumentation(string rtfString);
    Color GetRtfBackgroundColor();
  }

  public interface IFitFunctionSelectionViewEventSink
  {
    void EhView_SelectionChanged(IFitFunctionInformation selectedtag);
    void EhView_EditItem(IFitFunctionInformation selectedtag);
    void EhView_RemoveItem(IFitFunctionInformation selectedtag);
  }

  public interface IFitFunctionSelectionController : IMVCAController
  {
    void Refresh();
  }

  public class FitFunctionSelectionController : IFitFunctionSelectionViewEventSink, IMVCAController
  {
    IFitFunction _doc;
    IFitFunctionInformation _tempdoc;
    IFitFunctionSelectionView _view;

    public FitFunctionSelectionController(IFitFunction doc)
    {
      _doc = doc;
      _tempdoc = null;
      Initialize();
    }


    public void Refresh()
    {
      Initialize();
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        _view.ClearFitFunctionList();
        _view.AddFitFunctionList("Builtin functions", Current.FitFunctionService.GetBuiltinFitFunctions(), FitFunctionContextMenuStyle.None);
        _view.AddFitFunctionList("Application functions", Current.FitFunctionService.GetApplicationFitFunctions(), FitFunctionContextMenuStyle.Edit);
        _view.AddFitFunctionList("User functions", Current.FitFunctionService.GetUserDefinedFitFunctions(), FitFunctionContextMenuStyle.EditAndDelete);
        _view.AddFitFunctionList("Document functions", Current.FitFunctionService.GetDocumentFitFunctions(), FitFunctionContextMenuStyle.EditAndDelete);
      }
    }


  
    public void EhView_SelectionChanged(IFitFunctionInformation selectedtag)
    {
        _tempdoc = selectedtag;
    }


    public void EhView_EditItem(IFitFunctionInformation selectedtag)
    {
      if (selectedtag is DocumentFitFunctionInformation)
      {
        EditItem(selectedtag.CreateFitFunction());
      }
      else if (selectedtag is FileBasedFitFunctionInformation)
      {
        IFitFunction func = Altaxo.Main.Services.FitFunctionService.ReadUserDefinedFitFunction(selectedtag as Altaxo.Main.Services.FileBasedFitFunctionInformation);
        EditItem(func);
      }
    }

    public void EhView_RemoveItem(IFitFunctionInformation selectedtag)
    {
      if (selectedtag is DocumentFitFunctionInformation)
      {
        Current.Project.FitFunctionScripts.Remove(selectedtag.CreateFitFunction() as FitFunctionScript);
        Initialize();
      }
      else if (selectedtag is FileBasedFitFunctionInformation)
      {
        Current.FitFunctionService.RemoveUserDefinedFitFunction(selectedtag as Altaxo.Main.Services.FileBasedFitFunctionInformation);
        Initialize();
      }
    }

    void EditItem(IFitFunction func)
    {
      if (null != func)
      {
        object[] args = new object[] { func };
        if (Current.Gui.ShowDialog(args, "Edit fit function script"))
        {
          if (args[0] is FitFunctionScript)
          {
            Altaxo.Gui.Scripting.FitFunctionNameAndCategoryController ctrl = new Altaxo.Gui.Scripting.FitFunctionNameAndCategoryController((FitFunctionScript)args[0]);
            if (Current.Gui.ShowDialog(ctrl, "Store?"))
            {
              // Note: category and/or name can have changed now, so it is more save to
              // completely reinitialize the fit function tree
              Initialize();
            }
          }

        }
      }
    }



      

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IFitFunctionSelectionView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if (_tempdoc == null) // nothing selected, so return the original doc
        return true;


      try
      {
        _doc = _tempdoc.CreateFitFunction();
        return true;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("Can not create fit function. An exception was thrown: " + ex.Message);
      }
      return false;
    }

    #endregion
  }
}
