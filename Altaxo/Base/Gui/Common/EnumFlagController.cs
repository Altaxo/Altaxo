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

#nullable enable
using System;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public interface IEnumFlagView : IDataContextAwareView
  {
  }

  [Obsolete("Use Altaxo.Gui.Common.BasicTypes.EnumValueController")]
  public class EnumFlagController : IMVCANController
  {
    private System.Enum? _doc;
    private long _tempDoc;
    private IEnumFlagView? _view;

    private SelectableListNodeList? _list = new();

    private int _checkedChangeLock = 0;

    private Exception NoDocumentException => new InvalidOperationException("This controller is not yet initialized with a document!");

    private Exception NotInitializedException => new InvalidProgramException("This controller has a document, but was not properly initialized!");

    #region Bindings

    public SelectableListNodeList Choices
    {
      get => _list;
    }

    #endregion


    private void Initialize(bool initData)
    {
      if (_doc is null)
        throw NoDocumentException;

      if (initData)
      {
        _list.Clear();
        var values = System.Enum.GetValues(_doc.GetType());
        foreach (var val in values)
        {
          var node = new SelectableListNode(System.Enum.GetName(_doc.GetType(), val!) ?? string.Empty, val, IsChecked(val, _tempDoc));
          node.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(EhNode_PropertyChanged);
          _list.Add(node);
        }
      }
    }

    private void EhNode_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (0 != _checkedChangeLock || "IsSelected" != e.PropertyName)
        return;

      if (sender is not SelectableListNode node)
        return;


      bool b = node.IsSelected;
      long x = Convert.ToInt64(node.Tag); // get the selected flag

      if (b && (x == 0)) // if the "None" flag is selected, then no flag should be selected, so _tempDoc must be 0
      {
        _tempDoc = 0;
      }
      else // a "normal" flag is selected
      {
        if (b)
          _tempDoc |= x;
        else
          _tempDoc &= ~x;
      }

      ++_checkedChangeLock; // avoid recursive calls when changing the checks in the view
      CalculateChecksFromDoc();
      --_checkedChangeLock;
    }

    private static bool IsChecked(object? flag, long document)
    {
      long x = Convert.ToInt64(flag);
      if (x == 0)
        return 0 == document;
      else
        return (x == (x & document));
    }

    private void CalculateChecksFromDoc()
    {
      if (_list is null) throw NotInitializedException;

      foreach (var n in _list)
      {
        n.IsSelected = IsChecked(n.Tag, _tempDoc);
      }
    }

    private void CalculateEnumFromChecks()
    {
      if (_list is null) throw NotInitializedException;

      // calculate enum from checks
      long sum = 0;
      for (int i = 0; i < _list.Count; i++)
      {
        long x = Convert.ToInt64(_list[i].Tag);
        if (_list[i].IsSelected)
          sum |= x;
      }
      _tempDoc = sum;
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is System.Enum))
        return false;

      _doc = (System.Enum)args[0];
      _tempDoc = ((IConvertible)_doc).ToInt64(System.Globalization.CultureInfo.InvariantCulture);

      Initialize(true);

      return true;
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
        DetachView();

        _view = value as IEnumFlagView;

        if (_view is not null)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    void AttachView()
    {
      if (_view is { } view)
        view.DataContext = this.Choices;
    }
    void DetachView()
    {
      if (_view is { } view)
        view.DataContext = null;
    }

    public object ModelObject
    {
      get
      {
        return _doc ?? throw NoDocumentException; ;
      }
    }

    public void Dispose()
    {
      DetachView();
    }

    #endregion IMVCController Members

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      if (_doc is null) throw NoDocumentException;
      _doc = (System.Enum)System.Enum.ToObject(_doc.GetType(), _tempDoc);
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
