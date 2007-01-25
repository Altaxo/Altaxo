#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using NUnit.Framework;
using Altaxo.Data;

namespace AltaxoTest.Data
{
  /// <summary>
  /// Summary description for DoubleColumn_Test.
  /// </summary>
  [TestFixture]
  public class DoubleColumn_Test 
  {
        
    [Test]
    public void ZeroElements()
    {
      DoubleColumn d = new DoubleColumn();
      
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
      Assert.AreEqual(true,d.IsElementEmpty(0));
      Assert.AreEqual(true,d.IsElementEmpty(1));       
    }

    [Test]
    public void TenEmptyElements()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false, d.IsDirty);
      for(int i=0;i<11;i++)
        Assert.AreEqual(true, d.IsElementEmpty(i));
    }

    [Test]
    public void TenElementsFirstFilled()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0, d.Count);
      d[0]=77.0;
      Assert.AreEqual(1, d.Count);
      Assert.AreEqual(false, d.IsDirty);
      Assert.AreEqual(false, d.IsElementEmpty(0));
      Assert.AreEqual(true, d.IsElementEmpty(1));

      // now delete again element 0
      d[0]=double.NaN;
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
      for(int i=0;i<11;i++)
        Assert.AreEqual(true,d.IsElementEmpty(i));

      Assert.AreEqual(false, d.IsDirty);
    }

    [Test]
    public void FiveElements89Filled()
    {
      DoubleColumn d = new DoubleColumn(5);
      Assert.AreEqual(0,d.Count);
      d[8]=77.0;
      d[9]=88;
      Assert.AreEqual(10,d.Count);
      Assert.AreEqual(false, d.IsDirty);

      Assert.AreEqual(true,d.IsElementEmpty(7));
      Assert.AreEqual(false,d.IsElementEmpty(8));
      Assert.AreEqual(false,d.IsElementEmpty(9));
      Assert.AreEqual(true,d.IsElementEmpty(10));

      d[9]=double.NaN;

      Assert.AreEqual(9,d.Count, 9);
      Assert.AreEqual(true,d.IsElementEmpty(7));
      Assert.AreEqual(false,d.IsElementEmpty(8));
      Assert.AreEqual(true,d.IsElementEmpty(9));
      Assert.AreEqual(true,d.IsElementEmpty(10));

      d[8]=double.NaN;
      Assert.AreEqual(0,d.Count, 0);
      Assert.AreEqual(true,d.IsElementEmpty(7));
      Assert.AreEqual(true,d.IsElementEmpty(8));
      Assert.AreEqual(true,d.IsElementEmpty(9));
      Assert.AreEqual(true,d.IsElementEmpty(10));

      Assert.AreEqual(false,d.IsDirty);
      for(int i=0;i<11;i++)
        Assert.AreEqual(true, d.IsElementEmpty(i));
    }


    class MyColumnParent : Altaxo.Main.IChildChangedEventSink
    {
      public EventHandler ChildChanged; 
      protected int _CallCount = 0;

      public void Reset()
      {
        ChildChanged = null;
        _CallCount=0;
      }

      public int CallCount { get { return _CallCount; } }

      public void EhChildChanged(object sender, EventArgs e)
      {
        Assert.IsNotNull(ChildChanged,"Test redirector for OnChildChange must not be null!");
        ChildChanged(sender,e);
      }


      public void DontCareNotSuspend(object sender, EventArgs e)
      {
      }

      public void DontCareAndSuspend(object sender, EventArgs e)
      {
        ((Altaxo.Main.ISuspendable)sender).Suspend();
      }

      public void ExpectingNotToBeCalledBecauseSuspended(object sender, EventArgs e)
      {
        Assert.Fail("This must not be called, since the sender should be suspended!");
      }

      public void ExpectingNotToBeCalledBecauseNoChange(object sender, EventArgs e)
      {
        Assert.Fail("This must not be called, since no data change is expected!");
      }

      public void TestParentAddNotification(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender);
        Assert.IsNotNull(e);
        Assert.IsTrue(e is Altaxo.Main.ParentChangedEventArgs);
        Altaxo.Main.ParentChangedEventArgs ea = (Altaxo.Main.ParentChangedEventArgs)e;
        Assert.AreEqual(null,ea.OldParent);
        Assert.AreEqual(this,ea.NewParent);
        _CallCount++;;
      }

      public void TestParentRemoveNotification(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender);
        Assert.IsNotNull(e);
        Assert.IsTrue(e is Altaxo.Main.ParentChangedEventArgs);
        Altaxo.Main.ParentChangedEventArgs ea = (Altaxo.Main.ParentChangedEventArgs)e;
        Assert.AreEqual(this,ea.OldParent);
        Assert.AreEqual(null,ea.NewParent);
        _CallCount++;;
      }

      public void TestSetSlot5(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(5, ea.MinRowChanged);
        Assert.AreEqual(6, ea.MaxRowChanged);
        Assert.AreEqual(false, ea.RowCountDecreased);
        _CallCount++;;
        
      }

      public void TestResetSlot5(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(5, ea.MinRowChanged);
        Assert.AreEqual(6, ea.MaxRowChanged);
        Assert.AreEqual(true, ea.RowCountDecreased);
        _CallCount++;;
        
      }


      public void TestSetSlot7AndSuspend(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(7, ea.MinRowChanged);
        Assert.AreEqual(8, ea.MaxRowChanged);
        Assert.AreEqual(false, ea.RowCountDecreased);
        _CallCount++;;
        ((Altaxo.Main.ISuspendable)sender).Suspend();
      }


      public void CheckChange0To12_Decreased(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(0, ea.MinRowChanged);
        Assert.AreEqual(13, ea.MaxRowChanged);
        Assert.AreEqual(true, ea.RowCountDecreased);
        _CallCount++;;
          
      }




      public void ExpectingDataChange5To12_NoDecrease(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(5, ea.MinRowChanged);
        Assert.AreEqual(13, ea.MaxRowChanged);
        Assert.AreEqual(false, ea.RowCountDecreased);
        _CallCount++;;
        
      }

      public void ExpectingDataChange0To12_NoDecrease(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(0, ea.MinRowChanged);
        Assert.AreEqual(13, ea.MaxRowChanged);
        Assert.AreEqual(false, ea.RowCountDecreased);
        _CallCount++;;
          
      }

      public void ExpectingDataChange9To12_NoDecrease(object sender, EventArgs e)
      {
        Assert.IsNotNull(sender,"Sender must be the column");
        Assert.AreEqual(typeof(Altaxo.Data.DoubleColumn), sender.GetType());
        Assert.IsNotNull(e,"Awaiting valid data change event args");
        Assert.AreEqual(typeof(Altaxo.Data.DataColumn.ChangeEventArgs), e.GetType());
        Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
        Assert.AreEqual(9, ea.MinRowChanged);
        Assert.AreEqual(13, ea.MaxRowChanged);
        Assert.AreEqual(false, ea.RowCountDecreased);
        _CallCount++;;
        
      }

    }



    [Test]
    public void ParentNotification()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
        
      // testing parent change notification
      MyColumnParent parent = new MyColumnParent();
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.AreEqual(1,parent.CallCount,"There was no parent add notification");
      parent.Reset();

      // testing parent change notification resetting parent
      parent.ChildChanged = new EventHandler(parent.TestParentRemoveNotification);
      d.ParentObject = null;
      Assert.AreEqual(1,parent.CallCount,"There was no parent remove notification");
      parent.Reset();
    }

    [Test]
    public void DataChangeNotification()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
        
      // testing parent change notification
      MyColumnParent parent = new MyColumnParent();
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.AreEqual(1,parent.CallCount,"There was no parent add notification");
      parent.Reset();
      
      
      // test data change notification setting row5
      parent.ChildChanged = new EventHandler(parent.TestSetSlot5);
      d.Changed += new EventHandler(parent.TestSetSlot5);
      d[5]=66;
      Assert.AreEqual(2,parent.CallCount,"There was no data change notification setting d[5]");
      d.Changed -= new EventHandler(parent.TestSetSlot5);
      parent.Reset();

      // testing data change notification resetting row5 -> count drops to zero
      parent.ChildChanged = new EventHandler(parent.TestResetSlot5);
      d[5]=double.NaN;
      Assert.AreEqual(1,parent.CallCount,"There was no data change notification setting d[5]");
      parent.Reset();


      // Test d[7] setting to NaN -> the column must not rise a change event
      parent.ChildChanged = new EventHandler(parent.TestSetSlot7AndSuspend);
      d[7]=double.NaN;
      Assert.AreEqual(0,parent.CallCount,"There is no data change notification expected setting d[7] to invalid");
      Assert.AreEqual(false, d.IsDirty);
      parent.Reset();

      // Set slot7 returning true -> the column must suspend
      parent.ChildChanged = new EventHandler(parent.TestSetSlot7AndSuspend);
      d.Changed += new EventHandler(parent.TestSetSlot7AndSuspend);
      d[7]=88;
      Assert.AreEqual(1,parent.CallCount,"There was no data change notification setting d[7]");
      Assert.AreEqual(true, d.IsDirty);
      d.Changed -= new EventHandler(parent.TestSetSlot7AndSuspend);
      parent.Reset();



      // now the column must not be call the OnChildChange function,
      // but accumulate the changed
      parent.ChildChanged = new EventHandler(parent.ExpectingNotToBeCalledBecauseSuspended);
      d.Changed += new EventHandler(parent.ExpectingNotToBeCalledBecauseSuspended);
      d[0]=3;
      d[7]=double.NaN; // to force the row count has decreased
      d[12]=22;
      d[14]=double.NaN;
      d.Changed -= new EventHandler(parent.ExpectingNotToBeCalledBecauseSuspended);

      // the accumulated state should now be row 0 - 12, and the row count decreased

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.CheckChange0To12_Decreased);
      d.Changed += new EventHandler(parent.CheckChange0To12_Decreased);
      d.Resume();
      Assert.AreEqual(2,parent.CallCount,"There was no data change notification setting d[0] to d[12]");
      Assert.AreEqual(false, d.IsDirty);
      d.Changed -= new EventHandler(parent.CheckChange0To12_Decreased);
      parent.Reset();
    }


    [Test]
    public void RowInsertionAtTheBeginning()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
        
      // set rows 0 to 9 to i
      for(int i=0;i<10;i++)
        d[i]=i;
      Assert.AreEqual(10, d.Count);


      // testing parent change notification setting the parent
      MyColumnParent parent = new MyColumnParent();
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.AreEqual(1,parent.CallCount,"There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingDataChange0To12_NoDecrease);
      // now insert
      d.InsertRows(0,3);
      Assert.AreEqual(1,parent.CallCount,"There was no data change notification");

      // test the data
      Assert.AreEqual(13, d.Count);

      for(int i=0;i<3;i++)
        Assert.AreEqual(double.NaN, d[i]);

      for(int i=3;i<(10+3);i++)
        Assert.AreEqual(i - 3, d[i]);

      Assert.AreEqual(double.NaN, d[13]);
    }


    [Test]
    public void RowInsertionInTheMiddle()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
        
      // set rows 0 to 9 to i
      for(int i=0;i<10;i++)
        d[i]=i;
      Assert.AreEqual(10, d.Count);

      // testing parent change notification setting the parent
      MyColumnParent parent = new MyColumnParent();
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.AreEqual(1,parent.CallCount,"There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingDataChange5To12_NoDecrease);
      // now insert
      d.InsertRows(5,3);
      Assert.AreEqual(1,parent.CallCount,"There was no data change notification");

      // test the data
      Assert.AreEqual(13, d.Count);

      for(int i=0;i<5;i++)
        Assert.AreEqual(i, d[i]);

      Assert.AreEqual(double.NaN, d[5]);
      Assert.AreEqual(double.NaN, d[6]);
      Assert.AreEqual(double.NaN, d[7]);

      for(int i=8;i<(8+5);i++)
        Assert.AreEqual(i - 3, d[i]);
    }

    [Test]
    public void RowInsertionOneBeforeEnd()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
        
      // set rows 0 to 9 to i
      for(int i=0;i<10;i++)
        d[i]=i;
      Assert.AreEqual(10, d.Count);

      // testing parent change notification setting the parent
      MyColumnParent parent = new MyColumnParent();
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.AreEqual(1,parent.CallCount,"There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingDataChange9To12_NoDecrease);
      // now insert
      d.InsertRows(9,3);
      Assert.AreEqual(1,parent.CallCount,"There was no data change notification");

      // test the data
      Assert.AreEqual(13, d.Count);

      for(int i=0;i<9;i++)
        Assert.AreEqual(i, d[i]);

      Assert.AreEqual(double.NaN, d[9]);
      Assert.AreEqual(double.NaN, d[10]);
      Assert.AreEqual(double.NaN, d[11]);

      for(int i=12;i<13;i++)
        Assert.AreEqual(i - 3, d[i]);
    }

    [Test]
    public void RowInsertionAtTheEnd()
    {
      DoubleColumn d = new DoubleColumn(10);
      Assert.AreEqual(0,d.Count);
      Assert.AreEqual(false,d.IsDirty);
        
      // set rows 0 to 9 to i
      for(int i=0;i<10;i++)
        d[i]=i;
      Assert.AreEqual(10, d.Count);

      // testing parent change notification setting the parent
      MyColumnParent parent = new MyColumnParent();
      parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
      d.ParentObject = parent;
      Assert.AreEqual(1,parent.CallCount,"There was no parent add notification");
      parent.Reset();

      parent.Reset();
      parent.ChildChanged = new EventHandler(parent.ExpectingNotToBeCalledBecauseNoChange);
      // now insert
      d.InsertRows(10,3);
    
      // test the data
      Assert.AreEqual(10, d.Count);

      for(int i=0;i<10;i++)
        Assert.AreEqual(i, d[i]);

      Assert.AreEqual(double.NaN, d[10]);
    }

  }
}
