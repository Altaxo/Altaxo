#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Data
{
	using Altaxo.Collections;

	/// <summary>
	/// Interaction logic for ExpandCyclingVariableControl.xaml
	/// </summary>
	public partial class ExpandCyclingVariableDataControl : UserControl, IExpandCyclingVariableDataView
	{
		public event Action SelectedTableChanged;

		public event Action SelectedGroupNumberChanged;

		public event Action UseSelectedAvailableColumnsAsParticipatingColumns;

		public event Action DeleteSelectedParticipatingColumn;

		public ExpandCyclingVariableDataControl()
		{
			InitializeComponent();
		}

		public void InitializeCyclingVariableColumn(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiColumnWithCyclingVariable, list);
		}

		private void EhCyclicVariableColumnChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiColumnWithCyclingVariable);
		}

		public void InitializeColumnsToAverage(SelectableListNodeList list)
		{
			_guiColumnsToAverage.Initialize(list);
		}

		private void EhTables_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(this._guiAvailableTables);
			if (null != SelectedTableChanged)
				SelectedTableChanged();
		}

		private void EhUseSelectedAvailableColumnsAsParticipatingColumns(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAvailableColumnNames);
			if (null != UseSelectedAvailableColumnsAsParticipatingColumns)
				UseSelectedAvailableColumnsAsParticipatingColumns();
		}

		private void EhDeleteSelectedParticipatingColumn(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiColumnsParticipating);
			if (null != DeleteSelectedParticipatingColumn)
				DeleteSelectedParticipatingColumn();
		}

		public void InitializeAvailableTables(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiAvailableTables, items);
		}

		private void EhGroupNumberChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var ev = SelectedGroupNumberChanged;
			if (null != ev)
				ev();
		}

		public int GroupNumber
		{
			get
			{
				return _guiGroupNumber.Value;
			}
			set
			{
				_guiGroupNumber.Value = value;
			}
		}

		public void InitializeAvailableColumns(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiAvailableColumnNames, items);
		}

		public void InitializeParticipatingColumns(SelectableListNodeList items)
		{
			GuiHelper.Initialize(_guiColumnsParticipating, items);
		}
	}
}