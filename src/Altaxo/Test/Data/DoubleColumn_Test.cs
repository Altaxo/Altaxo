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
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				Assertion.AssertEquals(d.IsElementEmpty(0),true);
				Assertion.AssertEquals(d.IsElementEmpty(1),true);				
			}

			[Test]
			public void TenEmptyElements()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				for(int i=0;i<11;i++)
					Assertion.AssertEquals(d.IsElementEmpty(i),true);
			}

			[Test]
			public void TenElementsFirstFilled()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				d[0]=77.0;
				Assertion.AssertEquals(d.Count,1);
				Assertion.AssertEquals(false,d.IsDirty);
				Assertion.AssertEquals(d.IsElementEmpty(0),false);
				Assertion.AssertEquals(d.IsElementEmpty(1),true);

				// now delete again element 0
				d[0]=double.NaN;
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				for(int i=0;i<11;i++)
					Assertion.AssertEquals(d.IsElementEmpty(i),true);
			
				Assertion.AssertEquals(false,d.IsDirty);
			}

			[Test]
			public void FiveElements89Filled()
			{
				DoubleColumn d = new DoubleColumn(5);
				Assertion.AssertEquals(d.Count,0);
				d[8]=77.0;
				d[9]=88;
				Assertion.AssertEquals(d.Count,10);
				Assertion.AssertEquals(false,d.IsDirty);

				Assertion.AssertEquals(d.IsElementEmpty(7),true);
				Assertion.AssertEquals(d.IsElementEmpty(8),false);
				Assertion.AssertEquals(d.IsElementEmpty(9),false);
				Assertion.AssertEquals(d.IsElementEmpty(10),true);

				d[9]=double.NaN;

				Assertion.AssertEquals(d.Count,9);
				Assertion.AssertEquals(d.IsElementEmpty(7),true);
				Assertion.AssertEquals(d.IsElementEmpty(8),false);
				Assertion.AssertEquals(d.IsElementEmpty(9),true);
				Assertion.AssertEquals(d.IsElementEmpty(10),true);

				d[8]=double.NaN;
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsElementEmpty(7),true);
				Assertion.AssertEquals(d.IsElementEmpty(8),true);
				Assertion.AssertEquals(d.IsElementEmpty(9),true);
				Assertion.AssertEquals(d.IsElementEmpty(10),true);

				Assertion.AssertEquals(d.IsDirty,false);
				for(int i=0;i<11;i++)
					Assertion.AssertEquals(d.IsElementEmpty(i),true);
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

				public void OnChildChanged(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Test redirector for OnChildChange must not be null!", ChildChanged);
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
					Assertion.Fail("This must not be called, since the sender should be suspended!");
				}

				public void ExpectingNotToBeCalledBecauseNoChange(object sender, EventArgs e)
				{
					Assertion.Fail("This must not be called, since no data change is expected!");
				}

				public void TestParentAddNotification(object sender, EventArgs e)
				{
					Assertion.AssertNotNull(sender);
					Assertion.AssertNotNull(e);
					Assertion.Assert(e is Altaxo.Main.ParentChangedEventArgs);
					Altaxo.Main.ParentChangedEventArgs ea = (Altaxo.Main.ParentChangedEventArgs)e;
					Assertion.AssertEquals(null,ea.OldParent);
					Assertion.AssertEquals(this,ea.NewParent);
					_CallCount++;;
				}

				public void TestParentRemoveNotification(object sender, EventArgs e)
				{
					Assertion.AssertNotNull(sender);
					Assertion.AssertNotNull(e);
					Assertion.Assert(e is Altaxo.Main.ParentChangedEventArgs);
					Altaxo.Main.ParentChangedEventArgs ea = (Altaxo.Main.ParentChangedEventArgs)e;
					Assertion.AssertEquals(this,ea.OldParent);
					Assertion.AssertEquals(null,ea.NewParent);
					_CallCount++;;
				}

				public void TestSetSlot5(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(5,ea.MinRowChanged);
					Assertion.AssertEquals(5,ea.MaxRowChanged);
					Assertion.AssertEquals(false,ea.RowCountDecreased);
					_CallCount++;;
				
				}

				public void TestResetSlot5(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(5,ea.MinRowChanged);
					Assertion.AssertEquals(5,ea.MaxRowChanged);
					Assertion.AssertEquals(true,ea.RowCountDecreased);
					_CallCount++;;
				
				}


				public void TestSetSlot7AndSuspend(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(7,ea.MinRowChanged);
					Assertion.AssertEquals(7,ea.MaxRowChanged);
					Assertion.AssertEquals(false,ea.RowCountDecreased);
					_CallCount++;;
					((Altaxo.Main.ISuspendable)sender).Suspend();
				}


				public void CheckChange0To12_Decreased(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(0,ea.MinRowChanged);
					Assertion.AssertEquals(12,ea.MaxRowChanged);
					Assertion.AssertEquals(true,ea.RowCountDecreased);
					_CallCount++;;
					
				}




				public void ExpectingDataChange5To12_NoDecrease(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(5,ea.MinRowChanged);
					Assertion.AssertEquals(12,ea.MaxRowChanged);
					Assertion.AssertEquals(false,ea.RowCountDecreased);
					_CallCount++;;
				
				}

				public void ExpectingDataChange0To12_NoDecrease(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(0,ea.MinRowChanged);
					Assertion.AssertEquals(12,ea.MaxRowChanged);
					Assertion.AssertEquals(false,ea.RowCountDecreased);
					_CallCount++;;
					
				}

				public void ExpectingDataChange9To12_NoDecrease(object sender, EventArgs e)
				{
					Assertion.AssertNotNull("Sender must be the column",sender);
					Assertion.AssertEquals(typeof(Altaxo.Data.DoubleColumn),sender.GetType());
					Assertion.AssertNotNull("Awaiting valid data change event args",e);
					Assertion.AssertEquals(typeof(Altaxo.Data.DataColumn.ChangeEventArgs),e.GetType());
					Altaxo.Data.DataColumn.ChangeEventArgs ea = (Altaxo.Data.DataColumn.ChangeEventArgs)e;
					Assertion.AssertEquals(9,ea.MinRowChanged);
					Assertion.AssertEquals(12,ea.MaxRowChanged);
					Assertion.AssertEquals(false,ea.RowCountDecreased);
					_CallCount++;;
				
				}

			}



			[Test]
			public void ParentNotification()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				
				// testing parent change notification
				MyColumnParent parent = new MyColumnParent();
				parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
				d.ParentObject = parent;
				Assertion.AssertEquals("There was no parent add notification",1,parent.CallCount);
				parent.Reset();

				// testing parent change notification resetting parent
				parent.ChildChanged = new EventHandler(parent.TestParentRemoveNotification);
				d.ParentObject = null;
				Assertion.AssertEquals("There was no parent remove notification",1,parent.CallCount);
				parent.Reset();
			}

			[Test]
			public void DataChangeNotification()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				
				// testing parent change notification
				MyColumnParent parent = new MyColumnParent();
				parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
				d.ParentObject = parent;
				Assertion.AssertEquals("There was no parent add notification",1,parent.CallCount);
				parent.Reset();
			
			
				// test data change notification setting row5
				parent.ChildChanged = new EventHandler(parent.TestSetSlot5);
				d.Changed += new EventHandler(parent.TestSetSlot5);
				d[5]=66;
				Assertion.AssertEquals("There was no data change notification setting d[5]",2,parent.CallCount);
				d.Changed -= new EventHandler(parent.TestSetSlot5);
				parent.Reset();

				// testing data change notification resetting row5 -> count drops to zero
				parent.ChildChanged = new EventHandler(parent.TestResetSlot5);
				d[5]=double.NaN;
				Assertion.AssertEquals("There was no data change notification setting d[5]",1,parent.CallCount);
				parent.Reset();


				// Test d[7] setting to NaN -> the column must not rise a change event
				parent.ChildChanged = new EventHandler(parent.TestSetSlot7AndSuspend);
				d[7]=double.NaN;
				Assertion.AssertEquals("There is no data change notification expected setting d[7] to invalid",0,parent.CallCount);
				Assertion.AssertEquals(false,d.IsDirty);
				parent.Reset();

				// Set slot7 returning true -> the column must suspend
				parent.ChildChanged = new EventHandler(parent.TestSetSlot7AndSuspend);
				d.Changed += new EventHandler(parent.TestSetSlot7AndSuspend);
				d[7]=88;
				Assertion.AssertEquals("There was no data change notification setting d[7]",1,parent.CallCount);
				Assertion.AssertEquals(true,d.IsDirty);
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
				Assertion.AssertEquals("There was no data change notification setting d[0] to d[12]",2,parent.CallCount);
				Assertion.AssertEquals(false,d.IsDirty);
				d.Changed -= new EventHandler(parent.CheckChange0To12_Decreased);
				parent.Reset();
			}


			[Test]
			public void RowInsertionAtTheBeginning()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				
				// set rows 0 to 9 to i
				for(int i=0;i<10;i++)
					d[i]=i;
				Assertion.AssertEquals(10,d.Count);


				// testing parent change notification setting the parent
				MyColumnParent parent = new MyColumnParent();
				parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
				d.ParentObject = parent;
				Assertion.AssertEquals("There was no parent add notification",1,parent.CallCount);
				parent.Reset();

				parent.Reset();
				parent.ChildChanged = new EventHandler(parent.ExpectingDataChange0To12_NoDecrease);
				// now insert
				d.InsertRows(0,3);
				Assertion.AssertEquals("There was no data change notification",1,parent.CallCount);

				// test the data
				Assertion.AssertEquals(13,d.Count);

				for(int i=0;i<3;i++)
					Assertion.AssertEquals(double.NaN,d[i]);

				for(int i=3;i<(10+3);i++)
					Assertion.AssertEquals(i-3,d[i]);

				Assertion.AssertEquals(double.NaN,d[13]);
			}


			[Test]
			public void RowInsertionInTheMiddle()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				
				// set rows 0 to 9 to i
				for(int i=0;i<10;i++)
					d[i]=i;
					Assertion.AssertEquals(10,d.Count);

				// testing parent change notification setting the parent
				MyColumnParent parent = new MyColumnParent();
				parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
				d.ParentObject = parent;
				Assertion.AssertEquals("There was no parent add notification",1,parent.CallCount);
				parent.Reset();

				parent.Reset();
				parent.ChildChanged = new EventHandler(parent.ExpectingDataChange5To12_NoDecrease);
				// now insert
				d.InsertRows(5,3);
				Assertion.AssertEquals("There was no data change notification",1,parent.CallCount);

				// test the data
				Assertion.AssertEquals(13,d.Count);

				for(int i=0;i<5;i++)
					Assertion.AssertEquals(i,d[i]);

				Assertion.AssertEquals(double.NaN,d[5]);
				Assertion.AssertEquals(double.NaN,d[6]);
				Assertion.AssertEquals(double.NaN,d[7]);

				for(int i=8;i<(8+5);i++)
					Assertion.AssertEquals(i-3,d[i]);
			}

			[Test]
			public void RowInsertionOneBeforeEnd()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				
				// set rows 0 to 9 to i
				for(int i=0;i<10;i++)
					d[i]=i;
				Assertion.AssertEquals(10,d.Count);

				// testing parent change notification setting the parent
				MyColumnParent parent = new MyColumnParent();
				parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
				d.ParentObject = parent;
				Assertion.AssertEquals("There was no parent add notification",1,parent.CallCount);
				parent.Reset();

				parent.Reset();
				parent.ChildChanged = new EventHandler(parent.ExpectingDataChange9To12_NoDecrease);
				// now insert
				d.InsertRows(9,3);
				Assertion.AssertEquals("There was no data change notification",1,parent.CallCount);

				// test the data
				Assertion.AssertEquals(13,d.Count);

				for(int i=0;i<9;i++)
					Assertion.AssertEquals(i,d[i]);

				Assertion.AssertEquals(double.NaN,d[9]);
				Assertion.AssertEquals(double.NaN,d[10]);
				Assertion.AssertEquals(double.NaN,d[11]);

				for(int i=12;i<13;i++)
					Assertion.AssertEquals(i-3,d[i]);
			}

			[Test]
			public void RowInsertionAtTheEnd()
			{
				DoubleColumn d = new DoubleColumn(10);
				Assertion.AssertEquals(d.Count,0);
				Assertion.AssertEquals(d.IsDirty,false);
				
				// set rows 0 to 9 to i
				for(int i=0;i<10;i++)
					d[i]=i;
				Assertion.AssertEquals(10,d.Count);

				// testing parent change notification setting the parent
				MyColumnParent parent = new MyColumnParent();
				parent.ChildChanged = new EventHandler(parent.TestParentAddNotification);
				d.ParentObject = parent;
				Assertion.AssertEquals("There was no parent add notification",1,parent.CallCount);
				parent.Reset();

				parent.Reset();
				parent.ChildChanged = new EventHandler(parent.ExpectingNotToBeCalledBecauseNoChange);
				// now insert
				d.InsertRows(10,3);
		
				// test the data
				Assertion.AssertEquals(10,d.Count);

				for(int i=0;i<10;i++)
					Assertion.AssertEquals(i,d[i]);

				Assertion.AssertEquals(double.NaN,d[10]);
			}

		}
}
