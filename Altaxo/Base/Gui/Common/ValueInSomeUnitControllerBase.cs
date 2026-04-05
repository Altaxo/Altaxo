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

namespace Altaxo.Gui.Common
{
  using Altaxo.Units;

  /// <summary>
  /// View interface for editing a dimensionful quantity in selectable units.
  /// </summary>
  public interface IDimensionfulQuantityView
  {
    /// <summary>
    /// Gets or sets the selected quantity.
    /// </summary>
    DimensionfulQuantity SelectedQuantity { get; set; }

    /// <summary>
    /// Occurs when the selected quantity changes.
    /// </summary>
    event Action SelectedQuantityChanged;

    /// <summary>
    /// Sets the unit environment used by the view.
    /// </summary>
    QuantityWithUnitGuiEnvironment UnitEnvironment { set; }
  }

  /// <summary>
  /// Base controller for editing numeric values that are displayed in selectable units.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionfulQuantityView))]
  public abstract class ValueInSomeUnitControllerBase : MVCANDControllerEditImmutableDocBase<double, IDimensionfulQuantityView>
  {
    /// <summary>
    /// Gets the unit of the underlying stored value.
    /// </summary>
    protected abstract IUnit UnitOfValue { get; }

    /// <summary>
    /// Gets the unit environment used by the view.
    /// </summary>
    protected abstract QuantityWithUnitGuiEnvironment UnitEnvironment { get; }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view is not null)
      {
        _view.UnitEnvironment = UnitEnvironment;
        _view.SelectedQuantity = new DimensionfulQuantity(_doc, UnitOfValue).AsQuantityIn(UnitEnvironment.DefaultUnit);
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      GetQuantityFromView();
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    protected override void AttachView()
    {
      if (_view is null) throw new InvalidProgramException();

      _view.SelectedQuantityChanged += EhQuantityChanged;
      base.AttachView();
    }

    /// <inheritdoc/>
    protected override void DetachView()
    {
      if (_view is null) throw new InvalidProgramException();

      _view.SelectedQuantityChanged -= EhQuantityChanged;
      base.DetachView();
    }

    private void GetQuantityFromView()
    {
      if (_view is not null)
      {
        var q = _view.SelectedQuantity;
        _doc = q.AsValueIn(UnitOfValue);
      }
    }

    private void EhQuantityChanged()
    {
      GetQuantityFromView();
      OnMadeDirty();
    }
  }
}
