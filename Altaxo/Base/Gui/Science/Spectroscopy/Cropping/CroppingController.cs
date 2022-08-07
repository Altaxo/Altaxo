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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;
using Altaxo.Science.Spectroscopy.Cropping;

namespace Altaxo.Gui.Science.Spectroscopy.Cropping
{
  public interface ICroppingView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ICroppingView))]
  public class CroppingController : MVCANControllerEditImmutableDocBase<ICropping, ICroppingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_subController, () => SubController = null);
    }

    #region Bindings

    private ItemsController<Type> _availableMethods;

    public ItemsController<Type> AvailableMethods
    {
      get => _availableMethods;
      set
      {
        if (!(_availableMethods == value))
        {
          _availableMethods = value;
          OnPropertyChanged(nameof(AvailableMethods));
        }
      }
    }


    private IMVCANController? _subController;

    public IMVCANController? SubController
    {
      get => _subController;
      set
      {
        if (!(_subController == value))
        {
          _subController?.Dispose();
          _subController = value;
          OnPropertyChanged(nameof(SubController));
        }
      }
    }


    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        CreateSubController();

        var methodTypes = new List<Type>(ReflectionService.GetNonAbstractSubclassesOf(typeof(ICropping)));
        methodTypes.Sort(new TypeSorter());
        
        var methods = new SelectableListNodeList();
        foreach (var methodType in methodTypes)
        {
          methods.Add(new SelectableListNode(methodType.Name, methodType, methodType == _doc.GetType()));
        }
        AvailableMethods = new ItemsController<Type>(methods, EhMethodTypeChanged);
      }
    }

    private void CreateSubController()
    {
      var subController = (IMVCANController)Current.Gui.GetController(new object[] { _doc }, typeof(IMVCANController));
      if (subController?.GetType() == GetType())
      {
        subController = null;
      }
      if (subController is not null)
      {
        Current.Gui.FindAndAttachControlTo(subController);
      }
      SubController = subController;
    }

    private void EhMethodTypeChanged(Type newMethodType)
    {
      _doc = (ICropping)Activator.CreateInstance(newMethodType);
      CreateSubController();
    }

    public override bool Apply(bool disposeController)
    {
      if(SubController is not null)
      {
        if (!SubController.Apply(disposeController))
          return ApplyEnd(false, disposeController);
        else
          _doc = (ICropping)SubController.ModelObject;
      }

      return ApplyEnd(true, disposeController);
    }


    #region TypeSorter

    class TypeSorter : IComparer<Type>
    {
      public int Compare(Type x, Type y)
      {
        var xn = x.Name.EndsWith("None");
        var yn = y.Name.EndsWith("None");

        if (xn != yn)
        {
          return xn ? -1 : 1;
        }
        else
        {
          return string.Compare(x.Name, y.Name);
        }
      }
    }


    #endregion
  }
}
