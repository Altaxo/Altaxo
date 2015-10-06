using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Graph3D;
	using Altaxo.Gui.Graph3D.Common;

	/// <summary>
	/// Interaction logic for Graph3DControl.xaml
	/// </summary>
	public partial class Graph3DControl : UserControl, IGraph3DView
	{
		private Graph3DControllerWpf _controller;

		private Scene _scene;
		private D3D10GraphicContext _drawing;

		public Graph3DControl()
		{
			InitializeComponent();

			_scene = new Scene();
			this._d3dCanvas.Scene = _scene;
			_d3dCanvas.D3DStarted += EhD3DStarted;
		}

		private void EhD3DStarted(object sender, EventArgs e)
		{
			_d3dCanvas.D3DStarted -= EhD3DStarted;
			EhDocumentChanged();
		}

		Graph3DController IGraph3DView.Controller
		{
			set
			{
				var oldcontroller = _controller;
				_controller = value as Graph3DControllerWpf;

				if (!object.ReferenceEquals(oldcontroller, _controller))
					EhDocumentChanged();
			}
		}

		internal void SetPanelCursor(Cursor arrow)
		{
			_d3dCanvas.Cursor = arrow;
		}

		public object GuiInitiallyFocusedElement
		{
			get
			{
				return this;
			}
		}

		public int[] CurrentLayer
		{
			set
			{
			}
		}

		public void EhDocumentChanged()
		{
			if (null == _controller)
				return;

			var drawing = new D3D10GraphicContext();
			_controller.Doc.Paint(drawing);

			var olddrawing = _drawing;
			_drawing = drawing;

			if (null != olddrawing)
			{
				olddrawing.Dispose();
			}

			_scene.SetSceneSettings(_controller.Doc.Scene);
			_scene.SetDrawing(drawing);
		}

		private void EhGraphPanel_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.NumPad7)
			{
			}

			if (e.Key == Key.Up)
				_controller.EhMoveOrRoll(0, 1, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			else if (e.Key == Key.Down)
				_controller.EhMoveOrRoll(0, -1, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			else if (e.Key == Key.Right)
				_controller.EhMoveOrRoll(1, 0, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
			else if (e.Key == Key.Left)
				_controller.EhMoveOrRoll(-1, 0, e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control));
		}

		private void EhMouseWheel(object sender, MouseWheelEventArgs e)
		{
			var mousePosition = e.GetPosition(this._d3dCanvas);
			double relX = mousePosition.X / _d3dCanvas.ActualWidth;
			double relY = 1 - mousePosition.Y / _d3dCanvas.ActualHeight;
			_controller.EhMouseWheel(relX, relY, _d3dCanvas.ActualHeight / _d3dCanvas.ActualWidth, e.Delta);
		}

		private Altaxo.Graph3D.PointD3D GetMousePosition(MouseEventArgs e)
		{
			var p = e.GetPosition(_d3dCanvas);
			return new Altaxo.Graph3D.PointD3D(p.X / _d3dCanvas.ActualWidth, 1 - p.Y / _d3dCanvas.ActualHeight, _d3dCanvas.ActualHeight / _d3dCanvas.ActualWidth);
		}

		private void EhGraphPanel_MouseMove(object sender, MouseEventArgs e)
		{
			var guiController = _controller;
			if (null != guiController)
				guiController.EhView_GraphPanelMouseMove(GetMousePosition(e), e);
		}

		private void EhGraphPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var guiController = _controller;
			Keyboard.Focus(_d3dCanvas);
			var pos = GetMousePosition(e);
			if (null != guiController)
			{
				guiController.EhView_GraphPanelMouseDown(pos, e);
				if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
					guiController.EhView_GraphPanelMouseDoubleClick(pos, e);
				else if (e.ClickCount == 1 && e.LeftButton == MouseButtonState.Pressed)
					guiController.EhView_GraphPanelMouseClick(pos, e);
			}
		}

		private void EhGraphPanel_MouseUp(object sender, MouseButtonEventArgs e)
		{
			var guiController = _controller;
			if (null != guiController)
				guiController.EhView_GraphPanelMouseUp(GetMousePosition(e), e);
		}
	}
}