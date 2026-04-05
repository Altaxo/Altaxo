#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common.BasicTypes
{
  /// <summary>
  /// Defines the view contract for editing <see cref="TimeSpan"/> values.
  /// </summary>
  public interface ITimeSpanValueView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="TimeSpan"/> values.
  /// </summary>
  [UserControllerForObject(typeof(TimeSpan), 100)]
  [ExpectedTypeOfView(typeof(ITimeSpanValueView))]
  public class TimeSpanValueController : MVCANDControllerEditImmutableDocBase<TimeSpan, ITimeSpanValueView>
  {
    #region Bindings

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public TimeSpan Value
    {
      get
      {
        return _doc;
      }
      set
      {
        if(!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(Value));
          OnMadeDirty();
        }
      }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanValueController"/> class.
    /// </summary>
    public TimeSpanValueController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanValueController"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    public TimeSpanValueController(TimeSpan value)
    {
      _doc = _originalDoc = value;
      Initialize(true);
    }

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
