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
  #region Interfaces

  /// <summary>
  /// Defines the view contract for editing boolean values.
  /// </summary>
  public interface IBooleanValueView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Defines additional metadata for a boolean value controller.
  /// </summary>
  public interface IBooleanValueController : IMVCAController
  {
    /// <summary>
    /// Gets or sets the descriptive text shown by the view.
    /// </summary>
    string DescriptionText { get; set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for a boolean value.
  /// </summary>
  [UserControllerForObject(typeof(bool), 100)]
  [ExpectedTypeOfView(typeof(IBooleanValueView))]
  public class BooleanValueController : MVCANControllerEditImmutableDocBase<bool, IBooleanValueView>, IBooleanValueController, IMVCANDController
  {
    /// <summary>
    /// The descriptive text shown by the view.
    /// </summary>
    protected string _descriptionText = "Enter value:";

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanValueController"/> class.
    /// </summary>
    public BooleanValueController() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanValueController"/> class.
    /// </summary>
    /// <param name="val">The initial value.</param>
    public BooleanValueController(bool val)
    {
      InitializeDocument(val);
    }

    /// <inheritdoc/>
    public string DescriptionText
    {
      get
      {
        return _descriptionText;
      }
      set
      {
        if (!(_descriptionText == value))
        {
          _descriptionText = value;
          OnPropertyChanged(nameof(DescriptionText));
        }
      }
    }

    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    public bool Value
    {
      get => _doc;
      set
      {
        if(!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(Value));
          MadeDirty?.Invoke(this);
        }
      }
    }

    /// <inheritdoc/>
    public object ProvisionalModelObject => _doc;

    /// <inheritdoc/>
    public event Action<IMVCANDController>? MadeDirty;

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
