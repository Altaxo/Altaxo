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

using System;
using Altaxo.Main;
using Xunit;

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for DoubleColumn_Test.
  /// </summary>

  public class DoubleColumn_Test
  {
    [Fact]
    public void ZeroElements()
    {
      var d = new DoubleColumn();

      Assert.Empty(d);
      Assert.False(d.IsDirty);
      Assert.True(d.IsElementEmpty(0));
      Assert.True(d.IsElementEmpty(1));
    }

    [Fact]
    public void TenEmptyElements()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);
      for (int i = 0; i < 11; i++)
        Assert.True(d.IsElementEmpty(i));
    }

    [Fact]
    public void TenElementsFirstFilled()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      d[0] = 77.0;
      Assert.Single(d);
      Assert.False(d.IsDirty);
      Assert.False(d.IsElementEmpty(0));
      Assert.True(d.IsElementEmpty(1));

      // now delete again element 0
      d[0] = double.NaN;
      Assert.Empty(d);
      Assert.False(d.IsDirty);
      for (int i = 0; i < 11; i++)
        Assert.True(d.IsElementEmpty(i));

      Assert.False(d.IsDirty);
    }

    [Fact]
    public void FiveElements89Filled()
    {
      var d = new DoubleColumn(5);
      Assert.Empty(d);
      d[8] = 77.0;
      d[9] = 88;
      Assert.Equal(10, d.Count);
      Assert.False(d.IsDirty);

      Assert.True(d.IsElementEmpty(7));
      Assert.False(d.IsElementEmpty(8));
      Assert.False(d.IsElementEmpty(9));
      Assert.True(d.IsElementEmpty(10));

      d[9] = double.NaN;

      Assert.Equal(9, d.Count);
      Assert.True(d.IsElementEmpty(7));
      Assert.False(d.IsElementEmpty(8));
      Assert.True(d.IsElementEmpty(9));
      Assert.True(d.IsElementEmpty(10));

      d[8] = double.NaN;
      Assert.Empty(d);
      Assert.True(d.IsElementEmpty(7));
      Assert.True(d.IsElementEmpty(8));
      Assert.True(d.IsElementEmpty(9));
      Assert.True(d.IsElementEmpty(10));

      Assert.False(d.IsDirty);
      for (int i = 0; i < 11; i++)
        Assert.True(d.IsElementEmpty(i));
    }

    private class MyColumnParent : Altaxo.Main.IDocumentNode
    {
      public EventHandler ChildChanged;
      protected int _CallCount = 0;
      protected string _identifier;
      private Altaxo.Main.ISuspendToken _suspendToken;

      public MyColumnParent(string identifier)
      {
        _identifier = identifier;
      }

      public void Reset()
      {
        ChildChanged = null;
        _CallCount = 0;
      }

      public void Resume()
      {
        if (_suspendToken is not null)
        {
          _suspendToken.Resume();
          _suspendToken = null;
        }
      }

      public int CallCount { get { return _CallCount; } }

      public void EhChildChanged(object sender, EventArgs e)
      {
        if (ChildChanged is null && sender is SuspendableDocumentLeafNode n && n.IsDisposeInProgress)
          return; // ignore Dispose messages coming from the Column after the test has finished

        Assert.NotNull(ChildChanged); // "Test redirector for OnChildChange must not be null!");
        ChildChanged(sender, e);
      }

      public void EhParentTunnelingEventHappened(IDocumentNode sender, IDocumentNode originalSource, TunnelingEventArgs e)
      {
      }

      public void DontCareNotSuspend(object sender, EventArgs e)
      {
      }

      public Altaxo.Main.ISuspendToken DontCareAndSuspend(object sender, EventArgs e)
      {
        return ((Altaxo.Main.ISuspendableByToken)sender).SuspendGetToken();
      }

      public void ExpectingNotToBeCalledBecauseSuspended(object sender, EventArgs e)
      {
        Assert.True(false, "This must not be called, since the sender should be suspended!");
      }

      public void ExpectingNotToBeCalledBecauseNoChange(object sender, EventArgs e)
      {
        Assert.True(false, "This must not be called, since no data change is expected!");
      }

      public void TestParentAddNotification(object sender, EventArgs e)
      {
        Assert.NotNull(sender);
        Assert.NotNull(e);
        Assert.True(e is Altaxo.Main.ParentChangedEventArgs);
        var ea = (Altaxo.Main.ParentChangedEventArgs)e;
        Assert.Null(ea.OldParent);
        Assert.Equal(this, ea.NewParent);
        _CallCount++;
        ;
      }

      public void TestParentRemoveNotification(object sender, EventArgs e)
      {
        Assert.NotNull(sender);
        Assert.NotNull(e);
        Assert.True(e is Altaxo.Main.ParentChangedEventArgs);
        var ea = (Altaxo.Main.ParentChangedEventArgs)e;
        Assert.Equal(this, ea.OldParent);
        Assert.Null(ea.NewParent);
        _CallCount++;
        ;
      }

      public void TestSetSlot5(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(5, ea.MinRowChanged);
        Assert.Equal(6, ea.MaxRowChanged);
        Assert.False(ea.HasRowCountDecreased);
        _CallCount++;
        ;
      }

      public void TestResetSlot5(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(5, ea.MinRowChanged);
        Assert.Equal(6, ea.MaxRowChanged);
        Assert.True(ea.HasRowCountDecreased);
        _CallCount++;
        ;
      }

      public void TestSetSlot7AndSuspend(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(7, ea.MinRowChanged);
        Assert.Equal(8, ea.MaxRowChanged);
        Assert.False(ea.HasRowCountDecreased);
        _CallCount++;
        ;
        _suspendToken = ((Altaxo.Main.ISuspendableByToken)sender).SuspendGetToken();
      }

      public void CheckChange0To12_Decreased(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(0, ea.MinRowChanged);
        Assert.Equal(13, ea.MaxRowChanged);
        Assert.True(ea.HasRowCountDecreased);
        _CallCount++;
        ;
      }

      public void ExpectingDataChange5To12_NoDecrease(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(5, ea.MinRowChanged);
        Assert.Equal(13, ea.MaxRowChanged);
        Assert.False(ea.HasRowCountDecreased);
        _CallCount++;
        ;
      }

      public void ExpectingDataChange0To12_NoDecrease(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(0, ea.MinRowChanged);
        Assert.Equal(13, ea.MaxRowChanged);
        Assert.False(ea.HasRowCountDecreased);
        _CallCount++;
        ;
      }

      public void ExpectingDataChange9To12_NoDecrease(object sender, EventArgs e)
      {
        Assert.True(sender is not null, "Sender must be the column");
        Assert.Equal(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.True(e is not null, "Awaiting valid data change event args");
        if (e is not Altaxo.Data.DataColumnChangedEventArgs ea)
        {
          throw new Xunit.Sdk.XunitException("Expected DataColumnChangedEventArgs");
        }
        Assert.Equal(9, ea.MinRowChanged);
        Assert.Equal(13, ea.MaxRowChanged);
        Assert.False(ea.HasRowCountDecreased);
        _CallCount++;
        ;
      }

      public string Name
      {
        get { throw new NotImplementedException(); }
      }

      public bool TryGetName(out string name)
      {
        throw new NotImplementedException();
      }

      public event EventHandler Changed { add { } remove { } }

      public Altaxo.Main.ISuspendToken SuspendGetToken()
      {
        throw new NotImplementedException();
      }

      public bool IsSuspended
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public Altaxo.Main.IDocumentNode ParentObject
      {
        get
        {
          throw new NotImplementedException();
        }
        set
        {
          throw new NotImplementedException();
        }
      }

      public IDocumentLeafNode GetChildObjectNamed(string name)
      {
        throw new NotImplementedException();
      }

      public string GetNameOfChildObject(IDocumentLeafNode o)
      {
        throw new NotImplementedException();
      }

      public event Action<object, object, TunnelingEventArgs> TunneledEvent { add { } remove { } }

      public System.Collections.Generic.IEnumerable<IDocumentLeafNode> ChildNodes
      {
        get { throw new NotImplementedException(); }
      }

      public IDocumentLeafNode ParentNode
      {
        get { throw new NotImplementedException(); }
      }

      public void Dispose()
      {
        throw new NotImplementedException();
      }

      public bool IsDisposed
      {
        get { throw new NotImplementedException(); }
      }

      public bool IsDisposeInProgress
      {
        get { throw new NotImplementedException(); }
      }

      public void SetDisposeInProgress()
      {
        throw new NotImplementedException();
      }
    }

    [Fact]
    public void ParentNotification()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);

      // testing parent change notification
      var parent = new MyColumnParent(nameof(ParentNotification));
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.Equal(1, parent.CallCount); // "There was no parent add notification");
      parent.Reset();

      // testing parent change notification resetting parent
      parent.ChildChanged = new EventHandler(parent.TestParentRemoveNotification);
      d.ParentObject = null;
      Assert.Equal(1, parent.CallCount); // "There was no parent remove notification");
      parent.Reset();


    }

    [Fact]
    public void DataChangeNotification()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);

      // testing parent change notification
      var parent = new MyColumnParent(nameof(DataChangeNotification));
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.Equal(1, parent.CallCount); // "There was no parent add notification");
      parent.Reset();

      // test data change notification setting row5
      parent.ChildChanged = new EventHandler(parent.TestSetSlot5);
      d.Changed += new EventHandler(parent.TestSetSlot5);
      d[5] = 66;
      Assert.Equal(2, parent.CallCount); // "There was no data change notification setting d[5]");
      d.Changed -= new EventHandler(parent.TestSetSlot5);
      parent.Reset();

      // testing data change notification resetting row5 -> count drops to zero
      parent.ChildChanged = new EventHandler(parent.TestResetSlot5);
      d[5] = double.NaN;
      Assert.Equal(1, parent.CallCount); // "There was no data change notification setting d[5]");
      parent.Reset();

      // Test d[7] setting to NaN -> the column must not rise a change event
      parent.ChildChanged = new EventHandler(parent.TestSetSlot7AndSuspend);
      d[7] = double.NaN;
      Assert.Equal(0, parent.CallCount); // "There is no data change notification expected setting d[7] to invalid");
      Assert.False(d.IsDirty);
      parent.Reset();

      // Set slot7 returning true -> the column must suspend
      parent.ChildChanged = new EventHandler(parent.TestSetSlot7AndSuspend);
      d.Changed += new EventHandler(parent.TestSetSlot7AndSuspend);
      d[7] = 88;
      Assert.Equal(1, parent.CallCount); // "There was no data change notification setting d[7]");
      Assert.True(d.IsDirty);
      d.Changed -= new EventHandler(parent.TestSetSlot7AndSuspend);
      parent.Reset();

      // now the column must not be call the OnChildChange function,
      // but accumulate the changed
      parent.ChildChanged = new EventHandler(parent.ExpectingNotToBeCalledBecauseSuspended);
      d.Changed += new EventHandler(parent.ExpectingNotToBeCalledBecauseSuspended);
      d[0] = 3;
      d[7] = double.NaN; // to force the row count has decreased
      d[12] = 22;
      d[14] = double.NaN;
      d.Changed -= new EventHandler(parent.ExpectingNotToBeCalledBecauseSuspended);

      // the accumulated state should now be row 0 - 12, and the row count decreased

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.CheckChange0To12_Decreased);
      d.Changed += new EventHandler(parent.CheckChange0To12_Decreased);
      parent.Resume();
      Assert.Equal(2, parent.CallCount); // "There was no data change notification setting d[0] to d[12]");
      Assert.False(d.IsDirty);
      d.Changed -= new EventHandler(parent.CheckChange0To12_Decreased);
      parent.Reset();
    }

    [Fact]
    public void RowInsertionAtTheBeginning()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);

      // set rows 0 to 9 to i
      for (int i = 0; i < 10; i++)
        d[i] = i;
      Assert.Equal(10, d.Count);

      // testing parent change notification setting the parent
      var parent = new MyColumnParent(nameof(RowInsertionAtTheBeginning));
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.Equal(1, parent.CallCount); // "There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingDataChange0To12_NoDecrease);
      // now insert
      d.InsertRows(0, 3);
      Assert.Equal(1, parent.CallCount); // "There was no data change notification");

      // test the data
      Assert.Equal(13, d.Count);

      for (int i = 0; i < 3; i++)
        Assert.Equal(double.NaN, d[i]);

      for (int i = 3; i < (10 + 3); i++)
        Assert.Equal(i - 3, d[i]);

      Assert.Equal(double.NaN, d[13]);

      parent.Reset(); // avoid that Dispose event args are sent
    }

    [Fact]
    public void RowInsertionInTheMiddle()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);

      // set rows 0 to 9 to i
      for (int i = 0; i < 10; i++)
        d[i] = i;
      Assert.Equal(10, d.Count);

      // testing parent change notification setting the parent
      var parent = new MyColumnParent(nameof(RowInsertionInTheMiddle));
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.Equal(1, parent.CallCount); // "There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingDataChange5To12_NoDecrease);
      // now insert
      d.InsertRows(5, 3);
      Assert.Equal(1, parent.CallCount); // "There was no data change notification");

      // test the data
      Assert.Equal(13, d.Count);

      for (int i = 0; i < 5; i++)
        Assert.Equal(i, d[i]);

      Assert.Equal(double.NaN, d[5]);
      Assert.Equal(double.NaN, d[6]);
      Assert.Equal(double.NaN, d[7]);

      for (int i = 8; i < (8 + 5); i++)
        Assert.Equal(i - 3, d[i]);

      parent.Reset(); // avoid that Dispose event args are sent
    }

    [Fact]
    public void RowInsertionOneBeforeEnd()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);

      // set rows 0 to 9 to i
      for (int i = 0; i < 10; i++)
        d[i] = i;
      Assert.Equal(10, d.Count);

      // testing parent change notification setting the parent
      var parent = new MyColumnParent(nameof(RowInsertionOneBeforeEnd));
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.Equal(1, parent.CallCount); // "There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingDataChange9To12_NoDecrease);
      // now insert
      d.InsertRows(9, 3);
      Assert.Equal(1, parent.CallCount); // "There was no data change notification");

      // test the data
      Assert.Equal(13, d.Count);

      for (int i = 0; i < 9; i++)
        Assert.Equal(i, d[i]);

      Assert.Equal(double.NaN, d[9]);
      Assert.Equal(double.NaN, d[10]);
      Assert.Equal(double.NaN, d[11]);

      for (int i = 12; i < 13; i++)
        Assert.Equal(i - 3, d[i]);

      parent.Reset(); // avoid that Dispose event args are sent
    }

    [Fact]
    public void RowInsertionAtTheEnd()
    {
      var d = new DoubleColumn(10);
      Assert.Empty(d);
      Assert.False(d.IsDirty);

      // set rows 0 to 9 to i
      for (int i = 0; i < 10; i++)
        d[i] = i;
      Assert.Equal(10, d.Count);

      // testing parent change notification setting the parent
      var parent = new MyColumnParent(nameof(RowInsertionAtTheEnd));
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.Equal(1, parent.CallCount); // "There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingNotToBeCalledBecauseNoChange);
      // now insert
      d.InsertRows(10, 3);

      // test the data
      Assert.Equal(10, d.Count);

      for (int i = 0; i < 10; i++)
        Assert.Equal(i, d[i]);

      Assert.Equal(double.NaN, d[10]);

      parent.Reset(); // avoid that Dispose event args are sent
    }
  }
}
