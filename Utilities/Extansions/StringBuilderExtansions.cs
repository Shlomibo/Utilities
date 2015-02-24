﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Extansions
{
	/// <summary>
	/// Extends StringBuilder.
	/// </summary>
	public static class StringBuilderExtantions
	{
		/// <summary>
		/// Creates new, identical StringBuilder object.
		/// </summary>
		/// <param name="builder">The StringBuilder to clone.</param>
		/// <returns>A new, logicaly identical, StringBuilder object.</returns>
		public static StringBuilder Clone(this StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			var newBuilder = new StringBuilder(builder.Capacity);

			for (int charIndex = 0; charIndex < builder.Length; charIndex++)
			{
				newBuilder.Append(builder[charIndex]);
			}

			return newBuilder;
		}

		/// <summary>
		/// Returns an enumerable wrapper for the StringBuilder, which enables to iterates the characters 
		/// in the StringBuilder with foreach loop.
		/// </summary>
		/// <param name="builder">The StringBuilder object to wrap.</param>
		/// <returns>An IEnumerable&lt;char&gt; object to iterate through the StringBuilder.</returns>
		public static IEnumerable<char> AsEnumerable(this StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
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

		/// <summary>
		/// Returns an IList&lt;char&gt; wrapper for the StringBuilder.
		/// </summary>
		/// <param name="builder">The StringBuilder object to wrap.</param>
		/// <returns>A wrapper for the StringBuilder that implements IList&lt;char&gt;.</returns>
		public static IList<char> AsList(this StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			return new StringBuilderWrapper(builder);
		}

		/// <summary>
		/// Checks if the character is contained in the StringBuilder.
		/// </summary>
		/// <param name="builder">The StringBuilder object, to search the char in it.</param>
		/// <param name="value">The character to search.</param>
		/// <returns>true if the character is contained by the StringBuildler; otherwise false.</returns>
		public static bool Contains(this StringBuilder builder, char value)
		{
			builder.ThrowWhen(
				when: obj => obj == null,
				what: new ArgumentNullException(nameof(builder)));

			return builder.AsEnumerable().Contains(value);
		}

		/// <summary>
		/// Checks if the list of characters is contained in the StringBuilder, with order.
		/// </summary>
		/// <param name="builder">The StringBuilder object, to search the chars list in it.</param>
		/// <param name="value">The list of character to search.</param>
		/// <returns>
		/// true if the list of character, is contained, with order, in the StringBuidler;
		/// otherwise false.
		/// </returns>
		public static bool Contains(this StringBuilder builder, IReadOnlyList<char> value)
		{
			bool result = false;

			builder.ThrowWhen(
				when: obj => obj == null,
				what: new ArgumentNullException(nameof(builder)));
			value.ThrowWhen(
				when: obj => obj == null,
				what: new ArgumentNullException(nameof(value)));

			if (value.Count == 0)
			{
				result = true;
			}
			else if (builder.Length >= value.Count)
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

		/// <summary>
		/// Checks if the string is contained in the StringBuilder.
		/// </summary>
		/// <param name="builder">The StringBuilder object, to search the string in it.</param>
		/// <param name="value">The string to search.</param>
		/// <returns>
		/// true if the string, is contained in the StringBuidler;
		/// otherwise false.
		/// </returns>
		public static bool Contains(this StringBuilder builder, string value) =>
			builder.Contains(value.AsList());

		/// <summary>
		/// Checks if the other StringBuilder is contained in the StringBuilder.
		/// </summary>
		/// <param name="builder">The StringBuilder object, to search the other StringBuilder in it.</param>
		/// <param name="value">The other StringBuilder to search.</param>
		/// <returns>
		/// true if the other StringBuilder, is contained in the StringBuidler;
		/// otherwise false.
		/// </returns>
		public static bool Contains(this StringBuilder builder, StringBuilder value) =>
			builder.Contains(value.AsList().AsReadOnly());

		private class StringBuilderWrapper : IList<char>, IReadOnlyList<char>
		{
			private const int NOT_FOUND = -1;
			#region Fields

			private readonly StringBuilder builder;
			#endregion

			#region Properties

			public char this[int index]
			{
				get { return this.builder[index]; }
				set { this.builder[index] = value; }
			}

			public int Count => this.builder.Length; 

			public bool IsReadOnly => false; 
			#endregion

			#region Ctor

			public StringBuilderWrapper(StringBuilder builder)
			{
				if (builder == null)
				{
					throw new ArgumentNullException(nameof(builder));
				}

				this.builder = builder;
			}
			#endregion

			#region Methods

			public int IndexOf(char item)
			{
				int result = NOT_FOUND;

				for (int i = 0; (result == NOT_FOUND) && (i < this.builder.Length); i++)
				{
					if (this.builder[i] == item)
					{
						result = i;
					}
				}

				return result;
			}

			public void Insert(int index, char item) =>
				this.builder.Insert(index, item);

			public void RemoveAt(int index) =>
				this.builder.Remove(index, 1);

			public void Add(char item) =>
				this.builder.Append(item);

			public void Clear() =>
				this.builder.Clear();

			public bool Contains(char item) =>
				this.builder.Contains(item);

			public void CopyTo(char[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if ((arrayIndex < 0) || (arrayIndex >= array.Length))
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex));
				}

				if ((arrayIndex + this.builder.Length) >= array.Length)
				{
					throw new ArgumentException();
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

				if (index != NOT_FOUND)
				{
					this.builder.Remove(index, 1);
					result = true;
				}

				return result;
			}

			public IEnumerator<char> GetEnumerator() =>
				this.builder.AsEnumerable().GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();
			#endregion
		}
	}
}
