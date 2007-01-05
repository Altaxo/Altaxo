#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Gui.Common
{
  public class SingleInstanceChoice
  {
    System.Type _instanceType;
    object      _instance;

    public System.Type InstanceType
    {
      get
      {
        return _instanceType;
      }
      
    }
    public object Instance
    {
      get
      {
        return _instance;
      }

      set
      {
        if(null!=value && !Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(value.GetType(),_instanceType))
          throw new ArgumentException("The provided instance object is not a subclass (or implements) type instanceType");
        _instance = value;
      }
    }

    public SingleInstanceChoice(System.Type instanceType, object instance)
    {
      Initialize(instanceType, instance);
    }

    public void Initialize(System.Type instanceType, object instance)
    {
      if(instanceType==null)
        throw new ArgumentException("instanceType must not be null");
      if(null!=instance && !Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(instance.GetType(),instanceType))
        throw new ArgumentException("The provided instance object is not a subclass (or implements) type instanceType");

      _instanceType = instanceType;
      _instance = instance;
    }
  }

  
  /// <summary>
  /// Summary description for SingleInstanceChoiceController.
  /// </summary>
  [UserControllerForObject(typeof(SingleInstanceChoice))]
  public class SingleInstanceChoiceController : SingleChoiceController
  {
    SingleInstanceChoice _doc;
    System.Type[] _types;

    public SingleInstanceChoiceController(SingleInstanceChoice doc)
    {
      _doc = doc;
      _types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(_doc.InstanceType);

      int selectedIndex=-1;
      string[] choices = new string[_types.Length];
      for(int i=0;i<_types.Length;i++)
      {
        choices[i] = Current.Gui.GetUserFriendlyClassName(_types[i]);
        if(_doc.Instance!=null && _doc.Instance.GetType()==_types[i])
          selectedIndex=i;
      }

      base.Initialize(choices,selectedIndex);


    }

      

    public override object ModelObject
    {
      get
      {
        return _doc;
      }
    }


    public override bool Apply()
    {
      if(!base.Apply())
        return false;

      int selected = base._choice;
      if(_doc.Instance!=null && _doc.Instance.GetType()==_types[selected])
        return true;
      else
      {
        object current = System.Activator.CreateInstance(_types[selected]);
        if(current!=null)
        {
          _doc.Instance = current;
          return true;
        }
      }

      return false;


  
    }
  }
}
