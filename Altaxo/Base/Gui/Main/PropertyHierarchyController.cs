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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Main
{
  using Altaxo.Collections;
  using Altaxo.Main.Properties;

  public interface IPropertyHierarchyView
  {
    SelectableListNodeList PropertyValueList { set; }

    SelectableListNodeList AvailablePropertyKeyList { set; }

    /// <summary>
    /// Occurs when the selected item should be edited.
    /// </summary>
    event Action ItemEditing;

    /// <summary>
    /// Occurs when the user double-clicks on an available property key in order to create a new property value.
    /// </summary>
    event Action PropertyCreation;

    /// <summary>
    /// Occurs when the user wants to create a new basic property (string, double, int, DateTime) with a user defined name.
    /// </summary>
    event Action AddNewBasicProperty;

    /// <summary>
    /// Occurs when the user wants to remove the selected property values.
    /// </summary>
    event Action ItemRemoving;

    /// <summary>
    /// Occurs when the user wants to change the view. If the argument is true, all properties (including the inherited properties) should be shown, otherwise only the properties of the topmost bag.
    /// </summary>
    event Action<bool> ShowAllPropertiesChanged;

    /// <summary>
    /// Sets a value indicating to the user whether to show all properties or only those of the topmost property bag.
    /// </summary>
    /// <value>
    ///   <c>true</c> if to show all properties; otherwise, <c>false</c>.
    /// </value>
    bool ShowAllProperties { set; }
  }

  [ExpectedTypeOfView(typeof(IPropertyHierarchyView))]
  [UserControllerForObject(typeof(PropertyHierarchy))]
  public class PropertyHierarchyController : MVCANControllerEditCopyOfDocBase<PropertyHierarchy, IPropertyHierarchyView>
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

      public string Text1S { set { _subText[0] = value; } }

      public string Text2S { set { _subText[1] = value; } }

      public string Text3S { set { _subText[2] = value; } }
    }

    #endregion Inner types

    private SelectableListNodeList _propertyList;

    private SelectableListNodeList _availablePropertyKeys;

    /// <summary>If <c>true</c>, all properties (also the inherited properties) are shown. If <c>false</c>, only the inherited properties are shown.</summary>
    protected bool _showAllProperties = true;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _propertyList = null;
      _availablePropertyKeys = null;

      base.Dispose(isDisposing);
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (null == args || 0 == args.Length || !(args[0] is PropertyHierarchy))
        return false;

      _doc = _originalDoc = (PropertyHierarchy)args[0];
      if (_useDocumentCopy)
        _doc = _originalDoc.CreateCopyWithOnlyTopmostBagCloned();

      Initialize(true);
      return true;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Altaxo.Main.Services.ReflectionService.ForceRegisteringOfAllPropertyKeys();
        InitializeAvailablePropertyList();
        InitializeExistingPropertyValuesList();
      }
      if (null != _view)
      {
        _view.AvailablePropertyKeyList = _availablePropertyKeys;
        _view.PropertyValueList = _propertyList;
        _view.ShowAllProperties = _showAllProperties;
      }
    }

    private void InitializeAvailablePropertyList()
    {
      _availablePropertyKeys = new SelectableListNodeList();

      var sortedKeys = new List<Altaxo.Main.Properties.PropertyKeyBase>(Altaxo.Main.Properties.PropertyKeyBase.AllRegisteredPropertyKeys);
      sortedKeys.Sort((x, y) => string.Compare(x.PropertyName, y.PropertyName));

      foreach (var prop in sortedKeys)
      {
        // show only the keys that are applicable to the topmost property bag in the hierarchy
        if (0 == (prop.ApplicationLevel & _doc.TopmostBagInformation.ApplicationLevel))
          continue;
        if ((PropertyLevel.Document == (prop.ApplicationLevel & _doc.TopmostBagInformation.ApplicationLevel)))
        {
          if (!Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(_doc.TopmostBagInformation.ApplicationItemType, prop.ApplicationItemType))
            continue;
        }

        var node = new MyListNode(prop.PropertyName, prop)
        {
          Text1S = prop.PropertyType.Name
        };

        _availablePropertyKeys.Add(node);
      }
    }

    private void InitializeExistingPropertyValuesList()
    {
      var sortedNames = new List<KeyValuePair<string, string>>(); // key is the property key string, value is the property name
      foreach (var key in _doc.GetAllPropertyNames())
      {
        string keyName = PropertyKeyBase.GetPropertyName(key);
        if (null == keyName)
          keyName = key;
        sortedNames.Add(new KeyValuePair<string, string>(key, keyName));
      }
      sortedNames.Sort(((entry1, entry2) => string.Compare(entry1.Value, entry2.Value))); // sort the entries not by the key, but by the keyname

      _propertyList = new SelectableListNodeList();

      foreach (var entry in sortedNames)
      {
        object value;
        IPropertyBag bag;
        PropertyBagInformation bagInfo;

        if (_doc.TryGetValue<object>(entry.Key, !_showAllProperties, out value, out bag, out bagInfo))
        {
          var node = new MyListNode(entry.Value, new Tuple<string, string, IPropertyBag>(entry.Key, entry.Value, bag))
          {
            Text1S = value == null ? "n.a." : value.GetType().Name,
            Text2S = value == null ? "null" : value.ToString().Replace('\n', '_').Replace('\r', '_'),
            Text3S = bagInfo.Name
          };

          _propertyList.Add(node);
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!object.ReferenceEquals(_doc, _originalDoc))
        _originalDoc.TopmostBag.CopyFrom(_doc.TopmostBag);

      if (disposeController)
        Dispose();

      return true;
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.ItemEditing += EhItemEditing;
      _view.PropertyCreation += EhCreateNewProperty;
      _view.ItemRemoving += EhItemRemoving;
      _view.ShowAllPropertiesChanged += EhShowAllProperties;
      _view.AddNewBasicProperty += EhAddNewBasicProperty;
    }

    protected override void DetachView()
    {
      _view.AddNewBasicProperty -= EhAddNewBasicProperty;
      _view.ShowAllPropertiesChanged -= EhShowAllProperties;
      _view.ItemRemoving -= EhItemRemoving;
      _view.PropertyCreation -= EhCreateNewProperty;
      _view.ItemEditing -= EhItemEditing;
      base.DetachView();
    }

    private void EhItemEditing()
    {
      var node = _propertyList.FirstSelectedNode;
      if (null == node)
        return;

      var nodeTag = (Tuple<string, string, IPropertyBag>)node.Tag;
      var propertyKey = nodeTag.Item1;
      var propertyName = nodeTag.Item2;

      object value;
      IPropertyBag bag;
      PropertyBagInformation bagInfo;

      _doc.TryGetValue(propertyKey, out value, out bag, out bagInfo);

      ShowPropertyValueDialog(propertyKey, propertyName, value);
    }

    private void EhItemRemoving()
    {
      var node = _propertyList.FirstSelectedNode;
      if (null == node)
        return;

      var nodeTag = (Tuple<string, string, IPropertyBag>)node.Tag;
      var propertyKey = nodeTag.Item1;
      _doc.TopmostBag.RemoveValue(propertyKey);

      // update list and view
      InitializeExistingPropertyValuesList();
      if (null != _view)
        _view.PropertyValueList = _propertyList;
    }

    private void EhCreateNewProperty()
    {
      var node = _availablePropertyKeys.FirstSelectedNode;
      if (null == node)
        return;

      var propertyKey = (PropertyKeyBase)node.Tag;

      object propertyValue;
      IPropertyBag bag;
      PropertyBagInformation bagInfo;

      if (!_doc.TryGetValue(propertyKey.GuidString, out propertyValue, out bag, out bagInfo))
      {
        // Try to create a new value
        try
        {
          propertyValue = System.Activator.CreateInstance(propertyKey.PropertyType);
        }
        catch (Exception)
        {
          Current.Gui.ErrorMessageBox("Sorry! The property value could not be created because the constructor threw an exception.");
          return;
        }
      }

      ShowPropertyValueDialog(propertyKey.GuidString, propertyKey.PropertyName, propertyValue);
    }

    private void EhAddNewBasicProperty()
    {
      var propertyData = new AddBasicPropertyValueData();
      if (true == Current.Gui.ShowDialog(ref propertyData, "Add new user defined property value", false))
      {
        _doc.TopmostBag.SetValue(propertyData.PropertyName, propertyData.PropertyValue);

        // update list and view
        InitializeExistingPropertyValuesList();
        if (null != _view)
          _view.PropertyValueList = _propertyList;
      }
    }

    private void EhShowAllProperties(bool value)
    {
      var oldValue = _showAllProperties;
      _showAllProperties = value;

      if (oldValue != value)
      {
        // update list and view
        InitializeExistingPropertyValuesList();
        if (null != _view)
          _view.PropertyValueList = _propertyList;
      }
    }

    private void ShowPropertyValueDialog(string propertyKey, string propertyName, object propertyValue)
    {
      IMVCAController controller = null;
      var pk = PropertyKeyBase.GetPropertyKey(propertyKey);
      if (null != pk && pk.CanCreateEditingController)
      {
        controller = pk.CreateEditingController(propertyValue);
      }

      if (null == controller)
        controller = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { propertyValue }, typeof(IMVCAController), UseDocument.Copy);

      if (null == controller)
      {
        Current.Gui.ErrorMessageBox("Sorry! Didn't find a Gui controller to edit this property value!");
        ;
        return;
      }

      if (Current.Gui.ShowDialog(controller, "Edit property " + propertyName, false))
      {
        var newValue = controller.ModelObject;
        _doc.TopmostBag.SetValue(propertyKey, newValue);
        InitializeExistingPropertyValuesList();
        if (null != _view)
          _view.PropertyValueList = _propertyList;
      }
    }
  }
}
