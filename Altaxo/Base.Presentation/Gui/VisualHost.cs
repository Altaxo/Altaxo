using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Altaxo.Gui
{
	// Create a host visual derived from the FrameworkElement class.
	// This class provides layout, event handling, and container support for
	// the child visual objects.
	public class VisualHost : FrameworkElement
	{
		// Create a collection of child visual objects.
		private DrawingVisual _child;

		Action<DrawingContext> _renderMethod;

		public VisualHost()
			: this(null)
		{
		}

		public VisualHost(Action<DrawingContext> renderMethod)
		{
			_child = new DrawingVisual();
			_renderMethod = renderMethod;
		}

		public DrawingContext OpenDrawingContext()
		{
			return _child.RenderOpen();
		}

		// Provide a required override for the VisualChildrenCount property.
		protected override int VisualChildrenCount
		{
			get { return 1; }
		}

		// Provide a required override for the GetVisualChild method.
		protected override Visual GetVisualChild(int index)
		{
			if (index != 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			return _child;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (null != _renderMethod)
				_renderMethod(drawingContext);

			base.OnRender(drawingContext);
		}
	}

}
