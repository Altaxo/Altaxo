using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Common.PropertyGrid
{
  public interface IPropertyGridView : IDataContextAwareView
  {
    /// <summary>
    /// Initializes the view's values. Item1 is the Name of the value to set; Item2 is the value type, and Item3 is the value itself.
    /// </summary>
    /// <param name="values">The values.</param>
    void Values_Initialize(IEnumerable<Tuple<string, Type, object?>> values);

    /// <summary>
    /// Gets the value at index idx.
    /// </summary>
    /// <param name="idx">The index.</param>
    /// <returns></returns>
    object Value_Get(int idx);
  }

  [ExpectedTypeOfView(typeof(IPropertyGridView))]
  public class PropertyGridController<TObject> : MVCANControllerEditImmutableDocBase<TObject, IPropertyGridView> where TObject: class
  {
    private class ValueInfo
    {
      public string Name { get; }
      public object? Value { get; }
      public Type ValueType { get; }
      public object MethodOrPropertyInfo { get; }

      public ValueInfo(string name, Type valueType, MethodInfo method, object? value)
      {
        Name = name;
        ValueType = valueType;
        MethodOrPropertyInfo = method;
        Value = value;
      }

      public ValueInfo(string name, PropertyInfo propertyInfo, object? value)
      {
        Name = name;
        ValueType = propertyInfo.PropertyType;
        MethodOrPropertyInfo = propertyInfo;
        Value = value;
      }
    }

    private List<ValueInfo> _valueInfo = new List<ValueInfo>();



    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        GetPropertiesAndImmutableMethods();
      }
      if (_view is not null)
      {
        _view.Values_Initialize(_valueInfo.Select(x => new Tuple<string, Type, object?>(x.Name, x.ValueType, x.Value)));
      }
    }

    private void GetPropertiesAndImmutableMethods()
    {
      var doctype = _doc.GetType();

      var propertyInfos = doctype.GetProperties();
      foreach (var propertyInfo in propertyInfos.Where(x => x.CanWrite)) // First, we are interested in the writable properties
      {
        var valueInfo = new ValueInfo(propertyInfo.Name, propertyInfo, propertyInfo.GetValue(_doc));
        _valueInfo.Add(valueInfo);
      }

      foreach (var method in doctype.GetMethods())
      {
        if (!(method.ReturnType == _doc.GetType()))
          continue;

        var methodParameters = method.GetParameters();
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

        _valueInfo.Add(new ValueInfo(parameterName, parameterType, method, parameterValue));
      }
    }


    public override bool Apply(bool disposeController)
    {
      if (_view is null)
        throw NoViewException;

      for (int i = 0; i < _valueInfo.Count; ++i)
      {
        var value = _view.Value_Get(i);


        if (_valueInfo[i].MethodOrPropertyInfo is MethodInfo mi && mi.Invoke(_doc, new[] { value }) is TObject newdoc)
          _doc = newdoc;
        else if (_valueInfo[i].MethodOrPropertyInfo is PropertyInfo pi)
          pi.SetMethod.Invoke(_doc, new[] { value });
      }

      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
