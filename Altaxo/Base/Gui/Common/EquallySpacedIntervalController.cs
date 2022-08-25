#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Collections;
using Altaxo.Serialization;

namespace Altaxo.Gui.Common
{
  #region Interfaces

  public interface IEquallySpacedIntervalView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for an equally spaced interval.
  /// </summary>
  [ExpectedTypeOfView(typeof(IEquallySpacedIntervalView))]
  public class EquallySpacedIntervalController : MVCANControllerEditImmutableDocBase<ISpacedInterval, IEquallySpacedIntervalView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public EquallySpacedIntervalController()
    {
    }

    public EquallySpacedIntervalController(ISpacedInterval doc)
    {
      _doc = _originalDoc = doc ?? throw new ArgumentNullException(nameof(doc));
      Initialize(true);
    }

    #region Bindings

    private ItemsController<Type> _intervalChoice;

    public ItemsController<Type> IntervalChoice
    {
      get => _intervalChoice;
      set
      {
        if (!(_intervalChoice == value))
        {
          _intervalChoice = value;
          OnPropertyChanged(nameof(IntervalChoice));
        }
      }
    }

    private SelectableListNodeList _previewList;

    public SelectableListNodeList PreviewList
    {
      get => _previewList;
      set
      {
        if (!(_previewList == value))
        {
          _previewList = value;
          OnPropertyChanged(nameof(PreviewList));
        }
      }
    }

    private double _start;

    public double Start
    {
      get => _start;
      set
      {
        if (!(_start == value))
        {
          _start = value;
          OnPropertyChanged(nameof(Start));
          if (IsStartEnabled)
          {
            OnStartEndStepCountChanged(_doc.GetType());
          }
        }
      }
    }

    private bool _isStartEnabled;

    public bool IsStartEnabled
    {
      get => _isStartEnabled;
      set
      {
        if (!(_isStartEnabled == value))
        {
          _isStartEnabled = value;
          OnPropertyChanged(nameof(IsStartEnabled));
        }
      }
    }


    private double _end;

    public double End
    {
      get => _end;
      set
      {
        if (!(_end == value))
        {
          _end = value;
          OnPropertyChanged(nameof(End));
          if (IsEndEnabled)
          {
            OnStartEndStepCountChanged(_doc.GetType());
          }
        }
      }
    }

    private bool _isEndEnabled;

    public bool IsEndEnabled
    {
      get => _isEndEnabled;
      set
      {
        if (!(_isEndEnabled == value))
        {
          _isEndEnabled = value;
          OnPropertyChanged(nameof(IsEndEnabled));
        }
      }
    }


    private double _step;

    public double Step
    {
      get => _step;
      set
      {
        if (!(_step == value))
        {
          _step = value;
          OnPropertyChanged(nameof(Step));
          if (IsStepEnabled)
          {
            OnStartEndStepCountChanged(_doc.GetType());
          }
        }
      }
    }

    private bool _isStepEnabled;

    public bool IsStepEnabled
    {
      get => _isStepEnabled;
      set
      {
        if (!(_isStepEnabled == value))
        {
          _isStepEnabled = value;
          OnPropertyChanged(nameof(IsStepEnabled));
        }
      }
    }


    private int _count;

    public int Count
    {
      get => _count;
      set
      {
        if (!(_count == value))
        {
          _count = value;
          OnPropertyChanged(nameof(Count));
          if(IsCountEnabled)
          {
            OnStartEndStepCountChanged(_doc.GetType());
          }
        }
      }
    }

    private bool _isCountEnabled;

    public bool IsCountEnabled
    {
      get => _isCountEnabled;
      set
      {
        if (!(_isCountEnabled == value))
        {
          _isCountEnabled = value;
          OnPropertyChanged(nameof(IsCountEnabled));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        // first, set the properties silently
        _start = _doc.Start;
        _end = _doc.End;
        _step = _doc.Step;
        _count = _doc.Count;

        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ISpacedInterval));

        IntervalChoice = new ItemsController<Type>(
          new SelectableListNodeList(types.Select(t => new SelectableListNode(t.Name, t, false))),
          EhIntervalTypeChanged);
        IntervalChoice.SelectedValue = _doc.GetType();

        OnStartEndStepCountChanged(_doc.GetType());
      }
    }

    private void EhIntervalTypeChanged(Type newType)
    {
      if(newType is not null && _doc.GetType() != newType)
      {
        OnStartEndStepCountChanged(newType);
      }
    }

    void OnStartEndStepCountChanged(Type type)
    {
      if (true == CreateDocument(type, true))
      {
        CreatePreviewList();

        Start = _doc.Start;
        End = _doc.End;
        Step = _doc.Step;
        Count = _doc.Count;
      }

      IsStartEnabled = _doc.IsStartEditable;
      IsEndEnabled = _doc.IsEndEditable;
      IsStepEnabled = _doc.IsStepEditable;
      IsCountEnabled = _doc.IsCountEditable;
    }

    bool CreateDocument(Type newType, bool silentlyIgnoreException=true)
    {
      try
      {
        if (newType.Name.EndsWith("StartCountStep"))
        {
          _doc = (ISpacedInterval)Activator.CreateInstance(newType, new object[] { Start, Count, Step });
        }
        else if (newType.Name.EndsWith("EndCountStep"))
        {
          _doc = (ISpacedInterval)Activator.CreateInstance(newType, new object[] { End, Count, Step });
        }
        else if (newType.Name.EndsWith("StartEndCount"))
        {
          _doc = (ISpacedInterval)Activator.CreateInstance(newType, new object[] { Start, End, Count });
        }
        else if (newType.Name.EndsWith("StartEndStep"))
        {
          _doc = (ISpacedInterval)Activator.CreateInstance(newType, new object[] { Start, End, Step });
        }
        else
        {
          Current.Gui.ErrorMessageBox($"Can not construct object of type {newType}");
          return false;
        }
      }
      catch(Exception ex)
      {
        // if we have a brand new type, maybe the current parameters are not appropriate
        // thus we try to use a default constructor
        if(newType != _doc.GetType())
        {
          try
          {
            _doc = (ISpacedInterval)Activator.CreateInstance(newType);
            return true;
          }
          catch(Exception)
          {
          }
        }

        if (!silentlyIgnoreException)
        {
          Current.Gui.ErrorMessageBox($"Can not create {newType.Name}. Reason: {(ex is System.Reflection.TargetInvocationException ex1? ex1.InnerException: ex.Message)}");
        }
        return false;
      }
      return true;
    }

    void CreatePreviewList()
    {
      SelectableListNodeList list;

      if(_doc.Count<=7)
      {
        list = new SelectableListNodeList(_doc.Select(number => new SelectableListNode(number.ToString(), number, false)));
      }
      else
      {
        list = new SelectableListNodeList(_doc.Take(3).Select(number => new SelectableListNode(number.ToString(), null, false)));
        list.Add(new SelectableListNode("...", null, false));
        for (int i = _doc.Count - 3; i< _doc.Count; ++i)
          list.Add(new SelectableListNode(_doc[i].ToString(), null, false));
      }

      PreviewList = list;
    }


    public override bool Apply(bool disposeController)
    {
      if (false == CreateDocument(IntervalChoice.SelectedValue, silentlyIgnoreException: false))
        return ApplyEnd(false, disposeController);

      return ApplyEnd(true, disposeController);
    }
  }
}
