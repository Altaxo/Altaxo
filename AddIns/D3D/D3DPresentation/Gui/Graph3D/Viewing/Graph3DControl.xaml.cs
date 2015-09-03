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
	/// <summary>
	/// Interaction logic for Graph3DControl.xaml
	/// </summary>
	public partial class Graph3DControl : UserControl, IGraph3DView
	{
		public Graph3DControl()
		{
			InitializeComponent();

			this.Canvas1.Scene = new Scene();
		}

		public object GuiInitiallyFocusedElement
		{
			get
			{
				return this;
			}
		}
	}
}