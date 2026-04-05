#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Science.Spectroscopy.Normalization;

namespace Altaxo.Gui.Science.Spectroscopy.Normalization
{
  /// <summary>
  /// View interface for area normalization.
  /// </summary>
  public interface INormalizationAreaView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="NormalizationArea"/>.
  /// </summary>
  [UserControllerForObject(typeof(NormalizationArea))]
  [ExpectedTypeOfView(typeof(INormalizationAreaView))]
  public class NormalizationAreaController : MVCANControllerEditImmutableDocBase<NormalizationArea, INormalizationAreaView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    /// <summary>
    /// Gets or sets the minimum x value.
    /// </summary>
    public double MinimumXValue
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MinimumXValue));
        }
      }
    }


    /// <summary>
    /// Gets or sets the maximum x value.
    /// </summary>
    public double MaximumXValue
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MaximumXValue));
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether normalization is based on the minimum y value.
    /// </summary>
    public bool BasedOnMinimumYValue
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(BasedOnMinimumYValue));
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the minimum-y-value option can be changed.
    /// </summary>
    public bool IsBasedOnMinimumYValueEnabled => true;


    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MinimumXValue = _doc.MinimumXValue;
        MaximumXValue = _doc.MaximumXValue;
        BasedOnMinimumYValue = _doc.BasedOnMinimumYValue;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        MinimumXValue = MinimumXValue,
        MaximumXValue = MaximumXValue,
        BasedOnMinimumYValue = BasedOnMinimumYValue
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
