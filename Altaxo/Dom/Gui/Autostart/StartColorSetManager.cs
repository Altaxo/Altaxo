using Altaxo.AddInItems;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.Autostart
{
	public class StartColorSetManager : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			var colorSets = AddInTree.GetTreeNode("/Altaxo/ApplicationColorSets").BuildChildItems<Tuple<IColorSet, bool>>(this);
			foreach (var entry in colorSets)
			{
				IColorSet storedList;
				ColorSetManager.Instance.TryRegisterList(entry.Item1, ItemDefinitionLevel.Application, out storedList);
				if (entry.Item2)
					ColorSetManager.Instance.DeclareAsPlotColorList(storedList);
			}
		}
	}
}