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

		public object GuiInitiallyFocusedElement
		{
			get
			{
				return this;
			}
		}

		public void EhDocumentChanged()
		{
			if (null == _controller)
				return;

			var drawing = new D3D10GraphicContext();
			_controller.Doc.Draw(drawing);

			var olddrawing = _drawing;
			_drawing = drawing;

			if (null != olddrawing)
			{
				olddrawing.Dispose();
			}
			_scene.SetDrawing(drawing);
		}
	}
}