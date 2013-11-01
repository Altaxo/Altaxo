using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class ItemLocationDirectAutoSize : ItemLocationDirect, ICloneable
	{
		#region Construction and copying

		public ItemLocationDirectAutoSize()
		{
		}

		public ItemLocationDirectAutoSize(ItemLocationDirect from)
		{
			CopyFrom(from);
		}

		public ItemLocationDirectAutoSize(IItemLocation from)
		{
			CopyFrom(from);
		}

		object System.ICloneable.Clone()
		{
			return new ItemLocationDirectAutoSize(this);
		}

		public override ItemLocationDirect Clone()
		{
			return new ItemLocationDirectAutoSize(this);
		}

		#endregion Construction and copying

		public override bool IsAutoSized
		{
			get
			{
				return true;
			}
		}
	}
}