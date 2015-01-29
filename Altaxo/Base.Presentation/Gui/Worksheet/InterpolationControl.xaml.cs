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

#endregion Copyright

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for InterpolationControl.xaml
	/// </summary>
	public partial class InterpolationControl : UserControl, IInterpolationParameterView
	{
		public event Action<ValidationEventArgs<string>> ValidatingFrom;

		public event Action<ValidationEventArgs<string>> ValidatingTo;

		public event Action<ValidationEventArgs<string>> ValidatingNumberOfPoints;

		public event Action ChangedInterpolationMethod;

		public InterpolationControl()
		{
			InitializeComponent();
		}

		public void InitializeClassList(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbInterpolationClass, list);
		}

		public void InitializeNumberOfPoints(string val)
		{
			this._edNumberOfPoints.Text = val;
		}

		public void InitializeXOrg(string val)
		{
			this._edFrom.Text = val;
		}

		public void InitializeXEnd(string val)
		{
			this._edTo.Text = val;
		}

		private UIElement _detailControl;

		public void SetDetailControl(object detailControl)
		{
			if (_detailControl != null)
				_mainGrid.Children.Remove(_detailControl);

			_detailControl = (UIElement)detailControl;

			if (null != _detailControl)
			{
				_detailControl.SetValue(Grid.RowProperty, 4);
				_detailControl.SetValue(Grid.ColumnProperty, 2);
				_mainGrid.Children.Add(_detailControl);
			}
		}

		private void EhValueTo_Validating(object sender, ValidationEventArgs<string> e)
		{
			if (null != ValidatingTo)
				ValidatingTo(e);
		}

		private void EhValueNumberOfPoints_Validating(object sender, Altaxo.Gui.ValidationEventArgs<string> e)
		{
			if (null != ValidatingNumberOfPoints)
				ValidatingNumberOfPoints(e);
		}

		private void EhValueFrom_Validating(object sender, Altaxo.Gui.ValidationEventArgs<string> e)
		{
			if (null != ValidatingFrom)
				ValidatingFrom(e);
		}

		private void EhInterpolationClassChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbInterpolationClass);
			if (null != ChangedInterpolationMethod)
				ChangedInterpolationMethod();
		}
	}
}