#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Gui.Main
{
  using Altaxo.Collections;
  using Altaxo.Main.Properties;

  public interface IPropertyBagView
  {
    SelectableListNodeList PropertyList { set; }
  }

  [ExpectedTypeOfView(typeof(IPropertyBagView))]
  [UserControllerForObject(typeof(PropertyBag))]
  public class PropertyBagController : MVCANControllerEditOriginalDocBase<PropertyBag, IPropertyBagView>
  {
    #region Inner types

    private class MyListNode : SelectableListNode
    {
      private string[] _subText = new string[3];

      public MyListNode(string text, object tag)
        : base(text, tag, false)
      {
      }

      public override int SubItemCount
      {
        get
        {
          return _subText.Length;
        }
      }

      public override string SubItemText(int i)
      {
        return _subText[i - 1];
      }

      public string Text1a { set { _subText[0] = value; } }

      public string Text2a { set { _subText[1] = value; } }

      public string Text3a { set { _subText[2] = value; } }
    }

    #endregion Inner types

    private SelectableListNodeList _propertyList;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _propertyList = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _propertyList = new SelectableListNodeList();
        foreach (var entry in _doc)
        {
          string key = entry.Key;
          object value = entry.Value;

          var node = new MyListNode(key, key)
          {
            Text1a = value == null ? "n.a." : value.GetType().Name,
            Text2a = value == null ? "null" : value.ToString()
          };

          _propertyList.Add(node);
        }
      }
      if (null != _view)
      {
        _view.PropertyList = _propertyList;
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}
