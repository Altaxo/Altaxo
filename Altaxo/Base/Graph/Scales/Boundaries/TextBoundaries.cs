using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Graph.Scales.Boundaries
{
  public class TextBoundaries : IPhysicalBoundaries
  {
    AltaxoSet<string> _itemList;

    [NonSerialized]
    protected int _eventSuspendCount;
    [NonSerialized]
    protected int _savedNumberOfItems;

    [field:NonSerialized]
    public event BoundaryChangedHandler BoundaryChanged;
    [field:NonSerialized]
    public event ItemNumberChangedHandler NumberOfItemsChanged;

    public TextBoundaries()
    {
      _itemList = new AltaxoSet<string>();
    }

    public TextBoundaries(TextBoundaries from)
    {
      _itemList = new AltaxoSet<string>();
      BeginUpdate();
      foreach (string s in from._itemList)
        _itemList.Add(s);
      EndUpdate();
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

    public  void BeginUpdate()
    {
      ++_eventSuspendCount;
      if (_eventSuspendCount == 1) // events are freshly disabled
      {
        this._savedNumberOfItems = this._itemList.Count;
      }
    }

    public  void EndUpdate()
    {
      --_eventSuspendCount;
      if (_eventSuspendCount == 0)
      {
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
    public  bool Add(Altaxo.Data.IReadableColumn col, int idx)
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
    public  bool Add(Altaxo.Data.AltaxoVariant item)
    {
      if(item.IsType(Altaxo.Data.AltaxoVariant.Content.VString))
      {
        string s = item.ToString();
        if (!_itemList.Contains(s))
        {
          _itemList.Add(s);

          if (_eventSuspendCount == 0)
          {
            OnNumberOfItemsChanged();
            OnBoundaryChanged(false, true);
          }

          return true;
        }
      }
      return false;
    }

    public  void Add(IPhysicalBoundaries b)
    {
      if (b is TextBoundaries)
      {
        this.BeginUpdate();
        TextBoundaries from = (TextBoundaries)b;
        foreach (string s in from._itemList)
        {
          if (!_itemList.Contains(s))
            _itemList.Add(s);
        }
        this.EndUpdate();
      }
    }

    public object Clone()
    {
      return new TextBoundaries(this);
    }

    #endregion

    #region IPhysicalBoundaries Members

   

    public bool EventsEnabled
    {
      get
      {
        return _eventSuspendCount == 0;
      }
    }

    public void Reset()
    {
      _itemList.Clear();
    }

    public int NumberOfItems
    {
      get 
      {
        return _itemList.Count;
      }
    }

    public bool IsEmpty
    {
      get 
      {
        return _itemList.Count == 0;
      }
    }

    #endregion

    protected virtual void OnBoundaryChanged(bool bLowerBoundChanged, bool bUpperBoundChanged)
    {
      if (null != BoundaryChanged)
        BoundaryChanged(this, new BoundariesChangedEventArgs(bLowerBoundChanged, bUpperBoundChanged));
    }

    protected virtual void OnNumberOfItemsChanged()
    {
      if (null != NumberOfItemsChanged)
        NumberOfItemsChanged(this, new System.EventArgs());
    }

  }
}
