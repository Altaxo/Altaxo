using System;

namespace Altaxo.TableView
{
	/// <summary>
	/// Summary description for IndexSelection.
	/// </summary>
	public class IndexSelection : System.Collections.SortedList 
	{
		protected int lastSelectedIndex=0;
		
		public IndexSelection()
		{
		}

		public int LastSelection
		{
			get
			{
				return this.Count>0 ? lastSelectedIndex : 0;
			}
		}

		public int this[int i]
		{
		get { return (int)base.GetKey(i); }
		}

		public bool IsSelected(int nIndex)
		{
			return this.ContainsKey(nIndex);
		}

		public void Select(int nIndex, bool bShiftKey, bool bControlKey)
		{
			if(bControlKey) // Control pressed
			{
				if(this.ContainsKey(nIndex))
					this.Remove(nIndex);
				else
					this.Add(nIndex,null);
			}
			else if(bShiftKey)
			{
				// deselect all and then select from lastSelectedColumn to this
				// this is the behaviour of the windows explorer
				if(0==this.Count)
					lastSelectedIndex=0;
				else
					this.Clear();
				int step = lastSelectedIndex<=nIndex ? 1 : -1;
				for(int i=lastSelectedIndex;i<=nIndex;i+=step)
					this.Add(i,null);
			}
			else // no modifier key 
			{
				this.Clear();
				this.Add(nIndex,null);
			}
			lastSelectedIndex = nIndex;
		}
	}
}
