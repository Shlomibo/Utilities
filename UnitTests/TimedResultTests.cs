using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UnitTests
{
	[TestClass]
	public class TimedResultTests
	{
		[TestMethod]
		public void TimedResultTest()
		{
			var obj = new object();
			var result = new TimedResult<object>(obj);

			Assert.AreSame(obj, result.Result,
				"result object is different that provided object");
			Assert.IsFalse(result.DidTimedOut,
				"result timed out, although object was provided");

			result = new TimedResult<object>();

			Assert.IsNull(result.Result,
				"timed out result returned an object");
			Assert.IsTrue(result.DidTimedOut,
				"timed out result isn't timed out");
		}
	}
}
