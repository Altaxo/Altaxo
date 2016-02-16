#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Geometry;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Graph3D.Shapes
{
	[Serializable]
	public abstract class ImageGraphic : GraphicBase
	{
		/// <summary>
		/// If true, the size of this object is calculated based on the source size, taking into account the scaling for x and y.
		/// If false, the size of this object is used, and the scaling values will be ignored.
		/// </summary>
		private bool _isSizeCalculationBasedOnSourceSize;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ImageGraphic", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.ImageGraphic", 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Try to serialize old version");
				/*
				ImageGraphic s = (ImageGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(ImageGraphic).BaseType);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ImageGraphic s = (ImageGraphic)o;
				info.GetBaseValueEmbedded(s, typeof(ImageGraphic).BaseType, parent);

				s._isSizeCalculationBasedOnSourceSize = false;
				var aspectPreserving = AspectRatioPreservingMode.None;
				((ItemLocationDirectAspectPreserving)s._location).AspectRatioPreserving = aspectPreserving;

				return s;
			}
		}

		// 2012-03-21: Properties 'SizeBasedOnSourceSize' and 'AspectPreserving' added
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.ImageGraphic", 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				ImageGraphic s = (ImageGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(ImageGraphic).BaseType);

				info.AddValue("SizeBasedOnSourceSize", s._isSizeCalculationBasedOnSourceSize);
				info.AddEnum("AspectPreserving", s._aspectPreserving);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ImageGraphic s = (ImageGraphic)o;
				info.GetBaseValueEmbedded(s, typeof(ImageGraphic).BaseType, parent);

				s._isSizeCalculationBasedOnSourceSize = info.GetBoolean("SizeBasedOnSourceSize");
				var aspectPreserving = (AspectRatioPreservingMode)info.GetEnum("AspectPreserving", typeof(AspectRatioPreservingMode));
				((ItemLocationDirectAspectPreserving)s._location).AspectRatioPreserving = aspectPreserving;

				return s;
			}
		}

		/// <summary>
		/// 2013-12-12: Properties 'AspectPreserving' removed, because now it is part of ItemLocationDirectAspectPreserving
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImageGraphic), 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ImageGraphic s = (ImageGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(ImageGraphic).BaseType);

				info.AddValue("SizeBasedOnSourceSize", s._isSizeCalculationBasedOnSourceSize);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ImageGraphic s = (ImageGraphic)o;
				info.GetBaseValueEmbedded(s, typeof(ImageGraphic).BaseType, parent);

				s._isSizeCalculationBasedOnSourceSize = info.GetBoolean("SizeBasedOnSourceSize");

				return s;
			}
		}

		#endregion Serialization

		protected ImageGraphic()
			:
			base(new ItemLocationDirectAspectPreserving())
		{
			_location = new ItemLocationDirectAspectPreserving() { ParentObject = this };
		}

		protected ImageGraphic(ImageGraphic from)
			:
			base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as ImageGraphic;
				if (from != null)
				{
					this._isSizeCalculationBasedOnSourceSize = from._isSizeCalculationBasedOnSourceSize;
				}
			}
			return isCopied;
		}

		public override bool AutoSize
		{
			get
			{
				return false;
			}
		}

		public bool IsSizeCalculationBasedOnSourceSize
		{
			get
			{
				return _isSizeCalculationBasedOnSourceSize;
			}
			set
			{
				// this property is for use in the controller only, it has no relevance for the item itself
				_isSizeCalculationBasedOnSourceSize = value;
			}
		}

		public AspectRatioPreservingMode AspectRatioPreserving
		{
			get
			{
				return ((ItemLocationDirectAspectPreserving)_location).AspectRatioPreserving;
			}
			set
			{
				((ItemLocationDirectAspectPreserving)_location).AspectRatioPreserving = value;
			}
		}

		/// <summary>Get the size of the original image in points (1/72 inch).</summary>
		public abstract PointD2D GetImageSizePt();

		public abstract Image GetImage();

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			IHitTestObject result = base.HitTest(htd);
			if (result != null)
				result.DoubleClick = EhHitDoubleClick;
			return result;
		}

		private static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Image properties", true);
			((ImageGraphic)hitted).EhSelfChanged(EventArgs.Empty);
			return true;
		}

		#region HitTestObject

		/// <summary>Creates a new hit test object. Here, a special hit test object is constructed, which suppresses the scale grips.</summary>
		/// <returns>A newly created hit test object.</returns>
		protected override IHitTestObject GetNewHitTestObject()
		{
			return new MyHitTestObject(this);
		}

		private class MyHitTestObject : GraphicBaseHitTestObject
		{
			public MyHitTestObject(ImageGraphic obj)
				: base(obj)
			{
			}

			public override IGripManipulationHandle[] GetGrips(int gripLevel)
			{
				switch (gripLevel)
				{
					case 0:
						return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move);

					case 1:
						return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move | GripKind.Resize);

					case 2:
						return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move | GripKind.Rotate);

					case 3:
						return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move | GripKind.Shear);
				}
				return null;
			}
		}

		#endregion HitTestObject
	} //  End Class
}