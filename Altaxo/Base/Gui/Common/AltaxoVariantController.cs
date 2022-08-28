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

using System;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Gui.Common
{
  public interface IAltaxoVariantView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IAltaxoVariantView))]
  [UserControllerForObject(typeof(AltaxoVariant), 1)]
  public class AltaxoVariantController : MVCANControllerEditImmutableDocBase<AltaxoVariant, IAltaxoVariantView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }


    public AltaxoVariantController() { }
    public AltaxoVariantController(AltaxoVariant doc)
    {
      _doc = _originalDoc = doc;
      Initialize(true);
    }

    #region Bindings

    private ItemsController<Type> _valueType;

    public ItemsController<Type> ValueType
    {
      get => _valueType;
      set
      {
        if (!(_valueType == value))
        {
          _valueType = value;
          OnPropertyChanged(nameof(ValueType));
        }
      }
    }

    public double ValueAsDouble
    {
      get => (double)_doc;
      set
      {
        _doc = new AltaxoVariant(value);
      }
    }

    public string ValueAsString
    {
      get => _doc.ToString();
      set
      {
        _doc = new AltaxoVariant(value);
      }
    }

    public DateTime ValueAsDateTime
    {
      get => _doc.ToDateTime();
      set
      {
        _doc = new AltaxoVariant(value);
      }
    }

    public bool IsDouble => ValueType.SelectedValue == typeof(double);
    public bool IsString => ValueType.SelectedValue == typeof(string);
    public bool IsDateTime => ValueType.SelectedValue == typeof(DateTime);

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ValueType = new ItemsController<Type>(
          new SelectableListNodeList(
            new[]
            {
              new SelectableListNode("Numeric", typeof(double), false),
              new SelectableListNode("Text", typeof(string), false),
              new SelectableListNode("DateTime", typeof(DateTime), false)
            }
          ),
          EhValueTypeChanged
          );

        if (_doc.IsType(AltaxoVariant.Content.VDouble))
          ValueType.SelectedValue = typeof(double);
        else if (_doc.IsType(AltaxoVariant.Content.VDateTime))
          ValueType.SelectedValue = typeof(DateTime);
        else
          ValueType.SelectedValue = typeof(string);

      }
    }

    private void EhValueTypeChanged(Type obj)
    {
      OnPropertyChanged(nameof(IsDouble));
      OnPropertyChanged(nameof(IsString));
      OnPropertyChanged(nameof(IsDateTime));

      OnPropertyChanged(nameof(ValueAsDouble));
      OnPropertyChanged(nameof(ValueAsString));
      OnPropertyChanged(nameof(ValueAsDateTime));

    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}
