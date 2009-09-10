using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{

  public interface IRoutedPropertyReceiver
  {
    void SetRoutedProperty(IRoutedSetterProperty property);
    void GetRoutedProperty(IRoutedGetterProperty property);
  }


  public interface IRoutedSetterProperty
  {
    string Name { get; }
    bool InheritToChilds { get; }
    System.Type TypeOfValue { get; }
    object ValueAsObject { get; }
  }

  public interface IRoutedGetterProperty
  {
    string Name { get; }
    System.Type TypeOfValue { get; }
  }


  public class RoutedSetterProperty<T> : IRoutedSetterProperty
  {
    T _value;
    string _name;
    bool _inheritToChilds;

    public RoutedSetterProperty(string name, T value)
    {
      _name = name;
      _value = value;
    }

    public virtual string Name { get { return _name; } }

    public T Value { get { return _value; } }

    public System.Type TypeOfValue { get { return typeof(T); } }

    public bool InheritToChilds { get { return _inheritToChilds; } set { _inheritToChilds = value; } }
    
    public object ValueAsObject
    {
      get
      {
        return _value;
      }
    }
  }
 
  public class RoutedGetterProperty<T> : IRoutedGetterProperty
  {
    public string Name { get; set; }
    public System.Type TypeOfValue { get { return typeof(T); } }

    T _value;
    bool _wasSet;
    bool _doNotMatch;

    public T Value { get { return _value; } }

    public void Merge(T t)
    {
      if (!_wasSet)
      {
        _value = t;
        _wasSet = true;
      }
      else
      {
        if (!_doNotMatch && !object.Equals(t, _value))
          _doNotMatch = true;
      }
    }
  }
 

}
