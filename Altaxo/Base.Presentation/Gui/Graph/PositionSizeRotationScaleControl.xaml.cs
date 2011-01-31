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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for PositionSizeRotationScaleControl.xaml
	/// </summary>
	public partial class PositionSizeRotationScaleControl : UserControl
	{
		public PositionSizeRotationScaleControl()
		{
			InitializeComponent();

			_positionSizeGlue.EdPositionX = _edPositionX;
			_positionSizeGlue.EdPositionY = _edPositionY;
			_positionSizeGlue.EdSizeX = _edSizeX;
			_positionSizeGlue.EdSizeY = _edSizeY;

			_positionSizeGlue.GuiShear = _edShear;
			_positionSizeGlue.CbRotation = _edRotation;
			_positionSizeGlue.GuiScaleX = _edScaleX;
			_positionSizeGlue.GuiScaleY = _edScaleY;

		}

		public ObjectPositionAndSizeGlue PositionSizeGlue
		{
			get
			{
				return _positionSizeGlue;
			}
		}
	}
}
