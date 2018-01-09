using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main
{
	/// <summary>
	/// Base class for project items that can be accessed by index.
	/// </summary>
	/// <typeparam name="TItem">The type of the project item.</typeparam>
	/// <seealso cref="Altaxo.Main.ProjectItemCollectionBase{TItem}" />
	public abstract class ProjectItemListBase<TItem> : ProjectItemCollectionBase<TItem> where TItem : IProjectItem
	{
		protected List<string> _nameList = new List<string>();

		public ProjectItemListBase(IDocumentNode parent)
				: base(parent)
		{
		}

		protected override void InternalAdd(TItem item)
		{
			base.InternalAdd(item);
			_nameList.Add(item.Name);
		}

		protected override bool InternalRemove(TItem item)
		{
			var success = base.InternalRemove(item);
			if (success && !_nameList.Remove(item.Name))
				throw new InvalidProgramException("Item was removed successfully, but not found in name list.");
			return success;
		}

		protected override void InternalClear()
		{
			_nameList.Clear();
			base.Clear();
		}

		public virtual TItem this[int idx]
		{
			get
			{
				return this[_nameList[idx]];
			}
		}

		public int IndexOf(TItem item)
		{
			return IndexOf(item.Name);
		}

		public int IndexOf(string itemName)
		{
			for (int i = _nameList.Count - 1; i >= 0; --i)
			{
				if (_nameList[i] == itemName)
					return i;
			}
			return -1;
		}

		protected override void InternalExchange(TItem oldItem, TItem newItem)
		{
			base.InternalExchange(oldItem, newItem);
			_nameList[IndexOf(oldItem)] = newItem.Name;
		}
	}
}