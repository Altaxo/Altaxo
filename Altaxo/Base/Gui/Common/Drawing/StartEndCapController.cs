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

using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Units;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Controller that controls a <see cref="ILineCap"/> object. 
  /// </summary>
  /// <remarks>
  /// Usually, this controller has no separate view. Instead, it is used to bind the suitable Gui elements.
  /// </remarks>
  public class StartEndCapController : MVCANDControllerEditImmutableDocBase<ILineCap, object>
  {
    #region Bindings

    bool _isForEndCap;

    /// <summary>
    /// Gets or sets a value indicating whether the controller edits an end cap.
    /// </summary>
    public bool IsForEndCap
    {
      get => _isForEndCap;
      set
      {
        if (!(_isForEndCap == value))
        {
          _isForEndCap = value;
          OnPropertyChanged(nameof(IsForEndCap));
        }
      }
    }

    /// <summary>
    /// Gets or sets the line cap.
    /// </summary>
    public ILineCap LineCap
    {
      get => _doc;
      set
      {
        if (!object.ReferenceEquals(LineCap, value))
        {
          _doc = value;
          OnPropertyChanged(nameof(LineCap));
          OnPropertyChanged(nameof(MinimumAbsoluteSize));
          OnPropertyChanged(nameof(MinimumRelativeSize));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the environment for the minimum absolute size.
    /// </summary>
    public QuantityWithUnitGuiEnvironment MinimumAbsoluteSizeEnvironment { get; set; } = Altaxo.Gui.LineCapSizeEnvironment.Instance;

    /// <summary>
    /// Gets or sets the minimum absolute size.
    /// </summary>
    public DimensionfulQuantity MinimumAbsoluteSize
    {
      get => new DimensionfulQuantity(_doc.MinimumAbsoluteSizePt, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MinimumAbsoluteSizeEnvironment.DefaultUnit);
      set
      {
        if (!(MinimumAbsoluteSize == value))
        {
          _doc = _doc.WithMinimumAbsoluteAndRelativeSize(value.AsValueIn(Altaxo.Units.Length.Point.Instance), _doc.MinimumRelativeSize);
          OnPropertyChanged(nameof(MinimumAbsoluteSize));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the environment for the minimum relative size.
    /// </summary>
    public QuantityWithUnitGuiEnvironment MinimumRelativeSizeEnvironment { get; set; } = Altaxo.Gui.RelationEnvironment.Instance;

    /// <summary>
    /// Gets or sets the minimum relative size.
    /// </summary>
    public DimensionfulQuantity MinimumRelativeSize
    {
      get => new DimensionfulQuantity(_doc.MinimumRelativeSize, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MinimumRelativeSizeEnvironment.DefaultUnit);
      set
      {
        if (!(MinimumRelativeSize == value))
        {
          _doc = _doc.WithMinimumAbsoluteAndRelativeSize(_doc.MinimumAbsoluteSizePt, value.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance));
          OnPropertyChanged(nameof(MinimumRelativeSize));
          OnMadeDirty();
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
