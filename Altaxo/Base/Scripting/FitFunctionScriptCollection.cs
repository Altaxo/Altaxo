using System;
using System.Collections;
using System.Text;

namespace Altaxo.Scripting
{

  public class FitFunctionScriptCollection : System.Collections.ICollection
  {
    Hashtable _InnerList = new Hashtable();

    public void Add(FitFunctionScript script)
    {
      if (!Contains(script))
        _InnerList.Add(script,null);
    }

    public bool Contains(FitFunctionScript script)
    {
      return _InnerList.Contains(script);
    }

  
    #region ICollection Members

    public void CopyTo(Array array, int index)
    {
      _InnerList.Keys.CopyTo(array,index);
    }

    public int Count
    {
      get { return _InnerList.Count; }
    }

    public bool IsSynchronized
    {
      get { return _InnerList.IsSynchronized; }
    }

    public object SyncRoot
    {
      get { return _InnerList.SyncRoot; }
    }

    #endregion

    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
      return _InnerList.Keys.GetEnumerator();
    }

    #endregion
}
}
