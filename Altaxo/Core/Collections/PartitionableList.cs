#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2013 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 3 of the License, or
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
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// List of items of class T. From this list partitions can be created, which basically are views inside this list, which have certain selection criterions. See remarks for details.
  /// </summary>
  /// <typeparam name="T">Type of the items in the list.</typeparam>
  /// <remarks>
  /// Say you have two classes, A and B, both having the base class T. Then you can create a PartionableList, holding items of class A as well as items of class B.
  /// From this list you then can create a partition, i.e. a view, that shows only items of class A, and another partition, that shows only items of class B.
  /// These partitions support all list operations, including deletion, insertion, movement, and setting of items. These list operations are propagated to
  /// the main list. Of course, it will cause an exception if you try to insert items of class B into the partition of class A (and vice versa). When you set
  /// or insert an item in a partition, that item has to fulfill the selection criterion for the particular partition.
  /// </remarks>
  public partial class PartitionableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
  {
    /// <summary>Contains all partial views that were created for this instance and are still alive.</summary>
    protected LinkedList<WeakReference> _partialViews = new LinkedList<WeakReference>();

    /// <summary>
    /// The partial views that need to notify after modifications that they have changed.
    /// </summary>
    protected HashSet<PartialViewBase> _partialViewsToNotify = new HashSet<PartialViewBase>();

    /// <summary>
    /// Defines an action that is executed before an item is inserted. The 1st argument is the item to insert.
    /// </summary>
    protected Action<T>? _actionBeforeInsertion;

    /// <summary>
    /// Get information whether the CollectionChanged events are enabled or disabled.
    /// </summary>
    protected TemporaryDisabler _eventState;

    /// <summary>
    /// Holds the last fired CollectionChanged event in case the event state is disabled.
    /// </summary>
    private NotifyCollectionChangedEventArgs? _pendingEvent;

    #region Constructors

    public PartitionableList()
    {
      _eventState = new TemporaryDisabler(OnReenableEvents);
    }

    public PartitionableList(Action<T> actionBeforeInsertion)
    {
      _eventState = new TemporaryDisabler(OnReenableEvents);
      _actionBeforeInsertion = actionBeforeInsertion;
    }

    #endregion Constructors

    #region Partial view creation

    /// <summary>
    /// Creates a partial view. A partial view is a view of the list of items of the original collection, that fulfill a certain condition.
    /// </summary>
    /// <param name="selectionCriterium">
    /// The selection condition.
    /// If this function returns <c>true</c> on an item of the original collection, that item is member of the partial view.
    /// Note that during the lifetime of the partition, the behaviour of the selection criterion must not change.
    /// </param>
    /// <returns>List of items of the original collection, that fullfils the selection condition. The returned list is automatically updated, when the original collection changed or when
    /// the user changed the partial view itself.</returns>
    public IObservableList<T> CreatePartialView(Func<T, bool> selectionCriterium)
    {
      var result = new PartialView<T>(this, selectionCriterium);
      _partialViews.AddLast(new WeakReference(result));
      return result;
    }

    /// <summary>
    /// Creates a partial view. A partial view is a view of the list of items of the original collection, that fulfill a certain condition.
    /// </summary>
    /// <param name="selectionCriterium">
    /// The selection condition.
    /// If this function returns <c>true</c> on an item of the original collection, that item is member of the partial view.
    /// Note that during the lifetime of the partition, the behaviour of the selection criterion must not change.
    /// </param>
    /// <param name="actionBeforeInsertion">Action that is executed on an item before it is inserted.</param>
    /// <returns>List of items of the original collection, that fullfils the selection condition. The returned list is automatically updated, when the original collection changed or when
    /// the user changed the partial view itself.</returns>
    public IObservableList<T> CreatePartialView(Func<T, bool> selectionCriterium, Action<T> actionBeforeInsertion)
    {
      var result = new PartialView<T>(this, selectionCriterium, actionBeforeInsertion);
      _partialViews.AddLast(new WeakReference(result));
      return result;
    }

    /// <summary>
    /// Creates the partial view that consists of all elements in the original collection that are of type M.
    /// </summary>
    /// <typeparam name="M">Type of the elements of the partial view.</typeparam>
    /// <returns>View of all elements of type M of the original collection.</returns>
    public IObservableList<M> CreatePartialViewOfType<M>() where M : T
    {
      var result = new PartialView<M>(this, x => x is M);
      _partialViews.AddLast(new WeakReference(result));
      return result;
    }

    /// <summary>
    /// Creates the partial view that consists of all elements in the original collection that are of type M.
    /// </summary>
    /// <typeparam name="M">Type of the elements of the partial view.</typeparam>
    /// <param name="actionBeforeInsertion">Action that is executed on an item before it is inserted.</param>
    /// <returns>View of all elements of type M of the original collection.</returns>
    public IObservableList<M> CreatePartialViewOfType<M>(Action<M> actionBeforeInsertion) where M : T
    {
      var result = new PartialView<M>(this, x => x is M, actionBeforeInsertion);
      _partialViews.AddLast(new WeakReference(result));
      return result;
    }

    /// <summary>
    /// Creates the partial view that consisist of elements of type M that fullfil a given condition.
    /// </summary>
    /// <typeparam name="M">Type of the elements of the partial view.</typeparam>
    /// <param name="selectionCriterium">The selection criterium.</param>
    /// <returns></returns>
    public IObservableList<M> CreatePartialViewOfType<M>(Func<M, bool> selectionCriterium) where M : T
    {
      var result = new PartialView<M>(this, x => (x is M mx) && selectionCriterium(mx));
      _partialViews.AddLast(new WeakReference(result));
      return result;
    }

    /// <summary>
    /// Creates the partial view that consisist of elements of type M that fullfil a condition provided by the argument <paramref name="selectionCriterium"/>.
    /// </summary>
    /// <typeparam name="M">Type of the elements of the partial view.</typeparam>
    /// <param name="selectionCriterium">The selection criterium. If this function applied to an element returns <c>true</c>, this element is included into the partial view.</param>
    /// <param name="actionBeforeInsertion">Action that is called before elements are included into the partial view. Note that this action is executed only if items are directly added into the partial view,
    /// but it is not executed if items are indirectly included into the partial view by adding items that fullfil the selection criterium to the parent list.</param>
    /// <returns></returns>
    public IObservableList<M> CreatePartialViewOfType<M>(Func<M, bool> selectionCriterium, Action<M> actionBeforeInsertion) where M : T
    {
      var result = new PartialView<M>(this, x => (x is M mx) && selectionCriterium(mx), actionBeforeInsertion);
      _partialViews.AddLast(new WeakReference(result));
      return result;
    }

    #endregion Partial view creation

    #region Event management

    /// <summary>
    /// Notifies the partial views that have changed after each operation.
    /// </summary>
    ///
    protected void NotifyPartialViewsThatHaveChanged()
    {
      foreach (var pv in _partialViewsToNotify)
        pv.OnNotifyCollectionChanged();

      _partialViewsToNotify.Clear();
    }

    /// <summary>
    /// Gets a token that will temporarily disable the CollectionChanged events from this collection. The best practice is to use this token inside a using statement, because at the end
    /// of the using statement the Dispose function of the token is called automatically, which then reenables the events.
    /// </summary>
    /// <returns></returns>
    public ISuspendToken GetEventDisableToken()
    {
      return _eventState.Disable();
    }

    protected virtual void OnReenableEvents()
    {
      if (null != _pendingEvent)
      {
        var e = _pendingEvent;
        _pendingEvent = null;
        base.OnCollectionChanged(e);
      }
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (_eventState.IsDisabled)
      {
        if (null == _pendingEvent)
          _pendingEvent = e;
        else if (_pendingEvent.Action != NotifyCollectionChangedAction.Reset)
          _pendingEvent = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
      }
      else
      {
        if (null == _pendingEvent)
        {
          base.OnCollectionChanged(e);
        }
        else
        {
          _pendingEvent = null;
          base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
      }
    }

    #endregion Event management

    #region Some special list operations

    public void AddRange(IEnumerable<T> enumeration)
    {
      using (var token = _eventState.Disable())
      {
        foreach (var item in enumeration)
          Add(item);
      }
    }

    #endregion Some special list operations

    #region List change operation overrides

    protected override void ClearItems()
    {
      var node = _partialViews.First;
      while (null != node)
      {
        var pv = node.Value.Target as PartialViewBase;
        if (null != pv)
        {
          if (pv._itemIndex.Count > 0)
            _partialViewsToNotify.Add(pv);

          pv._itemIndex.Clear();
          node = node.Next;
        }
        else
        {
          var oldNode = node;
          _partialViews.Remove(node);
          node = node.Next;
        }
      }

      base.ClearItems();

      NotifyPartialViewsThatHaveChanged();
    }

    protected override void InsertItem(int index, T item)
    {
      _actionBeforeInsertion?.Invoke(item);

      for (var node = _partialViews.First; null != node; node = node.Next)
      {
        var pv = node.Value.Target as PartialViewBase;
        if (null != pv)
        {
          // Note: there is no need to notify the partial view if only the item indices were adjusted
          var itemIndex = pv._itemIndex;
          int i;
          for (i = itemIndex.Count - 1; i >= 0 && itemIndex[i] >= index; --i)
            ++itemIndex[i]; // adjust item indices

          if (pv._selectionCriterium(item))
          {
            itemIndex.Insert(i + 1, index);
            _partialViewsToNotify.Add(pv); // but of course when the item is inserted into the partial view, then the partial view must be notified
          }
        }
        else
        {
          var oldNode = node;
          _partialViews.Remove(node);
        }
      }

      base.InsertItem(index, item);

      NotifyPartialViewsThatHaveChanged();
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
      if (oldIndex != newIndex)
      {
        for (var node = _partialViews.First; null != node; node = node.Next)
        {
          if (node.Value.Target is PartialViewBase pv)
          {
            var itemIndex = pv._itemIndex;
            int i, j;
            int oIdx = FindIndexOfItemGreaterThanOrEqualTo(itemIndex, oldIndex);
            bool isItemIncluded = oIdx < itemIndex.Count && itemIndex[oIdx] == oldIndex;

            if (oldIndex < newIndex) // item is moved to higher index
            {
              if (isItemIncluded)
              {
                for (i = oIdx, j = oIdx + 1; j < itemIndex.Count && itemIndex[j] <= newIndex; ++i, ++j)
                  itemIndex[i] = itemIndex[j] - 1;
                itemIndex[i] = newIndex;
                _partialViewsToNotify.Add(pv);
              }
              else // item is not included - we only need to adjust the item indices
              {
                for (i = oIdx; i < itemIndex.Count && itemIndex[i] <= newIndex; ++i)
                  --itemIndex[i];
              }
            }
            else // item is moved to lower index
            {
              if (isItemIncluded)
              {
                for (i = oIdx, j = oIdx - 1; j >= 0 && itemIndex[j] >= newIndex; --i, --j)
                  itemIndex[i] = itemIndex[j] + 1;
                itemIndex[i] = newIndex;
                _partialViewsToNotify.Add(pv);
              }
              else // item is not included - we only need to adjust the item indices
              {
                for (i = oIdx - 1; i >= 0 && itemIndex[i] >= newIndex; --i)
                  ++itemIndex[i];
              }
            }
          }
          else
          {
            var oldNode = node;
            _partialViews.Remove(node);
          }
        }
      }

      base.MoveItem(oldIndex, newIndex);

      NotifyPartialViewsThatHaveChanged();
    }

    protected override void RemoveItem(int index)
    {
      for (var node = _partialViews.First; null != node; node = node.Next)
      {
        var pv = node.Value.Target as PartialViewBase;
        if (null != pv)
        {
          // Note: there is no need to notify the partial view if only the item indices were adjusted
          var itemIndex = pv._itemIndex;
          int i;
          for (i = itemIndex.Count - 1; i >= 0 && itemIndex[i] > index; --i)
            --itemIndex[i];

          if (i >= 0 && itemIndex[i] == index)
          {
            itemIndex.RemoveAt(i);
            _partialViewsToNotify.Add(pv); // but of course when the item is deleted into the partial view, then the partial view must be notified
          }
        }
        else
        {
          var oldNode = node;
          _partialViews.Remove(node);
        }
      }

      base.RemoveItem(index);

      NotifyPartialViewsThatHaveChanged();
    }

    protected override void SetItem(int index, T item)
    {
      _actionBeforeInsertion?.Invoke(item);

      for (var node = _partialViews.First; null != node; node = node.Next)
      {
        var pv = node.Value.Target as PartialViewBase;
        if (null != pv)
        {
          bool partialViewChanged = false;
          bool isIncluded = pv._selectionCriterium(item);
          var itemIndex = pv._itemIndex;
          int i = FindIndexOfItemGreaterThanOrEqualTo(itemIndex, index);
          if (i < itemIndex.Count)
          {
            if (itemIndex[i] == index)
            {
              if (!isIncluded)
              {
                itemIndex.RemoveAt(i);
                partialViewChanged = true;
              }
            }
            else
            {
              if (isIncluded)
              {
                itemIndex.Insert(i, index);
                partialViewChanged = true;
              }
            }
          }
          else // at the end of the collection
          {
            if (isIncluded)
            {
              itemIndex.Add(index);
              partialViewChanged = true;
            }
          }

          if (partialViewChanged) // if the item fulfills the selection criterion,
            _partialViewsToNotify.Add(pv); // the partial view must be notified in any case
        }
        else
        {
          var oldNode = node;
          _partialViews.Remove(node);
        }
      }

      base.SetItem(index, item);
      NotifyPartialViewsThatHaveChanged();
    }

    /// <summary>
    /// Finds the item in the list that than or equal to.
    /// </summary>
    /// <param name="itemIndex">List with the ordered list of item indices.</param>
    /// <param name="value">The index to find.</param>
    /// <returns></returns>
    private static int FindIndexOfItemGreaterThanOrEqualTo(IList<int> itemIndex, int value)
    {
      int upperIndex = itemIndex.Count - 1;
      if (upperIndex < 0)
        return 0;

      int lowerIndex = 0;
      int upperValue = itemIndex[upperIndex];
      int lowerValue = itemIndex[lowerIndex];

      while (lowerValue < value && value < upperValue && (lowerIndex + 1) < upperIndex)
      {
        int middleIndex = upperIndex - ((upperIndex - lowerIndex) / 2);
        int middleValue = itemIndex[middleIndex];
        if (middleValue < value)
        {
          lowerIndex = middleIndex;
          lowerValue = middleValue;
        }
        else
        {
          upperIndex = middleIndex;
          upperValue = middleValue;
        }
      }
      return value <= lowerValue ? lowerIndex : (value > upperValue ? upperIndex + 1 : upperIndex);
    }

    #endregion List change operation overrides

    #region EventDisabler

    public interface ISuspendToken : IDisposable
    {
      void Resume();

      void ResumeSilently();
    }

    /// <summary>
    /// Helper class to temporarily disable something, e.g. some events. By calling <see cref="Disable"/> one gets a disposable token, that,
    /// when disposed, enables again, which fires then the action that is given as parameter to the constructor. It is possible to make nested calls to <see cref="Disable"/>. In this case all tokens
    /// must be disposed before the <see cref="IsEnabled"/> is again <c>true</c> and the re-enabling action is fired.
    /// </summary>
    protected class TemporaryDisabler
    {
      #region Inner class SuppressToken

      private class SuspendToken : ISuspendToken
      {
        private TemporaryDisabler? _parent;

        internal SuspendToken(TemporaryDisabler parent)
        {
          var suspendLevel = System.Threading.Interlocked.Increment(ref parent._suspendLevel);
          _parent = parent;
        }

        ~SuspendToken()
        {
          Dispose();
        }

        /// <summary>
        /// Disarms this SuppressToken so that it can not raise the resume event anymore.
        /// </summary>
        public void ResumeSilently()
        {
          var parent = System.Threading.Interlocked.Exchange<TemporaryDisabler?>(ref _parent, null);
          if (parent != null)
          {
            int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

            if (0 == newLevel)
            {
              try
              {
                parent.OnResumeSilently();
              }
              finally
              {
              }
            }
            else if (newLevel < 0)
            {
              throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
            }
          }
        }

        public void Resume()
        {
          Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
          var parent = System.Threading.Interlocked.Exchange<TemporaryDisabler?>(ref _parent, null);
          if (parent != null)
          {
            int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

            if (0 == newLevel)
            {
              try
              {
                parent.OnResume();
              }
              finally
              {
              }
            }
            else if (newLevel < 0)
            {
              throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
            }
          }
        }

        #endregion IDisposable Members
      }

      #endregion Inner class SuppressToken

      /// <summary>How many times was the <see cref="Disable"/> function called (without disposing the tokens got in these calls)</summary>
      private int _suspendLevel;

      /// <summary>Action that is taken when the suppress levels falls down to zero and the event count is equal to or greater than one (i.e. during the suspend phase, at least an event had occured).</summary>
      private Action _reenablingEventHandler;

      /// <summary>
      /// Constructor. You have to provide a callback function, that is been called when the event handling resumes.
      /// </summary>
      /// <param name="reenablingEventHandler">The callback function called when the events resume. See remarks when the callback function is called.</param>
      /// <remarks>The callback function is called only (i) if the event resumes (exactly: the _suppressLevel changes from 1 to 0),
      /// and (ii) in that moment the _eventCount is &gt;0.
      /// To get the _eventCount&gt;0, someone must call either GetEnabledWithCounting or GetDisabledWithCounting
      /// during the suspend period.</remarks>
      public TemporaryDisabler(Action reenablingEventHandler)
      {
        _reenablingEventHandler = reenablingEventHandler;
      }

      /// <summary>
      /// Increase the SuspendLevel.
      /// </summary>
      /// <returns>An object, which must be disposed in order to re-enabling again.
      /// The most convenient way is to use a using statement with this function call
      /// </returns>
      public ISuspendToken Disable()
      {
        return new SuspendToken(this);
      }

      /// <summary>
      /// Returns true when the disabling level is equal to zero (initial state).
      /// Otherwise false.
      /// </summary>
      public bool IsEnabled { get { return _suspendLevel == 0; } }

      /// <summary>
      /// Returns true when the disabling level is greater than zero (after calling the <see cref="Disable"/> function).
      /// Returns false if the disabling level is zero.
      /// </summary>
      public bool IsDisabled { get { return _suspendLevel != 0; } }

      /// <summary>
      /// Just fires the reenabling action that was given in the constructor,
      /// without changing the disabling level.
      /// </summary>
      public void ReenableShortly()
      {
        OnResume();
      }

      /// <summary>
      /// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
      /// Per default, the resume event handler is called that you provided in the constructor.
      /// </summary>
      protected virtual void OnResume()
      {
        _reenablingEventHandler?.Invoke();
      }

      /// <summary>
      /// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
      /// Per default, the resume event handler is called that you provided in the constructor.
      /// </summary>
      protected virtual void OnResumeSilently()
      {
      }
    }

    #endregion EventDisabler
  }
}
