using System;

namespace Altaxo.Graph.Axes.Boundaries
{
	/// <summary>
	/// Summary description for AbstractBoundaries.
	/// </summary>
	public abstract class AbstractPhysicalBoundaries : IPhysicalBoundaries
	{

    protected int numberOfItems=0;
 
  
    protected int m_EventsSuspendCount=0;
    protected int m_SavedNumberOfItems; // stores the number of items when events are disabled
 
  
		
    public AbstractPhysicalBoundaries()
		{
			numberOfItems = 0;
    }
    public AbstractPhysicalBoundaries(AbstractPhysicalBoundaries from)
    {
      numberOfItems = from.numberOfItems;
    }


    #region IPhysicalBoundaries Members

    public event BoundaryChangedHandler   BoundaryChanged;
    public event ItemNumberChangedHandler NumberOfItemsChanged;

    public bool EventsEnabled
    {
      get
      { 
        return m_EventsSuspendCount<=0;
      }
    }

    public abstract void BeginUpdate();
    

    public abstract void EndUpdate();
   

    /// <summary>
    /// Processes a single value from a numeric column <paramref name="col"/>[<paramref name="idx"/>].
    /// If the data value is inside the considered value range, the boundaries are
    /// updated and the number of items is increased by one. The function has to return true
    /// in this case. On the other hand, if the value is outside the range, the function has to
    /// return false.
    /// </summary>
    /// <param name="col">The numeric data column</param>
    /// <param name="idx">The index into this numeric column where the data value is located</param>
    /// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
    public abstract bool Add(Altaxo.Data.IReadableColumn col, int idx);
    

    public virtual void Reset()
    {
      numberOfItems = 0;
    }

    public int NumberOfItems
    {
       get 
       { 
         return numberOfItems;
       } 
    }

    public virtual bool IsEmpty 
    { 
      get
      {
        return numberOfItems==0;
      } 
    }

    public abstract void Add(IPhysicalBoundaries b);
    

    #endregion

    #region ICloneable Members

    public abstract object Clone();


    protected void OnBoundaryChanged(bool bLowerBoundChanged, bool bUpperBoundChanged)
    {
      if(null!=BoundaryChanged)
        BoundaryChanged(this, new BoundariesChangedEventArgs(bLowerBoundChanged,bUpperBoundChanged));
    }

    protected void OnNumberOfItemsChanged()
    {
      if(null!=NumberOfItemsChanged)
        NumberOfItemsChanged(this, new System.EventArgs());
    }

    #endregion
  }
}
