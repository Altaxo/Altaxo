using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Rescaling
{
	public class AngularRescaleConditions : ICloneable, Altaxo.Main.IChangedEventSource
	{
		public event EventHandler Changed;

		/// <summary>Origin of the scale in degrees.</summary>
		protected int _scaleOrigin; 

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularRescaleConditions), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AngularRescaleConditions s = (AngularRescaleConditions)obj;

				info.AddValue("ScaleOrigin", s._scaleOrigin);

			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularRescaleConditions s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual AngularRescaleConditions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularRescaleConditions s = null != o ? (AngularRescaleConditions)o : new AngularRescaleConditions();

				s._scaleOrigin = info.GetInt32("ScaleOrigin");

				return s;
			}
		}
		#endregion

		public AngularRescaleConditions()
		{
		}

		public AngularRescaleConditions(AngularRescaleConditions from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(AngularRescaleConditions from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			this._scaleOrigin = from._scaleOrigin;
		}



		public int ScaleOrigin
		{
			get
			{
				return _scaleOrigin;
			}
			set
			{
				_scaleOrigin = value;
			}
		}


	

		#region ICloneable Members

		public object Clone()
		{
			return new AngularRescaleConditions(this);
		}

		#endregion
	}
}
