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

  public interface IBooleanValueView : IDataContextAwareView
  {
  }

  public interface IBooleanValueController : IMVCAController
  {
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
    protected string _descriptionText = "Enter value:";

    public BooleanValueController() { }

    public BooleanValueController(bool val)
    {
      InitializeDocument(val);
    }

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

    public object ProvisionalModelObject => _doc;

    public event Action<IMVCANDController>? MadeDirty;

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
