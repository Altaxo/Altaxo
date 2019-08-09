#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Scripting
{
  public class FitFunctionScriptCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ICollection<FitFunctionScript>
  {
    /// <summary>
    /// The inner list of fit function scripts. Keys and values are the <b>same instance</b> of a fit function script.
    /// I use a dictionary in order to be able to retrieve a previous version of a fit function script with exactly
    /// the same properties (ScriptText, Category and Name).
    /// </summary>
    private Dictionary<FitFunctionScript, FitFunctionScript> _innerList = new Dictionary<FitFunctionScript, FitFunctionScript>();

    public FitFunctionScriptCollection(Main.IDocumentNode parent)
    {
      _parent = parent;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var entry in _innerList.Keys)
      {
        var asNode = entry as Main.IDocumentLeafNode;
        if (null != asNode)
          yield return new Main.DocumentNodeAndName(asNode, entry.CreationTime.ToUniversalTime().ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }

    protected override void Dispose(bool isDisposing)
    {
      var list = _innerList;
      if (null != list && list.Count > 0)
      {
        _innerList = new Dictionary<FitFunctionScript, FitFunctionScript>();
        foreach (var item in list.Keys)
          item.Dispose();
      }

      base.Dispose(isDisposing);
    }

    public List<FitFunctionScript> Find(string category, string name)
    {
      var result = new List<FitFunctionScript>();

      foreach (FitFunctionScript script in this)
      {
        if (name != null && script.FitFunctionName != name)
          continue;
        if (category != null && script.FitFunctionCategory != category)
          continue;
        result.Add(script);
      }

      result.Sort(FitFunctionScriptDateTimeComparer);

      return result;
    }

    private int FitFunctionScriptDateTimeComparer(FitFunctionScript x, FitFunctionScript y)
    {
      return DateTime.Compare(x.CreationTime, y.CreationTime);
    }

    /// <summary>
    /// Adds the specified fit function script.
    /// </summary>
    /// <param name="script">The script to add.</param>
    /// <returns>If this script is new (there is no existing script with the same properties ScriptText, Category and Name), the newly added script is returned.
    /// Otherwise, the existing script with the same properties is returned.</returns>
    public FitFunctionScript Add(FitFunctionScript script)
    {
      if (null == script)
        throw new ArgumentNullException();

      if (!_innerList.TryGetValue(script, out var returnValue))
      {
        _innerList.Add(script, script);
        script.ParentObject = this;
        returnValue = script;
      }
      return returnValue;
    }

    void ICollection<FitFunctionScript>.Add(FitFunctionScript script)
    {
      Add(script);
    }

    public bool Contains(FitFunctionScript script)
    {
      return _innerList.ContainsKey(script);
    }

    public void Remove(FitFunctionScript script)
    {
      if (_innerList.ContainsKey(script))
      {
        _innerList.Remove(script);
        script.Dispose();
      }
    }

    #region ICollection Members

    public void CopyTo(FitFunctionScript[] array, int index)
    {
      _innerList.Keys.CopyTo(array, index);
    }

    public int Count
    {
      get { return _innerList.Count; }
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    private object _syncRoot = new object();

    public object SyncRoot
    {
      get { return _syncRoot; }
    }

    #endregion ICollection Members

    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
      return _innerList.Keys.GetEnumerator();
    }

    public void Clear()
    {
      var list = _innerList;
      if (null != list && list.Count > 0)
      {
        _innerList = new Dictionary<FitFunctionScript, FitFunctionScript>();
        foreach (var item in list.Keys)
          item.Dispose();
      }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    bool ICollection<FitFunctionScript>.Remove(FitFunctionScript item)
    {
      var success = _innerList.Remove(item);
      if (success)
        item.Dispose();
      return success;
    }

    IEnumerator<FitFunctionScript> IEnumerable<FitFunctionScript>.GetEnumerator()
    {
      return _innerList.Keys.GetEnumerator();
    }

    #endregion IEnumerable Members
  }
}
