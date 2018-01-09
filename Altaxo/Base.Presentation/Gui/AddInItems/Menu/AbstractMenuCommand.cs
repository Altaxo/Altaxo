using Altaxo.AddInItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui.AddInItems
{
	/// <summary>
	/// For compatibility with SD 4.x.
	/// New code should use SimpleCommand instead.
	///
	/// TODO: make obsolete
	/// </summary>
	public abstract class AbstractMenuCommand : ICommand
	{
		private bool isEnabled = true;

		public virtual bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; }
		}

		public object Owner { get; set; }

		public abstract void Run();

		void ICommand.Execute(object parameter)
		{
			this.Owner = parameter;
			Run();
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add { CommandWrapper.RegisterConditionRequerySuggestedHandler(value); }
			remove { CommandWrapper.UnregisterConditionRequerySuggestedHandler(value); }
		}

		bool ICommand.CanExecute(object parameter)
		{
			return this.IsEnabled;
		}
	}

	/// <summary>
	/// For compatibility with SD 4.x.
	///
	/// TODO: make obsolete
	/// </summary>
	public abstract class AbstractCheckableMenuCommand : AbstractMenuCommand, ICheckableMenuCommand
	{
		public virtual bool IsChecked { get; set; }

		public override void Run()
		{
			IsChecked = !IsChecked;
		}

		event EventHandler ICheckableMenuCommand.IsCheckedChanged
		{
			add { CommandWrapper.RegisterConditionRequerySuggestedHandler(value); }
			remove { CommandWrapper.UnregisterConditionRequerySuggestedHandler(value); }
		}

		bool ICheckableMenuCommand.IsChecked(object parameter)
		{
			this.Owner = parameter;
			return this.IsChecked;
		}
	}
}