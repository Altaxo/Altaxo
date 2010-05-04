using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Class that holds the location, rotation, shear and scale of an graphical item.
	/// </summary>
	[Serializable]
	public class LocationRotationShearxScaleTransformation2D
	{
		double _x;
		double _y;
		double _rotationDeg;
		double _shearX;
		double _scaleX=1;
		double _scaleY=1;
		TransformationMatrix2D _transformation = new TransformationMatrix2D();
		
		[field:NonSerialized]
		public event EventHandler Changed;

		/// <summary>
		/// Translation (or location) of this transformation.
		/// </summary>
		public PointD2D Location
		{
			get
			{
				return new PointD2D(_x, _y);
			}
			set
			{
				var chg = !(_x==value.X && _y==value.Y);
				_x = value.X;
				_y = value.Y;
				if(chg)
					OnChanged();
			}
		}

		/// <summary>
		/// X value of the translation (location).
		/// </summary>
		public double X
		{
			get
			{
				return _x;
			}
			set
			{
				var chg = !(_x==value);
				_x = value;
				if(chg)
					OnChanged();
				
			}
		}


		/// <summary>
		/// Y value of the translation (location).
		/// </summary>
		public double Y
		{
			get
			{
				return _y;
			}
			set
			{
				var chg = !(_y==value);
				_y = value;
				if(chg)
					OnChanged();
				
			}
		}

		/// <summary>
		/// Rotation value in degree counterclockwise.
		/// </summary>
		public double RotationDeg
		{
			get
			{
				return _rotationDeg;
			}
			set
			{
				var chg = !(_rotationDeg==value);
				_rotationDeg = value;
				if(chg)
					OnChanged();
			}
		}

		/// <summary>
		/// Rotation value in rad counterclockwise.
		/// </summary>
		public double RotationRad
		{
			get
			{
				return _rotationDeg * (Math.PI / 180);
			}
			set
			{
				value *= (180 / Math.PI);
				var chg = !(_rotationDeg == value);
				_rotationDeg = value;
				if (chg)
					OnChanged();
			}
		}

		/// <summary>
		/// Shear value. This is x shear, meaning that the x value is shifted along the y axis by y*shear.
		/// </summary>
		public double ShearX
		{
			get
			{
				return _shearX;
			}
			set
			{
				var chg = !(_shearX==value);
				_shearX = value;
				if(chg)
					OnChanged();
			}
		}

		/// <summary>
		/// Scale in x and y direction. Both values are normally 1.
		/// </summary>
		public PointD2D Scale
		{
			get
			{
				return new PointD2D(_scaleX, _scaleY);
			}
			set
			{
			var chg = !(_scaleX==value.X && _scaleY==value.Y);
				_scaleX = value.X;
				_scaleY = value.Y;
				if(chg)
					OnChanged();
			}
		}

		/// <summary>
		/// Scale in x direction. The default value is 1.
		/// </summary>
		public double ScaleX
		{
			get
			{
				return _scaleX;
			}
			set
			{
				var chg = !(_scaleX==value);
				_scaleX = value;
				if(chg)
					OnChanged();
				
			}
		}

		/// <summary>
		/// Scale in y direction. The default value is 1.
		/// </summary>
		public double ScaleY
		{
			get
			{
				return _scaleY;
			}
			set
			{
				var chg = !(_scaleY==value);
				_scaleY = value;
				if(chg)
					OnChanged();
				
			}
		}

		/// <summary>
		/// Returns the transformation matrix. For performance reasons, this is the value stored in this instance.
		/// If you intend to change the transformation, consider using <see cref="TransformationClone"/> instead.
		/// </summary>
		public TransformationMatrix2D Transformation
		{
			get
			{
				return _transformation;
			}
		}

		/// <summary>
		/// Returns a clone of the transformation matrix.
		/// </summary>
		public TransformationMatrix2D TransformationClone
		{
			get
			{
				return _transformation.Clone();
			}
		}

		/// <summary>
		/// Sets all transformation values and updates the transformation matrix.
		/// </summary>
		/// <param name="x">Translation in x direction.</param>
		/// <param name="y">Translation in y direction.</param>
		/// <param name="rotation">Roation in degrees counterclockwise.</param>
		/// <param name="shearX">Shear in x-direction.</param>
		/// <param name="scaleX">X scale value.</param>
		/// <param name="scaleY">Y scale value.</param>
		public void SetTranslationRotationShearxScale(double x, double y, double rotation, double shearX, double scaleX, double scaleY)
		{
				_x = x;
				_y = y;
				_rotationDeg = rotation;
				_shearX = shearX;
				_scaleX = scaleX;
				_scaleY = scaleY;

			OnChanged();
		}

		/// <summary>
		/// Sets all provided transformation values and updates the transformation matrix. You can leave out
		/// multiple parameters by setting them to null.
		/// </summary>
		/// <param name="x">Translation in x direction.</param>
		/// <param name="y">Translation in y direction.</param>
		/// <param name="rotation">Roation in degrees counterclockwise.</param>
		/// <param name="shearX">Shear in x-direction.</param>
		/// <param name="scaleX">X scale value.</param>
		/// <param name="scaleY">Y scale value.</param>
		/// <param name="suppressChangeEvent">If true, the <see cref="Changed"/> event will be suppressed, but the transformation matrix will be updated in any case.</param>
		public void SetTranslationRotationShearxScale(double? x, double? y, double? rotation, double? shearX, double? scaleX, double? scaleY, bool suppressChangeEvent)
		{
			if(null!=x)
				_x = (double)x;
			if(null!=y)
				_y = (double)y;
			if(null!=rotation)
				_rotationDeg = (double)rotation;
			if(null!=shearX)
				_shearX = (double)shearX;
			if(null!=scaleX)
				_scaleX = (double)scaleX;
			if(null!=_scaleY)
				_scaleY = (double)scaleY;

			OnChanged(suppressChangeEvent);
		}

	
		/// <summary>
		/// Sets the value for translation, roation, shear and scale from a transformation matrix.
		/// </summary>
		/// <param name="transformation"></param>
		public void SetFrom(TransformationMatrix2D transformation)
		{
			SetTranslationRotationShearxScale(transformation.X,transformation.Y,transformation.Rotation, transformation.Shear, transformation.ScaleX, transformation.ScaleY);
		}

		/// <summary>
		/// Updates the transformation matrix and fires the <see cref="Changed"/> event.
		/// </summary>
		protected void OnChanged()
		{
			OnChanged(false);
		}

		/// <summary>
		/// Updates the transformation matrix and fires the Changed event, if not suppressed intentionally.
		/// </summary>
		/// <param name="suppressEventNotification"></param>
		protected void OnChanged(bool suppressEventNotification)
		{
			_transformation.SetTranslationRotationShearxScale(_x,_y,_rotationDeg, _shearX, _scaleX, _scaleY);

			if (!suppressEventNotification && null != Changed)
				Changed(this, EventArgs.Empty);
		}
	}
}
