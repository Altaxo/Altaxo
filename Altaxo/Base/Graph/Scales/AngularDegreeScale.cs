using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales
{
	public class AngularDegreeScale : AngularScale
	{

		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularDegreeScale), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(AngularScale));
				AngularDegreeScale s = (AngularDegreeScale)obj;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularDegreeScale s = SDeserialize(o, info, parent);
				OnAfterDeserialization(s);
				return s;
			}

			protected virtual void OnAfterDeserialization(AngularDegreeScale s)
			{
				
			}

			protected virtual AngularDegreeScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularDegreeScale s = null!=o ? (AngularDegreeScale)o : new AngularDegreeScale();
				info.GetBaseValueEmbedded(s, typeof(AngularScale), s);
				return s;
			}
		}
		#endregion


		public AngularDegreeScale()
		{
		}

		public AngularDegreeScale(AngularDegreeScale from)
			: base(from)
		{
		}

		public override object Clone()
		{
			return new AngularDegreeScale();
		}

		protected override bool UseDegree
		{
			get { return true; }
		}
	}

}
