using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Altaxo.Gui
{
  /// <summary>
  /// Implements a XAML proxy which can be used to bind items (ContextMenu, TreeViewItem, ListViewItem etc)
  /// for which it is not possible to find the ancestor user control via Binding RelativeSource...
  /// </summary>
  /// <remarks>
  /// Source: see <see href="http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/"/>,
  /// Issue: see <see href="http://stackoverflow.com/questions/9994241/mvvm-binding-command-to-contextmenu-item"/>
  /// </remarks>
  /// <example>
  /// Do something like this: (you may have to adapt it a bit to make it work in your control):
  /// (i) This will give you access to the UserControl DataContext using a StaticResource:
  /// <code>
  /// <UserControl.Resources>
  ///     <BindingProxy x:Key="DataContextProxy" Data="{Binding}" />
  /// </UserControl.Resources>
  /// </code>
  /// (ii) this uses the DataContextProxy defined in (i):
  /// <code>
  /// <Button.ContextMenu>
  ///     <ContextMenu>
  ///         <MenuItem   Header = "Remove" CommandParameter="{Binding Name}"
  ///                     Command="{Binding Path=Data.RemoveCommand, Source={StaticResource DataContextProxy}}"/>
  ///     </ContextMenu>
  /// </Button.ContextMenu>
  /// </code>
  /// </example>
  public class BindingProxy : Freezable
  {
    public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the data object this class is forwarding to everyone
    /// who has a reference to this object.
    /// </summary>
    public object Data
    {
      get { return (object)this.GetValue(DataProperty); }
      set { this.SetValue(DataProperty, value); }
    }

    /// <summary>
    /// Overrides of Freezable
    /// </summary>
    /// <returns></returns>
    protected override Freezable CreateInstanceCore()
    {
      return new BindingProxy();
    }
  }
}
