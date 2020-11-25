#region Copyright

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
using Altaxo.Data;
using Altaxo.Main;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface IMasterCurveCreationDataView
  {
    void InitializeListData(List<SelectableListNodeList> list);
  }

  #endregion Interfaces

  /// <summary>
  /// Responsible for the ordering of multiple curves that subsequently can be used to form a master curve.
  /// </summary>
  [UserControllerForObject(typeof(List<List<DoubleColumn>>))]
  [ExpectedTypeOfView(typeof(IMasterCurveCreationDataView))]
  public class MasterCurveCreationDataController : IMVCANController
  {
    private IMasterCurveCreationDataView _view;
    private List<List<DoubleColumn>> _doc;
    private List<List<DoubleColumn>> _docOriginal;

    private List<SelectableListNodeList> _viewList;

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _viewList = new List<SelectableListNodeList>();

        foreach (var srcGroup in _doc)
        {
          var destGroup = new SelectableListNodeList();
          _viewList.Add(destGroup);
          foreach (var srcEle in srcGroup)
          {
            var destEle = new SelectableListNode(AbsoluteDocumentPath.GetAbsolutePath(srcEle).ToString(), srcEle, false);
            destGroup.Add(destEle);
          }
        }
      }
      if (_view is not null)
      {
        _view.InitializeListData(_viewList);
      }
    }

    private void CopyDoc(List<List<DoubleColumn>> src, List<List<DoubleColumn>> dest)
    {
      dest.Clear();
      foreach (var e1 in src)
      {
        var destElement = new List<DoubleColumn>();
        dest.Add(destElement);
        foreach (var e2 in e1)
        {
          destElement.Add(e2);
        }
      }
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0 || !(args[0] is List<List<DoubleColumn>>))
        return false;

      _docOriginal = args[0] as List<List<DoubleColumn>>;
      _doc = new List<List<DoubleColumn>>();
      CopyDoc(_docOriginal, _doc);

      Initialize(true);

      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IMasterCurveCreationDataView;

        if (_view is not null)
        {
          Initialize(false);
        }
      }
    }

    public object ModelObject
    {
      get { return _docOriginal; }
    }

    public void Dispose()
    {
    }

    public bool Apply(bool disposeController)
    {
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
