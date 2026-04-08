/*
** Code by Fredrik Hedblad, Sweden, published under 'Unlicense' terms
*  see https://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Used to bind read-only dependence properties one way to source. This is not possible in WPF by design.
  /// See <see cref="PushBindingManager"/> for an example.
  /// </summary>
  /// <seealso cref="FreezableCollection{PushBinding}" />
  public class PushBindingCollection : FreezableCollection<PushBinding>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PushBindingCollection"/> class.
    /// </summary>
    public PushBindingCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PushBindingCollection"/> class.
    /// </summary>
    /// <param name="targetObject">The target object whose read-only dependency properties are observed.</param>
    public PushBindingCollection(DependencyObject targetObject)
    {
      TargetObject = targetObject;
      ((INotifyCollectionChanged)this).CollectionChanged += CollectionChanged;
    }

    void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (PushBinding pushBinding in e.NewItems)
        {
          pushBinding.SetupTargetBinding(TargetObject);
        }
      }
    }

    /// <summary>
    /// Gets the target object whose dependency properties are mirrored back to the source.
    /// </summary>
    public DependencyObject TargetObject
    {
      get;
      private set;
    }
  }
}
