using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Graph;

namespace Altaxo.Gui.Graph
{
	public class ObjectPositionAndSizeGlue : FrameworkElement
	{
		#region Input/Output
		static void SetPositionText(TextBox t, double value)
		{
			if (t != null)
				t.Text = GUIConversion.GetLengthMeasureText(value);
		}

		static bool GetPositionValue(TextBox t, ref double value)
		{
			return GUIConversion.GetLengthMeasureValue(t.Text, ref value);
		}

		static void SetSizeText(TextBox t, double value)
		{
			SetPositionText(t, value);
		}
		static bool GetSizeValue(TextBox t, ref double value)
		{
			return GetPositionValue(t, ref value);
		}

		#endregion

		#region Position
		double _positionX;
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double PositionX
		{
			get {
				if (null != _edPositionX)
					return _edPositionX.PositionDistanceValue;
				else 
					return _positionX;
			}
			set
			{
				_positionX = value;
				if (null != _edPositionX)
					_edPositionX.PositionDistanceValue = value;
			}
		}


		double _positionY;
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double PositionY
		{
			get
			{
				if (null != _edPositionY)
					return _edPositionY.PositionDistanceValue;
				else
					return _positionY;
			}
			set
			{
				_positionY = value;
				if (null != _edPositionY)
					_edPositionY.PositionDistanceValue = value;
			}
		}

		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PointD2D Position
		{
			get
			{
				return new PointD2D(PositionX, PositionY);
			}
			set
			{
				PositionX = value.X;
				PositionY = value.Y;
			}
		}

		Altaxo.Gui.Common.Drawing.PositionBox _edPositionX;
		public Altaxo.Gui.Common.Drawing.PositionBox EdPositionX
		{
			get { return _edPositionX; }
			set
			{
				_edPositionX = value;

				if (_edPositionX != null)
					_edPositionX.PositionDistanceValue = _positionX;
			}
		}

		Altaxo.Gui.Common.Drawing.PositionBox _edPositionY;
		public Altaxo.Gui.Common.Drawing.PositionBox EdPositionY
		{
			get { return _edPositionY; }
			set
			{
				_edPositionY = value;

				if (_edPositionY != null)
					_edPositionY.PositionDistanceValue = _positionY;
			}
		}
		#endregion

		#region Size

		double _sizeX;
		public double SizeX
		{
			get
			{
				if (null != _edSizeX)
					return _edSizeX.PositionDistanceValue;
				else
					return _sizeX; 
			}
			set
			{
				_sizeX = value;
				if (null != _edSizeX)
					_edSizeX.PositionDistanceValue = value;
			}
		}

		double _sizeY;
		public double SizeY
		{
			get
			{
				if (null != _edSizeY)
					return _edSizeY.PositionDistanceValue;
				else
					return _sizeY;
			}
			set
			{
				_sizeY = value;
				if (null != _edSizeY)
					_edSizeY.PositionDistanceValue = value;
			}
		}

		public PointD2D Size
		{
			get
			{
				return new PointD2D(SizeX, SizeY);
			}
			set
			{
				SizeX = value.X;
				SizeY = value.Y;
			}
		}

		Altaxo.Gui.Common.Drawing.PositionBox _edSizeX;
		Altaxo.Gui.Common.Drawing.PositionBox _edSizeY;

		public Altaxo.Gui.Common.Drawing.PositionBox EdSizeX
		{
			get { return _edSizeX; }
			set
			{
				_edSizeX = value;

				if (_edSizeX != null)
					_edSizeX.PositionDistanceValue = _sizeX;
			}
		}
		public Altaxo.Gui.Common.Drawing.PositionBox EdSizeY
		{
			get { return _edSizeY; }
			set
			{
				_edSizeY = value;

				if (_edSizeY != null)
					_edSizeY.PositionDistanceValue = _sizeY;

				SetPositionText(_edSizeY, _sizeY);
			}
		}

		#endregion


		#region Rotation

		double _rotation;
		public double Rotation
		{
			get
			{
				if (null != _cbRotation)
					return _cbRotation.SelectedRotation;
				else
					return _rotation;
			}
			set
			{
				_rotation = value;
				if (_cbRotation != null)
					_cbRotation.SelectedRotation = value;
			}
		}

		Altaxo.Gui.Common.Drawing.RotationComboBox _cbRotation;
		public Altaxo.Gui.Common.Drawing.RotationComboBox CbRotation
		{
			get { return _cbRotation; }
			set
			{
				_cbRotation = value;
				if (_cbRotation != null)
				{
					_cbRotation.SelectedRotation = _rotation;
				}
			}
		}

		#endregion


		#region Shear

		double _shear;
		public double Shear
		{
			get
			{
				if (null != _edShear)
					return _edShear.Shear;
				else 
					return _shear;
			}
			set
			{
				_shear = value;
				if (_edShear != null)
					_edShear.Shear = value;
			}
		}

		Altaxo.Gui.Common.Drawing.ShearComboBox _edShear;
		public Altaxo.Gui.Common.Drawing.ShearComboBox GuiShear
		{
			get { return _edShear; }
			set
			{
				_edShear = value;

				if (_edShear != null)
				{
					_edShear.Shear = _shear;
				}
			}
		}
		
		#endregion


		#region ScaleX

		double _scaleX = 1;
		public double ScaleX
		{
			get
			{
				if (_edScaleX != null)
					return _edScaleX.SelectedScale;
				else
					return _scaleX;
			}
			set
			{
				_scaleX = value;
				if (_edScaleX != null)
					_edScaleX.SelectedScale = value;
			}
		}

		Altaxo.Gui.Common.Drawing.ScaleComboBox _edScaleX;
		public Altaxo.Gui.Common.Drawing.ScaleComboBox GuiScaleX
		{
			get { return _edScaleX; }
			set
			{
				_edScaleX = value;

				if (_edScaleX != null)
				{
					_edScaleX.SelectedScale = _scaleX;
				}
			}
		}
	
		#endregion

		#region ScaleY

		double _scaleY = 1;
		public double ScaleY
		{
			get
			{
				if(_edScaleY!=null)
					return _edScaleY.SelectedScale;
				else 
					return _scaleY;
			}
			set
			{
				_scaleY = value;
				if (_edScaleY != null)
					_edScaleY.SelectedScale = value;
			}
		}

		Altaxo.Gui.Common.Drawing.ScaleComboBox _edScaleY;
		public Altaxo.Gui.Common.Drawing.ScaleComboBox GuiScaleY
		{
			get { return _edScaleY; }
			set
			{
				_edScaleY = value;
				if (_edScaleY != null)
				{
					_edScaleY.SelectedScale = _scaleY;
				}
			}
		}
		#endregion
	}
}
