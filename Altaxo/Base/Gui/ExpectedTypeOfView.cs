using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui
{
  /// <summary>
  /// Can be used for a controller to denote which type can be controlled by this.
  /// </summary>
  public class ExpectedTypeOfViewAttribute : System.Attribute, IComparable, IClassForClassAttribute
  {
    System.Type _type;
    int _priority = 0;
    public ExpectedTypeOfViewAttribute(System.Type type)
    {
      _type = type;
    }
    public ExpectedTypeOfViewAttribute(System.Type type, int priority)
    {
      _type = type;
      _priority = priority;
    }

    public System.Type TargetType
    {
      get { return _type; }
    }

    public int Priority
    {
      get { return _priority; }
    }
    #region IComparable Members

    public int CompareTo(object obj)
    {
      ExpectedTypeOfViewAttribute to = (ExpectedTypeOfViewAttribute)obj;
      return this._priority == to._priority ? 0 : (this._priority > to._priority ? 1 : -1);
    }

    #endregion
  }
}
