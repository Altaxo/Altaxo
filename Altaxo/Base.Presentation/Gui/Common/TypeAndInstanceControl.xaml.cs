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
	/// Interaction logic for TypeAndInstanceControl.xaml
	/// </summary>
	public partial class TypeAndInstanceControl : UserControl, ITypeAndInstanceView
	{
		public TypeAndInstanceControl()
		{
			InitializeComponent();
		}

		private void EhSelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbTypeChoice);
			if (null != TypeChoiceChanged)
				TypeChoiceChanged(this, EventArgs.Empty);
		}

		#region  ITypeAndInstanceView

		public string TypeLabel
		{
			set { _lblCSType.Content = value; }
		}

		public void InitializeTypeNames(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbTypeChoice, list);
		}

		public void SetInstanceControl(object instanceControl)
		{
			_instanceHost.Child = instanceControl as UIElement;
		}

		public event EventHandler TypeChoiceChanged;
	}

		#endregion
}
