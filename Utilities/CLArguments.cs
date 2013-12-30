using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extansions;

namespace Utilities
{
	/// <summary>
	/// Helper class to siplify command line arguments parsing.
	/// </summary>
	public class CLArguments : IList<string>, IDictionary<string, string[]>, ICloneable
	{
		#region Consts

		/// <summary>The key for unkeyed values</summary>
		public const string NO_KEY = " ";
		/// <summary>A collection of the default characters that designate args which start with it as keys</summary>
		public static readonly ReadOnlyCollection<string> DefaultDesignators =
			Array.AsReadOnly(new string[] { "-", "/" });
		#endregion

		#region Fields

		private string[] keyDesignators = DefaultDesignators.ToArray();
		private Dictionary<string, string[]> asDict;
		private Predicate<string> isKeyPred;
		#endregion

		#region Properties

		private int LastKeyIndex
		{
			get
			{
				string lastKey = (from value in this.AsList
								  where IsKey(value)
								  select value).LastOrDefault();

				return lastKey != null
					? this.AsList.LastIndexOf(lastKey)
					: -1;
			}
		}

		/// <summary>
		/// Gets or sets array of strings whic when an arg is started with, it considered a key.
		/// </summary>
		public string[] KeyDesignators
		{
			get { return keyDesignators; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}

				keyDesignators = value;
			}
		}

		private List<string> AsList { get; set; }

		private Dictionary<string, string[]> AsDict
		{
			get
			{
				if (this.asDict == null)
				{
					this.asDict = BuildDictionary();
				}

				return this.asDict;
			}
			set { this.asDict = value; }
		}

		/// <summary>
		/// Gets or sets the arg in the specified index.
		/// </summary>
		/// <param name="index">The index of the arg.</param>
		/// <returns>The arg which was stored in the specified index.</returns>
		public string this[int index]
		{
			get { return this.AsList[index]; }
			set
			{
				this.AsList[index] = value;
				this.AsDict = null;
			}
		}

		/// <summary>
		/// Gets the count of item stored in the object.
		/// </summary>
		public int Count
		{
			get { return this.AsList.Count; }
		}

		bool ICollection<string>.IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a collection of keys args
		/// </summary>
		public ICollection<string> Keys
		{
			get { return this.AsDict.Keys; }
		}

		/// <summary>
		/// Gets a collection of values args
		/// </summary>
		public ICollection<string[]> Values
		{
			get { return this.AsDict.Values; }
		}
		
		/// <summary>
		/// Gets or sets the values that stored under the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string[] this[string key]
		{
			get { return this.AsDict[key]; }
			set
			{
				this.Add(key, value);
			}
		}

