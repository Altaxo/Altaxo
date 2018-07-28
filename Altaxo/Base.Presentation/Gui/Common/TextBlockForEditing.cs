#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// This is a modified TextBlock that can switch between two states:
  /// editing and normal. When it is in editing mode, the content is
  /// displayed in a TextBox that provides editing capability. When
  /// in normal mode, its content is displayed in this TextBlock that is not editable.
  /// Important: the binding options for the <see cref="P:TextBlock.Text"/> property are changed to two-way mode by default in this class!
  /// </summary>
  public class TextBlockForEditing : TextBlock
  {
    #region Static variables

    /// <summary>
    /// Internal class to hold some information about a certain type of <see cref="ItemsControl"/>.
    /// </summary>
    internal class ItemsControlInfo
    {
      /// <summary>The type of the item which is used by the <see cref="ItemsControl"/> (for instance ListBoxItem for a ListBox).</summary>
      public Type TypeOfItem;

      /// <summary>Function which takes a Item of the ItemsControl, and determines if this item is selected. For instance, for a ListBox, you should provide ListBoxItem.IsSelected.</summary>
      public Func<FrameworkElement, bool> IsItemSelected;

      /// <summary>Function whick takes the ItemsControl, and determines the number of selected elements.</summary>
      public Func<ItemsControl, int> SelectionCount;
    }

    /// <summary>
    /// Dictionary which stores information about certain types of <see cref="ItemsControl"/>. The key is the type of <see cref="ItemsControl"/>,
    /// and the value is the <see cref="ItemsControlInfo"/> holding information about this type of <see cref="ItemsControl"/>.
    /// </summary>
    private static Dictionary<Type, ItemsControlInfo> _itemsControlTypeToControlInfo;

    /// <summary>
    /// The types of item controls that are also items, and thus are not the 'final' items control of all the items. As for now, the <see cref="TreeViewItem"/> is such a type,
    /// but other types could be added.
    /// </summary>
    private static HashSet<System.Type> _itemsControlTypesToSkipOver;

    #endregion Static variables

    #region Member variables

    /// <summary>
    /// The adorner that adornes the _textBlock. In normal mode the adorner is null, and is created when the this instance is switched to edit mode.
    /// </summary>
    private TextEditingAdorner _adorner;

    /// <summary>
    /// Specifies whether this instance can switch to editing mode with the next LeftMouseButtonMouseUp event. If the current time is equal to or greater than this value,
    /// the next LeftMouseButtonMouseUp event schedules the item for editing. Set this member to DateTime.Max if the item is not eligible for editing.
    /// </summary>
    private DateTime _earliestTimeItemIsEligibleForEditing = DateTime.MaxValue;

    /// <summary>
    /// True when the timer scheduled the edit box to open.
    /// </summary>
    private bool _isAboutToSwitchToEditingMode = false;

    /// <summary>
    /// The items control (for instance ListBox, ListView, etc.) that contains this instance.
    /// </summary>
    private ItemsControl _itemsControl;

    /// <summary>
    /// The item (for instance ListBoxItem, ListViewItem, etc.) that contains this instance.
    /// </summary>
    private FrameworkElement _listItem;

    #endregion Member variables

    #region Static construction

    static TextBlockForEditing()
    {
      _itemsControlTypeToControlInfo = new Dictionary<Type, ItemsControlInfo>();

      _itemsControlTypeToControlInfo.Add(typeof(ListBox), new ItemsControlInfo { TypeOfItem = typeof(ListBoxItem), IsItemSelected = (x => ((ListBoxItem)x).IsSelected), SelectionCount = (x => ((ListBox)x).SelectedItems.Count) });
      _itemsControlTypeToControlInfo.Add(typeof(ListView), new ItemsControlInfo { TypeOfItem = typeof(ListViewItem), IsItemSelected = (x => ((ListViewItem)x).IsSelected), SelectionCount = (x => ((ListView)x).SelectedItems.Count) });
      _itemsControlTypeToControlInfo.Add(typeof(TreeView), new ItemsControlInfo { TypeOfItem = typeof(TreeViewItem), IsItemSelected = (x => ((TreeViewItem)x).IsSelected), SelectionCount = (x => 1) });

      _itemsControlTypesToSkipOver = new HashSet<Type>();
      _itemsControlTypesToSkipOver.Add(typeof(TreeViewItem));

      TextBlockForEditing.TextProperty.OverrideMetadata(typeof(TextBlockForEditing), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }

    /// <summary>
    /// Registers a kind of <see cref="ItemsControl"/> to work with these TextBlockForEditing.
    /// </summary>
    /// <param name="itemsControlType">Type of the <see cref="ItemsControl"/>. For instance, for a <see cref="ListBox"/>, provide <c>typeof(ListBox)</c>.</param>
    /// <param name="itemType">Type of the item that is used in the <see cref="ItemsControl"/>. For instance, for a <see cref="ListBox"/>, provide <c>typeof(ListBoxItem)</c>.</param>
    /// <param name="IsItemSelected">The function to determine if an item is selected. The argument is the provided item. The function must return true if the item is selected.</param>
    /// <param name="SelectionCount">The function to determine how many items in the <see cref="ItemsControl"/> are selected. The argument is the <see cref="ItemsControl"/>. The function must return the number of selected items.</param>
    /// <exception cref="System.ArgumentNullException">
    /// itemsControlType
    /// or
    /// itemType
    /// or
    /// IsItemSelected
    /// or
    /// SelectionCount
    /// </exception>
    public static void RegisterItemsControlType(Type itemsControlType, Type itemType, Func<FrameworkElement, bool> IsItemSelected, Func<ItemsControl, int> SelectionCount)
    {
      if (null == itemsControlType)
        throw new ArgumentNullException("itemsControlType");
      if (null == itemType)
        throw new ArgumentNullException("itemType");
      if (null == IsItemSelected)
        throw new ArgumentNullException("IsItemSelected");
      if (null == SelectionCount)
        throw new ArgumentNullException("SelectionCount");

      _itemsControlTypeToControlInfo[itemsControlType] = new ItemsControlInfo { TypeOfItem = itemType, IsItemSelected = IsItemSelected, SelectionCount = SelectionCount };
    }

    /// <summary>
    /// Registers a type of <see cref="ItemsControl"/> that are also items, and thus are not the 'final' items control of all the items. For example, the <see cref="TreeViewItem"/> is such a type (this type is pre-registered).
    /// </summary>
    /// <param name="itemsControlType">Type of the items control to skip over.</param>
    /// <exception cref="System.ArgumentNullException">itemsControlType</exception>
    public static void RegisterItemsControlTypeToSkipOver(Type itemsControlType)
    {
      if (null == itemsControlType)
        throw new ArgumentNullException("itemsControlType");

      _itemsControlTypesToSkipOver.Add(itemsControlType);
    }

    #endregion Static construction

    #region Construction

    /// <summary>
    /// This method is invoked whenever a item is initialized. Here, the binding to the TextBlock.Text property is created, if no such binding already exists.
    /// Additionally, the parent items control is retrieved.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      bool isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
      if (isInDesignMode)
        return;

      DependencyObject parent = this;
      for (; ; )
      {
        _itemsControl = GetDependencyObjectFromVisualTree(parent, typeof(ItemsControl)) as ItemsControl;
        if (!(_itemsControl != null))
          throw new InvalidOperationException(nameof(_itemsControl) + " is null. Underlying ItemsControl not found");

        if (!_itemsControlTypesToSkipOver.Contains(_itemsControl.GetType()))
          break;
        parent = VisualTreeHelper.GetParent(_itemsControl);
      }

      Type typeOfItemsControl = _itemsControl.GetType();
      // make sure that the type of ItemsControl is registered
      if (!_itemsControlTypeToControlInfo.ContainsKey(typeOfItemsControl))
        throw new InvalidOperationException(string.Format("Sorry, the type of ItemsControl ({0}) is not registered. Please use the static function 'RegisterItemsControlType' to register your ItemsControl type", _itemsControl.GetType()));

      Type typeOfItem = _itemsControlTypeToControlInfo[typeOfItemsControl].TypeOfItem;
      _listItem = GetDependencyObjectFromVisualTree(this, typeOfItem) as FrameworkElement;
      if (!(_listItem != null))
        throw new InvalidOperationException(string.Format("Underlying item of type {0} was not found in the visual hierarchy.", typeOfItem));
    }

    #endregion Construction

    #region Dependency properties

    #region IsEditing

    /// <summary>
    /// IsEditing DependencyProperty
    /// </summary>
    public static DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                    "IsEditing",
                    typeof(bool),
                    typeof(TextBlockForEditing),
                    new FrameworkPropertyMetadata(false, EhIsEditingChanged, EhIsEditingCoerce));

    /// <summary>
    /// Returns true if this instance is in editing mode.
    /// </summary>
    public bool IsEditing
    {
      get { return (bool)GetValue(IsEditingProperty); }
      private set { SetValue(IsEditingProperty, value); }
    }

    private static void EhIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var thiss = (TextBlockForEditing)d;

      if (false == (bool)e.OldValue && true == (bool)e.NewValue)
      {
        thiss.BeginEditing();
      }
      else if (true == (bool)e.OldValue && false == (bool)e.NewValue)
      {
        thiss.EndEditing();
      }

      thiss._itemsControl.SetValue(TextBlockForEditing.CurrentItemOpenForEditingProperty, true == (bool)e.NewValue ? thiss : null);
    }

    private static object EhIsEditingCoerce(DependencyObject d, object baseValue)
    {
      var thiss = (TextBlockForEditing)d;
      return (bool)baseValue && thiss.IsEditingEnabled;
    }

    /// <summary>
    /// Begins the editing. Here, the adorner is created and added to the adorner layer, and events to the editing text box are wired.
    /// </summary>
    private void BeginEditing()
    {
      AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
      _adorner = new TextEditingAdorner(this, this.Text, TextBoxStyle, TextBoxValidationRule);
      layer.Add(_adorner);

      _adorner.EditingFinished += EhEditingFinished;
    }

    private void EhEditingFinished(object sender, EventArgs e)
    {
      if (e is KeyboardEventArgs)
        _earliestTimeItemIsEligibleForEditing = DateTime.MaxValue;

      IsEditing = false;
    }

    /// <summary>
    /// Ends the editing. Events are unwired, and the adorner is removed from the adorner layer.
    /// </summary>
    private void EndEditing()
    {
      if (null == _adorner)
        return;
      _adorner.EditingFinished -= EhEditingFinished;

      // Processing order important here
      // because setting the Text dependency property can cause updates of the ItemsControl, set this property at the very end
      // before this, dispose all the plumbing neccessary in edit mode

      bool validationHasErrors = _adorner.ValidationHasErrors;
      string textBoxText = _adorner.EditedText;
      AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

      if (null != layer)
        layer.Remove(_adorner);
      _adorner = null;

      if (!validationHasErrors)
      {
        // update the Text roperty with the new text box value
        this.SetValue(TextProperty, textBoxText);
      }
    }

    #endregion IsEditing

    #region IsEditingEnabled

    /// <summary>
    /// IsEditing DependencyProperty, see <see cref="IsEditingEnabled"/>.
    /// </summary>
    public static DependencyProperty IsEditingEnabledProperty =
            DependencyProperty.Register(
                    "IsEditingEnabled",
                    typeof(bool),
                    typeof(TextBlockForEditing),
                    new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Get/sets the IsEditingEnabled property. If this property is set to true, it is allowed to enter edit mode. If this property is false, editing is not allowed.
    /// </summary>
    public bool IsEditingEnabled
    {
      get { return (bool)GetValue(IsEditingEnabledProperty); }
      set { SetValue(IsEditingEnabledProperty, value); }
    }

    #endregion IsEditingEnabled

    #region TextBoxValidationRule

    /// <summary>
    /// DependencyProperty that stores a validation rule for the edit box. This validation rule is used in editing mode only. If the validation is not successful,
    /// the error is indicated by a red border around the TextBox. Additionally, if validation is not successful, the edited value is not used to update the <see cref="P:TextBlock.Text"/> property when switching back to normal mode.
    /// </summary>
    public static readonly DependencyProperty TextBoxValidationRuleProperty =
            DependencyProperty.Register(
                    "TextBoxValidationRule",
                    typeof(ValidationRule),
                    typeof(TextBlockForEditing),
                    new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Stores a validation rule for the edit box. This validation rule is used in editing mode only. If the validation is not successful,
    /// the error is indicated by a red border around the TextBox. Additionally, if validation is not successful, the edited value is not used to update the <see cref="P:TextBlock.Text"/> property when switching back to normal mode.
    /// </summary>
    public ValidationRule TextBoxValidationRule
    {
      get { return GetValue(TextBoxValidationRuleProperty) as ValidationRule; }
      set { SetValue(TextBoxValidationRuleProperty, value); }
    }

    #endregion TextBoxValidationRule

    #region TextBoxStyle

    /// <summary>
    /// DependencyProperty to store a custom style for the <see cref="TextBox"/> that is used for editing. This property is especially useful to set a custom style when the validation (set by <see cref="ValidationRule"/> fails.
    /// Please make sure to add some padding (e.g. 16) to the right side of the TextBox, to ensure the text on the right side is not hidden.
    /// </summary>
    public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(
                    "TextBoxStyle",
                    typeof(Style),
                    typeof(TextBlockForEditing),
                    new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Property to store a custom style for the <see cref="TextBox"/> that is used for editing. This property is especially useful to set a custom style when the validation (set by <see cref="ValidationRule"/> fails.
    /// Please make sure to add some padding (e.g. 16) to the right side of the TextBox, to ensure the text on the right side is not hidden.
    /// </summary>
    public Style TextBoxStyle
    {
      get { return GetValue(TextBoxStyleProperty) as Style; }
      set { SetValue(TextBoxStyleProperty, value); }
    }

    #endregion TextBoxStyle

    #region Attached properties

    /// <summary>
    /// Attached property that is used to store the instance of this class that is currently in edit mode in the ItemsControl.
    /// </summary>
    public static readonly DependencyProperty CurrentItemOpenForEditingProperty =
          DependencyProperty.RegisterAttached(
                  "CurrentItemOpenForEditing",
                  typeof(TextBlockForEditing),
                  typeof(TextBlockForEditing),
                  new FrameworkPropertyMetadata(null, EhCurrentItemOpenForEditingChanged));

    /// <summary>
    /// Handles the event that the edit box instance that is currently in edit mode changed.
    /// </summary>
    /// <param name="d">The items control (or generally the DependencyObject to which the <see cref="CurrentItemOpenForEditingProperty"/> is attached.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void EhCurrentItemOpenForEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var itemsControl = d as ItemsControl;
      if (null == itemsControl)
        return;

      var instance = e.OldValue as TextBlockForEditing;
      if (null != instance)
      {
        // unwire events
        itemsControl.SizeChanged -= instance.EhItemsControl_SizeChanged;
        itemsControl.PreviewMouseDown -= instance.EhItemsControl_PreviewMouseDown;
        itemsControl.RemoveHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(instance.EhItemsControl_ScrollViewerChanged));
        itemsControl.RemoveHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler(instance.EhItemsControl_ScrollViewerMouseWheelEvent));
      }

      instance = e.NewValue as TextBlockForEditing;
      if (null != instance)
      {
        // wire events
        itemsControl.SizeChanged += instance.EhItemsControl_SizeChanged;
        itemsControl.PreviewMouseDown += instance.EhItemsControl_PreviewMouseDown;
        itemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(instance.EhItemsControl_ScrollViewerChanged));
        itemsControl.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler(instance.EhItemsControl_ScrollViewerMouseWheelEvent), true);
      }
    }

    #endregion Attached properties

    #endregion Dependency properties

    #region Own event handling / overrides

    /// <summary>
    /// If the MouseLeave event occurs for this instance, it is no longer eligible for editing (but a current editing remains unaffected).
    /// </summary>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      _earliestTimeItemIsEligibleForEditing = DateTime.MaxValue;
    }

    /// <summary>
    /// Gets the double click time. Shame on Microsoft for not providing this data in WPF.
    /// </summary>
    /// <returns>The double click time in Milliseconds.</returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern int GetDoubleClickTime();

    /// <summary>
    /// Handles the MouseLeftButtonUp event. If the item is eligible for editing, it will switch this instance to edit mode (after a short delay).
    /// If not already eligible for editing, this event will make this instance eligible for editing (when the presumtions are fulfilled).
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was released.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);

      if (!EnsureValidParents(e.GetPosition(this)))
        return;

      // 1st: do nothing if we are already in edit mode
      if (IsEditing)
      {
        return; // do nothing if we are already in edit mode
      }
      else
      {
        // we are not in edit mode yet, but our item is already marked for editing
        if (!e.Handled && DateTime.UtcNow >= _earliestTimeItemIsEligibleForEditing && !_isAboutToSwitchToEditingMode)
        {
          // Delay the setting of IsEditing to true by the DoubleClickTime.
          // In this way, if the user DoubleClicks the item, nothing will happen, because in the DoubleClick handler _isEligibleForEditing is set back to false
          int doubleClickTime_ms = GetDoubleClickTime();
          _isAboutToSwitchToEditingMode = true;
          new DispatcherTimer(new TimeSpan(0, 0, 0, 0, doubleClickTime_ms + 10), DispatcherPriority.Background, EhMouseWaitTimer_Elapsed, Dispatcher.CurrentDispatcher);

          return;
        }

        IsEditing = false;

        if (_isAboutToSwitchToEditingMode)
        {
          _earliestTimeItemIsEligibleForEditing = DateTime.MaxValue; // when the item is already about to be edited, a new click during the DoubleClick time will prevent the item from editing
        }
        else
        {
          // our item was not already marked for editing, thus mark it
          // mark our item as eligible for editing (but only if a single item is selected)
          int selectionCount = _itemsControlTypeToControlInfo[_itemsControl.GetType()].SelectionCount(_itemsControl);
          var itemIsEligibleForEditing = IsParentItemSelected && 1 == selectionCount;
          if (itemIsEligibleForEditing)
            _earliestTimeItemIsEligibleForEditing = DateTime.UtcNow.AddMilliseconds(GetDoubleClickTime() + 10);
          else
            _earliestTimeItemIsEligibleForEditing = DateTime.MaxValue;
        }
      }
    }

    /// <summary>
    /// Switches this instance to edit mode when the time has elapsed and it is still eligible for editing.
    /// </summary>
    /// <param name="sender">The source of the event (the timer).</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void EhMouseWaitTimer_Elapsed(object sender, EventArgs e)
    {
      var timer = (DispatcherTimer)sender;
      timer.Tick -= EhMouseWaitTimer_Elapsed;
      timer.Stop();

      _isAboutToSwitchToEditingMode = false;

      if (_earliestTimeItemIsEligibleForEditing != DateTime.MaxValue)
        IsEditing = true;
    }

    #endregion Own event handling / overrides

    #region ItemsControl event hooking and handling

    /// <summary>
    /// If the <see cref="ItemsControl"/> is resized while in editing mode, this instance switches back to normal mode.
    /// </summary>
    private void EhItemsControl_SizeChanged(object sender, RoutedEventArgs e)
    {
      IsEditing = false;
    }

    /// <summary>
    /// If this instance is in editing mode and the content of the <see cref="ItemsControl"/> is
    /// scrolled, then it switches back to normal mode.
    /// </summary>
    private void EhItemsControl_ScrollViewerChanged(object sender, RoutedEventArgs args)
    {
      if (IsEditing && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
      {
        IsEditing = false;
      }
    }

    /// <summary>
    /// Sets IsEditing to false when the Mouse wheel is turned
    /// </summary>
    private void EhItemsControl_ScrollViewerMouseWheelEvent(object sender, RoutedEventArgs e)
    {
      IsEditing = false;
    }

    private bool EnsureValidParents(Point pt)
    {
      if (_listItem == null || _itemsControl == null)
      {
        HitTestResult result = VisualTreeHelper.HitTest(this, pt);
        if (null != result)
        {
          _listItem = FindParent<ListBoxItem>(this);
          _itemsControl = FindParent<ItemsControl>(this);
        }
      }
      return _listItem != null && _itemsControl != null;
    }

    /// <summary>
    /// Sets IsEditing to false when the mouse goes down outside the list box item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
    private void EhItemsControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!IsEditing)
        return; // don't care if we are not editing

      // Find the listbox item under the mouse
      var pt = e.GetPosition(this);
      HitTestResult result = VisualTreeHelper.HitTest(_listItem, pt);

      if (null == result)
      {
        IsEditing = false;
      }
    }

    #endregion ItemsControl event hooking and handling

    #region Helper functions

    /// <summary>
    /// Gets whether the item that contains this instance is selected.
    /// </summary>
    private bool IsParentItemSelected
    {
      get
      {
        if (_listItem == null)
          return false;
        else
          return _itemsControlTypeToControlInfo[_itemsControl.GetType()].IsItemSelected(_listItem);
      }
    }

    #endregion Helper functions

    #region Static helper functions

    /// <summary>
    /// Walk the visual tree to find the first DependencyObject of the specified type.
    /// </summary>
    private static DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
    {
      //Walk the visual tree to get the parent(ItemsControl)
      //of this control
      DependencyObject parent = startObject;
      while (parent != null)
      {
        if (type.IsInstanceOfType(parent))
          break;
        else
          parent = VisualTreeHelper.GetParent(parent);
      }

      return parent;
    }

    public static T FindParent<T>(DependencyObject from) where T : class
    {
      T result = null;
      DependencyObject parent = VisualTreeHelper.GetParent(from);

      if (parent is T)
        result = parent as T;
      else if (parent != null)
        result = FindParent<T>(parent);

      return result;
    }

    #endregion Static helper functions
  }
}
