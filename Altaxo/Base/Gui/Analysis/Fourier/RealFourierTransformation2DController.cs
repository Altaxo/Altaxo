#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Worksheet.Commands.Analysis;

namespace Altaxo.Gui.Analysis.Fourier
{
  public interface IRealFourierTransformation2DView
  {
    bool IsUserDefinedXIncrement { get; set; }

    double XIncrement { get; set; }

    void SetXIncrementWarning(string warning);

    bool IsUserDefinedYIncrement { get; set; }

    double YIncrement { get; set; }

    void SetYIncrementWarning(string warning);

    void SetOutputQuantities(SelectableListNodeList list);

    bool CenterFrequencies { get; set; }

    double ResultingFractionOfRowsUsed { get; set; }

    double ResultingFractionOfColumnsUsed { get; set; }

    int? DataPretreatmentOrder { get; set; }

    double? ReplacementValueForNaNMatrixElements { get; set; }

    double? ReplacementValueForInfiniteMatrixElements { get; set; }

    SelectableListNodeList FourierWindowChoice { set; }

    bool OutputFrequencyHeaderColumns { get; set; }

    string FrequencyRowHeaderColumnName { get; set; }

    string FrequencyColumnHeaderColumnName { get; set; }

    bool OutputPeriodHeaderColumns { get; set; }

    string PeriodRowHeaderColumnName { get; set; }

    string PeriodColumnHeaderColumnName { get; set; }
  }

  [ExpectedTypeOfView(typeof(IRealFourierTransformation2DView))]
  [UserControllerForObject(typeof(RealFourierTransformation2DOptions))]
  public class RealFourierTransformation2DController : MVCANControllerEditOriginalDocBase<RealFourierTransformation2DOptions, IRealFourierTransformation2DView>
  {
    private SelectableListNodeList _outputQuantities;
    private SelectableListNodeList _fourierWindowChoice;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _outputQuantities = null;
      _fourierWindowChoice = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _outputQuantities = new SelectableListNodeList();
        _fourierWindowChoice = GetFourierWindowChoice(_doc.FourierWindow);
      }

      if (_view != null)
      {
        _view.IsUserDefinedXIncrement = _doc.IsUserDefinedRowIncrementValue;
        _view.XIncrement = _doc.RowIncrementValue;

        _view.IsUserDefinedYIncrement = _doc.IsUserDefinedColumnIncrementValue;
        _view.YIncrement = _doc.ColumnIncrementValue;

        _view.SetXIncrementWarning(_doc.RowIncrementMessage);

        _view.SetYIncrementWarning(_doc.ColumnIncrementMessage);

        _outputQuantities.FillWithEnumeration(_doc.OutputKind);
        _view.SetOutputQuantities(_outputQuantities);

        _view.FourierWindowChoice = _fourierWindowChoice;

        _view.DataPretreatmentOrder = _doc.DataPretreatmentCorrectionOrder;

        _view.CenterFrequencies = _doc.CenterResult;

        _view.ReplacementValueForNaNMatrixElements = _doc.ReplacementValueForNaNMatrixElements;
        _view.ReplacementValueForInfiniteMatrixElements = _doc.ReplacementValueForInfiniteMatrixElements;

        _view.ResultingFractionOfRowsUsed = _doc.ResultingFractionOfRowsUsed;
        _view.ResultingFractionOfColumnsUsed = _doc.ResultingFractionOfColumnsUsed;

        _view.OutputFrequencyHeaderColumns = _doc.OutputFrequencyHeaderColumns;
        _view.FrequencyRowHeaderColumnName = _doc.FrequencyRowHeaderColumnName;
        _view.FrequencyColumnHeaderColumnName = _doc.FrequencyColumnHeaderColumnName;

        _view.OutputPeriodHeaderColumns = _doc.OutputPeriodHeaderColumns;
        _view.PeriodRowHeaderColumnName = _doc.PeriodRowHeaderColumnName;
        _view.PeriodColumnHeaderColumnName = _doc.PeriodColumnHeaderColumnName;
      }
    }

    private SelectableListNodeList GetFourierWindowChoice(Altaxo.Calc.Fourier.Windows.IWindows2D currentWindowChoice)
    {
      var result = new SelectableListNodeList();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.Fourier.Windows.IWindows2D));

      result.Add(new SelectableListNode("None", null, currentWindowChoice == null));

      var currentType = null != currentWindowChoice ? currentWindowChoice.GetType() : null;

      foreach (var type in types)
      {
        try
        {
          var o = Activator.CreateInstance(type);
          result.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(type), o, type == currentType));
        }
        catch (Exception)
        {
        }
      }
      return result;
    }

    public override bool Apply(bool disposeController)
    {
      _doc.IsUserDefinedRowIncrementValue = _view.IsUserDefinedXIncrement;
      _doc.RowIncrementValue = _view.XIncrement;

      _doc.IsUserDefinedColumnIncrementValue = _view.IsUserDefinedYIncrement;
      _doc.ColumnIncrementValue = _view.YIncrement;

      _doc.OutputKind = (Altaxo.Calc.Fourier.RealFourierTransformationOutputKind)_outputQuantities.FirstSelectedNode.Tag;

      _doc.DataPretreatmentCorrectionOrder = _view.DataPretreatmentOrder;
      _doc.CenterResult = _view.CenterFrequencies;

      _doc.ReplacementValueForNaNMatrixElements = _view.ReplacementValueForNaNMatrixElements;
      _doc.ReplacementValueForInfiniteMatrixElements = _view.ReplacementValueForInfiniteMatrixElements;

      _doc.ResultingFractionOfRowsUsed = _view.ResultingFractionOfRowsUsed;
      _doc.ResultingFractionOfColumnsUsed = _view.ResultingFractionOfColumnsUsed;

      _doc.FourierWindow = _fourierWindowChoice.FirstSelectedNode.Tag as Altaxo.Calc.Fourier.Windows.IWindows2D;

      _doc.OutputFrequencyHeaderColumns = _view.OutputFrequencyHeaderColumns;
      _doc.FrequencyRowHeaderColumnName = _view.FrequencyRowHeaderColumnName;
      _doc.FrequencyColumnHeaderColumnName = _view.FrequencyColumnHeaderColumnName;

      _doc.OutputPeriodHeaderColumns = _view.OutputPeriodHeaderColumns;
      _doc.PeriodRowHeaderColumnName = _view.PeriodRowHeaderColumnName;
      _doc.PeriodColumnHeaderColumnName = _view.PeriodColumnHeaderColumnName;

      return base.ApplyEnd(true, disposeController);
    }
  }
}
