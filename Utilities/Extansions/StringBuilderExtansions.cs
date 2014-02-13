using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extansions.Enumerable;
using Utilities.Extansions.Enum;

namespace Utilities.Extansions.Text
{
	public static class StringBuilderExtantions
	{
		public static StringBuilder Clone(this StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException();
			}

			var newBuilder = new StringBuilder(builder.Capacity);

			for (int charIndex = 0; charIndex < builder.Length; charIndex++)
			{
				newBuilder.Append(builder[charIndex]);
			}

			return newBuilder;
		}

		public static IEnumerable<char> AsEnumerable(this StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException();
			}

			return AsEnumerableInternal(builder);
		}

		private static IEnumerable<char> AsEnumerableInternal(StringBuilder builder)
		{
			for (int charIndex = 0; charIndex < builder.Length; charIndex++)
			{
				yield return builder[charIndex];
			}
		}

		public static IList<char> AsList(this StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException();
			}

			return new StringBuilderWrapper(builder);
		}

		public static bool Contains(this StringBuilder builder, char value)
		{
			if (builder == null)
			{
				throw new ArgumentNullException();
			}

			bool result = false;

			foreach (char @char in builder.AsEnumerable())
			{
				if (@char == value)
				{
					result = true;
					break;
				}
			}

			return result;
		}

		public static bool Contains(this StringBuilder builder, IReadOnlyList<char> value)
		{
			bool result = false;

			if (builder == null)
			{
				throw new ArgumentNullException();
			}

			if (builder.Length >= value.Count)
			{
				for (int builderIndex = 0; builderIndex < builder.Length; builderIndex++)
				{
					int strIndex = 0;

					for (; strIndex < value.Count; strIndex++)
					{
						if (builder[builderIndex + strIndex] != value[strIndex])
						{
							break;
						}
					}

					if (strIndex == value.Count)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		public static bool Contains(this StringBuilder builder, string value)
		{
			return builder.Contains(value.AsList());
		}

		public static bool Contains(this StringBuilder builder, StringBuilder value)
		{
			return builder.Contains(value.AsList().AsReadOnly());
		}
		
		private class StringBuilderWrapper : IList<char>, IReadOnlyList<char>
		{
			#region Fields

			private StringBuilder builder; 
			#endregion

			#region Properties

			public char this[int index]
			{
				get { return this.builder[index]; }
				set { this.builder[index] = value; }
			}

			public int Count
			{
				get { return this.builder.Length; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}
			#endregion

			#region Ctor

			public StringBuilderWrapper(StringBuilder builder)
			{
				if (builder == null)
				{
					throw new ArgumentNullException();
				}

				this.builder = builder;
			} 
			#endregion

			#region Methods

			public int IndexOf(char item)
			{
				int result = -1;

				for (int i = 0; i < this.builder.Length; i++)
				{
					if (this.builder[i] == item)
					{
						result = i;
						break;
					}
				}

				return result;
			}

			public void Insert(int index, char item)
			{
				this.builder.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				this.builder.Remove(index, 1);
			}

			public void Add(char item)
			{
				this.builder.Append(item);
			}

			public void Clear()
			{
				this.builder.Clear();
			}

			public bool Contains(char item)
			{
				return this.builder.Contains(item);
			}

			public void CopyTo(char[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException();
				}

				if ((arrayIndex + this.builder.Length) >= array.Length)
				{
					throw new IndexOutOfRangeException();
				}

				for (int i = 0; i < this.builder.Length; i++)
				{
					array[arrayIndex + i] = this.builder[i];
				}
			}

			public bool Remove(char item)
			{
				int index = IndexOf(item);
				bool result = false;

				if (index != -1)
				{
					this.builder.Remove(index, 1);
					result = true;
				}

				return result;
			}

			public IEnumerator<char> GetEnumerator()
			{
				return this.builder.AsEnumerable().GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			#endregion
		}
	}
}
