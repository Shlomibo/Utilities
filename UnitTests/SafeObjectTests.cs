using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UnitTests
{
	[TestClass]
	public class SafeObjectTests
	{
		[TestMethod]
		[ExpectedException(typeof(OperationPerformedException), "Object didn't released")]
		public void DisposeTest()
		{
			var obj = new object();
			var so = new SafeObject<object>(obj, @object => { throw new OperationPerformedException(); });
			so.Dispose();
		}

		[TestMethod]
		[ExpectedException(typeof(OperationPerformedException), "Object didn't released")]
		public void UsingTest()
		{
			var obj = new object();
			using (var so = new SafeObject<object>(obj, @object => { throw new OperationPerformedException(); }))
			{
			}
		}

		[TestMethod]
		[ExpectedException(typeof(OperationPerformedException), "Event wasn't raised")]
		public void DisposedEventTest()
		{
			var obj = new object();
			using (var so = new SafeObject<object>(obj))
			{
				so.Disposed += (s, e) => { throw new OperationPerformedException(); };
			}
		}

		[TestMethod]
		public void ObjectTest()
		{
			var obj = new object();
			var so = new SafeObject<object>(obj);

			Assert.AreSame(obj, so.Object,
				"Safe object holds different object that initialized with");
		}

		[TestMethod]
		public void IsDisposedTest()
		{
			var obj = new object();
			var so = new SafeObject<object>(obj);

			Assert.IsFalse(so.IsDisposed, "Object is dispsed before disposing");
			so.Dispose();
			Assert.IsTrue(so.IsDisposed, "Object isn't disposed after disposing");
		}
	}
}
