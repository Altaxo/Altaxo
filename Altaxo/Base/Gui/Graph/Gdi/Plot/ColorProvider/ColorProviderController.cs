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

#nullable disable
using System;
using System.Drawing;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.ColorProvider;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
  /// <summary>
  /// Interface that must be implemented by Gui classes that allow to select a color provider.
  /// </summary>
  public interface IColorProviderView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Summary description for ColorProviderController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColorProviderView))]
  //[UserControllerForObject(typeof(IColorProvider), 101)]
  public class ColorProviderController : MVCANDControllerEditImmutableDocBase<IColorProvider, IColorProviderView>
  {
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_detailController, () => DetailController = null);
    }

    #region Bindings

    private ItemsController<Type> _availableClasses;

    public ItemsController<Type> AvailableClasses
    {
      get => _availableClasses;
      set
      {
        if (!(_availableClasses == value))
        {

          _availableClasses = value;
          OnPropertyChanged(nameof(AvailableClasses));
        }
      }
    }

    private IMVCAController _detailController;

    public IMVCAController DetailController
    {
      get => _detailController;
      set
      {
        if (!(_detailController == value))
        {
          if (_detailController is IMVCANDController oldD)
            oldD.MadeDirty -= EhDetailsChanged;
          _detailController?.Dispose();
          _detailController = value;
          if (_detailController is IMVCANDController newD)
            newD.MadeDirty += EhDetailsChanged;

          OnPropertyChanged(nameof(DetailController));
        }
      }
    }

    #endregion

    public ColorProviderController(Action<IColorProvider> SetInstanceInParentDoc)
      : base(SetInstanceInParentDoc)
    {
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        InitClassTypes();
        InitDetailController();
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_detailController is not null)
      {
        if (!_detailController.Apply(disposeController))
          return false;
      }

      _doc = (IColorProvider)_detailController.ModelObject;

      return ApplyEnd(true, disposeController);
    }

    public void InitClassTypes()
    {
      var availableClasses = new SelectableListNodeList();
      Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorProvider));
      foreach (var ty in classes)
      {
        var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(ty), ty, _doc.GetType() == ty);
        availableClasses.Add(node);
      }
      AvailableClasses = new ItemsController<Type>(availableClasses, EhColorProviderSelectionChanged);
    }

    public void InitDetailController()
    {
      object providerObject = _doc;

      var detailController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { providerObject }, typeof(IMVCANController), UseDocument.Directly);

      if (detailController is not null && GetType() == detailController.GetType()) // the returned controller is of this common type here -> thus no specialized controller seems to exist for this type of color provider
      {
        detailController.Dispose();
        detailController = null;
      }

      if (detailController is not null)
      {
        Current.Gui.FindAndAttachControlTo(detailController);
      }

      DetailController = detailController;
    }

    private void EhColorProviderSelectionChanged(Type chosenType)
    {
      if (chosenType is null)
        return;

      try
      {
        if (chosenType != _doc.GetType())
        {
          // replace the current axis by a new axis of the type axistype
          var oldDoc = _doc;
          var newDoc = (IColorProvider)System.Activator.CreateInstance(chosenType);

          if (newDoc is ColorProviderBase && oldDoc is ColorProviderBase)
          {
            var oldBase = (ColorProviderBase)oldDoc;

            newDoc = ((ColorProviderBase)newDoc)
              .WithColorBelow(oldBase.ColorBelow)
              .WithColorAbove(oldBase.ColorAbove)
              .WithColorInvalid(oldBase.ColorInvalid)
              .WithTransparency(oldBase.Transparency)
              .WithColorSteps(oldBase.ColorSteps);
          }

          _doc = newDoc;
          OnMadeDirty(); // Change for the controller up in hierarchy to grab new document

          InitDetailController();
        }
      }
      catch (Exception)
      {
      }
    }

    private void EhDetailsChanged(IMVCANDController ctrl)
    {
      _detailController.Apply(false); // we use the instance directly, thus no further taking of the instance is neccessary here
      _doc = (IColorProvider)(_detailController.ModelObject);
    }
  }
}

