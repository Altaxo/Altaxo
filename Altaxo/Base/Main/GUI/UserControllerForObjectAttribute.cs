using System;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Can be used for a controller to denote which type can be controlled by this.
	/// </summary>
	public class UserControllerForObjectAttribute : System.Attribute, IComparable, IClassForClassAttribute
	{
    System.Type _type;
    int         _priority = 100;
		public UserControllerForObjectAttribute(System.Type type)
		{
			_type = type;
		}
    public UserControllerForObjectAttribute(System.Type type, int priority)
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
      UserControllerForObjectAttribute to = (UserControllerForObjectAttribute)obj;
      return this._priority==to._priority ? 0 : (this._priority>to._priority ? 1 : -1);
    }

    #endregion
  }
}
