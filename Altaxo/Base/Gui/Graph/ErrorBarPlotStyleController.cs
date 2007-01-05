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

using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IErrorBarPlotStyleView
  {
    bool IndependentColor { get; set; }
    PenX StrokePen { get; set; }
    bool IndependentSize { get; set; }
    bool LineSymbolGap { get; set; }
    bool ShowEndBars { get; set; }
    bool DoNotShiftIndependentVariable { get; set; }
    bool IsHorizontalStyle { get; set; }
    void InitializeSymbolSizeList(string[] names, int selection);
    string SymbolSize { get; }
    bool IndependentNegativeError { get; set; }
    string PositiveError { get; set; }
    string NegativeError { get; set; }
    string SkipFrequency { get; set; }
    event EventHandler ChoosePositiveError;
    event EventHandler ChooseNegativeError;
    event EventHandler IndependentNegativeError_CheckChanged;
    event System.ComponentModel.CancelEventHandler VerifySymbolSize;
    event System.ComponentModel.CancelEventHandler VerifySkipFrequency;
    event EventHandler ClearPositiveError;
    event EventHandler ClearNegativeError;
  }

  #endregion

  [UserControllerForObject(typeof(ErrorBarPlotStyle))]
  [ExpectedTypeOfView(typeof(IErrorBarPlotStyleView))]
  public class ErrorBarPlotStyleController : Gui.IMVCANController
  {
    IErrorBarPlotStyleView _view;
    ErrorBarPlotStyle _doc;

    INumericColumn _tempPosErrorColumn;
    INumericColumn _tempNegErrorColumn;
    double _tempSymbolSize;
    int _tempSkipFreq;


    void Initialize(bool docAlso)
    {
      if (_view != null)
      {
        _view.IndependentColor = _doc.IndependentColor;
        _view.StrokePen = _doc.Pen;
        _view.IndependentSize = _doc.IndependentSymbolSize;
        _view.LineSymbolGap = _doc.SymbolGap;
        _view.ShowEndBars = _doc.ShowEndBars;
        _view.DoNotShiftIndependentVariable = _doc.DoNotShiftIndependentVariable;
        _view.IsHorizontalStyle = _doc.IsHorizontalStyle;

        _tempSymbolSize = _doc.SymbolSize;
        _view.InitializeSymbolSizeList(new string[] { Serialization.GUIConversion.ToString(_tempSymbolSize) }, 0);

        _tempSkipFreq = _doc.SkipFrequency;
        _view.SkipFrequency = Serialization.GUIConversion.ToString(_tempSkipFreq);


        // Errors
        _tempPosErrorColumn = _doc.PositiveErrorColumn;
        _tempNegErrorColumn = _doc.NegativeErrorColumn;
        string posError = null == _tempPosErrorColumn ? string.Empty : _tempPosErrorColumn.FullName;
        string negError = null == _tempNegErrorColumn ? string.Empty : _tempNegErrorColumn.FullName;
        _view.IndependentNegativeError = !object.ReferenceEquals(_tempPosErrorColumn, _tempNegErrorColumn);
        _view.PositiveError = posError;
        _view.NegativeError = negError;
      }
    }

 void EhView_ChoosePositiveError(object sender, EventArgs e)
 {
   SingleColumnChoice choice = new SingleColumnChoice();
   choice.SelectedColumn = _tempPosErrorColumn != null ? _tempPosErrorColumn as DataColumn : _doc.PositiveErrorColumn as DataColumn;
   object choiceAsObject = choice;
   if (Current.Gui.ShowDialog(ref choiceAsObject, "Select error column"))
   {
     choice = (SingleColumnChoice)choiceAsObject;
     if (choice.SelectedColumn is INumericColumn)
     {
       _tempPosErrorColumn = (INumericColumn)choice.SelectedColumn;
       _view.PositiveError = _tempPosErrorColumn.FullName;
       if (!_view.IndependentNegativeError)
       {
         _tempNegErrorColumn = (INumericColumn)choice.SelectedColumn;
         _view.NegativeError = _tempNegErrorColumn.FullName;
       }

     }
   }
 }

    void EhView_ClearPositiveError(object sender, EventArgs e)
    {
      _tempPosErrorColumn = null;
      _view.PositiveError = string.Empty;
    }


 void EhView_ChooseNegativeError(object sender, EventArgs e)
 {
   SingleColumnChoice choice = new SingleColumnChoice();
   choice.SelectedColumn = _tempNegErrorColumn != null ? _tempNegErrorColumn as DataColumn: _doc.NegativeErrorColumn as DataColumn;
   object choiceAsObject = choice;
   if (Current.Gui.ShowDialog(ref choiceAsObject, "Select negative error column"))
   {
     choice = (SingleColumnChoice)choiceAsObject;

     if (choice.SelectedColumn is INumericColumn)
     {
       _tempNegErrorColumn = (INumericColumn)choice.SelectedColumn;
       _view.NegativeError = _tempNegErrorColumn.FullName;
     }
   }
 }

    void EhView_ClearNegativeError(object sender, EventArgs e)
    {
      _tempNegErrorColumn = null;
      _view.NegativeError = string.Empty;
    }


void EhView_IndependentNegativeError_CheckChanged(object sender, EventArgs e)
{

}
    void EhView_VerifySymbolSize(object sender, System.ComponentModel.CancelEventArgs e)
    {
      string s = _view.SymbolSize;
      double val;
      if (Serialization.GUIConversion.IsDouble(s, out val))
      {
        _tempSymbolSize = val;
      }
      else
      {
        e.Cancel = true;
      }
    }

    void EhView_VerifySkipFrequency(object sender, System.ComponentModel.CancelEventArgs e)
    {
      string s = _view.SkipFrequency;
      int val;
      if (Serialization.GUIConversion.IsInteger(s, out val))
      {
        _tempSkipFreq = val;
      }
      else
      {
        e.Cancel = true;
      }
    }


    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;
      ErrorBarPlotStyle doc = args[0] as ErrorBarPlotStyle;
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
        if (_view != null)
        {
          _view.ChoosePositiveError -= EhView_ChoosePositiveError;
          _view.ChooseNegativeError -= EhView_ChooseNegativeError;
          _view.IndependentNegativeError_CheckChanged -= EhView_IndependentNegativeError_CheckChanged;
          _view.VerifySymbolSize -= EhView_VerifySymbolSize;
          _view.VerifySkipFrequency -= EhView_VerifySkipFrequency;
          _view.ChoosePositiveError -= EhView_ClearPositiveError;
          _view.ChooseNegativeError -= EhView_ClearNegativeError;

        }

        _view = value as IErrorBarPlotStyleView;
        Initialize(false);

        if (_view != null)
        {
          _view.ChoosePositiveError += EhView_ChoosePositiveError;
          _view.ChooseNegativeError += EhView_ChooseNegativeError;
          _view.IndependentNegativeError_CheckChanged += EhView_IndependentNegativeError_CheckChanged;
          _view.VerifySymbolSize += EhView_VerifySymbolSize;
          _view.VerifySkipFrequency += EhView_VerifySkipFrequency;
          _view.ChoosePositiveError += EhView_ClearPositiveError;
          _view.ChooseNegativeError += EhView_ClearNegativeError;
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
      _doc.IndependentColor = _view.IndependentColor;
      _doc.Pen = _view.StrokePen;
      _doc.IndependentSymbolSize = _view.IndependentSize;
      _doc.SymbolGap = _view.LineSymbolGap;
      _doc.ShowEndBars = _view.ShowEndBars;
      _doc.DoNotShiftIndependentVariable = _view.DoNotShiftIndependentVariable;
      _doc.IsHorizontalStyle = _view.IsHorizontalStyle;


      //_view.InitializeSymbolSizeList
      _doc.SymbolSize = (float)_tempSymbolSize;

      _doc.SkipFrequency = _tempSkipFreq;

      // Errors
      _doc.PositiveErrorColumn = _tempPosErrorColumn;
      _doc.NegativeErrorColumn = _tempNegErrorColumn;
      return true;
    }

    #endregion
  }
}
