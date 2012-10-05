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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>
	/// Base class for all open (not closed) shapes, like line, curly brace etc.
	/// </summary>
	[Serializable]
	public abstract class OpenPathShapeBase : GraphicBase
	{
		/// <summary>If not null, this pens draw the outline of the shape.</summary>
		protected PenX _outlinePen;

		/// <summary>Pen to draw the shape.</summary>
		protected PenX _linePen;

		#region Serialization

		#region Clipboard serialization

		protected OpenPathShapeBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			OpenPathShapeBase s = this;
			base.GetObjectData(info, context);

			info.AddValue("LinePen", s._linePen);
			info.AddValue("OutlinePen", s._outlinePen);

		}
		public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			OpenPathShapeBase s = (OpenPathShapeBase)base.SetObjectData(obj, info, context, selector);

			s.Pen = (PenX)info.GetValue("LinePen", typeof(PenX));
			s.OutlinePen = (PenX)info.GetValue("OutlinePen", typeof(PenX));

			return s;
		} // end of SetObjectData

		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}

		#endregion


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenPathShapeBase), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				OpenPathShapeBase s = (OpenPathShapeBase)obj;
				info.AddBaseValueEmbedded(s, typeof(OpenPathShapeBase).BaseType);

				info.AddValue("LinePen", s._linePen);
				info.AddValue("OutlinePen", s._outlinePen);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				OpenPathShapeBase s = (OpenPathShapeBase)o;
				info.GetBaseValueEmbedded(s, typeof(OpenPathShapeBase).BaseType, parent);


				s.Pen = (PenX)info.GetValue("LinePen", s);
				s.OutlinePen = (PenX)info.GetValue("OutlinePen", s);
				return s;
			}
		}


		#endregion

		public OpenPathShapeBase()
		{
			Pen = new PenX(NamedColors.Black);
		}

		public OpenPathShapeBase(PointD2D Position, PointD2D Size)
			: base(Position, Size)
		{
			Pen = new PenX(NamedColors.Black);
		}

		public OpenPathShapeBase(OpenPathShapeBase from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}
		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as OpenPathShapeBase;
				if (from != null)
				{
					this._outlinePen = null == from._outlinePen ? null : (PenX)from._outlinePen.Clone();
					this._linePen = (PenX)from._linePen.Clone();
				}
			}
			return isCopied;
		}

		public virtual PenX Pen
		{
			get
			{
				return _linePen;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("The line pen must not be null");

				if (_linePen != null)
					_linePen.Changed -= this.EhChildChanged;


				_linePen = (PenX)value.Clone();
				_linePen.Changed += this.EhChildChanged;
				OnChanged();

			}
		}

		public virtual PenX OutlinePen
		{
			get
			{
				return _outlinePen;
			}
			set
			{
				if (_outlinePen != null)
					_outlinePen.Changed -= this.EhChildChanged;

				_outlinePen = null == value ? null : (PenX)value.Clone();

				if (_outlinePen != null)
					_outlinePen.Changed += this.EhChildChanged;

				OnChanged();
			}
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			IHitTestObject result = base.HitTest(htd);
			if (result != null)
				result.DoubleClick = EhHitDoubleClick;
			return result;
		}

		public override IHitTestObject HitTest(HitTestRectangularData rect)
		{
			IHitTestObject result = base.HitTest(rect);
			if (result != null)
				result.DoubleClick = EhHitDoubleClick;
			return result;
		}

		static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
			((OpenPathShapeBase)hitted).OnChanged();
			return true;
		}


	} //  End Class
} // end Namespace
