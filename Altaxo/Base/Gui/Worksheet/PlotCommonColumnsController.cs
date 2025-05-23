﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Worksheet.Commands;

namespace Altaxo.Gui.Worksheet
{
  public interface IPlotCommonColumnsView
  {
    void InitializeXCommonColumns(SelectableListNodeList list);

    void InitializeYCommonColumns(SelectableListNodeList list);

    bool UseCurrentXColumn { get; set; }
  }

  [ExpectedTypeOfView(typeof(IPlotCommonColumnsView))]
  [UserControllerForObject(typeof(PlotCommonColumnsCommand))]
  public class PlotCommonColumnsController : IMVCANController
  {
    private IPlotCommonColumnsView _view;
    private PlotCommonColumnsCommand _doc;

    private SelectableListNodeList _xCommonColumns;
    private SelectableListNodeList _yCommonColumns;

    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0 || !(args[0] is PlotCommonColumnsCommand))
        return false;

      _doc = (PlotCommonColumnsCommand)args[0];
      Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _xCommonColumns = new SelectableListNodeList();
        _yCommonColumns = new SelectableListNodeList();

        var names = _doc.GetCommonColumnNamesOrderedByAppearanceInFirstTable();
        bool isFirst = true;
        foreach (var name in names)
        {
          _xCommonColumns.Add(new SelectableListNode(name, name, name == _doc.XCommonColumnNameForPlot || (isFirst && _doc.XCommonColumnNameForPlot is null)));
          _yCommonColumns.Add(new SelectableListNode(name, name, false));
          isFirst = false;
        }
      }

      if (_view is not null)
      {
        _view.InitializeXCommonColumns(_xCommonColumns);
        _view.InitializeYCommonColumns(_yCommonColumns);
        _view.UseCurrentXColumn = _doc.XCommonColumnNameForPlot is null;
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
        if (_view is not null)
        {
        }

        _view = value as IPlotCommonColumnsView;

        if (_view is not null)
        {
          Initialize(false);
        }
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    public void Dispose()
    {
    }

    public bool Apply(bool disposeController)
    {
      if (_view.UseCurrentXColumn)
        _doc.XCommonColumnNameForPlot = null;
      else
        _doc.XCommonColumnNameForPlot = _xCommonColumns.FirstSelectedNode is null ? null : (string)_xCommonColumns.FirstSelectedNode.Tag;

      _doc.YCommonColumnNamesForPlotting.Clear();
      foreach (var node in _yCommonColumns.Where(x => x.IsSelected))
        _doc.YCommonColumnNamesForPlotting.Add((string)node.Tag);

      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }
  }
}
