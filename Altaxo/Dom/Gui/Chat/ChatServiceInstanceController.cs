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

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Chat;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Chat
{
  /// <summary>
  /// Defines the view used to edit a chat service instance.
  /// </summary>
  public interface IChatServiceInstanceView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Edits a chat service instance.
  /// </summary>
  [ExpectedTypeOfView(typeof(IChatServiceInstanceView))]
  [UserControllerForObject(typeof(IChatService), priority: -1)]
  public class ChatServiceInstanceController : MVCANControllerEditImmutableDocBase<IChatService, IChatServiceInstanceView>
  {
    private Dictionary<Type, IChatService> _generatedInstances = new Dictionary<Type, IChatService>();

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the controller for the currently selected service instance.
    /// </summary>
    public ItemsController<Type> ServiceTypes
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ServiceTypes));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for the currently selected service instance.
    /// </summary>
    public IMVCAController InstanceController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(InstanceController));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length > 0 && args[0] is null)
      {
        Initialize(true);
        return true;
      }
      else
      {
        return base.InitializeDocument(args);
      }
    }

    /// <inheritdoc/>
    protected override void ThrowIfNotInitialized()
    {
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var types = ReflectionService.GetNonAbstractSubclassesOf(typeof(IChatService));

        ServiceTypes = new ItemsController<Type>(new SelectableListNodeList(types.Select(t => new SelectableListNode(t.Name, t, false))), EhServiceTypeChanged);
        if (_doc is not null)
        {
          _generatedInstances.Add(_doc.GetType(), _doc);
          ServiceTypes.SelectedValue = _doc.GetType();
        }
      }
    }

    private void EhServiceTypeChanged(Type type)
    {
      if (type is null)
        return;

      if (!_generatedInstances.TryGetValue(type, out var instance))
      {
        instance = (IChatService)Activator.CreateInstance(type);
        _generatedInstances.Add(type, instance);
      }
      _doc = instance;
      var controller = (IMVCANController)Current.Gui.GetController(new object[] { _doc }, typeof(IMVCANController));

      if (controller.GetType() != this.GetType())
      {
        Current.Gui.FindAndAttachControlTo(controller);
        InstanceController = controller;
      }


    }
    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (InstanceController is { } ctrl)
      {
        if (!ctrl.Apply(disposeController))
          return ApplyEnd(false, disposeController);
        _doc = (IChatService)ctrl.ModelObject;
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
