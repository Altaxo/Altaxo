using System;
using System.Collections;
using NUnit.Framework;
using Altaxo.Calc.LinearAlgebra;

namespace AltaxoTest.Calc.LinearAlgebra {
	[TestFixture]
	public class DoubleVectorEnumeratorTest{
		private const double TOLERENCE = 0.001;

		//Test Current Method
		[Test]
		public void Current(){
			DoubleVector test = new DoubleVector(new double[2]{1,2});
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
			DoubleVector test = new DoubleVector(new double[2]{1,2});
			foreach (double f in test) 
				Assert.IsTrue(test.Contains(f));
		}
		
		//Test Current Exception with index=-1.
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentException() {
			DoubleVector test = new DoubleVector(new double[2]{1,2});
			IEnumerator enumerator = test.GetEnumerator();
			object value=enumerator.Current;
		}
		
		//Test Current Exception with index>length
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CurrentException2() {
			DoubleVector test = new DoubleVector(new double[2]{1,2});
			IEnumerator enumerator = test.GetEnumerator();
			enumerator.MoveNext();
			enumerator.MoveNext();
			enumerator.MoveNext();
			object value=enumerator.Current;
		}
		
	}
}
