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

using Altaxo.Graph.Plot.Groups;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
	/// <summary>
	/// Extends the <see cref="PlotGroupStyleCollectionBase"/> with a coordinate transforming style.
	/// </summary>
	public class PlotGroupStyleCollection
		:
		PlotGroupStyleCollectionBase,
		ICloneable // is already implemented in base but is hidden because of inheritance
	{
		private ICoordinateTransformingGroupStyle _coordinateTransformingStyle;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotGroupStyleCollection), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				PlotGroupStyleCollection s = (PlotGroupStyleCollection)obj;
				info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);

				info.AddValue("TransformingStyle", s._coordinateTransformingStyle);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				PlotGroupStyleCollection s = null != o ? (PlotGroupStyleCollection)o : new PlotGroupStyleCollection();

				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

				s._coordinateTransformingStyle = (ICoordinateTransformingGroupStyle)info.GetValue("TransformingStyle", s);

				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public PlotGroupStyleCollection()
		{
		}

		public PlotGroupStyleCollection(PlotGroupStyleCollection from)
		{
			CopyFrom(from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as PlotGroupStyleCollection;

			if (null != from)
			{
				using (var suspendToken = SuspendGetToken())
				{
					base.CopyFrom(from);
					this.CoordinateTransformingStyle = null == from._coordinateTransformingStyle ? null : (ICoordinateTransformingGroupStyle)from._coordinateTransformingStyle.Clone();

					suspendToken.Resume();
				}
				return true;
			}
			else
			{
				return base.CopyFrom(obj);
			}
		}

		#endregion Constructors

		#region ICloneable Members

		public new PlotGroupStyleCollection Clone()
		{
			return new PlotGroupStyleCollection(this);
		}

		object ICloneable.Clone()
		{
			return new PlotGroupStyleCollection(this);
		}

		#endregion ICloneable Members

		public override void Clear()
		{
			_coordinateTransformingStyle = null;
			base.Clear();
		}

		/// <summary>
		/// Gets/sets the coordinate transforming style.
		/// </summary>
		public ICoordinateTransformingGroupStyle CoordinateTransformingStyle
		{
			get
			{
				return _coordinateTransformingStyle;
			}
			set
			{
				if (ChildSetMember(ref _coordinateTransformingStyle, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}
	}
}