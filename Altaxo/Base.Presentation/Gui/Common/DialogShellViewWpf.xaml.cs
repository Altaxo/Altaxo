#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
	/// Interaction logic for DialogShellViewWpf.xaml
	/// </summary>
	public partial class DialogShellViewWpf : Window, IDialogShellView
	{
		UIElement _hostedControl;

		public DialogShellViewWpf()
		{
			InitializeComponent();
		}

		public DialogShellViewWpf(System.Windows.UIElement hostedControl)
			: this()
		{
			_hostedControl = hostedControl;
			_hostedControl.SetValue(Grid.RowProperty, 0);
			_hostedControl.SetValue(Grid.ColumnProperty, 0);
			_grid.Children.Add(_hostedControl);


		}


		/// <summary>
		/// Overrides the logic for determining the size of the dialog window. See remarks for details.
		/// </summary>
		/// <param name="availableSize"></param>
		/// <returns></returns>
		/// <remarks>
		/// There are two changes from the default behaviour: 
		/// <para>(i) when the dialog is loaded, the size is adjusted so that it is not bigger than
		/// the available working area on the screen. If the initial position of the dialog is chosen so that the right lower corner of the dialog would be outside
		/// of the working area, it is adjusted so that it is inside the working area.</para>
		/// <para>
		/// If during the dialog is showed the size of the content changed, the dialog box size is adjusted so that the lower right corner of the dialog window would 
		/// always be inside the working area. This does not apply if the user had manually changed the size of the dialog box before.
		/// </para>
		/// 
		/// </remarks>
		protected override Size MeasureOverride(Size availableSize)
		{
			var workArea = System.Windows.SystemParameters.WorkArea; // the working area on the screen

			if (!this.IsLoaded) // when the dialog is initially loaded
			{
				// adjust the size of the dialog box so that is is maximum the size of the working area
				if (availableSize.Height > workArea.Height)
					availableSize.Height = workArea.Height;
				if (availableSize.Width > workArea.Width)
					availableSize.Width = workArea.Width;

				// adjust the top and left position of the dialog if neccessary so that the dialog box fits inside the working area
				if (this.Top + availableSize.Height > workArea.Bottom)
					this.Top = workArea.Bottom - availableSize.Height;
				if (this.Left + availableSize.Width > workArea.Right)
					this.Left = workArea.Right - availableSize.Width;

			}
			else if (this.SizeToContent == System.Windows.SizeToContent.WidthAndHeight) // when the content size changed and the user had not manually resized the box
			{
				// adjust the size of the dialog box so that it fits inside of the working area (without changing the position of the dialog
				if (this.Top + availableSize.Height > workArea.Bottom)
					availableSize.Height = workArea.Bottom - this.Top;
				if (this.Left + availableSize.Width > workArea.Right)
					availableSize.Width = workArea.Right - this.Left;
			}

			if (availableSize.Height < 0)
				availableSize.Height = 0;
			if (availableSize.Width < 0)
				availableSize.Width = 0;

			return base.MeasureOverride(availableSize);
		}



		#region IDialogShellView

		public bool ApplyVisible
		{
			set
			{
				_btApply.Visibility = value ? Visibility.Visible : Visibility.Hidden;
			}
		}

		public event Action<System.ComponentModel.CancelEventArgs> ButtonOKPressed;

		public event Action ButtonCancelPressed;

		public event Action ButtonApplyPressed;

		#endregion



		#region Event handlers

		private void EhButtonOKPressed(object sender, RoutedEventArgs e)
		{
			_btOk.Focus(); // Trick to drag the focus away from the embedded control in order to trigger its validation code <b>before</b> the controls Apply routine is called. (This is only needed if user pressed Enter on the keyboard).
			var eventArgs = new System.ComponentModel.CancelEventArgs();
			if (null != ButtonOKPressed)
				ButtonOKPressed(eventArgs);

			if (!eventArgs.Cancel)
			{
				this.DialogResult = true;
			}

		}

		private void EhButtonCancelPressed(object sender, RoutedEventArgs e)
		{
			if (null != ButtonCancelPressed)
				ButtonCancelPressed();

			this.DialogResult = false;
		}

		private void EhButtonApplyPressed(object sender, RoutedEventArgs e)
		{
			if (null != ButtonApplyPressed)
				ButtonApplyPressed();

		}

		private void EhViewLoaded(object sender, RoutedEventArgs e)
		{
			_hostedControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
		}

		#endregion



	}
}
