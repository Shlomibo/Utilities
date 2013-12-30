using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;
using System.Linq;

namespace UnitTests
{
	[TestClass]
	public class CLArgumentsTest
	{
		#region Fields

		private static readonly string[] ARG_LIST = new string[]
			{
				"not", 
				"keyed", 
				"values",
				"-dup key",
				"first dup",
				"second dup",
				"-first key",
				"-second key",
				"first value", 
				"second value",
				"-third key",
				"value",
				"-dup key",
				"third dup",
				"forth dup",
			};
		#endregion

		#region Properties

		public CLArguments Template { get; set; }
		#endregion

		[TestInitialize]
		public void InitTests()
		{
			this.Template = new CLArguments(ARG_LIST);
		}

		[TestMethod]
		public void AddTest()
		{
			const string TO_LAST_KEY = "value_to_the_last_key";
			const string ADDED_LONE_KEY = "-added_lone_key";
			const string ADDED_TO_EXISTING_VALUE = "added_to_existing_value";
			const string ADDED_WITH_VALUE_KEY = "-added_with_value_key";
			const string ADDED_TO_MISSING_KEY = "added_to_missing_key";

			CLArguments addList = this.Template.Clone();
			string lastKey = CLArguments.NO_KEY;

			for (int i = addList.Count - 1; i >= 0; i--)
			{
				if (addList.IsKey(addList[i]))
				{
					lastKey = addList[i];
					break;
				}
			}

			addList.Add(TO_LAST_KEY);

			Assert.AreEqual(TO_LAST_KEY, addList[addList.Count - 1],
				"Last item in list wasn't the added value");

			string[] lastValues = addList[lastKey];
			Assert.AreEqual(TO_LAST_KEY, lastValues[lastValues.Length - 1],
				"Last item in last key's values list wasn't teh added value");

			addList.Add(ADDED_LONE_KEY);
			Assert.AreEqual(ADDED_LONE_KEY, addList[addList.Count - 1],
				"Last item isn't the added key");
			Assert.IsTrue(addList.IsKey(addList[addList.Count - 1]),
				"Last key isn't considered as a key");

			addList.Add(ADDED_LONE_KEY, ADDED_TO_EXISTING_VALUE);
			Assert.IsTrue(addList[ADDED_LONE_KEY].Any(),
				"Existing key doesn't contain values");
			Assert.AreEqual(ADDED_TO_EXISTING_VALUE,
				addList[ADDED_LONE_KEY][addList[ADDED_LONE_KEY].Length - 1],
				"Last value of existing key is invalid");

			addList.Add(ADDED_WITH_VALUE_KEY, ADDED_TO_MISSING_KEY);
			Assert.IsTrue(addList.ContainsKey(ADDED_WITH_VALUE_KEY),
				"New key doesn't exist");
			Assert.IsTrue(addList[ADDED_WITH_VALUE_KEY].Length == 1,
				"New key have incorrent number of values");
			Assert.AreEqual(ADDED_TO_MISSING_KEY, addList[ADDED_WITH_VALUE_KEY][0],
				"New key doesn't contains the correct value");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Failed to throw on invalid key")]
		public void AddValidateKey()
		{
			const string INVALID_KEY = "invalid_key";
			const string VALID_VALUE = "valid_value";
			
			CLArguments addList = this.Template.Clone();

			addList.Add(INVALID_KEY, VALID_VALUE);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Failed to throw on invalid value")]
		public void AddValidateValue()
		{
			const string VALID_KEY = "-valid_key";
			const string INVALID_VALUE = "-invalud_value";

			CLArguments addList = this.Template.Clone();

			addList.Add(VALID_KEY, INVALID_VALUE);
		}
	}
}
