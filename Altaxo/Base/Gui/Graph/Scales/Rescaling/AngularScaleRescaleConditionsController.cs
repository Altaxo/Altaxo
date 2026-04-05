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
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
  /// <summary>
  /// View contract for angular scale rescaling conditions.
  /// </summary>
  public interface IAngularScaleRescaleConditionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing <see cref="AngularRescaleConditions"/>.
  /// </summary>
  [UserControllerForObject(typeof(AngularRescaleConditions))]
  [ExpectedTypeOfView(typeof(IAngularScaleRescaleConditionsView))]
  public class AngularScaleRescaleConditionsController : MVCANControllerEditOriginalDocBase<AngularRescaleConditions, IAngularScaleRescaleConditionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    private static readonly int[] DefaultScaleOrigins = new int[] { -135, -90, -45, 0, 45, 90, 135, 180, 225, 270 };

    #region Bindings

    private ItemsController<int> _origin;

    /// <summary>
    /// Gets or sets the scale origin.
    /// </summary>
    public ItemsController<int> Origin
    {
      get => _origin;
      set
      {
        if (!(_origin == value))
        {
          _origin = value;
          OnPropertyChanged(nameof(Origin));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    public override void Dispose(bool isDisposing)
    {
      _origin?.Dispose();
      base.Dispose(isDisposing);
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _origin = new ItemsController<int>(BuildOriginList());
        _origin.SelectedValue = _doc.ScaleOrigin;
      }

    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc.ScaleOrigin = Origin.SelectedValue;
      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Builds the list of selectable scale origins.
    /// </summary>
    /// <returns>The selectable origin list.</returns>
    private SelectableListNodeList BuildOriginList()
    {
      var result = new SelectableListNodeList(DefaultScaleOrigins.Select(sc => new SelectableListNode($"{sc}°", sc, false)));

      if (!DefaultScaleOrigins.Contains(_doc.ScaleOrigin))
      {
        result.Insert(0, new SelectableListNode($"{_doc.ScaleOrigin}°", _doc.ScaleOrigin, false));
      }

      return result;
    }
  }
}
