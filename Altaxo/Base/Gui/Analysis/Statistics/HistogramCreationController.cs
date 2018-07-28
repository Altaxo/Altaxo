#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Analysis.Statistics.Histograms;
using Altaxo.Collections;
using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.Statistics
{
  public interface IHistogramCreationView
  {
    IEnumerable<string> Errors { set; }

    IEnumerable<string> Warnings { set; }

    double NumberOfValuesOriginal { set; }

    double NumberOfValuesFiltered { set; }

    double NumberOfNaNValues { set; }

    double NumberOfInfiniteValues { set; }

    double MinimumValue { set; }

    double MaximumValue { set; }

    bool IgnoreNaNValues { get; set; }

    bool IgnoreInfiniteValues { get; set; }

    bool IgnoreValuesBelowLowerBoundary { get; set; }

    bool IsLowerBoundaryInclusive { get; set; }

    double LowerBoundary { get; set; }

    bool IgnoreValuesAboveUpperBoundary { get; set; }

    bool IsUpperBoundaryInclusive { get; set; }

    double UpperBoundary { get; set; }

    bool UseAutomaticBinning { get; set; }

    SelectableListNodeList BinningType { set; }

    object BinningView { set; }

    event Action BinningTypeChanged;

    event Action AutomaticBinningTypeChanged;
  }

  [UserControllerForObject(typeof(HistogramCreationInformation))]
  [ExpectedTypeOfView(typeof(IHistogramCreationView))]
  public class HistogramCreationController : MVCANControllerEditOriginalDocBase<HistogramCreationInformation, IHistogramCreationView>
  {
    private IMVCANController _binningController;
    private SelectableListNodeList _binningTypes = new SelectableListNodeList();

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      if (null != _binningController)
        yield return new ControllerAndSetNullMethod(_binningController, () => _binningController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _binningTypes.Clear();

        var binningTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBinning));

        foreach (var type in binningTypes)
          _binningTypes.Add(new SelectableListNode(type.ToString(), type, type == _doc.CreationOptions.Binning.GetType()));

        if (null != _binningController)
        {
          _binningController.Dispose();
          _binningController = null;
        }

        _binningController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.CreationOptions.Binning }, typeof(IMVCANController), UseDocument.Directly);
      }
      if (null != _view)
      {
        _view.Errors = _doc.Errors;
        _view.Warnings = _doc.Warnings;

        _view.NumberOfValuesOriginal = _doc.NumberOfValuesOriginal;
        _view.NumberOfValuesFiltered = _doc.NumberOfValuesFiltered;
        _view.NumberOfNaNValues = _doc.NumberOfNaNValues;
        _view.NumberOfInfiniteValues = _doc.NumberOfInfiniteValues;
        _view.MinimumValue = _doc.MinimumValue;
        _view.MaximumValue = _doc.MaximumValue;

        _view.IgnoreNaNValues = _doc.CreationOptions.IgnoreNaN;
        _view.IgnoreInfiniteValues = _doc.CreationOptions.IgnoreInfinity;

        _view.IgnoreValuesBelowLowerBoundary = _doc.CreationOptions.LowerBoundaryToIgnore.HasValue;
        _view.IsLowerBoundaryInclusive = _doc.CreationOptions.IsLowerBoundaryInclusive;
        if (_doc.CreationOptions.LowerBoundaryToIgnore.HasValue)
          _view.LowerBoundary = _doc.CreationOptions.LowerBoundaryToIgnore.Value;

        _view.IgnoreValuesAboveUpperBoundary = _doc.CreationOptions.UpperBoundaryToIgnore.HasValue;
        _view.IsUpperBoundaryInclusive = _doc.CreationOptions.IsUpperBoundaryInclusive;
        if (_doc.CreationOptions.UpperBoundaryToIgnore.HasValue)
          _view.UpperBoundary = _doc.CreationOptions.UpperBoundaryToIgnore.Value;

        _view.BinningType = _binningTypes;

        _view.UseAutomaticBinning = !_doc.CreationOptions.IsUserDefinedBinningType;

        _view.BinningView = _binningController.ViewObject;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.CreationOptions.IgnoreNaN = _view.IgnoreNaNValues;
      _doc.CreationOptions.IgnoreInfinity = _view.IgnoreInfiniteValues;

      if (_view.IgnoreValuesBelowLowerBoundary)
      {
        _doc.CreationOptions.IsLowerBoundaryInclusive = _view.IsLowerBoundaryInclusive;
        _doc.CreationOptions.LowerBoundaryToIgnore = _view.LowerBoundary;
      }
      else
      {
        _doc.CreationOptions.IsLowerBoundaryInclusive = true;
        _doc.CreationOptions.LowerBoundaryToIgnore = null;
      }

      if (_view.IgnoreValuesAboveUpperBoundary)
      {
        _doc.CreationOptions.IsUpperBoundaryInclusive = _view.IsUpperBoundaryInclusive;
        _doc.CreationOptions.UpperBoundaryToIgnore = _view.UpperBoundary;
      }
      else
      {
        _doc.CreationOptions.IsUpperBoundaryInclusive = true;
        _doc.CreationOptions.UpperBoundaryToIgnore = null;
      }

      _doc.CreationOptions.IsUserDefinedBinningType = !_view.UseAutomaticBinning;

      if (!_binningController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      bool shouldShowDialog = HistogramCreation.PopulateHistogramCreationInformation(_doc);

      if (disposeController) // user pressed ok
      {
        if (ShouldLeaveDialogOpen(_doc))
        {
          Initialize(true);
          return ApplyEnd(false, disposeController);
        }
        else
        {
          return ApplyEnd(true, disposeController);
        }
      }
      else
      {
        // we pressed apply thus we must update the gui
        if (ShouldLeaveDialogOpen(_doc))
        {
          Initialize(true);
          return ApplyEnd(false, disposeController);
        }
        else
        {
          Initialize(true);
          return ApplyEnd(true, disposeController);
        }
      }
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.BinningTypeChanged += EhBinningTypeChanged;
      _view.AutomaticBinningTypeChanged += EhAutomaticBinningTypeChanged;
    }

    protected override void DetachView()
    {
      _view.AutomaticBinningTypeChanged -= EhAutomaticBinningTypeChanged;
      _view.BinningTypeChanged -= EhBinningTypeChanged;
      base.DetachView();
    }

    private void EhBinningTypeChanged()
    {
      var selNode = _binningTypes.FirstSelectedNode;
      if (null == selNode)
        return;
      var bintype = (Type)selNode.Tag;

      if (_doc.CreationOptions.Binning.GetType() == bintype)
        return;

      var binning = (IBinning)Activator.CreateInstance(bintype);

      _doc.CreationOptions.Binning = binning;

      HistogramCreation.PopulateHistogramCreationInformation(_doc);
      Initialize(true);
    }

    private void EhAutomaticBinningTypeChanged()
    {
      var wasUserBefore = _doc.CreationOptions.IsUserDefinedBinningType;
      _doc.CreationOptions.IsUserDefinedBinningType = !_view.UseAutomaticBinning;

      if (!_doc.CreationOptions.IsUserDefinedBinningType && wasUserBefore)
      {
        HistogramCreation.PopulateHistogramCreationInformation(_doc);
        Initialize(true);
      }
    }

    private static bool ShouldLeaveDialogOpen(HistogramCreationInformation histInfo)
    {
      bool showDialog;
      switch (histInfo.UserInteractionLevel)
      {
        case Gui.UserInteractionLevel.None:
          showDialog = false;
          break;

        case Gui.UserInteractionLevel.InteractOnErrors:
          showDialog = histInfo.Errors.Count > 0;
          break;

        case Gui.UserInteractionLevel.InteractOnWarningsAndErrors:
          showDialog = histInfo.Errors.Count > 0 || histInfo.Warnings.Count > 0;
          break;

        case Gui.UserInteractionLevel.InteractAlways:
          showDialog = histInfo.Errors.Count > 0 || histInfo.Warnings.Count > 0;
          break;

        default:
          throw new NotImplementedException("userInteractionLevel");
      }
      return showDialog;
    }
  }
}
