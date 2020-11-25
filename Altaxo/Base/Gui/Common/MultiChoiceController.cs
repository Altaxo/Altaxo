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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Document class for the <see cref="MultiChoiceController"/>.
  /// </summary>
  public class MultiChoiceList
  {
    /// <summary>The description text shown above the list of items.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>List of items. Has to be filled before showing the control.</summary>
    public SelectableListNodeList List { get; protected set; }

    /// <summary>List of column names. The number of names must match the number of subitems in the list items.</summary>
    public List<string> ColumnNameList { get { return _columnNames; } }

    /// <summary>Set the list of column names (can be used in the initializer of this object).</summary>
    public string[] ColumnNames
    {
      set
      {
        _columnNames.Clear();
        _columnNames.AddRange(value);
      }
    }

    private List<string> _columnNames;

    /// <summary>
    /// Constructs an empty list with no items and no column names.
    /// </summary>
    public MultiChoiceList()
    {
      List = new SelectableListNodeList();
      _columnNames = new List<string>();
    }
  }

  /// <summary>Gui view interface for use with the <see cref="MultiChoiceController"/>.</summary>
  public interface IMultiChoiceView
  {
    /// <summary>Initialize the description text.</summary>
    /// <param name="value">Description text.</param>
    void InitializeDescription(string value);

    /// <summary>
    /// Initialize the column names.
    /// </summary>
    /// <param name="colNames">Column names.</param>
    void InitializeColumnNames(string[] colNames);

    /// <summary>
    /// Initializes the list.
    /// </summary>
    /// <param name="list">List of items shown.</param>
    void InitializeList(SelectableListNodeList list);
  }

  /// <summary>
  /// Controller for the <see cref="MultiChoiceList"/> document type.
  /// </summary>
  [ExpectedTypeOfView(typeof(IMultiChoiceView))]
  [UserControllerForObject(typeof(MultiChoiceList))]
  public class MultiChoiceController : IMVCANController
  {
    private MultiChoiceList? _doc;
    private IMultiChoiceView? _view;

    private Exception NoDocumentException => new InvalidOperationException("This controller is not yet initialized with a document!");


    protected void Initialize(bool initData)
    {
      if (_doc is null)
        throw NoDocumentException;

      if (_view is not null)
      {
        _view.InitializeDescription(_doc.Description);

        if (_doc.ColumnNameList.Count == 0)
          _view.InitializeColumnNames(new string[] { "Name" });
        else
          _view.InitializeColumnNames(_doc.ColumnNameList.ToArray());

        _view.InitializeList(_doc.List);
      }
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length > 0 && args[0] is MultiChoiceList)
      {
        _doc = (MultiChoiceList)args[0];
        Initialize(true);
        return true;
      }
      else
        return false;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    #endregion IMVCANController Members

    #region IMVCController Members

    public object? ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IMultiChoiceView;
        if (_view is not null)
        {
          Initialize(false);
        }
      }
    }

    public object ModelObject
    {
      get
      {
        if (_doc is null)
          throw NoDocumentException;

        return _doc;
      }
    }

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

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

    #endregion IApplyController Members
  }
}
