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
using System.Linq;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{

  /// <summary>
  /// Interface for the view of the ActionTestConditionForTemplateController.
  /// </summary>
  public interface IActionPutToPropertyBagView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing an <see cref="ActionTextConditionForTemplate"/> instance.
  /// </summary>
  [ExpectedTypeOfView(typeof(IActionPutToPropertyBagView))]
  [UserControllerForObject(typeof(ActionPutToPropertyBag))]
  public class ActionPutToPropertyBagController : MVCANControllerEditImmutableDocBase<ActionPutToPropertyBag, IActionPutToPropertyBagView>
  {
    private List<string> _propertyNames;

    /// <summary>
    /// Gets the subcontrollers used by this controller.
    /// </summary>
    /// <returns>An enumeration of subcontrollers.</returns>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the property names to choose from.
    /// </summary>
    public ItemsController<string> PropertyNames
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(PropertyNames));
        }
      }
    }

    /// <summary>
    /// Gets or sets the level to put the property into.
    /// </summary>
    public int Level
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Level));
        }
      }
    }


    #endregion Bindings

    /// <inheritdoc />
    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length > 1 && args[1] is IEnumerable<string> propertyNames)
      {
        _propertyNames = propertyNames.ToList();
      }
      else
      {
        _propertyNames = new List<string>();
      }

      return base.InitializeDocument(args);
    }

    /// <summary>
    /// Initializes the controller state from the document.
    /// </summary>
    /// <param name="initData">If set to <see langword="true"/>, initializes binding properties from the document.</param>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        if (!_propertyNames.Contains(_doc.PropertyName))
        {
          _propertyNames.Add(_doc.PropertyName);
        }
        _propertyNames.Sort();

        PropertyNames = new ItemsController<string>(
          new SelectableListNodeList(_propertyNames.Select(name => new SelectableListNode(name, name, false)))
          )
        {
          SelectedValue = _doc.PropertyName
        };
        Level = _doc.Level;
      }
    }

    /// <summary>
    /// Applies the current controller values to the immutable document.
    /// </summary>
    /// <param name="disposeController">If set to <see langword="true"/>, disposes the controller after apply.</param>
    /// <returns><see langword="true"/> if applying succeeded; otherwise <see langword="false"/>.</returns>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        PropertyName = PropertyNames.SelectedValue,
        Level = Level,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
