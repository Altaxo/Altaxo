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
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public class ParameterSetViewItem
  {
    public string Name { get; set; }

    public double Value { get; set; }

    public bool Vary { get; set; }

    public double Variance { get; set; }
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
          Variance = _doc[i].Variance
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

      }

      return ApplyEnd(true, disposeController);
    }
  }
}
