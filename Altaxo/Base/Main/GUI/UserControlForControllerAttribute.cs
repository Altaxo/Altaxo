using System;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Can be used for a control to denote which type of controller can control this.
	/// </summary>
	public class UserControlForControllerAttribute : System.Attribute, IComparable, IClassForClassAttribute
	{
    System.Type _type;
    int         _priority = 100;
		public UserControlForControllerAttribute(System.Type type)
		{
			_type = type;
		}
    public UserControlForControllerAttribute(System.Type type, int priority)
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
      UserControlForControllerAttribute to = (UserControlForControllerAttribute)obj;
      return this._priority==to._priority ? 0 : (this._priority>to._priority ? 1 : -1);
    }

  
    #endregion

   
  }
}
