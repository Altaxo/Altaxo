using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class ItemLocationDirectAutoSize : ItemLocationDirect, ICloneable
	{
		#region Serialization

		/// <summary>
		/// 2013-11-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirectAutoSize), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(ItemLocationDirectAutoSize).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationDirectAutoSize)o : new ItemLocationDirectAutoSize();
				info.GetBaseValueEmbedded(s, typeof(ItemLocationDirectAutoSize).BaseType, parent);
				return s;
			}
		}

		#endregion Serialization

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