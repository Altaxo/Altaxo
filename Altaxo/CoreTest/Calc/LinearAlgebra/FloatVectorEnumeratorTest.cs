using System;
using System.Collections;
using NUnit.Framework;
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra {
	[TestFixture]
	public class FloatVectorEnumeratorTest{
		private const double TOLERENCE = 0.001;

		//Test Current Method
		[Test]
		public void Current(){
			FloatVector test = new FloatVector(new float[2]{1f,2f});
			IEnumerator enumerator = test.GetEnumerator();
			bool movenextresult;
			
			movenextresult=enumerator.MoveNext();
			Assert.IsTrue(movenextresult);
			Assert.AreEqual(enumerator.Current,test[0]);
			
			movenextresult=enumerator.MoveNext();
			Assert.IsTrue(movenextresult);
			Assert.AreEqual(enumerator.Current,test[1]);
			
			movenextresult=enumerator.MoveNext();
			Assert.IsFalse(movenextresult);
		}
		
		//Test foreach
		[Test]
		public void ForEach(){
			FloatVector test = new FloatVector(new float[2]{1f,2f});
			foreach (float f in test) 
				Assert.IsTrue(test.Contains(f));
		}
		
		//Test Current Exception with index=-1.
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentException() {
			FloatVector test = new FloatVector(new float[2]{1f,2f});
			IEnumerator enumerator = test.GetEnumerator();
			object value=enumerator.Current;
		}
		
		//Test Current Exception with index>length
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentException2() {
			FloatVector test = new FloatVector(new float[2]{1f,2f});
			IEnumerator enumerator = test.GetEnumerator();
			enumerator.MoveNext();
			enumerator.MoveNext();
			enumerator.MoveNext();
			object value=enumerator.Current;
		}
		
	}
}
