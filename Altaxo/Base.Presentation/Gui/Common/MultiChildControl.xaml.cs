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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for MultiChildControl.xaml
	/// </summary>
	[UserControlForController(typeof(IMultiChildViewEventSink))]
	public partial class MultiChildControl : UserControl, IMultiChildView
	{
		/// <summary>Event fired when one of the child controls is entered.</summary>
		public event EventHandler ChildControlEntered;
		/// <summary>Event fired when one of the child controls is validated.</summary>
		public event EventHandler ChildControlValidated;

		IMultiChildViewEventSink _controller;


		public MultiChildControl()
		{
			InitializeComponent();
		}

		public IMultiChildViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void InitializeBegin()
		{
		}

		public void InitializeEnd()
		{
		}

		public void InitializeLayout(bool horizontalLayout)
		{
			_stackPanel.Orientation = horizontalLayout ? Orientation.Horizontal : Orientation.Vertical;
		}

		public void InitializeDescription(string value)
		{
			_lblDescription.Content = value;
		}

		public void InitializeChilds(ViewDescriptionElement[] childs, int initialFocusedChild)
		{
			foreach (UIElement uiEle in _stackPanel.Children)
			{
				uiEle.GotFocus -= EhUIElement_GotFocus;
				uiEle.LostFocus -= EhUIElement_LostFocus;
			}

			_stackPanel.Children.Clear();
			foreach (var child in childs)
			{
				UIElement uiEle;

			
				if (child.View is System.Windows.Forms.Control)
				{
					var host = new System.Windows.Forms.Integration.WindowsFormsHost();
					host.Child = (System.Windows.Forms.Control)child.View;
					uiEle = host;
				}
				else
				{
					uiEle = (UIElement)child.View;
				}
				uiEle.GotFocus += EhUIElement_GotFocus;
				uiEle.LostFocus += EhUIElement_LostFocus;

				if (!string.IsNullOrEmpty(child.Title))
				{
					var gbox = new GroupBox();
					gbox.Header = child.Title;
					gbox.Content = uiEle;
					uiEle = gbox;
				}

				_stackPanel.Children.Add(uiEle);
			}

			_stackPanel.Children[initialFocusedChild].Focus();
		}

		void EhUIElement_LostFocus(object sender, RoutedEventArgs e)
		{
			if (null != ChildControlValidated)
				ChildControlValidated(this, EventArgs.Empty);
		}

		void EhUIElement_GotFocus(object sender, RoutedEventArgs e)
		{
			if (null != ChildControlEntered)
				ChildControlEntered(this, EventArgs.Empty);
		}

		
	}
}
