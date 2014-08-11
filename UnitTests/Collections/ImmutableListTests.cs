using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
	[TestClass]
	public class ImmutableListTests
	{
		/*
IsEmpty:
 i: false

Element:
 i: retrieve element

Contains:
 i: has element ? true : false

CopyTo:
 i: param check -> copy elements

IndexOf:
 i: exists ? index : -1

Inserts:
 i: param check -> updated list
	- to the end
	- to the start
	- to the middle

RemoveAt:
 i: param check -> updated list
	- from the end
	- from the start
	- from the middle

SetItem:
 i: param check -> updated list
	- at the end
	- at the start
	- at the middle

Add:
 i: new list with the item

Remove: 
 i: list's tail

Remove(T):
 i: has item -> new list without item
    doesn't have -> same list

Remove(T, out bool):
 i: has item -> new list without item, true
    doesn't have -> same list, false*/
		[TestMethod]
		public void TestMethod1()
		{
		}
	}
}
