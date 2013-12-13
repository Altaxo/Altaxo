using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public class ItemLocationDirectAutoSize : ItemLocationDirect, ICloneable
	{
		#region Serialization

		/// <summary>
		/// 2013-11-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirectAutoSize), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, typeof(ItemLocationDirectAutoSize).BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ItemLocationDirectAutoSize)o : new ItemLocationDirectAutoSize();
				info.GetBaseValueEmbedded(s, typeof(ItemLocationDirectAutoSize).BaseType, parent);
				return s;
			}
		}

		#endregion Serialization

		#region Construction and copying

		public ItemLocationDirectAutoSize()
		{
		}

		public ItemLocationDirectAutoSize(ItemLocationDirect from)
		{
			CopyFrom(from);
		}

		public ItemLocationDirectAutoSize(IItemLocation from)
		{
			CopyFrom(from);
		}

		object System.ICloneable.Clone()
		{
			return new ItemLocationDirectAutoSize(this);
		}

		public override ItemLocationDirect Clone()
		{
			return new ItemLocationDirectAutoSize(this);
		}

		#endregion Construction and copying

		#region New methods/properties



		/// <summary>
		/// If the <see cref="IsAutoSize"/> property is <c>true</c> for this instance, the graphical object has to use this function to indicate its size.
		/// </summary>
		/// <param name="autoSize">Size of the graphical object.</param>
		/// <exception cref="System.InvalidOperationException">Using SetAutoSize is not supported because IsAutoSized is false</exception>
		public void SetSizeInAutoSizeMode(PointD2D autoSize)
		{
			SetSizeInAutoSizeMode(autoSize, true);
		}

		/// <summary>
		/// If the <see cref="IsAutoSize"/> property is <c>true</c> for this instance, the graphical object has to use this function to indicate its size.
		/// </summary>
		/// <param name="autoSize">Size of the graphical object.</param>
		/// <param name="isChangeEventEnabled">If true, the Change event will be fired if the size has changed.</param>
		/// <exception cref="System.InvalidOperationException">Using SetAutoSize is not supported because IsAutoSized is false</exception>
		public void SetSizeInAutoSizeMode(PointD2D autoSize, bool isChangeEventEnabled)
		{
			if (_sizeX.IsRelative || _sizeY.IsRelative || _sizeX.Value != autoSize.X || _sizeY.Value != autoSize.Y)
			{
				_sizeX = RADouble.NewAbs(autoSize.X);
				_sizeY = RADouble.NewAbs(autoSize.Y);

				if(isChangeEventEnabled)
					OnChanged();
			}
		}

		#endregion

		#region overrides

		public override bool IsAutoSized
		{
			get
			{
				return true;
			}
		}

		protected override void InternalSetSizeXSilent(RADouble value)
		{
			throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
		}

		protected override void InternalSetSizeYSilent(RADouble value)
		{
			throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
		}

		protected override void InternalSetSizeSilent(RADouble valueX, RADouble valueY)
		{
			throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
		}

		/// <summary>
		/// Sets the position and size. Since this location is for autosized items, we silently ignore the size parameter here.
		/// </summary>
		/// <param name="x">The x position.</param>
		/// <param name="y">The y position.</param>
		/// <param name="width">The width (ignored).</param>
		/// <param name="height">The height (ignored).</param>
		public override void SetPositionAndSize(RADouble x, RADouble y, RADouble width, RADouble height)
		{
			bool isChanged = x != _positionX || y != _positionY;

			_positionX = x;
			_positionY = y;

			if (isChanged)
				OnChanged();
		}

		#endregion

	}
}