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
	[Serializable]
	public abstract class ClosedPathShapeBase : GraphicBase
	{
		protected BrushX _fillBrush;
		protected PenX _linePen;

		#region Serialization

		#region Clipboard serialization

		protected ClosedPathShapeBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			SetObjectData(this, info, context, null);
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			ClosedPathShapeBase s = this;
			base.GetObjectData(info, context);

			info.AddValue("LinePen", s._linePen);
			info.AddValue("FillBrush", s._fillBrush);

		}
		public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
		{
			ClosedPathShapeBase s = (ClosedPathShapeBase)base.SetObjectData(obj, info, context, selector);

			s.Pen = (PenX)info.GetValue("LinePen", typeof(PenX));
			s.Brush = (BrushX)info.GetValue("FillBrush", typeof(BrushX));

			return s;
		} // end of SetObjectData

		public override void OnDeserialization(object obj)
		{
			base.OnDeserialization(obj);
		}

		#endregion


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ShapeGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.ShapeGraphic", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedPathShapeBase), 2)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ClosedPathShapeBase s = (ClosedPathShapeBase)obj;
				info.AddBaseValueEmbedded(s, typeof(ClosedPathShapeBase).BaseType);

				info.AddValue("LinePen", s._linePen);
				info.AddValue("Fill", s._fillBrush.IsVisible);
				info.AddValue("FillBrush", s._fillBrush);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				ClosedPathShapeBase s = (ClosedPathShapeBase)o;
				info.GetBaseValueEmbedded(s, typeof(ClosedPathShapeBase).BaseType, parent);


				s.Pen = (PenX)info.GetValue("LinePen", s);
				bool fill = info.GetBoolean("Fill");
				s.Brush = (BrushX)info.GetValue("FillBrush", s);
				return s;
			}
		}


		#endregion

		public ClosedPathShapeBase()
		{
			Brush = new BrushX(NamedColor.Transparent);
			Pen = new PenX(NamedColor.Black);
		}

		public ClosedPathShapeBase(PointD2D Position, PointD2D Size)
			: base(Position, Size)
		{
		}

		public ClosedPathShapeBase(ClosedPathShapeBase from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}
		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as ClosedPathShapeBase;
				if (null != from)
				{
					this._fillBrush = (BrushX)from._fillBrush.Clone();
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

		public virtual BrushX Brush
		{
			get
			{
				return _fillBrush;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("The fill brush must not be null");

				if (_fillBrush != null)
					_fillBrush.Changed -= this.EhChildChanged;



				_fillBrush = (BrushX)value.Clone();
				_fillBrush.Changed += this.EhChildChanged;
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

		protected static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
			((ClosedPathShapeBase)hitted).OnChanged();
			return true;
		}


	} //  End Class
} // end Namespace
