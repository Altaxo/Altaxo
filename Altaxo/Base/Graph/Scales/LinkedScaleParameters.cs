#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales
{
	public class LinkedScaleParameters
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		ICloneable
	{
		/// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
		private double _orgA;

		/// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
		private double _orgB;

		/// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
		private double _endA;

		/// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
		private double _endB;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedScaleParameters), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LinkedScaleParameters s = (LinkedScaleParameters)obj;

				info.AddValue("OrgA", s._orgA);
				info.AddValue("OrgB", s._orgB);
				info.AddValue("EndA", s._endA);
				info.AddValue("EndB", s._endB);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScaleParameters s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual LinkedScaleParameters SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LinkedScaleParameters s = null != o ? (LinkedScaleParameters)o : new LinkedScaleParameters();

				s._orgA = info.GetDouble("OrgA");
				s._orgB = info.GetDouble("OrgB");
				s._endA = info.GetDouble("EndA");
				s._endB = info.GetDouble("EndB");

				return s;
			}
		}

		#endregion Serialization

		public LinkedScaleParameters()
		{
			_orgA = 0;
			_orgB = 1;
			_endA = 0;
			_endB = 1;
		}

		public LinkedScaleParameters(LinkedScaleParameters from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(LinkedScaleParameters from)
		{
			if (object.ReferenceEquals(this, from))
				return;

			// this call has the advantage, that a change event is raised when the parameter really change
			SetTo(from._orgA, from._orgB, from._endA, from.EndB);
		}

		public object Clone()
		{
			return new LinkedScaleParameters(this);
		}

		public bool IsStraightLink
		{
			get
			{
				return OrgA == 0 && OrgB == 1 && EndA == 0 && EndB == 1;
			}
		}

		public void SetToStraightLink()
		{
			SetTo(0, 1, 0, 1);
		}

		public void SetTo(LinkedScaleParameters from)
		{
			SetTo(from.OrgA, from.OrgB, from.EndA, from.EndB);
		}

		/// <summary>
		/// Set all parameters of the axis link by once.
		/// </summary>
		/// <param name="orgA">The value a of x-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="orgB">The value b of x-axis link for link of axis origin: org' = a + b*org.</param>
		/// <param name="endA">The value a of x-axis link for link of axis end: end' = a + b*end.</param>
		/// <param name="endB">The value b of x-axis link for link of axis end: end' = a + b*end.</param>
		public void SetTo(double orgA, double orgB, double endA, double endB)
		{
			if (
				(orgA != this.OrgA) ||
				(orgB != this.OrgB) ||
				(endA != this.EndA) ||
				(endB != this.EndB))
			{
				this._orgA = orgA;
				this._orgB = orgB;
				this._endA = endA;
				this._endB = endB;

				EhSelfChanged(EventArgs.Empty);
			}
		}

		public double OrgA
		{
			get { return _orgA; }
			set
			{
				if (_orgA != value)
				{
					_orgA = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public double OrgB
		{
			get { return _orgB; }
			set
			{
				if (_orgB != value)
				{
					_orgB = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public double EndA
		{
			get { return _endA; }
			set
			{
				if (_endA != value)
				{
					_endA = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public double EndB
		{
			get { return _endB; }
			set
			{
				if (_endB != value)
				{
					_endB = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}
	}
}