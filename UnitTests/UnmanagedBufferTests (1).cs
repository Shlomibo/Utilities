using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace UnitTests
{
	[TestClass]
	public unsafe class UnmanagedBufferTests
	{
		#region Consts

		private const int BUFFER_SIZE = 1024; 
		#endregion

		private UnmanagedBuffer GetBuffer(int bufferSize, bool isSizeKnown)
		{
			if (isSizeKnown)
			{
				return new UnmanagedBuffer(bufferSize);
			}
			else
			{
				IntPtr ptr = Marshal.AllocHGlobal(bufferSize);
				return new UnmanagedBuffer(ptr);
			}
		}

		#region Reallocate

		[TestMethod]
		public void ReallocateTest()
		{
			using (UnmanagedBuffer buffer = GetBuffer(BUFFER_SIZE, true))
			{
				buffer.Reallocate(BUFFER_SIZE * 2);
				Assert.IsTrue(buffer.Size.HasValue,
					"Missing size for allocated buffer");
				Assert.AreEqual(BUFFER_SIZE * 2, buffer.Size.Value,
					"Incorrect size for reallocated buffer");
			}

			using (UnmanagedBuffer buffer = GetBuffer(BUFFER_SIZE, false))
			{
				buffer.Reallocate(BUFFER_SIZE * 2);
				Assert.IsTrue(buffer.Size.HasValue,
					"Missing size for allocated buffer (prev: unkown size)");
				Assert.AreEqual(BUFFER_SIZE * 2, buffer.Size.Value,
					"Incorrect size for reallocated buffer (prev: unkown size)");
			}
		} 
		#endregion

		#region Copy 

		[TestMethod]
		public void CopyToTest()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target);
			}

			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE * 2, true))
			{
				source.CopyTo(target);
			}

			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE * 2, true),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target);
			}

			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, false),
				target = GetBuffer(BUFFER_SIZE, false))
			{
				source.CopyTo(target, BUFFER_SIZE);
			}

			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE, false))
			{
				source.CopyTo(target, BUFFER_SIZE);
			}

			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, false),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target, BUFFER_SIZE);
			}

			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target, BUFFER_SIZE);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException),
			"Didn't throw on copy size mismatch")]
		public void CopyValidateSize1()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE * 2, true),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target, BUFFER_SIZE * 2);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException),
			"Didn't throw on copy size mismatch")]
		public void CopyValidateSize2()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE * 2, true))
			{
				source.CopyTo(target, BUFFER_SIZE * 2);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException),
			"Didn't throw on copy size mismatch")]
		public void CopyValidateSize3()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target, BUFFER_SIZE * 2);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), 
			"Didn't throw on copying without size")]
		public void CopyToValidation1()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, false),
				target = GetBuffer(BUFFER_SIZE, true))
			{
				source.CopyTo(target);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException),
			"Didn't throw on copying without size")]
		public void CopyToValidation2()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, true),
				target = GetBuffer(BUFFER_SIZE, false))
			{
				source.CopyTo(target);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException),
			"Didn't throw on copying without size")]
		public void CopyToValidation3()
		{
			using (UnmanagedBuffer source = GetBuffer(BUFFER_SIZE, false),
				target = GetBuffer(BUFFER_SIZE, false))
			{
				source.CopyTo(target);
			}
		}
		#endregion
	}
}
