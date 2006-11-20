using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Graph.Scales.Boundaries
{
  public class TextBoundaries : AbstractPhysicalBoundaries
  {
    AltaxoSet<string> _itemList = new AltaxoSet<string>();
    
    [NonSerialized]
    int _savedNumberOfItems;
    [NonSerialized]
    int _eventSuspendCount;

    void CopyFrom(TextBoundaries from)
    {
      _itemList.Clear();
      foreach (string s in from._itemList)
        _itemList.Add(s);
    }

    /// <summary>
    /// Try to find the text item and returns the index in the collection. If the 
    /// item is not found, the function returns -1.
    /// </summary>
    /// <param name="item">The text item to find.</param>
    /// <returns>The ordinal number  or double.NaN if the item is not found.</returns>
    public int IndexOf(string item)
    {
      return _itemList.IndexOf(item);
    }

    public string GetItem(int i)
    {
      return _itemList[i];
    }

    #region AbstractPhysicalBoundaries implementation

    public override void BeginUpdate()
    {
      ++_eventSuspendCount;
      if (_eventSuspendCount == 1) // events are freshly disabled
      {
        this._savedNumberOfItems = this._itemList.Count;
      }
    }

    public override void EndUpdate()
    {
      if (_eventSuspendCount > 0)
      {
        --_eventSuspendCount;
        // if anything changed in the meantime, fire the event
        if (this._savedNumberOfItems != this._itemList.Count)
        {
          OnNumberOfItemsChanged();
          OnBoundaryChanged(false, true); // bLower, bUpper);
        }
      }
    }

    /// <summary>
    /// Processes a single value from a data column.
    /// If the data value is text, the boundaries are
    /// updated and the number of items is increased by one (if not contained already). The function returns true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="col">The data column</param>
    /// <param name="idx">The index into this data column where the data value is located.</param>
    /// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
    public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
    {
      return Add(col[idx]);
    }

    /// <summary>
    /// Processes a single value .
    /// If the data value is text, the boundaries are
    /// updated and the number of items is increased by one (if not contained already). The function returns true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
    public override bool Add(Altaxo.Data.AltaxoVariant item)
    {
      if(item.IsType(Altaxo.Data.AltaxoVariant.Content.VString))
      {
        string s = item.ToString();
        if (!_itemList.Contains(s))
        {
          _itemList.Add(s);
          return true;
        }
      }
      return false;
    }

    public override void Add(IPhysicalBoundaries b)
    {
      if (b is TextBoundaries)
      {
        TextBoundaries from = (TextBoundaries)b;
        foreach (string s in from._itemList)
        {
          if (!_itemList.Contains(s))
            _itemList.Add(s);
        }
      }
    }

    public override object Clone()
    {
      TextBoundaries result = new TextBoundaries();
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }
}
