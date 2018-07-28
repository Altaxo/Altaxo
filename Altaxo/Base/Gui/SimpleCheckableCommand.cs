using Altaxo.AddInItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui
{
  /// <summary>
  /// Simple command that toggles a <see cref="IsChecked"/> flag.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.SimpleCommand" />
  /// <seealso cref="Altaxo.AddInItems.ICheckableMenuCommand" />
  public abstract class SimpleCheckableCommand : SimpleCommand, ICheckableMenuCommand
  {
    public virtual bool IsChecked { get; set; }

    public override void Execute(object parameter)
    {
      IsChecked = !IsChecked;
    }

    event EventHandler ICheckableMenuCommand.IsCheckedChanged
    {
      add { Current.Gui.RegisterRequerySuggestedHandler(value); }
      remove { Current.Gui.UnregisterRequerySuggestedHandler(value); }
    }

    bool ICheckableMenuCommand.IsChecked(object parameter)
    {
      return this.IsChecked;
    }
  }
}
