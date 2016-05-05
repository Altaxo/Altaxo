using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Background
{
	/// <summary>
	/// Represents a rectangular background with sharp edges.
	/// </summary>
	/// <seealso cref="Altaxo.Main.SuspendableDocumentNodeWithSetOfEventArgs" />
	/// <seealso cref="Altaxo.Main.ICopyFrom" />
	/// <seealso cref="Altaxo.Graph.Graph3D.Background.IBackgroundStyle" />
	public class RectangularBackground : Main.SuspendableDocumentNodeWithSetOfEventArgs, Main.ICopyFrom, IBackgroundStyle
	{
		private IMaterial _material = Materials.GetSolidMaterial(Drawing.NamedColors.AliceBlue);

		private double? _customDistance;
		private double? _customThickness;

		/// <summary>
		/// The extra padding of the content. Normally, zero for all elements.
		/// </summary>
		private Margin2D _padding;

		[NonSerialized]
		private double _resultingDistance;

		[NonSerialized]
		private double _resultingThickness;

		#region Serialization

		/// <summary>
		/// 2016-03-08 Initial version
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangularBackground), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (RectangularBackground)obj;
				info.AddValue("Material", s._material);
				info.AddValue("Padding", s._padding);
				info.AddValue("CustomDistance", s._customDistance);
				info.AddValue("CustomThickness", s._customThickness);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (RectangularBackground)o ?? new RectangularBackground();
				s._material = (IMaterial)info.GetValue("Material", s);
				s._padding = (Margin2D)info.GetValue("Padding", s);
				s._customDistance = info.GetNullableDouble("CustomDistance");
				s._customThickness = info.GetNullableDouble("CustomThickness");

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="RectangularBackground"/> class.
		/// </summary>
		public RectangularBackground()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RectangularBackground"/> class.
		/// </summary>
		/// <param name="from">From.</param>
		public RectangularBackground(RectangularBackground from)
		{
			CopyFrom(from);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new RectangularBackground(this);
		}

		/// <summary>
		/// Try to copy from another object. Should try to copy even if the object to copy from is not of
		/// the same type, but a base type. In this case only the base properties should be copied.
		/// </summary>
		/// <param name="obj">Object to copy from.</param>
		/// <returns>
		/// True if at least parts of the object could be copied, false if the object to copy from is incompatible.
		/// </returns>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as RectangularBackground;
			if (null != from)
			{
				this._material = from._material;
				this._customDistance = from._customDistance;
				this._customThickness = from._customThickness;
				this._padding = from._padding;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets or sets the material used to draw the background.
		/// </summary>
		/// <value>
		/// The material.
		/// </value>
		/// <exception cref="ArgumentNullException"></exception>
		public IMaterial Material
		{
			get
			{
				return _material;
			}
			set
			{
				if (null == _material)
					throw new ArgumentNullException(nameof(value));

				var oldValue = _material;
				_material = value;

				if (!object.ReferenceEquals(value, oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this background supports user defined material. For some background classes, the
		/// kind of material may be fixed, for instance a black background. In this case the value is false. For other backgrounds, you
		/// are free to chose the material, in this case the value is true.
		/// </summary>
		/// <value>
		/// <c>true</c> if this background allows to chose the material;  otherwise <c>false</c>.
		/// </value>
		public bool SupportsUserDefinedMaterial
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets the user defined background thickness. If this value is null, the background class has to find an appropriate thickness in the <see cref="Measure" /> step by itself.
		/// </summary>
		/// <value>
		/// The user defined thickness.
		/// </value>
		/// <exception cref="ArgumentException">Thickness has to be number >=0</exception>
		public double? Thickness
		{
			get
			{
				return _customThickness;
			}
			set
			{
				var oldValue = _customThickness;
				if (value.HasValue && !(value.Value >= 0))
					throw new ArgumentException("Thickness has to be number >=0", nameof(value));

				_customThickness = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the user defined distance. If this value is null, the background class has to find an appropriate distance in the <see cref="Measure" /> step by itself.
		/// </summary>
		/// <value>
		/// The user defined distance.
		/// </value>
		/// <exception cref="ArgumentException">Distance has to be a finite number</exception>
		public double? Distance
		{
			get
			{
				return _customDistance;
			}
			set
			{
				var oldValue = _customDistance;
				if (value.HasValue && !Altaxo.Calc.RMath.IsFinite(value.Value))
					throw new ArgumentException("Distance has to be a finite number", nameof(value));
				_customDistance = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the padding.
		/// </summary>
		/// <value>
		/// The padding.
		/// </value>
		public Margin2D Padding
		{
			get
			{
				return _padding;
			}
			set
			{
				var oldValue = _padding;
				_padding = value;
				if (oldValue != value)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Measures the background size and position.
		/// </summary>
		/// <param name="itemRectangle">Position and size of the item for which this background is intended. For text, this is the position and size of the text rectangle, already with a margin around.</param>
		/// <returns>
		/// The position and size of the rectangle that fully includes the background (but not the item).
		/// </returns>
		public RectangleD3D Measure(RectangleD3D itemRectangle)
		{
			_resultingDistance = _customDistance.HasValue ? _customDistance.Value : itemRectangle.SizeZ / 2;
			_resultingThickness = _customThickness.HasValue ? _customThickness.Value : itemRectangle.SizeZ;

			return GetRectangleToDraw(itemRectangle);
		}

		/// <summary>
		/// Gets the rectangle for the background to draw. Used both by <see cref="Measure"/> as well as <see cref="Draw"/>.
		/// </summary>
		/// <param name="itemRectangle">The item rectangle.</param>
		/// <returns></returns>
		private RectangleD3D GetRectangleToDraw(RectangleD3D itemRectangle)
		{
			return new RectangleD3D(
			itemRectangle.X - _padding.Left,
			itemRectangle.Y - _padding.Bottom,
			itemRectangle.Z - _resultingDistance - itemRectangle.SizeZ,
			itemRectangle.SizeX + _padding.Left + _padding.Right,
			itemRectangle.SizeY + _padding.Top + _padding.Bottom,
			_resultingThickness
			);
		}

		/// <summary>
		/// Draws the specified background
		/// </summary>
		/// <param name="g">The drawing context.</param>
		/// <param name="itemRectangle">Position and size of the item for which this background is intended. For text, this is the position and size of the text rectangle, already with a margin around.
		/// This parameter should have the same size as was used in the previous call to <see cref="Measure(RectangleD3D)" /></param>
		/// <exception cref="NotImplementedException"></exception>
		public void Draw(IGraphicsContext3D g, RectangleD3D itemRectangle)
		{
			var rectangleToDraw = GetRectangleToDraw(itemRectangle);

			var buffers = g.GetPositionNormalIndexedTriangleBuffer(_material);

			if (null != buffers.PositionNormalColorIndexedTriangleBuffer)
			{
				var c = _material.Color.Color;
				var voffs = buffers.PositionNormalColorIndexedTriangleBuffer.VertexCount;
				Altaxo.Drawing.D3D.SolidCube.Add(
					rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Z, rectangleToDraw.SizeX, rectangleToDraw.SizeY, rectangleToDraw.SizeZ,
					(point, normal) => buffers.PositionNormalColorIndexedTriangleBuffer.AddTriangleVertex(point.X, point.Y, point.Z, normal.X, normal.Y, normal.Z, c.ScR, c.ScG, c.ScB, c.ScA),
					(i1, i2, i3) => buffers.IndexedTriangleBuffer.AddTriangleIndices(i1 + voffs, i2 + voffs, i3 + voffs),
					ref voffs);
			}
			else if (null != buffers.PositionNormalIndexedTriangleBuffer)
			{
				var voffs = buffers.PositionNormalIndexedTriangleBuffer.VertexCount;
				Altaxo.Drawing.D3D.SolidCube.Add(
					rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Z, rectangleToDraw.SizeX, rectangleToDraw.SizeY, rectangleToDraw.SizeZ,
					(point, normal) => buffers.PositionNormalIndexedTriangleBuffer.AddTriangleVertex(point.X, point.Y, point.Z, normal.X, normal.Y, normal.Z),
					(i1, i2, i3) => buffers.IndexedTriangleBuffer.AddTriangleIndices(i1 + voffs, i2 + voffs, i3 + voffs),
					ref voffs);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Infrastructure: Gets all child nodes of this instance along with its name, and optionally, a method to set the member variable which holds that child node to <c>null</c>.
		/// The returned enumeration is used to (i) provide default implementations for <see cref="Altaxo.Main.INamedObjectCollection.GetChildObjectNamed" /> and <see cref="Altaxo.Main.INamedObjectCollection.GetNameOfChildObject" /> and (ii) to dispose all
		/// child objects when this instance (the parent) is disposed.
		/// </summary>
		/// <returns>
		/// Enumeration of all child nodes of this instance along with their name and optionally a method to set the member variable which holds that child node to <c>null</c>.
		/// </returns>
		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}
	}
}