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

#nullable disable
using System;
using Altaxo.Graph.Graph3D.Axis;
using Altaxo.Gui.Graph.Scales.Ticks;
using Altaxo.Gui.Common;
using System.Windows.Input;

namespace Altaxo.Gui.Graph.Graph3D.Axis
{

  /// <summary>
  /// Provides the view contract for <see cref="AxisStyleController"/>.
  /// </summary>
  public interface IAxisStyleView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Controller for <see cref="AxisStyle"/>.
  /// </summary>
  [UserControllerForObject(typeof(AxisStyle), 90)]
  [ExpectedTypeOfView(typeof(IAxisStyleView))]
  public class AxisStyleController : MVCANDControllerEditOriginalDocBase<AxisStyle, IAxisStyleView>
  {
    private Altaxo.Main.Properties.IReadOnlyPropertyBag _context;

    /// <inheritdoc />
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_axisLineStyleController, () => _axisLineStyleController = null);
      yield return new ControllerAndSetNullMethod(_tickSpacingController, () => _tickSpacingController = null);
      yield return new ControllerAndSetNullMethod(_majorLabelCondController, () => _majorLabelCondController = null);
      yield return new ControllerAndSetNullMethod(_minorLabelCondController, () => _minorLabelCondController = null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisStyleController"/> class.
    /// </summary>
    public AxisStyleController()
    {
      CmdEditAxisTitle = new RelayCommand(EhEditAxisTitle);
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the command to edit the axis title.
    /// </summary>
    public ICommand CmdEditAxisTitle { get; }

    #region AxisLine


    private IMVCANController _axisLineStyleController;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public IMVCANController AxisLineStyleController
    {
      get
      {
        if (_axisLineStyleController is null && _doc.AxisLineStyle is { } lineStyle)
        {
          _axisLineStyleController = new AxisLineStyleController();
          _axisLineStyleController.InitializeDocument(lineStyle);
        }
        return _axisLineStyleController;
      }
    }


    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public object? AxisLineStyleView
    {
      get
      {
        if (_doc.AxisLineStyle is not null)
        {
          if (AxisLineStyleController is { } ctrl && ctrl.ViewObject is null)
            Current.Gui.FindAndAttachControlTo(ctrl);

          return AxisLineStyleController?.ViewObject;
        }
        else
        {
          return null;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the axis line is shown.
    /// </summary>
    public bool ShowAxisLine
    {
      get => _doc.AxisLineStyle is not null;
      set
      {
        if (!(ShowAxisLine == value))
        {
          _axisLineStyleController?.Dispose();
          _axisLineStyleController = null;
          if (value is true)
          {
            _doc.ShowAxisLine(_context);
          }
          else
          {
            _doc.HideAxisLine();
          }

          OnPropertyChanged(nameof(AxisLineStyleController));
          OnPropertyChanged(nameof(AxisLineStyleView));
          OnPropertyChanged(nameof(ShowAxisLine));
        }
      }
    }

    #endregion AxisLine

    #region Major Label

    private ConditionalDocumentController<AxisLabelStyle> _majorLabelCondController;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public ConditionalDocumentController<AxisLabelStyle> MajorLabelCondController
    {
      get
      {
        if (_majorLabelCondController is null)
        {
          _majorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(
            () => { ShowMajorLabels = true; return _doc.MajorLabelStyle; },
            () => { ShowMajorLabels = false; })
          {
            UseDocumentCopy = UseDocument.Directly
          };
          _majorLabelCondController.InitializeDocument(_doc.MajorLabelStyle);
        }
        return _majorLabelCondController;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the major labels are shown.
    /// </summary>
    public bool ShowMajorLabels
    {
      get => _doc.MajorLabelStyle is not null;
      set
      {
        if (!(ShowMajorLabels == value))
        {
          if (value == true)
          {
            _doc.ShowMajorLabels(_context);
          }
          else
          {
            _doc.HideMajorLabels();
          }

          if (_majorLabelCondController is not null)
          {
            _majorLabelCondController.IsConditionalViewEnabled = value;
          }
          OnPropertyChanged(nameof(ShowMajorLabels));
        }
      }
    }
    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public IConditionalDocumentView MajorLabelCondView
    {
      get
      {
        if (MajorLabelCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");

        if (MajorLabelCondController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(MajorLabelCondController);
        return (IConditionalDocumentView)MajorLabelCondController.ViewObject;
      }
    }

    #endregion Major Label

    #region Minor Label

    private ConditionalDocumentController<AxisLabelStyle> _minorLabelCondController;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public ConditionalDocumentController<AxisLabelStyle> MinorLabelCondController
    {
      get
      {
        if (_minorLabelCondController is null)
        {
          _minorLabelCondController = new ConditionalDocumentController<AxisLabelStyle>(
            () => { ShowMinorLabels = true; return _doc.MinorLabelStyle; },
            () => { ShowMinorLabels = false; }
            )
          {
            UseDocumentCopy = UseDocument.Directly
          };
          _minorLabelCondController.InitializeDocument(_doc.MinorLabelStyle);
        }
        return _minorLabelCondController;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the minor labels are shown.
    /// </summary>
    public bool ShowMinorLabels
    {
      get => _doc.MinorLabelStyle is not null;
      set
      {
        if (!(ShowMinorLabels == value))
        {
          if (value == true)
          {
            _doc.ShowMinorLabels(_context);
          }
          else
          {
            _doc.HideMinorLabels();
          }

          if (_minorLabelCondController is not null)
          {
            _minorLabelCondController.IsConditionalViewEnabled = value;
          }
          OnPropertyChanged(nameof(ShowMinorLabels));
        }
      }
    }

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public IConditionalDocumentView MinorLabelCondView
    {
      get
      {
        if (MinorLabelCondController is null)
          throw new InvalidOperationException("Instance is not initialized!");

        if (MinorLabelCondController.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(MinorLabelCondController);
        return (IConditionalDocumentView)MinorLabelCondController.ViewObject;
      }
    }

    #endregion Minor Label

    #region AxisTitle

    private string _axisTitle;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
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

    #endregion

    #region Custom Tick Spacing

    /// <summary>
    /// Gets or sets a value indicating whether custom tick spacing is shown.
    /// </summary>
    public bool ShowCustomTickSpacing
    {
      get => _doc.TickSpacing is not null;
      set
      {
        if (!(ShowCustomTickSpacing == value))
        {
          EhShowCustomTickSpacingChanged(value);
          OnPropertyChanged(nameof(ShowCustomTickSpacing));
        }
      }
    }

    private void EhShowCustomTickSpacingChanged(bool isShown)
    {
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
    /// <summary>
    /// Gets or sets the ticks pacing controller.
    /// </summary>
    protected TickSpacingController _tickSpacingController;

    /// <summary>
    /// Gets or sets the custom ticks pacing view.
    /// </summary>
    public object? CustomTickSpacingView => _tickSpacingController?.ViewObject;

    #endregion Custom Tick Spacing

    #endregion


    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _context = _doc.GetPropertyContext();
        AxisTitle = _doc.TitleText;
        EhShowCustomTickSpacingChanged(_doc.TickSpacing is not null);
      }
    }

    /// <inheritdoc />
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
        return ApplyEnd(false, disposeController);

      if (ShowCustomTickSpacing && _tickSpacingController is not null)
        _doc.TickSpacing = (Altaxo.Graph.Scales.Ticks.TickSpacing)_tickSpacingController.ModelObject;

      return ApplyEnd(true, disposeController); // all ok
    }

    private void EhEditAxisTitle()
    {
      var title = _doc.Title;
      if (Current.Gui.ShowDialog(ref title, "Edit title", true))
      {
        _doc.Title = title;
        AxisTitle = _doc.TitleText;
      }
    }
  }
}
