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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Altaxo.Graph.Gdi.Shapes
{
	[Serializable]
	public abstract class ImageGraphic : GraphicBase
	{
		/// <summary>
		/// If true, the size of this object is calculated based on the source size, taking into account the scaling for x and y.
		/// If false, the size of this object is used, and the scaling values will be ignored.
		/// </summary>
		private bool _isSizeCalculationBasedOnSourceSize;

		/// <summary>
		/// Indicates the aspect preserving of this object.
		/// </summary>
		private AspectRatioPreservingMode _aspectPreserving;

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
				s._aspectPreserving = AspectRatioPreservingMode.None;

				return s;
			}
		}

		// 2012-03-21: Properties 'SizeBasedOnSourceSize' and 'AspectPreserving' added
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImageGraphic), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ImageGraphic s = (ImageGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(ImageGraphic).BaseType);

				info.AddValue("SizeBasedOnSourceSize", s._isSizeCalculationBasedOnSourceSize);
				info.AddEnum("AspectPreserving", s._aspectPreserving);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ImageGraphic s = (ImageGraphic)o;
				info.GetBaseValueEmbedded(s, typeof(ImageGraphic).BaseType, parent);

				s._isSizeCalculationBasedOnSourceSize = info.GetBoolean("SizeBasedOnSourceSize");
				s._aspectPreserving = (AspectRatioPreservingMode)info.GetEnum("AspectPreserving", typeof(AspectRatioPreservingMode));

				return s;
			}
		}

		#endregion Serialization

		protected ImageGraphic()
			:
			base()
		{
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
					this._aspectPreserving = from._aspectPreserving;
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
				return _aspectPreserving;
			}
			set
			{
				var oldValue = _aspectPreserving;
				_aspectPreserving = value;
				if (value != oldValue)
				{
					ClampSizeAndScaleDueToAspectRatioKeeping();
				}
			}
		}

		public void ClampSizeAndScaleDueToAspectRatioKeeping()
		{
			PointD2D sourceSize = GetImageSizePt();

			PointD2D destSize = Size;
			PointD2D currentSize = new PointD2D(destSize.X * ScaleX, destSize.Y * ScaleY);
			switch (_aspectPreserving)
			{
				case AspectRatioPreservingMode.PreserveXPriority:
					// calculate y-scale based on x-scale
					destSize.Y = (currentSize.X * sourceSize.Y / sourceSize.X) / ScaleY;
					break;

				case AspectRatioPreservingMode.PreserveYPriority:
					destSize.X = (currentSize.Y * sourceSize.X / sourceSize.Y) / ScaleX;
					break;
			}
			this.Size = destSize;
		}

		public void NormalizeToScaleOne()
		{
			PointD2D size = Size;
			size.X *= ScaleX;
			size.Y *= ScaleY;
			base.Scale = new PointD2D(1, 1);
			this.Size = size;
		}

		protected override void SetSize(double width, double height, bool suppressChangeEvent)
		{
			if (_aspectPreserving == AspectRatioPreservingMode.PreserveXPriority)
			{
				var srcSize = GetImageSizePt();
				height = width * srcSize.Y / srcSize.X;
			}
			else if (_aspectPreserving == AspectRatioPreservingMode.PreserveYPriority)
			{
				var srcSize = GetImageSizePt();
				width = height * srcSize.X / srcSize.Y;
			}

			base.Scale = new PointD2D(1, 1);
			base.SetSize(width, height, suppressChangeEvent);
		}

		public override PointD2D Scale
		{
			get
			{
				return base.Scale;
			}
			set
			{
				var scale11 = new PointD2D(1, 1);
				var oldScale = base.Scale; // should always be one, because when setting the scale, as a result the size changed
				if (oldScale == scale11)
				{
					if (value == oldScale)
						return;
				}
				else // oops..., the old scale was not (1,1)!
				{
					base.Scale = scale11; // then set it now to (1,1)
					value = value * oldScale; // and multiply it with the now intended value for the scale
				}

				// completely ignore this, Scale should always be one.
				var w = Size.X * value.X;
				var h = Size.Y * value.Y;
				SetSize(w, h, false);
			}
		}

		/// <summary>Get the size of the original image in points (1/72 inch).</summary>
		public abstract PointD2D GetImageSizePt();

		public abstract Image GetImage();

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return GetRectangularObjectOutline();
		}

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
			((ImageGraphic)hitted).OnChanged();
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

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				switch (gripLevel)
				{
					case 0:
						return ((ImageGraphic)_hitobject).GetGrips(this, pageScale, GripKind.Move);

					case 1:
						return ((ImageGraphic)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Resize);

					case 2:
						return ((ImageGraphic)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Rotate);

					case 3:
						return ((ImageGraphic)_hitobject).GetGrips(this, pageScale, GripKind.Move | GripKind.Shear);
				}
				return null;
			}
		}

		#endregion HitTestObject
	} //  End Class
}