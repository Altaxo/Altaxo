using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales
{
	public class AngularRadianScale : AngularScale
	{

		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularRadianScale), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(AngularScale));
				AngularRadianScale s = (AngularRadianScale)obj;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularRadianScale s = SDeserialize(o, info, parent);
				OnAfterDeserialization(s);
				return s;
			}

			protected virtual void OnAfterDeserialization(AngularRadianScale s)
			{

			}

			protected virtual AngularRadianScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularRadianScale s = null != o ? (AngularRadianScale)o : new AngularRadianScale();
				info.GetBaseValueEmbedded(s, typeof(AngularScale), s);
				return s;
			}
		}
		#endregion


		public AngularRadianScale()
		{
		}

		public AngularRadianScale(AngularRadianScale from)
			: base(from)
		{
		}

		public override object Clone()
		{
			return new AngularRadianScale();
		}

		protected override bool UseDegree
		{
			get { return false; }
		}
	}
}
