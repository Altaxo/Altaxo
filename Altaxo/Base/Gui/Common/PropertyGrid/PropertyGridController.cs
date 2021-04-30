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
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Gui.Common.BasicTypes;

namespace Altaxo.Gui.Common.PropertyGrid
{
  public interface IPropertyGridView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IPropertyGridView))]
  public class PropertyGridController : MVCANControllerEditImmutableDocBase<object, IPropertyGridView> 
  {
    private Type TypeOfDocument { get; set; }

    public class ValueInfo : ICategoryNameView
    {
      public string Category { get; set; } = string.Empty;
      public string Name { get; }
      public object? Value { get; }
      public Type ValueType { get; }
      public object? MethodOrPropertyInfo { get; }

      public IMVCAController Controller { get; set; }

      public object View => Controller.ViewObject!;

      public ValueInfo(string name, IMVCAController controller)
      {
        Name = name;
        Controller = controller;
        Value = controller.ModelObject;
        ValueType = Value.GetType();
      }

      public ValueInfo(string name, Type valueType, MethodInfo method, object? value, IMVCAController controller)
      {
        Name = name;
        ValueType = valueType;
        MethodOrPropertyInfo = method;
        Value = value;
        Controller = controller;
      }

      public ValueInfo(string name, PropertyInfo propertyInfo, object? value, IMVCAController controller)
      {
        Name = name;
        ValueType = propertyInfo.PropertyType;
        MethodOrPropertyInfo = propertyInfo;
        Value = value;
        Controller = controller;
      }
    }


    public ObservableCollection<ValueInfo> ValueInfos { get; private set; } = new ObservableCollection<ValueInfo>();

    /// <summary>
    /// Gets or sets a label text that is shown if our document is a basic type (i.e. e.g. int, string double and so on).
    /// </summary>
    public string LabelIfDocumentIsBasicType { get; set; } = "Value";

    public PropertyGridController()
    {
      TypeOfDocument = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridController"/> class for an arbitrary object.
    /// </summary>
    /// <param name="value">The value.</param>
    public PropertyGridController(object value)
    {
      _doc = _originalDoc = value ?? throw new ArgumentNullException(nameof(value));
      TypeOfDocument = _doc.GetType();
      Initialize(true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridController"/> class for a basic type, like string, int, double and so on.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="labelForBasicType">Label that is shown next to the edit box.</param>
    public PropertyGridController(object value, string labelForBasicType)
    {
      _doc = _originalDoc = value ?? throw new ArgumentNullException(nameof(value));
      TypeOfDocument = _doc.GetType();
      LabelIfDocumentIsBasicType = labelForBasicType;
      Initialize(true);
    }

    protected override void Initialize(bool initData)
    {
      if (_doc is null)
        throw new InvalidOperationException("Controller is not initialized with a document");

      base.Initialize(initData);

      if(initData)
      {
        TypeOfDocument = _doc.GetType();
        InitializeValueInfos();
      }
    }

    protected virtual void InitializeValueInfos()
    {
      var doctype = _doc.GetType();

      // Special treatment is neccessary if our document itself is a basic type.
      if(IsBasicType(doctype)) // if our document is a basic type, then we can add a controller directly
      {
        if (TryGetControllerAndControl(_doc, doctype) is { } controller)
        {
          ValueInfos.Add(new ValueInfo(LabelIfDocumentIsBasicType, controller));
          return;
        }
      }

      var propertyInfos = doctype.GetProperties();
      foreach (var propertyInfo in propertyInfos.Where(x => x.CanWrite && x.CanRead)) // First, we are interested in the writable properties
      {
        var parameterValue = propertyInfo.GetValue(_doc);

        if (TryGetControllerAndControl(parameterValue, propertyInfo.PropertyType) is { } controller)
        {
          var valueInfo = new ValueInfo(propertyInfo.Name, propertyInfo, parameterValue, controller);
          if (propertyInfo.GetCustomAttribute(typeof(System.ComponentModel.CategoryAttribute)) is System.ComponentModel.CategoryAttribute attr)
          {
            valueInfo.Category = attr.Category;
          }
          ValueInfos.Add(valueInfo);
        }
      }

      foreach (var methodInfo in doctype.GetMethods())
      {
        if (!(methodInfo.ReturnType == _doc.GetType()))
          continue;

        var methodParameters = methodInfo.GetParameters();
        if (methodParameters.Length != 1)
          continue;

        var parameterType = methodParameters[0].ParameterType;

        if (parameterType != typeof(double) && parameterType != typeof(DateTime) && parameterType != typeof(string))
          continue;

        var parameterName = methodParameters[0].Name;
        if (parameterName is null)
          continue;

        parameterName = parameterName.Substring(0, 1).ToUpperInvariant() + parameterName.Substring(1); // Capitalize parameter name

        object? parameterValue = null;
        var getterProp = _doc.GetType().GetProperty(parameterName);
        if (getterProp is not null && getterProp.PropertyType == parameterType)
          parameterValue = getterProp.GetValue(_doc, null);

        if (TryGetControllerAndControl(parameterValue, parameterType) is { } controller)
        {
          var valueInfo = new ValueInfo(parameterName, parameterType, methodInfo, parameterValue, controller);
          if (methodInfo.GetCustomAttribute(typeof(System.ComponentModel.CategoryAttribute)) is System.ComponentModel.CategoryAttribute attr)
          {
            valueInfo.Category = attr.Category;
          }

          ValueInfos.Add(valueInfo);
        }
      }

      GroupValueInfosByCategory();
    }
    /// <summary>
    /// Sorts the <see cref="ValueInfos"/> by its category. Here, the order of the items is preserved, thus the categories are
    /// not sorted alphabetically, but grouped according to its first occurence.
    /// </summary>
    protected virtual void GroupValueInfosByCategory()
    {
      for(int i=0;i<ValueInfos.Count;++i)
      {
        var currentCategory = ValueInfos[i].Category;
        var currentInsertPosition = i + 1;
        for (int j = i + 1; j < ValueInfos.Count; ++j) 
        {
          if (ValueInfos[j].Category == currentCategory)
          {
            var toInsert = ValueInfos[j];
            ValueInfos.RemoveAt(j);
            ValueInfos.Insert(currentInsertPosition, toInsert);
            ++currentInsertPosition;
          }
        }
        // move forward until the next item's category is no longer currentCategory
        for (; i < (ValueInfos.Count - 1) && ValueInfos[i + 1].Category == currentCategory; ++i)
        {
        }
      }
    }

    protected IMVCAController? TryGetControllerAndControl(object? value, Type valueType)
    {
      IMVCAController? controller = null;

      if (valueType == typeof(bool))
        controller = new BasicTypes.BooleanValueController((bool)(value ?? Activator.CreateInstance(valueType)!)) { DescriptionText = "    " }; // descriptionText consist of some spaces because then checkbox is easier to click on with a mouse
      else if (valueType == typeof(Enum))
        controller = new BasicTypes.EnumValueController((Enum)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(sbyte))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(byte))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(short))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(ushort))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(int))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(uint))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(long))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(ulong))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(BigInteger))
        controller = new BasicTypes.IntegerValueController((value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(decimal))
        controller = new BasicTypes.DecimalValueController((decimal)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(float))
        controller = new NumericFloatValueController((float)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(double))
        controller = new NumericDoubleValueController((double)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(DateTime))
        controller = new BasicTypes.DateTimeValueController((DateTime)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(DateTimeOffset))
        controller = new BasicTypes.DateTimeOffsetValueController((DateTimeOffset)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(TimeSpan))
        controller = new BasicTypes.TimeSpanValueController((TimeSpan)(value ?? Activator.CreateInstance(valueType)!));
      else if (valueType == typeof(string))
        controller = new StringValueController((string)(value ?? string.Empty));
      
      if (controller is not null)
      {
        Current.Gui.FindAndAttachControlTo(controller);
        if (controller.ViewObject is not null)
        {
          return controller;
        }
        else
        {
          controller.Dispose();
          controller = null;
        }
      }
      return TryGetControllerForOther(value, valueType);
    }

    private IMVCAController? TryGetControllerForOther(object? value, Type valueType)
    {
      IMVCAController? controller = null;
      if (value is not null)
      {
        controller = (IMVCAController?)Current.Gui.GetController(new object[] { value }, typeof(IMVCAController));
      }
      else
      {
        controller = (IMVCAController?)Current.Gui.GetController(new object[] { null! }, valueType, typeof(IMVCAController), UseDocument.Copy);
      }

      if (controller is not null && controller is not PropertyGridController)
      {
        Current.Gui.FindAndAttachControlTo(controller);
        if (controller.ViewObject is not null)
        {
          return controller;
        }
        else
        {
          controller.Dispose();
          return null;
        }

      }
      return null;
    }

    public override bool Apply(bool disposeController)
    {
      if (IsBasicType(TypeOfDocument))
      {
        var valueInfo = ValueInfos[0];
        if (valueInfo.Controller.Apply(disposeController))
        {
          _doc = valueInfo.Controller.ModelObject;
          return ApplyEnd(true, disposeController);
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }
      else // not a basic type
      {
        for (int i = 0; i < ValueInfos.Count; ++i)
        {
          var valueInfo = ValueInfos[i];

          if (!valueInfo.Controller.Apply(disposeController))
          {
            return ApplyEnd(false, disposeController);
          }

          object newValue = valueInfo.Controller.ModelObject;

          if (ValueInfos[i].MethodOrPropertyInfo is PropertyInfo pi && pi.SetMethod is not null)
          {
            pi.SetMethod.Invoke(_doc, new[] { newValue });
          }

          else if (ValueInfos[i].MethodOrPropertyInfo is MethodInfo mi && mi.Invoke(_doc, new[] { newValue }) is { } newdoc && TypeOfDocument.IsAssignableFrom(newdoc.GetType()))
          {
            _doc = newdoc;
          }
        }


        return ApplyEnd(true, disposeController);
      }
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      foreach (var item in ValueInfos)
      {
        if (item.Controller is not null)
          yield return new ControllerAndSetNullMethod(item.Controller, () => item.Controller = null!);
      }
    }

    private static readonly HashSet<Type> _basicTypes = new HashSet<Type>
    {
      typeof(bool),
      typeof(sbyte),
      typeof(byte),
      typeof(short),
      typeof(ushort),
      typeof(int),
      typeof(uint),
      typeof(long),
      typeof(ulong),
      typeof(BigInteger),
      typeof(decimal),
      typeof(float),
      typeof(double),
      typeof(DateTime),
      typeof(DateTimeOffset),
      typeof(TimeSpan),
      typeof(Enum),
      typeof(string)
    };

    private static bool IsBasicType(Type t)
    {
      return _basicTypes.Contains(t);
    }
  }
}
