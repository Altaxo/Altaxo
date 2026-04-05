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
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Groups
{

  /// <summary>
  /// Provides the view contract for <see cref="WaterfallTransformController"/>.
  /// </summary>
  public interface IWaterfallTransformView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Controller for <see cref="WaterfallTransform"/>.
  /// </summary>
  [UserControllerForObject(typeof(WaterfallTransform))]
  [ExpectedTypeOfView(typeof(IWaterfallTransformView))]
  public class WaterfallTransformController : MVCANControllerEditOriginalDocBase<WaterfallTransform, IWaterfallTransformView>
  {
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets the unit environment for the X scale.
    /// </summary>
    public QuantityWithUnitGuiEnvironment XScaleEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _xScale;

    /// <summary>
    /// Gets or sets the X scale.
    /// </summary>
    public DimensionfulQuantity XScale
    {
      get => _xScale;
      set
      {
        if (!(_xScale == value))
        {
          _xScale = value;
          OnPropertyChanged(nameof(XScale));
        }
      }
    }


    /// <summary>
    /// Gets the unit environment for the Y scale.
    /// </summary>
    public QuantityWithUnitGuiEnvironment YScaleEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _yScale;

    /// <summary>
    /// Gets or sets the Y scale.
    /// </summary>
    public DimensionfulQuantity YScale
    {
      get => _yScale;
      set
      {
        if (!(_yScale == value))
        {
          _yScale = value;
          OnPropertyChanged(nameof(YScale));
        }
      }
    }

    private bool _useClipping;

    /// <summary>
    /// Gets or sets a value indicating whether clipping is used.
    /// </summary>
    public bool UseClipping
    {
      get => _useClipping;
      set
      {
        if (!(_useClipping == value))
        {
          _useClipping = value;
          OnPropertyChanged(nameof(UseClipping));
        }
      }
    }

    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        XScale = new DimensionfulQuantity(_doc.XScale, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(XScaleEnvironment.DefaultUnit);
        YScale = new DimensionfulQuantity(_doc.YScale, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(YScaleEnvironment.DefaultUnit);
        UseClipping = _doc.UseClipping;
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc.UseClipping = UseClipping;
        _doc.XScale = XScale.AsValueInSIUnits;
        _doc.YScale = YScale.AsValueInSIUnits;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Error while applying..");
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
