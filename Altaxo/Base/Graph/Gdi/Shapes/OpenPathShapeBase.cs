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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

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

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenPathShapeBase), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

		#endregion Serialization

		protected OpenPathShapeBase(ItemLocationDirect location, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
			: base(location)
		{
		}

		protected OpenPathShapeBase(ItemLocationDirect location, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: base(location)
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			var penWidth = GraphDocument.GetDefaultPenWidth(context);
			var foreColor = context.GetValue(GraphDocument.PropertyKeyDefaultForeColor);
			Pen = new PenX(foreColor, penWidth);
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

		private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
		{
			if (null != _linePen)
				yield return new Main.DocumentNodeAndName(_linePen, "LinePen");

			if (null != _outlinePen)
				yield return new Main.DocumentNodeAndName(_outlinePen, "OutlinePen");
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
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
					_linePen.ParentObject = null;

				_linePen = (PenX)value.Clone();
				_linePen.ParentObject = this;
				EhSelfChanged(EventArgs.Empty);
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
					_outlinePen.ParentObject = null;

				_outlinePen = null == value ? null : (PenX)value.Clone();

				if (_outlinePen != null)
					_outlinePen.ParentObject = this;

				EhSelfChanged(EventArgs.Empty);
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

		private static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
			((OpenPathShapeBase)hitted).EhSelfChanged(EventArgs.Empty);
			return true;
		}
	} //  End Class
} // end Namespace