#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Altaxo.Gui.Graph.Graph3D.Plot.Data
{
	/// <summary>
	/// Interaction logic for SingleColumnControl.xaml
	/// </summary>
	public partial class SingleColumnControl : Grid, INotifyPropertyChanged
	{
		private Brush _defaultBackBrush;

		public SingleColumnControl()
		{
			InitializeComponent();
			_defaultBackBrush = _guiColumnText.Background;
		}

		public SingleColumnControl(object tag, string labelText)
		{
			this.Tag = tag;
			this.LabelText = labelText;

			InitializeComponent();

			_defaultBackBrush = _guiColumnText.Background;
		}

		public SingleColumnControl(object tag, string labelText, string columnText, string columnTooltip, int severityLevel)
		{
			this.Tag = tag;
			this.LabelText = labelText;

			InitializeComponent();

			_defaultBackBrush = _guiColumnText.Background;
			SetSeverityLevel(severityLevel);
			_guiColumnText.Text = columnText;
			_guiColumnText.ToolTip = columnTooltip;
		}

		private string _labelText;

		public event PropertyChangedEventHandler PropertyChanged;

		public string LabelText
		{
			get
			{
				return _labelText;
			}
			set
			{
				_labelText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelText)));
			}
		}

		public string ColumnText
		{
			get
			{
				return _guiColumnText.Text;
			}
			set
			{
				_guiColumnText.Text = value;
			}
		}

		public string ToolTipText
		{
			get
			{
				return _guiColumnText.ToolTip as string;
			}
			set
			{
				_guiColumnText.ToolTip = value;
			}
		}

		public string TransformationText
		{
			get
			{
				return _guiColumnTransformation.Text;
			}
			set
			{
				_guiColumnTransformation.Text = value;
			}
		}

		public string TransformationToolTipText
		{
			get
			{
				return _guiColumnTransformation.ToolTip as string;
			}
			set
			{
				_guiColumnTransformation.ToolTip = value;
			}
		}

		public void ShowTransformationSinglePrependAppendPopup(bool isOpen)
		{
			_guiPopup.IsOpen = isOpen;
		}

		public void SetSeverityLevel(int severity)
		{
			switch (severity)
			{
				case 0:
					_guiColumnText.Background = _defaultBackBrush;
					break;

				case 1:
					_guiColumnText.Background = Brushes.Yellow;
					break;

				case 2:
					_guiColumnText.Background = Brushes.LightPink;
					break;
			}
		}

		private void EhPopupFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (false == (bool)e.NewValue)
				_guiPopup.IsOpen = false;
		}

		private void EhPopup_Cancel(object sender, RoutedEventArgs e)
		{
			_guiPopup.IsOpen = false;
		}
	}
}