		bool ICollection<KeyValuePair<string, string[]>>.IsReadOnly
		{
			get { return (this as ICollection<string>).IsReadOnly; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of command line arguments collection.
		/// </summary>
		public CLArguments()
			: this(new string[0], DefaultDesignators)
		{ }

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with a predicate to determine which args are keys.
		/// </summary>
		/// <param name="isKeyPred">A predicate the returns true if the arg is a key.</param>
		public CLArguments(Predicate<string> isKeyPred)
			: this(new string[0], isKeyPred)
		{ }

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with the args in the specified args list.
		/// </summary>
		/// <param name="list">The list of arguments to initialize the object with.</param>
		public CLArguments(IEnumerable<string> list)
			: this(list, DefaultDesignators)
		{ }

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with the args in the specified args list, and a key designators list.
		/// </summary>
		/// <param name="list">The list of arguments to initialize the object with.</param>
		/// <param name="keyDesignators">
		/// Sequence of strings which when arg is started with one of them, it considered as key.
		/// </param>
		public CLArguments(IEnumerable<string> list, IEnumerable<string> keyDesignators)
			: this(list, key => IsKeyPredicate(key, keyDesignators))
		{ }

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with the args in the specified args list, and with a predicate to determine which args are keys.
		/// </summary>
		/// <param name="list">The list of arguments to initialize the object with.</param>
		/// <param name="isKeyPred">A predicate the returns true if the arg is a key.</param>
		public CLArguments(IEnumerable<string> list, Predicate<string> isKeyPred)
		{
			this.AsList = new List<string>(list);
			this.isKeyPred = isKeyPred;
		}

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with the args in the specified args dictionary.
		/// </summary>
		/// <param name="keys">The dictionary of arguments</param>
		public CLArguments(IDictionary<string, string[]> keys)
			: this(keys, DefaultDesignators)
		{ }

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with the args in the specified args dictionary, and a key designators list.
		/// </summary>
		/// <param name="keys">The dictionary of arguments to initialize the object with.</param>
		/// <param name="keyDesignators">
		/// Sequence of strings which when arg is started with one of them, it considered as key.
		/// </param>
		public CLArguments(IDictionary<string,string[]> keys, IEnumerable<string> keyDesignators)
			: this(keys, key => IsKeyPredicate(key, keyDesignators))
		{ }

		/// <summary>
		/// Creates new instance of command line arguments collection, 
		/// with the args in the specified args dictionary, with a predicate to determine which args are keys.
		/// </summary>
		/// <param name="keys">The dictionary of arguments to initialize the object with.</param>
		/// <param name="isKeyPred">A predicate the returns true if the arg is a key.</param>
		public CLArguments(IDictionary<string,string[]> keys, Predicate<string> isKeyPred)
		{
			this.isKeyPred = isKeyPred;

			this.asDict = new Dictionary<string, string[]>(keys);
			this.AsList = new List<string>(keys.Count * 2);

			if (keys.ContainsKey(NO_KEY))
			{
				this.AsList.AddRange(keys[NO_KEY]);
			}

			foreach (string key in keys.Keys)
			{
				if (key != NO_KEY)
				{
					this.AsList.Add(key);
					this.AsList.AddRange(keys[key]);
				}
			}
		}
		#endregion

		#region Methods

		/// <summary>
		/// The default key predicate.
		/// </summary>
		/// <param name="arg">The argument to check if it is a key.</param>
		/// <param name="designators">The key designators sequence.</param>
		/// <returns>
		/// true if arg starts with one of the strings in designators; otherwise false.
		/// </returns>
		public static bool IsKeyPredicate(string arg, IEnumerable<string> designators)
		{
			return designators.Any(des => arg.StartsWith(des, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Checks if the given arg is key.
		/// </summary>
		/// <param name="arg">The arg to check.</param>
		/// <returns>true if arg is key; otherwise false.</returns>
		public bool IsKey(string arg)
		{
			return this.isKeyPred(arg);
		}

		/// <summary>
		/// Returns the index of the given argument.
		/// </summary>
		/// <param name="arg">The argument to search.</param>
		/// <returns>
		/// The index of the arg in the argument list. 
		/// If the arg does not exist in the list, -1 is returned.
		/// </returns>
		public int IndexOf(string arg)
		{
			return this.AsList.IndexOf(arg);
		}

		/// <summary>
		/// Returns the index of the given argument.
		/// </summary>
		/// <param name="arg">The argument to search.</param>
		/// <param name="startIndex">The index to start the search from.</param>
		/// <returns>
		/// The index of the arg in the argument list. 
		/// If the arg does not exist in the list after the given index, -1 is returned.
		/// </returns>
		public int IndexOf(string arg, int startIndex)
		{
			return this.AsList.IndexOf(arg, startIndex);
		}

		/// <summary>
		/// Returns the index of the given argument.
		/// </summary>
		/// <param name="arg">The argument to search.</param>
		/// <param name="startIndex">The index to start the search from.</param>
		/// <param name="count">The number of element to check.</param>
		/// <returns>
		/// The index of the arg in the argument list. 
		/// If the arg does not exist in the list after the given index, -1 is returned.
		/// </returns>
		public int IndexOf(string arg, int startIndex, int count)
		{
			return this.AsList.IndexOf(arg, startIndex, count);
		}

		/// <summary>
		/// Inserts the given arg in the given index.
		/// </summary>
		/// <param name="index">The index which the arg will be inserted into.</param>
		/// <param name="arg">The arg to insert.</param>
		public void Insert(int index, string arg)
		{
			this.AsList.Insert(index, arg);
			this.AsDict = null;
		}

		private Dictionary<string, string[]> BuildDictionary()
		{
			var dict = new Dictionary<string, string[]>(this.AsList.Count);
			var duplicates = new Dictionary<string, int>(this.AsList.Count);
			string key;
			int keyIndex;
			IEnumerable<string> value;

			List<int> keyIndices = (from item in this.AsList
									where this.IsKey(item)
									select this.AsList.IndexOf(item)).ToList();

			if (!keyIndices.Any())
			{
				if (this.AsList.Any())
				{
					dict.Add(NO_KEY, this.AsList.ToArray());
				}
			}
			else
			{
				if (keyIndices.First() > 0)
				{
					dict.Add(NO_KEY, this.AsList.Take(keyIndices.First()).ToArray());
				}

				for (int index = 0; index < keyIndices.Count - 1; index++)
				{
					keyIndex = keyIndices[index];
					int nextKeyIndex = keyIndices[index + 1];
					key = this.AsList[keyIndex];

					if (duplicates.ContainsKey(key))
					{
						keyIndex = this.AsList.IndexOf(key, duplicates[key] + 1);
					}

					if (duplicates.ContainsKey(this.AsList[nextKeyIndex]))
					{
						nextKeyIndex = this.AsList.IndexOf(
							this.AsList[nextKeyIndex],
							duplicates[this.AsList[nextKeyIndex]] + 1);
					}

					duplicates.SetKey(key, keyIndex);

					value = this.AsList
						.Skip(keyIndex + 1)
						.Take(nextKeyIndex - keyIndex - 1);

					value = dict.ContainsKey(key)
						? dict[key].Concat(value)
						: value;

					dict.SetKey(key, value.ToArray());
				}

				keyIndex = keyIndices.Last();
				key = this.AsList[keyIndex];

				if (duplicates.ContainsKey(key))
				{
					keyIndex = this.AsList.LastIndexOf(key);
				}

				value = this.AsList.Skip(keyIndex + 1);
				value = dict.ContainsKey(key)
					? dict[key].Concat(value)
					: value;

				dict.SetKey(key, value.ToArray());
			}

			return dict;
		}

		/// <summary>
		/// Removes the arg at the specified index.
		/// </summary>
		/// <param name="index">The index to remove the arg from.</param>
		public void RemoveAt(int index)
		{
			this.AsList.RemoveAt(index);
			this.AsDict = null;
		}

		/// <summary>
		/// Add arg to the end of the list.
		/// </summary>
		/// <param name="arg">The arg to add.</param>
		public void Add(string arg)
		{
			this.AsList.Add(arg);

			if (this.asDict != null)
			{
				if (IsKey(arg))
				{
					if (!this.AsDict.ContainsKey(arg))
					{
						this.AsDict.Add(arg, new string[0]);
					}
				}
				else
				{
					string key = this.AsList[this.LastKeyIndex];
					string[] value = this.AsDict[key];

					Array.Resize(ref value, value.Length + 1);
					value[value.Length - 1] = arg;

					this.AsDict[key] = value;
				}
			}
		}

		/// <summary>
		/// Clears all args from the list.
		/// </summary>
		public void Clear()
		{
			this.AsList.Clear();
			this.asDict = null;
		}

		/// <summary>
		/// Check if the arg is contained in the list.
		/// </summary>
		/// <param name="arg">The arg to check.</param>
		/// <returns>true if arg exists in the list; otherwise false.</returns>
		public bool Contains(string arg)
		{
			return this.AsList.Contains(arg);
		}

		/// <summary>
		/// Copies the args in the list to the specified array.
		/// </summary>
		/// <param name="array">The array which args in the list would be copied to.</param>
		/// <param name="arrayIndex">The first index in the array to copy to.</param>
		public void CopyTo(string[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (array.Length < this.Count + arrayIndex)
			{
				throw new IndexOutOfRangeException();
			}

			for (int i = 0; i < this.Count; i++)
			{
				array[arrayIndex + i] = this[i];
			}
		}

		/// <summary>
		/// Removes the first occurence of the arg from the list.
		/// </summary>
		/// <param name="arg">The arg to remove.</param>
		/// <returns>true if arg has been removed; otherwise false.</returns>
		public bool Remove(string arg)
		{
			bool isRemoved = this.AsList.Remove(arg);

			if (isRemoved)
			{
				this.asDict = null;
			}

			return isRemoved;
		}

		/// <summary>
		/// Returns enumerator to enumerate the args in the list.
		/// </summary>
		/// <returns>Enumerator for the args in the list.</returns>
		public IEnumerator<string> GetEnumerator()
		{
			return this.AsList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds the given value args under the given key arg.
		/// </summary>
		/// <param name="key">The key arg.</param>
		/// <param name="value">The value args array.</param>
		public void Add(string key, string[] value)
		{
			if (!IsKey(key))
			{
				throw new InvalidOperationException("The key argument must be a key");
			}

			if (value.Any(item => IsKey(item)))
			{
				throw new InvalidOperationException("value list must contain only values");
			}

			int keyIndex = this.AsList.IndexOf(key);
			value = value ?? new string[0];

			if (keyIndex == -1)
			{
				this.AsList.Add(key);
				this.AsList.AddRange(value);

				if (this.asDict != null)
				{
					this.AsDict.Add(key, value);
				}
			}
			else
			{
				int nextIndex;

				for (nextIndex = keyIndex + 1;
					(nextIndex < this.AsList.Count) && !IsKey(this.AsList[nextIndex]);
					nextIndex++) ;

				nextIndex--;

				this.AsList.InsertRange(nextIndex, value);

				if ((this.asDict != null) && value.Any())
				{
					this.AsDict[key] = this.AsDict[key].Concat(value).ToArray();
				}
			}
		}

		/// <summary>
		/// Adds the given value args under the given key arg.
		/// </summary>
		/// <param name="key">The key arg.</param>
		/// <param name="value">The value to be added to the args array.</param>
		public void Add(string key, string value)
		{
			Add(key, new string[] { value });
		}

		/// <summary>
		/// Check if the args list contains the specified key.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>true if the key exists; otherwise false.</returns>
		public bool ContainsKey(string key)
		{
			if (this.asDict != null)
			{
				return this.AsDict.ContainsKey(key);
			}
			else
			{
				return IsKey(key) && this.AsList.Contains(key);
			}
		}

		/// <summary>
		/// Returns the values under the specified key, if the key exists.
		/// </summary>
		/// <param name="key">The key for the values.</param>
		/// <param name="value">Out: the values that was stored under the specified key.</param>
		/// <returns>true if the key exists, and the values have returned; otherwise false.</returns>
		public bool TryGetValue(string key, out string[] value)
		{
			return this.AsDict.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<string, string[]>>.Add(KeyValuePair<string, string[]> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<string, string[]>>.Contains(KeyValuePair<string, string[]> item)
		{
			return this.ContainsKey(item.Key) &&
				(object.ReferenceEquals(item.Value, this.AsDict[item.Key]) ||
				this.AsDict[item.Key].Zip(
					item.Value,
					(first, second) => new { first, second })
				.All(zip => zip.first == zip.second));
		}

		void ICollection<KeyValuePair<string, string[]>>.CopyTo(
			KeyValuePair<string, string[]>[] array,
			int arrayIndex)
		{
			if (array.Length < this.AsDict.Count + arrayIndex)
			{
				throw new IndexOutOfRangeException();
			}

			int i = 0;

			foreach (string key in this.AsDict.Keys)
			{
				array[arrayIndex + i++] = new KeyValuePair<string, string[]>(key, this.AsDict[key]);
			}
		}

		bool ICollection<KeyValuePair<string, string[]>>.Remove(KeyValuePair<string, string[]> item)
		{
			bool didRemoved = (this as IDictionary<string, string[]>).Contains(item);

			if (didRemoved)
			{
				this.AsDict.Remove(item.Key);
			}

			return didRemoved;
		}

		IEnumerator<KeyValuePair<string, string[]>> IEnumerable<KeyValuePair<string, string[]>>.GetEnumerator()
		{
			return this.AsDict.GetEnumerator();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public CLArguments Clone()
		{
			var clone = new CLArguments()
			{
				isKeyPred = this.isKeyPred,
				AsList = new List<string>(this.AsList),
				asDict = this.asDict != null
					? new Dictionary<string, string[]>(this.asDict)
					: null,
			};

			return clone;
		}
		#endregion
	}
}
