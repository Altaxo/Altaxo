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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Properties;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public class ParameterSetViewItem : INotifyPropertyChanged
  {
    private double _value;

    public string Name { get; set; }

    public double Value
    {
      get => _value;
      set
      {
        if (!(_value == value))
        {
          _value = value;
          OnPropertyChanged(nameof(Value));
        }
      }
    }

    private bool _vary;

    public bool Vary
    {
      get => _vary;
      set
      {
        if (!(_vary == value))
        {
          _vary = value;
          OnPropertyChanged(nameof(Vary));
        }
      }
    }


    private double _variance;

    public double Variance
    {
      get => _variance;
      set
      {
        if (!(_variance == value))
        {
          _variance = value;
          OnPropertyChanged(nameof(Variance));
        }
      }
    }


    private double? _lowerBound;

    public double? LowerBound
    {
      get => _lowerBound;
      set
      {
        if (!(_lowerBound == value))
        {
          _lowerBound = value;
          OnPropertyChanged(nameof(LowerBound));
        }
      }
    }

    private double? _upperBound;

    public double? UpperBound
    {
      get => _upperBound;
      set
      {
        if (!(_upperBound == value))
        {
          _upperBound = value;
          OnPropertyChanged(nameof(UpperBound));
        }
      }
    }

    private bool _isLowerBoundExclusive;

    public bool IsLowerBoundExclusive
    {
      get => _isLowerBoundExclusive;
      set
      {
        if (!(_isLowerBoundExclusive == value))
        {
          _isLowerBoundExclusive = value;
          OnPropertyChanged(nameof(IsLowerBoundExclusive));
        }
      }
    }

    private bool _isUpperBoundExclusive;

    public bool IsUpperBoundExclusive
    {
      get => _isUpperBoundExclusive;
      set
      {
        if (!(_isUpperBoundExclusive == value))
        {
          _isUpperBoundExclusive = value;
          OnPropertyChanged(nameof(IsUpperBoundExclusive));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public interface IParameterSetView : IDataContextAwareView
  {
  }



  /// <summary>
  /// Summary description for ParameterSetController.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSet))]
  [ExpectedTypeOfView(typeof(IParameterSetView))]
  public class ParameterSetController1 : MVCANControllerEditOriginalDocBase<ParameterSet, IParameterSetView>
  {
    public ObservableCollection<ParameterSetViewItem> ParameterList { get; } = new ObservableCollection<ParameterSetViewItem>();

    public SelectableListNodeList LowerBoundConditions { get; } = new SelectableListNodeList {
      new SelectableListNode(">=", false, false),
      new SelectableListNode(">", true, false),
      };

    public SelectableListNodeList UpperBoundConditions { get; } = new SelectableListNodeList {
      new SelectableListNode("<=", false, false),
      new SelectableListNode("<", true, false),
      };


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public void OnParametersChanged()
    {
      ParameterList.Clear();

      for (int i = 0; i < _doc.Count; i++)
      {
        var item = new ParameterSetViewItem
        {
          Name = _doc[i].Name,
          Value = _doc[i].Parameter,
          Vary = _doc[i].Vary,
          Variance = _doc[i].Variance,
          LowerBound = _doc[i].LowerBound,
          UpperBound = _doc[i].UpperBound,
          IsLowerBoundExclusive = _doc[i].IsLowerBoundExclusive,
          IsUpperBoundExclusive = _doc[i].IsUpperBoundExclusive,
        };

        ParameterList.Add(item);
      }
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      OnParametersChanged();
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.DataContext = this;
    }

    protected override void DetachView()
    {
      _view.DataContext = null;
      base.DetachView();
    }


    public override bool Apply(bool disposeController)
    {
      var list = ParameterList;

      for (int i = 0; i < _doc.Count; i++)
      {

        // Parameter
        _doc[i].Parameter = list[i].Value;

        // Vary
        _doc[i].Vary = list[i].Vary;

        // Variance
        _doc[i].Variance = list[i].Variance;

        // Bounds
        _doc[i].LowerBound = list[i].LowerBound;
        _doc[i].UpperBound = list[i].UpperBound;
        _doc[i].IsLowerBoundExclusive = list[i].IsLowerBoundExclusive;
        _doc[i].IsUpperBoundExclusive = list[i].IsUpperBoundExclusive;
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
