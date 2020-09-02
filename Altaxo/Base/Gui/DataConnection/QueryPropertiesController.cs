#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.DataConnection;

namespace Altaxo.Gui.DataConnection
{
  public interface IQueryPropertiesView
  {
    void UpdateDialogValues(bool isDistinct, int topN, SelectableListNodeList groupBy);

    int GetTopN();

    bool GetDistinct();
  }

  [ExpectedTypeOfView(typeof(IQueryPropertiesView))]
  public class QueryPropertiesController : IMVCAController
  {
    private IQueryPropertiesView _view;
    private QueryBuilder _builder;
    private SelectableListNodeList _groupByChoices;

    public QueryPropertiesController(QueryBuilder builder)
    {
      _builder = builder;
      Initialize(true);
    }

    public QueryBuilder QueryBuilder
    {
      get { return _builder; }
      set
      {
        if (_builder != value)
        {
          _builder = value;
          Initialize(false);
        }
      }
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _groupByChoices = new SelectableListNodeList();
        _groupByChoices.FillWithEnumeration(_builder.GroupByExtension);
      }
      if (_view is not null)
      {
        _groupByChoices.ClearSelectionsAll();
        if (_builder.GroupBy)
        {
          _groupByChoices[(int)_builder.GroupByExtension].IsSelected = true;
        }
        _view.UpdateDialogValues(_builder.Distinct, _builder.Top, _groupByChoices);
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
        _view = value as IQueryPropertiesView;
        if (_view is not null)
        {
          Initialize(false);
        }
      }
    }

    public object ModelObject
    {
      get { return null; }
    }

    public void Dispose()
    {
      ViewObject = null;
    }

    public bool Apply(bool disposeController)
    {
      _builder.Top = _view.GetTopN();
      if (_builder.GroupBy)
      {
        _builder.GroupByExtension = (GroupByExtension)_groupByChoices.FirstSelectedNode.Tag;
      }
      _builder.Distinct = true == _view.GetDistinct();

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
