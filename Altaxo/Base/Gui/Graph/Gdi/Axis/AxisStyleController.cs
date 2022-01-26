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
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Graph.Scales.Ticks;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  #region Interfaces

  public interface IAxisStyleView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for TitleFormatLayerController.
  /// </summary>
  [UserControllerForObject(typeof(AxisStyle), 90)]
  [ExpectedTypeOfView(typeof(IAxisStyleView))]
  public class AxisStyleController : MVCANDControllerEditOriginalDocBase<AxisStyle, IAxisStyleView>
  {
    protected IMVCAController _axisLineStyleController;

    protected TickSpacingController _tickSpacingController;

    private Altaxo.Main.Properties.IReadOnlyPropertyBag _context;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_axisLineStyleController, () => _axisLineStyleController = null);
      yield return new ControllerAndSetNullMethod(_tickSpacingController, () => _tickSpacingController = null);
    }

    #region Binding

    private string _axisTitle;

    public string AxisTitle
    {
      get => _axisTitle;
      set
      {
        if (!(_axisTitle == value))
        {
          _axisTitle = value;
          OnPropertyChanged(nameof(AxisTitle));
        }
      }
    }

    private bool _showMajorLabels;

    public bool ShowMajorLabels
    {
      get => _showMajorLabels;
      set
      {
        if (!(_showMajorLabels == value))
        {
          _showMajorLabels = value;
          EhShowMajorLabelsChanged();
          OnPropertyChanged(nameof(ShowMajorLabels));
        }
      }
    }

    private bool _showMinorLabels;

    public bool ShowMinorLabels
    {
      get => _showMinorLabels;
      set
      {
        if (!(_showMinorLabels == value))
        {
          _showMinorLabels = value;
          EhShowMinorLabelsChanged();
          OnPropertyChanged(nameof(ShowMinorLabels));
        }
      }
    }

    private bool _showAxisLine;

    public bool ShowAxisLine
    {
      get => _showAxisLine;
      set
      {
        if (!(_showAxisLine == value))
        {
          _showAxisLine = value;
          EhShowAxisLineChanged();
          OnPropertyChanged(nameof(ShowAxisLine));
        }
      }
    }
    private bool _showCustomTickSpacing;

    public bool ShowCustomTickSpacing
    {
      get => _showCustomTickSpacing;
      set
      {
        if (!(_showCustomTickSpacing == value))
        {
          _showCustomTickSpacing = value;
          EhShowCustomTickSpacingChanged();
          OnPropertyChanged(nameof(ShowCustomTickSpacing));
        }
      }
    }

    public object? AxisLineView => _axisLineStyleController?.ViewObject;

    public object? CustomTickSpacingView => _tickSpacingController?.ViewObject;

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _context = _doc.GetPropertyContext();

        if (_doc.AxisLineStyle is not null)
        {
          _axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
        }
        else
        {
          _axisLineStyleController = null;
        }
        OnPropertyChanged(nameof(AxisLineView));

        if (_doc.TickSpacing is not null)
        {
          _tickSpacingController = new TickSpacingController() { UseDocumentCopy = UseDocument.Directly };
          _tickSpacingController.InitializeDocument(_doc.TickSpacing);
          Current.Gui.FindAndAttachControlTo(_tickSpacingController);
          OnPropertyChanged(nameof(CustomTickSpacingView));
        }

        AxisTitle = _doc.TitleText;
        ShowAxisLine = _doc.IsAxisLineEnabled;
        ShowMajorLabels = _doc.AreMajorLabelsEnabled;
        ShowMinorLabels = _doc.AreMinorLabelsEnabled;
        ShowCustomTickSpacing = _tickSpacingController?.ViewObject is not null;
      }

      if (_view is not null)
      {
      }
    }

    public override bool Apply(bool disposeController)
    {
      // read axis title
      _doc.TitleText = AxisTitle;

      if (_axisLineStyleController is not null)
      {
        if (!_axisLineStyleController.Apply(disposeController))
          return false;
        else
          _doc.AxisLineStyle = (AxisLineStyle)_axisLineStyleController.ModelObject;
      }

      if (ShowMajorLabels)
        _doc.ShowMajorLabels(_context);
      else
        _doc.HideMajorLabels();

      if (ShowMinorLabels)
        _doc.ShowMinorLabels(_context);
      else
        _doc.HideMinorLabels();

      if (_tickSpacingController is not null && !_tickSpacingController.Apply(disposeController))
        return false;
      if (ShowCustomTickSpacing && _tickSpacingController is not null)
        _doc.TickSpacing = (Altaxo.Graph.Scales.Ticks.TickSpacing)_tickSpacingController.ModelObject;

      return ApplyEnd(true, disposeController); // all ok
    }

    /// <summary>Can be called by an external controller if the state of either the major or the minor label has been changed by an external controller. This will update
    /// the state of the checkboxes for major and minor labels in the view that is controlled by this controller.</summary>
    public void AnnounceExternalChangeOfMajorOrMinorLabelState()
    {
      if (_view is not null)
      {
        ShowMajorLabels = _doc.AreMajorLabelsEnabled;
        ShowMinorLabels = _doc.AreMinorLabelsEnabled;
      }
    }

    private void EhShowCustomTickSpacingChanged()
    {
      var isShown = ShowCustomTickSpacing;

      if (isShown)
      {
        if (_tickSpacingController is null)
        {
          if (_doc.TickSpacing is null)
          {
            _doc.TickSpacing = new Altaxo.Graph.Scales.Ticks.LinearTickSpacing();
          }
          _tickSpacingController = new TickSpacingController() { UseDocumentCopy = UseDocument.Directly };
          _tickSpacingController.InitializeDocument(_doc.TickSpacing);
          Current.Gui.FindAndAttachControlTo(_tickSpacingController);
        }
      }
      else
      {
        _doc.TickSpacing = null;
        _tickSpacingController = null;
      }
      OnPropertyChanged(nameof(CustomTickSpacingView));
    }

    private void EhShowAxisLineChanged()
    {
      var oldValue = _doc.IsAxisLineEnabled;
      if (ShowAxisLine && (_doc.AxisLineStyle is null))
      {
        _doc.ShowAxisLine(_context);
        _axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
      }
      OnPropertyChanged(nameof(AxisLineView));

      if (oldValue != _doc.IsAxisLineEnabled)
        OnMadeDirty();
    }

    private void EhShowMajorLabelsChanged()
    {
      var oldValue = _doc.AreMajorLabelsEnabled;
      var newValue = ShowMajorLabels;

      if (oldValue != newValue)
      {
        if (newValue)
          _doc.ShowMajorLabels(_context);
        else
          _doc.HideMajorLabels();
        OnMadeDirty();
      }
    }

    private void EhShowMinorLabelsChanged()
    {
      var oldValue = _doc.AreMinorLabelsEnabled;
      var newValue = ShowMinorLabels;

      if (oldValue != newValue)
      {
        if (newValue)
          _doc.ShowMinorLabels(_context);
        else
          _doc.HideMinorLabels();

        OnMadeDirty();
      }
    }
  }
}
